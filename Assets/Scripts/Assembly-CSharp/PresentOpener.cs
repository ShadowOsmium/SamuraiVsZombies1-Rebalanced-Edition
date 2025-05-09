using UnityEngine;

public class PresentOpener : SUIProcess, IHasVisualAttributes
{
	private readonly Vector2 kBoosterPackScale = new Vector2(0.6f, 0.6f);

	private SUISprite mPresentSprite;

	private ListCells.IconNameCounter mContentIcon;

	private BoosterPackSprite mBoosterPack;

	private float mFrame;

	private float mPlayDelay;

	private float mPlaySpeed;

	private Vector2 mPosition;

	private bool mVisible = true;

	private float mAlpha = 1f;

	public Vector2 position
	{
		get
		{
			return mPosition;
		}
		set
		{
			mPosition = value;
			mContentIcon.position = value - mContentIcon.iconOffset;
			if (mBoosterPack != null)
			{
				mBoosterPack.position = value;
			}
			if (mPresentSprite != null)
			{
				mPresentSprite.position = value;
			}
		}
	}

	public float priority
	{
		get
		{
			return mContentIcon.priority;
		}
		set
		{
			mContentIcon.priority = value;
			if (mBoosterPack != null)
			{
				mBoosterPack.priority = value;
			}
			if (mPresentSprite != null)
			{
				mPresentSprite.priority = value + 0.1f;
			}
		}
	}

	public float frame
	{
		get
		{
			return mFrame;
		}
		set
		{
			mFrame = Mathf.Clamp(value, 0f, 1f);
			RedrawFrame();
		}
	}

	public bool visible
	{
		get
		{
			return mVisible;
		}
		set
		{
			mVisible = value;
			RedrawFrame();
		}
	}

	public float alpha
	{
		get
		{
			return mAlpha;
		}
		set
		{
			mAlpha = Mathf.Clamp(value, 0f, 1f);
			RedrawFrame();
		}
	}

	public SUILabel label
	{
		get
		{
			return mContentIcon.title;
		}
	}

	public Vector2 scale
	{
		get
		{
			return new Vector2(1f, 1f);
		}
		set
		{
		}
	}

	public PresentOpener(string presentSprite, string contentSprite, string contentTitle, string fontName, int maxTitleWidth, int num)
	{
		if (presentSprite != null && presentSprite != string.Empty)
		{
			mPresentSprite = new SUISprite(presentSprite);
		}
		mContentIcon = new ListCells.IconNameCounter(fontName, null, true);
		if (contentSprite[0] == '@')
		{
			mBoosterPack = new BoosterPackSprite();
			mBoosterPack.Render(Singleton<BoosterPackCodex>.instance.GetPackData(contentSprite.Substring(1)).to("render"));
			mBoosterPack.scale = kBoosterPackScale;
		}
		else
		{
			mContentIcon.icon.texture = contentSprite;
		}
		mContentIcon.title.maxWidth = maxTitleWidth;
		mContentIcon.title.text = contentTitle;
		if (num > 1)
		{
			mContentIcon.counter.text = "x" + num;
		}
		priority = 0f;
		frame = 0f;
	}

	public void Destroy()
	{
		if (mPresentSprite != null)
		{
			mPresentSprite.Destroy();
			mPresentSprite = null;
		}
		mContentIcon.Destroy();
		mContentIcon = null;
		if (mBoosterPack != null)
		{
			mBoosterPack.Destroy();
			mBoosterPack = null;
		}
	}

	public void Update()
	{
		if (mPlayDelay > 0f)
		{
			mPlayDelay = Mathf.Max(0f, mPlayDelay - SUIScreen.deltaTime);
		}
		else if (mPlaySpeed != 0f)
		{
			if (frame == 0f)
			{
				Singleton<SUISoundManager>.instance.Play("presentOpening");
			}
			frame += SUIScreen.deltaTime / mPlaySpeed;
			if (frame == 1f)
			{
				mPlaySpeed = 0f;
			}
		}
	}

	public void Open(float atSpeed, float afterDelay)
	{
		mPlayDelay = afterDelay;
		mPlaySpeed = atSpeed;
	}

	public void EditorRenderOnGUI()
	{
	}

	private void RedrawFrame()
	{
		if (mPresentSprite != null)
		{
			mPresentSprite.alpha = (1f - mFrame) * mAlpha;
			mPresentSprite.scale = new Vector2(1f + mFrame, 1f + mFrame);
			mPresentSprite.visible = mFrame != 1f && mVisible;
		}
		mContentIcon.alpha = mFrame * mAlpha;
		mContentIcon.visible = mFrame != 0f && mVisible;
		if (mBoosterPack != null)
		{
			mBoosterPack.alpha = mFrame * mAlpha;
			mBoosterPack.visible = mFrame != 0f && mVisible;
		}
	}
}
