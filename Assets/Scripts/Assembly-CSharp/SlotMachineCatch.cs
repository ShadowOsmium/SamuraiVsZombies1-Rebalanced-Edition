using System.Collections.Generic;
using UnityEngine;

public class SlotMachineCatch : MonoBehaviour
{
	private struct PachinkoAwardData
	{
		public string awardName;

		public int weight;
	}

	private List<PachinkoAwardData> awardList = new List<PachinkoAwardData>();

	private int awardWeightTotal;

	private SDFTreeNode mPachinkoData;

	private void Start()
	{
		mPachinkoData = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/Pachinko");
		foreach (KeyValuePair<string, SDFTreeNode> child in mPachinkoData.childs)
		{
			PachinkoAwardData item = default(PachinkoAwardData);
			item.awardName = child.Key;
			SDFTreeNode value = child.Value;
			try
			{
				item.weight = int.Parse(value["weight"]);
			}
			catch
			{
			}
			if (Debug.isDebugBuild && item.awardName == "gem5")
			{
				item.weight = 500;
			}
			awardWeightTotal += item.weight;
			awardList.Add(item);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		string result = "loss";
		if (!Singleton<Profile>.instance.hasWonPachinkoBefore)
		{
			result = "revivePotion";
			Singleton<Profile>.instance.hasWonPachinkoBefore = true;
		}
		else
		{
			int num = Random.Range(0, awardWeightTotal);
			int num2 = 0;
			foreach (PachinkoAwardData award in awardList)
			{
				num2 += award.weight;
				if (num < num2)
				{
					result = award.awardName;
					break;
				}
			}
		}
		WeakGlobalSceneBehavior<PachinkoMachineImpl>.instance.EnqueSlotSpin(result);
	}
}
