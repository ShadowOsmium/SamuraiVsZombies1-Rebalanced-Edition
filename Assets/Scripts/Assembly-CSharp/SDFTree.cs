using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SDFTree
{
	private static int kNumSpacesForTabs = 4;

	private SDFTree()
	{
	}

	public static void Save(SDFTreeNode root, string filename)
	{
		if (File.Exists(filename))
		{
			if (Debug.isDebugBuild)
			{
				Debug.Log("*** Creating backup ***");
			}
			File.Copy(filename, Profile.prevSaveFilePath, true);
		}
		TextWriter textWriter = new StreamWriter(filename);
		try
		{
			SaveRecursively(textWriter, root, 0);
		}
		finally
		{
			textWriter.Close();
			if (Debug.isDebugBuild)
			{
				File.Copy(filename, Application.persistentDataPath + "/saveBefore.data", true);
			}
			FileEncryptAndDecrypt.EncryptFile(filename);
			if (Debug.isDebugBuild)
			{
				string contents = FileEncryptAndDecrypt.DecryptFile(filename, false);
				File.WriteAllText(Application.persistentDataPath + "/saveAfter.data", contents);
			}
		}
	}

	public static SDFTreeNode LoadFromResources(string filename)
	{
		TextAsset textAsset = (TextAsset)Resources.Load(filename, typeof(TextAsset));
		if (textAsset == null)
		{
			Debug.Log("ERROR: Unable to load the SDF file: '" + filename + "'");
			return null;
		}
		SDFTreeNode sDFTreeNode = LoadFromSingleString(textAsset.text);
		sDFTreeNode.ExpandLinks();
		return sDFTreeNode;
	}

	public static SDFTreeNode LoadFromBundle(AssetBundle bundle, string filename)
	{
		TextAsset textAsset = (TextAsset)bundle.LoadAsset(filename, typeof(TextAsset));
		if (textAsset == null)
		{
			Debug.Log("ERROR: Unable to load the SDF file from provided bundle: '" + filename + "'");
			return null;
		}
		SDFTreeNode sDFTreeNode = LoadFromSingleString(textAsset.text);
		sDFTreeNode.ExpandLinks();
		return sDFTreeNode;
	}

	public static SDFTreeNode LoadFromFile(string filename)
	{
		SDFTreeNode result = null;
		if (!File.Exists(filename))
		{
			return result;
		}
		try
		{
			string str = FileEncryptAndDecrypt.DecryptFile(filename, false);
			bool flag = RemoveNullCharacters(ref str);
			if (Debug.isDebugBuild)
			{
				Debug.Log("Save data is: " + flag);
				TextWriter textWriter = new StreamWriter(Application.persistentDataPath + "/save_load.data");
				textWriter.WriteLine(str);
				textWriter.Close();
			}
			result = LoadFromSingleString(str);
		}
		catch (Exception ex)
		{
			if (Debug.isDebugBuild)
			{
				Debug.Log(ex.Message);
			}
			return null;
		}
		result.ExpandLinks();
		return result;
	}

	private static void SaveRecursively(TextWriter tw, SDFTreeNode node, int level)
	{
		string text = new string('\t', level);
		foreach (KeyValuePair<string, string> attribute in node.attributes)
		{
			tw.WriteLine(text + attribute.Key + " = " + attribute.Value);
		}
		foreach (KeyValuePair<string, SDFTreeNode> child in node.childs)
		{
			tw.WriteLine(text + "[" + child.Key + "]");
			SaveRecursively(tw, child.Value, level + 1);
		}
	}

	public static SDFTreeNode LoadFromSingleString(string entireFile, bool needValidate = false)
	{
		char[] separator = new char[2] { '\r', '\n' };
		string[] source = entireFile.Split(separator, StringSplitOptions.RemoveEmptyEntries);
		SmarterStringIterator iter = new SmarterStringIterator(source);
		SDFTreeNode sDFTreeNode = new SDFTreeNode();
		LoadBlock(sDFTreeNode, 0, iter, needValidate, string.Empty);
		return sDFTreeNode;
	}

	private static void LoadBlock(SDFTreeNode node, int level, SmarterStringIterator iter, bool needValidate = false, string blockName = "")
	{
		while (iter.Current != null)
		{
			int num = FindLineLevel(iter.Current);
			string text = iter.Current.Trim();
			if (text.Length == 0 || text[0] == '/')
			{
				iter.MoveNext();
			}
			else
			{
				if (num < level)
				{
					break;
				}
				if (num > level)
				{
					Debug.Log("ERROR: the SDF indentation is malformed.");
					break;
				}
				iter.MoveNext();
				if (text[0] == '[')
				{
					string text2 = ExtractBlockTitle(text);
					if (text2 != string.Empty)
					{
						SDFTreeNode sDFTreeNode = new SDFTreeNode();
						LoadBlock(sDFTreeNode, level + 1, iter, needValidate, text2);
						if (needValidate && node.hasChild(text2))
						{
							sDFTreeNode.MergeFrom(node.to(text2));
						}
						node.SetChild(text2, sDFTreeNode);
					}
				}
				else if (text[0] == '{')
				{
					node.AddFileLink(ExtractFileLink(text));
				}
				else
				{
					KeyValuePair<string, string> keyValuePair = ExtractAttribute(text, needValidate, blockName);
					if (needValidate && node.hasAttribute(keyValuePair.Key))
					{
						keyValuePair = new KeyValuePair<string, string>(string.Empty, string.Empty);
					}
					if (keyValuePair.Key == string.Empty)
					{
						if (keyValuePair.Value == string.Empty)
						{
							Debug.Log("WARNING: SDF attribute malformed: " + text);
						}
						else
						{
							node[node.attributeCount] = keyValuePair.Value;
						}
					}
					else
					{
						node[keyValuePair.Key] = keyValuePair.Value;
					}
				}
			}
			if (iter.Current == null)
			{
				break;
			}
		}
	}

	private static int FindLineLevel(string line)
	{
		if (line == null || line.Length == 0)
		{
			return 0;
		}
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < line.Length; i++)
		{
			switch (line[i])
			{
			case '\t':
				num++;
				continue;
			case ' ':
				num2++;
				if (num2 == kNumSpacesForTabs)
				{
					num2 = 0;
					num++;
				}
				continue;
			}
			break;
		}
		return num;
	}

	private static string ExtractBlockTitle(string blockTitle)
	{
		if (blockTitle[blockTitle.Length - 1] != ']')
		{
			Debug.Log("ERROR: malformed SDF block header: " + blockTitle);
			return string.Empty;
		}
		return blockTitle.Substring(1, blockTitle.Length - 2);
	}

	private static string ExtractFileLink(string line)
	{
		if (line[line.Length - 1] != '}')
		{
			Debug.Log("ERROR: malformed SDF file link: " + line);
			return string.Empty;
		}
		return line.Substring(1, line.Length - 2);
	}

	private static KeyValuePair<string, string> ExtractAttribute(string attributeLine, bool needValidate = false, string blockName = "")
	{
		int num = attributeLine.IndexOf('=');
		if (Debug.isDebugBuild && num == 0)
		{
			Debug.Log("**** splitIndex ZERO!!!!! ****");
		}
		if (num == -1 || num == 0)
		{
			return new KeyValuePair<string, string>(string.Empty, attributeLine.Trim());
		}
		string[] array = new string[2]
		{
			attributeLine.Substring(0, num - 1).Trim(),
			attributeLine.Substring(num + 1).Trim()
		};
		if (needValidate)
		{
			try
			{
				if (!IsKeyValidate(array[0], array[1], blockName))
				{
					array[0] = string.Empty;
					array[1] = string.Empty;
				}
			}
			catch (Exception ex)
			{
				if (Debug.isDebugBuild)
				{
					Debug.Log("IsKeyValidate (" + array[0] + ", " + array[1] + "): " + ex.Message);
				}
				return new KeyValuePair<string, string>(string.Empty, string.Empty);
			}
		}
		return new KeyValuePair<string, string>(array[0], array[1]);
	}

	private static bool RemoveNullCharacters(ref string str)
	{
		string text = string.Empty;
		bool result = true;
		bool flag = false;
		for (int i = 0; i < str.Length; i++)
		{
			if (str[i] != 0)
			{
				text += str[i];
				if (flag && char.IsLetterOrDigit(str[i]))
				{
					result = false;
				}
			}
			else
			{
				flag = true;
			}
		}
		str = text;
		return result;
	}

	private static bool IsKeyValidate(string key, string value, string blockName)
	{
		string[,] array = new string[49, 2]
		{
			{ "neverShowRateMeAlertAgain", "b" },
			{ "timeStamp", "f" },
			{ "timeRemainingUntilNotification", "i" },
			{ "currentDailyRewardNumber", "i" },
			{ "hasReportedUser", "b" },
			{ "hasReportedGemPurchase", "b" },
			{ "hasWonPachinkoBefore", "b" },
			{ "coins", "i" },
			{ "purchasedCoins", "i" },
			{ "earnsDoubleCoins", "b" },
			{ "gems", "i" },
			{ "purchasedGems", "i" },
			{ "pachinkoBalls", "i" },
			{ "pachinkoBallsLaunched", "i" },
			{ "storeVisitsCount", "i" },
			{ "wavesSinceLastBonusWave", "i" },
			{ "bonusWaveToBeat", "i" },
			{ "waveToBeat", "i" },
			{ "profileName", "s" },
			{ "heroLevel", "i" },
			{ "heroID", "s" },
			{ "initialLeadershipLevel", "i" },
			{ "swordLevel", "i" },
			{ "swordID", "s" },
			{ "bowLevel", "i" },
			{ "bowID", "s" },
			{ "archerLevel", "i" },
			{ "baseLevel", "i" },
			{ "bellLevel", "i" },
			{ "fromTheShadows", "b" },
			{ "upgradedFarmers", "i" },
			{ "upgradedArchers", "i" },
			{ "UndeadKilledViaTroopTrample", "i" },
			{ "UndeadKilledViaKatanaSlash", "i" },
			{ "hasSummonedFarmer", "b" },
			{ "hasSummonedSwordWarrior", "b" },
			{ "hasSummonedSpearWarrior", "b" },
			{ "hasSummonedBowman", "b" },
			{ "hasSummonedPanzerSamurai", "b" },
			{ "hasSummonedPriest", "b" },
			{ "hasSummonedNobunaga", "b" },
			{ "hasSummonedSpearHorseman", "b" },
			{ "hasSummonedTakeda", "b" },
			{ "hasSummonedRifleman", "b" },
			{ "hasSummonedFrostie", "b" },
			{ "hasSummonedSwordsmith", "b" },
			{ "wavesWithZeroGateDamage", "i" },
			{ "waveRecycle", "i" },
			{ "fbp", "i" }
		};
		if (blockName.Length == 0)
		{
			for (int i = 0; i < array.GetLength(0); i++)
			{
				if (string.Compare(key, array[i, 0], true) == 0 && TryToConvert(value, array[i, 1]))
				{
					return true;
				}
			}
		}
		else if (string.Compare(blockName, "novelties", true) != 0 && string.Compare(blockName, "alreadyEarnedAchievements", true) != 0)
		{
			if (string.Compare(blockName, "helperLevels", true) == 0)
			{
				if (TryToConvert(value, "i"))
				{
					string[] allIDs = Singleton<HelpersDatabase>.instance.allIDs;
					foreach (string strB in allIDs)
					{
						if (string.Compare(key, strB, true) == 0)
						{
							return true;
						}
					}
				}
			}
			else if (string.Compare(blockName, "abilityLevels", true) == 0)
			{
				if (TryToConvert(value, "i"))
				{
					string[] allIDs2 = Singleton<AbilitiesDatabase>.instance.allIDs;
					foreach (string strB2 in allIDs2)
					{
						if (string.Compare(key, strB2, true) == 0)
						{
							return true;
						}
					}
				}
			}
			else if (string.Compare(blockName, "numPotions", true) == 0)
			{
				if (TryToConvert(value, "i"))
				{
					foreach (string allID in Singleton<PotionsDatabase>.instance.allIDs)
					{
						if (string.Compare(key, allID, true) == 0)
						{
							return true;
						}
					}
				}
			}
			else if (string.Compare(blockName, "numCharms", true) == 0)
			{
				if (TryToConvert(value, "i"))
				{
					foreach (string allID2 in Singleton<CharmsDatabase>.instance.allIDs)
					{
						if (string.Compare(key, allID2, true) == 0)
						{
							return true;
						}
					}
				}
			}
			else if (string.Compare(blockName, "chapterUnlock", true) != 0)
			{
				if (string.Compare(blockName, "waves", true) == 0)
				{
					if (key[0] == 'w')
					{
						string text = key.Substring(1);
						for (int l = 0; l < text.Length; l++)
						{
							if (!char.IsDigit(text[l]))
							{
								return false;
							}
						}
						if (TryToConvert(value, "i"))
						{
							return true;
						}
					}
				}
				else if (string.Compare(blockName, "tutorials", true) == 0)
				{
					string[] array2 = new string[17]
					{
						"TutorialWave2_leadership", "TutorialWave2_summonAlly", "TutorialWave1_protectGate", "TutorialWave1_showMovements", "TutorialWave1_autoAttack", "TutorialWave1_ability", "TutorialBonusWave1_welcome", "TutorialBonusWave1_allies", "TutorialBonusWave1_charm", "TutorialBonusWave1_unlock",
						"StoreDealPacks_dealpacks", "Store_intro", "PachinkoTutorial_lever", "PachinkoTutorial_spin", "PachinkoTutorial_multiplier", "reactives_flyingEnemies", "BoosterPacksTutorial_freePack"
					};
					if (TryToConvert(value, "b"))
					{
						for (int m = 0; m < array2.Length; m++)
						{
							if (string.Compare(key, array2[m], true) == 0)
							{
								return true;
							}
						}
					}
				}
				else if (string.Compare(blockName, "achievementsDisplayed", true) == 0)
				{
					if (key.Length == 3 && char.IsDigit(key[0]) && char.IsDigit(key[1]) && char.IsDigit(key[2]))
					{
						return true;
					}
				}
				else if (string.Compare(blockName, "stats_enemiesKilled", true) == 0 && TryToConvert(value, "i"))
				{
					SDFTreeNode sDFTreeNode = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/Enemies");
					foreach (KeyValuePair<string, SDFTreeNode> child in sDFTreeNode.childs)
					{
						if (string.Compare(child.Key, key, true) == 0)
						{
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	private static bool TryToConvert(string str, string type)
	{
		switch (type)
		{
		case "i":
		{
			int result2;
			if (int.TryParse(str, out result2))
			{
				return true;
			}
			break;
		}
		case "b":
		{
			bool result3;
			if (bool.TryParse(str, out result3))
			{
				return true;
			}
			break;
		}
		case "f":
		{
			float result;
			if (float.TryParse(str, out result))
			{
				return true;
			}
			break;
		}
		case "s":
			return true;
		}
		return false;
	}
}
