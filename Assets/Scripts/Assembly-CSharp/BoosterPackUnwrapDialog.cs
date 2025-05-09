using System.Collections.Generic;
using UnityEngine;

public class BoosterPackUnwrapDialog : IDialog
{
	private const int kMaxCardsForCentering = 4;

	private readonly Rect kCardListArea = new Rect(0f, 184f, 1024f, 370f);

	private readonly Vector2 kCardListCellSize = new Vector2(234f, 370f);

	private SUILayout mLayout;

	private bool mDone;

	private BoosterPackCodex.CardDesc[] mGeneratedCards;

	private BoosterUnwrapListController mCardListController;

	private SUIScrollList mCardScrollList;

	private BoosterPackCardPreviewDialog mPreviewCardDialog;

	private string m_packId;

	public bool isBlocking
	{
		get
		{
			return true;
		}
	}

	public bool isDone
	{
		get
		{
			return mDone;
		}
	}

	public BoosterPackUnwrapDialog(int packIndex, Cost cost)
	{
		mLayout = new SUILayout("Layouts/BoosterPackUnwrapLayout");
		m_packId = Singleton<BoosterPackCodex>.instance.GetPackID(packIndex);
		mGeneratedCards = Singleton<BoosterPackCodex>.instance.GeneratePackContent(packIndex);
		BoosterPackCodex.CardDesc[] array = mGeneratedCards;
		foreach (BoosterPackCodex.CardDesc card in array)
		{
			CashInCard(card);
		}
		Rect rect = kCardListArea;
		if (mGeneratedCards.Length <= 4)
		{
			float num = (float)mGeneratedCards.Length * kCardListCellSize.x;
			rect.xMin = (SUIScreen.width - num) / 2f;
			rect.width = num;
		}
		mCardListController = new BoosterUnwrapListController(rect, kCardListCellSize, mGeneratedCards);
		mCardScrollList = new SUIScrollList(mCardListController, rect, kCardListCellSize, SUIScrollList.ScrollDirection.Horizontal, 1);
		mCardScrollList.onItemTouched = OnCardTouched;
		cost.Spend(m_packId);
		Singleton<Profile>.instance.Save();
	}

	public void Destroy()
	{
		mLayout.Destroy();
		mLayout = null;
		mCardScrollList.Destroy();
		mCardScrollList = null;
	}

	public void Update()
	{
		if (mPreviewCardDialog != null)
		{
			mPreviewCardDialog.Update();
			if (mPreviewCardDialog.isDone)
			{
				mPreviewCardDialog.Destroy();
				mPreviewCardDialog = null;
			}
			return;
		}
		if (mCardScrollList != null)
		{
			mCardScrollList.Update();
		}
		if (mLayout != null)
		{
			mLayout.Update();
		}
		if (WeakGlobalInstance<SUIScreen>.instance.inputs.justTouched && !((SUISprite)mLayout["bg"]).area.Contains(WeakGlobalInstance<SUIScreen>.instance.inputs.position))
		{
			Dismiss();
		}
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			Dismiss();
		}
	}

	private void Dismiss()
	{
		mDone = true;
	}

	private void CashInCard(BoosterPackCodex.CardDesc card)
	{
		SDFTreeNode sDFTreeNode = Singleton<BoosterPackCodex>.instance.GetCardData(card.id).to("payload");
		foreach (KeyValuePair<string, string> attribute in sDFTreeNode.attributes)
		{
			string key = attribute.Key;
			string val = ((card.args == null || card.args.Length <= 0) ? string.Empty : string.Format(attribute.Value, card.args[0]));
			CashIn.From(key, val, m_packId, "CREDIT_IN_GAME_AWARD");
		}
	}

	private void OnCardTouched(int index)
	{
		if (mCardListController.IsCardRevealed(index))
		{
			if (mPreviewCardDialog != null)
			{
				mPreviewCardDialog.Destroy();
			}
			SDFTreeNode cardData = Singleton<BoosterPackCodex>.instance.GetCardData(mGeneratedCards[index].id);
			string cardArg = ((mGeneratedCards[index].args.Length <= 0) ? string.Empty : mGeneratedCards[index].args[0]);
			mPreviewCardDialog = new BoosterPackCardPreviewDialog(cardData, mGeneratedCards[index].groupID, cardArg);
		}
	}
}
