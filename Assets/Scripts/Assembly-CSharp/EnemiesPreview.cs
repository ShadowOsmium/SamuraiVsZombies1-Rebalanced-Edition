using System.Collections.Generic;
using UnityEngine;

public class EnemiesPreview
{
	private class PreviewPrioritySorter : IComparer<string>
	{
		private WaveManager mWaveManager;

		public PreviewPrioritySorter(WaveManager wm)
		{
			mWaveManager = wm;
		}

		public int Compare(string id1, string id2)
		{
			int result = 0;
			int result2 = 0;
			int.TryParse(mWaveManager.GetEnemyData(id1)["previewPriority"], out result);
			int.TryParse(mWaveManager.GetEnemyData(id2)["previewPriority"], out result2);
			return result.CompareTo(result2);
		}
	}

	private const int kMaxEnemyTypes = 9;

	private List<Character> mChars = new List<Character>();

	private bool mNeedToForceIdle = true;

	public EnemiesPreview(BoxCollider spawnArea)
	{
		WaveManager waveManager = new WaveManager(Singleton<Profile>.instance.waveToBeat, null, 0f);
		List<string> allDifferentEnemies = waveManager.allDifferentEnemies;
		if (allDifferentEnemies.Count > 9)
		{
			Debug.Log("Too many enemies for preview, culling out from " + allDifferentEnemies.Count);
			allDifferentEnemies.Sort(new PreviewPrioritySorter(waveManager));
			allDifferentEnemies.RemoveRange(0, allDifferentEnemies.Count - 9);
		}
		Vector3[] spotsList = GetSpotsList(allDifferentEnemies.Count);
		SDFTreeNode sDFTreeNode = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/Enemies");
		WaveManager.ApplySpecialEvent(sDFTreeNode);
		for (int i = 0; i < allDifferentEnemies.Count; i++)
		{
			string text = allDifferentEnemies[i];
			if (sDFTreeNode.hasChild(text))
			{
				Vector3 pos = ((i >= spotsList.Length) ? new Vector3(0f, 0f, 0f) : spotsList[i]);
				Spawn(sDFTreeNode.to(text), pos);
			}
			else
			{
				Debug.Log("Error: Unable to find data for enemy ID: " + text);
			}
		}
	}

	public void Update()
	{
		foreach (Character mChar in mChars)
		{
			if (mNeedToForceIdle)
			{
				mChar.controller.StartWalkLeft();
				mChar.controller.Idle();
			}
			mChar.Update();
		}
		mNeedToForceIdle = false;
	}

	private void Spawn(SDFTreeNode data, Vector3 pos)
	{
		Character character = new Character();
		character.controlledObject = (GameObject)Object.Instantiate(Resources.Load(data["prefab"]));
		SoundThemePlayer component = character.controlledObject.GetComponent<SoundThemePlayer>();
		if (component != null)
		{
			component.autoPlayEvent = string.Empty;
		}
		character.controller.position = pos;
		character.controller.constraintLeft = 0f;
		character.controller.constraintRight = 10000f;
		character.controller.facing = FacingType.Left;
		if (data.hasAttribute("weaponMeleePrefab"))
		{
			character.meleeWeaponPrefab = Resources.Load(data["weaponMeleePrefab"]) as GameObject;
		}
		if (data.hasAttribute("weaponRangedPrefab"))
		{
			character.rangedWeaponPrefab = Resources.Load(data["weaponRangedPrefab"]) as GameObject;
		}
		character.maxHealth = 1f;
		character.health = 1f;
		if (Singleton<PlayModesManager>.instance.gameDirection == PlayModesManager.GameDirection.RightToLeft)
		{
			Vector3 localScale = character.controlledObject.transform.localScale;
			localScale.x = 0f - localScale.x;
			character.controlledObject.transform.localScale = localScale;
		}
		mChars.Add(character);
	}

	private Vector3 GetSpotInGrid(int x, int z)
	{
		return new Vector3(90f * (float)x + -50f, 25f, 70f * (float)z + 420f);
	}

	private Vector3[] GetSpotsList(int count)
	{
		switch (count)
		{
		case 1:
			return new Vector3[1] { GetSpotInGrid(1, 2) };
		case 2:
			return new Vector3[2]
			{
				GetSpotInGrid(0, 1),
				GetSpotInGrid(1, 3)
			};
		case 3:
			return new Vector3[3]
			{
				GetSpotInGrid(1, 4),
				GetSpotInGrid(1, 2),
				GetSpotInGrid(0, 1)
			};
		case 4:
			return new Vector3[4]
			{
				GetSpotInGrid(2, 4),
				GetSpotInGrid(0, 3),
				GetSpotInGrid(2, 2),
				GetSpotInGrid(1, 0)
			};
		case 5:
			return new Vector3[5]
			{
				GetSpotInGrid(0, 4),
				GetSpotInGrid(2, 4),
				GetSpotInGrid(0, 2),
				GetSpotInGrid(2, 1),
				GetSpotInGrid(1, 0)
			};
		case 6:
			return new Vector3[6]
			{
				GetSpotInGrid(0, 0),
				GetSpotInGrid(2, 0),
				GetSpotInGrid(0, 2),
				GetSpotInGrid(2, 2),
				GetSpotInGrid(0, 4),
				GetSpotInGrid(2, 4)
			};
		case 7:
			return new Vector3[7]
			{
				GetSpotInGrid(0, 0),
				GetSpotInGrid(2, 0),
				GetSpotInGrid(0, 2),
				GetSpotInGrid(2, 2),
				GetSpotInGrid(0, 4),
				GetSpotInGrid(2, 4),
				GetSpotInGrid(1, 4)
			};
		case 8:
			return new Vector3[8]
			{
				GetSpotInGrid(0, 0),
				GetSpotInGrid(2, 0),
				GetSpotInGrid(0, 2),
				GetSpotInGrid(2, 2),
				GetSpotInGrid(0, 4),
				GetSpotInGrid(2, 4),
				GetSpotInGrid(1, 4),
				GetSpotInGrid(1, 2)
			};
		default:
			return new Vector3[9]
			{
				GetSpotInGrid(0, 0),
				GetSpotInGrid(2, 0),
				GetSpotInGrid(0, 2),
				GetSpotInGrid(2, 2),
				GetSpotInGrid(0, 4),
				GetSpotInGrid(2, 4),
				GetSpotInGrid(1, 4),
				GetSpotInGrid(1, 2),
				GetSpotInGrid(1, 0)
			};
		}
	}
}
