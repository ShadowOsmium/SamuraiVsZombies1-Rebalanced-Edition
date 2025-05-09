using UnityEngine;

public class IntroMovie : MonoBehaviour
{
	private int mStaggeredOperationsPerUpdates;

	private void Update()
	{
		mStaggeredOperationsPerUpdates++;
		switch (mStaggeredOperationsPerUpdates)
		{
		case 4:
			NUF.AllowPortrait(false);
			break;
		case 8:
			StartMovie();
			break;
		case 15:
			NUF.AllowPortrait(true);
			break;
		case 21:
			Debug.Log("IntroMovie: main menu load");
			Application.LoadLevel("StoryMoviePre");
			break;
		}
	}

	private void StartMovie()
	{
		string text = "1024x768";
		switch (Screen.width)
		{
		case 640:
			text = "960x640";
			break;
		case 320:
			text = "480x320";
			break;
		}
		string text2 = "Glu_logo_" + text + ".mp4";
		Debug.Log("Playing movie:" + text2);
	}
}
