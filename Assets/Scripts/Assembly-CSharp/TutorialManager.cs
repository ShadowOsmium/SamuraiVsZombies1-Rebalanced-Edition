using System.Collections.Generic;
using Debug = UnityEngine.Debug;

public class TutorialManager : IDialog
{
	private delegate bool ConditionalDelegate();

	private const float kDelayBetweenPopups = 0.5f;

	private string mScriptName = string.Empty;

	private List<SDFTreeNode> mEvents = new List<SDFTreeNode>();

	private List<SDFTreeNode> mReactives = new List<SDFTreeNode>();

	private TutorialHookup mHookups = new TutorialHookup();

	private TutorialPopup mPopup;

	private ConditionalDelegate mPopupEndCondition;

	private bool mPopupIsBlocking;

	private float mTimerSinceLastPopup;

	private float mPopupTimer;

	public bool isBlocking
	{
		get
		{
			return mPopupIsBlocking;
		}
	}

	public bool isDone
	{
		get
		{
			return mEvents.Count == 0 && mReactives.Count == 0 && (mPopup == null || !mPopup.visible);
		}
	}

	public bool nextCommandIsHighPriority
	{
		get
		{
			foreach (SDFTreeNode mEvent in mEvents)
			{
				if (IsCommandConditionMet(mEvent))
				{
					if (mEvent.hasAttribute("highPriority"))
					{
						return SUILayoutConv.GetBool(mEvent["highPriority"]);
					}
					return false;
				}
			}
			return false;
		}
	}

	public bool isShowingPopup
	{
		get
		{
			return mPopup != null;
		}
	}

	public TutorialManager(string script, string reactives)
	{
		if (script != string.Empty)
		{
			SetScript(script);
		}
		if (reactives != string.Empty)
		{
			AddReactives(reactives);
		}
	}

	public void Destroy()
	{
		End();
	}

	public void Update()
	{
		if (isDone)
		{
			return;
		}
		if (mPopup != null)
		{
			mPopup.Update();
			if (mPopup.visible)
			{
				mPopupTimer -= SUIScreen.deltaTime;
				if (mPopupEndCondition())
				{
					mPopup.Hide();
					mTimerSinceLastPopup = 0f;
					mPopupIsBlocking = false;
				}
				return;
			}
		}
		mTimerSinceLastPopup += SUIScreen.deltaTime;
		RunNextCommand();
	}

	public void RunNextCommand()
	{
		if (mPopup != null && mPopup.alpha > 0f)
		{
			return;
		}
		if (mEvents.Count > 0)
		{
			SDFTreeNode sDFTreeNode = mEvents[0];
			if (IsCommandConditionMet(sDFTreeNode))
			{
				Run(sDFTreeNode, mScriptName);
				mEvents.Remove(sDFTreeNode);
				return;
			}
		}
		foreach (SDFTreeNode mReactive in mReactives)
		{
			if (IsCommandConditionMet(mReactive))
			{
				Run(mReactive, "reactives");
				mReactives.Remove(mReactive);
				break;
			}
		}
	}

	private void SetScript(string script)
	{
		mScriptName = script;
		SDFTreeNode sDFTreeNode = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/Tutorials/" + script);
		SDFTreeNode sDFTreeNode2 = sDFTreeNode.to("events");
		if (sDFTreeNode2 == null)
		{
			Debug.Log("ERROR: Could not find [events] section in tutorial file: " + script);
			return;
		}
		for (int i = 0; sDFTreeNode2.hasAttribute(i); i++)
		{
			string text = sDFTreeNode2[i];
			if (sDFTreeNode.hasChild(text) && !Singleton<Profile>.instance.IsTutorialDone(script, text))
			{
				SDFTreeNode sDFTreeNode3 = sDFTreeNode.to(text);
				sDFTreeNode3["_name"] = text;
				mEvents.Add(sDFTreeNode3);
			}
		}
	}

	private void AddReactives(string filename)
	{
		SDFTreeNode sDFTreeNode = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/Tutorials/" + filename);
		foreach (KeyValuePair<string, SDFTreeNode> child in sDFTreeNode.childs)
		{
			if (!Singleton<Profile>.instance.IsTutorialDone("reactives", child.Key))
			{
				SDFTreeNode value = child.Value;
				value["_name"] = child.Key;
				mReactives.Add(value);
			}
		}
	}

	private void End()
	{
		mHookups.Destroy();
		mHookups = null;
		mEvents.Clear();
		if (mPopup != null)
		{
			mPopup.Destroy();
			mPopup = null;
		}
	}

	private bool IsCommandConditionMet(SDFTreeNode data)
	{
		if (!data.hasAttribute("condition"))
		{
			return true;
		}
		string text = data["condition"];
		try
		{
			float num = float.Parse(text);
			return mTimerSinceLastPopup >= num;
		}
		catch
		{
			switch (text)
			{
			case "firstHelperAvailable":
				return mHookups.firstHelperAvailable;
			case "enemyInRange":
				return mHookups.enemyIsInRangeOFAttack;
			case "flyingEnemyInView":
				return mHookups.flyingEnemyInView;
			case "free_booster_packs":
				return Singleton<Profile>.instance.freeBoosterPacks > 0;
			}
		}
		return true;
	}

	private void Run(SDFTreeNode data, string groupName)
	{
		Singleton<Profile>.instance.SetTutorialDone(groupName, data["_name"]);
		if (mPopup == null)
		{
			mPopup = new TutorialPopup();
		}
		else
		{
			mPopup.Hide();
		}
		mPopup.ShowPanel(Singleton<Localizer>.instance.Parse(data["text"]));
		if (data.hasAttribute("position"))
		{
			mPopup.SetPanelPosition(SUILayoutConv.GetVector2(data["position"]));
		}
		if (data.hasAttribute("leftarrow"))
		{
			mPopup.ShowLeftArrow(SUILayoutConv.GetVector2(data["leftarrow"]));
		}
		if (data.hasAttribute("rightarrow"))
		{
			mPopup.ShowRightArrow(SUILayoutConv.GetVector2(data["rightarrow"]));
		}
		if (data.hasAttribute("circle"))
		{
			mPopup.ShowCircle(SUILayoutConv.GetVector2(data["circle"]));
		}
		mPopupEndCondition = GetConditional(data["endCondition"]);
		if (data.hasAttribute("blocking"))
		{
			mPopupIsBlocking = SUILayoutConv.GetBool(data["blocking"]);
		}
		else
		{
			mPopupIsBlocking = false;
		}
		if (data.hasAttribute("flag") && data["flag"] == "showKatanaSlash")
		{
			WeakGlobalInstance<TutorialHookup>.instance.showKatanaSlash = true;
		}
	}

	private ConditionalDelegate GetConditional(string conditionalID)
	{
		try
		{
			mPopupTimer = float.Parse(conditionalID);
			return () => mPopupTimer <= 0f;
		}
		catch
		{
			switch (conditionalID)
			{
			case "touchScreen":
				mPopupTimer = 1f;
				return () => mPopupTimer <= 0f && WeakGlobalInstance<SUIScreen>.instance.inputs.justTouched;
			case "pressBothDirections":
				mHookups.playerPressedLeft = false;
				mHookups.playerPressedRight = false;
				return () => mHookups.playerPressedLeft && mHookups.playerPressedRight;
			case "useAbility":
				mHookups.usedAbility = false;
				return () => mHookups.usedAbility;
			case "summonAlly":
				mHookups.summonedAlly = false;
				return () => mHookups.summonedAlly;
			case "touchVillagerTab":
				mHookups.storeTabTouched[1] = false;
				return () => mHookups.storeTabTouched[1];
			case "touchExtraTab":
				mHookups.storeTabTouched[2] = false;
				return () => mHookups.storeTabTouched[2];
			}
		}
		return null;
	}
}
