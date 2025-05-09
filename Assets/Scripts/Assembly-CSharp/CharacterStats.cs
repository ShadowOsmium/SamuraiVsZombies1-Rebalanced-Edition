public struct CharacterStats
{
	public string uniqueID;

	public bool isUnique;

	public bool isEnemy;

	public bool isPlayer;

	public bool isBase;

	public bool isBoss;

	public bool isGateRusher;

	public bool isFlying;

	public bool exploseOnMelee;

	public float meleeFreeze;

	public float health;

	public float maxHealth;

	public float autoHealthRecovery;

	public int knockbackPower;

	public int knockbackResistance;

	public float meleeAttackRange;

	public float meleeAttackDamage;

	public float meleeAttackFrequency;

	public bool meleeWeaponIsABlade;

	public float bowAttackRange;

	public float bowAttackDamage;

	public float bowAttackFrequency;

	public Projectile.EProjectileType projectile;

	public float speed;

	public float criticalChance;

	public float criticalMultiplier;

	public float damageBuffPercent;

	public string upgradeAlliesFrom;

	public string upgradeAlliesTo;

	public string spawnFriendID;
}
