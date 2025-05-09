using UnityEngine;

public class CostDisplay : SUIProcess, IHasVisualAttributes
{
	private const float kYOffset = 32f;

	private SingleCostDisplay[] mCurrencies = new SingleCostDisplay[2];

	private bool[] mVisibleSettings = new bool[2];

	private Vector2 mPosition = new Vector2(0f, 0f);

	private bool mVisible = true;

	private float mAlpha = 1f;

	public float priority
	{
		get
		{
			return mCurrencies[0].priority;
		}
		set
		{
			SingleCostDisplay[] array = mCurrencies;
			foreach (SingleCostDisplay singleCostDisplay in array)
			{
				singleCostDisplay.priority = value;
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
			int num = 0;
			SingleCostDisplay[] array = mCurrencies;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].position += vector;
				num++;
			}
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
			for (int i = 0; i < mCurrencies.Length; i++)
			{
				mCurrencies[i].visible = value && mVisibleSettings[i];
			}
		}
	}

	public float alpha
	{
		get
		{
			return mAlpha;
		}
		set
		{
			mAlpha = Mathf.Clamp(value, 0f, 1f);
			SingleCostDisplay[] array = mCurrencies;
			foreach (SingleCostDisplay singleCostDisplay in array)
			{
				singleCostDisplay.alpha = mAlpha;
			}
		}
	}

	public float height
	{
		get
		{
			if (mVisibleSettings[1])
			{
				return 64f;
			}
			if (mVisibleSettings[0])
			{
				return 32f;
			}
			return 0f;
		}
	}

	public float width
	{
		get
		{
			if (mVisibleSettings[1])
			{
				return Mathf.Max(mCurrencies[0].width, mCurrencies[1].width);
			}
			if (mVisibleSettings[0])
			{
				return mCurrencies[0].width;
			}
			return 0f;
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

	public CostDisplay(string fontName)
	{
		Init(fontName);
	}

	public CostDisplay()
	{
		Init("default18");
	}

	private void Init(string fontName)
	{
		Vector2 vector = new Vector2(0f, 0f);
		for (int i = 0; i < mCurrencies.Length; i++)
		{
			mVisibleSettings[i] = false;
			mCurrencies[i] = new SingleCostDisplay(fontName);
			mCurrencies[i].visible = false;
			mCurrencies[i].position = vector;
			vector.y += 32f;
		}
	}

	public void Update()
	{
	}

	public void Destroy()
	{
		SingleCostDisplay[] array = mCurrencies;
		foreach (SingleCostDisplay singleCostDisplay in array)
		{
			singleCostDisplay.Destroy();
		}
		mCurrencies = null;
	}

	public void EditorRenderOnGUI()
	{
	}

	public void SetCost(Cost c)
	{
		SetCost(c, string.Empty);
	}

	public void SetCost(Cost c, string freeString)
	{
		for (int i = 0; i < mCurrencies.Length; i++)
		{
			mCurrencies[i].visible = false;
			mVisibleSettings[i] = false;
		}
		int num = 0;
		if (c.soft != 0)
		{
			if (c.isOnSale)
			{
				mCurrencies[num].SetCost("Sprites/Menus/coin", c.preSaleSoft, true, 0, freeString);
				mCurrencies[num].visible = true;
				mVisibleSettings[num] = true;
				num++;
			}
			mCurrencies[num].SetCost("Sprites/Menus/coin", c.soft, false, c.percentOff, freeString);
			mCurrencies[num].visible = true;
			mVisibleSettings[num] = true;
			num++;
		}
		else if (c.hard != 0)
		{
			if (c.isOnSale)
			{
				mCurrencies[num].SetCost("Sprites/Menus/gem", c.preSaleHard, true, 0, freeString);
				mCurrencies[num].visible = true;
				mVisibleSettings[num] = true;
				num++;
			}
			mCurrencies[num].SetCost("Sprites/Menus/gem", c.hard, false, c.percentOff, freeString);
			mCurrencies[num].visible = true;
			mVisibleSettings[num] = true;
			num++;
		}
		else if (freeString != string.Empty)
		{
			mCurrencies[num].SetCost(string.Empty, 0, false, 0, freeString);
			mCurrencies[num].visible = true;
			mVisibleSettings[num] = true;
			num++;
		}
		float num2 = 0f;
		for (int j = 0; j < num; j++)
		{
			float num3 = mCurrencies[j].width;
			if (num2 < num3)
			{
				num2 = num3;
			}
		}
		Vector2 vector = mPosition;
		for (int k = 0; k < num; k++)
		{
			mCurrencies[k].position = vector + new Vector2((num2 - mCurrencies[k].width) / 2f, 0f);
			vector.y += 32f;
		}
	}
}
