using UnityEngine;

public class TimeControllerBehavior : MonoBehaviour
{
	public float timeRate = 1f;

	private void OnGUI()
	{
		Time.timeScale = GUI.HorizontalSlider(new Rect(50f, 25f, 100f, 30f), Time.timeScale, 0f, 5f);
		GUI.Label(new Rect(5f, 20f, 50f, 20f), "Time Scale");
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
