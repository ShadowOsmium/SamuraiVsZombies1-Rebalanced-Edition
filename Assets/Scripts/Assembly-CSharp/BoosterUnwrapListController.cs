using System.Collections.Generic;
using UnityEngine;

public class BoosterUnwrapListController : SUIScrollList.IController
{
	private const float kCellPriority = 101f;

	private const int kTitleMaxWidth = 156;

	private const float kRevealInitialDelay = 0.6f;

	private const float kRevealStagger = 0.65f;

	private List<BoosterCardListController.CardInfo> mAllCards = new List<BoosterCardListController.CardInfo>();

	private Vector2 mCellSize;

	private float mRevealTimer;

	public BoosterUnwrapListController(Rect listArea, Vector2 cellSize, BoosterPackCodex.CardDesc[] allCards)
	{
		mCellSize = cellSize;
		foreach (BoosterPackCodex.CardDesc cardDesc in allCards)
		{
			BoosterCardListController.CardInfo cardInfo = new BoosterCardListController.CardInfo
			{
				groupID = cardDesc.groupID
			};
			if (cardDesc.args != null && cardDesc.args.Length > 0)
			{
				cardInfo.arg = cardDesc.args[0];
			}
			cardInfo.data = Singleton<BoosterPackCodex>.instance.GetCardData(cardDesc.id);
			mAllCards.Add(cardInfo);
		}
	}

	public void Update()
	{
		UpdateReveals();
	}

	public void Destroy()
	{
	}

	public bool IsCardRevealed(int index)
	{
		return GetCardRevealTimer(index) == 0f;
	}

	public int ScrollList_NumEntries()
	{
		return mAllCards.Count;
	}

	public SUIScrollList.Cell ScrollList_CreateCell()
	{
		BoosterListCells.Card card = new BoosterListCells.Card(1f, mCellSize);
		card.priority = 101f;
		return card;
	}

	public void ScrollList_DrawCellContent(SUIScrollList.Cell c, int dataIndex, bool isSelected)
	{
		BoosterListCells.Card card = (BoosterListCells.Card)c;
		BoosterCardListController.CardInfo cardInfo = mAllCards[dataIndex];
		card.Render(cardInfo.data.to("render"), cardInfo.groupID, cardInfo.arg);
		card.revealTimer = GetCardRevealTimer(dataIndex);
	}

	private float GetCardRevealTimer(int index)
	{
		return Mathf.Max(0.65f * (float)index + 0.6f - mRevealTimer, 0f);
	}

	private void UpdateReveals()
	{
		int currentRevealIndex = GetCurrentRevealIndex();
		mRevealTimer += Time.deltaTime;
		int currentRevealIndex2 = GetCurrentRevealIndex();
		if (currentRevealIndex2 >= 0 && currentRevealIndex2 != currentRevealIndex)
		{
			Singleton<SUISoundManager>.instance.Play("Sounds/UI_LeaderShip_Pickup_01");
		}
	}

	private int GetCurrentRevealIndex()
	{
		float num = mRevealTimer;
		if (num <= 0.6f)
		{
			return -1;
		}
		num -= 0.6f;
		int num2 = (int)(num / 0.65f);
		if (num2 >= mAllCards.Count)
		{
			return -1;
		}
		return num2;
	}
}
