using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebriefingImpl : SceneBehaviour
{
	private Vector2 kIconAnimPositionOffset = new Vector2(0f, 100f);

	private SUILayout mLayout;

	private SpoilsDisplay mSpoilsDisplay;

	private bool mWasInBonusWave;

	private TwoOptionDialog m_rateDialog;

	private YesNoDialog m_rateAwardDialog;

	private static Vector2 GetUnlockIconPos(int index, int total)
	{
		float num = SUIScreen.width / 2f - (float)((total - 1) * 200 / 2);
		num += (float)(200 * index);
		return new Vector2(num, 620f);
	}

	private static Vector2 GetUnlockTitlePos(int index, int total)
	{
		Vector2 unlockIconPos = GetUnlockIconPos(index, total);
		unlockIconPos.y += 60f;
		return unlockIconPos;
	}

	private static SUILayout.NormalRange GetAnimRange(int index)
	{
		return new SUILayout.NormalRange(Mathf.Clamp(0.2f * (float)index, 0f, 1f), Mathf.Clamp(0.4f + 0.2f * (float)index, 0f, 1f));
	}

	private void Start()
	{
		WeakGlobalInstance<SUIScreen>.instance.fader.speed = 1f;
		WeakGlobalInstance<SUIScreen>.instance.fader.FadeFromBlack();
		mWasInBonusWave = Singleton<Profile>.instance.inBonusWave;
		if (mWasInBonusWave && Singleton<PlayStatistics>.instance.stats.lastWaveWon && Singleton<PlayModesManager>.instance.selectedMode == "classic")
		{
			Singleton<Profile>.instance.freeBoosterPacks++;
		}
		string text = Singleton<PlayModesManager>.instance.selectedModeData["layout_DebriefingMenu"];
		mLayout = new SUILayout(text);
		if (Singleton<PlayStatistics>.instance.stats.lastWaveWon)
		{
			mSpoilsDisplay = new SpoilsDisplay(mLayout, null, "default18", new Vector2(SUIScreen.width / 2f, SUIScreen.height / 2f - 30f), 2f, 6, 160f, string.Empty);
			Singleton<EventsManager>.instance.CashInRewards();
			if (Singleton<PlayStatistics>.instance.stats.lastWavePlayed == 3 && Singleton<PlayStatistics>.instance.stats.lastWaveLevel == 1 && Singleton<PlayModesManager>.instance.selectedMode == "classic")
			{
				Singleton<PlayHavenTowerControl>.instance.InvokeContent("tutorial_end");
			}
			else
			{
				int lastWavePlayed = Singleton<PlayStatistics>.instance.stats.lastWavePlayed;
				if ((lastWavePlayed < 10 && Singleton<PlayStatistics>.instance.stats.lastWaveLevel == 1) || lastWavePlayed % 10 == 0)
				{
					Singleton<PlayHavenTowerControl>.instance.InvokeContent("victory_wave_" + lastWavePlayed);
				}
			}
		}
		mLayout.AnimateIn();
		((SUIButton)mLayout["continueBtn"]).onButtonPressed = OnNextMenu;
		DisplayStats();
		DisplayUnlockedFeatures();
	}

	private void Update()
	{
		if (SceneBehaviourUpdate())
		{
			return;
		}
		mLayout.Update();
		if (mSpoilsDisplay != null)
		{
			mSpoilsDisplay.Update();
		}
		if (m_rateDialog != null)
		{
			m_rateDialog.Update();
			if (m_rateDialog.isDone)
			{
				m_rateDialog.Destroy();
				m_rateDialog = null;
			}
		}
		else if (m_rateAwardDialog != null)
		{
			m_rateAwardDialog.Update();
			if (m_rateAwardDialog.isDone)
			{
				m_rateAwardDialog.Destroy();
				m_rateAwardDialog = null;
			}
		}
		else if (Input.GetKeyUp(KeyCode.Escape))
		{
			OnNextMenu();
		}
	}

	private void DisplayStats()
	{
		string text = ((!mWasInBonusWave) ? string.Format(Singleton<Localizer>.instance.Get("debrief_title") + " ", WaveManager.GetWaveNumberDisplay(Singleton<PlayStatistics>.instance.stats.lastWavePlayed, Singleton<PlayStatistics>.instance.stats.lastWaveLevel)) : (Singleton<Localizer>.instance.Get("debrief_title_bonus") + " "));
		if (Singleton<PlayStatistics>.instance.stats.lastWaveWon)
		{
			if (!Singleton<PlayStatistics>.instance.stats.heroUsedHisMeleeAttack && !Singleton<PlayStatistics>.instance.stats.heroUsedHisRangedAttack && !mWasInBonusWave)
			{
				SingletonMonoBehaviour<Achievements>.instance.Award("SAMUZOMBIE_ACHIEVE_COWARD");
			}
			if (!Singleton<PlayStatistics>.instance.stats.summonedTroopOtherThanFarmerOrSwordsmith && Singleton<PlayStatistics>.instance.stats.summonedTroopFarmer && Singleton<PlayStatistics>.instance.stats.summonedTroopSwordsmith)
			{
				SingletonMonoBehaviour<Achievements>.instance.Award("SAMUZOMBIE_ACHIEVE_THEY_GOT_THIS");
			}
			if (!Singleton<PlayStatistics>.instance.stats.heroInvokedHelpers && (Singleton<PlayStatistics>.instance.stats.lastWavePlayed > 2 || Singleton<Profile>.instance.wasBasicGameBeaten))
			{
				SingletonMonoBehaviour<Achievements>.instance.Award("SAMUZOMBIE_ACHIEVE_ALL_WE_NEED");
			}
			switch (Singleton<PlayStatistics>.instance.stats.lastWavePlayed)
			{
			case 25:
				SingletonMonoBehaviour<Achievements>.instance.Award("SAMUZOMBIE_ACHIEVE_HALFWAY_THERE");
				break;
			case 50:
				SingletonMonoBehaviour<Achievements>.instance.Award("SAMUZOMBIE_ACHIEVE_THIS_LAND_IS_SAFE");
				break;
			}
			if (!Singleton<PlayStatistics>.instance.stats.gateWasDamaged)
			{
				Singleton<Profile>.instance.wavesWithZeroGateDamage++;
			}
			if (!Singleton<Profile>.instance.wasBasicGameBeaten && !Singleton<Profile>.instance.neverShowRateMeAlertAgain)
			{
				int lastWavePlayed = Singleton<PlayStatistics>.instance.stats.lastWavePlayed;
				if (lastWavePlayed == 5 || lastWavePlayed == 10 || lastWavePlayed == 20 || lastWavePlayed == 30 || lastWavePlayed == 40)
				{
					if (ApplicationUtilities.IsBuildType("amazon"))
					{
						m_rateDialog = new TwoOptionDialog(SUILayoutConv.GetFormattedText(Singleton<Localizer>.instance.Get("like_our_game_android_amazon")), false, SUILayoutConv.GetFormattedText(Singleton<Localizer>.instance.Get("rate_it_android")), delegate
						{
							OnClickRateDialog(true);
						}, Singleton<Localizer>.instance.Get("cancel_rating_android"), delegate
						{
							OnClickRateDialog(false);
						}, null);
						m_rateDialog.priority = 500f;
					}
					else if (ApplicationUtilities.IsBuildType("google"))
					{
						m_rateDialog = new TwoOptionDialog(SUILayoutConv.GetFormattedText(Singleton<Localizer>.instance.Get("like_our_game_android")), false, SUILayoutConv.GetFormattedText(Singleton<Localizer>.instance.Get("rate_it_android")), delegate
						{
							OnClickRateDialog(true);
						}, Singleton<Localizer>.instance.Get("cancel_rating_android"), delegate
						{
							OnClickRateDialog(false);
						}, null);
						m_rateDialog.priority = 500f;
					}
				}
			}
			text += Singleton<Localizer>.instance.Get("debrief_victory");
		}
		else
		{
			text += Singleton<Localizer>.instance.Get("debrief_lost");
		}
		((SUILabel)mLayout["title"]).text = text;
		int num = 0;
		((SUILabel)mLayout["awardedValue"]).text = Singleton<PlayStatistics>.instance.stats.lastWaveCoinsAward.ToString();
		num += Singleton<PlayStatistics>.instance.stats.lastWaveCoinsAward;
		((SUILabel)mLayout["collectedValue"]).text = Singleton<PlayStatistics>.instance.stats.lastWaveCoinsCollected.ToString();
		num += Singleton<PlayStatistics>.instance.stats.lastWaveCoinsCollected;
		Singleton<Analytics>.instance.LogEvent("CoinsGained", Singleton<Profile>.instance.waveToBeat.ToString(), Singleton<PlayStatistics>.instance.stats.lastWaveCoinsCollected);
		((SUILabel)mLayout["totalValue"]).text = num.ToString();
	}

	private static void OnNeverShowAlertAgain()
	{
		Singleton<Profile>.instance.neverShowRateMeAlertAgain = true;
		Singleton<Profile>.instance.Save();
	}

	private void DisplayUnlockedFeatures()
	{
		List<StoreData.Item> previousStoreItems = Singleton<PlayStatistics>.instance.stats.previousStoreItems;
		List<StoreData.Item> list = StoreAvailability.GetList();
		for (int num = previousStoreItems.Count - 1; num >= 0; num--)
		{
			if (previousStoreItems[num].locked)
			{
				previousStoreItems.RemoveAt(num);
			}
		}
		for (int num2 = list.Count - 1; num2 >= 0; num2--)
		{
			if (list[num2].locked)
			{
				list.RemoveAt(num2);
			}
		}
		foreach (StoreData.Item item2 in previousStoreItems)
		{
			for (int num3 = list.Count - 1; num3 >= 0; num3--)
			{
				StoreData.Item item = list[num3];
				if (item2.id == item.id)
				{
					list.RemoveAt(num3);
					break;
				}
			}
		}
		SetNovelties(list);
		if (list.Count == 0)
		{
			((SUILabel)mLayout["unlockedTitle"]).visible = false;
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			AddUnlockedToLayout(i, list.Count, list[i].icon, list[i].unlockTitle);
		}
	}

	private void SetNovelties(List<StoreData.Item> itemsList)
	{
		List<string> list = new List<string>();
		foreach (StoreData.Item items in itemsList)
		{
			list.Add(items.id);
		}
		Singleton<Profile>.instance.novelties = list;
	}

	private void AddUnlockedToLayout(int index, int total, string iconFile, string title)
	{
		SUISprite sUISprite = new SUISprite(iconFile);
		sUISprite.priority = 1f;
		sUISprite.position = GetUnlockIconPos(index, total);
		SUILayout.ObjectData objectData = new SUILayout.ObjectData();
		objectData.obj = sUISprite;
		objectData.animPosition = new SUILayoutAnim.AnimVector2(sUISprite.position, sUISprite.position + kIconAnimPositionOffset, GetAnimRange(index), Ease.BackOut);
		objectData.animAlpha = new SUILayoutAnim.AnimFloat(1f, 0f, GetAnimRange(index), Ease.Linear);
		mLayout.Add("unlockIcon" + index, objectData);
		SUILabel sUILabel = new SUILabel("default18");
		sUILabel.priority = 1f;
		sUILabel.maxWidth = 185;
		sUILabel.position = GetUnlockTitlePos(index, total);
		sUILabel.shadowColor = Color.black;
		sUILabel.shadowOffset = new Vector2(2f, 2f);
		sUILabel.alignment = TextAlignment.Center;
		sUILabel.anchor = TextAnchor.UpperCenter;
		sUILabel.text = title;
		SUILayout.ObjectData objectData2 = new SUILayout.ObjectData();
		objectData2.obj = sUILabel;
		objectData2.animPosition = new SUILayoutAnim.AnimVector2(sUILabel.position, sUILabel.position + kIconAnimPositionOffset, GetAnimRange(index), Ease.BackOut);
		objectData2.animAlpha = new SUILayoutAnim.AnimFloat(1f, 0f, GetAnimRange(index), Ease.Linear);
		mLayout.Add("unlockLabel" + index, objectData2);
		if (index == 0)
		{
			Singleton<SUISoundManager>.instance.Play("Sounds/UI_Debrief_Unlock_ST_OS_01", sUISprite.gameObject);
		}
	}

	private void OnNextMenu()
	{
		mLayout.AnimateOut(WeakGlobalInstance<SUIScreen>.instance.fader.speed);
		WeakGlobalInstance<SUIScreen>.instance.fader.onFadingDone = delegate
		{
			Singleton<Profile>.instance.ClearBonusWaveData();
			if (Singleton<Profile>.instance.readyToStartBonusWave)
			{
				WaveManager.LoadNextWaveLevel();
			}
			else if (Singleton<Profile>.instance.zombieModeUnlocked && !Singleton<PlayStatistics>.instance.stats.zombieModeStartedUnlocked && !mWasInBonusWave && !Singleton<Profile>.instance.zombieModeUnlockedMessageShown)
			{
				Singleton<MenusFlow>.instance.LoadScene("PlayModeMenu");
			}
			else if (Singleton<Profile>.instance.freeBoosterPacks > 0 && Singleton<PlayModesManager>.instance.selectedMode == "classic")
			{
				Singleton<MenusFlow>.instance.LoadScene("BoosterPackStore");
			}
			else
			{
				Singleton<MenusFlow>.instance.LoadScene("Store");
			}
		};
		WeakGlobalInstance<SUIScreen>.instance.fader.FadeToBlack();
		WeakGlobalInstance<SUIScreen>.instance.inputs.processInputs = false;
	}

	private void OnClickRateDialog(bool rated)
	{
		if (rated)
		{
			OnNeverShowAlertAgain();
			if (!ApplicationUtilities.IsBuildType("amazon"))
			{
				StartCoroutine(AwardGameCurrencyAfterRating());
			}
			Application.OpenURL(ApplicationUtilities.MarketURL);
		}
		m_rateDialog.Close();
	}

	private IEnumerator AwardGameCurrencyAfterRating()
	{
		yield return new WaitForSeconds(2f);
		m_rateAwardDialog = new YesNoDialog(SUILayoutConv.GetFormattedText(Singleton<Localizer>.instance.Get("rating_award")), false, delegate
		{
		}, null);
		m_rateAwardDialog.priority = 500f;
		ApplicationUtilities.GWalletBalance(5, "Incentivized Rate Me", "CREDIT_IN_GAME_AWARD");
		Singleton<Profile>.instance.Save();
	}
}
