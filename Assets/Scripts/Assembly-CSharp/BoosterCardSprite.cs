using System.Collections.Generic;
using UnityEngine;

public class BoosterCardSprite : SUIProcess, IHasVisualAttributes
{
	private const float kPriorityDelta = 0.1f;

	private readonly Vector2 kLabelOffset = new Vector2(70f, 90f);

	private List<SUISprite> mComponents = new List<SUISprite>();

	private SUILabel mLabel;

	private string mLabelText = string.Empty;

	private float mAlpha = 1f;

	private Vector2 mPosition = default(Vector2);

	private float mPriority;

	private Vector2 mScale = new Vector2(1f, 1f);

	private bool mVisible = true;

	public float alpha
	{
		get
		{
			return mAlpha;
		}
		set
		{
			mAlpha = value;
			foreach (SUISprite mComponent in mComponents)
			{
				mComponent.alpha = mAlpha;
			}
			if (mLabel != null)
			{
				mLabel.alpha = mAlpha;
			}
		}
	}

	public Vector2 position
	{
		get
		{
			return mPosition;
		}
		set
		{
			mPosition = value;
			foreach (SUISprite mComponent in mComponents)
			{
				mComponent.position = mPosition;
			}
			UpdateLabelScaleAndPosition();
		}
	}

	public float priority
	{
		get
		{
			return mPriority;
		}
		set
		{
			float num = value - mPriority;
			mPriority = value;
			foreach (SUISprite mComponent in mComponents)
			{
				mComponent.priority += num;
			}
			if (mLabel != null)
			{
				mLabel.priority += num;
			}
		}
	}

	public Vector2 scale
	{
		get
		{
			return mScale;
		}
		set
		{
			mScale = value;
			foreach (SUISprite mComponent in mComponents)
			{
				mComponent.scale = mScale;
			}
			UpdateLabelScaleAndPosition();
		}
	}

	public bool visible
	{
		get
		{
			return mVisible;
		}
		set
		{
			mVisible = value;
			foreach (SUISprite mComponent in mComponents)
			{
				mComponent.visible = mVisible;
			}
			if (mLabel != null)
			{
				mLabel.visible = mVisible;
			}
		}
	}

	public BoosterCardSprite(SDFTreeNode renderData, string groupID, string cardArg)
	{
		Render(renderData, groupID, cardArg);
	}

	public void Update()
	{
		foreach (SUISprite mComponent in mComponents)
		{
			mComponent.Update();
		}
		if (mLabel != null)
		{
			mLabel.Update();
		}
	}

	public void Destroy()
	{
		foreach (SUISprite mComponent in mComponents)
		{
			mComponent.Destroy();
		}
		mComponents.Clear();
		if (mLabel != null)
		{
			mLabel.Destroy();
		}
	}

	public void EditorRenderOnGUI()
	{
	}

	private void UpdateLabelScaleAndPosition()
	{
		if (mLabel != null)
		{
			if (mLabelText != string.Empty)
			{
				CreateLabel(mLabelText);
			}
			SUILabel sUILabel = mLabel;
			Vector2 vector = mPosition;
			Vector2 vector2 = kLabelOffset;
			float x = vector2.x * mScale.x;
			Vector2 vector3 = kLabelOffset;
			sUILabel.position = vector + new Vector2(x, vector3.y * mScale.y);
		}
	}

	private void Render(SDFTreeNode renderData, string groupID, string cardArg)
	{
		CreateSpriteLayer(Singleton<BoosterPackCodex>.instance.GetGroupBG(groupID));
		int num = 1;
		while (true)
		{
			string text = string.Format("layer{0}", num);
			if (renderData.hasAttribute(text))
			{
				CreateSpriteLayer(renderData[text]);
			}
			else
			{
				if (!renderData.hasChild(text))
				{
					break;
				}
				CreateSpriteLayer(renderData.to(text)[groupID]);
			}
			num++;
		}
		if (renderData.hasAttribute("label"))
		{
			CreateLabel(string.Format(Singleton<Localizer>.instance.Parse(renderData["label"]), cardArg));
		}
		position = mPosition;
		alpha = mAlpha;
		priority = mPriority;
		scale = mScale;
		visible = mVisible;
	}

	private void CreateSpriteLayer(string spriteFile)
	{
		SUISprite sUISprite = new SUISprite(spriteFile);
		sUISprite.priority = mPriority + 0.1f * (float)mComponents.Count;
		mComponents.Add(sUISprite);
	}

	private void CreateLabel(string labelStr)
	{
		if (mLabel != null)
		{
			if (mLabelText == labelStr && ((mScale.y > 0.8f && mLabel.font == "default32") || (mScale.y <= 0.8f && mLabel.font == "default18")))
			{
				return;
			}
			mLabel.Destroy();
			mLabel = null;
		}
		mLabelText = labelStr;
		mLabel = new SUILabel((!(mScale.y > 0.8f)) ? "default18" : "default32");
		mLabel.text = mLabelText;
		mLabel.anchor = TextAnchor.MiddleRight;
		mLabel.alignment = TextAlignment.Right;
		mLabel.priority = mPriority + 0.1f * (float)mComponents.Count;
	}
}
