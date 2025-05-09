using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using UnityEngine;

public static class Assertion
{
	public class Exception : System.Exception
	{
		public Exception()
		{
		}

		public Exception(string message)
			: base(message)
		{
		}

		public Exception(string message, System.Exception innerException)
			: base(message, innerException)
		{
		}
	}

	public const string DefineEnabled = "ASSERTION_ENABLED";

	private static HashSet<string> g_ignoredAssertions;

	public static bool Enabled
	{
		get
		{
			return false;
		}
	}

	[Conditional("UNITY_EDITOR")]
	[Conditional("ASSERTION_ENABLED")]
	public static void Check(bool condition)
	{
		if (!condition)
		{
			FailImpl(string.Empty);
		}
	}

	[Conditional("UNITY_EDITOR")]
	[Conditional("ASSERTION_ENABLED")]
	public static void Check(bool condition, string format, params object[] args)
	{
		if (!condition)
		{
			FailImpl(format, args);
		}
	}

	[Conditional("UNITY_EDITOR")]
	[Conditional("ASSERTION_ENABLED")]
	public static void Fail()
	{
		FailImpl(string.Empty);
	}

	[Conditional("UNITY_EDITOR")]
	[Conditional("ASSERTION_ENABLED")]
	public static void Fail(string format, params object[] args)
	{
		FailImpl(format, args);
	}

	private static void FailImpl(string format, params object[] args)
	{
		StackTrace stackTrace = new StackTrace(true);
		StringBuilder stringBuilder = new StringBuilder();
		if (format.Length > 0)
		{
			stringBuilder.Append("Message: ");
			stringBuilder.Append(format);
			stringBuilder.Append("\n\n");
		}
		string text = stringBuilder.ToString();
		stringBuilder.Remove(0, stringBuilder.Length);
		int frameCount = stackTrace.FrameCount;
		for (int i = 2; i < frameCount; i++)
		{
			StackFrame frame = stackTrace.GetFrame(i);
			MethodBase method = frame.GetMethod();
			stringBuilder.AppendFormat("{0}.{1} (at {2}:{3})\n", method.DeclaringType, method.Name, frame.GetFileName(), frame.GetFileLineNumber());
		}
		string text2 = stringBuilder.ToString();
		Application.Quit();
	}
}
