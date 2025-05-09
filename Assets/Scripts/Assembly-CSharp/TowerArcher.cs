using UnityEngine;

public class TowerArcher : Character
{
	public TowerArcher(string model, Vector3 pos, string weaponPropLocation, Projectile.EProjectileType projectileType, float damage, float range, float attackFrequency, bool againstPlayer)
	{
		CharacterStats characterStats = new CharacterStats
		{
			maxHealth = 100f,
			health = 100f,
			bowAttackFrequency = attackFrequency,
			bowAttackDamage = damage,
			bowAttackRange = range,
			meleeAttackRange = 150f,
			projectile = projectileType,
			isEnemy = againstPlayer
		};
		base.controlledObject = (GameObject)Object.Instantiate(Resources.Load(model));
		base.controller.position = pos;
		base.controller.facing = FacingType.Right;
		base.stats = characterStats;
		if (weaponPropLocation != null)
		{
			base.rangedWeaponPrefab = Resources.Load(weaponPropLocation) as GameObject;
		}
		SetRangeAttackMode(true);
	}

	public override void Update()
	{
		base.Update();
		if (base.health != 0f && base.isEffectivelyIdle)
		{
			if (WeakGlobalSceneBehavior<InGameImpl>.instance.playerWon && !base.isEnemy)
			{
				base.controller.PlayVictoryAnim();
			}
			else if (base.canUseRangedAttack && IsInBowRangeOfOpponent())
			{
				StartRangedAttackDelayTimer();
				base.singleAttackTarget = WeakGlobalInstance<CharactersManager>.instance.GetBestRangedAttackTarget(this, base.bowAttackGameRange);
				base.controller.RangeAttack(base.bowAttackFrequency);
			}
		}
	}
}
