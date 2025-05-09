using UnityEngine;

public class FlowerCatch : MonoBehaviour
{
	private bool mOpen;

	private TaggedAnimPlayer animPlayer;

	private void Start()
	{
		animPlayer = base.transform.parent.GetComponent<TaggedAnimPlayer>();
		if (animPlayer == null && Debug.isDebugBuild)
		{
			Debug.Log("Failed to find TaggedAnimPlayer on " + base.transform.parent.name, base.transform.parent);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		GameObject gameObject;
		if (!mOpen)
		{
			Singleton<Profile>.instance.coins += WeakGlobalSceneBehavior<PachinkoMachineImpl>.instance.flowerRewardClosed;
			Singleton<Profile>.instance.Save();
			WeakGlobalSceneBehavior<PachinkoMachineImpl>.instance.AddToSpoil("coins", WeakGlobalSceneBehavior<PachinkoMachineImpl>.instance.flowerRewardClosed);
			gameObject = (GameObject)Object.Instantiate(Resources.Load("FX/Plus3Pachinko"), base.transform.position + new Vector3(0f, 0f, -2f), Quaternion.identity);
			Object.Destroy(gameObject, 0.92f);
			animPlayer.PlayAnim("open", 1f, WrapMode.Once, 2f);
			GetComponent<SoundThemePlayer>().PlaySoundEvent("FlowerOpen");
			mOpen = true;
		}
		else
		{
			Singleton<Profile>.instance.coins += WeakGlobalSceneBehavior<PachinkoMachineImpl>.instance.flowerRewardOpen;
			Singleton<Profile>.instance.Save();
			WeakGlobalSceneBehavior<PachinkoMachineImpl>.instance.AddToSpoil("coins", WeakGlobalSceneBehavior<PachinkoMachineImpl>.instance.flowerRewardOpen);
			gameObject = (GameObject)Object.Instantiate(Resources.Load("FX/Plus5Pachinko"), base.transform.position + new Vector3(0f, 0f, -2f), Quaternion.identity);
			Object.Destroy(gameObject, 0.92f);
			animPlayer.PlayAnim("open", 1f, WrapMode.Once, -2f);
			GetComponent<SoundThemePlayer>().PlaySoundEvent("FlowerClose");
			mOpen = false;
		}
		if (WeakGlobalSceneBehavior<PachinkoMachineImpl>.instance.multiplier > 1)
		{
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
