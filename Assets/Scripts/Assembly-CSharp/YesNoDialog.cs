using UnityEngine;

public class YesNoDialog : IDialog
{
	private SUILayout mLayout;

	private bool mDone;

	private bool mInChargeOfFadeOut;

	public bool isDone
	{
		get
		{
			return mDone && !mLayout.isAnimating;
		}
	}

	public bool isBlocking
	{
		get
		{
			return true;
		}
	}

	public float priority
	{
		get
		{
			return mLayout.basePriority;
		}
		set
		{
			mLayout.basePriority = value;
		}
	}

	public YesNoDialog(string text, bool inChargeOfFadeOut, bool useSmallFont)
	{
		Init(text, inChargeOfFadeOut, null, null, useSmallFont);
	}

	public YesNoDialog(string text, bool inChargeOfFadeOut, OnSUIGenericCallback onYes, OnSUIGenericCallback onNo)
	{
		Init(text, inChargeOfFadeOut, onYes, onNo, false);
	}

	private void Init(string text, bool inChargeOfFadeOut, OnSUIGenericCallback onYes, OnSUIGenericCallback onNo, bool useSmallFont)
	{
		mLayout = new SUILayout("Layouts/YesNoDialog");
		mInChargeOfFadeOut = inChargeOfFadeOut;
		if (useSmallFont)
		{
			((SUILabel)mLayout["title18"]).text = text;
			((SUILabel)mLayout["title18"]).visible = true;
			((SUILabel)mLayout["title32"]).visible = false;
		}
		else
		{
			((SUILabel)mLayout["title32"]).text = text;
		}
		SUIButton sUIButton = (SUIButton)mLayout["yesBtn"];
		SUIButton sUIButton2 = (SUIButton)mLayout["noBtn"];
		Vector2 position = new Vector2((sUIButton.position.x + sUIButton2.position.x) / 2f, sUIButton.position.y);
		sUIButton.onButtonPressed = delegate
		{
			if (onYes != null)
			{
				onYes();
			}
			Close();
		};
		if (onNo != null)
		{
			sUIButton2.onButtonPressed = delegate
			{
				onNo();
				Close();
			};
		}
		else
		{
			sUIButton2.visible = false;
			sUIButton.position = position;
			sUIButton.text = Singleton<Localizer>.instance.Get("ok");
		}
		((SUIButton)mLayout["back"]).visible = false;
		mLayout.AnimateIn();
	}

	public void Destroy()
	{
		mLayout.Destroy();
		mLayout = null;
	}

	public void Update()
	{
		mLayout.Update();
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			SUIButton sUIButton = (SUIButton)mLayout["noBtn"];
			if (sUIButton.onButtonPressed != null)
			{
				sUIButton.onButtonPressed();
			}
			else
			{
				Close();
			}
		}
	}

	private void Close()
	{
		((SUIButton)mLayout["yesBtn"]).onButtonPressed = null;
		((SUIButton)mLayout["noBtn"]).onButtonPressed = null;
		mDone = true;
		mLayout.AnimateOut();
		if (mInChargeOfFadeOut && WeakGlobalInstance<DialogHandler>.instance != null)
		{
			WeakGlobalInstance<DialogHandler>.instance.FadeOut();
		}
	}
}
