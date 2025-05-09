using UnityEngine;

public class GameHUD
{
	private SUILayout mLayout;

	private HUDHealthBar mHealthBar;

	private HUDHelpersBar mHelpersBar;

	private HUDAbilities mAbilities;

	private HUDConsumables mConsumables;

	private HUDLegionBar mLegionBar;

	private HUDSmithy mSmithyBar;

	private HUDBloodPulse mBloodPulse;

	private Rect mPauseButtonRect;

	private int mLastDisplayedCoins = -1;

	private int mLastDisplayedGems = -1;

	private bool m_cheatClicked;

	public OnSUIGenericCallback onPauseGameRequest
	{
		get
		{
			return ((SUIButton)mLayout["pauseBtn"]).onButtonPressed;
		}
		set
		{
			((SUIButton)mLayout["pauseBtn"]).onButtonPressed = value;
		}
	}

	public OnSUIStringCallback onAbilityTriggered
	{
		get
		{
			return mAbilities.onAbilityTriggered;
		}
		set
		{
			mAbilities.onAbilityTriggered = value;
		}
	}

	public Character observedCharacter
	{
		get
		{
			return mHealthBar.observedCharacter;
		}
		set
		{
			mHealthBar.observedCharacter = value;
		}
	}

	public SUISprite coinsUISprite
	{
		get
		{
			return (SUISprite)mLayout["coinsIcon"];
		}
	}

	public SUISprite gemsUISprite
	{
		get
		{
			return (SUISprite)mLayout["gemsIcon"];
		}
	}

	public Vector2 smithyBarLocation
	{
		get
		{
			return ((SUISprite)mLayout["leadershipIcon"]).position;
		}
	}

	public HUDConsumables consumables
	{
		get
		{
			return mConsumables;
		}
	}

	public GameHUD()
	{
		mLayout = new SUILayout(Singleton<PlayModesManager>.instance.selectedModeData["hudLayout"]);
		mPauseButtonRect = ((SUIButton)mLayout["pauseBtn"]).area;
		mHealthBar = new HUDHealthBar((SUISprite)mLayout["lifebar"]);
		mHelpersBar = new HUDHelpersBar(mLayout);
		mAbilities = new HUDAbilities(mLayout);
		mConsumables = new HUDConsumables(mLayout);
		mLegionBar = new HUDLegionBar(mLayout);
		mSmithyBar = new HUDSmithy(mLayout);
		mBloodPulse = new HUDBloodPulse();
		SUIButton sUIButton = (SUIButton)mLayout["cheats"];
		sUIButton.scale = new Vector2(2f, 2f);
		if (/*Debug.isDebugBuild*/false)
		{
			sUIButton.onButtonPressed = WinTheWave;
		}
		else
		{
			sUIButton.visible = false;
		}
		m_cheatClicked = false;
		InitCharm();
		UpdateCoins();
		mLayout.AnimateIn();
	}

	public void Update()
	{
		mLayout.Update();
		UpdateCoins();
		mHelpersBar.Update();
		mAbilities.Update();
		mConsumables.Update();
		mLegionBar.Update();
		mSmithyBar.Update();
		mBloodPulse.Update();
		if (!mLayout.isAnimating)
		{
			mHealthBar.Update();
		}
	}

	public void UpdateConsumableVisualsOnly()
	{
		mConsumables.UpdateVisualsOnly();
	}

	public void UpdateCurrenciesOnly()
	{
		UpdateCoins();
	}

	public bool IsTouchingHUD(Vector2 pos)
	{
		if (mSmithyBar.IsTouchZone(pos) || mHealthBar.IsTouchZone(pos) || mHelpersBar.IsTouchZone(pos) || mAbilities.IsTouchZone(pos) || mConsumables.IsTouchZone(pos) || mLegionBar.IsTouchZone(pos) || mPauseButtonRect.Contains(pos))
		{
			return true;
		}
		return false;
	}

	private void UpdateCoins()
	{
		int coins = Singleton<Profile>.instance.coins;
		int gems = Singleton<Profile>.instance.gems;
		if (coins != mLastDisplayedCoins || gems != mLastDisplayedGems)
		{
			mLastDisplayedCoins = coins;
			mLastDisplayedGems = gems;
			((SUILabel)mLayout["currencyCounter"]).text = mLastDisplayedCoins + "\n" + mLastDisplayedGems;
		}
	}

	private void InitCharm()
	{
		if (Singleton<Profile>.instance.selectedCharm != string.Empty)
		{
			string selectedCharm = Singleton<Profile>.instance.selectedCharm;
			SUISprite sUISprite = (SUISprite)mLayout["charm"];
			sUISprite.texture = Singleton<CharmsDatabase>.instance.GetAttribute(selectedCharm, "icon");
			sUISprite.visible = true;
		}
	}

	private void WinTheWave()
	{
		if (Debug.isDebugBuild)
		{
			m_cheatClicked = true;
		}
	}

	public bool cheatClicked()
	{
		return m_cheatClicked;
	}
}
