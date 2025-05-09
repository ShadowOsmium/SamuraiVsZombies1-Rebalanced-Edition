using System;
using System.Collections.Generic;
using UnityEngine;

public class HUDHelpersBar
{
	private SUILayout mLayoutRef;

	private List<HUDIconButton> mHelperHUDs = new List<HUDIconButton>();

	public HUDHelpersBar(SUILayout layoutRef)
	{
		mLayoutRef = layoutRef;
	}

	private static Vector2 GetIconPosition(int index)
	{
		return new Vector2(index * 110 + 70, SUIScreen.height - 70f);
	}

	public void Update()
	{
		if (!Application.isMobilePlatform)
		{
			for (int i = 0; i < 5; i++)
			{
				if (mHelperHUDs.Count > i && Input.GetKeyDown((KeyCode)Enum.Parse(typeof(KeyCode), "Alpha" + (i + 1))))
				{
					mHelperHUDs[i].Trigger();
				}
			}
		}
		if (WeakGlobalInstance<Smithy>.instance == null)
		{
			return;
		}
		if (mHelperHUDs.Count == 0)
		{
			CreateHUD();
		}
		foreach (HUDIconButton mHelperHUD in mHelperHUDs)
		{
			mHelperHUD.UpdateCoolDown();
		}
	}

	public bool IsTouchZone(Vector2 pos)
	{
		foreach (HUDIconButton mHelperHUD in mHelperHUDs)
		{
			if (mHelperHUD.IsTouchZone(pos))
			{
				return true;
			}
		}
		return false;
	}

	private void DestroyHUD()
	{
		foreach (HUDIconButton mHelperHUD in mHelperHUDs)
		{
			mHelperHUD.Destroy();
		}
		mHelperHUDs.Clear();
	}

	private void CreateHUD()
	{
		DestroyHUD();
		if (WeakGlobalInstance<Smithy>.instance.numTypes <= 0)
		{
			return;
		}
		for (int i = 0; i < WeakGlobalInstance<Smithy>.instance.numTypes; i++)
		{
			int index = i;
			HUDIconButton hUDIconButton = new HUDIconButton(mLayoutRef, GetIconPosition(i), WeakGlobalInstance<Smithy>.instance.GetIconFile(i), "Helper", i, isAvailCallback, getCoolDownCallback);
			hUDIconButton.onTriggered = delegate
			{
				onHelperTriggered(index);
			};
			mHelperHUDs.Add(hUDIconButton);
		}
	}

	private bool isAvailCallback(int index)
	{
		if (WeakGlobalInstance<Smithy>.instance == null)
		{
			return false;
		}
		return WeakGlobalInstance<Smithy>.instance.IsAvailable(index);
	}

	private float getCoolDownCallback(int index)
	{
		if (WeakGlobalInstance<Smithy>.instance == null)
		{
			return 0f;
		}
		return WeakGlobalInstance<Smithy>.instance.GetCoolDown(index);
	}

	private void onHelperTriggered(int index)
	{
		if (WeakGlobalInstance<Smithy>.instance != null && !WeakGlobalSceneBehavior<InGameImpl>.instance.gameOver && WeakGlobalInstance<Smithy>.instance.IsAvailable(index))
		{
			WeakGlobalInstance<Smithy>.instance.Spawn(index);
			if (WeakGlobalInstance<TutorialHookup>.instance != null)
			{
				WeakGlobalInstance<TutorialHookup>.instance.summonedAlly = true;
			}
			Profile instance = Singleton<Profile>.instance;
			if (instance.hasSummonedFarmer && instance.hasSummonedSwordWarrior && instance.hasSummonedSpearWarrior && instance.hasSummonedBowman && instance.hasSummonedPanzerSamurai && instance.hasSummonedPriest && instance.hasSummonedNobunaga && instance.hasSummonedSpearHorseman && instance.hasSummonedTakeda && instance.hasSummonedRifleman && instance.hasSummonedFrostie && instance.hasSummonedSwordsmith)
			{
				SingletonMonoBehaviour<Achievements>.instance.Award("SAMUZOMBIE_ACHIEVE_WHOS_ALL_HERE");
			}
		}
	}
}
