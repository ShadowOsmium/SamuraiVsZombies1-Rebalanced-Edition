using UnityEngine;

public class SUILayoutConv
{
	public static string GetFormattedText(string str)
	{
		str = str.Replace("&#10;", "\r");
		str = str.Replace("\\r", "\r");
		str = str.Replace("&#13;", "\n");
		str = str.Replace("\\n", "\n");
		return str;
	}

	public static bool GetBool(string str)
	{
		str = str.ToLower();
		int result;
		switch (str)
		{
		default:
			result = ((str == "on") ? 1 : 0);
			break;
		case "true":
		case "yes":
		case "1":
			result = 1;
			break;
		}
		return (byte)result != 0;
	}

	public static Vector2 GetVector2(string strList)
	{
		float[] screenRelativeFloatList = GetScreenRelativeFloatList(strList);
		if (screenRelativeFloatList.Length == 2)
		{
			return new Vector2(screenRelativeFloatList[0], screenRelativeFloatList[1]);
		}
		Debug.Log("SUILayout ERROR: Malformed Vector2: " + strList);
		return Vector2.zero;
	}

	public static SUILayout.NormalRange GetNormalRange(string strList)
	{
		float[] screenRelativeFloatList = GetScreenRelativeFloatList(strList);
		if (screenRelativeFloatList.Length == 2)
		{
			SUILayout.NormalRange result = new SUILayout.NormalRange(screenRelativeFloatList[0], screenRelativeFloatList[1]);
			if (result.min <= result.max)
			{
				return result;
			}
		}
		Debug.Log("SUILayout ERROR: Malformed NormalRange: " + strList);
		return default(SUILayout.NormalRange);
	}

	public static Rect GetRect(string strList)
	{
		float[] screenRelativeFloatList = GetScreenRelativeFloatList(strList);
		if (screenRelativeFloatList.Length == 4)
		{
			return new Rect(screenRelativeFloatList[0], screenRelativeFloatList[1], screenRelativeFloatList[2], screenRelativeFloatList[3]);
		}
		Debug.Log("SUILayout ERROR: Malformed Rect: " + strList);
		return default(Rect);
	}

	public static Color GetColor(string strList)
	{
		switch (strList.Trim().ToLower())
		{
		case "black":
			return Color.black;
		case "blue":
			return Color.blue;
		case "cyan":
			return Color.cyan;
		case "clear":
			return Color.clear;
		case "gray":
			return Color.gray;
		case "green":
			return Color.green;
		case "grey":
			return Color.grey;
		case "magenta":
			return Color.magenta;
		case "red":
			return Color.red;
		case "white":
			return Color.white;
		case "yellow":
			return Color.yellow;
		default:
		{
			float[] floatList = GetFloatList(strList);
			if (floatList.Length == 3)
			{
				return new Color(floatList[0], floatList[1], floatList[2]);
			}
			if (floatList.Length == 4)
			{
				return new Color(floatList[0], floatList[1], floatList[2], floatList[3]);
			}
			Debug.Log("SUILayout ERROR: Malformed Color: " + strList);
			return default(Color);
		}
		}
	}

	public static TextAnchor GetAnchor(string str)
	{
		switch (str.ToLower())
		{
		case "lowercenter":
			return TextAnchor.LowerCenter;
		case "lowerleft":
			return TextAnchor.LowerLeft;
		case "lowerright":
			return TextAnchor.LowerRight;
		case "middlecenter":
			return TextAnchor.MiddleCenter;
		case "middleleft":
			return TextAnchor.MiddleLeft;
		case "middleright":
			return TextAnchor.MiddleRight;
		case "uppercenter":
			return TextAnchor.UpperCenter;
		case "upperleft":
			return TextAnchor.UpperLeft;
		case "upperright":
			return TextAnchor.UpperRight;
		default:
			Debug.Log("SUILayout ERROR: Malformed Anchor: " + str);
			return TextAnchor.LowerCenter;
		}
	}

	public static TextAlignment GetAlignment(string str)
	{
		switch (str.ToLower())
		{
		case "center":
			return TextAlignment.Center;
		case "right":
			return TextAlignment.Right;
		default:
			return TextAlignment.Left;
		}
	}

	public static Ease.Function GetEaseFunc(string easeFuncStr)
	{
		switch (easeFuncStr.ToLower())
		{
		case "sinein":
			return Ease.SineIn;
		case "sineout":
			return Ease.SineOut;
		case "sineinout":
			return Ease.SineInOut;
		case "quadin":
			return Ease.QuadIn;
		case "quadout":
			return Ease.QuadOut;
		case "quadinout":
			return Ease.QuadInOut;
		case "cubicin":
			return Ease.CubicIn;
		case "cubicout":
			return Ease.CubicOut;
		case "cubicinout":
			return Ease.CubicInOut;
		case "quartin":
			return Ease.QuartIn;
		case "quartout":
			return Ease.QuartOut;
		case "quartinout":
			return Ease.QuartInOut;
		case "quintin":
			return Ease.QuintIn;
		case "quintout":
			return Ease.QuintOut;
		case "quintinout":
			return Ease.QuintInOut;
		case "expoin":
			return Ease.ExpoIn;
		case "expoout":
			return Ease.ExpoOut;
		case "expoinout":
			return Ease.ExpoInOut;
		case "circin":
			return Ease.CircIn;
		case "circout":
			return Ease.CircOut;
		case "circinout":
			return Ease.CircInOut;
		case "backin":
			return Ease.BackIn;
		case "backout":
			return Ease.BackOut;
		case "backinout":
			return Ease.BackInOut;
		case "bouncein":
			return Ease.BounceIn;
		case "bounceout":
			return Ease.BounceOut;
		case "bounceinout":
			return Ease.BounceInOut;
		default:
			return Ease.Linear;
		}
	}

	public static int[] GetIntegerList(string strList)
	{
		string[] array = strList.Split(',');
		int[] array2 = new int[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = int.Parse(array[i]);
		}
		return array2;
	}

	public static float[] GetFloatList(string strList)
	{
		string[] array = strList.Split(',');
		float[] array2 = new float[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = float.Parse(array[i]);
		}
		return array2;
	}

	public static float[] GetScreenRelativeFloatList(string strList)
	{
		string[] array = strList.Split(',');
		float[] array2 = new float[array.Length];
		bool flag = true;
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = GetFloatScreenRelative(array[i], (!flag) ? SUIScreen.height : SUIScreen.width);
			flag = !flag;
		}
		return array2;
	}

	public static float GetFloatScreenRelative(string str, float eValue)
	{
		float num = 0f;
		bool flag = false;
		str = str.Trim();
		string text = str;
		while (str.Length > 0)
		{
			if (str[0] == 'E' || str[0] == 'e')
			{
				num = eValue;
				str = str.Substring(1).Trim();
			}
			else if (str[0] == 'C' || str[0] == 'c')
			{
				num = eValue / 2f;
				str = str.Substring(1).Trim();
			}
			else if (str[0] == '*')
			{
				flag = true;
				str = str.Substring(1).Trim();
			}
			else if (str[0] == '+')
			{
				str = str.Substring(1).Trim();
			}
			else if (str[0] == '-' || str[0] == '.' || (str[0] >= '0' && str[0] <= '9'))
			{
				float num2 = float.Parse(str);
				num = ((!flag) ? (num + num2) : (num * num2));
				str = string.Empty;
			}
			else
			{
				Debug.Log("ERROR: Malformed relative coordinate: " + text);
				str = string.Empty;
			}
		}
		return num;
	}
}
