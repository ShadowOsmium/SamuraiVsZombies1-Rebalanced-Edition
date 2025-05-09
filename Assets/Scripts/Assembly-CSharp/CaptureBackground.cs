using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CaptureBackground : MonoBehaviour
{
	public Material postMaterial;

	public LayerMask postLayer;

	public int blurScale = 4;

	private RenderTexture m_CaptureTexture;

	private int m_CaptureWidth;

	private int m_CaptureHeight;

	private int m_CaptureBlur;

	private static readonly string TEXTURE_NAME = "__CaptureTexture";

	private GameObject m_PostCamera;

	protected void Start()
	{
		if (!SystemInfo.supportsRenderTextures)
		{
			base.enabled = false;
			return;
		}
		if ((bool)postMaterial)
		{
			m_PostCamera = new GameObject(base.name + " __PostCaptureBackground__", typeof(Camera));
			m_PostCamera.GetComponent<Camera>().CopyFrom((Camera)GetComponent(typeof(Camera)));
			m_PostCamera.transform.parent = base.transform;
			m_PostCamera.GetComponent<Camera>().clearFlags = CameraClearFlags.Nothing;
			m_PostCamera.GetComponent<Camera>().cullingMask = postLayer.value;
			m_PostCamera.GetComponent<Camera>().depth += 1f;
		}
		StartCoroutine(ChangeClearFlags());
	}

	private IEnumerator ChangeClearFlags()
	{
		GetComponent<Camera>().clearFlags = CameraClearFlags.Depth;
		yield return null;
		GetComponent<Camera>().clearFlags = CameraClearFlags.Skybox;
	}

	protected void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		blurScale = Math.Max(1, blurScale);
		if (m_CaptureTexture == null || m_CaptureWidth != Screen.width || m_CaptureHeight != Screen.height || m_CaptureBlur != blurScale)
		{
			UnityEngine.Object.Destroy(m_CaptureTexture);
			m_CaptureWidth = Screen.width;
			m_CaptureHeight = Screen.height;
			m_CaptureBlur = blurScale;
			m_CaptureTexture = new RenderTexture(m_CaptureWidth / blurScale, m_CaptureHeight / blurScale, 16);
			m_CaptureTexture.Create();
			m_CaptureTexture.name = TEXTURE_NAME;
			m_CaptureTexture.hideFlags = HideFlags.HideAndDontSave;
			if ((bool)postMaterial)
			{
				postMaterial.SetTexture("_Background", m_CaptureTexture);
			}
		}
		Graphics.Blit(source, m_CaptureTexture);
		Graphics.Blit(source, destination);
	}

	private void OnDisable()
	{
		if (m_CaptureTexture == null)
		{
			UnityEngine.Object.Destroy(m_CaptureTexture);
			m_CaptureTexture = null;
		}
	}
}
