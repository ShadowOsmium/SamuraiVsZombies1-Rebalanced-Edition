using System.Collections.Generic;
using UnityEngine;

public class StoreData
{
	public class Item
	{
		public string id = string.Empty;

		public string icon = string.Empty;

		public string title = string.Empty;

		public Details details = new Details();

		public string unlockTitle = string.Empty;

		public string unlockCondition = string.Empty;

		public bool isFoundInPresent;

		public List<string> containedInPresent = new List<string>();

		public Cost cost;

		public bool isEvent;

		public bool locked;

		public int unlockAtWave;

		public List<SpoilsDisplay.Entry> contentList;

		public int _reserved;

		private OnSUIGenericCallback mPurchaseFunc;

		public bool isNovelty
		{
			get
			{
				foreach (string novelty in Singleton<Profile>.instance.novelties)
				{
					if (novelty == id)
					{
						return true;
					}
				}
				return false;
			}
		}

		public Item(OnSUIGenericCallback execFunc)
		{
			mPurchaseFunc = execFunc;
		}

		public void Apply()
		{
			mPurchaseFunc();
			if (cost.soft > 0)
			{
				Singleton<Analytics>.instance.LogEvent("CoinsSpent", string.Empty, cost.soft);
			}
			if (cost.hard > 0)
			{
				Singleton<Analytics>.instance.LogEvent("GemsSpent", string.Empty, cost.hard);
			}
		}

		public override bool Equals(object o)
		{
			if (o == null)
			{
				return false;
			}
			Item item = (Item)o;
			return icon == item.icon && title == item.title && details == item.details && cost == item.cost;
		}

		public override int GetHashCode()
		{
			return icon.GetHashCode() + title.GetHashCode() + details.GetHashCode() + cost.GetHashCode();
		}

		public static bool operator ==(Item a, Item b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Item a, Item b)
		{
			return !a.Equals(b);
		}
	}

	public class ItemsListSorter : IComparer<Item>
	{
		public int Compare(Item a, Item b)
		{
			if (a.locked == b.locked)
			{
				if (a.locked)
				{
					return a.unlockAtWave - b.unlockAtWave;
				}
				return a._reserved - b._reserved;
			}
			if (a.locked)
			{
				return 1;
			}
			return -1;
		}
	}

	public class Details
	{
		private List<KeyValuePair<string, string[]>> mStats = new List<KeyValuePair<string, string[]>>();

		private string mDescription = string.Empty;

		private bool mDescriptionIsSmall;

		private string leftColumnTitle = string.Empty;

		private string rightColumnTitle = string.Empty;

		public void AddStat(string iconFile, string val1, string val2)
		{
			if (!(val1 == "0") || !(val2 == "0"))
			{
				string[] value = new string[2]
				{
					val1,
					(!(val1 == val2)) ? val2 : string.Empty
				};
				mStats.Add(new KeyValuePair<string, string[]>(iconFile, value));
			}
		}

		public void AddDescription(string description)
		{
			mDescription = Singleton<Localizer>.instance.Parse(description);
			mDescriptionIsSmall = false;
		}

		public void AddSmallDescription(string description)
		{
			mDescription = Singleton<Localizer>.instance.Parse(description);
			mDescriptionIsSmall = true;
		}

		public void SetColumns(int leftLevel, int rightLevel)
		{
			leftColumnTitle = string.Format(Singleton<Localizer>.instance.Get("stat_level"), leftLevel.ToString());
			rightColumnTitle = string.Format(Singleton<Localizer>.instance.Get("stat_level"), rightLevel.ToString());
		}

		public void Render(SUILayout layout, bool locked, bool isNotEnoughDialog = false)
		{
			if (isNotEnoughDialog) return;
			if (mDescription != string.Empty)
			{
				SUILabel sUILabel = ((!mDescriptionIsSmall) ? ((SUILabel)layout["details"]) : ((SUILabel)layout["detailsSmall"]));
				sUILabel.text = mDescription;
				sUILabel.visible = true;
			}
			if (locked)
			{
				return;
			}
			bool flag = false;
			foreach (KeyValuePair<string, string[]> mStat in mStats)
			{
				if (mStat.Value[1] != string.Empty)
				{
					flag = true;
					break;
				}
			}
			Vector2 vector = new Vector2(0f, (3 - mStats.Count) * 30);
			SUILabel sUILabel2 = (SUILabel)layout["levelLeft"];
			SUILabel sUILabel3 = (SUILabel)layout["levelRight"];
			sUILabel2.text = leftColumnTitle;
			sUILabel2.position += vector;
			sUILabel2.visible = true;
			if (flag)
			{
				sUILabel3.text = rightColumnTitle;
				sUILabel3.position += vector;
				sUILabel3.visible = true;
			}
			int num = 0;
			foreach (KeyValuePair<string, string[]> mStat2 in mStats)
			{
				string text = num.ToString();
				SUISprite sUISprite = (SUISprite)layout["statIcon" + text];
				SUILabel sUILabel4 = (SUILabel)layout["statLeft" + text];
				SUILabel sUILabel5 = (SUILabel)layout["statRight" + text];
				SUISprite sUISprite2 = (SUISprite)layout["statArrow" + text];
				sUISprite.position += vector;
				sUILabel4.position += vector;
				sUILabel5.position += vector;
				sUISprite2.position += vector;
				sUISprite.texture = "Sprites/Menus/" + mStat2.Key;
				sUISprite.visible = true;
				sUILabel4.text = mStat2.Value[0];
				sUILabel4.visible = true;
				if (flag)
				{
					if (mStat2.Value[1] != string.Empty)
					{
						sUILabel5.text = mStat2.Value[1];
						sUILabel5.fontColor = Color.green;
						sUISprite2.visible = true;
					}
					else
					{
						sUILabel5.text = mStat2.Value[0];
					}
					sUILabel5.visible = true;
				}
				num++;
			}
		}
	}
}
