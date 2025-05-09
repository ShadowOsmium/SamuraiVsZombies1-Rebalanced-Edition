using UnityEngine;

public class GraveHandsEffect : WeakGlobalInstance<GraveHandsEffect>
{
	private const float kZDeltaBetweenHands = 200f;

	private const float kTimerBetweenHands = 0.2f;

	private readonly string kFxPrefabFile = "Props/PFGraveHands";

	private GameObject mPrefab;

	private int mNumHandsInQueue;

	private float mNextZToSpawn;

	private float mTimerForNextSpawn;

	private float mDurationForEach;

	public GraveHandsEffect()
	{
		SetUniqueInstance(this);
	}

	public void Update()
	{
		ProcessQueue();
	}

	public void Play(float range, float duration)
	{
		CachePrefab();
		mNumHandsInQueue = (int)(range / 200f) + 1;
		mNextZToSpawn = WeakGlobalSceneBehavior<InGameImpl>.instance.hero.position.z - 200f;
		mTimerForNextSpawn = 0f;
		mDurationForEach = duration - 0.2f * (float)mNumHandsInQueue;
		ProcessQueue();
	}

	private void CachePrefab()
	{
		if (mPrefab == null)
		{
			mPrefab = Resources.Load(kFxPrefabFile) as GameObject;
		}
	}

	private void ProcessQueue()
	{
		if (mNumHandsInQueue > 0)
		{
			mTimerForNextSpawn -= Time.deltaTime;
			if (!(mTimerForNextSpawn > 0f))
			{
				GameObject gameObject = Object.Instantiate(mPrefab) as GameObject;
				Object.Destroy(gameObject, mDurationForEach);
				Vector3 position = WeakGlobalSceneBehavior<InGameImpl>.instance.hero.position;
				position.z = mNextZToSpawn;
				position.y = WeakGlobalInstance<RailManager>.instance.GetY(position.z);
				gameObject.transform.position = position;
				mNumHandsInQueue--;
				mNextZToSpawn -= 200f;
				mTimerForNextSpawn = 0.2f;
			}
		}
	}
}
