using UnityEngine;

public class StoryMovie : MonoBehaviour
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
			Application.LoadLevel("MainMenu");
			break;
		}
	}

	private void StartMovie()
	{
		string text = "SvZRough.mp4";
		Debug.Log("Playing movie: " + text);
		//Handheld.PlayFullScreenMovie(text, Color.black, FullScreenMovieControlMode.CancelOnTouch, FullScreenMovieScalingMode.AspectFit);
	}
}
