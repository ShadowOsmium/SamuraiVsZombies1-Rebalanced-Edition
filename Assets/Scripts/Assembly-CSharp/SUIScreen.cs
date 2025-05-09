using UnityEngine;

public class SUIScreen : WeakGlobalInstance<SUIScreen>, SUIProcess
{
	private const float kMaxDeltaTime = 0.1f;

	private AutoScaler mAutoScaler;

	private SUIInputManager mInputManager;

	private SUIFader mFader;

	private static bool mIs_iPhone4;

	private static bool mIs_iPad3;

	private static float mWidth;

	private static float mHeight;

	private static double mLastRecordedTime;

	private static double mCurrentDeltaTime;

	private static int m_screenDpi;

	private static float m_screenDensity;

	public AutoScaler autoScaler
	{
		get
		{
			return mAutoScaler;
		}
	}

	public SUIInputManager inputs
	{
		get
		{
			return mInputManager;
		}
	}

	public SUIFader fader
	{
		get
		{
			return mFader;
		}
	}

	public static float width
	{
		get
		{
			return mWidth;
		}
	}

	public static float height
	{
		get
		{
			return mHeight;
		}
	}

	public static bool isDevice_iPhone4
	{
		get
		{
			return mIs_iPhone4;
		}
	}

	public static bool isDevice_iPad3
	{
		get
		{
			return mIs_iPad3;
		}
	}

	public static bool isDeviceRetina
	{
		get
		{
			return m_screenDpi >= 300;
		}
	}

	public static float deltaTime
	{
		get
		{
			return Mathf.Min((float)mCurrentDeltaTime, 0.1f);
		}
	}

	public static bool aspectRatioStandard
	{
		get
		{
			return width / height == (float)Screen.width / (float)Screen.height;
		}
	}

	public static float density
	{
		get
		{
			return m_screenDensity;
		}
	}

	public SUIScreen()
	{
		SetUniqueInstance(this);
		if (mLastRecordedTime == 0.0)
		{
			mLastRecordedTime = Time.realtimeSinceStartup;
		}
		Time.timeScale = 1f;
		mAutoScaler = new AutoScaler(new Vector2(mWidth, mHeight));
		mInputManager = new SUIInputManager();
		mFader = new SUIFader();
	}

	static SUIScreen()
	{
		mWidth = 1024f;
		mHeight = 768f;
		mCurrentDeltaTime = 0.0;
		mIs_iPad3 = Screen.width == 2048 && Screen.height == 1536;
		mIs_iPhone4 = Screen.width == 960 && Screen.height == 640;
		m_screenDpi = 0;
		m_screenDensity = 1f;
	}

	public void Update()
	{
		UpdateTimeOnly();
		mInputManager.Update();
		mFader.Update();
	}

	public void UpdateTimeOnly()
	{
		mCurrentDeltaTime = (double)Time.realtimeSinceStartup - mLastRecordedTime;
		mLastRecordedTime = Time.realtimeSinceStartup;
	}

	public void Destroy()
	{
		mFader.Destroy();
		mFader = null;
	}

	public void EditorRenderOnGUI()
	{
	}
}
