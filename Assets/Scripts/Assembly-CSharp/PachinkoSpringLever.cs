using UnityEngine;

public class PachinkoSpringLever : MonoBehaviour
{
	private enum ELeverState
	{
		eLeverState_Waiting = 0,
		eLeverState_Pulling = 1,
		eLeverState_Release = 2
	}

	public float MaxTouchYPos;

	public float MinTouchYPos;

	public float MaxRotation;

	public float MinRotation;

	public float LeverSmoothing = 2f;

	public float LeverSnapBackSpeed = 50f;

	public GameObject BallLauncherObject;

	public float NumberOfShotsAllowerPerSecond = 2f;

	public float MaxForce = -800f;

	private Vector2 touchStartPosition = new Vector2(0f, 0f);

	private bool mShouldFireBall;

	private float mTimer;

	private float mTimerResetValue;

	private Quaternion mStartRotation;

	private ELeverState mState;

	private void Start()
	{
		mStartRotation = base.transform.rotation;
		mTimerResetValue = 1f / NumberOfShotsAllowerPerSecond;
		mShouldFireBall = false;
		mState = ELeverState.eLeverState_Waiting;
	}

	private void Update()
	{
		if (Time.timeScale == 0f)
		{
			return;
		}
		if (mTimer > 0f)
		{
			mTimer -= Time.deltaTime;
		}
		float num = 640f;
		float num2 = 768f;
		switch (mState)
		{
		case ELeverState.eLeverState_Waiting:
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, mStartRotation, Time.deltaTime * LeverSnapBackSpeed);
			if (IsTouchingLeverArea())
			{
				touchStartPosition = new Vector2(0f, WeakGlobalInstance<SUIScreen>.instance.inputs.position.y - num);
				mState = ELeverState.eLeverState_Pulling;
			}
			break;
		case ELeverState.eLeverState_Pulling:
			if (WeakGlobalInstance<SUIScreen>.instance.inputs.isTouching)
			{
				Vector2 position = WeakGlobalInstance<SUIScreen>.instance.inputs.position;
				float num4 = num2 - num;
				float num5 = (position.y - touchStartPosition.y - num) / num4;
				if (IsTouchingLeverArea())
				{
					position.y = Mathf.Min(MinTouchYPos, Mathf.Max(position.y, MaxTouchYPos));
					float x = MaxRotation * num5;
					Quaternion to = Quaternion.Euler(x, 270f, 0f);
					base.transform.rotation = Quaternion.Slerp(base.transform.rotation, to, Time.deltaTime * 100f);
					if (mTimer <= 0f)
					{
						mShouldFireBall = true;
					}
				}
			}
			else
			{
				mState = ELeverState.eLeverState_Release;
			}
			break;
		case ELeverState.eLeverState_Release:
			if (mShouldFireBall)
			{
				if (Singleton<Profile>.instance.pachinkoBalls > 0)
				{
					mShouldFireBall = false;
					mTimer = mTimerResetValue;
					float num3 = Quaternion.Angle(base.transform.rotation, mStartRotation) / 30f;
					BallLauncherObject.SendMessage("OnLaunchBall", num3);
					Singleton<Profile>.instance.pachinkoBalls--;
					Singleton<Profile>.instance.Save();
				}
				else
				{
					GameObject[] array = GameObject.FindGameObjectsWithTag("PachinkoBall");
					if (array.Length <= 0)
					{
						Debug.Log("Trigger store from the lever");
						WeakGlobalSceneBehavior<PachinkoMachineImpl>.instance.TriggerStoreFromLever();
					}
				}
			}
			mState = ELeverState.eLeverState_Waiting;
			break;
		}
	}

	private bool IsTouchingLeverStartArea()
	{
		if (WeakGlobalInstance<SUIScreen>.instance.inputs.isTouching)
		{
			Vector2 position = WeakGlobalInstance<SUIScreen>.instance.inputs.position;
			return position.x > SUIScreen.width * 0.71f && position.y > 300f && position.y <= 650f;
		}
		return false;
	}

	private bool IsTouchingLeverArea()
	{
		if (WeakGlobalInstance<SUIScreen>.instance.inputs.isTouching)
		{
			Vector2 position = WeakGlobalInstance<SUIScreen>.instance.inputs.position;
			return position.x > SUIScreen.width * 0.71f && position.y > 640f && position.y <= 768f;
		}
		return false;
	}
}
