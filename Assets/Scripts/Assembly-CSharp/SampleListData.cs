using System;
using UnityEngine;

[Serializable]
public class SampleListData
{
	public string name = string.Empty;

	public int cost = 1;

	public float boost;

	public GameObject Model;

	public string[] siblingNames = new string[0];

	public int[] countArray = new int[0];

	public SampleSubListData[] skins;

	public AudioClip[] audioData = new AudioClip[0];

	public SampleData sample;
}
