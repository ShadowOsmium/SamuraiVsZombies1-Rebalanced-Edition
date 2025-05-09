using UnityEngine;

public class OnlineItemsManager : Singleton<OnlineItemsManager>
{
	public enum ItemType
	{
		Vip_Gold = 0,
		Vip_Silver = 1,
		Coins = 2,
		Gems = 3
	}

	public class Item
	{
		public string id;

		public string desc;

		public ItemType type;

		public int amount;

		public string price;

		public Item(string _id, string _desc, ItemType _type, int _amount, string _price)
		{
			id = _id;
			desc = _desc;
			type = _type;
			amount = _amount;
			price = _price;
		}
	}

	private Item[] mItems = new Item[14]
	{
		new Item("com.glu.samuzombie.2000_coins", string.Empty, ItemType.Coins, 2000, "$0.00"),
		new Item("com.glu.samuzombie.5500_coins", string.Empty, ItemType.Coins, 5500, "$0.00"),
		new Item("com.glu.samuzombie.13000_coins", string.Empty, ItemType.Coins, 13000, "$0.00"),
		new Item("com.glu.samuzombie.32000_coins", string.Empty, ItemType.Coins, 32000, "$0.00"),
		new Item("com.glu.samuzombie.100000_coins", string.Empty, ItemType.Coins, 100000, "$0.00"),
		new Item("com.glu.samuzombie.260000_coins", string.Empty, ItemType.Coins, 260000, "$0.00"),
		new Item("com.glu.samuzombie.gold.monthly", string.Empty, ItemType.Vip_Gold, 0, "$0.00"),
		new Item("com.glu.samuzombie.silver.monthly", string.Empty, ItemType.Vip_Silver, 0, "$0.00"),
		new Item("com.glu.samuzombie.40_gems", string.Empty, ItemType.Gems, 40, "$0.00"),
		new Item("com.glu.samuzombie.110_gems", string.Empty, ItemType.Gems, 110, "$0.00"),
		new Item("com.glu.samuzombie.260_gems", string.Empty, ItemType.Gems, 260, "$0.00"),
		new Item("com.glu.samuzombie.640_gems", string.Empty, ItemType.Gems, 640, "$0.00"),
		new Item("com.glu.samuzombie.2000_gems", string.Empty, ItemType.Gems, 2000, "$0.00"),
		new Item("com.glu.samuzombie.5200_gems", string.Empty, ItemType.Gems, 5200, "$0.00")
	};

	private GameObject mGluListener;

	public Item[] items
	{
		get
		{
			return mItems;
		}
	}

	public OnlineItemsManager()
	{
		RefreshItemsDesc();
	}

	public void ApplyOnlineInformations(IAPItemDescriptor[] items)
	{
		foreach (IAPItemDescriptor d in items)
		{
			ParseDescriptor(d);
		}
		RefreshItemsDesc();
	}

	public Item Find(string id)
	{
		Item[] array = mItems;
		foreach (Item item in array)
		{
			if (item.id == id)
			{
				return item;
			}
		}
		return null;
	}

	public void CompletePurchase(string id)
	{
		Item item = Find(id);
		if (item == null)
		{
			return;
		}
		switch (item.type)
		{
		case ItemType.Coins:
			Singleton<Profile>.instance.coins += item.amount;
			Singleton<Profile>.instance.purchasedCoins += item.amount;
			Singleton<Analytics>.instance.LogEvent("UserPurchasedCoins", "tablet", item.amount);
			Singleton<PlayHavenTowerControl>.instance.InvokeContent("iap_coins_" + item.amount);
			Debug.Log("*** UserPurchasedCoins ***");
			break;
		case ItemType.Gems:
			ApplicationUtilities.GWalletBalance(item.amount, id, "CREDIT_GC_PURCHASE");
			Singleton<Profile>.instance.purchasedGems += item.amount;
			Singleton<Analytics>.instance.LogEvent("UserPurchasedGems", "tablet", item.amount);
			Singleton<PlayHavenTowerControl>.instance.InvokeContent("iap_gems_" + item.amount);
			Debug.Log("*** UserPurchasedGems ***");
			if (!Singleton<Profile>.instance.hasReportedGemPurchase)
			{
				Singleton<Analytics>.instance.LogEvent("UniquePurchasedGems", string.Empty);
				Singleton<Profile>.instance.hasReportedGemPurchase = true;
			}
			break;
		}
		Singleton<Profile>.instance.Save();
	}

	private GameObject CreateGluListener()
	{
		GameObject gameObject = new GameObject("IAPListener");
		Object.DontDestroyOnLoad(gameObject);
		gameObject.AddComponent<IAPListenerScript>();
		return gameObject;
	}

	private void RefreshItemsDesc()
	{
		Item[] array = mItems;
		foreach (Item item in array)
		{
			string arg = Singleton<Localizer>.instance.Get((item.type != ItemType.Coins) ? "gems" : "coins");
			item.desc = string.Format("{0} {1}\n{2}", item.amount.ToString(), arg, item.price);
		}
	}

	private void ParseDescriptor(IAPItemDescriptor d)
	{
		Item[] array = mItems;
		foreach (Item item in array)
		{
			if (item.id == d.id)
			{
				item.price = d.price;
				return;
			}
		}
		Debug.Log("WARNING: Unknown IAP item -> " + d.id);
	}
}
