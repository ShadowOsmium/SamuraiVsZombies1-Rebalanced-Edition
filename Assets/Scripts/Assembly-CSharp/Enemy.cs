using UnityEngine;

public class Enemy : Character
{
	private bool mIsLeftToRightGameplay = true;

	public Enemy(string model, float zTarget, Vector3 pos)
	{
		base.isEnemy = true;
		mIsLeftToRightGameplay = Singleton<PlayModesManager>.instance.gameDirection == PlayModesManager.GameDirection.LeftToRight;
		ActivateHealthBar();
		base.controlledObject = (GameObject)Object.Instantiate(Resources.Load(model));
		if (Singleton<PlayModesManager>.instance.selectedMode != "classic")
		{
			SoundThemePlayer component = base.controlledObject.GetComponent<SoundThemePlayer>();
			if (component != null)
			{
				component.autoPlayEvent = string.Empty;
			}
		}
		base.controller.position = pos;
		if (mIsLeftToRightGameplay)
		{
			base.controller.constraintLeft = zTarget;
			base.controller.constraintRight = WeakGlobalInstance<WaveManager>.instance.enemiesSpawnArea.transform.position.z;
		}
		else
		{
			base.controller.constraintLeft = WeakGlobalInstance<WaveManager>.instance.enemiesSpawnArea.transform.position.z;
			base.controller.constraintRight = zTarget;
		}
		StartWalkingTowardGoal();
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
		if (base.isFlying && WeakGlobalInstance<TutorialHookup>.instance != null)
		{
			float num = base.position.z - WeakGlobalSceneBehavior<InGameImpl>.instance.hero.position.z;
			if (num > 0f && num < 250f)
			{
				WeakGlobalInstance<TutorialHookup>.instance.flyingEnemyInView = true;
			}
		}
		bool flag = ((!mIsLeftToRightGameplay) ? (base.controller.position.z >= base.controller.constraintRight) : (base.controller.position.z <= base.controller.constraintLeft));
		if (base.canMeleeAttack && IsInMeleeRangeOfOpponent(true))
		{
			SetRangeAttackMode(false);
			if (base.exploseOnMelee)
			{
				AttackByExploding();
				return;
			}
			StartMeleeAttackDelayTimer();
			base.controller.Attack(base.meleeAttackFrequency);
		}
		else if (base.isSpellCasterOnAllies && base.canUseRangedAttack && IsInRangeOfHurtAlly())
		{
			StartRangedAttackDelayTimer();
			if (base.bowProjectile == Projectile.EProjectileType.SpawnFriend)
			{
				base.controller.PerformSpecialAction("cast", base.OnCastSpawnAllies, base.bowAttackFrequency);
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
		else if (!base.canMove || flag || IsInMeleeRangeOfOpponent(false) || IsInBowRangeOfOpponent() || (base.isSpellCasterOnAllies && IsInRangeOfHurtAlly()))
		{
			base.controller.Idle();
		}
		else if (WeakGlobalSceneBehavior<InGameImpl>.instance.enemiesWon)
		{
			base.controller.PlayVictoryAnim();
		}
		else
		{
			StartWalkingTowardGoal();
		}
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	private void StartWalkingTowardGoal()
	{
		if (mIsLeftToRightGameplay)
		{
			base.controller.StartWalkLeft();
		}
		else
		{
			base.controller.StartWalkRight();
		}
	}
}
