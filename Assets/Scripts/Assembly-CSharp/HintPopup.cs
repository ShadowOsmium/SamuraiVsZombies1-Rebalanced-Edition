using System.Collections.Generic;
using UnityEngine;

public class HintPopup : IHasVisualAttributes
{
	private const int kTextMaxWidth = 280;

	private const float kFadingSpeed = 0.2f;

	private readonly Vector2 kTextOffset = new Vector2(0f, -2f);

	private readonly Vector2 kIconOffset = new Vector2(-184f, -58f);

	private readonly Vector2 kIconScale = new Vector2(0.7f, 0.7f);

	private SUISprite mDescPanelRef;

	private SUILabel mDescTextRef;

	private SUISprite mDescIconRef;

	private float mAlpha = 1f;

	private float mInternalTransitionAlpha;

	private float mInternalTransitionAlphaTarget;

	private Vector2 mPosition;

	private float mPriority;

	private List<KeyValuePair<string, Vector2>> mIconScalingFilter = new List<KeyValuePair<string, Vector2>>();

	public string text
	{
		get
		{
			return mDescTextRef.text;
		}
		set
		{
			mDescTextRef.text = value;
		}
	}

	public string icon
	{
		get
		{
			if (mDescIconRef != null)
			{
				return mDescIconRef.texture;
			}
			return string.Empty;
		}
		set
		{
			if (value == string.Empty)
			{
				if (mDescIconRef != null)
				{
					mDescIconRef.Destroy();
					mDescIconRef = null;
				}
				return;
			}
			if (mDescIconRef == null)
			{
				mDescIconRef = new SUISprite(value);
				UpdateAlpha();
				UpdatePosition();
				UpdatePriority();
			}
			else
			{
				mDescIconRef.texture = value;
			}
			Vector2 value2 = kIconScale;
			foreach (KeyValuePair<string, Vector2> item in mIconScalingFilter)
			{
				if (value.Contains(item.Key))
				{
					value2 = item.Value;
					break;
				}
			}
			mDescIconRef.scale = value2;
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
			mAlpha = value;
			UpdateAlpha();
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
			UpdatePosition();
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
			mPriority = value;
			UpdatePriority();
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

	public bool visible
	{
		get
		{
			return mInternalTransitionAlphaTarget == 1f;
		}
		set
		{
			if (value)
			{
				mInternalTransitionAlphaTarget = 1f;
			}
			else
			{
				mInternalTransitionAlphaTarget = 0f;
			}
		}
	}

	public HintPopup()
	{
		mDescPanelRef = new SUISprite("Sprites/Menus/pop_up_extra");
		mDescPanelRef.scale = new Vector2(WeakGlobalInstance<SUIScreen>.instance.autoScaler.aspectRatioModifier, 1f);
		mDescTextRef = new SUILabel("default18");
		mDescTextRef.shadowColor = Color.black;
		mDescTextRef.shadowOffset = new Vector2(2f, 2f);
		mDescTextRef.alignment = TextAlignment.Center;
		mDescTextRef.anchor = TextAnchor.MiddleCenter;
		mDescTextRef.maxWidth = 280;
		UpdatePosition();
		UpdateAlpha();
	}

	public void Update()
	{
		UpdateInternalFading();
	}

	public void AddIconScalingFilter(string filter, Vector2 scaling)
	{
		mIconScalingFilter.Add(new KeyValuePair<string, Vector2>(filter, scaling));
	}

	private void UpdateAlpha()
	{
		float num = mAlpha * mInternalTransitionAlpha;
		mDescPanelRef.alpha = num;
		mDescTextRef.alpha = num;
		if (mDescIconRef != null)
		{
			mDescIconRef.alpha = num;
		}
	}

	private void UpdatePosition()
	{
		mDescPanelRef.position = mPosition;
		mDescTextRef.position = mPosition + kTextOffset;
		if (mDescIconRef != null)
		{
			mDescIconRef.position = mPosition + kIconOffset;
		}
	}

	private void UpdatePriority()
	{
		mDescPanelRef.priority = mPriority;
		mDescTextRef.priority = mPriority + 0.1f;
		if (mDescIconRef != null)
		{
			mDescIconRef.priority = mPriority + 0.1f;
		}
	}

	private void UpdateInternalFading()
	{
		if (mInternalTransitionAlpha > mInternalTransitionAlphaTarget)
		{
			mInternalTransitionAlpha = Mathf.Max(mInternalTransitionAlpha - Time.deltaTime / 0.2f, mInternalTransitionAlphaTarget);
			UpdateAlpha();
		}
		else if (mInternalTransitionAlpha < mInternalTransitionAlphaTarget)
		{
			mInternalTransitionAlpha = Mathf.Min(mInternalTransitionAlpha + Time.deltaTime / 0.2f, mInternalTransitionAlphaTarget);
			UpdateAlpha();
		}
	}
}
