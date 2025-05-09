using UnityEngine;

public class TwoOptionDialog : IDialog
{
	private OnSUIGenericCallback m_backKeyDelegate;

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

	public TwoOptionDialog(string text, bool inChargeOfFadeOut, string button1, OnSUIGenericCallback onButton1, string button2, OnSUIGenericCallback onbutton2, string layout)
	{
		Init(text, inChargeOfFadeOut, button1, onButton1, button2, onbutton2, layout);
	}

	private void Init(string text, bool inChargeOfFadeOut, string button1, OnSUIGenericCallback onButton1, string button2, OnSUIGenericCallback onbutton2, string layout)
	{
		if (layout != null)
		{
			mLayout = new SUILayout(layout);
		}
		else
		{
			mLayout = new SUILayout("Layouts/TwoOptionDialog");
		}
		mInChargeOfFadeOut = inChargeOfFadeOut;
		((SUILabel)mLayout["title32"]).text = text;
		((SUIButton)mLayout["button1"]).text = button1;
		((SUIButton)mLayout["button2"]).text = button2;
		SUIButton sUIButton = (SUIButton)mLayout["button1"];
		SUIButton sUIButton2 = (SUIButton)mLayout["button2"];
		Vector2 position = new Vector2((sUIButton.position.x + sUIButton2.position.x) / 2f, sUIButton.position.y);
		sUIButton.onButtonPressed = delegate
		{
			if (onButton1 != null)
			{
				onButton1();
			}
		};
		if (onbutton2 != null)
		{
			sUIButton2.onButtonPressed = delegate
			{
				onbutton2();
			};
		}
		else
		{
			sUIButton2.visible = false;
			sUIButton.position = position;
			sUIButton.text = Singleton<Localizer>.instance.Get("ok");
		}
		m_backKeyDelegate = null;
		if (mLayout.Exists("back"))
		{
			((SUIButton)mLayout["back"]).onButtonPressed = delegate
			{
				Close();
			};
		}
		mLayout.AnimateIn();
	}

	public void DisableButtons()
	{
		SUIButton sUIButton = (SUIButton)mLayout["button1"];
		SUIButton sUIButton2 = (SUIButton)mLayout["button2"];
		SUIButton sUIButton3 = (SUIButton)mLayout["back"];
		sUIButton.offsetWhenPressed = new Vector2(0f, 0f);
		sUIButton2.offsetWhenPressed = new Vector2(0f, 0f);
		sUIButton3.offsetWhenPressed = new Vector2(0f, 0f);
		if (sUIButton.onButtonPressed != null)
		{
			sUIButton.onButtonPressed = null;
		}
		if (sUIButton2.onButtonPressed != null)
		{
			sUIButton2.onButtonPressed = null;
		}
		if (sUIButton3.onButtonPressed != null)
		{
			sUIButton3.onButtonPressed = null;
		}
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
			if (m_backKeyDelegate != null)
			{
				m_backKeyDelegate();
			}
			else
			{
				Close();
			}
		}
	}

	public void Close()
	{
		SUIButton sUIButton = (SUIButton)mLayout["button1"];
		SUIButton sUIButton2 = (SUIButton)mLayout["button2"];
		if (sUIButton != null)
		{
			sUIButton.onButtonPressed = null;
		}
		if (sUIButton2 != null)
		{
			sUIButton2.onButtonPressed = null;
		}
		mDone = true;
		mLayout.AnimateOut();
		if (mInChargeOfFadeOut && WeakGlobalInstance<DialogHandler>.instance != null)
		{
			WeakGlobalInstance<DialogHandler>.instance.FadeOut();
		}
		m_backKeyDelegate = null;
	}

	public void SetBackKey(OnSUIGenericCallback onButton)
	{
		m_backKeyDelegate = onButton;
	}
}
