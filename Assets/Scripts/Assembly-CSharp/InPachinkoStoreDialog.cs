using System.Collections.Generic;
using Debug = UnityEngine.Debug;

public class InPachinkoStoreDialog
{
	public static IDialog Create(string itemID, bool applyCostMultiplier)
	{
		StoreData.Item storeItemDesc = GetStoreItemDesc(itemID, applyCostMultiplier);
		if (storeItemDesc != null)
		{
			PachinkoItemDisplayDialog pachinkoItemDisplayDialog = new PachinkoItemDisplayDialog(storeItemDesc);
			pachinkoItemDisplayDialog.priority = 1000f;
			pachinkoItemDisplayDialog.onItemPurchased = delegate
			{
				Singleton<Analytics>.instance.LogEvent("InPachinkoPurchase", itemID);
			};
			return pachinkoItemDisplayDialog;
		}
		Debug.Log("ERROR: unable to find store item ID: " + itemID);
		return null;
	}

	public static StoreData.Item GetStoreItemDesc(string id, bool applyCostMultiplier)
	{
		List<StoreData.Item> list = new List<StoreData.Item>();
		StoreAvailability.GetPotions(list);
		StoreAvailability.GetCharms(list);
		StoreAvailability.GetPachinkoBalls(list);
		foreach (StoreData.Item item in list)
		{
			if (item.id == id)
			{
				if (applyCostMultiplier)
				{
					ApplyCostMultiplier(item);
				}
				return item;
			}
		}
		return null;
	}

	private static void ApplyCostMultiplier(StoreData.Item item)
	{
		SDFTreeNode sDFTreeNode = Singleton<Config>.instance.root.to("Store");
		if (sDFTreeNode != null && sDFTreeNode.hasAttribute("inGameCostMultiplier"))
		{
			string[] array = sDFTreeNode["inGameCostMultiplier"].Split(',');
			item.cost.soft = (int)((float)item.cost.soft * float.Parse(array[0]));
			item.cost.hard = (int)((float)item.cost.hard * float.Parse(array[1]));
		}
	}
}
