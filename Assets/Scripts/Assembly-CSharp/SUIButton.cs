using System.Collections.Generic;
using UnityEngine;

public class SUIButton : SUIWidget
{
	public enum State
	{
		Normal = 0,
		Pressed = 1,
		Disabled = 2,
		_count = 3
	}

	private const float kLabelPriorityOffset = 0.01f;

	public OnSUIGenericCallback onButtonPressed;

	private bool mInteractionStarted;

	private bool mIsPressed;

	private SUIAnimatedSprite mSprite;

	private List<string> mSpriteFrames = new List<string>(3);

	private Vector2 mOffsetWhenPressed;

	private SUILabel mLabel;

	private Vector2 mLabelOffset;

	private Rect mTouchArea;

	private bool mAreaOverridden;

	private bool mEnabled = true;

	private float mAlphaWhenDisabled = 1f;

	private string mPressedSound = "buttonPressed";

	private string mReleasedSound = "buttonReleased";

	private float mAlpha = 1f;

	public string text
	{
		get
		{
			if (mLabel != null)
			{
				return mLabel.text;
			}
			return string.Empty;
		}
		set
		{
			if (mLabel != null)
			{
				mLabel.text = value;
				UpdateLayout();
			}
		}
	}

	public string frameNormal
	{
		get
		{
			return mSpriteFrames[0];
		}
		set
		{
			mSpriteFrames[0] = value;
			ReloadSprite();
		}
	}

	public string framePressed
	{
		get
		{
			return mSpriteFrames[1];
		}
		set
		{
			mSpriteFrames[1] = value;
			ReloadSprite();
		}
	}

	public string frameDisabled
	{
		get
		{
			return mSpriteFrames[2];
		}
		set
		{
			mSpriteFrames[2] = value;
			ReloadSprite();
		}
	}

	public Vector2 offsetWhenPressed
	{
		get
		{
			return mOffsetWhenPressed;
		}
		set
		{
			mOffsetWhenPressed = value;
			UpdateLayout();
		}
	}

	public Vector2 labelOffset
	{
		get
		{
			return mLabelOffset;
		}
		set
		{
			mLabelOffset = value;
			UpdateLayout();
		}
	}

	public float alphaWhenDisabled
	{
		get
		{
			return mAlphaWhenDisabled;
		}
		set
		{
			mAlphaWhenDisabled = value;
			UpdateLayout();
		}
	}

	public string pressedSound
	{
		get
		{
			return mPressedSound;
		}
		set
		{
			mPressedSound = value;
		}
	}

	public string releasedSound
	{
		get
		{
			return mReleasedSound;
		}
		set
		{
			mReleasedSound = value;
		}
	}

	public override float alpha
	{
		get
		{
			return mAlpha;
		}
		set
		{
			mAlpha = value;
			UpdateLayout();
		}
	}

	public override bool visible
	{
		get
		{
			return base.visible;
		}
		set
		{
			base.visible = value;
			if (mSprite != null)
			{
				mSprite.visible = value;
			}
			if (mLabel != null)
			{
				mLabel.visible = value;
			}
		}
	}

	public bool enabled
	{
		get
		{
			return mEnabled;
		}
		set
		{
			mEnabled = value;
			DrawState((!mEnabled) ? State.Disabled : State.Normal);
			UpdateLayout();
		}
	}

	public override Rect area
	{
		get
		{
			return mTouchArea;
		}
		set
		{
			mTouchArea = value;
			mAreaOverridden = true;
		}
	}

	public bool autoscaleKeepAspectRatio
	{
		get
		{
			if (mSprite != null)
			{
				return mSprite.autoscaleKeepAspectRatio;
			}
			return true;
		}
		set
		{
			if (mSprite != null)
			{
				mSprite.autoscaleKeepAspectRatio = value;
			}
		}
	}

	public override Vector2 scale
	{
		get
		{
			if (mSprite != null)
			{
				return mSprite.scale;
			}
			return base.scale;
		}
		set
		{
			if (mSprite != null)
			{
				mSprite.scale = value;
			}
		}
	}

	public SUILabel label
	{
		get
		{
			return mLabel;
		}
	}

	public SUIButton()
	{
		Init();
	}

	public SUIButton(string fontFile)
	{
		Init();
		mLabel = new SUILabel(fontFile);
		mLabel.alignment = TextAlignment.Center;
		mLabel.anchor = TextAnchor.MiddleCenter;
		UpdateLayout();
	}

	public override void Update()
	{
		base.Update();
		if (visible)
		{
			UpdateInput();
		}
	}

	public override void Destroy()
	{
		if (mSprite != null)
		{
			mSprite.Destroy();
			mSprite = null;
		}
		if (mLabel != null)
		{
			mLabel.Destroy();
			mLabel = null;
		}
		base.Destroy();
	}

	public void SetFrames(string normalState, string pressedState, string disabledState)
	{
		mSpriteFrames[0] = normalState;
		mSpriteFrames[1] = pressedState;
		mSpriteFrames[2] = disabledState;
		ReloadSprite();
	}

	public override void EditorRenderOnGUI()
	{
		if (mSprite != null)
		{
			mSprite.EditorRenderOnGUI();
		}
		if (mLabel != null)
		{
			mLabel.EditorRenderOnGUI();
		}
	}

	private void Init()
	{
		for (int i = 0; i < 3; i++)
		{
			mSpriteFrames.Add(string.Empty);
		}
	}

	private void UpdateInput()
	{
		if (!mEnabled)
		{
			mInteractionStarted = false;
			ShowPressed(false);
			DrawState(State.Disabled);
		}
		else if (mInteractionStarted)
		{
			if (WeakGlobalInstance<SUIScreen>.instance.inputs.isTouching)
			{
				ShowPressed(mTouchArea.Contains(WeakGlobalInstance<SUIScreen>.instance.inputs.position));
				return;
			}
			mInteractionStarted = false;
			ShowPressed(false);
			if (mTouchArea.Contains(WeakGlobalInstance<SUIScreen>.instance.inputs.position) && onButtonPressed != null)
			{
				onButtonPressed();
			}
		}
		else if (WeakGlobalInstance<SUIScreen>.instance.inputs.justTouched && mTouchArea.Contains(WeakGlobalInstance<SUIScreen>.instance.inputs.position))
		{
			mInteractionStarted = true;
			ShowPressed(true);
		}
	}

	protected override void updatePosition()
	{
		base.updatePosition();
		UpdateLayout();
	}

	private void UpdateLayout()
	{
		Vector2 vector = position;
		if (mIsPressed)
		{
			vector += mOffsetWhenPressed;
		}
		if (mSprite != null)
		{
			int num = (mIsPressed ? 1 : 0);
			if (num < mSprite.numFrames)
			{
				mSprite.frame = num;
			}
			mSprite.position = vector;
			mSprite.priority = priority;
			mSprite.alpha = alpha * ((!mEnabled) ? mAlphaWhenDisabled : 1f);
		}
		if (mLabel != null)
		{
			mLabel.position = vector + mLabelOffset;
			mLabel.priority = priority + 0.01f;
			mLabel.alpha = alpha * ((!mEnabled) ? mAlphaWhenDisabled : 1f);
		}
		RecalcTouchArea();
	}

	private void ReloadSprite()
	{
		if (mSpriteFrames[0] == string.Empty && mSpriteFrames[1] == string.Empty && mSpriteFrames[2] == string.Empty)
		{
			if (mSprite != null)
			{
				mSprite.Destroy();
				mSprite = null;
			}
			UpdateLayout();
			return;
		}
		List<string> list = new List<string>();
		string item = string.Empty;
		foreach (string mSpriteFrame in mSpriteFrames)
		{
			if (mSpriteFrame != string.Empty)
			{
				item = mSpriteFrame;
				break;
			}
		}
		foreach (string mSpriteFrame2 in mSpriteFrames)
		{
			if (mSpriteFrame2 == string.Empty)
			{
				list.Add(item);
			}
			else
			{
				list.Add(mSpriteFrame2);
			}
		}
		if (mSprite == null)
		{
			mSprite = new SUIAnimatedSprite(list);
		}
		UpdateLayout();
	}

	private void RecalcTouchArea()
	{
		if (mAreaOverridden)
		{
			return;
		}
		if (mSprite != null && mLabel != null)
		{
			Rect rect = mSprite.area;
			Rect rect2 = mLabel.area;
			if (rect2.width == 0f || rect2.height == 0f)
			{
				mTouchArea = rect;
			}
			else
			{
				float num = Mathf.Min(rect.xMin, rect2.xMin);
				float num2 = Mathf.Min(rect.yMin, rect2.yMin);
				float num3 = Mathf.Max(rect.xMax, rect2.xMax);
				float num4 = Mathf.Max(rect.yMax, rect2.yMax);
				mTouchArea = new Rect(num, num2, num3 - num, num4 - num2);
			}
		}
		else if (mSprite != null)
		{
			mTouchArea = mSprite.area;
		}
		else if (mLabel != null)
		{
			mTouchArea = mLabel.area;
		}
		else
		{
			mTouchArea = new Rect(position.x, position.y, 0f, 0f);
		}
		if (mIsPressed)
		{
			mTouchArea = new Rect(mTouchArea.xMin - mOffsetWhenPressed.x, mTouchArea.yMin - mOffsetWhenPressed.y, mTouchArea.width, mTouchArea.height);
		}
	}

	private void ShowPressed(bool pressed)
	{
		if (mIsPressed != pressed)
		{
			mIsPressed = pressed;
			if (mSprite != null)
			{
				mSprite.frame = (mIsPressed ? 1 : 0);
			}
			DrawState(mIsPressed ? State.Pressed : State.Normal);
			UpdateLayout();
			if (pressed)
			{
				Singleton<SUISoundManager>.instance.Play(mPressedSound, mGameObject);
			}
			else
			{
				Singleton<SUISoundManager>.instance.Play(mReleasedSound, mGameObject);
			}
		}
	}

	private void DrawState(State state)
	{
		if (mSprite != null)
		{
			mSprite.frame = (int)state;
		}
	}
}
