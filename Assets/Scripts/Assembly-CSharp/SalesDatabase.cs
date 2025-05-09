using System;
using System.Collections.Generic;

public class SalesDatabase : Singleton<SalesDatabase>
{
	private enum SaleCondition
	{
		None = 0,
		FromWave = 1
	}

	private class SaleData
	{
		public float factor;

		public SaleCondition condition;

		public int conditionVal;

		public SaleData(string entryString)
		{
			string[] array = entryString.Split(':');
			if (array.Length >= 1)
			{
				factor = float.Parse(array[0].Trim()) / 100f;
			}
			if (array.Length == 2)
			{
				string[] array2 = array[1].Trim().Split(' ');
				if (array2.Length >= 1)
				{
					condition = (SaleCondition)(int)Enum.Parse(typeof(SaleCondition), array2[0].Trim());
				}
				if (array2.Length == 2)
				{
					conditionVal = int.Parse(array2[1].Trim());
				}
			}
		}

		public float CalcFactor()
		{
			SaleCondition saleCondition = condition;
			if (saleCondition == SaleCondition.FromWave && !Singleton<Profile>.instance.wasBasicGameBeaten && Singleton<Profile>.instance.waveToBeat < conditionVal)
			{
				return 0f;
			}
			return factor;
		}
	}

	private class Entry
	{
		public List<SaleData> data = new List<SaleData>();

		public Dictionary<string, Entry> subs;

		public float FindSale()
		{
			foreach (SaleData datum in data)
			{
				float num = datum.CalcFactor();
				if (num != 0f)
				{
					return num;
				}
			}
			return 0f;
		}
	}

	private Dictionary<string, Entry> mEntries = new Dictionary<string, Entry>();

	public SalesDatabase()
	{
		ResetCachedData();
		SingletonMonoBehaviour<ResourcesManager>.instance.onInvalidateCache += ForceReload;
	}

	public float GetSaleFor(string id)
	{
		id = id.ToLower();
		if (mEntries.ContainsKey(id))
		{
			return mEntries[id].FindSale();
		}
		return 0f;
	}

	public float GetSaleFor(string id, string sub)
	{
		id = id.ToLower();
		if (mEntries.ContainsKey(id))
		{
			sub = sub.ToLower();
			Entry entry = mEntries[id];
			if (entry.subs != null && entry.subs.ContainsKey(sub))
			{
				return entry.subs[sub].FindSale();
			}
			return entry.FindSale();
		}
		return 0f;
	}

	public void ForceReload()
	{
		ResetCachedData();
	}

	private void ResetCachedData()
	{
		mEntries.Clear();
		SDFTreeNode sDFTreeNode = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/Sales");
		if (sDFTreeNode != null)
		{
			ParseSalesData(mEntries, sDFTreeNode);
		}
	}

	private void ParseSalesData(Dictionary<string, Entry> list, SDFTreeNode data)
	{
		foreach (KeyValuePair<string, SDFTreeNode> child in data.childs)
		{
			Entry entry = new Entry();
			for (int i = 0; i < child.Value.attributeCount; i++)
			{
				string text = child.Value[i].Trim();
				if (text != string.Empty)
				{
					entry.data.Add(new SaleData(text));
				}
			}
			if (child.Value.childCount > 0)
			{
				entry.subs = new Dictionary<string, Entry>(child.Value.childCount);
				ParseSalesData(entry.subs, child.Value);
			}
			list.Add(child.Key.ToLower(), entry);
		}
	}
}
