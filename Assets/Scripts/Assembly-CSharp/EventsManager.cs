using System.Collections.Generic;

public class EventsManager : Singleton<EventsManager>
{
	private List<string> mEvents = new List<string>();

	private List<KeyValuePair<string, string[]>> mRewards = new List<KeyValuePair<string, string[]>>();

	public List<string> activeRewards
	{
		get
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, string[]> mReward in mRewards)
			{
				if (!Singleton<Profile>.instance.IsEventRewardsGiven(mReward.Key))
				{
					string[] value = mReward.Value;
					foreach (string item in value)
					{
						list.Add(item);
					}
				}
			}
			return list;
		}
	}

	public EventsManager()
	{
		ResetCachedData();
		SingletonMonoBehaviour<ResourcesManager>.instance.onInvalidateCache += ForceReload;
	}

	public bool IsEventActive(string eventID)
	{
		if (eventID == null || eventID == string.Empty)
		{
			return true;
		}
		eventID = eventID.ToLower();
		foreach (string mEvent in mEvents)
		{
			if (mEvent == eventID)
			{
				return true;
			}
		}
		return false;
	}

	public void ForceReload()
	{
		ResetCachedData();
	}

	public void CashInRewards()
	{
		foreach (KeyValuePair<string, string[]> mReward in mRewards)
		{
			if (!Singleton<Profile>.instance.IsEventRewardsGiven(mReward.Key))
			{
				Singleton<Profile>.instance.SetEventRewardsGiven(mReward.Key);
				string[] value = mReward.Value;
				foreach (string text in value)
				{
					CashIn.From(text, 1, mReward.Key + "_" + text, "CREDIT_IN_GAME_AWARD");
				}
			}
		}
	}

	private void ResetCachedData()
	{
		mEvents.Clear();
		mRewards.Clear();
		SDFTreeNode sDFTreeNode = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/Events");
		if (sDFTreeNode != null)
		{
			foreach (KeyValuePair<string, string> attribute in sDFTreeNode.attributes)
			{
				mEvents.Add(attribute.Value.ToLower());
			}
		}
		if (!sDFTreeNode.hasChild("rewards"))
		{
			return;
		}
		SDFTreeNode sDFTreeNode2 = sDFTreeNode.to("rewards");
		foreach (KeyValuePair<string, SDFTreeNode> child in sDFTreeNode2.childs)
		{
			if (child.Value.attributeCount <= 0)
			{
				continue;
			}
			string[] array = new string[child.Value.attributeCount];
			int num = 0;
			foreach (KeyValuePair<string, string> attribute2 in child.Value.attributes)
			{
				array[num++] = attribute2.Value;
			}
			mRewards.Add(new KeyValuePair<string, string[]>(child.Key, array));
		}
	}
}
