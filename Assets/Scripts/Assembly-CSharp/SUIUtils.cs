using UnityEngine;

internal class SUIUtils
{
	public static Vector2 touchToUser(Vector3 pos)
	{
		return WeakGlobalInstance<SUIScreen>.instance.autoScaler.toGame(new Vector2(pos.x, (float)Screen.height - pos.y - 1f));
	}

	public static Vector2 touchToUser(Vector2 pos)
	{
		return WeakGlobalInstance<SUIScreen>.instance.autoScaler.toGame(new Vector2(pos.x, (float)Screen.height - pos.y - 1f));
	}

	public static Vector2 userToUnity(Vector2 pos)
	{
		return new Vector2(pos.x / SUIScreen.width, 1f - pos.y / SUIScreen.height);
	}

	public static Rect unityToUser(Rect r)
	{
		return WeakGlobalInstance<SUIScreen>.instance.autoScaler.toGame(new Rect(r.xMin, (float)Screen.height - r.yMin - r.height - 1f, r.width, r.height));
	}
}
