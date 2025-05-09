using System.Diagnostics;
using UnityEngine;

public class LoggerSingleton<T> where T : new()
{
	public const string DefineEnabled = "LOGGING_ENABLED";

	public const string DefineLevelDebug = "LOGGING_LEVEL_DEBUG";

	public const string DefineLevelInfo = "LOGGING_LEVEL_INFO";

	public const string DefineLevelWarn = "LOGGING_LEVEL_WARN";

	public const string DefineLevelError = "LOGGING_LEVEL_ERROR";

	public const string DefineLevelCritical = "LOGGING_LEVEL_CRITICAL";

	public const string DefineLevelAtLeastOne = "LOGGING_LEVEL_AT_LEAST_ONE";

	private static Logging.ILogger g_instance;

	public static bool Propagate
	{
		get
		{
			return Instance.Propagate;
		}
		set
		{
			Instance.Propagate = value;
		}
	}

	public static Logging.ILogger Instance
	{
		get
		{
			if (g_instance != null)
			{
				return g_instance;
			}
			new T();
			if (g_instance != null)
			{
				return g_instance;
			}
			SetLoggerName("App." + typeof(T).Name);
			return g_instance;
		}
	}

	public static int StaticLevel
	{
		get
		{
			return -1;
		}
	}

	[Conditional("LOGGING_LEVEL_AT_LEAST_ONE")]
	[Conditional("UNITY_EDITOR")]
	public static void Log(int level, string format, params object[] args)
	{
		if (IsEnabledForDefine(level))
		{
			Instance.Log(level, format, args);
		}
	}

	[Conditional("LOGGING_LEVEL_DEBUG")]
	[Conditional("UNITY_EDITOR")]
	public static void Debug(string format, params object[] args)
	{
		Instance.Debug(format, args);
	}

	[Conditional("UNITY_EDITOR")]
	[Conditional("LOGGING_LEVEL_INFO")]
	public static void Info(string format, params object[] args)
	{
		Instance.Info(format, args);
	}

	[Conditional("LOGGING_LEVEL_WARN")]
	[Conditional("UNITY_EDITOR")]
	public static void Warn(string format, params object[] args)
	{
		Instance.Warn(format, args);
	}

	[Conditional("UNITY_EDITOR")]
	[Conditional("LOGGING_LEVEL_ERROR")]
	public static void Error(string format, params object[] args)
	{
		Instance.Error(format, args);
	}

	[Conditional("LOGGING_LEVEL_CRITICAL")]
	[Conditional("UNITY_EDITOR")]
	public static void Critical(string format, params object[] args)
	{
		Instance.Critical(format, args);
	}

	[Conditional("LOGGING_LEVEL_AT_LEAST_ONE")]
	[Conditional("UNITY_EDITOR")]
	public static void Log(Object context, int level, string format, params object[] args)
	{
		if (IsEnabledForDefine(level))
		{
			Instance.Log(context, level, format, args);
		}
	}

	[Conditional("LOGGING_LEVEL_DEBUG")]
	[Conditional("UNITY_EDITOR")]
	public static void Debug(Object context, string format, params object[] args)
	{
		Instance.Debug(context, format, args);
	}

	[Conditional("LOGGING_LEVEL_INFO")]
	[Conditional("UNITY_EDITOR")]
	public static void Info(Object context, string format, params object[] args)
	{
		Instance.Info(context, format, args);
	}

	[Conditional("UNITY_EDITOR")]
	[Conditional("LOGGING_LEVEL_WARN")]
	public static void Warn(Object context, string format, params object[] args)
	{
		Instance.Warn(context, format, args);
	}

	[Conditional("LOGGING_LEVEL_ERROR")]
	[Conditional("UNITY_EDITOR")]
	public static void Error(Object context, string format, params object[] args)
	{
		Instance.Error(context, format, args);
	}

	[Conditional("UNITY_EDITOR")]
	[Conditional("LOGGING_LEVEL_CRITICAL")]
	public static void Critical(Object context, string format, params object[] args)
	{
		Instance.Critical(context, format, args);
	}

	public static bool IsEnabledFor(int level)
	{
		if (!IsEnabledForDefine(level))
		{
			return false;
		}
		return Instance.IsEnabledFor(level);
	}

	public static void SetLevel(int level)
	{
		Instance.SetLevel(level);
	}

	public static int GetLevel()
	{
		return Instance.GetLevel();
	}

	public static int GetEffectiveLevel()
	{
		return Instance.GetEffectiveLevel();
	}

	public static void AddFilter(Logging.IFilter filter)
	{
		Instance.AddFilter(filter);
	}

	public static void RemoveFilter(Logging.IFilter filter)
	{
		Instance.RemoveFilter(filter);
	}

	public static bool Filter(ref Logging.LogRecord record)
	{
		return Instance.Filter(ref record);
	}

	public static int GetFilterCount()
	{
		return Instance.GetFilterCount();
	}

	public static Logging.IFilter GetFilter(int i)
	{
		return Instance.GetFilter(i);
	}

	public static void AddHandler(Logging.IHandler handler)
	{
		Instance.AddHandler(handler);
	}

	public static void RemoveHandler(Logging.IHandler handler)
	{
		Instance.RemoveHandler(handler);
	}

	public static int GetHandlerCount()
	{
		return Instance.GetHandlerCount();
	}

	public static Logging.IHandler GetHandler(int i)
	{
		return Instance.GetHandler(i);
	}

	private static bool IsEnabledForDefine(int level)
	{
		return false;
	}

	protected static void SetLoggerName(string name)
	{
		g_instance = Logging.GetLogger(name);
		Logging.RegisterDestructor(delegate
		{
			g_instance = null;
		});
	}
}
