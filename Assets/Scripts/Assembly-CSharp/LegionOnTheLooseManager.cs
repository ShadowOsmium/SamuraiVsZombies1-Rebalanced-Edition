using System.Collections.Generic;
using UnityEngine;

public class LegionOnTheLooseManager : WeakGlobalInstance<LegionOnTheLooseManager>
{
	private class Entry
	{
		public string moduleID = string.Empty;

		public float maxTimer;

		public float currentTimer;

		public string helperID = string.Empty;

		public int numSpawns;

		public int queuedSpawns;

		public float deltaTimerMax;

		public float currentDeltaTimer;
	}

	public OnSUIStringCallback onSpawnStart;

	private List<Entry> mEntries = new List<Entry>();

	private int mMaxHelpers;

	private bool allowedToSpawn
	{
		get
		{
			return WeakGlobalInstance<CharactersManager>.instance.helpersCount < mMaxHelpers;
		}
	}

	public LegionOnTheLooseManager()
	{
		SetUniqueInstance(this);
		int num = 0;
		if (WeakGlobalSceneBehavior<InGameImpl>.instance.activeCharm == "horde" || WeakGlobalSceneBehavior<InGameImpl>.instance.activeCharm.Contains("+horde"))
		{
			num = int.Parse(Singleton<CharmsDatabase>.instance.GetAttribute(WeakGlobalSceneBehavior<InGameImpl>.instance.activeCharm, "legionOnTheLooseBonus"));
		}
		switch (Singleton<PlayModesManager>.instance.selectedMode)
		{
		case "zombies":
			AddModule("LegionOnTheLoose", Singleton<Profile>.instance.legionOnTheLooseLevel + num, num > 0);
			AddModule("DeathFromAbove", Singleton<Profile>.instance.deathFromAboveLevel, false);
			break;
		}
	}

	public void Update()
	{
		foreach (Entry mEntry in mEntries)
		{
			if (mEntry.queuedSpawns > 0)
			{
				mEntry.currentDeltaTimer += Time.deltaTime;
				if (mEntry.currentDeltaTimer >= mEntry.deltaTimerMax)
				{
					mEntry.currentDeltaTimer -= mEntry.deltaTimerMax;
					mEntry.queuedSpawns--;
					Spawn(mEntry.helperID);
				}
				continue;
			}
			mEntry.currentTimer += Time.deltaTime;
			if (mEntry.currentTimer >= mEntry.maxTimer)
			{
				if (allowedToSpawn && onSpawnStart != null)
				{
					onSpawnStart(mEntry.moduleID);
				}
				mEntry.currentTimer -= mEntry.maxTimer;
				mEntry.queuedSpawns = mEntry.numSpawns - 1;
				mEntry.currentDeltaTimer = 0f;
				Spawn(mEntry.helperID);
			}
		}
	}

	private void AddModule(string moduleID, int level, bool startRightAway)
	{
		if (level == 0)
		{
			return;
		}
		SDFTreeNode sDFTreeNode = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/" + moduleID);
		if (sDFTreeNode == null)
		{
			Debug.Log("WARNING: Unable to load legion module: " + moduleID);
			return;
		}
		SDFTreeNode sDFTreeNode2 = null;
		while (sDFTreeNode2 == null && level > 0)
		{
			sDFTreeNode2 = sDFTreeNode.to(level);
			level--;
		}
		if (sDFTreeNode2 != null)
		{
			if (mMaxHelpers == 0)
			{
				mMaxHelpers = int.Parse(sDFTreeNode["maxHelpers"]);
			}
			Entry entry = new Entry();
			entry.moduleID = moduleID;
			entry.maxTimer = float.Parse(sDFTreeNode2["delay"]);
			if (startRightAway)
			{
				entry.currentTimer = entry.maxTimer;
			}
			entry.deltaTimerMax = float.Parse(sDFTreeNode["inBetweenDelay"]);
			entry.helperID = sDFTreeNode2["helperID"];
			entry.numSpawns = int.Parse(sDFTreeNode2["num"]);
			mEntries.Add(entry);
		}
	}

	private void Spawn(string helperID)
	{
		if (allowedToSpawn)
		{
			WeakGlobalInstance<Smithy>.instance.ForceSpawn(helperID);
		}
	}
}
