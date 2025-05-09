using System.Collections.Generic;
using UnityEngine;

public class PachinkoSummaryDialog : IDialog
{
	private const int kMaxSpoilsPerScreens = 8;

	private SUILayout mLayout;

	private List<SpoilsDisplay> mSpoilsDisplay = new List<SpoilsDisplay>();

	private bool mDone;

	private List<SpoilsDisplay.Entry> mLeftOverSpoils = new List<SpoilsDisplay.Entry>();

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

	public PachinkoSummaryDialog(List<SpoilsDisplay.Entry> spoils)
	{
		while (spoils.Count > 8)
		{
			int index = spoils.Count - 1;
			mLeftOverSpoils.Add(spoils[index]);
			spoils.RemoveAt(index);
		}
		mLayout = new SUILayout("Layouts/PachinkoSummary");
		RenderSpoils(mLayout, spoils);
		mLayout.basePriority = 500f;
		mLayout.AnimateIn();
		((SUIButton)mLayout["back"]).onButtonPressed = Close;
	}

	public void Destroy()
	{
		foreach (SpoilsDisplay item in mSpoilsDisplay)
		{
			item.Destroy();
		}
		mSpoilsDisplay.Clear();
		mLayout.Destroy();
		mLayout = null;
	}

	public void Update()
	{
		mLayout.Update();
		foreach (SpoilsDisplay item in mSpoilsDisplay)
		{
			item.Update();
		}
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			Close();
		}
	}

	private void Close()
	{
		((SUIButton)mLayout["back"]).onButtonPressed = null;
		if (mLeftOverSpoils.Count != 0)
		{
			WeakGlobalInstance<DialogHandler>.instance.PushCreator(() => new PachinkoSummaryDialog(mLeftOverSpoils));
		}
		mLayout.AnimateOut();
		mLayout.onTransitionOver = delegate
		{
			mDone = true;
		};
	}

	private void RenderSpoils(SUILayout layout, List<SpoilsDisplay.Entry> spoils)
	{
		float num = ((spoils.Count > 4) ? 380 : 480);
		List<SpoilsDisplay.Entry> list = new List<SpoilsDisplay.Entry>(4);
		foreach (SpoilsDisplay.Entry spoil in spoils)
		{
			list.Add(spoil);
			if (list.Count == 4)
			{
				mSpoilsDisplay.Add(new SpoilsDisplay(layout, list, "default18", new Vector2(SUIScreen.width / 2f, num), 2f, 4, 160f, num.ToString()));
				list.Clear();
				num += 184f;
			}
		}
		if (list.Count > 0)
		{
			mSpoilsDisplay.Add(new SpoilsDisplay(layout, list, "default18", new Vector2(SUIScreen.width / 2f, num), 2f, 4, 160f, num.ToString()));
		}
	}
}
