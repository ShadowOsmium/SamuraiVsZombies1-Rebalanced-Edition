public class TutorialHookup : WeakGlobalInstance<TutorialHookup>
{
	public bool playerPressedLeft;

	public bool playerPressedRight;

	public bool enemyIsInRangeOFAttack;

	public bool usedAbility;

	public bool summonedAlly;

	public bool flyingEnemyInView;

	public bool showKatanaSlash;

	public bool firstHelperAvailable;

	public bool[] storeTabTouched = new bool[3];

	public TutorialHookup()
	{
		SetUniqueInstance(this);
	}

	public void Destroy()
	{
		SetUniqueInstance(null);
	}
}
