using UnityEngine;

public class Base : Character
{
	public Base(GameObject baseObject)
	{
		base.isBase = true;
		ActivateHealthBar();
		base.controlledObject = baseObject;
		base.controller.Idle();
		SetupBaseAttributes();
	}

	public override void Update()
	{
		base.Update();
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	public void Revive()
	{
		base.controller.startedDieAnim = false;
		if (base.health <= 0f)
		{
			base.controller.StopWalking();
			base.controller.Idle();
			base.controller.PlayHurtAnim("revive");
		}
		base.health = base.maxHealth;
	}

	private void SetupBaseAttributes()
	{
		SDFTreeNode sDFTreeNode = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/Base");
		SDFTreeNode sDFTreeNode2 = sDFTreeNode.to(string.Format("{0:000}", Singleton<Profile>.instance.baseLevel));
		if (sDFTreeNode2 == null)
		{
			Debug.Log("ERROR: Could not find proper Base data for level: " + Singleton<Profile>.instance.baseLevel);
			return;
		}
		base.maxHealth = float.Parse(sDFTreeNode2["health"]);
		base.health = base.maxHealth;
	}
}
