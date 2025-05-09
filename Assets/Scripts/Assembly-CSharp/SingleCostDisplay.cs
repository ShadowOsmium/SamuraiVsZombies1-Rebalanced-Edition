using UnityEngine;

public class SingleCostDisplay : SUIProcess, IHasVisualAttributes
{
	private const float kSlashAlpha = 1f;

	private readonly Vector2 kIconOffset = new Vector2(16f, 16f);

	private readonly Vector2 kLabelOffset = new Vector2(36f, 18f);

	private readonly Vector2 kSlashOffset = new Vector2(50f, 16f);

	private SUISprite mIcon;

	private SUILabel mLabel;

	private SUISprite mSlash;

	private Vector2 mPosition = new Vector2(0f, 0f);

	private bool mIconValid;

	public float priority
	{
		get
		{
			return mIcon.priority;
		}
		set
		{
			mIcon.priority = value;
			mLabel.priority = value;
			if (mSlash != null)
			{
				mSlash.priority = value + 0.1f;
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
			Vector2 vector = value - mPosition;
			mPosition = value;
			mIcon.position += vector;
			mLabel.position += vector;
			if (mSlash != null)
			{
				mSlash.position += vector;
			}
		}
	}

	public bool visible
	{
		get
		{
			return mLabel.visible;
		}
		set
		{
			mIcon.visible = value && mIconValid;
			mLabel.visible = value;
			if (mSlash != null)
			{
				mSlash.visible = value;
			}
		}
	}

	public float alpha
	{
		get
		{
			return mIcon.alpha;
		}
		set
		{
			float num = Mathf.Clamp(value, 0f, 1f);
			mIcon.alpha = num;
			mLabel.alpha = num;
			if (mSlash != null)
			{
				mSlash.alpha = num * 1f;
			}
		}
	}

	public float width
	{
		get
		{
			if (mIcon.visible)
			{
				return mLabel.area.width + mIcon.area.width + 4f;
			}
			return mLabel.area.width;
		}
	}

	public Vector2 scale
	{
		get
		{
			return new Vector2(1f, 1f);
		}
		set
		{
		}
	}

	public SingleCostDisplay()
	{
		Init("default18");
	}

	public SingleCostDisplay(string fontName)
	{
		Init(fontName);
	}

	private void Init(string fontName)
	{
		mIcon = new SUISprite();
		mIcon.position = mPosition + kIconOffset;
		mLabel = new SUILabel(fontName);
		mLabel.alignment = TextAlignment.Left;
		mLabel.anchor = TextAnchor.MiddleLeft;
		mLabel.position = mPosition + kLabelOffset;
		mLabel.shadowOffset = new Vector2(2f, 2f);
		mLabel.shadowColor = new Color(0f, 0f, 0f);
	}

	public void Update()
	{
	}

	public void Destroy()
	{
		mIcon.Destroy();
		mIcon = null;
		mLabel.Destroy();
		mLabel = null;
		if (mSlash != null)
		{
			mSlash.Destroy();
			mSlash = null;
		}
	}

	public void EditorRenderOnGUI()
	{
	}

	public void SetCost(string currencyICon, int val)
	{
		SetCost(currencyICon, val, false, 0, string.Empty);
	}

	public void SetCost(string currencyICon, int val, bool slashed)
	{
		SetCost(currencyICon, val, slashed, 0, string.Empty);
	}

	public void SetCost(string currencyICon, int val, bool slashed, int percentOff)
	{
		SetCost(currencyICon, val, slashed, percentOff, string.Empty);
	}

	public void SetCost(string currencyICon, int val, bool slashed, int percentOff, string freeString)
	{
		if (currencyICon == string.Empty)
		{
			mIconValid = false;
			mIcon.visible = false;
			SUILabel sUILabel = mLabel;
			Vector2 vector = mPosition;
			Vector2 vector2 = kLabelOffset;
			sUILabel.position = vector + new Vector2(0f, vector2.y);
		}
		else
		{
			mIcon.texture = currencyICon;
			mIconValid = true;
			mIcon.position = mPosition + kIconOffset;
			mLabel.position = mPosition + kLabelOffset;
		}
		if (val == 0)
		{
			mLabel.fontColor = Color.green;
			mLabel.text = freeString;
		}
		else
		{
			string text = val.ToString();
			if (percentOff > 0)
			{
				text = text + " " + string.Format(Singleton<Localizer>.instance.Get("store_percent_off"), percentOff);
				mLabel.fontColor = Color.green;
			}
			else
			{
				mLabel.fontColor = Color.white;
			}
			mLabel.text = text;
		}
		if (slashed)
		{
			if (mSlash == null)
			{
				mSlash = new SUISprite("Sprites/Menus/sales_slash");
			}
			mSlash.position = mPosition + kSlashOffset;
			priority = mIcon.priority;
			alpha = mIcon.alpha;
			visible = mLabel.visible;
			position = mPosition;
		}
		else if (mSlash != null)
		{
			mSlash.Destroy();
			mSlash = null;
		}
	}
}
