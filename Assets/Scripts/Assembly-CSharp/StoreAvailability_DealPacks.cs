using System;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;

public class StoreAvailability_DealPacks
{
	public static void Get(List<StoreData.Item> items)
	{
		SDFTreeNode sDFTreeNode = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/DealPacks");
		if (sDFTreeNode == null)
		{
			return;
		}
		SDFTreeNode sDFTreeNode2 = sDFTreeNode.to("available");
		if (sDFTreeNode2 == null)
		{
			return;
		}
		for (int i = 0; i < sDFTreeNode2.attributeCount; i++)
		{
			string packID = sDFTreeNode2[i];
			SDFTreeNode sDFTreeNode3 = sDFTreeNode.to(packID);
			if (sDFTreeNode3 == null)
			{
				Debug.Log("WARNING: unknown Deal Pack: " + packID);
			}
			else
			{
				if (!Singleton<EventsManager>.instance.IsEventActive(sDFTreeNode3["eventID"]))
				{
					continue;
				}
				SDFTreeNode sDFTreeNode4 = sDFTreeNode3.to("content");
				if (sDFTreeNode4 == null)
				{
					Debug.Log("WARNING: Deal Pack doesn't have content: " + packID);
					continue;
				}
				List<SpoilsDisplay.Entry> contentList = new List<SpoilsDisplay.Entry>(sDFTreeNode4.attributeCount);
				StoreData.Item item = new StoreData.Item(delegate
				{
					BuyDealPack(contentList, packID);
				});
				item.id = packID;
				item.icon = sDFTreeNode3["icon"];
				item.title = Singleton<Localizer>.instance.Parse(sDFTreeNode3["displayName"]);
				item.details.AddSmallDescription(Singleton<Localizer>.instance.Parse(sDFTreeNode3["description"]));
				item.cost = new Cost(sDFTreeNode3["cost"], Singleton<SalesDatabase>.instance.GetSaleFor(packID));
				item.isEvent = sDFTreeNode3["eventID"] != string.Empty;
				item.contentList = contentList;
				foreach (KeyValuePair<string, string> attribute in sDFTreeNode4.attributes)
				{
					item.contentList.Add(SpoilsDisplay.BuildEntry(attribute.Key, int.Parse(attribute.Value)));
				}
				items.Add(item);
			}
		}
	}

	private static bool ArrayContainsString(string[] theStringArray, string theStringToFind)
	{
		foreach (string text in theStringArray)
		{
			if (text.Equals(theStringToFind, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	private static void BuyDealPack(List<SpoilsDisplay.Entry> contentList, string packID)
	{
		foreach (SpoilsDisplay.Entry content in contentList)
		{
			CashIn.From(content.id, (content.count == 0) ? 1 : content.count, packID, "CREDIT_GC_PURCHASE");
		}
		Singleton<Profile>.instance.Save();
		Singleton<Analytics>.instance.LogEvent("DealPackPurchased", packID);
	}
}
