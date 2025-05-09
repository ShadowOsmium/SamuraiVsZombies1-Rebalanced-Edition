public class IconGlowWidget : SUISprite
{
	private SUILayoutEffect.Effect mPulseEffect;

	public IconGlowWidget()
	{
		Init("Sprites/Icons/potion_glow", 1.3f, 1.55f);
	}

	public IconGlowWidget(string glowSpriteFile, float minScale, float maxScale)
	{
		Init(glowSpriteFile, minScale, maxScale);
	}

	private void Init(string spriteFile, float minScale, float maxScale)
	{
		base.texture = spriteFile;
		mPulseEffect = new SUILayoutEffect.ScalePingPong(this, minScale, maxScale, 1f);
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
