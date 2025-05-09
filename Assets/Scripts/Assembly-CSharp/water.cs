using UnityEngine;

public class water : MonoBehaviour
{
	public int materialIndex;

	public Vector2 uvAnimationRate = new Vector2(1f, 0f);

	public string textureName = "_MainTex";

	private Vector2 uvOffset = Vector2.zero;

	private void LateUpdate()
	{
		uvOffset += uvAnimationRate * Time.deltaTime;
		if (base.GetComponent<Renderer>().enabled)
		{
			base.GetComponent<Renderer>().materials[materialIndex].SetTextureOffset(textureName, uvOffset);
		}
	}
}
