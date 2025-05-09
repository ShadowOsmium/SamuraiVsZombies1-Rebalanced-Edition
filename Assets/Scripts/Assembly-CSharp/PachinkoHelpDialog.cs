using UnityEngine;

public class PachinkoHelpDialog : IDialog
{
	private SUILayout mLayout;

	private bool mDone;

	public bool isBlocking
	{
		get
		{
			return true;
		}
	}

	public bool isDone
	{
		get
		{
			return mDone;
		}
	}

	public PachinkoHelpDialog()
	{
		mLayout = new SUILayout("Layouts/PachinkoHelp");
		mLayout.basePriority = 500f;
		mLayout.AnimateIn();
		((SUIButton)mLayout["back"]).onButtonPressed = Close;
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
			Close();
		}
	}

	private void Close()
	{
		((SUIButton)mLayout["back"]).onButtonPressed = null;
		mLayout.AnimateOut();
		mLayout.onTransitionOver = delegate
		{
			mDone = true;
		};
	}
}
