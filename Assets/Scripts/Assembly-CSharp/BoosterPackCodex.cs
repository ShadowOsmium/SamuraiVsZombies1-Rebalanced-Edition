using System.Collections.Generic;
using UnityEngine;

public class BoosterPackCodex : Singleton<BoosterPackCodex>
{
	public struct Card
	{
		public SDFTreeNode data;

		public int arg1;

		public Card(SDFTreeNode _data, int _arg1)
		{
			data = _data;
			arg1 = _arg1;
		}
	}

	public class CardDesc
	{
		public string id = string.Empty;

		public string groupID = string.Empty;

		public int weight;

		public string[] args;
	}

	public class RarityGroup
	{
		public int totalWeight;

		public string baseBG = string.Empty;

		public List<CardDesc> mCards = new List<CardDesc>();
	}

	private const string kFreeBoosterPackID = "booster_free";

	private SDFTreeNode mCards;

	private SDFTreeNode mPacks;

	private List<string> mAllPackIDs = new List<string>();

	private Dictionary<string, RarityGroup> mRarityGroups = new Dictionary<string, RarityGroup>();

	public int numPacks
	{
		get
		{
			RefreshPackIDs();
			return mAllPackIDs.Count;
		}
	}

	public BoosterPackCodex()
	{
		LoadData();
	}

	public int PackIDToIndex(string packID)
	{
		int num = 0;
		foreach (string mAllPackID in mAllPackIDs)
		{
			if (packID == mAllPackID)
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	public SDFTreeNode GetPackData(int index)
	{
		RefreshPackIDs();
		return GetPackData(mAllPackIDs[index]);
	}

	public SDFTreeNode GetPackData(string packID)
	{
		RefreshPackIDs();
		return mPacks.to(packID);
	}

	public string GetPackID(int index)
	{
		RefreshPackIDs();
		return mAllPackIDs[index];
	}

	public Cost GetPackCost(int index)
	{
		string packID = GetPackID(index);
		SDFTreeNode packData = GetPackData(index);
		return new Cost(packData["cost"], Singleton<SalesDatabase>.instance.GetSaleFor(packID));
	}

	public bool IsPackOnSale(int index)
	{
		return Singleton<SalesDatabase>.instance.GetSaleFor(GetPackID(index)) != 0f;
	}

	public string GetGroupBG(string groupID)
	{
		return mRarityGroups[groupID].baseBG;
	}

	public int GetGroupRank(string groupID)
	{
		int num = 0;
		foreach (KeyValuePair<string, RarityGroup> mRarityGroup in mRarityGroups)
		{
			if (mRarityGroup.Key == groupID)
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	public RarityGroup GetRarityGroup(string groupID)
	{
		return mRarityGroups[groupID];
	}

	public SDFTreeNode GetCardData(string cardID)
	{
		return mCards.to(cardID);
	}

	public CardDesc[] GeneratePackContent(int packIndex)
	{
		SDFTreeNode sDFTreeNode = GetPackData(packIndex).to("recipe");
		if (sDFTreeNode != null)
		{
			return GeneratePackContent(sDFTreeNode);
		}
		return null;
	}

	private void LoadData()
	{
		mCards = SDFTree.LoadFromResources("Registry/BoosterCards");
		mPacks = SDFTree.LoadFromResources("Registry/BoosterPacks");
		mAllPackIDs.Clear();
		foreach (KeyValuePair<string, string> attribute in mPacks.to("all").attributes)
		{
			mAllPackIDs.Add(attribute.Value);
		}
		SDFTreeNode sDFTreeNode = SDFTree.LoadFromResources("Registry/BoosterCardGroups");
		mRarityGroups.Clear();
		foreach (KeyValuePair<string, SDFTreeNode> child in sDFTreeNode.childs)
		{
			SDFTreeNode value = child.Value;
			RarityGroup rarityGroup = new RarityGroup();
			rarityGroup.baseBG = value["baseBG"];
			foreach (KeyValuePair<string, string> attribute2 in value.to("cards").attributes)
			{
				CardDesc cardDesc = new CardDesc();
				string text = attribute2.Key;
				int num = text.IndexOf('(');
				if (num > 0)
				{
					string text2 = text.Substring(num + 1, text.Length - num - 2);
					text = text.Substring(0, num);
					string[] array = text2.Split(',');
					if (array.Length > 0)
					{
						for (int i = 0; i < array.Length; i++)
						{
							array[i] = array[i].Trim();
						}
						cardDesc.args = array;
					}
				}
				cardDesc.id = text;
				cardDesc.groupID = child.Key;
				cardDesc.weight = int.Parse(attribute2.Value);
				rarityGroup.totalWeight += cardDesc.weight;
				rarityGroup.mCards.Add(cardDesc);
			}
			mRarityGroups.Add(child.Key, rarityGroup);
		}
	}

	private CardDesc[] GeneratePackContent(SDFTreeNode recipe)
	{
		CardDesc[] array = new CardDesc[int.Parse(recipe["numCards"])];
		int num = 0;
		if (recipe.hasChild("specific"))
		{
			foreach (KeyValuePair<string, string> attribute in recipe.to("specific").attributes)
			{
				CardDesc cardDesc = new CardDesc();
				string[] array2 = attribute.Key.Split(':');
				if (array2.Length >= 1)
				{
					cardDesc.id = array2[0];
				}
				if (array2.Length >= 2)
				{
					cardDesc.groupID = array2[1];
				}
				cardDesc.args = new string[1];
				cardDesc.args[0] = attribute.Value;
				array[num++] = cardDesc;
			}
		}
		if (recipe.hasChild("fixed"))
		{
			foreach (KeyValuePair<string, string> attribute2 in recipe.to("fixed").attributes)
			{
				int num2 = int.Parse(attribute2.Value);
				for (int i = 0; i < num2; i++)
				{
					array[num++] = GetRandomCardOfRarity(attribute2.Key);
				}
			}
		}
		if (recipe.hasChild("random"))
		{
			foreach (KeyValuePair<string, string> attribute3 in recipe.to("random").attributes)
			{
				float num3 = float.Parse(attribute3.Value);
				if (Random.value <= num3)
				{
					array[num++] = GetRandomCardOfRarity(attribute3.Key);
				}
			}
		}
		while (num < array.Length)
		{
			array[num++] = GetRandomCardOfRarity(recipe["default"]);
		}
		return array;
	}

	private CardDesc GetRandomCardOfRarity(string rarityID)
	{
		RarityGroup rarityGroup = mRarityGroups[rarityID];
		while (true)
		{
			int num = RandomRangeInt.between(1, rarityGroup.totalWeight);
			foreach (CardDesc mCard in rarityGroup.mCards)
			{
				num -= mCard.weight;
				if (num <= 0)
				{
					return mCard;
				}
			}
		}
	}

	private void RefreshPackIDs()
	{
		if (Singleton<Profile>.instance.freeBoosterPacks > 0)
		{
			if (mAllPackIDs[0] != "booster_free")
			{
				mAllPackIDs.Insert(0, "booster_free");
			}
		}
		else if (mAllPackIDs[0] == "booster_free")
		{
			mAllPackIDs.RemoveAt(0);
		}
	}
}
