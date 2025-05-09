using System;
using System.Collections.Generic;
using UnityEngine;

public class Smithy : WeakGlobalInstance<Smithy>
{
	public struct SmithyCost
	{
		public float smithy;

		public int gems;

		public int coins;

		public string helperId;

		public bool canAfford
		{
			get
			{
				if (smithy > 0f)
				{
					return WeakGlobalInstance<Smithy>.instance.resources >= smithy;
				}
				if (gems > 0)
				{
					return Singleton<Profile>.instance.gems >= gems;
				}
				if (coins > 0)
				{
					return Singleton<Profile>.instance.coins >= coins;
				}
				return false;
			}
		}

		public SmithyCost(string data)
		{
			smithy = 0f;
			gems = 0;
			coins = 0;
			helperId = string.Empty;
			string[] array = data.Split(',');
			if (array.Length >= 1)
			{
				smithy = float.Parse(array[0]);
			}
			if (array.Length >= 2)
			{
				gems = int.Parse(array[1]);
			}
			if (array.Length >= 3)
			{
				coins = int.Parse(array[2]);
			}
		}

		public void Spend()
		{
			if (smithy > 0f)
			{
				WeakGlobalInstance<Smithy>.instance.resources -= smithy;
			}
			else if (gems > 0)
			{
				ApplicationUtilities.GWalletBalance(-gems, "Smithy Spend: " + helperId, "DEBIT_IN_APP_PURCHASE");
			}
			else if (coins > 0)
			{
				Singleton<Profile>.instance.coins -= coins;
			}
		}
	}

	private class HelperTypeData
	{
		public string id;

		public string displayName;

		public string prefabPath;

		public GameObject prefab;

		public string HUDIcon;

		public bool unique;

		public SmithyCost smithyCost;

		public float health;

		public float speedMin;

		public float speedMax;

		public bool isFlying;

		public bool exploseOnMelee;

		public float meleeFreeze;

		public float autoHealthRecovery;

		public float swordAttackRange;

		public float bowAttackRange;

		public float meleeDamage;

		public float bowDamage;

		public float attackFrequency;

		public float damageBuffPercent;

		public int knockbackPower;

		public int knockbackResistance;

		public Projectile.EProjectileType projectile;

		public CharactersManager.ELanePreference lanePref;

		public float totalCooldown;

		public float currentCooldown;

		public string weaponPrefabLocation;

		public bool rangedWeapon;

		public bool bladedWeapon;

		public string upgradeAlliesFrom;

		public string upgradeAlliesTo;

		public string spawnOnDeathType;

		public int spawnOnDeathNum;

		public string spawnFriendID;

		public HelperTypeData(string helperID)
		{
			id = helperID;
			displayName = Singleton<HelpersDatabase>.instance.GetAttribute(helperID, "displayName");
			prefabPath = Singleton<HelpersDatabase>.instance.GetAttribute(helperID, "prefab");
			prefab = Resources.Load(prefabPath) as GameObject;
			HUDIcon = Singleton<HelpersDatabase>.instance.GetAttribute(helperID, "HUDIcon");
			smithyCost = new SmithyCost(Singleton<HelpersDatabase>.instance.GetAttribute(helperID, "resourcesCost"));
			smithyCost.helperId = helperID;
			totalCooldown = float.Parse(Singleton<HelpersDatabase>.instance.GetAttribute(helperID, "cooldownTimer"));
			currentCooldown = totalCooldown;
			health = float.Parse(Singleton<HelpersDatabase>.instance.GetAttribute(helperID, "health"));
			speedMin = float.Parse(Singleton<HelpersDatabase>.instance.GetAttribute(helperID, "speedMin"));
			speedMax = float.Parse(Singleton<HelpersDatabase>.instance.GetAttribute(helperID, "speedMax"));
			bool.TryParse(Singleton<HelpersDatabase>.instance.GetAttributeOrNull(helperID, "flying"), out isFlying);
			bool.TryParse(Singleton<HelpersDatabase>.instance.GetAttributeOrNull(helperID, "exploseOnMelee"), out exploseOnMelee);
			float.TryParse(Singleton<HelpersDatabase>.instance.GetAttributeOrNull(helperID, "meleeFreeze"), out meleeFreeze);
			float.TryParse(Singleton<HelpersDatabase>.instance.GetAttributeOrNull(helperID, "autoHealthRecovery"), out autoHealthRecovery);
			float.TryParse(Singleton<HelpersDatabase>.instance.GetAttributeOrNull(helperID, "meleeRange"), out swordAttackRange);
			float.TryParse(Singleton<HelpersDatabase>.instance.GetAttributeOrNull(helperID, "bowRange"), out bowAttackRange);
			float.TryParse(Singleton<HelpersDatabase>.instance.GetAttributeOrNull(helperID, "meleeDamage"), out meleeDamage);
			float.TryParse(Singleton<HelpersDatabase>.instance.GetAttributeOrNull(helperID, "bowDamage"), out bowDamage);
			attackFrequency = float.Parse(Singleton<HelpersDatabase>.instance.GetAttribute(helperID, "attackFrequency"));
			float.TryParse(Singleton<HelpersDatabase>.instance.GetAttributeOrNull(helperID, "damageBuffPercent"), out damageBuffPercent);
			bool.TryParse(Singleton<HelpersDatabase>.instance.GetAttributeOrNull(helperID, "usesBladeWeapon"), out bladedWeapon);
			int.TryParse(Singleton<HelpersDatabase>.instance.GetAttributeOrNull(helperID, "knockbackPower"), out knockbackPower);
			int.TryParse(Singleton<HelpersDatabase>.instance.GetAttributeOrNull(helperID, "knockbackResistance"), out knockbackResistance);
			bool.TryParse(Singleton<HelpersDatabase>.instance.GetAttributeOrNull(helperID, "unique"), out unique);
			spawnOnDeathType = Singleton<HelpersDatabase>.instance.GetAttributeOrNull(helperID, "spawnOnDeath");
			int.TryParse(Singleton<HelpersDatabase>.instance.GetAttributeOrNull(helperID, "spawnOnDeathCount"), out spawnOnDeathNum);
			spawnFriendID = Singleton<HelpersDatabase>.instance.GetAttributeOrNull(helperID, "spawnFriendID");
			string attributeOrNull = Singleton<HelpersDatabase>.instance.GetAttributeOrNull(helperID, "projectile");
			if (!string.IsNullOrEmpty(attributeOrNull))
			{
				projectile = (Projectile.EProjectileType)(int)Enum.Parse(typeof(Projectile.EProjectileType), attributeOrNull);
			}
			attributeOrNull = Singleton<HelpersDatabase>.instance.GetAttributeOrNull(helperID, "lane");
			if (!string.IsNullOrEmpty(attributeOrNull))
			{
				lanePref = (CharactersManager.ELanePreference)(int)Enum.Parse(typeof(CharactersManager.ELanePreference), attributeOrNull);
			}
			attributeOrNull = Singleton<HelpersDatabase>.instance.GetAttributeOrNull(helperID, "weaponMeleePrefab");
			if (!string.IsNullOrEmpty(attributeOrNull))
			{
				weaponPrefabLocation = attributeOrNull;
			}
			attributeOrNull = Singleton<HelpersDatabase>.instance.GetAttributeOrNull(helperID, "weaponRangedPrefab");
			if (!string.IsNullOrEmpty(attributeOrNull))
			{
				weaponPrefabLocation = attributeOrNull;
				rangedWeapon = true;
			}
			attributeOrNull = Singleton<HelpersDatabase>.instance.GetAttributeOrNull(helperID, "upgradeAlliesFrom");
			if (!string.IsNullOrEmpty(attributeOrNull))
			{
				upgradeAlliesFrom = attributeOrNull;
				rangedWeapon = true;
			}
			attributeOrNull = Singleton<HelpersDatabase>.instance.GetAttributeOrNull(helperID, "upgradeAlliesTo");
			if (!string.IsNullOrEmpty(attributeOrNull))
			{
				upgradeAlliesTo = attributeOrNull;
				rangedWeapon = true;
			}
			if (WeakGlobalSceneBehavior<InGameImpl>.instance.activeCharm == "haste" || WeakGlobalSceneBehavior<InGameImpl>.instance.activeCharm.Contains("+haste"))
			{
				float num = float.Parse(Singleton<CharmsDatabase>.instance.GetAttribute(WeakGlobalSceneBehavior<InGameImpl>.instance.activeCharm, "speedMultiplier"));
				attackFrequency /= num;
				speedMax *= num;
				speedMin *= num;
			}
		}

		public void Update()
		{
			currentCooldown = Mathf.Min(totalCooldown, currentCooldown + Time.deltaTime);
		}

		public void EngageCooldown()
		{
			currentCooldown = 0f;
		}
	}

	private List<HelperTypeData> mHelperTypes = new List<HelperTypeData>();

	private CharactersManager mCharManagerRef;

	private BoxCollider mHelperSpawnArea;

	private float mHelpersZTarget;

	private SDFTreeNode mSmithyData;

	private SDFTreeNode mLevelData;

	private static GameObject mSpawnEffect;

	private static GameObject mHelperReplaceEffect;

	private List<string> mUniquesAlive = new List<string>();

	private int mLevel;

	private int mMaxLevel;

	private int mExperience;

	private float mIncreaseRate;

	private float mMaxResources;

	private float mResources;

	private float mAbsoluteMaxResources = -1f;

	private float mLevelUpThreshold;

	private bool mIsLeftToRightGameplay = true;

	private float mSharedUndeathRegeneration = Undeath.regeneration;

	private Dictionary<string, HelperTypeData> mHelperDataCache = new Dictionary<string, HelperTypeData>();

	public int numTypes
	{
		get
		{
			return mHelperTypes.Count;
		}
	}

	public CharactersManager characterManagerRef
	{
		get
		{
			return mCharManagerRef;
		}
		set
		{
			mCharManagerRef = value;
		}
	}

	public BoxCollider helperSpawnArea
	{
		get
		{
			return mHelperSpawnArea;
		}
		set
		{
			mHelperSpawnArea = value;
		}
	}

	public float helpersZTarget
	{
		get
		{
			return mHelpersZTarget;
		}
		set
		{
			mHelpersZTarget = value;
		}
	}

	public float maxResources
	{
		get
		{
			return mMaxResources;
		}
	}

	public float absoluteMaxResources
	{
		get
		{
			return mAbsoluteMaxResources;
		}
	}

	public float resources
	{
		get
		{
			return mResources;
		}
		set
		{
			mResources = Mathf.Clamp(value, 0f, mMaxResources);
		}
	}

	public int level
	{
		get
		{
			return mLevel;
		}
		set
		{
			SetSmithyLevel(value);
		}
	}

	public int maxLevel
	{
		get
		{
			return mMaxLevel;
		}
	}

	public float levelUpThreshold
	{
		get
		{
			return mLevelUpThreshold;
		}
	}

	public bool isUpgradable
	{
		get
		{
			if (mLevel == mMaxLevel)
			{
				return false;
			}
			if (mResources < mLevelUpThreshold)
			{
				return false;
			}
			return true;
		}
	}

	public Smithy()
	{
		SetUniqueInstance(this);
		mIsLeftToRightGameplay = Singleton<PlayModesManager>.instance.gameDirection == PlayModesManager.GameDirection.LeftToRight;
		mSmithyData = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/Smithy");
		mMaxLevel = 50;
		while (mMaxLevel >= 1 && mSmithyData.to(mMaxLevel) == null)
		{
			mMaxLevel--;
		}
		mAbsoluteMaxResources = float.Parse(mSmithyData.to(mMaxLevel)["maxResource"]);
		SetSmithyLevel(Singleton<Profile>.instance.initialSmithyLevel);
		List<string> selectedHelpers = Singleton<Profile>.instance.GetSelectedHelpers();
		foreach (string item in selectedHelpers)
		{
			HelperTypeData helperTypeData = LoadHelperData(item);
			mHelperTypes.Add(helperTypeData);
			WeakGlobalInstance<PrefabPreloader>.instance.Preload(helperTypeData.prefabPath, helperTypeData.prefab);
		}
		mResources = 0f;
		mSpawnEffect = Resources.Load("FX/TeleportAlly") as GameObject;
		mHelperReplaceEffect = Resources.Load("FX/Swapping") as GameObject;
	}

	public void Update()
	{
		resources += Time.deltaTime * mIncreaseRate;
		foreach (HelperTypeData mHelperType in mHelperTypes)
		{
			mHelperType.Update();
		}
		if (mHelperTypes.Count > 0 && WeakGlobalInstance<TutorialHookup>.instance != null)
		{
			WeakGlobalInstance<TutorialHookup>.instance.firstHelperAvailable = IsAvailable(0);
		}
	}

	public float GetCoolDown(int typeIndex)
	{
		return mHelperTypes[typeIndex].currentCooldown / mHelperTypes[typeIndex].totalCooldown;
	}

	public string GetIconFile(int typeIndex)
	{
		return mHelperTypes[typeIndex].HUDIcon;
	}

	public bool IsAvailable(int typeIndex)
	{
		HelperTypeData helperTypeData = mHelperTypes[typeIndex];
		if (helperTypeData.unique && IsUniqueAlive(helperTypeData.id))
		{
			return false;
		}
		float num = 0f;
		if (helperTypeData.smithyCost.smithy > 0f)
		{
			num = Mathf.Min(1f, mResources / helperTypeData.smithyCost.smithy);
		}
		else if (helperTypeData.smithyCost.canAfford)
		{
			num = 1f;
		}
		return GetCoolDown(typeIndex) == 1f && num == 1f;
	}

	public Helper Spawn(int typeIndex)
	{
		if (typeIndex < 0 || typeIndex >= mHelperTypes.Count)
		{
			Debug.Log("ERROR: Trying to spawn an out of bound helper index: " + typeIndex);
		}
		HelperTypeData helperTypeData = mHelperTypes[typeIndex];
		if (!(helperTypeData.id == "Farmer") && !(helperTypeData.id == "Swordsmith"))
		{
			Singleton<PlayStatistics>.instance.stats.summonedTroopOtherThanFarmerOrSwordsmith = true;
		}
		switch (helperTypeData.id)
		{
		case "Farmer":
			Singleton<Profile>.instance.hasSummonedFarmer = true;
			Singleton<PlayStatistics>.instance.stats.summonedTroopFarmer = true;
			break;
		case "HeavySamurai":
			Singleton<Profile>.instance.hasSummonedPanzerSamurai = true;
			break;
		case "IceArcher":
			Singleton<Profile>.instance.hasSummonedFrostie = true;
			break;
		case "Nobunaga":
			Singleton<Profile>.instance.hasSummonedNobunaga = true;
			break;
		case "Priest":
			Singleton<Profile>.instance.hasSummonedPriest = true;
			break;
		case "Samurai":
			Singleton<Profile>.instance.hasSummonedSwordWarrior = true;
			break;
		case "SamuraiArcher":
			Singleton<Profile>.instance.hasSummonedBowman = true;
			break;
		case "Rifleman":
			Singleton<Profile>.instance.hasSummonedRifleman = true;
			break;
		case "SpearHorseman":
			Singleton<Profile>.instance.hasSummonedSpearHorseman = true;
			break;
		case "SpearSamurai":
			Singleton<Profile>.instance.hasSummonedSpearWarrior = true;
			break;
		case "Swordsmith":
			Singleton<Profile>.instance.hasSummonedSwordsmith = true;
			Singleton<PlayStatistics>.instance.stats.summonedTroopSwordsmith = true;
			break;
		case "Takeda":
			Singleton<Profile>.instance.hasSummonedTakeda = true;
			break;
		}
		if (helperTypeData.unique)
		{
			if (IsUniqueAlive(helperTypeData.id))
			{
				Debug.Log("ERROR: Trying to spawn a second copy of a unique helper!");
				return null;
			}
			RegisterUnique(helperTypeData.id, true);
		}
		Singleton<Analytics>.instance.LogEvent("AllySummoned", Singleton<Profile>.instance.waveToBeat.ToString(), typeIndex);
		helperTypeData.smithyCost.Spend();
		helperTypeData.EngageCooldown();
		return SpawnHelper(helperTypeData, mHelperSpawnArea.size.x, mHelperSpawnArea.transform.position);
	}

	public int GetRandomHelperToSpawn()
	{
		List<int> list = new List<int>();
		for (int i = 0; i < mHelperTypes.Count; i++)
		{
			if (!mHelperTypes[i].unique)
			{
				list.Add(i);
			}
		}
		if (list.Count == 0)
		{
			return -1;
		}
		return list[UnityEngine.Random.Range(0, list.Count)];
	}

	public Helper SpawnForFree(int typeIndex, float sizeOfSpawnArea, Vector3 spawnPos)
	{
		if (typeIndex < 0 || typeIndex >= mHelperTypes.Count)
		{
			Debug.Log("ERROR: Trying to spawn an out of bound helper index: " + typeIndex);
		}
		HelperTypeData data = mHelperTypes[typeIndex];
		return SpawnHelper(data, sizeOfSpawnArea, spawnPos);
	}

	public Helper ForceSpawn(string helperID)
	{
		return SpawnHelper(LoadHelperData(helperID), mHelperSpawnArea.size.x, mHelperSpawnArea.transform.position);
	}

	public void ReplaceHelperWith(Character c, string helperID)
	{
		if (c != null && !(c.health <= 0f))
		{
			int helperLevel = Singleton<Profile>.instance.GetHelperLevel(helperID);
			Singleton<Profile>.instance.SetHelperLevel(helperID, Singleton<Profile>.instance.GetHelperLevel(c.uniqueID), false);
			HelperTypeData data = LoadHelperData(helperID);
			((Helper)c).ReinitializeModel(data.prefab, data.weaponPrefabLocation, data.rangedWeapon);
			InitializeHelper(ref data, c, true);
			Singleton<Profile>.instance.SetHelperLevel(helperID, helperLevel, false);
		}
	}

	public void LevelUp()
	{
		if (isUpgradable)
		{
			mResources -= mLevelUpThreshold;
			SetSmithyLevel(mLevel + 1);
		}
	}

	public void RegisterUnique(string id, bool isAlive)
	{
		if (isAlive)
		{
			if (!IsUniqueAlive(id))
			{
				mUniquesAlive.Add(id);
			}
		}
		else if (IsUniqueAlive(id))
		{
			mUniquesAlive.Remove(id);
		}
	}

	private HelperTypeData LoadHelperData(string helperID)
	{
		if (!mHelperDataCache.ContainsKey(helperID))
		{
			HelperTypeData helperTypeData = new HelperTypeData(helperID);
			mHelperDataCache.Add(helperID, helperTypeData);
			if (helperTypeData.spawnOnDeathType != string.Empty)
			{
				LoadHelperData(helperTypeData.spawnOnDeathType);
			}
			if (helperTypeData.spawnFriendID != string.Empty)
			{
				LoadHelperData(helperTypeData.spawnFriendID);
			}
		}
		return mHelperDataCache[helperID];
	}

	private Helper SpawnHelper(HelperTypeData data, float sizeOfSpawnArea, Vector3 spawnPos)
	{
		Singleton<PlayStatistics>.instance.stats.heroInvokedHelpers = true;
		float num = Mathf.Max(data.swordAttackRange, data.bowAttackRange);
		float zTarget = ((!mIsLeftToRightGameplay) ? (mHelpersZTarget + num) : (mHelpersZTarget - num));
		spawnPos.x = WeakGlobalInstance<CharactersManager>.instance.GetBestSpawnXPos(spawnPos, sizeOfSpawnArea, data.lanePref, false, false, data.bowAttackRange > 0f);
		Helper helper = new Helper(data.prefab, zTarget, spawnPos, data.weaponPrefabLocation, data.rangedWeapon);
		InitializeHelper(ref data, helper, false);
		if (data.spawnOnDeathType != null && data.spawnOnDeathType != string.Empty && data.spawnOnDeathNum > 0)
		{
			helper.SetSpawnOnDeath(data.spawnOnDeathType, data.spawnOnDeathNum);
		}
		mCharManagerRef.AddCharacter(helper);
		return helper;
	}

	private void InitializeHelper(ref HelperTypeData data, Character h, bool asReplacement)
	{
		h.uniqueID = data.id;
		h.isUnique = data.unique;
		h.meleeAttackFrequency = data.attackFrequency;
		h.bowAttackFrequency = data.attackFrequency;
		h.meleeAttackRange = data.swordAttackRange;
		h.bowAttackRange = data.bowAttackRange;
		h.bowProjectile = data.projectile;
		h.meleeDamage = data.meleeDamage;
		h.bowDamage = data.bowDamage;
		h.knockbackPower = data.knockbackPower;
		h.knockbackResistance = data.knockbackResistance;
		h.isFlying = data.isFlying;
		h.autoHealthRecovery = ((data.autoHealthRecovery == 0f) ? mSharedUndeathRegeneration : data.autoHealthRecovery);
		h.exploseOnMelee = data.exploseOnMelee;
		h.meleeFreeze = data.meleeFreeze;
		h.controller.speed = UnityEngine.Random.value * (data.speedMax - data.speedMin) + data.speedMin;
		h.meleeWeaponIsABlade = data.bladedWeapon;
		h.spawnFriendID = data.spawnFriendID;
		if (asReplacement)
		{
			float num = h.health / h.maxHealth;
			h.maxHealth = data.health;
			h.health = data.health * num;
			UnityEngine.Object.Instantiate(mHelperReplaceEffect, h.position, Quaternion.identity);
		}
		else
		{
			h.maxHealth = data.health;
			h.health = data.health;
			UnityEngine.Object.Instantiate(mSpawnEffect, h.position, Quaternion.identity);
		}
		h.damageBuffPercent = data.damageBuffPercent;
		h.upgradeAlliesFrom = data.upgradeAlliesFrom;
		h.upgradeAlliesTo = data.upgradeAlliesTo;
		if (WeakGlobalSceneBehavior<InGameImpl>.instance.activeCharm == "haste" || WeakGlobalSceneBehavior<InGameImpl>.instance.activeCharm.Contains("+haste"))
		{
			h.buffIcon.Show("FX/SpeedIcon");
		}
	}

	private void SetSmithyLevel(int level)
	{
		mLevel = Mathf.Clamp(level, 0, mMaxLevel);
		mLevelData = null;
		for (int num = mLevel; num > 0; num--)
		{
			mLevelData = mSmithyData.to(string.Format("{0:000}", mLevel));
			if (mLevelData != null)
			{
				break;
			}
		}
		mIncreaseRate = float.Parse(mLevelData["resourcesPerSeconds"]);
		mMaxResources = float.Parse(mLevelData["maxResource"]);
		mLevelUpThreshold = float.Parse(mLevelData["levelUpThreshold"]);
	}

	private bool IsUniqueAlive(string id)
	{
		return mUniquesAlive.Contains(id);
	}
}
