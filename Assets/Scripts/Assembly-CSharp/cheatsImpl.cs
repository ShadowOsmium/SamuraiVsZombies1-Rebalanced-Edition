using System;
using System.Collections.Generic;
using UnityEngine;

public class cheatsImpl : SceneBehaviour
{
	private bool m_quit;

	private void Start()
	{
		m_quit = false;
	}

	private void Update()
	{
		if (!m_quit && Input.GetKeyUp(KeyCode.Escape))
		{
			m_quit = true;
			Singleton<MenusFlow>.instance.LoadScene("MainMenu");
			Singleton<Profile>.instance.Save();
		}
	}

	private void Quit()
	{
	}

	private void OnGUI()
	{
		if (m_quit || Event.current.type == EventType.Layout)
		{
			return;
		}
		int num = 800;
		int num2 = 480;
		GUI.matrix = Matrix4x4.Scale(new Vector3((float)Screen.width / 800f, (float)Screen.height / 480f, 1f));
		GUI.Box(new Rect(10f, 10f, num - 20, num2 - 20), string.Empty);
		int num3 = 20;
		int num4 = 35;
		GUI.Label(new Rect(20f, num3, 200f, 20f), "Current Mode: " + Singleton<PlayModesManager>.instance.selectedMode);
		num3 += 30;
		if (GUI.Button(new Rect(20f, num3 - 5, 50f, 30f), "Reset"))
		{
			Singleton<Profile>.instance.ResetData();
		}
		num3 += num4;
		GUI.Label(new Rect(20f, num3, 80f, 20f), "Add coins:");
		GUI.Label(new Rect(100f, num3, 60f, 20f), Singleton<Profile>.instance.coins.ToString());
		if (GUI.Button(new Rect(160f, num3 - 5, 50f, 30f), "99999"))
		{
			AddCurreny("coins", 99999);
		}
		num3 += num4;
		GUI.Label(new Rect(20f, num3, 80f, 20f), "Add gems:");
		GUI.Label(new Rect(100f, num3, 60f, 20f), Singleton<Profile>.instance.gems.ToString());
		if (GUI.Button(new Rect(160f, num3 - 5, 50f, 30f), "9999"))
		{
			AddCurreny("gems", 9999);
		}
		num3 += num4;
		GUI.Label(new Rect(20f, num3, 80f, 20f), "Balls:");
		GUI.Label(new Rect(100f, num3, 120f, 20f), Singleton<Profile>.instance.pachinkoBalls.ToString());
		num3 += 30;
		GUI.Label(new Rect(20f, num3, 40f, 20f), "Wave:");
		if (GUI.Button(new Rect(60f, num3 - 5, 50f, 30f), "<"))
		{
			SelectLevel(-1, "classic");
		}
		GUI.Label(new Rect(115f, num3, 20f, 20f), Singleton<Profile>.instance.waveToBeat.ToString());
		if (GUI.Button(new Rect(135f, num3 - 5, 50f, 30f), ">"))
		{
			SelectLevel(1, "classic");
		}
		if (GUI.Button(new Rect(190f, num3 - 5, 50f, 30f), ">>"))
		{
			for (int i = 0; i < 10; i++)
			{
				SelectLevel(1, "classic");
			}
		}
		int num5 = 20;
		int num6 = num3 + num4;
		if (GUI.Button(new Rect(num5, num6 - 5, 140f, 30f), "+100 Levels to Hero"))
		{
			Singleton<Profile>.instance.swordLevel += 100;
			Singleton<Profile>.instance.bowLevel += 100;
			Singleton<Profile>.instance.heroLevel += 100;
		}
		GUI.Label(new Rect(num5 + 144, num6, 100f, 20f), Singleton<Profile>.instance.heroLevel + "," + Singleton<Profile>.instance.swordLevel + "," + Singleton<Profile>.instance.bowLevel);
		num6 += num4;
		List<StoreData.Item> list = new List<StoreData.Item>();
		StoreAvailability_Abilities.GetAll(list);
		StoreAvailability_Helpers.Get(list);
		for (int j = 0; j < list.Count; j++)
		{
			if (list[j].locked)
			{
				if (GUI.Button(new Rect(num5, num6, 60f, 30f), "Unlock"))
				{
					Unlock(list[j].id);
				}
				GUI.Label(new Rect(num5 + 60, num6 + 5, 200f, 20f), list[j].unlockTitle + ", w" + list[j].unlockAtWave);
				num6 += 35;
				if (num6 + 30 >= num2 - 10)
				{
					num5 += 250;
					num6 = 10;
				}
			}
		}
		int num7 = num5;
		int num8 = num6;
		List<StoreData.Item> list2 = StoreAvailability.GetList(StoreAvailability.Group.Consumables);
		StoreAvailability.GetCharms(list2);
		for (int k = 0; k < list2.Count; k++)
		{
			if (list2[k].id == "special_earnsDoubleCoins")
			{
				if (!Singleton<Profile>.instance.earnsDoubleCoins)
				{
					if (GUI.Button(new Rect(num7, num8, 60f, 30f), "Buy"))
					{
						BuyConsumable(list2[k].id);
					}
					GUI.Label(new Rect(num7 + 60, num8 + 5, 200f, 20f), list2[k].title);
					num8 += 35;
					if (num8 + 30 >= num2 - 10)
					{
						num7 += 250;
						num8 = 10;
					}
				}
				continue;
			}
			if (GUI.Button(new Rect(num7, num8, 60f, 30f), "Buy"))
			{
				BuyConsumable(list2[k].id);
			}
			string text = string.Empty;
			if (ArrayContainsString(Singleton<PotionsDatabase>.instance.allIDs.ToArray(), list2[k].id))
			{
				text = Singleton<Profile>.instance.GetNumPotions(list2[k].id).ToString();
			}
			else if (ArrayContainsString(Singleton<CharmsDatabase>.instance.allIDs.ToArray(), list2[k].id))
			{
				text = Singleton<Profile>.instance.GetNumCharms(list2[k].id).ToString();
			}
			GUI.Label(new Rect(num7 + 60, num8 + 5, 200f, 20f), list2[k].title + ((!(text != string.Empty)) ? string.Empty : (", " + text)));
			num8 += 35;
			if (num8 + 30 >= num2 - 10)
			{
				num7 += 250;
				if (num7 > 520)
				{
					break;
				}
				num8 = 10;
			}
		}
	}

	private void AddCurreny(string type, int cnt)
	{
		if (type == "gems")
		{
			ApplicationUtilities.GWalletBalance(cnt, "cheat", "CREDIT_GC_PURCHASE");
			Singleton<Profile>.instance.purchasedGems += cnt;
		}
		else if (type == "coins")
		{
			Singleton<Profile>.instance.coins += cnt;
		}
	}

	private void SelectLevel(int offset, string mode)
	{
		Singleton<Profile>.instance.SetWaveLevel(Singleton<Profile>.instance.waveToBeat, Singleton<Profile>.instance.GetWaveLevel(Singleton<Profile>.instance.waveToBeat) + 1);
		Singleton<Profile>.instance.waveToBeat += offset;
		if (Singleton<Profile>.instance.GetWaveLevel(Singleton<Profile>.instance.waveToBeat) == 0)
		{
			Singleton<Profile>.instance.SetWaveLevel(Singleton<Profile>.instance.waveToBeat, 1);
		}
	}

	private void Unlock(string id)
	{
		List<StoreData.Item> list = new List<StoreData.Item>();
		StoreAvailability_Abilities.GetAll(list);
		StoreAvailability_Helpers.Get(list);
		for (int i = 0; i < list.Count; i++)
		{
			if (!(list[i].id == id))
			{
				continue;
			}
			SDFTreeNode sDFTreeNode = null;
			if (ArrayContainsString(Singleton<HelpersDatabase>.instance.allIDs, id))
			{
				Singleton<Profile>.instance.SetHelperLevel(id, 1, true);
				sDFTreeNode = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/Helpers/" + id);
			}
			else
			{
				sDFTreeNode = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/Abilities/" + id);
			}
			if (sDFTreeNode == null)
			{
				break;
			}
			int num = int.Parse(sDFTreeNode["waveToUnlock"]);
			if (num == 999)
			{
				break;
			}
			for (int j = 1; j <= num; j++)
			{
				if (Singleton<Profile>.instance.GetWaveLevel(j) == 0)
				{
					Singleton<Profile>.instance.SetWaveLevel(j, 1);
				}
			}
			break;
		}
	}

	private void BuyConsumable(string id)
	{
		if (id == "special_earnsDoubleCoins")
		{
			Singleton<Profile>.instance.earnsDoubleCoins = true;
		}
		else if (id == "special_pachinkoBallsPack")
		{
			int num = int.Parse(Singleton<Config>.instance.root.to("miscStore")["ballsPackNum"]);
			Singleton<Profile>.instance.pachinkoBalls += num;
		}
		List<StoreData.Item> list = new List<StoreData.Item>();
		StoreAvailability.GetPotions(list);
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].id == id)
			{
				int num2 = int.Parse(Singleton<PotionsDatabase>.instance.GetAttribute(id, "storePack"));
				Singleton<Profile>.instance.SetNumPotions(id, Singleton<Profile>.instance.GetNumPotions(id) + num2);
				break;
			}
		}
		list.Clear();
		StoreAvailability_DealPacks.Get(list);
		for (int j = 0; j < list.Count; j++)
		{
			if (!(list[j].id == id))
			{
				continue;
			}
			SDFTreeNode sDFTreeNode = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/DealPacks");
			if (sDFTreeNode == null)
			{
				break;
			}
			SDFTreeNode sDFTreeNode2 = sDFTreeNode.to(id);
			if (sDFTreeNode2 == null)
			{
				break;
			}
			SDFTreeNode sDFTreeNode3 = sDFTreeNode2.to("content");
			if (sDFTreeNode3 == null)
			{
				break;
			}
			List<SpoilsDisplay.Entry> list2 = new List<SpoilsDisplay.Entry>(sDFTreeNode3.attributeCount);
			foreach (KeyValuePair<string, string> attribute in sDFTreeNode3.attributes)
			{
				list2.Add(SpoilsDisplay.BuildEntry(attribute.Key, int.Parse(attribute.Value)));
			}
			foreach (SpoilsDisplay.Entry item in list2)
			{
				if (item.id == "coins")
				{
					Singleton<Profile>.instance.coins += item.count;
				}
				if (item.id == "gems")
				{
					ApplicationUtilities.GWalletBalance(item.count, "cheat", "CREDIT_GC_PURCHASE");
					Singleton<Profile>.instance.purchasedGems += item.count;
				}
				if (item.id == "balls")
				{
					Singleton<Profile>.instance.pachinkoBalls += item.count;
				}
				if (ArrayContainsString(Singleton<PotionsDatabase>.instance.allIDs.ToArray(), item.id))
				{
					Singleton<Profile>.instance.SetNumPotions(item.id, Singleton<Profile>.instance.GetNumPotions(item.id) + item.count);
				}
				else if (ArrayContainsString(Singleton<CharmsDatabase>.instance.allIDs.ToArray(), item.id))
				{
					Singleton<Profile>.instance.SetNumCharms(item.id, Singleton<Profile>.instance.GetNumCharms(item.id) + item.count);
				}
			}
			break;
		}
		list.Clear();
		StoreAvailability.GetCharms(list);
		for (int k = 0; k < list.Count; k++)
		{
			if (list[k].id == id)
			{
				Singleton<Profile>.instance.SetNumCharms(id, Singleton<Profile>.instance.GetNumCharms(id) + 1);
				break;
			}
		}
	}

	private bool ArrayContainsString(string[] theStringArray, string theStringToFind)
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
}
