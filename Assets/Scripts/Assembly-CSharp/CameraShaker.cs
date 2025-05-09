using UnityEngine;

public class CameraShaker : MonoBehaviour
{
	private const float kCameraShakeReductionRate = 5f;

	private const float kMinCameraShakeRolloff = 500f;

	private const float kMaxCameraShakeRolloff = 1000f;

	private static CameraShaker mInstance;

	private static float mLastValidDeltaTime;

	private Transform mCameraTransform;

	private float mIntensity;

	private bool mFlipBit;

	public static void RequestShake(Vector3 shakeOrigin, float shakeIntensity)
	{
		if (shakeIntensity <= 0f)
		{
			return;
		}
		if (mInstance == null || mInstance.mCameraTransform != Camera.main.transform)
		{
			GameObject gameObject = new GameObject("CameraShaker");
			mInstance = gameObject.AddComponent<CameraShaker>();
			mInstance.mCameraTransform = Camera.main.transform;
		}
		if (!(mInstance.mIntensity > 0f))
		{
			mInstance.StartShake(shakeOrigin, shakeIntensity);
			if (mLastValidDeltaTime == 0f)
			{
				mLastValidDeltaTime = Time.maximumDeltaTime;
			}
		}
	}

	private void StartShake(Vector3 shakeOrigin, float shakeIntensity)
	{
		float z = shakeOrigin.z;
		float z2 = mCameraTransform.position.z;
		float num = Mathf.Abs(z - z2);
		if (num <= 500f)
		{
			mIntensity = Mathf.Max(shakeIntensity, mIntensity);
		}
		else if (num < 1000f)
		{
			mIntensity = Mathf.Max(mIntensity, shakeIntensity * (1000f - num) / 500f);
		}
	}

	private void LateUpdate()
	{
		if (mIntensity <= 0f)
		{
			return;
		}
		Vector3 eulerAngles = mCameraTransform.eulerAngles;
		if (eulerAngles.z == 0f)
		{
			mFlipBit = !mFlipBit;
			if (mFlipBit)
			{
				eulerAngles.z = mIntensity;
			}
			else
			{
				eulerAngles.z = 0f - mIntensity;
			}
		}
		else
		{
			eulerAngles.z = 0f;
		}
		float num = Time.deltaTime;
		if (Time.timeScale == 0f)
		{
			num = mLastValidDeltaTime;
		}
		else if (Time.timeScale != 1f)
		{
			num /= Time.timeScale;
		}
		mLastValidDeltaTime = num;
		mIntensity -= Mathf.Max(5f * num, mIntensity * 5f * num);
		if (mIntensity <= 0f)
		{
			eulerAngles.z = 0f;
			Object.Destroy(base.gameObject, 0.1f);
			mInstance = null;
		}
		mCameraTransform.eulerAngles = eulerAngles;
	}
}
