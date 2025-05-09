using System;
using UnityEngine;

public class MenuWith3DLevelManager : WeakGlobalInstance<MenuWith3DLevelManager>
{
	private const float kDialogsPriority = 500f;

	private readonly Vector2 kHintPosition = new Vector2(840f, 300f);

	private MenuWith3DLevel mCurrentMenu;

	private EnemiesPreview mEnemiesPreview;

	private SUILayout mLayout;

	private HintPopup mHintPopup;

	private YesNoDialog mNoWaveSelectDialog;

	public string description
	{
		get
		{
			return mHintPopup.text;
		}
		set
		{
			mHintPopup.text = value;
			mHintPopup.visible = value != string.Empty;
		}
	}

	public string descriptionIcon
	{
		get
		{
			return mHintPopup.icon;
		}
		set
		{
			mHintPopup.icon = value;
		}
	}

	public MenuWith3DLevelManager(BoxCollider spawnArea)
	{
		SetUniqueInstance(this);
		mLayout = new SUILayout(Singleton<PlayModesManager>.instance.selectedModeData["layout_MenuWith3D"]);
		((SUIButton)mLayout["waveButton"]).onButtonPressed = OnSelectWaveButtonPress;
		((SUIButton)mLayout["waveButton"]).text = WaveManager.GetNextWaveNumberDisplay();
		if (Singleton<Profile>.instance.wasBasicGameBeaten)
		{
			((SUILabel)mLayout["waveInst"]).visible = true;
			((SUISprite)mLayout["waveNewTag"]).visible = true;
		}
		mHintPopup = new HintPopup();
		mHintPopup.AddIconScalingFilter("charm", new Vector2(1f, 1f));
		mHintPopup.position = kHintPosition;
		mEnemiesPreview = new EnemiesPreview(spawnArea);
		LoadMenu("SelectHelpersImpl");
		RenderSettings.fog = false;
	}

	public void Update()
	{
		if (mNoWaveSelectDialog != null)
		{
			mNoWaveSelectDialog.Update();
			if (mNoWaveSelectDialog.isDone)
			{
				mNoWaveSelectDialog.Destroy();
				mNoWaveSelectDialog = null;
			}
			return;
		}
		mLayout.Update();
		mHintPopup.Update();
		if (mCurrentMenu != null)
		{
			mCurrentMenu.Update();
			mEnemiesPreview.Update();
		}
	}

	public void LoadMenu(string menuClass)
	{
		if (mCurrentMenu != null)
		{
			mCurrentMenu.Destroy();
			mCurrentMenu = null;
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}
		mCurrentMenu = (MenuWith3DLevel)Activator.CreateInstance(Type.GetType(menuClass));
	}

	public void GoToMenu(string newMenu)
	{
		mLayout.AnimateOut();
		WeakGlobalInstance<SUIScreen>.instance.fader.FadeToBlack();
		WeakGlobalInstance<SUIScreen>.instance.fader.onFadingDone = delegate
		{
			Singleton<MenusFlow>.instance.LoadScene(newMenu);
		};
		WeakGlobalInstance<SUIScreen>.instance.inputs.processInputs = false;
	}

	private void OnSelectWaveButtonPress()
	{
		if (Singleton<Profile>.instance.wasBasicGameBeaten)
		{
			if (mCurrentMenu != null)
			{
				mCurrentMenu.SaveChanges();
			}
			GoToMenu("SelectWave");
		}
		else
		{
			mNoWaveSelectDialog = new YesNoDialog(string.Format(Singleton<Localizer>.instance.Get("chapter_screen_locked"), Singleton<Profile>.instance.maxBaseWave), false, false);
			mNoWaveSelectDialog.priority = 500f;
		}
	}
}
