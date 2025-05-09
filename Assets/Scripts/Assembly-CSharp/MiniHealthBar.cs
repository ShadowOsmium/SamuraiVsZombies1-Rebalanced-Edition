using UnityEngine;

public class MiniHealthBar
{
	private const float kYOffset = 200f;

	private const float kXOffset = 16f;

	private const float kTimerToDisappear = 2f;

	private Vector2 kBarScale = new Vector2(6f, 1f);

	private Vector2 kGateBarScale = new Vector2(10f, 1f);

	private int kGateBarBorderWidth = 88;

	private int kGateBarBorderHeight = 16;

	private Vector2 kGateBarBorderHotspot = new Vector2(4f, 4f);

	private Rect kVisibleRange = new Rect(-20f, -20f, SUIScreen.width + 40f, SUIScreen.height + 40f);

	private SUISprite mBg;

	private SUISprite mBar;

	private SUISprite mBorder;

	private bool mGateBar;

	private float mProgress = 1f;

	private float mTimerSinceLastChange;

	private float mGateBarFixedY;

	public float importance
	{
		get
		{
			float num = 1f - mProgress;
			if (mBorder != null || mGateBar)
			{
				return num += 1f;
			}
			return num;
		}
	}

	public bool visible
	{
		get
		{
			return mBg.visible;
		}
		set
		{
			if (mBg.visible != value)
			{
				mBg.visible = value;
				mBar.visible = value;
				if (mBorder != null)
				{
					mBorder.visible = value;
				}
			}
		}
	}

	public MiniHealthBar(Color barColor, bool gateVersion)
	{
		if (WeakGlobalInstance<MiniHealthBarRegistrar>.instance != null)
		{
			WeakGlobalInstance<MiniHealthBarRegistrar>.instance.Register(this);
		}
		mBg = new SUISprite(new Color(0f, 0f, 0f, (!gateVersion) ? 0.4f : 1f), 8, 8);
		mBg.priority = ((!gateVersion) ? 0f : 0.03f);
		mBg.hotspotPixels = new Vector2(0f, 0f);
		mBg.scale = ((!gateVersion) ? kBarScale : kGateBarScale);
		mBar = new SUISprite(barColor, 8, 8);
		mBar.priority = ((!gateVersion) ? 0.01f : 0.04f);
		mBar.hotspotPixels = new Vector2(0f, 0f);
		mBar.scale = ((!gateVersion) ? kBarScale : kGateBarScale);
		mGateBar = gateVersion;
		if (gateVersion)
		{
			mBorder = new SUISprite(new Color(1f, 1f, 1f, 1f), kGateBarBorderWidth, kGateBarBorderHeight);
			mBorder.priority = 0.02f;
			mBorder.hotspotPixels = kGateBarBorderHotspot;
		}
		visible = false;
		mTimerSinceLastChange = 2f;
	}

	public void Destroy()
	{
		if (WeakGlobalInstance<MiniHealthBarRegistrar>.instance != null)
		{
			WeakGlobalInstance<MiniHealthBarRegistrar>.instance.Unregister(this);
		}
		mBg.Destroy();
		mBg = null;
		mBar.Destroy();
		mBar = null;
	}

	public void Update(Vector3 worldPos, float progress)
	{
		if (Camera.current == null)
		{
			return;
		}
		if (progress == 0f)
		{
			visible = false;
			return;
		}
		mTimerSinceLastChange += Time.deltaTime;
		float num = Mathf.Clamp(progress, 0f, 1f);
		Vector2 vector = ((!mGateBar) ? kBarScale : kGateBarScale);
		if (mProgress != num)
		{
			mProgress = num;
			mTimerSinceLastChange = 0f;
			mBar.scale = new Vector2(vector.x * mProgress, vector.y);
		}
		Vector2 vector2 = SUIUtils.touchToUser(Camera.current.WorldToScreenPoint(worldPos));
		vector2.y -= 200f;
		vector2.x -= 16f;
		if (mGateBar)
		{
			if (mGateBarFixedY == 0f)
			{
				mGateBarFixedY = vector2.y;
			}
			vector2.y = mGateBarFixedY;
			if (Singleton<PlayModesManager>.instance.gameDirection == PlayModesManager.GameDirection.LeftToRight)
			{
				vector2.x = Mathf.Max(vector2.x, kGateBarBorderHotspot.x);
			}
			else
			{
				vector2.x = Mathf.Min(vector2.x, SUIScreen.width - (float)kGateBarBorderWidth + kGateBarBorderHotspot.x);
			}
		}
		if (kVisibleRange.Contains(vector2) && mTimerSinceLastChange < 2f)
		{
			visible = true;
			SetPosition(vector2);
		}
		else
		{
			visible = false;
		}
	}

	private void SetPosition(Vector2 pos)
	{
		mBg.position = pos;
		mBar.position = pos;
		if (mBorder != null)
		{
			mBorder.position = pos;
		}
	}
}
