using UnityEngine;

public class PlayModeMenuImpl : SceneBehaviour
{
	private SUILayout mLayout;

	private TutorialPopup mUnlockDialog;

	private void Start()
	{
		WeakGlobalInstance<SUIScreen>.instance.fader.FadeFromBlack();
		mLayout = new SUILayout("Layouts/PlayModesLayout");
		mLayout.AnimateIn();
		((SUIButton)mLayout["backBtn"]).onButtonPressed = delegate
		{
			GotoScene(delegate
			{
				Singleton<MenusFlow>.instance.LoadScene("MainMenu");
			});
		};
		((SUITouchArea)mLayout["mode_classic"]).onAreaTouched = delegate
		{
			SelectMode("classic");
		};
		((SUITouchArea)mLayout["mode_zombies"]).onAreaTouched = delegate
		{
			SelectMode("zombies");
		};
		if (!Singleton<Profile>.instance.zombieModeUnlockedMessageShown)
		{
			Singleton<Profile>.instance.zombieModeUnlockedMessageShown = true;
			Singleton<Profile>.instance.Save();
			ShowUnlockMessage();
		}
	}

	private void Update()
	{
		if (SceneBehaviourUpdate())
		{
			return;
		}
		if (mUnlockDialog != null)
		{
			mUnlockDialog.Update();
			if (WeakGlobalInstance<SUIScreen>.instance.inputs.justTouched)
			{
				mUnlockDialog.Destroy();
				mUnlockDialog = null;
			}
			return;
		}
		mLayout.Update();
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			GotoScene(delegate
			{
				Singleton<MenusFlow>.instance.LoadScene("MainMenu");
			});
		}
	}

	private void GotoScene(OnSUIGenericCallback menuJump)
	{
		WeakGlobalInstance<SUIScreen>.instance.fader.onFadingDone = delegate
		{
			menuJump();
		};
		WeakGlobalInstance<SUIScreen>.instance.fader.FadeToBlack();
		mLayout.AnimateOut();
		WeakGlobalInstance<SUIScreen>.instance.inputs.processInputs = false;
	}

	private void SelectMode(string mode)
	{
		Singleton<PlayModesManager>.instance.selectedMode = mode;
		((SUIButton)mLayout["backBtn"]).onButtonPressed = null;
		((SUITouchArea)mLayout["mode_classic"]).onAreaTouched = null;
		((SUITouchArea)mLayout["mode_zombies"]).onAreaTouched = null;
		if (mode == "classic" && ((Singleton<Profile>.instance.waveToBeat == 1 && Singleton<Profile>.instance.GetWaveLevel(2) == 0) || Singleton<Profile>.instance.readyToStartBonusWave))
		{
			GotoScene(delegate
			{
				WaveManager.LoadNextWaveLevel();
			});
		}
		else
		{
			GotoScene(delegate
			{
				Singleton<MenusFlow>.instance.LoadScene("Store");
			});
		}
	}

	private void ShowUnlockMessage()
	{
		mUnlockDialog = new TutorialPopup();
		mUnlockDialog.ShowPanel(Singleton<Localizer>.instance.Get("unlocked_zombies"));
		mUnlockDialog.SetPanelPosition(new Vector2(SUIScreen.width / 2f, SUIScreen.height / 2f));
	}
}
