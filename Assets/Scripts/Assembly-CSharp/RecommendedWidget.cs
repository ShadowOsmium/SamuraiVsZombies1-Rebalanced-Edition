public class RecommendedWidget : SUISprite
{
	private SUILayoutEffect.Effect mPulseEffect;

	public RecommendedWidget()
	{
		base.texture = "Sprites/Localized/recommended!";
		mPulseEffect = new SUILayoutEffect.ScalePingPong(this, 0.75f, 0.95f, 0.95f);
	}

	public override void Destroy()
	{
		mPulseEffect = null;
		base.Destroy();
	}

	public override void Update()
	{
		base.Update();
		mPulseEffect.Update();
	}
}
