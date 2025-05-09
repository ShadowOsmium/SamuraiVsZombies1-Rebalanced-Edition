using System;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;

public class VillageArchers
{
	private const float kTimeFireRate = 2.5f;

	private const float kTimerTriggerDamages = 3.2f;

	private int mNumArchers = 1;

	private string mBowPrefabPath;

	private string mArcherPrefabPath;

	private List<TowerArcher> mArcherList = new List<TowerArcher>();

	private int mArcherLevel;

	private float mRange;

	private float mDamage;

	private float mAttackSpeed;

	private bool mAgainstPlayer;

	private Projectile.EProjectileType mArrowType = Projectile.EProjectileType.Arrow;

	public VillageArchers()
	{
		switch (Singleton<PlayModesManager>.instance.selectedMode)
		{
		case "classic":
			mArcherLevel = Singleton<Profile>.instance.archerLevel;
			break;
		case "zombies":
			mArcherLevel = WeakGlobalInstance<WaveManager>.instance.villageArchersLevel;
			break;
		}
		if (mArcherLevel != 0)
		{
			GetVillageArcherStats();
			for (int i = 0; i < mNumArchers; i++)
			{
				mArcherList.Add(new TowerArcher(mArcherPrefabPath, WeakGlobalSceneBehavior<InGameImpl>.instance.villageArcher[i].position, mBowPrefabPath, mArrowType, mDamage, mRange, mAttackSpeed, mAgainstPlayer));
			}
		}
	}

	public void Update()
	{
		if (mArcherLevel == 0)
		{
			return;
		}
		foreach (TowerArcher mArcher in mArcherList)
		{
			mArcher.Update();
		}
	}

	private void GetVillageArcherStats()
	{
		SDFTreeNode sDFTreeNode = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/VillageArchers");
		SDFTreeNode sDFTreeNode2 = sDFTreeNode.to(string.Format("{0:000}", mArcherLevel));
		if (sDFTreeNode2 == null)
		{
			Debug.Log("ERROR: Unable to find proper village archers stats for level " + mArcherLevel);
			return;
		}
		mRange = float.Parse(sDFTreeNode2["bowRange"]);
		mDamage = float.Parse(sDFTreeNode2["bowDamage"]);
		mArcherPrefabPath = sDFTreeNode["archerPrefab"];
		mNumArchers = int.Parse(sDFTreeNode2["numberOfArchers"]);
		mBowPrefabPath = sDFTreeNode["weaponRangedPrefab"];
		mAttackSpeed = float.Parse(sDFTreeNode2["attackFrequency"]);
		mAgainstPlayer = Singleton<PlayModesManager>.instance.selectedMode != "classic";
		mArrowType = (Projectile.EProjectileType)(int)Enum.Parse(typeof(Projectile.EProjectileType), sDFTreeNode2["projectile"]);
	}
}
