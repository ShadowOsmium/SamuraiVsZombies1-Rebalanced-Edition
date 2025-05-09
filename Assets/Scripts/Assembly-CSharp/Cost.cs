using UnityEngine;

public struct Cost
{
	public int soft;

	public int hard;

	public int preSaleSoft;

	public int preSaleHard;

	public bool isOnSale
	{
		get
		{
			return preSaleSoft != soft || preSaleHard != hard;
		}
	}

	public bool canAffordWithSoft
	{
		get
		{
			if (soft == 0)
			{
				return false;
			}
			return soft <= Singleton<Profile>.instance.coins;
		}
	}

	public bool canAffordWithHard
	{
		get
		{
			if (hard == 0)
			{
				return false;
			}
			return hard <= Singleton<Profile>.instance.gems;
		}
	}

	public bool canAfford
	{
		get
		{
			if (soft == 0 && hard == 0)
			{
				return true;
			}
			return canAffordWithSoft || canAffordWithHard;
		}
	}

	public int percentOff
	{
		get
		{
			float num = 1f;
			if (soft > 0 && preSaleSoft > 0)
			{
				num = (float)soft / (float)preSaleSoft;
			}
			else if (hard > 0 && preSaleHard > 0)
			{
				num = (float)hard / (float)preSaleHard;
			}
			return 100 - (int)(num * 100f);
		}
	}

	public Cost(int _soft, int _hard, float salePercentage)
	{
		salePercentage = Mathf.Clamp(salePercentage, 0f, 1f);
		preSaleSoft = _soft;
		preSaleHard = _hard;
		soft = (int)((float)preSaleSoft * (1f - salePercentage));
		hard = (int)((float)preSaleHard * (1f - salePercentage));
	}

	public Cost(string str, float salePercentage) : this()
	{
		preSaleSoft = 0;
		preSaleHard = 0;
		string[] array = str.Split(',');
		if (array != null && array.Length != 0)
		{
			if (array[0].Length > 0)
			{
				preSaleSoft = int.Parse(array[0]);
			}
			if (array.Length > 1 && array[1].Length > 0)
			{
				preSaleHard = int.Parse(array[1]);
			}
			soft = (int)((float)preSaleSoft * (1f - salePercentage));
			hard = (int)((float)preSaleHard * (1f - salePercentage));
		}
	}

	public override string ToString()
	{
		if (soft == 0 && hard == 0)
		{
			return "Free";
		}
		if (soft == 0)
		{
			return hard + " Gems";
		}
		string text = soft + " Coins";
		if (hard > 0)
		{
			text = text + " or " + hard + " Gems";
		}
		int num = percentOff;
		if (num != 0)
		{
			text += string.Format(" at {0}% off.", num);
		}
		return text;
	}

	public override bool Equals(object o)
	{
		Cost cost = (Cost)o;
		return soft == cost.soft && hard == cost.hard;
	}

	public override int GetHashCode()
	{
		return ToString().GetHashCode();
	}

	public void Spend(string itemId)
	{
		Singleton<Profile>.instance.coins -= soft;
		ApplicationUtilities.GWalletBalance(-hard, itemId, "DEBIT_IN_APP_PURCHASE");
	}

	public static bool operator ==(Cost a, Cost b)
	{
		return a.Equals(b);
	}

	public static bool operator !=(Cost a, Cost b)
	{
		return !a.Equals(b);
	}
}
