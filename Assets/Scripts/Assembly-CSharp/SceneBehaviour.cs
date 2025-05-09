using UnityEngine;

public class SceneBehaviour : MonoBehaviour
{
	private float mPlayHavenTimer = 1f;

	protected bool mJustShownPlayHaven;

	private SUIScreen mScreen;

	private int mSkipNumUpdate = 3;

	private static SceneBehaviour mUniqueInstance;

	public static SceneBehaviour sceneBehaviourInstance
	{
		get
		{
			return mUniqueInstance;
		}
	}

	public bool justShownPlayHaven
	{
		get
		{
			return mJustShownPlayHaven;
		}
		set
		{
			mJustShownPlayHaven = value;
		}
	}

	private void Awake()
	{
		mUniqueInstance = this;
		Singleton<PlayHavenTowerControl>.instance.ClearHistory();
		SingletonMonoBehaviour<Achievements>.instance.Init();
		SingletonMonoBehaviour<ResourcesManager>.instance.Init();
		ApplicationUtilities.instance.Init();
		mScreen = new SUIScreen();
	}

	protected bool SceneBehaviourUpdate()
	{
		if (mSkipNumUpdate > 0)
		{
			mScreen.UpdateTimeOnly();
			mSkipNumUpdate--;
			if (mSkipNumUpdate == 0)
			{
				SingletonMonoBehaviour<WaitingIconBetweenScenes>.instance.visible = false;
			}
			return true;
		}
		UpdatePlayHavenGameLaunch();
		mScreen.Update();
		return false;
	}

	private void UpdatePlayHavenGameLaunch()
	{
		if (mJustShownPlayHaven)
		{
			mJustShownPlayHaven = false;
			ApplicationUtilities.instance.mustShowGameLauchPlayHavenAds = false;
		}
		else if (mPlayHavenTimer > 0f)
		{
			mPlayHavenTimer -= Time.deltaTime;
		}
		else if (ApplicationUtilities.instance.mustShowGameLauchPlayHavenAds)
		{
			Debug.Log("**** DISPLAY PLAYHAVEN ****");
			ApplicationUtilities.instance.mustShowGameLauchPlayHavenAds = false;
			Singleton<PlayHavenTowerControl>.instance.InvokeContent("game_launch");
		}
	}
}
