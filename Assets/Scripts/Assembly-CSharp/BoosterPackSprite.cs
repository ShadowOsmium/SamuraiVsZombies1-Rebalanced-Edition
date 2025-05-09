using System.Collections.Generic;
using UnityEngine;

public class BoosterPackSprite : IHasVisualAttributes
{
	private const float kPriorityDelta = 0.1f;

	private List<SUISprite> mComponents = new List<SUISprite>();

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
		}
	}

	public void Render(SDFTreeNode renderData)
	{
		foreach (SUISprite mComponent in mComponents)
		{
			mComponent.Destroy();
		}
		mComponents.Clear();
		int num = 1;
		while (true)
		{
			string text = string.Format("layer{0}", num);
			if (renderData.hasAttribute(text))
			{
				CreateSpriteLayer(renderData[text]);
				num++;
				continue;
			}
			break;
		}
		position = mPosition;
		alpha = mAlpha;
		priority = mPriority;
		scale = mScale;
		visible = mVisible;
	}

	public void Update()
	{
		foreach (SUISprite mComponent in mComponents)
		{
			mComponent.Update();
		}
	}

	public void Destroy()
	{
		foreach (SUISprite mComponent in mComponents)
		{
			mComponent.Destroy();
		}
		mComponents.Clear();
	}

	private void CreateSpriteLayer(string spriteFile)
	{
		SUISprite sUISprite = new SUISprite(spriteFile);
		sUISprite.priority = mPriority + 0.1f * (float)mComponents.Count;
		mComponents.Add(sUISprite);
	}
}
