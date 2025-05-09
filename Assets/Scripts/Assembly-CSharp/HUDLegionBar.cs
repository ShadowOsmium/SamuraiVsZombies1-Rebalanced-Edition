using UnityEngine;

public class HUDLegionBar
{
	private SUILabel mPercentRef;

	private int mPreviouslyKilledEnemies = -1;

	public HUDLegionBar(SUILayout layout)
	{
		mPercentRef = (SUILabel)layout["legionPercent"];
	}

	public void Update()
	{
		if (WeakGlobalInstance<WaveManager>.instance != null)
		{
			UpdateProgress();
		}
	}

	public bool IsTouchZone(Vector2 pos)
	{
		return false;
	}

	private void UpdateProgress()
	{
		if (mPreviouslyKilledEnemies != WeakGlobalInstance<WaveManager>.instance.enemiesKilledSoFar)
		{
			mPreviouslyKilledEnemies = WeakGlobalInstance<WaveManager>.instance.enemiesKilledSoFar;
			int num = mPreviouslyKilledEnemies * 100 / WeakGlobalInstance<WaveManager>.instance.totalEnemies;
			mPercentRef.text = num + "%";
		}
	}
}
