using System.Collections.Generic;
using UnityEngine;

public class SUILayout : SUIProcess
{
	public struct NormalRange
	{
		public float min;

		public float max;

		public NormalRange(float _min, float _max)
		{
			min = _min;
			max = _max;
		}
	}

	public class ObjectData
	{
		public SUIProcess obj;

		public SUILayoutAnim.AnimVector2 animPosition;

		public SUILayoutAnim.AnimFloat animAlpha;

		public List<SUILayoutEffect.Effect> effects;

		public void Update()
		{
			obj.Update();
			if (effects == null)
			{
				return;
			}
			foreach (SUILayoutEffect.Effect effect in effects)
			{
				effect.Update();
			}
		}

		public void Destroy()
		{
			obj.Destroy();
			obj = null;
			effects = null;
			animPosition = null;
			animAlpha = null;
		}

		public void AddEffect(SUILayoutEffect.Effect e)
		{
			if (effects == null)
			{
				effects = new List<SUILayoutEffect.Effect>(1);
			}
			effects.Add(e);
		}
	}

	private class TransitionSoundTrigger
	{
		public float frameVal;

		public string soundID;

		public bool played;

		public TypedWeakReference<GameObject> objectToPlayFrom;
	}

	public OnSUIGenericCallback onTransitionOver;

	private Dictionary<string, ObjectData> mObjects = new Dictionary<string, ObjectData>();

	private SDFTreeNode mExtraData;

	private float mAnimProg = 1f;

	private float mAnimTarget = 1f;

	private float mAnimSpeed = 1f;

	private float mDefaultAnimSpeed = 1f;

	private float mBasePriority;

	private Vector2 mBasePosition = new Vector2(0f, 0f);

	private TransitionSoundTrigger mOnTransitInPlaySound;

	private TransitionSoundTrigger mOnTransitOutPlaySound;

	public bool isAnimating
	{
		get
		{
			return mAnimProg != mAnimTarget;
		}
	}

	public float frame
	{
		get
		{
			return mAnimProg;
		}
		set
		{
			mAnimProg = Mathf.Clamp(value, 0f, 1f);
			DrawAnimFrame(mAnimProg);
		}
	}

	public float defaultTransitionSpeed
	{
		get
		{
			return mDefaultAnimSpeed;
		}
		set
		{
			mDefaultAnimSpeed = value;
		}
	}

	public float basePriority
	{
		get
		{
			return mBasePriority;
		}
		set
		{
			float num = value - mBasePriority;
			mBasePriority = value;
			foreach (KeyValuePair<string, ObjectData> mObject in mObjects)
			{
				if (mObject.Value.obj is IHasVisualAttributes)
				{
					((IHasVisualAttributes)mObject.Value.obj).priority += num;
				}
			}
		}
	}

	public Vector2 basePosition
	{
		get
		{
			return mBasePosition;
		}
		set
		{
			Vector2 vector = new Vector2(value.x - mBasePosition.x, value.y - mBasePosition.y);
			mBasePosition = value;
			foreach (KeyValuePair<string, ObjectData> mObject in mObjects)
			{
				if (mObject.Value.obj is IHasVisualAttributes)
				{
					((IHasVisualAttributes)mObject.Value.obj).position += vector;
				}
			}
		}
	}

	public SUIProcess this[string name]
	{
		get
		{
			SUIProcess result = null;
			try
			{
				result = mObjects[name.ToLower()].obj;
			}
			catch
			{
				Debug.Log("ERROR: Trying to access non-existant object '" + name + "'");
			}
			return result;
		}
	}

	public Dictionary<string, ObjectData> objects
	{
		get
		{
			return mObjects;
		}
	}

	public SDFTreeNode data
	{
		get
		{
			if (mExtraData == null)
			{
				mExtraData = new SDFTreeNode();
			}
			return mExtraData;
		}
	}

	public SUILayout()
	{
	}

	public SUILayout(string layoutFile)
	{
		Load(layoutFile);
	}

	private static NormalRange StringToNormalRangeOptional(string str)
	{
		if (str != string.Empty)
		{
			return SUILayoutConv.GetNormalRange(str);
		}
		return new NormalRange(0f, 1f);
	}

	private TransitionSoundTrigger CreateSoundTrigger(string data, GameObject toPlayFrom)
	{
		string[] array = data.Split(',');
		if (array.Length != 2)
		{
			Debug.Log("WARNING: malformed layout sound trigger: " + data);
			return null;
		}
		TransitionSoundTrigger transitionSoundTrigger = new TransitionSoundTrigger();
		transitionSoundTrigger.frameVal = float.Parse(array[0].Trim());
		transitionSoundTrigger.soundID = array[1].Trim();
		transitionSoundTrigger.objectToPlayFrom = new TypedWeakReference<GameObject>(toPlayFrom);
		return transitionSoundTrigger;
	}

	public void Load(string layoutFile)
	{
		SDFTreeNode sDFTreeNode = SingletonMonoBehaviour<ResourcesManager>.instance.Open(layoutFile);
		if (sDFTreeNode == null)
		{
			return;
		}
		GameObject gameObject = null;
		foreach (KeyValuePair<string, SDFTreeNode> child in sDFTreeNode.childs)
		{
			if (child.Key == "_data")
			{
				mExtraData = child.Value;
				continue;
			}
			SUIProcess sUIProcess = SUILayoutCreator.Create(child.Value);
			if (sUIProcess != null)
			{
				mObjects[child.Key] = new ObjectData();
				mObjects[child.Key].obj = sUIProcess;
				if (child.Value.to("transition") != null)
				{
					ExtractAnimOutData(child.Value.to("transition"), mObjects[child.Key]);
				}
				if (child.Value.to("effect") != null)
				{
					ExtractEffectData(child.Value.to("effect"), mObjects[child.Key]);
				}
				if (gameObject == null && sUIProcess is SUIWidget)
				{
					gameObject = ((SUIWidget)sUIProcess).gameObject;
				}
			}
			else
			{
				Debug.Log("ERROR: Could not create widget '" + child.Key + "'.");
			}
		}
		if (sDFTreeNode.hasAttribute("transitionSpeed"))
		{
			mDefaultAnimSpeed = float.Parse(sDFTreeNode["transitionSpeed"]);
		}
		if (sDFTreeNode.hasAttribute("onTransitionInPlaySound"))
		{
			mOnTransitInPlaySound = CreateSoundTrigger(sDFTreeNode["onTransitionInPlaySound"], gameObject);
		}
		if (sDFTreeNode.hasAttribute("onTransitionOutPlaySound"))
		{
			mOnTransitOutPlaySound = CreateSoundTrigger(sDFTreeNode["onTransitionOutPlaySound"], gameObject);
		}
	}

	public void Add(string key, SUIProcess obj)
	{
		ObjectData objectData = new ObjectData();
		objectData.obj = obj;
		Add(key, objectData);
	}

	public void Add(string key, ObjectData od)
	{
		key = key.ToLower();
		if (mObjects.ContainsKey(key))
		{
			Debug.Log("ERROR: Trying to add an object with existing key name: " + key);
		}
		else
		{
			mObjects.Add(key, od);
		}
	}

	public void Remove(string key)
	{
		Remove(key, false);
	}

	public void Remove(string key, bool destroy)
	{
		if (destroy)
		{
			ObjectData objectData = mObjects[key];
			if (objectData != null)
			{
				objectData.Destroy();
			}
		}
		mObjects.Remove(key.ToLower());
	}

	public virtual void Update()
	{
		UpdateAnimation();
		if (mObjects == null)
		{
			return;
		}
		foreach (KeyValuePair<string, ObjectData> mObject in mObjects)
		{
			mObject.Value.Update();
		}
	}

	public void AnimateIn(float speed)
	{
		mAnimSpeed = speed;
		StartAnimateIn();
	}

	public void AnimateIn()
	{
		mAnimSpeed = mDefaultAnimSpeed;
		StartAnimateIn();
	}

	public void AnimateOut(float speed)
	{
		mAnimSpeed = speed;
		StartAnimateOut();
	}

	public void AnimateOut()
	{
		mAnimSpeed = mDefaultAnimSpeed;
		StartAnimateOut();
	}

	public void SetAllVisible(bool visible)
	{
		foreach (KeyValuePair<string, ObjectData> mObject in mObjects)
		{
			if (mObject.Value.obj is IHasVisualAttributes)
			{
				((IHasVisualAttributes)mObject.Value.obj).visible = visible;
			}
		}
	}

	public virtual void Destroy()
	{
		foreach (KeyValuePair<string, ObjectData> mObject in mObjects)
		{
			mObject.Value.Destroy();
		}
		mObjects = null;
	}

	public void Clear()
	{
		Destroy();
		mObjects = new Dictionary<string, ObjectData>();
	}

	public bool Exists(string name)
	{
		SUIProcess sUIProcess = null;
		try
		{
			sUIProcess = mObjects[name.ToLower()].obj;
		}
		catch
		{
		}
		return sUIProcess != null;
	}

	public void EditorRenderOnGUI()
	{
		foreach (KeyValuePair<string, ObjectData> mObject in mObjects)
		{
			mObject.Value.obj.EditorRenderOnGUI();
		}
	}

	private void ExtractAnimOutData(SDFTreeNode outData, ObjectData targetObj)
	{
		SDFTreeNode sDFTreeNode = outData.to("position");
		if (sDFTreeNode != null)
		{
			IHasVisualAttributes hasVisualAttributes = (IHasVisualAttributes)targetObj.obj;
			Vector2 @out;
			if (sDFTreeNode.hasAttribute("position"))
			{
				@out = SUILayoutConv.GetVector2(sDFTreeNode["position"]);
			}
			else
			{
				if (!sDFTreeNode.hasAttribute("offset"))
				{
					Debug.Log("ERROR: animated position block missing either a 'position' or 'offset'.");
					return;
				}
				@out = hasVisualAttributes.position + SUILayoutConv.GetVector2(sDFTreeNode["offset"]);
			}
			targetObj.animPosition = new SUILayoutAnim.AnimVector2(hasVisualAttributes.position, @out, StringToNormalRangeOptional(sDFTreeNode["animRange"]), SUILayoutConv.GetEaseFunc(sDFTreeNode["anim"]));
		}
		SDFTreeNode sDFTreeNode2 = outData.to("alpha");
		if (sDFTreeNode2 != null)
		{
			float out2 = float.Parse(sDFTreeNode2["alpha"]);
			IHasVisualAttributes hasVisualAttributes2 = (IHasVisualAttributes)targetObj.obj;
			targetObj.animAlpha = new SUILayoutAnim.AnimFloat(hasVisualAttributes2.alpha, out2, StringToNormalRangeOptional(sDFTreeNode2["animRange"]), SUILayoutConv.GetEaseFunc(sDFTreeNode2["anim"]));
		}
	}

	private void ExtractEffectData(SDFTreeNode effectsData, ObjectData targetObj)
	{
		if (!(targetObj.obj is IHasVisualAttributes))
		{
			return;
		}
		foreach (KeyValuePair<string, SDFTreeNode> child in effectsData.childs)
		{
			SUILayoutEffect.Effect effect = SUILayoutEffect.CreateEffect((IHasVisualAttributes)targetObj.obj, child.Key, child.Value);
			if (effect != null && targetObj.effects == null)
			{
				targetObj.effects = new List<SUILayoutEffect.Effect>();
				targetObj.effects.Add(effect);
			}
		}
	}

	private void UpdateAnimation()
	{
		if (mAnimProg == mAnimTarget)
		{
			if (onTransitionOver != null)
			{
				onTransitionOver();
				onTransitionOver = null;
			}
			return;
		}
		if (mAnimProg < mAnimTarget)
		{
			mAnimProg = Mathf.Min(mAnimTarget, mAnimProg + SUIScreen.deltaTime / mAnimSpeed);
			if (mOnTransitInPlaySound != null && !mOnTransitInPlaySound.played && mAnimProg >= mOnTransitInPlaySound.frameVal)
			{
				mOnTransitInPlaySound.played = true;
				Singleton<SUISoundManager>.instance.Play(mOnTransitInPlaySound.soundID, mOnTransitInPlaySound.objectToPlayFrom.ptr);
			}
		}
		else
		{
			mAnimProg = Mathf.Max(mAnimTarget, mAnimProg - SUIScreen.deltaTime / mAnimSpeed);
			if (mOnTransitOutPlaySound != null && !mOnTransitOutPlaySound.played && mAnimProg <= mOnTransitOutPlaySound.frameVal)
			{
				mOnTransitOutPlaySound.played = true;
				Singleton<SUISoundManager>.instance.Play(mOnTransitOutPlaySound.soundID, mOnTransitOutPlaySound.objectToPlayFrom.ptr);
			}
		}
		DrawAnimFrame(mAnimProg);
	}

	private void DrawAnimFrame(float prog)
	{
		foreach (KeyValuePair<string, ObjectData> mObject in mObjects)
		{
			if (mObject.Value.animPosition != null && mObject.Value.obj is IHasVisualAttributes)
			{
				IHasVisualAttributes hasVisualAttributes = (IHasVisualAttributes)mObject.Value.obj;
				hasVisualAttributes.position = mObject.Value.animPosition.getAt(prog);
			}
			if (mObject.Value.animAlpha != null)
			{
				float at = mObject.Value.animAlpha.getAt(prog);
				if (mObject.Value.obj is IHasVisualAttributes)
				{
					IHasVisualAttributes hasVisualAttributes2 = (IHasVisualAttributes)mObject.Value.obj;
					hasVisualAttributes2.alpha = at;
				}
				else if (mObject.Value.obj is SUIScrollList)
				{
					SUIScrollList sUIScrollList = (SUIScrollList)mObject.Value.obj;
					sUIScrollList.alpha = at;
				}
			}
		}
	}

	private void StartAnimateIn()
	{
		mAnimProg = 0f;
		mAnimTarget = 1f;
		if (mOnTransitInPlaySound != null)
		{
			mOnTransitInPlaySound.played = false;
		}
		DrawAnimFrame(mAnimProg);
	}

	private void StartAnimateOut()
	{
		mAnimProg = 1f;
		mAnimTarget = 0f;
		if (mOnTransitOutPlaySound != null)
		{
			mOnTransitOutPlaySound.played = false;
		}
		DrawAnimFrame(mAnimProg);
	}
}
