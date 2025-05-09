using System;
using System.Collections.Generic;
using UnityEngine;

public class CollectableManager : WeakGlobalInstance<CollectableManager>
{
	private const float kHeroCollectRange = 32f;

	public static WaveSpoils currentWaveSpoils = new WaveSpoils();

	private List<Collectable> mCollectables;

	private ResourceTemplate[] mResourceTemplates;

	private float mTotalDropWeight;

	private float mLeftEdge;

	private float mRightEdge;

	private float mCenterX;

	public float magnetMaxDist { get; private set; }

	public float magnetMinSpeed { get; private set; }

	public float magnetMaxSpeed { get; private set; }

	public CollectableManager(float leftEdge, float rightEdge, float centerX)
	{
		SetUniqueInstance(this);
		mCollectables = new List<Collectable>();
		mLeftEdge = leftEdge;
		mRightEdge = rightEdge;
		mCenterX = centerX;
		mTotalDropWeight = 0f;
		currentWaveSpoils = new WaveSpoils();
		if (WeakGlobalSceneBehavior<InGameImpl>.instance.activeCharm == "wealth" || WeakGlobalSceneBehavior<InGameImpl>.instance.activeCharm.Contains("+wealth"))
		{
			magnetMaxDist = float.Parse(Singleton<CharmsDatabase>.instance.GetAttribute(WeakGlobalSceneBehavior<InGameImpl>.instance.activeCharm, "magnetRange"));
			magnetMinSpeed = float.Parse(Singleton<CharmsDatabase>.instance.GetAttribute(WeakGlobalSceneBehavior<InGameImpl>.instance.activeCharm, "magnetMinPullSpeed"));
			magnetMaxSpeed = float.Parse(Singleton<CharmsDatabase>.instance.GetAttribute(WeakGlobalSceneBehavior<InGameImpl>.instance.activeCharm, "magnetMaxPullSpeed"));
		}
		else
		{
			magnetMaxDist = 0f;
			magnetMinSpeed = 0f;
			magnetMaxSpeed = 0f;
		}
		SDFTreeNode sDFTreeNode = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/resources");
		if (sDFTreeNode == null)
		{
			return;
		}
		mResourceTemplates = new ResourceTemplate[8];
		for (int i = 0; i < 8; i++)
		{
			ECollectableType eCollectableType = (ECollectableType)i;
			SDFTreeNode sDFTreeNode2 = sDFTreeNode.to(eCollectableType.ToString());
			ResourceTemplate resourceTemplate = new ResourceTemplate();
			if (sDFTreeNode2 != null)
			{
				resourceTemplate.prefab = Resources.Load(sDFTreeNode2.to("prefab")[Singleton<PlayModesManager>.instance.selectedMode]) as GameObject;
				resourceTemplate.amount = int.Parse(sDFTreeNode2["amount"]);
				resourceTemplate.lifetime = float.Parse(sDFTreeNode2["lifetime"]);
				resourceTemplate.weight = float.Parse(sDFTreeNode2["weight"]);
				resourceTemplate.contentsTotalWeight = 0f;
				resourceTemplate.contents = null;
				resourceTemplate.postDeathContentsTotalWeight = 0f;
				resourceTemplate.postDeathContents = null;
				if (i >= 3 && i <= 6)
				{
					SDFTreeNode sDFTreeNode3 = sDFTreeNode2.to("normal");
					if (sDFTreeNode3 != null)
					{
						sDFTreeNode3 = sDFTreeNode3.to(Singleton<PlayModesManager>.instance.selectedMode);
						if (sDFTreeNode3 != null)
						{
							resourceTemplate.contents = new Dictionary<string, float>();
							foreach (KeyValuePair<string, string> attribute in sDFTreeNode3.attributes)
							{
								if (IsValidPresentContent(attribute.Key) && !AlreadyHasPermenantItem(attribute.Key) && !ItemRestrictedByLevel(attribute.Key))
								{
									float num = float.Parse(attribute.Value);
									resourceTemplate.contents.Add(attribute.Key, num);
									resourceTemplate.contentsTotalWeight += num;
								}
							}
						}
					}
					sDFTreeNode3 = sDFTreeNode2.to("death");
					if (sDFTreeNode3 != null)
					{
						sDFTreeNode3 = sDFTreeNode3.to(Singleton<PlayModesManager>.instance.selectedMode);
						if (sDFTreeNode3 != null)
						{
							resourceTemplate.postDeathContents = new Dictionary<string, float>();
							foreach (KeyValuePair<string, string> attribute2 in sDFTreeNode3.attributes)
							{
								if (IsValidPresentContent(attribute2.Key) && !AlreadyHasPermenantItem(attribute2.Key) && !ItemRestrictedByLevel(attribute2.Key))
								{
									float num2 = float.Parse(attribute2.Value);
									resourceTemplate.postDeathContents.Add(attribute2.Key, num2);
									resourceTemplate.postDeathContentsTotalWeight += num2;
								}
							}
						}
					}
				}
			}
			mResourceTemplates[i] = resourceTemplate;
			mTotalDropWeight += resourceTemplate.weight;
		}
	}

	public void Update()
	{
		for (int num = mCollectables.Count - 1; num >= 0; num--)
		{
			if (mCollectables[num].isReadyToBeCollected && IsInRange(WeakGlobalSceneBehavior<InGameImpl>.instance.hero, mCollectables[num]))
			{
				mCollectables[num].OnCollected();
			}
			else if (mCollectables[num].isReadyToDie)
			{
				mCollectables[num].Destroy();
				mCollectables.RemoveAt(num);
			}
			else
			{
				mCollectables[num].Update();
			}
		}
	}

	public void SpawnResources(ResourceDrops drops, Vector3 position)
	{
		int num = drops.amountDropped.randInRange();
		if (num <= 0)
		{
			return;
		}
		List<ECollectableType> list = new List<ECollectableType>();
		for (int i = 0; i < num; i++)
		{
			float num2 = UnityEngine.Random.Range(0f, mTotalDropWeight - float.Epsilon);
			for (int j = 0; j < 8; j++)
			{
				ECollectableType eCollectableType = (ECollectableType)j;
				num2 -= mResourceTemplates[j].weight;
				if (num2 < 0f && mResourceTemplates[j].weight > 0f)
				{
					if (eCollectableType >= ECollectableType.presentA && eCollectableType <= ECollectableType.presentD)
					{
						mCollectables.Add(new Collectable(eCollectableType, mResourceTemplates[(int)eCollectableType], position, FindNewCollectableFinalPosition(position)));
						return;
					}
					list.Add(eCollectableType);
					break;
				}
			}
		}
		foreach (ECollectableType item in list)
		{
			mCollectables.Add(new Collectable(item, mResourceTemplates[(int)item], position, FindNewCollectableFinalPosition(position)));
		}
	}

	public void ForceSpawnResourceType(string dropType, Vector3 position)
	{
		foreach (int value in Enum.GetValues(typeof(ECollectableType)))
		{
			if (((ECollectableType)value).ToString().Equals(dropType, StringComparison.OrdinalIgnoreCase))
			{
				mCollectables.Add(new Collectable((ECollectableType)value, mResourceTemplates[value], position, FindNewCollectableFinalPosition(position)));
				break;
			}
		}
	}

	public void OpenPresents(bool fromPlayerDeath)
	{
		foreach (CollectedPresent present in currentWaveSpoils.presents)
		{
			if (present.hasBeenOpened)
			{
				continue;
			}
			present.hasBeenOpened = true;
			float num = 0f;
			Dictionary<string, float> dictionary = null;
			if (fromPlayerDeath)
			{
				num = UnityEngine.Random.Range(0f, mResourceTemplates[(int)present.type].postDeathContentsTotalWeight - float.Epsilon);
				dictionary = mResourceTemplates[(int)present.type].postDeathContents;
			}
			else
			{
				num = UnityEngine.Random.Range(0f, mResourceTemplates[(int)present.type].contentsTotalWeight - float.Epsilon);
				dictionary = mResourceTemplates[(int)present.type].contents;
			}
			foreach (KeyValuePair<string, float> item in dictionary)
			{
				num -= item.Value;
				if (num < 0f)
				{
					present.contents = item.Key;
					RemovePermenantItemFromFuturePresents(item.Key);
					break;
				}
			}
		}
	}

	public void GiveResource(ECollectableType type, int amount)
	{
		switch (type)
		{
		case ECollectableType.coin:
			if (Singleton<Profile>.instance.earnsDoubleCoins)
			{
				currentWaveSpoils.coins += amount * 2;
			}
			else
			{
				currentWaveSpoils.coins += amount;
			}
			break;
		case ECollectableType.leadership:
			WeakGlobalInstance<Smithy>.instance.resources += amount;
			break;
		case ECollectableType.gem:
			currentWaveSpoils.gems += amount;
			break;
		case ECollectableType.pachinkoball:
			currentWaveSpoils.balls += amount;
			break;
		case ECollectableType.presentA:
		case ECollectableType.presentB:
		case ECollectableType.presentC:
		{
			CollectedPresent collectedPresent = new CollectedPresent();
			collectedPresent.type = type;
			currentWaveSpoils.presents.Add(collectedPresent);
			break;
		}
		case ECollectableType.presentD:
			break;
		}
	}

	public void GiveSpecificPresent(string id)
	{
		CollectedPresent collectedPresent = new CollectedPresent();
		collectedPresent.type = ECollectableType.presentD;
		collectedPresent.contents = id;
		collectedPresent.hasBeenOpened = true;
		currentWaveSpoils.presents.Add(collectedPresent);
	}

	public void BankAllResources()
	{
		Singleton<Profile>.instance.ForceActiveSaveData(false);
		Singleton<Profile>.instance.coins += currentWaveSpoils.coins;
		ApplicationUtilities.GWalletBalance(currentWaveSpoils.gems, "Mission " + Singleton<Profile>.instance.waveToBeat + " Complete", "CREDIT_IN_GAME_AWARD");
		Singleton<Profile>.instance.pachinkoBalls += currentWaveSpoils.balls;
		Singleton<PlayStatistics>.instance.stats.lastWaveCoinsCollected += currentWaveSpoils.coins;
		OpenPresents(false);
		foreach (CollectedPresent present in currentWaveSpoils.presents)
		{
			int val = 1;
			if (ArrayContainsString(Singleton<PotionsDatabase>.instance.allIDsForActivePlayMode, present.contents))
			{
				val = int.Parse(Singleton<PotionsDatabase>.instance.GetAttribute(present.contents, "storePack"));
			}
			else if (ArrayContainsString(Singleton<HelpersDatabase>.instance.allIDs, present.contents))
			{
				Singleton<Analytics>.instance.LogEvent("RecievedUnlockableHelper", present.contents);
			}
			if (!CashIn.From(present.contents, val, "Random Loot", "CREDIT_IN_GAME_AWARD"))
			{
				Debug.LogError("Could not identify present contents " + present.contents);
			}
		}
		Singleton<Profile>.instance.ForceActiveSaveData(true);
	}

	private bool IsInRange(Hero hero, Collectable obj)
	{
		return (hero.position.z > obj.position.z && !obj.wasAtLeftOfHero) || (hero.position.z < obj.position.z && obj.wasAtLeftOfHero) || Mathf.Abs(hero.position.z - obj.position.z) <= 32f;
	}

	public Vector3 FindNewCollectableFinalPosition(Vector3 spawnPosition)
	{
		Vector3 zero = Vector3.zero;
		zero.x = mCenterX + 115f + UnityEngine.Random.Range(-20f, 20f);
		zero.z = spawnPosition.z + UnityEngine.Random.Range(-25f, 90f);
		zero.z = Mathf.Clamp(zero.z, mLeftEdge, mRightEdge);
		zero.y = WeakGlobalInstance<RailManager>.instance.GetY(zero.z) + 16f;
		return zero;
	}

	private bool AlreadyHasPermenantItem(string theItemName)
	{
		if (Singleton<AbilitiesDatabase>.instance.Contains(theItemName))
		{
			if (Singleton<Profile>.instance.GetAbilityLevel(theItemName) > 0)
			{
				return true;
			}
		}
		else if (Singleton<HelpersDatabase>.instance.Contains(theItemName) && Singleton<Profile>.instance.GetHelperLevel(theItemName) > 0)
		{
			return true;
		}
		return false;
	}

	private bool ItemRestrictedByLevel(string theItemName)
	{
		bool flag = false;
		string[] allIDs = Singleton<HelpersDatabase>.instance.allIDs;
		foreach (string strA in allIDs)
		{
			if (string.Compare(strA, theItemName, true) == 0)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			return false;
		}
		SDFTreeNode sDFTreeNode = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/Helpers/" + theItemName);
		if (sDFTreeNode != null)
		{
			int num = 0;
			if (sDFTreeNode.hasAttribute("availableAtWave"))
			{
				num = int.Parse(sDFTreeNode["availableAtWave"]);
			}
			if (Singleton<Profile>.instance.highestUnlockedWave >= num)
			{
				return false;
			}
		}
		return true;
	}

	private void RemovePermenantItemFromFuturePresents(string theItemName)
	{
		if (!ArrayContainsString(Singleton<HelpersDatabase>.instance.allIDs, theItemName) && !ArrayContainsString(Singleton<AbilitiesDatabase>.instance.allIDs, theItemName))
		{
			return;
		}
		ResourceTemplate[] array = mResourceTemplates;
		foreach (ResourceTemplate resourceTemplate in array)
		{
			if (resourceTemplate.contents != null && resourceTemplate.contents.ContainsKey(theItemName))
			{
				resourceTemplate.contentsTotalWeight -= resourceTemplate.contents[theItemName];
				resourceTemplate.contents.Remove(theItemName);
			}
			if (resourceTemplate.postDeathContents != null && resourceTemplate.postDeathContents.ContainsKey(theItemName))
			{
				resourceTemplate.postDeathContentsTotalWeight -= resourceTemplate.postDeathContents[theItemName];
				resourceTemplate.postDeathContents.Remove(theItemName);
			}
		}
	}

	private bool ArrayContainsString(string[] theStringArray, string theStringToFind)
	{
		foreach (string text in theStringArray)
		{
			if (text.Equals(theStringToFind, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	private bool IsValidPresentContent(string itemID)
	{
		return SpoilsDisplay.BuildEntry(itemID, 1) != null;
	}
}
