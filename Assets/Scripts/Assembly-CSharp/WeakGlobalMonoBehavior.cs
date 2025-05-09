using UnityEngine;

public class WeakGlobalMonoBehavior<T> : MonoBehaviour
{
	private static TypedWeakReference<T> mUniqueInstance;

	public static T instance
	{
		get
		{
			if (mUniqueInstance == null)
			{
				return default(T);
			}
			return mUniqueInstance.ptr;
		}
	}

	protected void SetUniqueInstance(T ptr)
	{
		mUniqueInstance = new TypedWeakReference<T>(ptr);
	}
}
