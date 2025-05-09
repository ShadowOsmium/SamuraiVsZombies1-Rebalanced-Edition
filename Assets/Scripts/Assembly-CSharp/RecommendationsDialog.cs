public class RecommendationsDialog : IDialog
{
	private SUILayout mLayout;

	private bool mDone;

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
			return false;
		}
		set
		{
		}
	}

	public RecommendationsDialog(string text)
	{
		mLayout = new SUILayout("Layouts/Recommendations");
		((SUIButton)mLayout["okBtn"]).onButtonPressed = OnClose;
		((SUIButton)mLayout["back"]).onButtonPressed = OnClose;
		((SUILabel)mLayout["title18"]).text = text;
	}

	public void Destroy()
	{
		if (mLayout != null)
		{
			mLayout.Destroy();
			mLayout = null;
		}
	}

	public void Update()
	{
		mLayout.Update();
	}

	private void OnClose()
	{
		((SUIButton)mLayout["okBtn"]).onButtonPressed = null;
		((SUIButton)mLayout["noBtn"]).onButtonPressed = null;
		mDone = true;
		mLayout.AnimateOut();
		if (WeakGlobalInstance<DialogHandler>.instance != null)
		{
			WeakGlobalInstance<DialogHandler>.instance.FadeOut();
		}
	}
}
