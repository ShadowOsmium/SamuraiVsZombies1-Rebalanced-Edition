using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Framework/Do Not Use/PrefabLink")]
public class PrefabLink : MonoBehaviour
{
	[HideInInspector]
	public string sourcePrefabPath;

	[HideInInspector]
	public string basePrefabPath;

	[HideInInspector]
	public Object[] exemptObjects;

	private void Start()
	{
		if (!Application.isPlaying && sourcePrefabPath == "DELETE")
		{
			Object.DestroyImmediate(base.transform.root.gameObject);
		}

		//they're too loud and I don't want to save the scenes just in case
		if (name == "GateMagic" || name == "Vortex")
		{
			GetComponent<AudioSource>().volume *= 0.4f;
		}
	}
}
