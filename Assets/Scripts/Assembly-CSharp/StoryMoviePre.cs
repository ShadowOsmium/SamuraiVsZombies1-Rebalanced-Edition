using UnityEngine;

public class StoryMoviePre : MonoBehaviour
{
	private int mTicsCounter;

	private void Update()
	{
		mTicsCounter++;
		if (mTicsCounter == 2)
		{
			Application.LoadLevel("StoryMovie");
		}
	}
}
