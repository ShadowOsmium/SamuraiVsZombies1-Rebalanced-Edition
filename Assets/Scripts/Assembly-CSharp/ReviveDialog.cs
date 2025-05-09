using System.Collections.Generic;
using UnityEngine;

public class ReviveDialog : IDialog
{
	private const int kPackNum = 5;

	private const int kPackCostMultiplier = 4;

	private SUILayout mLayout;

	private bool mDone;

	private StoreData.Item mReviveItemDesc;

	private SpoilsDisplay mSpoilsDisplay;

	public bool isBlocking
	{
		get
		{
			return true;
		}
	}

	public bool isDone
	{
		get
		{
			return mDone && !mLayout.isAnimating;
		}
	}

	public float priority
	{
		get
		{
			return mLayout.basePriority;
		}
		set
		{
			mLayout.basePriority = value;
		}
	}

	public ReviveDialog()
	{
		List<SpoilsDisplay.Entry> entryList = SpoilsDisplay.GetEntryList();
		if (entryList.Count == 0)
		{
			mLayout = new SUILayout(Singleton<PlayModesManager>.instance.selectedModeData["layout_ReviveDialogSmall"]);
			((SUILabel)mLayout["title"]).text = Singleton<Localizer>.instance.Get("revive_title_nospoil");
		}
		else
		{
			mLayout = new SUILayout(Singleton<PlayModesManager>.instance.selectedModeData["layout_ReviveDialog"]);
		}
		mSpoilsDisplay = new SpoilsDisplay(mLayout, entryList, "default18", new Vector2(SUIScreen.width / 2f, SUIScreen.height / 2f - 74f), 1000f, 5, 144f, string.Empty);
		if (Singleton<Profile>.instance.GetNumPotions(Singleton<PlayModesManager>.instance.revivePotionID) > 0)
		{
			SUIButton sUIButton = (SUIButton)mLayout["yesBtn"];
			SUIButton sUIButton2 = (SUIButton)mLayout["yesBtn2"];
			sUIButton.onButtonPressed = delegate
			{
				Close(true);
				DoRevive();
			};
			sUIButton.position = new Vector2(sUIButton.position.x, (sUIButton.position.y + sUIButton2.position.y) / 2f);
		}
		else
		{
			SUIButton sUIButton3 = (SUIButton)mLayout["yesBtn"];
			SUIButton sUIButton4 = (SUIButton)mLayout["yesBtn2"];
			mReviveItemDesc = InGameStoreDialog.GetStoreItemDesc(Singleton<PlayModesManager>.instance.revivePotionID, false);
			sUIButton3.onButtonPressed = delegate
			{
				Close(DoPurchaseRevive(0));
			};
			sUIButton4.onButtonPressed = delegate
			{
				Close(DoPurchaseRevive(1));
			};
			sUIButton4.visible = true;
			DisplayReviveCostOnButton();
		}
		((SUIButton)mLayout["back"]).onButtonPressed = delegate
		{
			Close(true);
		};
		mLayout.AnimateIn();
		DrawTexts();
	}

	public void Destroy()
	{
		if (mSpoilsDisplay != null)
		{
			mSpoilsDisplay.Destroy();
			mSpoilsDisplay = null;
		}
		if (mLayout != null)
		{
			mLayout.Destroy();
			mLayout = null;
		}
	}

	public void Update()
	{
		mLayout.Update();
		if (mSpoilsDisplay != null)
		{
			mSpoilsDisplay.Update();
		}
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			Close(true);
		}
	}

	private void DisplayReviveCostOnButton()
	{
		if (!(mReviveItemDesc == null))
		{
			SUIButton sUIButton = (SUIButton)mLayout["yesBtn"];
			SUIButton sUIButton2 = (SUIButton)mLayout["yesBtn2"];
			sUIButton.text = " ";
			sUIButton2.text = " ";
			Cost cost = new Cost(mReviveItemDesc.cost.soft * 4, mReviveItemDesc.cost.hard * 4, 0f);
			SetButtonCost(sUIButton, cost, 5, true);
			SetButtonCost(sUIButton2, mReviveItemDesc.cost, 1, false);
		}
	}

	private void SetButtonCost(SUIButton btn, Cost cost, int count, bool showSale)
	{
		SUISprite sUISprite = new SUISprite("Sprites/Icons/potion_revive");
		sUISprite.position = btn.position + new Vector2(-84f, 0f);
		sUISprite.scale = new Vector2(0.6f, 0.6f);
		sUISprite.priority = 5000f;
		SUILayout.ObjectData objectData = new SUILayout.ObjectData();
		objectData.obj = sUISprite;
		objectData.animAlpha = new SUILayoutAnim.AnimFloat(1f, 0f, new SUILayout.NormalRange(0f, 1f), Ease.Linear);
		mLayout.Add("reviveIcon" + count, objectData);
		SUILabel sUILabel = new SUILabel("default32");
		sUILabel.position = btn.position + new Vector2(-46f, 0f);
		sUILabel.anchor = TextAnchor.MiddleLeft;
		sUILabel.shadowOffset = new Vector2(2f, 2f);
		sUILabel.shadowColor = Color.black;
		sUILabel.text = string.Format("x {0}", count);
		sUILabel.priority = 5000f;
		SUILayout.ObjectData objectData2 = new SUILayout.ObjectData();
		objectData2.obj = sUILabel;
		objectData2.animAlpha = new SUILayoutAnim.AnimFloat(1f, 0f, new SUILayout.NormalRange(0f, 1f), Ease.Linear);
		mLayout.Add("reviveCounter" + count, objectData2);
		CostDisplay costDisplay = new CostDisplay("default32");
		costDisplay.SetCost(cost);
		costDisplay.position = btn.position + new Vector2(30f, -16f);
		costDisplay.priority = 5000f;
		costDisplay.visible = true;
		SUILayout.ObjectData objectData3 = new SUILayout.ObjectData();
		objectData3.obj = costDisplay;
		objectData3.animAlpha = new SUILayoutAnim.AnimFloat(1f, 0f, new SUILayout.NormalRange(0f, 1f), Ease.Linear);
		mLayout.Add("reviveCost" + count, objectData3);
		if (showSale)
		{
			NoveltyWidget noveltyWidget = new NoveltyWidget();
			noveltyWidget.texture = "Sprites/Localized/on_sale";
			noveltyWidget.position = btn.position + new Vector2(160f, -24f);
			noveltyWidget.priority = 5000f;
			SUILayout.ObjectData objectData4 = new SUILayout.ObjectData();
			objectData4.obj = noveltyWidget;
			objectData4.animAlpha = new SUILayoutAnim.AnimFloat(1f, 0f, new SUILayout.NormalRange(0f, 1f), Ease.Linear);
			mLayout.Add("reviveSaleTicker" + count, objectData4);
		}
	}

	private void DrawTexts()
	{
		switch (Singleton<Profile>.instance.GetNumPotions(Singleton<PlayModesManager>.instance.revivePotionID))
		{
		case 0:
			((SUILabel)mLayout["question"]).text = Singleton<Localizer>.instance.Get("revive_purchase");
			break;
		case 1:
			((SUILabel)mLayout["question"]).text = Singleton<Localizer>.instance.Get("revive_use1");
			break;
		default:
			((SUILabel)mLayout["question"]).text = string.Format(Singleton<Localizer>.instance.Get("revive_useX"), Singleton<Profile>.instance.GetNumPotions(Singleton<PlayModesManager>.instance.revivePotionID).ToString());
			break;
		}
	}

	private void Close(bool fadeDialogHandler)
	{
		((SUIButton)mLayout["yesBtn"]).onButtonPressed = null;
		((SUIButton)mLayout["yesBtn2"]).onButtonPressed = null;
		((SUIButton)mLayout["back"]).onButtonPressed = null;
		mDone = true;
		mLayout.AnimateOut();
		if (fadeDialogHandler && WeakGlobalInstance<DialogHandler>.instance != null)
		{
			WeakGlobalInstance<DialogHandler>.instance.FadeOut();
		}
	}

	private void DoRevive()
	{
		SingletonMonoBehaviour<Achievements>.instance.Award("SAMUZOMBIE_ACHIEVE_NO_WAY");
		string text = string.Empty;
		List<CollectedPresent> presents = CollectableManager.currentWaveSpoils.presents;
		foreach (CollectedPresent item in presents)
		{
			text = text + " Present[" + item.contents + "]";
		}
		int eventValue = WeakGlobalInstance<WaveManager>.instance.enemiesKilledSoFar * 100 / WeakGlobalInstance<WaveManager>.instance.totalEnemies;
		Singleton<Analytics>.instance.LogEvent("Revived", "Spoils: Coins[" + CollectableManager.currentWaveSpoils.coins + "] Gems[" + CollectableManager.currentWaveSpoils.gems + "] Balls[" + CollectableManager.currentWaveSpoils.balls + "]" + text, eventValue);
		Singleton<Profile>.instance.SetNumPotions(Singleton<PlayModesManager>.instance.revivePotionID, Mathf.Max(0, Singleton<Profile>.instance.GetNumPotions(Singleton<PlayModesManager>.instance.revivePotionID) - 1));
		Singleton<Profile>.instance.Save();
		WeakGlobalSceneBehavior<InGameImpl>.instance.hero.Revive();
	}

	private bool DoPurchaseRevive(int btnIndex)
	{
		if (mReviveItemDesc == null)
		{
			Debug.Log("WARNING: Some logical flow error occured.  This should not be displayed.");
			return true;
		}
		Cost cost = new Cost(mReviveItemDesc.cost.soft, mReviveItemDesc.cost.hard, 0f);
		if (btnIndex == 0)
		{
			cost.soft *= 4;
			cost.hard *= 4;
		}
		if (cost.canAfford)
		{
			cost.Spend(mReviveItemDesc.id);
			if (btnIndex == 0)
			{
				for (int i = 0; i < 5; i++)
				{
					mReviveItemDesc.Apply();
				}
			}
			else
			{
				mReviveItemDesc.Apply();
			}
			DoRevive();
		}
		else
		{
			ShowPurchaseCurrencyDialog();
		}
		return false;
	}

	private void ShowPurchaseCurrencyDialog()
	{
		float dialogPriority = priority;
		WeakGlobalInstance<DialogHandler>.instance.PushCreator(() => new ReviveDialog
		{
			priority = dialogPriority
		});
		WeakGlobalInstance<DialogHandler>.instance.PushCreator(() => new PurchaseCurrencyDialog(mReviveItemDesc.cost)
		{
			priority = 1000f
		});
	}
}
