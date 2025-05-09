using System;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : WeakGlobalInstance<WaveManager>
{
	private enum CommandType
	{
		Spawn = 0,
		Delay = 1,
		LegionTag = 2,
		UserDefined = 3,
		Num = 4,
		Unknown = 5
	}

	public class WaveRecyclingMultipliers
	{
		public float enemiesHealth = 1f;

		public float enemiesDamages = 1f;

		public float drops = 1f;
	}

	private const float kSpawnDelayTimerMin = 1f;

	private const float kSpawnDelayTimerMax = 4f;

	private const int kEnemiesMax = 10;

	private int mWaveIndex;

	private BoxCollider mEnemiesSpawnArea;

	private float mZTarget;

	private SDFTreeNode waveRootData;

	private SDFTreeNode waveCommands;

	private SDFTreeNode enemiesData;

	private SDFTreeNode rewards;

	private GameObject mSpawnEffect;

	private GameObject mAltSpawnEffect;

	private WaveRecyclingMultipliers mLevelMultipliers = new WaveRecyclingMultipliers();

	private int mNextCommandToRun;

	private float mSpawnDelayTimer;

	private bool mSkipNextLegion;

	private string mTutorial = string.Empty;

	private int mTotalNumEnemies;

	private int mSpawnedEnemiesSoFar;

	private int mEnemiesKilledSoFar;

	private List<string> mAllDifferentEnemies = new List<string>();

	private List<int> mLegionMarkers = new List<int>();

	private int mVillageArchersLevel;

	private int mBellLevel;

	public bool isDone
	{
		get
		{
			return mNextCommandToRun >= waveCommands.attributeCount;
		}
	}

	public int totalEnemies
	{
		get
		{
			return mTotalNumEnemies;
		}
		set
		{
			mTotalNumEnemies = value;
		}
	}

	public int enemiesKilledSoFar
	{
		get
		{
			return mEnemiesKilledSoFar;
		}
	}

	public List<string> allDifferentEnemies
	{
		get
		{
			return mAllDifferentEnemies;
		}
	}

	public bool skipNextLegion
	{
		get
		{
			return mSkipNextLegion;
		}
		set
		{
			mSkipNextLegion = value;
		}
	}

	public BoxCollider enemiesSpawnArea
	{
		get
		{
			return mEnemiesSpawnArea;
		}
	}

	public List<int> legionMarkers
	{
		get
		{
			return mLegionMarkers;
		}
	}

	public int waveLevel
	{
		get
		{
			return mWaveIndex;
		}
	}

	public string tutorial
	{
		get
		{
			return mTutorial;
		}
	}

	public WaveRecyclingMultipliers multipliers
	{
		get
		{
			return mLevelMultipliers;
		}
	}

	public int villageArchersLevel
	{
		get
		{
			return mVillageArchersLevel;
		}
	}

	public int bellLevel
	{
		get
		{
			return mBellLevel;
		}
	}

	public event OnSUIGenericCallback onLegionStart;

	public WaveManager(int waveIndex, BoxCollider enemiesSpawnArea, float zTarget)
	{
		SetUniqueInstance(this);
		mWaveIndex = waveIndex;
		mEnemiesSpawnArea = enemiesSpawnArea;
		mZTarget = zTarget;
		LoadData();
		AnalyseWaveCommandsForStats();
		mNextCommandToRun = 0;
		mEnemiesKilledSoFar = 0;
		mSpawnEffect = Resources.Load("FX/TeleportEnemy") as GameObject;
		mAltSpawnEffect = Resources.Load("FX/Divine") as GameObject;
	}

	public void Update()
	{
		UpdateDelayTimer();
	}

	public static void LoadNextWaveLevel()
	{
		Singleton<Profile>.instance.ClearBonusWaveData();
		if (Singleton<Profile>.instance.readyToStartBonusWave)
		{
			SDFTreeNode waveData = GetWaveData(Singleton<Profile>.instance.bonusWaveToBeat, true);
			Singleton<Profile>.instance.LoadBonusWaveData(waveData);
			Singleton<MenusFlow>.instance.LoadScene(waveData["scene"]);
			SingletonMonoBehaviour<WaitingIconBetweenScenes>.instance.visible = true;
		}
		else
		{
			SDFTreeNode waveData2 = GetWaveData(Singleton<Profile>.instance.waveToBeat);
			Singleton<MenusFlow>.instance.LoadScene(waveData2["scene"]);
			SingletonMonoBehaviour<WaitingIconBetweenScenes>.instance.visible = true;
		}
	}

	public void registerEnemyKilled(string enemyID)
	{
		mEnemiesKilledSoFar++;
		Singleton<Profile>.instance.IncNumKillsOfEnemyType(enemyID);
	}

	public static string GetNextWaveNumberDisplay()
	{
		int waveToBeat = Singleton<Profile>.instance.waveToBeat;
		return GetWaveNumberDisplay(waveToBeat, Singleton<Profile>.instance.GetWaveLevel(waveToBeat));
	}

	public static string GetWaveNumberDisplay(int waveNum, int waveLevel)
	{
		if (waveLevel <= 1)
		{
			return waveNum.ToString();
		}
		return string.Format("{0}-{1}", waveNum.ToString(), waveLevel.ToString());
	}

	public void PreloadEnemies()
	{
		foreach (string mAllDifferentEnemy in mAllDifferentEnemies)
		{
			SDFTreeNode sDFTreeNode = enemiesData.to(mAllDifferentEnemy);
			WeakGlobalInstance<PrefabPreloader>.instance.Preload(sDFTreeNode["prefab"]);
		}
	}

	public SDFTreeNode GetEnemyData(string id)
	{
		return enemiesData.to(id);
	}

	public void AddSpecialRewardsToCollectables()
	{
		if (rewards == null || rewards.attributeCount == 0 || Singleton<Profile>.instance.GetWaveLevel(mWaveIndex) != 1)
		{
			return;
		}
		for (int i = 0; i < rewards.attributeCount; i++)
		{
			string[] array = rewards[i].Split(',');
			switch (array[0])
			{
			case "coins":
				WeakGlobalInstance<CollectableManager>.instance.GiveResource(ECollectableType.coin, int.Parse(array[1]));
				break;
			case "gems":
				WeakGlobalInstance<CollectableManager>.instance.GiveResource(ECollectableType.gem, int.Parse(array[1]));
				break;
			case "balls":
				WeakGlobalInstance<CollectableManager>.instance.GiveResource(ECollectableType.pachinkoball, int.Parse(array[1]));
				break;
			default:
				WeakGlobalInstance<CollectableManager>.instance.GiveSpecificPresent(array[0]);
				break;
			}
		}
	}

	public static void ApplySpecialEvent(SDFTreeNode enemiesData)
	{
		if (Singleton<EventsManager>.instance.IsEventActive("halloween") && Singleton<PlayModesManager>.instance.selectedMode == "classic")
		{
			SDFTreeNode sDFTreeNode = enemiesData.to("L_Zombie_Melee");
			sDFTreeNode["prefab"] = "Characters/PFLzombiePumpkin";
			sDFTreeNode = enemiesData.to("L_Zombie_Melee_Tutorial");
			sDFTreeNode["prefab"] = "Characters/PFLzombiePumpkin";
		}
	}

	public static SDFTreeNode GetWaveData(int waveID)
	{
		return GetWaveData(waveID, false);
	}

	public static SDFTreeNode GetWaveData(int waveID, bool bonusWave)
	{
		int num = waveID;
		while (num > 0)
		{
			string waveFilePath = GetWaveFilePath(waveID, bonusWave);
			if (Resources.Load(waveFilePath, typeof(TextAsset)) == null)
			{
				Debug.Log("WARNING: Could not find wave #" + num + ", will try with " + (num - 1));
				num--;
				continue;
			}
			return FilterEventData(SingletonMonoBehaviour<ResourcesManager>.instance.Open(waveFilePath));
		}
		return null;
	}

	private static SDFTreeNode FilterEventData(SDFTreeNode data)
	{
		if (Singleton<EventsManager>.instance.IsEventActive("halloween"))
		{
			if (data["scene"] == "InGame")
			{
				data["scene"] = "InGame_summernight";
			}
			else if (data["scene"] == "InGame_evil")
			{
				data["scene"] = "InGame_evilnight";
			}
			else if (data["scene"] == "InGame_fallday")
			{
				data["scene"] = "InGame_fallnight";
			}
			else if (data["scene"] == "InGame_springday")
			{
				data["scene"] = "InGame_springnight";
			}
			else if (data["scene"] == "InGame_winterday")
			{
				data["scene"] = "InGame_winternight";
			}
		}
		return data;
	}

	public static string GetWaveFilePath(int waveID)
	{
		return GetWaveFilePath(waveID, false);
	}

	public static string GetWaveFilePath(int waveID, bool bonusWave)
	{
		string text = "Registry/";
		text = ((!bonusWave && !Singleton<Profile>.instance.inBonusWave) ? (text + Singleton<PlayModesManager>.instance.selectedModeData["wavesPath"]) : (text + "BonusWaves/BonusWave"));
		return text + string.Format("{0:000}", waveID);
	}

	private void LoadData()
	{
		enemiesData = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/Enemies");
		ApplySpecialEvent(enemiesData);
		waveRootData = GetWaveData(mWaveIndex);
		waveCommands = waveRootData.to("Commands");
		rewards = waveRootData.to("rewards");
		if (waveRootData.hasAttribute("tutorial"))
		{
			mTutorial = waveRootData["tutorial"];
		}
		if (waveRootData.hasAttribute("villageArchers"))
		{
			mVillageArchersLevel = int.Parse(waveRootData["villageArchers"]);
		}
		if (waveRootData.hasAttribute("bell"))
		{
			mBellLevel = int.Parse(waveRootData["bell"]);
		}
		LoadLevelMultipliers();
	}

	private void LoadLevelMultipliers()
	{
		int num = Singleton<Profile>.instance.GetWaveLevel(mWaveIndex);
		if (num >= 2)
		{
			SDFTreeNode sDFTreeNode = Singleton<Config>.instance.root.to("waveLevelMultipliers");
			mLevelMultipliers.enemiesHealth = CalcMultiplier(float.Parse(sDFTreeNode["enemiesHealth"]), num - 1);
			mLevelMultipliers.enemiesDamages = CalcMultiplier(float.Parse(sDFTreeNode["enemiesDamages"]), num - 1);
			mLevelMultipliers.drops = CalcMultiplier(float.Parse(sDFTreeNode["drops"]), num - 1);
		}
	}

	private void RunNextCommand()
	{
		if (!waveCommands.hasAttribute(mNextCommandToRun))
		{
			return;
		}
		string text = waveCommands[mNextCommandToRun];
		mNextCommandToRun++;
		switch (GetCommandType(text))
		{
		case CommandType.UserDefined:
			WeakGlobalSceneBehavior<InGameImpl>.instance.RunSpecialWaveCommand(text.ToLower());
			break;
		case CommandType.Delay:
		{
			KeyValuePair<float, float> keyValuePair = ExtractTimer(text);
			mSpawnDelayTimer += UnityEngine.Random.value * (keyValuePair.Value - keyValuePair.Key) + keyValuePair.Key;
			break;
		}
		case CommandType.Spawn:
		{
			bool flag = false;
			foreach (string mAllDifferentEnemy in mAllDifferentEnemies)
			{
				if (string.Compare(mAllDifferentEnemy, text, true) == 0)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				WeakGlobalInstance<CharactersManager>.instance.AddCharacter(ConstructEnemy(text));
			}
			else
			{
				Debug.Log("WARNING: Unknown enemy ID in wave data: " + text);
			}
			break;
		}
		case CommandType.LegionTag:
			if (text[0] != '(')
			{
				break;
			}
			if (mSkipNextLegion)
			{
				SkipToEndOfLegion();
				mSkipNextLegion = false;
			}
			else if (mSpawnedEnemiesSoFar == mEnemiesKilledSoFar)
			{
				WeakGlobalSceneBehavior<InGameImpl>.instance.RunSpecialWaveCommand("@legionalert");
				if (this.onLegionStart != null)
				{
					this.onLegionStart();
				}
			}
			else
			{
				mNextCommandToRun--;
			}
			break;
		}
	}

	private void SkipToEndOfLegion()
	{
		while (mNextCommandToRun < waveCommands.attributeCount)
		{
			string text = waveCommands[mNextCommandToRun];
			CommandType commandType = GetCommandType(text);
			mNextCommandToRun++;
			switch (commandType)
			{
			case CommandType.Spawn:
				mEnemiesKilledSoFar++;
				break;
			case CommandType.LegionTag:
				if (text[0] == ')')
				{
					return;
				}
				break;
			}
		}
	}

	private KeyValuePair<float, float> ExtractTimer(string cmd)
	{
		string[] array = cmd.Split(',');
		if (array.Length != 2)
		{
			return new KeyValuePair<float, float>(-1f, -1f);
		}
		return new KeyValuePair<float, float>(float.Parse(array[0]), float.Parse(array[1]));
	}

	private void UpdateDelayTimer()
	{
		if (isDone)
		{
			return;
		}
		mSpawnDelayTimer -= Time.deltaTime;
		if (!(mSpawnDelayTimer > 0f))
		{
			mSpawnDelayTimer = 0f;
			if (WeakGlobalInstance<CharactersManager>.instance.enemiesCount < 10)
			{
				RunNextCommand();
			}
		}
	}

	private Enemy ConstructEnemy(string enemyID)
	{
		SDFTreeNode sDFTreeNode = enemiesData.to(enemyID);
		if (sDFTreeNode == null)
		{
			Debug.Log("ERROR: Unkown enemy: " + enemyID);
			return null;
		}
		return SpawnEnemy(enemyID, sDFTreeNode, mEnemiesSpawnArea.size.x, mEnemiesSpawnArea.transform.position, false);
	}

	public Enemy ConstructEnemy(string enemyID, float sizeOfSpawnArea, Vector3 spawnPos, bool dynamicSpawn)
	{
		SDFTreeNode sDFTreeNode = enemiesData.to(enemyID);
		if (sDFTreeNode == null)
		{
			Debug.Log("ERROR: Unkown enemy: " + enemyID);
			return null;
		}
		return SpawnEnemy(enemyID, sDFTreeNode, sizeOfSpawnArea, spawnPos, dynamicSpawn);
	}

	private Enemy SpawnEnemy(string uniqueID, SDFTreeNode data, float sizeOfSpawnArea, Vector3 spawnPos, bool dynamicSpawn)
	{
		mSpawnedEnemiesSoFar++;
		bool result = false;
		bool.TryParse(data["flying"], out result);
		float result2 = 0f;
		float.TryParse(data["bowRange"], out result2);
		CharactersManager.ELanePreference preference = CharactersManager.ELanePreference.any;
		if (data.hasAttribute("lane"))
		{
			preference = (CharactersManager.ELanePreference)(int)Enum.Parse(typeof(CharactersManager.ELanePreference), data["lane"]);
		}
		spawnPos.x = WeakGlobalInstance<CharactersManager>.instance.GetBestSpawnXPos(spawnPos, sizeOfSpawnArea, preference, true, result, result2 > 0f);
		Enemy e = new Enemy(data["prefab"], mZTarget, spawnPos);
		e.uniqueID = uniqueID;
		UnityEngine.Object.Instantiate((!dynamicSpawn) ? mSpawnEffect : mAltSpawnEffect, e.position, Quaternion.identity);
		e.maxHealth = float.Parse(data["health"]);
		e.health = e.maxHealth;
		float result3 = 0f;
		float.TryParse(data["meleeRange"], out result3);
		e.meleeAttackRange = result3;
		float.TryParse(data["meleeDamage"], out result3);
		e.meleeDamage = result3;
		e.bowAttackRange = result2;
		float.TryParse(data["bowDamage"], out result3);
		e.bowDamage = result3;
		float.TryParse(data["damageBuffPercent"], out result3);
		e.damageBuffPercent = result3;
		if (data.hasAttribute("projectile"))
		{
			e.bowProjectile = (Projectile.EProjectileType)(int)Enum.Parse(typeof(Projectile.EProjectileType), data["projectile"]);
		}
		bool result4 = false;
		bool.TryParse(data["usesBladeWeapon"], out result4);
		e.meleeWeaponIsABlade = result4;
		bool.TryParse(data["gateRusher"], out result4);
		e.isGateRusher = result4;
		bool.TryParse(data["exploseOnMelee"], out result4);
		e.exploseOnMelee = result4;
		float.TryParse(data["meleeFreeze"], out result3);
		e.meleeFreeze = result3;
		e.isFlying = result;
		float.TryParse(data["autoHealthRecovery"], out result3);
		e.autoHealthRecovery = result3;
		bool.TryParse(data["boss"], out result4);
		e.isBoss = result4;
		e.meleeAttackFrequency = float.Parse(data["attackFrequency"]);
		e.bowAttackFrequency = e.meleeAttackFrequency;
		e.spawnFriendID = data["spawnFriendID"];
		float num = float.Parse(data["speedMin"]);
		float num2 = float.Parse(data["speedMax"]);
		e.controller.speed = UnityEngine.Random.value * (num2 - num) + num;
		if (data.hasAttribute("knockbackPower"))
		{
			e.knockbackPower = int.Parse(data["knockbackPower"]);
		}
		if (data.hasAttribute("knockbackResistance"))
		{
			e.knockbackResistance = int.Parse(data["knockbackResistance"]);
		}
		e.resourceDrops.amountDropped.max = int.Parse(data["resourceDropMax"]);
		e.resourceDrops.amountDropped.min = int.Parse(data["resourceDropMin"]);
		e.resourceDrops.guaranteedCoinsAward = int.Parse(data["award"]);
		e.maxHealth *= mLevelMultipliers.enemiesHealth;
		e.health = e.maxHealth;
		e.meleeDamage *= mLevelMultipliers.enemiesDamages;
		e.bowDamage *= mLevelMultipliers.enemiesDamages;
		if (WeakGlobalSceneBehavior<InGameImpl>.instance.activeCharm == "peace" || WeakGlobalSceneBehavior<InGameImpl>.instance.activeCharm.Contains("+peace"))
		{
			float num3 = float.Parse(Singleton<CharmsDatabase>.instance.GetAttribute(WeakGlobalSceneBehavior<InGameImpl>.instance.activeCharm, "damageMultiplier"));
			e.meleeDamage *= num3;
			e.bowDamage *= num3;
		}
		if (data.hasAttribute("weaponMeleePrefab"))
		{
			e.meleeWeaponPrefab = Resources.Load(data["weaponMeleePrefab"]) as GameObject;
		}
		if (e.meleeWeaponPrefab != null && (WeakGlobalSceneBehavior<InGameImpl>.instance.activeCharm == "peace" || WeakGlobalSceneBehavior<InGameImpl>.instance.activeCharm.Contains("+peace")))
		{
			e.meleeWeaponPrefab = Resources.Load("Props/PFChickenSword") as GameObject;
		}
		if (data.hasAttribute("weaponRangedPrefab"))
		{
			e.rangedWeaponPrefab = Resources.Load(data["weaponRangedPrefab"]) as GameObject;
		}
		if (data.hasAttribute("spawnOnDeath"))
		{
			int spawnCount = 1;
			if (data.hasAttribute("spawnOnDeathCount"))
			{
				spawnCount = int.Parse(data["spawnOnDeathCount"]);
			}
			Enemy enemy = e;
			enemy.onDeathEvent = (Action)Delegate.Combine(enemy.onDeathEvent, (Action)delegate
			{
				e.SpawnOffspring(data["spawnOnDeath"], spawnCount);
			});
		}
		if (data.hasAttribute("resourceDropAlways"))
		{
			Character selfPtr = e;
			Enemy enemy2 = e;
			enemy2.onDeathEvent = (Action)Delegate.Combine(enemy2.onDeathEvent, (Action)delegate
			{
				SpawnExtraCollectable(data["resourceDropAlways"], selfPtr);
			});
		}
		return e;
	}

	private void SpawnExtraCollectable(string collectableType, Character theEnemy)
	{
		if (theEnemy != null)
		{
			WeakGlobalInstance<CollectableManager>.instance.ForceSpawnResourceType(collectableType, theEnemy.position);
		}
	}

	private void AnalyseWaveCommandsForStats()
	{
		mTotalNumEnemies = 0;
		mAllDifferentEnemies.Clear();
		for (int i = 0; waveCommands.hasAttribute(i); i++)
		{
			string data = waveCommands[i].ToLower();
			switch (GetCommandType(data))
			{
			case CommandType.Spawn:
				mTotalNumEnemies++;
				if (enemiesData.to(data) == null)
				{
					Debug.Log("WARNING: Unknown enemy ID in wave: " + data);
				}
				else if (!mAllDifferentEnemies.Exists((string element) => element == data))
				{
					mAllDifferentEnemies.Add(data);
				}
				break;
			case CommandType.LegionTag:
				if (data[0] == '(')
				{
					mLegionMarkers.Add(mTotalNumEnemies);
				}
				break;
			}
		}
		Singleton<PlayStatistics>.instance.stats.lastWaveTotalEnemies = mTotalNumEnemies;
	}

	private CommandType GetCommandType(string cmd)
	{
		if (cmd.Length == 0)
		{
			return CommandType.Unknown;
		}
		if (cmd[0] == '@')
		{
			return CommandType.UserDefined;
		}
		if (cmd[0] == '(' || cmd[0] == ')')
		{
			return CommandType.LegionTag;
		}
		if (ExtractTimer(cmd).Key != -1f)
		{
			return CommandType.Delay;
		}
		return CommandType.Spawn;
	}

	private static float CalcMultiplier(float mult, int times)
	{
		float num = mult;
		num -= 1f;
		num *= (float)times;
		return num + 1f;
	}
}
