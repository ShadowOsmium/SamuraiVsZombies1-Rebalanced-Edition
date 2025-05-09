using UnityEngine;

public class BoosterPackCardPreviewDialog : IDialog
{
	private const float kCardPreviewPriority = 201f;

	private const float kRarityDoubleLineOffsetY = 40f;

	private readonly Vector2 kCardPreviewPosition = new Vector2(180f, 372f);

	private SUILayout mLayout;

	private bool mDone;

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

	public BoosterPackCardPreviewDialog(SDFTreeNode cardData, string groupID, string cardArg)
	{
		mLayout = new SUILayout("Layouts/BoosterPackPreviewLayout");
		((SUILabel)mLayout["title"]).text = string.Format(Singleton<Localizer>.instance.Parse(cardData["title"]), cardArg);
		((SUILabel)mLayout["rarity"]).text = string.Format("({0})", Singleton<Localizer>.instance.Get("boosterpack_rarity_" + groupID));
		((SUILabel)mLayout["desc"]).text = string.Format(Singleton<Localizer>.instance.Parse(cardData["desc"]), cardArg);
		if (((SUILabel)mLayout["title"]).shownLines == 2)
		{
			SUILabel sUILabel = (SUILabel)mLayout["rarity"];
			sUILabel.position = new Vector2(sUILabel.position.x, sUILabel.position.y + 40f);
			SUILabel sUILabel2 = (SUILabel)mLayout["desc"];
			sUILabel2.position = new Vector2(sUILabel2.position.x, sUILabel2.position.y + 40f);
		}
		BoosterCardSprite obj = new BoosterCardSprite(cardData.to("render"), groupID, cardArg)
		{
			priority = 201f,
			position = kCardPreviewPosition
		};
		SUILayout.ObjectData od = new SUILayout.ObjectData
		{
			obj = obj,
			animAlpha = new SUILayoutAnim.AnimFloat(1f, 0f, new SUILayout.NormalRange(0f, 1f), Ease.Linear)
		};
		mLayout.Add("card_preview", od);
		mLayout.AnimateIn();
	}

	public void Destroy()
	{
		mLayout.Destroy();
		mLayout = null;
	}

	public void Update()
	{
		mLayout.Update();
		if (!mLayout.isAnimating && WeakGlobalInstance<SUIScreen>.instance.inputs.justTouched && !((SUISprite)mLayout["bg"]).area.Contains(WeakGlobalInstance<SUIScreen>.instance.inputs.position))
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
		mLayout.AnimateOut();
		mLayout.onTransitionOver = delegate
		{
			mDone = true;
		};
	}
}
