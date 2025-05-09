using UnityEngine;

public class HideByPlatform : MonoBehaviour
{
	public bool hideOnSlowDrawCallDevices;

	public bool hideOnFastDrawCallDevices;

	public bool hideOnSlowAlphaDevices;

	public bool hideOnFastAlphaDevices;

	public bool dontDestroyWhenHide;

	private bool mAlreadyHidden;

	//private static UnityEngine.iOS.DeviceGeneration? mGen;

	//nah
	//private void Awake()
	//{
	//	switch (SystemInfo.graphicsDeviceName)
	//	{
	//	case "PowerVR SGX 543":
	//	case "PowerVR SGX 540":
	//	case "Mali-400 MP":
	//	case "Adreno 205":
	//	case "Adreno (TM) 220":
	//	case "Adreno 220":
	//	case "ULP GeForce":
	//	case "NVIDIA Tegra":
	//	case "NVIDIA Tegra 3":
	//		if (SystemInfo.processorCount > 1)
	//		{
	//			QualitySettings.currentLevel = QualityLevel.Fantastic;
	//			if (Debug.isDebugBuild)
	//			{
	//				Debug.Log("Set Quality Level: Fantastic");
	//			}
	//			break;
	//		}
	//		QualitySettings.currentLevel = QualityLevel.Good;
	//		HideThisObject();
	//		if (Debug.isDebugBuild)
	//		{
	//			Debug.Log("Set Quality Level: Good");
	//		}
	//		break;
	//	default:
	//		QualitySettings.currentLevel = QualityLevel.Fast;
	//		HideThisObject();
	//		if (Debug.isDebugBuild)
	//		{
	//			Debug.Log("Set Quality Level: Fast");
	//		}
	//		break;
	//	}
	//}
//
	//private void HideThisObject()
	//{
	//	if (mAlreadyHidden)
	//	{
	//		return;
	//	}
	//	mAlreadyHidden = true;
	//	if (dontDestroyWhenHide)
	//	{
	//		Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
	//		Renderer[] array = componentsInChildren;
	//		foreach (Renderer renderer in array)
	//		{
	//			renderer.enabled = false;
	//		}
	//	}
	//	else
	//	{
	//		Object.Destroy(base.gameObject);
	//	}
	//}
//
	//private static UnityEngine.iOS.DeviceGeneration GetGeneration()
	//{
	//	UnityEngine.iOS.DeviceGeneration? iPhoneGeneration = mGen;
	//	if (!iPhoneGeneration.HasValue)
	//	{
	//		mGen = UnityEngine.iOS.Device.generation;
	//		UnityEngine.iOS.DeviceGeneration? iPhoneGeneration2 = mGen;
	//		if (iPhoneGeneration2.GetValueOrDefault() == UnityEngine.iOS.DeviceGeneration.Unknown && iPhoneGeneration2.HasValue && SUIScreen.isDevice_iPhone4)
	//		{
	//			mGen = UnityEngine.iOS.DeviceGeneration.iPhone4;
	//		}
	//	}
	//	return mGen.Value;
	//}
//
	//private void SetQualityLevel(QualityLevel level)
	//{
	//	if (QualitySettings.currentLevel > level)
	//	{
	//		while (QualitySettings.currentLevel > level)
	//		{
	//			QualitySettings.DecreaseLevel();
	//		}
	//	}
	//	else
	//	{
	//		while (QualitySettings.currentLevel < level)
	//		{
	//			QualitySettings.IncreaseLevel();
	//		}
	//	}
	//}
}
