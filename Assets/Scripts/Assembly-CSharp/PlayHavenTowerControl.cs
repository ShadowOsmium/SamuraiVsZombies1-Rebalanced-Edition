using PlayHaven;
using UnityEngine;

public class PlayHavenTowerControl : Singleton<PlayHavenTowerControl>
{
	private bool mInIAPMode;

	public bool inIAPMode
	{
		get
		{
			return mInIAPMode;
		}
		set
		{
			mInIAPMode = value;
		}
	}

	public void InvokeContent(string contentType)
	{
		if (contentType == "game_launch" && mInIAPMode)
		{
			Debug.Log("*** PlayHaven: ignoring 'game_launch' event as we are in IAP.");
			return;
		}
		if (contentType == "game_launch" && !Singleton<Profile>.instance.tutorialIsComplete)
		{
			Debug.Log(string.Format("*** PlayHaven: ignoring '{0}' event as the tutorial is not over with yet.", contentType));
			return;
		}
		SceneBehaviour.sceneBehaviourInstance.justShownPlayHaven = true;
		if (contentType == "more_games" && ApplicationUtilities.IsBuildType("amazon"))
		{
			Application.OpenURL("http://gcs.glu.com/gcs/fe?wsid=2&cid=157873&src=svz_app");
		}
		Debug.Log("*** PlayHaven Request Sent: " + contentType);
	}

	public void NotifyIAPEvent(string productIdentifier, PurchaseResolution eventID)
	{
	}

	public void ClearHistory()
	{
		mInIAPMode = false;
	}
}
