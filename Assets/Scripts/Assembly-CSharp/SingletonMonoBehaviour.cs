using System;
using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : Component
{
	private static T mUniqueInstance;

	public static T instance
	{
		get
		{
			if ((UnityEngine.Object)mUniqueInstance == (UnityEngine.Object)null)
			{
				GameObject gameObject = new GameObject("SingletonMonoBehaviour", typeof(T));
				mUniqueInstance = gameObject.GetComponent<T>();
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
			}
			return mUniqueInstance;
		}
	}

	public SingletonMonoBehaviour()
	{
		if ((UnityEngine.Object)mUniqueInstance != (UnityEngine.Object)null)
		{
			throw new InvalidOperationException("Cannot manually instantiate a Singleton<> class.");
		}
	}
}
