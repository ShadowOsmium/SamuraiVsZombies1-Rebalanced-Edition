using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : WeakGlobalInstance<ProjectileManager>
{
	private List<Projectile> mProjectiles = new List<Projectile>();

	private GameObject[] mProjectilePrefabs = new GameObject[Enum.GetNames(typeof(Projectile.EProjectileType)).Length];

	public ProjectileManager()
	{
		SetUniqueInstance(this);
		mProjectilePrefabs[1] = Resources.Load("Props/PFArrow") as GameObject;
		mProjectilePrefabs[2] = Resources.Load("Props/PFArrow") as GameObject;
		mProjectilePrefabs[3] = Resources.Load("Props/PFArrow") as GameObject;
		mProjectilePrefabs[4] = Resources.Load("Props/PFArrow") as GameObject;
		mProjectilePrefabs[5] = Resources.Load("Props/PFArrow") as GameObject;
		mProjectilePrefabs[6] = Resources.Load("Props/PFBullet") as GameObject;
		mProjectilePrefabs[7] = Resources.Load("Props/PFArrowIce") as GameObject;
		if (Singleton<PlayModesManager>.instance.selectedMode == "classic")
		{
			mProjectilePrefabs[8] = Resources.Load("FX/HealBolt") as GameObject;
			mProjectilePrefabs[9] = Resources.Load("FX/EvilHealBolt") as GameObject;
		}
		else
		{
			mProjectilePrefabs[8] = Resources.Load("FX/EvilHealBolt") as GameObject;
			mProjectilePrefabs[9] = Resources.Load("FX/HealBolt") as GameObject;
		}
		mProjectilePrefabs[10] = Resources.Load("Props/PFAssassinStar") as GameObject;
		mProjectilePrefabs[11] = Resources.Load("Props/PFArrowHero") as GameObject;
		mProjectilePrefabs[12] = Resources.Load("Props/PFArrowHeroUpgrade1") as GameObject;
		mProjectilePrefabs[13] = Resources.Load("Props/PFArrowHeroUpgrade2") as GameObject;
		mProjectilePrefabs[14] = Resources.Load("Props/PFArrowHeroUpgrade2") as GameObject;
		mProjectilePrefabs[15] = Resources.Load("Props/PFArrowHeroUpgrade2") as GameObject;
		mProjectilePrefabs[16] = Resources.Load("Props/PFZombieHeroArrow") as GameObject;
		mProjectilePrefabs[17] = Resources.Load("Props/PFZombieHeroArrowUpgrade") as GameObject;
	}

	public void Update()
	{
		for (int num = mProjectiles.Count - 1; num >= 0; num--)
		{
			Projectile projectile = mProjectiles[num];
			projectile.Update();
			if (projectile.isDone)
			{
				projectile.Destroy();
				mProjectiles.RemoveAt(num);
			}
		}
	}

	public void SpawnProjectile(Projectile.EProjectileType type, float damage, Character shooter, Character target, Vector3 spawnPos)
	{
		if (type != 0 && type != Projectile.EProjectileType.SpawnFriend)
		{
			mProjectiles.Add(new Arrow(type, shooter, target, damage, spawnPos));
		}
	}

	public GameObject PrefabForProjectile(Projectile.EProjectileType type)
	{
		return mProjectilePrefabs[(int)type];
	}

	public static bool ProjectileArcs(Projectile.EProjectileType type)
	{
		switch (type)
		{
		case Projectile.EProjectileType.Arrow:
		case Projectile.EProjectileType.FireArrow:
		case Projectile.EProjectileType.BlueFireArrow:
		case Projectile.EProjectileType.ExplodingArrow:
		case Projectile.EProjectileType.BigExplodingArrow:
		case Projectile.EProjectileType.IceArrow:
			return true;
		default:
			return false;
		}
	}

	public static bool ProjectileNeedsBothHands(Projectile.EProjectileType type)
	{
		switch (type)
		{
		case Projectile.EProjectileType.Arrow:
		case Projectile.EProjectileType.FireArrow:
		case Projectile.EProjectileType.BlueFireArrow:
		case Projectile.EProjectileType.ExplodingArrow:
		case Projectile.EProjectileType.BigExplodingArrow:
		case Projectile.EProjectileType.Bullet:
		case Projectile.EProjectileType.IceArrow:
		case Projectile.EProjectileType.HeroArrow:
		case Projectile.EProjectileType.HeroFireArrow:
		case Projectile.EProjectileType.HeroBlueFireArrow:
		case Projectile.EProjectileType.HeroExplodingArrow:
		case Projectile.EProjectileType.HeroBigExplodingArrow:
		case Projectile.EProjectileType.BoneArrow:
		case Projectile.EProjectileType.BoneArrowUpgraded:
			return true;
		default:
			return false;
		}
	}

	public static bool ProjectileShownWhileAiming(Projectile.EProjectileType type)
	{
		switch (type)
		{
		case Projectile.EProjectileType.Arrow:
		case Projectile.EProjectileType.FireArrow:
		case Projectile.EProjectileType.BlueFireArrow:
		case Projectile.EProjectileType.ExplodingArrow:
		case Projectile.EProjectileType.BigExplodingArrow:
		case Projectile.EProjectileType.IceArrow:
		case Projectile.EProjectileType.HeroArrow:
		case Projectile.EProjectileType.HeroFireArrow:
		case Projectile.EProjectileType.HeroBlueFireArrow:
		case Projectile.EProjectileType.HeroExplodingArrow:
		case Projectile.EProjectileType.HeroBigExplodingArrow:
		case Projectile.EProjectileType.BoneArrow:
		case Projectile.EProjectileType.BoneArrowUpgraded:
			return true;
		default:
			return false;
		}
	}

	public static Vector3 ProjectileAimPosForTarget(Projectile.EProjectileType type, Vector3 spawnPos, Vector3 targetPos)
	{
		if (ProjectileArcs(type))
		{
			if (Mathf.Abs(spawnPos.z - targetPos.z) <= 300f)
			{
				return targetPos;
			}
			float num = Vector3.Distance(spawnPos, targetPos) - 20f;
			float num2 = num * 0.2f;
			Vector3 result = (targetPos + spawnPos) * 0.5f;
			result.y += num2 * 2f;
			return result;
		}
		return targetPos;
	}
}
