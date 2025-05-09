using UnityEngine;

public class HUDHealthBar
{
	private const float kHealthChangeSpeed = 1f;

	private SUISprite mBarSprite;

	private float mCurrentDisplayedHealth;

	private Character mObservedCharacter;

	private bool mObservedCharacterIsHero;

	public Character observedCharacter
	{
		get
		{
			return mObservedCharacter;
		}
		set
		{
			mObservedCharacter = value;
			mCurrentDisplayedHealth = value.health;
			mObservedCharacterIsHero = mObservedCharacter is Hero;
			UpdateBarLengthAndColor();
		}
	}

	public HUDHealthBar(SUISprite barSprite)
	{
		mBarSprite = barSprite;
	}

	public void Update()
	{
		if (mObservedCharacter != null)
		{
			UpdateHealthChangeEasing();
			UpdateBarLengthAndColor();
		}
	}

	public bool IsTouchZone(Vector2 pos)
	{
		return false;
	}

	private void UpdateHealthChangeEasing()
	{
		if (mCurrentDisplayedHealth > mObservedCharacter.health)
		{
			mCurrentDisplayedHealth = Mathf.Max(mObservedCharacter.health, mCurrentDisplayedHealth - Time.deltaTime / 1f * mObservedCharacter.maxHealth);
		}
		else if (mCurrentDisplayedHealth < mObservedCharacter.health)
		{
			mCurrentDisplayedHealth = Mathf.Min(mObservedCharacter.health, mCurrentDisplayedHealth + Time.deltaTime / 1f * mObservedCharacter.maxHealth);
		}
	}

	private void UpdateBarLengthAndColor()
	{
		if (mObservedCharacterIsHero)
		{
			string healthBarFileToUse = ((Hero)mObservedCharacter).healthBarFileToUse;
			if (mBarSprite.texture != healthBarFileToUse)
			{
				mBarSprite.texture = healthBarFileToUse;
			}
		}
		float x = mCurrentDisplayedHealth / mObservedCharacter.maxHealth;
		mBarSprite.scale = new Vector2(x, 1f);
	}
}
