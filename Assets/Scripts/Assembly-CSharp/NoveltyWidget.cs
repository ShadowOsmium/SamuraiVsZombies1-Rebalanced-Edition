public class NoveltyWidget : SUISprite
{
	private SUILayoutEffect.Effect mPulseEffect;

	public NoveltyWidget()
	{
		base.texture = "Sprites/Menus/new_notification";
		mPulseEffect = new SUILayoutEffect.ScalePingPong(this, 0.8f, 1.2f, 0.5f);
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
