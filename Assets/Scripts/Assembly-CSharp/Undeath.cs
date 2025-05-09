public class Undeath
{
	public static float regeneration
	{
		get
		{
			if (Singleton<Profile>.instance.undeathLevel <= 0)
			{
				return 0f;
			}
			SDFTreeNode sDFTreeNode = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/Undeath");
			if (sDFTreeNode == null)
			{
				return 0f;
			}
			SDFTreeNode sDFTreeNode2 = null;
			int num = Singleton<Profile>.instance.undeathLevel;
			while (sDFTreeNode2 == null && num > 0)
			{
				sDFTreeNode2 = sDFTreeNode.to(num);
				num--;
			}
			if (sDFTreeNode2 == null)
			{
				return 0f;
			}
			if (!sDFTreeNode2.hasAttribute("regenerateHealth"))
			{
				return 0f;
			}
			return float.Parse(sDFTreeNode2["regenerateHealth"]);
		}
	}
}
