using UnityEngine;

public class WaveBanner
{
	private const float kAnimSpeed = 1f;

	private const float kInitialWait = 1.5f;

	private const float kPauseDelay = 1.5f;

	private OnSUIGenericCallback mUpdateFunc;

	private SUILayout mLayout;

	private float mTimer;

	public bool isDone
	{
		get
		{
			return mUpdateFunc == null;
		}
	}

	public WaveBanner()
	{
		mLayout = new SUILayout("Layouts/WaveBanner");
		if (Singleton<EventsManager>.instance.IsEventActive("halloween") && Singleton<GameVersion>.instance.language == "Default")
		{
			if (Singleton<PlayModesManager>.instance.selectedMode == "classic")
			{
				((SUISprite)mLayout["banner"]).texture = "Sprites/Localized/alert_allies_Halloween";
			}
			else
			{
				((SUISprite)mLayout["banner"]).texture = "Sprites/Localized/alert_legion_Halloween";
			}
		}
		else
		{
			((SUISprite)mLayout["banner"]).texture = Singleton<PlayModesManager>.instance.selectedModeData["bannerfile_start"];
		}
		if (MultiLanguages.isMultiLanguages)
		{
			if (Singleton<Profile>.instance.inBonusWave)
			{
				((SUILabel)mLayout["wave"]).text = Singleton<Localizer>.instance.Get("add_bonus_wave");
			}
			else
			{
				((SUILabel)mLayout["wave"]).text = string.Format(Singleton<Localizer>.instance.Get("add_wave"), WaveManager.GetNextWaveNumberDisplay());
			}
		}
		else if (Singleton<Profile>.instance.inBonusWave)
		{
			((SUILabel)mLayout["wave"]).text = "Bonus Wave";
		}
		else
		{
			((SUILabel)mLayout["wave"]).text = "Wave " + WaveManager.GetNextWaveNumberDisplay();
		}
		mLayout.frame = 0f;
		mTimer = 1.5f;
		mUpdateFunc = UpdateInitialWait;
	}

	public void Update()
	{
		if (mUpdateFunc != null)
		{
			mUpdateFunc();
		}
	}

	public void Destroy()
	{
		mLayout.Destroy();
		mLayout = null;
	}

	private void UpdateInitialWait()
	{
		mTimer -= Time.deltaTime;
		if (mTimer <= 0f)
		{
			mLayout.AnimateIn(1f);
			mUpdateFunc = UpdateIntro;
		}
	}

	private void UpdateIntro()
	{
		mLayout.Update();
		if (!mLayout.isAnimating)
		{
			mTimer = 1.5f;
			mUpdateFunc = UpdatePause;
		}
	}

	private void UpdatePause()
	{
		mTimer -= Time.deltaTime;
		if (mTimer <= 0f)
		{
			mLayout.AnimateOut(1f);
			mUpdateFunc = UpdateOutro;
		}
	}

	private void UpdateOutro()
	{
		mLayout.Update();
		if (!mLayout.isAnimating)
		{
			mUpdateFunc = null;
		}
	}

	private void SetRandomBanner()
	{
		int num = RandomRangeInt.between(int.Parse(Singleton<Config>.instance.root.to("randomWaveBanners")["min"]), int.Parse(Singleton<Config>.instance.root.to("randomWaveBanners")["max"]));
		((SUISprite)mLayout["banner"]).texture = "Sprites/Localized/wave_start_" + string.Format("{0:00}", num);
	}
}
