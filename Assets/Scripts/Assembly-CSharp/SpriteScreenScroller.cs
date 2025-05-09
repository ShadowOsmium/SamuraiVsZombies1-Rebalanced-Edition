using UnityEngine;

public class SpriteScreenScroller
{
	private SUISprite mOriginalRef;

	private SUISprite mClone;

	private Vector2 mScrollSpeed;

	private float mAbsSpeed;

	private float mTotalScrolled;

	private float mScrollAmountTrigger;

	private SUISprite mFront;

	public float speed
	{
		get
		{
			if (mScrollSpeed.x != 0f)
			{
				return mScrollSpeed.x;
			}
			return mScrollSpeed.y;
		}
		set
		{
			if (mScrollSpeed.x != 0f)
			{
				mScrollSpeed.x = value;
			}
			else
			{
				mScrollSpeed.y = value;
			}
			mAbsSpeed = Mathf.Abs(value);
		}
	}

	public SpriteScreenScroller(SUISprite originalSpr)
	{
		mOriginalRef = originalSpr;
		mClone = new SUISprite(originalSpr.texture);
		mClone.priority = originalSpr.priority;
		mClone.hotspotPixels = originalSpr.hotspotPixels;
		mClone.autoscaleKeepAspectRatio = originalSpr.autoscaleKeepAspectRatio;
		mClone.visible = false;
	}

	public void Destroy()
	{
		mFront = null;
		if (mClone != null)
		{
			mClone.Destroy();
			mClone = null;
		}
	}

	public void Update()
	{
		if (mClone.visible)
		{
			UpdateScrolling();
		}
	}

	public void ScrollHorizontally(float speed)
	{
		mAbsSpeed = Mathf.Abs(speed);
		mTotalScrolled = 0f;
		mScrollAmountTrigger = SUIScreen.width;
		mScrollSpeed = new Vector2(speed, 0f);
		if (speed > 0f)
		{
			mFront = mOriginalRef;
			mClone.position = new Vector2(mOriginalRef.position.x - mOriginalRef.area.width, mOriginalRef.position.y);
			mClone.visible = true;
		}
		else if (speed < 0f)
		{
			mFront = mOriginalRef;
			mClone.position = new Vector2(mOriginalRef.position.x + mOriginalRef.area.width, mOriginalRef.position.y);
			mClone.visible = true;
		}
	}

	private void UpdateScrolling()
	{
		Vector2 vector = new Vector2(mScrollSpeed.x * SUIScreen.deltaTime, mScrollSpeed.y * SUIScreen.deltaTime);
		mOriginalRef.position += vector;
		mClone.position += vector;
		mTotalScrolled += mAbsSpeed * SUIScreen.deltaTime;
		if (mTotalScrolled < mScrollAmountTrigger)
		{
			return;
		}
		mTotalScrolled -= mScrollAmountTrigger;
		if (mScrollSpeed.x != 0f)
		{
			if (mScrollSpeed.x > 0f)
			{
				mFront.position -= new Vector2(mScrollAmountTrigger * 2f, 0f);
			}
			else
			{
				mFront.position += new Vector2(mScrollAmountTrigger * 2f, 0f);
			}
		}
		if (mFront == mClone)
		{
			mFront = mOriginalRef;
		}
		else
		{
			mFront = mClone;
		}
	}
}
