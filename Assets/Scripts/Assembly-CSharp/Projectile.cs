public class Projectile
{
	public enum EProjectileType
	{
		None = 0,
		Arrow = 1,
		FireArrow = 2,
		BlueFireArrow = 3,
		ExplodingArrow = 4,
		BigExplodingArrow = 5,
		Bullet = 6,
		IceArrow = 7,
		HealBolt = 8,
		EvilHealBolt = 9,
		Shuriken = 10,
		HeroArrow = 11,
		HeroFireArrow = 12,
		HeroBlueFireArrow = 13,
		HeroExplodingArrow = 14,
		HeroBigExplodingArrow = 15,
		BoneArrow = 16,
		BoneArrowUpgraded = 17,
		SpawnFriend = 18
	}

	public const float kArcHeightDistPercent = 0.2f;

	public const float kMinZDistToArcShot = 300f;

	public EProjectileType type;

	public Character shooter;

	public virtual bool isDone
	{
		get
		{
			return false;
		}
	}

	public virtual void Update()
	{
	}

	public virtual void Destroy()
	{
	}
}
