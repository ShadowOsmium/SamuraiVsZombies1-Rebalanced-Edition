using UnityEngine;

public class ReviveSFX : MonoBehaviour
{
	private float mAudioVolume;

	private float mMusicVolume;

	private bool mAttemptingToQuit;

	private void Start()
	{
		mAudioVolume = SoundThemeManager.Instance.globalVolume;
		SoundThemeManager.Instance.globalVolume = 0f;
		base.GetComponent<AudioSource>().Play();
		GameObject gameObject = GameObject.Find("Main Camera");
		mMusicVolume = gameObject.GetComponent<AudioSource>().volume;
		gameObject.GetComponent<AudioSource>().volume = 0f;
		mAttemptingToQuit = false;
	}

	private void Update()
	{
	}

	private void OnApplicationQuit()
	{
		mAttemptingToQuit = true;
	}

	private void OnDestroy()
	{
		if (!mAttemptingToQuit)
		{
			SoundThemeManager.Instance.globalVolume = mAudioVolume;
			GameObject gameObject = GameObject.Find("Main Camera");
			gameObject.GetComponent<AudioSource>().volume = mMusicVolume;
		}
	}
}
