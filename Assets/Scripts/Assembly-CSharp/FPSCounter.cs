using UnityEngine;

public class FPSCounter
{
	private SUILabel mLabel;

	private float mTimer;

	private int mFrameCount;

	private int mLowestFPS = 10000;

	public FPSCounter(string font, Vector2 pos, float priority)
	{
		mLabel = new SUILabel(font);
		mLabel.position = pos;
		mLabel.priority = priority;
	}

	public void Update()
	{
		mTimer += Time.deltaTime;
		mFrameCount++;
		if (!(mTimer < 1f))
		{
			if (mFrameCount < mLowestFPS)
			{
				mLowestFPS = mFrameCount;
			}
			mLabel.text = mFrameCount + "\n" + mLowestFPS;
			mTimer -= 1f;
			mFrameCount = 0;
		}
	}
}
