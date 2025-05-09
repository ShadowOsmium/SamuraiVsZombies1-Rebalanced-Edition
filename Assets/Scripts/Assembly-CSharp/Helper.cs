using System;
using UnityEngine;

public class Helper : Character
{
	private bool mIsLeftToRightGameplay = true;

	public Helper(GameObject prefab, float zTarget, Vector3 pos, string weaponPropLocation, bool weaponIsRanged)
	{
		mIsLeftToRightGameplay = Singleton<PlayModesManager>.instance.gameDirection == PlayModesManager.GameDirection.LeftToRight;
		ActivateHealthBar();
		InitializeModel(prefab, weaponPropLocation, weaponIsRanged);
		base.controller.position = pos;
		if (mIsLeftToRightGameplay)
		{
			base.controller.constraintLeft = WeakGlobalInstance<Smithy>.instance.helperSpawnArea.transform.position.z;
			base.controller.constraintRight = zTarget;
		}
		else
		{
			base.controller.constraintLeft = zTarget;
			base.controller.constraintRight = WeakGlobalInstance<Smithy>.instance.helperSpawnArea.transform.position.z;
		}
		StartWalkingTowardGoal();
	}

	public void ReinitializeModel(GameObject prefab, string weaponPropLocation, bool weaponIsRanged)
	{
		Vector3 vector = base.controller.position;
		vector.x = base.controller.xPos;
		float constraintLeft = base.controller.constraintLeft;
		float constraintRight = base.controller.constraintRight;
		InitializeModel(prefab, weaponPropLocation, weaponIsRanged);
		base.controller.position = vector;
		base.controller.constraintLeft = constraintLeft;
		base.controller.constraintRight = constraintRight;
		StartWalkingTowardGoal();
	}

	private void InitializeModel(GameObject prefab, string weaponPropLocation, bool weaponIsRanged)
	{
		base.controlledObject = (GameObject)UnityEngine.Object.Instantiate(prefab);
		if (weaponPropLocation != null)
		{
			if (weaponIsRanged)
			{
				base.rangedWeaponPrefab = Resources.Load(weaponPropLocation) as GameObject;
			}
			else
			{
				base.meleeWeaponPrefab = Resources.Load(weaponPropLocation) as GameObject;
			}
		}
		if (base.bowAttackRange > 0f && base.meleeAttackRange == 0f)
		{
			SetRangeAttackMode(true);
		}
	}

	public override void Update()
	{
		base.Update();
		if (base.health == 0f || !base.isEffectivelyIdle || base.controller.impactPauseTime > 0f)
		{
			return;
		}
		bool flag = ((!mIsLeftToRightGameplay) ? (base.controller.position.z <= base.controller.constraintLeft) : (base.controller.position.z >= base.controller.constraintRight));
		if (WeakGlobalSceneBehavior<InGameImpl>.instance.playerWon)
		{
			base.controller.PlayVictoryAnim();
		}
		else if (base.canMeleeAttack && IsInMeleeRangeOfOpponent(true))
		{
			base.controller.SetUseCowerIdle(false);
			SetRangeAttackMode(false);
			if (base.exploseOnMelee)
			{
				AttackByExploding();
				return;
			}
			StartMeleeAttackDelayTimer();
			base.controller.Attack(base.meleeAttackFrequency);
		}
		else if ((base.isSpellCasterOnAllies && base.canUseRangedAttack && IsInRangeOfHurtAlly(false)) || (base.canUseRangedAttack && base.stats.projectile == Projectile.EProjectileType.SpawnFriend && IsInBowRangeOfOpponent()))
		{
			StartRangedAttackDelayTimer();
			if (base.bowProjectile == Projectile.EProjectileType.SpawnFriend)
			{
				UnityEngine.Object.Instantiate(Resources.Load("FX/DivineEnemy"), base.position + new Vector3(50f, 0f, 0f), Quaternion.identity);
				base.controller.PerformSpecialAction("cast", base.OnCastSpawnAllies, base.bowAttackFrequency / 2f);
			}
			else
			{
				base.controller.PerformSpecialAction("cast", base.OnCastHeal, base.bowAttackFrequency);
			}
		}
		else if (base.canUseRangedAttack && base.hasRangedAttack && IsInBowRangeOfOpponent())
		{
			SetRangeAttackMode(true);
			StartRangedAttackDelayTimer();
			base.singleAttackTarget = WeakGlobalInstance<CharactersManager>.instance.GetBestRangedAttackTarget(this, base.bowAttackExtGameRange);
			if (base.singleAttackTarget != null)
			{
				base.controller.RangeAttack(base.bowAttackFrequency);
			}
		}
		else if (IsInMeleeRangeOfOpponent(false))
		{
			base.controller.SetUseCowerIdle(!IsInMeleeRangeOfOpponent(true));
			base.controller.Idle();
		}
		else if (!base.canMove || flag || IsInBowRangeOfOpponent() || (base.isSpellCasterOnAllies && IsInRangeOfHurtAlly()))
		{
			base.controller.SetUseCowerIdle(false);
			base.controller.Idle();
		}
		else
		{
			base.controller.SetUseCowerIdle(false);
			StartWalkingTowardGoal();
		}
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	public void SetSpawnOnDeath(string spawnHelperID, int num)
	{
		base.onDeathEvent = (Action)Delegate.Combine(base.onDeathEvent, (Action)delegate
		{
			SpawnOffspring(spawnHelperID, num);
		});
	}

	private void StartWalkingTowardGoal()
	{
		if (mIsLeftToRightGameplay)
		{
			base.controller.StartWalkRight();
		}
		else
		{
			base.controller.StartWalkLeft();
		}
	}
}
