using UnityEngine;

public class PauseMenu : IDialog
{
	private const float kFadeLevel = 0.7f;

	private const float kFadePriority = 500f;

	private YesNoDialog mInternetRequired;

	private SUILayout mLayout;

	private SUIButton mGameCenter;

	private bool mIsDone;

	public bool isDone
	{
		get
		{
			return mIsDone;
		}
	}

	public bool isBlocking
	{
		get
		{
			return true;
		}
	}

	public OnSUIGenericCallback onQuitGameRequest
	{
		get
		{
			return ((SUIButton)mLayout["quitBtn"]).onButtonPressed;
		}
		set
		{
			((SUIButton)mLayout["quitBtn"]).onButtonPressed = value;
		}
	}

	public OnSUIGenericCallback onRestartGameRequest
	{
		get
		{
			return ((SUIButton)mLayout["restartBtn"]).onButtonPressed;
		}
		set
		{
			((SUIButton)mLayout["restartBtn"]).onButtonPressed = value;
		}
	}

	public PauseMenu()
	{
		mLayout = new SUILayout("Layouts/PauseMenuLayout");
		mLayout.AnimateIn(0.7f * mLayout.defaultTransitionSpeed);
		((SUIButton)mLayout["resumeBtn"]).onButtonPressed = onResume;
		ApplicationUtilities.instance.ShowAds((ApplicationUtilities.AdPosition)17);
		mGameCenter = (SUIButton)mLayout["gameCenter"];
		mGameCenter.onButtonPressed = delegate
		{
		};
		mGameCenter.visible = false;
	}

	public void Update()
	{
		mLayout.Update();
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			onResume();
		}
	}

	public void Destroy()
	{
		ApplicationUtilities.instance.HideAds();
		mLayout.Destroy();
		mLayout = null;
	}

	private void onResume()
	{
        ((SUIButton)mLayout["resumeBtn"]).enabled = false;
        ((SUIButton)mLayout["restartBtn"]).enabled = false;
        ((SUIButton)mLayout["quitBtn"]).enabled = false;
        mLayout.AnimateOut(0.7f * mLayout.defaultTransitionSpeed);
		mLayout.onTransitionOver = onFadingDone;
		if (WeakGlobalInstance<DialogHandler>.instance != null)
		{
			WeakGlobalInstance<DialogHandler>.instance.FadeOut();
		}
	}

	private void onFadingDone()
	{
		mIsDone = true;
	}
}
