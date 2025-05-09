using UnityEngine;

public class PachinkoBallLauncher : MonoBehaviour
{
	public GameObject PachinkoBall;

	public float BaseXForce = -1800f;

	public float BaseYForce = 1100f;

	public float AdditionalXForce = -800f;

	public float AdditionalYForce = 1000f;

	private void OnLaunchBall(float forceScalar)
	{
		Singleton<Profile>.instance.pachinkoBallsLaunched++;
		if (Singleton<Profile>.instance.pachinkoBallsLaunched > 1000)
		{
			SingletonMonoBehaviour<Achievements>.instance.Award("SAMUZOMBIE_ACHIEVE_YOU_ARE_WINNER");
		}
		//if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
		//{
		//	BaseXForce = -1800f;
		//	BaseYForce = 1100f;
		//	AdditionalXForce = -900f;
		//	AdditionalYForce = 920f;
		//}
		GameObject gameObject = Object.Instantiate(PachinkoBall, base.transform.position, Quaternion.identity) as GameObject;
		Vector3 force = new Vector3(BaseXForce + forceScalar * AdditionalXForce, BaseYForce + forceScalar * AdditionalYForce, 0f);
		gameObject.GetComponent<Rigidbody>().AddForce(force);
	}
}
