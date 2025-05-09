using UnityEngine;

public class EffectKiller : MonoBehaviour
{
	public bool deleteWhenAllChildrenDead = true;

	public float autoDeleteAfterTime;

	public bool letParticlesFinishForTimedDelete;

	private bool mHadChildren;

	private bool mStoppedParticleEmits;

	private ParticleEmitter[] mEmitters;

	private void Start()
	{
		mHadChildren = base.transform.GetChildCount() > 0;
		if (autoDeleteAfterTime > 0f)
		{
			if (!letParticlesFinishForTimedDelete)
			{
				Object.Destroy(base.gameObject, autoDeleteAfterTime);
			}
			else
			{
				mEmitters = GetComponentsInChildren<ParticleEmitter>();
				if (mEmitters.Length == 0)
				{
					letParticlesFinishForTimedDelete = false;
					Object.Destroy(base.gameObject, autoDeleteAfterTime);
				}
			}
		}
		else
		{
			letParticlesFinishForTimedDelete = false;
		}
		if (!mHadChildren && !letParticlesFinishForTimedDelete)
		{
			base.enabled = false;
		}
	}

	private void Update()
	{
		if (mHadChildren && base.transform.GetChildCount() <= 0)
		{
			Object.Destroy(base.gameObject);
		}
		if (!letParticlesFinishForTimedDelete)
		{
			return;
		}
		if (autoDeleteAfterTime > 0f)
		{
			autoDeleteAfterTime -= Time.deltaTime;
		}
		if (!(autoDeleteAfterTime <= 0f))
		{
			return;
		}
		if (mStoppedParticleEmits)
		{
			bool flag = false;
			if (mEmitters != null)
			{
				ParticleEmitter[] array = mEmitters;
				foreach (ParticleEmitter particleEmitter in array)
				{
					if (!(particleEmitter == null))
					{
						particleEmitter.emit = false;
						if (particleEmitter.particleCount > 0)
						{
							flag = true;
						}
					}
				}
			}
			if (!flag)
			{
				Object.Destroy(base.gameObject);
			}
			return;
		}
		mStoppedParticleEmits = true;
		if (mEmitters == null)
		{
			return;
		}
		ParticleEmitter[] array2 = mEmitters;
		foreach (ParticleEmitter particleEmitter2 in array2)
		{
			if (particleEmitter2 != null)
			{
				particleEmitter2.emit = false;
			}
		}
	}
}
