using System;
using System.Collections.Generic;
using UnityEngine;

public class CharactersManager : WeakGlobalInstance<CharactersManager>
{
	public enum ELanePreference
	{
		any = 0,
		front = 1,
		back = 2,
		center = 3
	}

	private const float kCloseDistRange = 45f;

	private const float kMinTimeBetweenAttacks = 0.25f;

	private const float kLaneWidth = 15f;

	private const float kLaneCharacterValue = 2500f;

	private List<Character> mCharacters = new List<Character>();

	private List<Character> mDyingCharacters = new List<Character>();

	public int helpersCount
	{
		get
		{
			int num = 0;
			foreach (Character mCharacter in mCharacters)
			{
				if (!mCharacter.isEnemy)
				{
					num++;
				}
			}
			return num - 1;
		}
	}

	public int enemiesCount
	{
		get
		{
			int num = 0;
			foreach (Character mCharacter in mCharacters)
			{
				if (mCharacter.isEnemy)
				{
					num++;
				}
			}
			return num;
		}
	}

	public float enemyMeleePreAttackDelay { get; private set; }

	public float enemyRangedPreAttackDelay { get; private set; }

	public float allyMeleePreAttackDelay { get; private set; }

	public float allyRangedPreAttackDelay { get; private set; }

	public Action postUpdateFunc { get; set; }

	public List<Character> allAlive
	{
		get
		{
			return mCharacters;
		}
	}

	public List<Character> allEnemies
	{
		get
		{
			List<Character> list = new List<Character>();
			foreach (Character mCharacter in mCharacters)
			{
				if (mCharacter.isEnemy)
				{
					list.Add(mCharacter);
				}
			}
			return list;
		}
	}

	public CharactersManager()
	{
		SetUniqueInstance(this);
	}

	public void Update()
	{
		if (enemyMeleePreAttackDelay > 0f)
		{
			enemyMeleePreAttackDelay = Mathf.Max(0f, enemyMeleePreAttackDelay - Time.deltaTime);
		}
		if (enemyRangedPreAttackDelay > 0f)
		{
			enemyRangedPreAttackDelay = Mathf.Max(0f, enemyRangedPreAttackDelay - Time.deltaTime);
		}
		if (allyMeleePreAttackDelay > 0f)
		{
			allyMeleePreAttackDelay = Mathf.Max(0f, allyMeleePreAttackDelay - Time.deltaTime);
		}
		if (allyRangedPreAttackDelay > 0f)
		{
			allyRangedPreAttackDelay = Mathf.Max(0f, allyRangedPreAttackDelay - Time.deltaTime);
		}
		foreach (Character mCharacter in mCharacters)
		{
			mCharacter.Update();
		}
		foreach (Character mDyingCharacter in mDyingCharacters)
		{
			mDyingCharacter.Update();
		}
		UpdateBattle();
		if (postUpdateFunc != null)
		{
			postUpdateFunc();
			postUpdateFunc = null;
		}
	}

	public void AddCharacter(Character c)
	{
		mCharacters.Add(c);
	}

	public void DestroyCharacter(Character c)
	{
		mCharacters.Remove(c);
		mDyingCharacters.Remove(c);
		c.Destroy();
	}

	public void RegisterAttackStarted(bool isEnemy, bool isRanged)
	{
		if (isEnemy && isRanged)
		{
			enemyRangedPreAttackDelay = 0.25f;
		}
		else if (isEnemy)
		{
			enemyMeleePreAttackDelay = 0.25f;
		}
		else if (isRanged)
		{
			allyRangedPreAttackDelay = 0.25f;
		}
		else
		{
			allyMeleePreAttackDelay = 0.25f;
		}
	}

	public Character GetBestRangedAttackTarget(Character attacker, GameRange range)
	{
		Character character = null;
		if (attacker == null)
		{
			return character;
		}
		List<Character> charactersInRange = GetCharactersInRange(range, !attacker.isEnemy);
		if (charactersInRange.Count == 0)
		{
			return character;
		}
		bool flag = !attacker.usesFocusedFire;
		float z = attacker.position.z;
		float num = float.MaxValue;
		float num2 = float.MaxValue;
		bool flag2 = false;
		int num3 = int.MaxValue;
		foreach (Character item in charactersInRange)
		{
			if (!(item.health > 0f))
			{
				continue;
			}
			bool flag3 = false;
			bool flag4 = item.isFlying || item.isInKnockback;
			float num4 = Mathf.Abs(item.position.z - z);
			int num5 = 0;
			if (flag)
			{
				num5 = item.DispersedAttackersCount(attacker);
			}
			if (character == null)
			{
				flag3 = true;
			}
			else
			{
				if (flag2 && !flag4)
				{
					continue;
				}
				if (flag4 && !flag2)
				{
					flag3 = true;
				}
				else
				{
					if (num4 > num + 45f)
					{
						continue;
					}
					if (flag && num5 < num3)
					{
						flag3 = true;
					}
					else if (num4 < num - 45f)
					{
						flag3 = true;
					}
					else
					{
						if (flag && num5 > num3)
						{
							continue;
						}
						if (item.health < num2)
						{
							flag3 = true;
						}
					}
				}
			}
			if (flag3)
			{
				character = item;
				num = num4;
				num2 = item.health;
				flag2 = item.isFlying;
				num3 = num5;
			}
		}
		return character;
	}

	public List<Character> GetCharactersInRange(GameRange range, bool areEnemies)
	{
		return GetCharactersInRange(range.left, range.right, areEnemies);
	}

	public List<Character> GetCharactersInRange(float zMin, float zMax, bool areEnemies)
	{
		List<Character> list = new List<Character>();
		foreach (Character mCharacter in mCharacters)
		{
			if (mCharacter.isEnemy == areEnemies && mCharacter.health > 0f && mCharacter.position.z >= zMin && mCharacter.position.z <= zMax && (!mCharacter.isPlayer || !Singleton<Profile>.instance.inBonusWave))
			{
				list.Add(mCharacter);
			}
		}
		return list;
	}

	public bool IsCharacterInRange(GameRange range, bool areEnemies, bool gateOnly, bool includeFlyers, bool injuredOnly, bool includeDecayingCharacters)
	{
		return IsCharacterInRange(range.left, range.right, areEnemies, gateOnly, includeFlyers, injuredOnly, includeDecayingCharacters);
	}

	public bool IsCharacterInRange(float zMin, float zMax, bool areEnemies, bool gateOnly, bool includeFlyers, bool injuredOnly, bool includeDecayingCharacters)
	{
		foreach (Character mCharacter in mCharacters)
		{
			if (mCharacter.isEnemy == areEnemies && mCharacter.position.z >= zMin && mCharacter.position.z <= zMax && mCharacter.health > 0f && (includeFlyers || !mCharacter.isInKnockback) && (!gateOnly || mCharacter.isBase) && (includeFlyers || !mCharacter.isFlying) && (!injuredOnly || mCharacter.health < mCharacter.maxHealth) && (!mCharacter.isPlayer || !Singleton<Profile>.instance.inBonusWave) && (mCharacter.autoHealthRecovery >= 0f || includeDecayingCharacters))
			{
				return true;
			}
		}
		return false;
	}

	public void idleAll()
	{
		foreach (Character mCharacter in mCharacters)
		{
			mCharacter.controller.Idle();
		}
	}

	public float GetBestSpawnXPos(Vector3 spawnPos, float sizeOfSpawnArea, ELanePreference preference, bool isEnemy, bool isFlyer, bool isRanged)
	{
		if (preference == ELanePreference.center)
		{
			return WeakGlobalSceneBehavior<InGameImpl>.instance.heroSpawnPoint.position.x;
		}
		if (sizeOfSpawnArea <= 0f)
		{
			return spawnPos.x;
		}
		float num = sizeOfSpawnArea / 2f;
		float num2 = spawnPos.x - num;
		float max = num2 + sizeOfSpawnArea;
		float num3 = 0f;
		if (preference == ELanePreference.back || preference == ELanePreference.front)
		{
			num3 = 750f;
		}
		int num4 = (int)Mathf.Ceil(sizeOfSpawnArea / 15f);
		float[] array = new float[num4];
		for (int i = 0; i < num4; i++)
		{
			array[i] = 0f;
		}
		int num5 = positionToLane(WeakGlobalSceneBehavior<InGameImpl>.instance.heroSpawnPoint.position.x, num2, num4);
		if (!isEnemy)
		{
			array[num5] = 25000f;
			if (num5 > 0)
			{
				array[num5 - 1] = 10000f;
			}
			if (num5 < num4 - 1)
			{
				array[num5 + 1] = 10000f;
			}
			if (num5 > 1)
			{
				array[num5 - 2] = 2500f;
			}
		}
		foreach (Character mCharacter in mCharacters)
		{
			if (mCharacter == null || !(mCharacter.health > 0f) || mCharacter.isEnemy != isEnemy || mCharacter.isFlying != isFlyer || mCharacter.bowAttackRange > 0f != isRanged)
			{
				continue;
			}
			int num6 = positionToLane(mCharacter.controller.xPos, num2, num4);
			float num7 = 2500f;
			num7 -= Mathf.Abs(mCharacter.position.z - spawnPos.z);
			if (num7 > 0f)
			{
				array[num6] += num7;
				num7 *= 0.5f;
				if (num6 > 0)
				{
					array[num6 - 1] += num7;
				}
				if (num6 < num4 - 1)
				{
					array[num6 + 1] += num7;
				}
				num7 *= 0.5f;
				if (num6 > 1)
				{
					array[num6 - 2] += num7;
				}
				if (num6 < num4 - 2)
				{
					array[num6 + 2] += num7;
				}
			}
		}
		int num8 = num5;
		switch (preference)
		{
		case ELanePreference.back:
			num8 = 0;
			break;
		case ELanePreference.front:
			num8 = num4 - 1;
			break;
		}
		float num9 = array[num8];
		for (int j = 0; j < num4; j++)
		{
			int num10 = j;
			switch (preference)
			{
			case ELanePreference.front:
				num10 = num4 - 1 - j;
				break;
			default:
				num10 = num5;
				if (((uint)j & (true ? 1u : 0u)) != 0)
				{
					num10 -= (j + 1) / 2;
					if (num10 < 0)
					{
						num10 = j;
						preference = ELanePreference.back;
					}
				}
				else
				{
					num10 += (j + 1) / 2;
					if (num10 >= num4)
					{
						num10 = num4 - 1 - j;
						preference = ELanePreference.front;
					}
				}
				break;
			case ELanePreference.back:
				break;
			}
			num10 = Mathf.Clamp(num10, 0, num4 - 1);
			float num11 = array[num10] + num3 * (float)j;
			if (num11 < num9)
			{
				num8 = num10;
				num9 = num11;
			}
		}
		float num12 = num2 + 15f * (float)num8 - 7.3500004f;
		float max2 = num12 + 13.5f;
		return Mathf.Clamp(UnityEngine.Random.Range(num12, max2), num2, max);
	}

	private void UpdateBattle()
	{
		for (int num = mCharacters.Count - 1; num >= 0; num--)
		{
			Character character = mCharacters[num];
			if (character.health <= 0f && !character.isBase)
			{
				if (character.isEnemy)
				{
					WeakGlobalInstance<WaveManager>.instance.registerEnemyKilled(character.uniqueID);
					if (character.lastAttackTypeHitWith == EAttackType.Trample)
					{
						Singleton<Profile>.instance.UndeadKilledViaTroopTrample++;
					}
					else if (character.lastAttackTypeHitWith == EAttackType.Slice)
					{
						Singleton<Profile>.instance.UndeadKilledViaKatanaSlash++;
					}
					if (Singleton<PlayStatistics>.instance != null)
					{
						Singleton<PlayStatistics>.instance.stats.lastWaveCoinsAward += character.resourceDrops.guaranteedCoinsAward;
					}
					Singleton<Profile>.instance.coins += character.resourceDrops.guaranteedCoinsAward;
					Vector3 jointPosition = character.controller.autoPaperdoll.GetJointPosition("impact_target");
					bool flag = WeakGlobalSceneBehavior<InGameImpl>.instance.activeCharm == "luck" || WeakGlobalSceneBehavior<InGameImpl>.instance.activeCharm.Contains("+luck");
					if (flag || (WeakGlobalInstance<WaveManager>.instance.isDone && enemiesCount <= 1))
					{
						character.resourceDrops.amountDropped.min = Mathf.Max(character.resourceDrops.amountDropped.min, 1);
						character.resourceDrops.amountDropped.max = Mathf.Max(character.resourceDrops.amountDropped.min, character.resourceDrops.amountDropped.max);
						WeakGlobalInstance<CollectableManager>.instance.SpawnResources(character.resourceDrops, jointPosition);
						if (flag)
						{
							UnityEngine.Object.Instantiate(Resources.Load("FX/Confetti"), jointPosition, Quaternion.identity);
						}
					}
					WeakGlobalInstance<CollectableManager>.instance.SpawnResources(character.resourceDrops, jointPosition);
				}
				mDyingCharacters.Add(character);
				mCharacters.RemoveAt(num);
			}
		}
		for (int num2 = mDyingCharacters.Count - 1; num2 >= 0; num2--)
		{
			Character character2 = mDyingCharacters[num2];
			if (character2.isOver && !character2.isPlayer && !character2.isBase)
			{
				character2.Destroy();
				mDyingCharacters.RemoveAt(num2);
			}
			else if (character2.health > 0f)
			{
				mCharacters.Add(character2);
				mDyingCharacters.RemoveAt(num2);
			}
		}
	}

	private int positionToLane(float xPos, float minX, int laneCount)
	{
		return Mathf.Clamp(Mathf.RoundToInt((xPos - minX) / 15f), 0, laneCount - 1);
	}
}
