using UnityEngine;

public class MenuWith3DLevelImpl : SceneBehaviour
{
	public BoxCollider spawnArea;

	private MenuWith3DLevelManager mManager;

	private void Start()
	{
		WeakGlobalInstance<SUIScreen>.instance.fader.speed = 1f;
		WeakGlobalInstance<SUIScreen>.instance.fader.FadeFromBlack();
		mManager = new MenuWith3DLevelManager(spawnArea);
		ApplicationUtilities.instance.ShowAds((ApplicationUtilities.AdPosition)17);
	}

	private void OnDestroy()
	{
		ApplicationUtilities.instance.HideAds();
	}

	private void Update()
	{
		if (!SceneBehaviourUpdate())
		{
			mManager.Update();
		}
	}

	private void OnGUI()
	{
	}
}
