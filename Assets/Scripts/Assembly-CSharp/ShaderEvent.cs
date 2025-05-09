using System.Collections.Generic;
using UnityEngine;

public class ShaderEvent
{
	protected bool mShouldDie;

	protected TypedWeakReference<GameObject> mObj;

	protected List<Material> mMaterials;

	public bool shouldDie
	{
		get
		{
			return mShouldDie;
		}
		set
		{
			mShouldDie = value;
		}
	}

	public ShaderEvent(GameObject obj)
	{
		mObj = new TypedWeakReference<GameObject>(obj);
		mMaterials = new List<Material>();
		if (obj != null)
		{
			Renderer[] componentsInChildren = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				mMaterials.AddRange(renderer.materials);
			}
			componentsInChildren = obj.GetComponentsInChildren<MeshRenderer>();
			Renderer[] array2 = componentsInChildren;
			foreach (Renderer renderer2 in array2)
			{
				mMaterials.AddRange(renderer2.materials);
			}
		}
	}

	public virtual void resetToBaseValues()
	{
	}

	public virtual void update()
	{
		if (mObj.ptr == null)
		{
			mShouldDie = true;
		}
	}

	protected void SetAllMaterialsToColor(Color color)
	{
		foreach (Material mMaterial in mMaterials)
		{
			mMaterial.SetColor("_Color", color);
			mMaterial.SetColor("_MainColor", color);
		}
	}

	protected Color GetCurrentObjectColor()
	{
		if (mMaterials == null || mMaterials.Count == 0)
		{
			return Color.white;
		}
		if (mMaterials[0].HasProperty("_Color"))
		{
			return mMaterials[0].GetColor("_Color");
		}
		if (mMaterials[0].HasProperty("_MainColor"))
		{
			return mMaterials[0].GetColor("_MainColor");
		}
		return Color.white;
	}
}
