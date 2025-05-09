using System.Collections.Generic;

public class DialogHandler : WeakGlobalInstance<DialogHandler>
{
	public delegate IDialog Creator();

	private const float kTransitionSpeed = 0.3f;

	private const float kDefaultFadeLevel = 0.7f;

	private IDialog mCurrentDialog;

	private Stack<Creator> mCreators = new Stack<Creator>();

	private SUIFader mFader;

	private OnSUIGenericCallback mExtraUpdateCode;

	public bool isDone
	{
		get
		{
			if ((mCurrentDialog != null && !mCurrentDialog.isDone) || mCreators.Count > 0)
			{
				return false;
			}
			return true;
		}
	}

	public bool isBlocking
	{
		get
		{
			return mCurrentDialog != null && mCurrentDialog.isBlocking;
		}
	}

	public float fadeLevel
	{
		set
		{
			mFader.FadeToLevel(value);
		}
	}

	public float fadeSpeed
	{
		set
		{
			mFader.speed = value;
		}
	}

	public OnSUIGenericCallback extraUpdateCode
	{
		get
		{
			return mExtraUpdateCode;
		}
		set
		{
			mExtraUpdateCode = value;
		}
	}

	public DialogHandler(float faderPriority, IDialog initialDialog)
	{
		SetUniqueInstance(this);
		mFader = new SUIFader();
		mFader.speed = 0.3f;
		mFader.priority = faderPriority;
		mFader.FadeToLevel(0.7f);
		mCurrentDialog = initialDialog;
	}

	public void Destroy()
	{
		if (mCurrentDialog != null)
		{
			mCurrentDialog.Destroy();
			mCurrentDialog = null;
		}
		if (mFader != null)
		{
			mFader.Destroy();
			mFader = null;
		}
		mCreators = null;
		SetUniqueInstance(null);
	}

	public void Update()
	{
		mFader.Update();
		if (mCurrentDialog != null)
		{
			mCurrentDialog.Update();
			if (mCurrentDialog.isDone)
			{
				mCurrentDialog.Destroy();
				mCurrentDialog = null;
				if (mCreators.Count > 0)
				{
					mCurrentDialog = mCreators.Pop()();
				}
			}
		}
		if (mExtraUpdateCode != null)
		{
			mExtraUpdateCode();
		}
	}

	public void PushCreator(Creator c)
	{
		mCreators.Push(c);
	}

	public void FadeOut()
	{
		if (mCreators == null || mCreators.Count <= 0)
		{
			mFader.speed = 0.3f;
			mFader.FadeToLevel(0f);
		}
	}
}
