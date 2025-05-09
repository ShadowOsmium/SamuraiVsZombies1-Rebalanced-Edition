using System.Collections.Generic;
using UnityEngine;

public class BoosterCardListController : SUIScrollList.IController
{
	public class CardInfo
	{
		public SDFTreeNode data;

		public string groupID;

		public string arg;
	}

	private const float kCellPriority = 1f;

	private const int kTitleMaxWidth = 156;

	private List<CardInfo> mAllCards = new List<CardInfo>();

	private Vector2 mCellSize;

	public BoosterCardListController(Rect listArea, Vector2 cellSize, int packIndex)
	{
		mCellSize = cellSize;
		CreateCardsList(Singleton<BoosterPackCodex>.instance.GetPackData(packIndex).to("recipe"));
	}

	public void Update()
	{
	}

	public void Destroy()
	{
	}

	public int ScrollList_NumEntries()
	{
		return mAllCards.Count;
	}

	public SUIScrollList.Cell ScrollList_CreateCell()
	{
		BoosterListCells.Card card = new BoosterListCells.Card(0.6f, mCellSize);
		card.priority = 1f;
		return card;
	}

	public void ScrollList_DrawCellContent(SUIScrollList.Cell c, int dataIndex, bool isSelected)
	{
		BoosterListCells.Card card = (BoosterListCells.Card)c;
		CardInfo cardInfo = mAllCards[dataIndex];
		card.Render(cardInfo.data.to("render"), cardInfo.groupID, cardInfo.arg);
	}

	public CardInfo GetCard(int index)
	{
		return mAllCards[index];
	}

	private void CreateCardsList(SDFTreeNode recipe)
	{
		List<string> list = new List<string>();
		AddNonDuplicate(list, recipe["default"]);
		if (recipe.hasChild("fixed"))
		{
			foreach (KeyValuePair<string, string> attribute in recipe.to("fixed").attributes)
			{
				AddNonDuplicate(list, attribute.Key);
			}
		}
		if (recipe.hasChild("random"))
		{
			foreach (KeyValuePair<string, string> attribute2 in recipe.to("random").attributes)
			{
				AddNonDuplicate(list, attribute2.Key);
			}
		}
		SortRarityGroups(list);
		foreach (string item in list)
		{
			AddAllCardsFromGroup(item);
		}
	}

	private void AddAllCardsFromGroup(string groupID)
	{
		BoosterPackCodex.RarityGroup rarityGroup = Singleton<BoosterPackCodex>.instance.GetRarityGroup(groupID);
		foreach (BoosterPackCodex.CardDesc mCard in rarityGroup.mCards)
		{
			CardInfo cardInfo = new CardInfo();
			cardInfo.groupID = groupID;
			cardInfo.data = Singleton<BoosterPackCodex>.instance.GetCardData(mCard.id);
			if (mCard.args != null && mCard.args.Length > 0)
			{
				cardInfo.arg = mCard.args[0];
			}
			mAllCards.Add(cardInfo);
		}
	}

	private void AddNonDuplicate(List<string> strList, string toAdd)
	{
		foreach (string str in strList)
		{
			if (str == toAdd)
			{
				return;
			}
		}
		strList.Add(toAdd);
	}

	private void SortRarityGroups(List<string> groups)
	{
		groups.Sort((string s1, string s2) => Singleton<BoosterPackCodex>.instance.GetGroupRank(s2).CompareTo(Singleton<BoosterPackCodex>.instance.GetGroupRank(s1)));
	}
}
