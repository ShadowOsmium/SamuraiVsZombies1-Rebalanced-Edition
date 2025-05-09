using System.Collections;
using UnityEngine;

public class PrestitialScreenImpl : SceneBehaviour
{
	private SUIScreen mScreen;

	private SUILayout mLayout;

	private SUIButton mGold;

	private SUIButton mSilver;

	private SUIButton mBenefits;

	private int mSkipNumUpdate = 3;

	public AudioClip ClickSFX;

	private IEnumerator Start()
	{
		mScreen = new SUIScreen();
		mScreen.fader.FadeFromBlack();
		mLayout = new SUILayout("Layouts/PrestitialScreen");
		mLayout.AnimateIn();
		mGold = (SUIButton)mLayout["goldbutton"];
		mGold.onButtonPressed = delegate
		{
			base.GetComponent<AudioSource>().PlayOneShot(ClickSFX);
			if (Debug.isDebugBuild)
			{
				Debug.Log("Gold Pressed...");
			}
			StartCoroutine(LoadScene("StoryMoviePre"));
		};
		mSilver = (SUIButton)mLayout["silverbutton"];
		mSilver.onButtonPressed = delegate
		{
			base.GetComponent<AudioSource>().PlayOneShot(ClickSFX);
			if (Debug.isDebugBuild)
			{
				Debug.Log("Silver Pressed...");
			}
			StartCoroutine(LoadScene("StoryMoviePre"));
		};
		mBenefits = (SUIButton)mLayout["benefitsbutton"];
		mBenefits.onButtonPressed = delegate
		{
			base.GetComponent<AudioSource>().PlayOneShot(ClickSFX);
			if (Debug.isDebugBuild)
			{
				Debug.Log("OnBenefit");
			}
			Application.OpenURL("http://m.glu.com/android/vip-club?navbar=N");
		};
		yield return new WaitForSeconds(0.001f);
	}

	private IEnumerator LoadScene(string sceneName)
	{
		yield return new WaitForSeconds(0.01f);
		Debug.Log("PrestitialScreen: " + sceneName);
		Application.LoadLevel(sceneName);
	}

	private void Update()
	{
		if (mSkipNumUpdate > 0)
		{
			mScreen.UpdateTimeOnly();
			mSkipNumUpdate--;
			return;
		}
		mScreen.Update();
		mLayout.Update();
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			Application.LoadLevel("StoryMoviePre");
		}
	}
}
