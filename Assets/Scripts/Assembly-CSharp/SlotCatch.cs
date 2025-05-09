using UnityEngine;

public class SlotCatch : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		Singleton<Profile>.instance.coins += WeakGlobalSceneBehavior<PachinkoMachineImpl>.instance.slotReward;
		Singleton<Profile>.instance.Save();
		WeakGlobalSceneBehavior<PachinkoMachineImpl>.instance.AddToSpoil("coins", WeakGlobalSceneBehavior<PachinkoMachineImpl>.instance.slotReward);
		GameObject gameObject = (GameObject)Object.Instantiate(Resources.Load("FX/Plus25Coins"), base.transform.position + new Vector3(0f, 0f, -2f), Quaternion.identity);
		Object.Destroy(gameObject, 0.92f);
		if (WeakGlobalSceneBehavior<PachinkoMachineImpl>.instance.multiplier > 1)
		{
			foreach (Transform item in gameObject.transform)
			{
				Debug.Log("child named: " + item.name);
			}
			MeshRenderer meshRenderer = (MeshRenderer)gameObject.transform.Find("Visuals/Multiplier").GetComponent<Renderer>();
			meshRenderer.enabled = true;
			switch (WeakGlobalSceneBehavior<PachinkoMachineImpl>.instance.multiplier)
			{
			case 2:
				meshRenderer.material = Resources.Load("Textures/FX/Materials/multiplierCounter2") as Material;
				break;
			case 3:
				meshRenderer.material = Resources.Load("Textures/FX/Materials/multiplierCounter3") as Material;
				break;
			case 4:
				meshRenderer.material = Resources.Load("Textures/FX/Materials/multiplierCounter4") as Material;
				break;
			case 5:
				meshRenderer.material = Resources.Load("Textures/FX/Materials/multiplierCounter5") as Material;
				break;
			}
		}
		Object.Destroy(other.gameObject);
	}
}
