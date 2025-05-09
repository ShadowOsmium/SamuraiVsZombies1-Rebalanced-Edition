using Debug = UnityEngine.Debug;

public class CashIn
{
	public enum ItemType
	{
		Unknown = 0,
		Coins = 1,
		Gems = 2,
		Balls = 3,
		BoosterPack = 4,
		Potion = 5,
		Charm = 6,
		Helper = 7,
		Ability = 8
	}

	public static bool From(string id, int val, string gwDesc, string gwType)
	{
		return From(id, val.ToString(), gwDesc, gwType);
	}

	public static bool From(string id, string val, string gwDesc, string gwType)
	{
		if (id == "gems")
		{
			ApplicationUtilities.GWalletBalance(int.Parse(val), gwDesc, gwType);
			return true;
		}
		if (id == "coins" && gwType == "CREDIT_GC_PURCHASE")
		{
			Singleton<Profile>.instance.coins += int.Parse(val);
		}
		return From(id, val);
	}

	private static bool From(string id, string val)
	{
		switch (GetType(id))
		{
		case ItemType.Coins:
			Singleton<Profile>.instance.coins += int.Parse(val);
			break;
		case ItemType.Gems:
			Singleton<Profile>.instance.gems += int.Parse(val);
			break;
		case ItemType.Balls:
			Singleton<Profile>.instance.pachinkoBalls += int.Parse(val);
			break;
		case ItemType.BoosterPack:
			if (id == "booster_free")
			{
				Singleton<Profile>.instance.freeBoosterPacks += int.Parse(val);
			}
			break;
		case ItemType.Potion:
			Singleton<Profile>.instance.SetNumPotions(id, Singleton<Profile>.instance.GetNumPotions(id) + int.Parse(val));
			break;
		case ItemType.Charm:
			Singleton<Profile>.instance.SetNumCharms(id, Singleton<Profile>.instance.GetNumCharms(id) + int.Parse(val));
			break;
		case ItemType.Helper:
			Singleton<Profile>.instance.SetHelperLevel(id, Singleton<Profile>.instance.GetHelperLevel(id) + int.Parse(val), true);
			break;
		case ItemType.Ability:
			Singleton<Profile>.instance.SetAbilityLevel(id, Singleton<Profile>.instance.GetAbilityLevel(id) + int.Parse(val));
			break;
		default:
			Debug.Log("ERROR: Unknown Booster Card Payload ID: '" + id + "'");
			return false;
		}
		return true;
	}

	public static ItemType GetType(string id)
	{
		switch (id)
		{
		case "coins":
			return ItemType.Coins;
		case "gems":
			return ItemType.Gems;
		case "balls":
			return ItemType.Balls;
		case "booster_free":
			return ItemType.BoosterPack;
		default:
			if (Singleton<PotionsDatabase>.instance.Contains(id))
			{
				return ItemType.Potion;
			}
			if (Singleton<CharmsDatabase>.instance.Contains(id))
			{
				return ItemType.Charm;
			}
			if (Singleton<HelpersDatabase>.instance.Contains(id))
			{
				return ItemType.Helper;
			}
			if (Singleton<AbilitiesDatabase>.instance.Contains(id))
			{
				return ItemType.Ability;
			}
			return ItemType.Unknown;
		}
	}
}
