using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

public static class Logging
{
	public struct LogRecord
	{
		public string Name;

		public int Level;

		public string Format;

		public object[] Args;

		public UnityEngine.Object Context;

		public Dictionary<string, object> Extra;

		public void SetExtraValue(string name, object val)
		{
			if (Extra == null)
			{
				Extra = new Dictionary<string, object>();
			}
			Extra[name] = val;
		}
	}

	public interface ILogger
	{
		bool Propagate { get; set; }

		ILogger Parent { get; set; }

		string Name { get; }

		void Log(int level, string format, params object[] args);

		void Debug(string format, params object[] args);

		void Info(string format, params object[] args);

		void Warn(string format, params object[] args);

		void Error(string format, params object[] args);

		void Critical(string format, params object[] args);

		void Log(UnityEngine.Object context, int level, string format, params object[] args);

		void Debug(UnityEngine.Object context, string format, params object[] args);

		void Info(UnityEngine.Object context, string format, params object[] args);

		void Warn(UnityEngine.Object context, string format, params object[] args);

		void Error(UnityEngine.Object context, string format, params object[] args);

		void Critical(UnityEngine.Object context, string format, params object[] args);

		bool IsEnabledFor(int level);

		void SetLevel(int level);

		int GetLevel();

		int GetEffectiveLevel();

		void AddFilter(IFilter filter);

		void RemoveFilter(IFilter filter);

		bool Filter(ref LogRecord record);

		int GetFilterCount();

		IFilter GetFilter(int i);

		void AddHandler(IHandler handler);

		void RemoveHandler(IHandler handler);

		int GetHandlerCount();

		IHandler GetHandler(int i);

		void CallHandlers(ref LogRecord record);
	}

	public interface IFilter
	{
		bool Filter(ref LogRecord record);
	}

	public interface IHandler
	{
		void Handle(ref LogRecord record);

		void Emit(ref LogRecord record);

		void SetLevel(int level);

		int GetLevel();

		void AddFilter(IFilter filter);

		void RemoveFilter(IFilter filter);

		int GetFilterCount();

		IFilter GetFilter(int i);

		void SetFormatter(IFormatter formatter);

		void Close();
	}

	public interface IFormatter
	{
		string Format(ref LogRecord record);
	}

	public sealed class Logger : ILogger
	{
		private string m_name;

		private int m_level;

		private Filterer m_filters;

		private List<IHandler> m_handlers;

		private ILogger m_parent;

		private bool m_propagate;

		public bool Propagate
		{
			get
			{
				return m_propagate;
			}
			set
			{
				m_propagate = value;
			}
		}

		public ILogger Parent
		{
			get
			{
				return m_parent;
			}
			set
			{
				m_parent = value;
			}
		}

		public string Name
		{
			get
			{
				return m_name;
			}
		}

		internal Logger(string name)
		{
			m_name = name ?? string.Empty;
			m_level = 0;
			m_filters = new Filterer();
			m_handlers = new List<IHandler>();
			m_propagate = true;
		}

		public void Log(int level, string format, params object[] args)
		{
			if (IsEnabledFor(level))
			{
				LogImpl(null, level, format, args);
			}
		}

		public void Debug(string format, params object[] args)
		{
			if (IsEnabledFor(10))
			{
				LogImpl(null, 10, format, args);
			}
		}

		public void Info(string format, params object[] args)
		{
			if (IsEnabledFor(20))
			{
				LogImpl(null, 20, format, args);
			}
		}

		public void Warn(string format, params object[] args)
		{
			if (IsEnabledFor(30))
			{
				LogImpl(null, 30, format, args);
			}
		}

		public void Error(string format, params object[] args)
		{
			if (IsEnabledFor(40))
			{
				LogImpl(null, 40, format, args);
			}
		}

		public void Critical(string format, params object[] args)
		{
			if (IsEnabledFor(50))
			{
				LogImpl(null, 50, format, args);
			}
		}

		public void Log(UnityEngine.Object context, int level, string format, params object[] args)
		{
			if (IsEnabledFor(level))
			{
				LogImpl(context, level, format, args);
			}
		}

		public void Debug(UnityEngine.Object context, string format, params object[] args)
		{
			if (IsEnabledFor(10))
			{
				LogImpl(context, 10, format, args);
			}
		}

		public void Info(UnityEngine.Object context, string format, params object[] args)
		{
			if (IsEnabledFor(20))
			{
				LogImpl(context, 20, format, args);
			}
		}

		public void Warn(UnityEngine.Object context, string format, params object[] args)
		{
			if (IsEnabledFor(30))
			{
				LogImpl(context, 30, format, args);
			}
		}

		public void Error(UnityEngine.Object context, string format, params object[] args)
		{
			if (IsEnabledFor(40))
			{
				LogImpl(context, 40, format, args);
			}
		}

		public void Critical(UnityEngine.Object context, string format, params object[] args)
		{
			if (IsEnabledFor(50))
			{
				LogImpl(context, 50, format, args);
			}
		}

		public bool IsEnabledFor(int level)
		{
			if (g_disable >= level)
			{
				return false;
			}
			return level >= GetEffectiveLevel();
		}

		public void SetLevel(int level)
		{
			m_level = level;
		}

		public int GetLevel()
		{
			return m_level;
		}

		public int GetEffectiveLevel()
		{
			if (m_level != 0)
			{
				return m_level;
			}
			return (m_parent != null) ? m_parent.GetEffectiveLevel() : 0;
		}

		public void AddFilter(IFilter filter)
		{
			m_filters.AddFilter(filter);
		}

		public void RemoveFilter(IFilter filter)
		{
			m_filters.RemoveFilter(filter);
		}

		public bool Filter(ref LogRecord record)
		{
			return m_filters.Filter(ref record);
		}

		public int GetFilterCount()
		{
			return m_filters.Count;
		}

		public IFilter GetFilter(int i)
		{
			return m_filters.GetFilter(i);
		}

		public void AddHandler(IHandler handler)
		{
			if (handler != null && m_handlers.IndexOf(handler) == -1)
			{
				m_handlers.Add(handler);
			}
		}

		public void RemoveHandler(IHandler handler)
		{
			m_handlers.Remove(handler);
		}

		public int GetHandlerCount()
		{
			return m_handlers.Count;
		}

		public IHandler GetHandler(int i)
		{
			return m_handlers[i];
		}

		public void CallHandlers(ref LogRecord record)
		{
			int count = m_handlers.Count;
			for (int i = 0; i < count; i++)
			{
				m_handlers[i].Handle(ref record);
			}
			if (m_propagate && m_parent != null)
			{
				m_parent.CallHandlers(ref record);
			}
		}

		private void LogImpl(UnityEngine.Object context, int level, string format, params object[] args)
		{
			LogRecord logRecord = default(LogRecord);
			logRecord.Name = m_name;
			logRecord.Level = level;
			logRecord.Format = format;
			logRecord.Args = args;
			logRecord.Context = context;
			LogRecord record = logRecord;
			if (Filter(ref record))
			{
				CallHandlers(ref record);
			}
		}
	}

	public sealed class Filterer
	{
		private List<IFilter> m_filters;

		public int Count
		{
			get
			{
				return m_filters.Count;
			}
		}

		public Filterer()
		{
			m_filters = new List<IFilter>();
		}

		public void AddFilter(IFilter filter)
		{
			if (filter != null && m_filters.IndexOf(filter) == -1)
			{
				m_filters.Add(filter);
			}
		}

		public void RemoveFilter(IFilter filter)
		{
			m_filters.Remove(filter);
		}

		public bool Filter(ref LogRecord record)
		{
			int count = m_filters.Count;
			for (int i = 0; i < count; i++)
			{
				if (!m_filters[i].Filter(ref record))
				{
					return false;
				}
			}
			return true;
		}

		public IFilter GetFilter(int i)
		{
			return m_filters[i];
		}
	}

	public abstract class HandlerBase : IHandler
	{
		private int m_level;

		private Filterer m_filters;

		private IFormatter m_formatter;

		public HandlerBase()
		{
			m_level = 0;
			m_filters = new Filterer();
			m_formatter = null;
		}

		public void Handle(ref LogRecord record)
		{
			if (record.Level >= m_level && Filter(ref record))
			{
				Emit(ref record);
			}
		}

		public abstract void Emit(ref LogRecord record);

		public virtual void Close()
		{
		}

		public void SetLevel(int level)
		{
			m_level = level;
		}

		public int GetLevel()
		{
			return m_level;
		}

		public void SetFormatter(IFormatter formatter)
		{
			m_formatter = formatter;
		}

		public void AddFilter(IFilter filter)
		{
			m_filters.AddFilter(filter);
		}

		public void RemoveFilter(IFilter filter)
		{
			m_filters.RemoveFilter(filter);
		}

		public bool Filter(ref LogRecord record)
		{
			return m_filters.Filter(ref record);
		}

		public int GetFilterCount()
		{
			return m_filters.Count;
		}

		public IFilter GetFilter(int i)
		{
			return m_filters.GetFilter(i);
		}

		public string Format(ref LogRecord record)
		{
			IFormatter formatter = m_formatter ?? g_defaultFormatter;
			return formatter.Format(ref record);
		}
	}

	public sealed class DebugLogHandler : HandlerBase
	{
		public override void Emit(ref LogRecord record)
		{
			string text = Format(ref record);
			RuntimePlatform platform = Application.platform;
			if (platform == RuntimePlatform.IPhonePlayer)
			{
				Console.WriteLine(text);
				return;
			}
			try
			{
				UnityLogLogger.Lock();
				UnityEngine.Object context = record.Context;
				if (context != null)
				{
					if (record.Level < 30)
					{
						UnityEngine.Debug.Log(text, context);
					}
					else if (record.Level < 40)
					{
						UnityEngine.Debug.LogWarning(text, context);
					}
					else
					{
						UnityEngine.Debug.LogError(text, context);
					}
				}
				else if (record.Level < 30)
				{
					UnityEngine.Debug.Log(text);
				}
				else if (record.Level < 40)
				{
					UnityEngine.Debug.LogWarning(text);
				}
				else
				{
					UnityEngine.Debug.LogError(text);
				}
			}
			finally
			{
				UnityLogLogger.Unlock();
			}
		}
	}

	public sealed class FileHandler : HandlerBase
	{
		private StreamWriter m_stream;

		public bool AutoFlush
		{
			get
			{
				return m_stream.AutoFlush;
			}
			set
			{
				m_stream.AutoFlush = value;
			}
		}

		public FileHandler(string path, FileMode mode)
		{
			FileStream fileStream = null;
			try
			{
				fileStream = File.Open(path, mode, FileAccess.Write, FileShare.Read);
				m_stream = new StreamWriter(fileStream, new UTF8Encoding(false));
				m_stream.AutoFlush = true;
			}
			catch (Exception)
			{
				if (m_stream != null)
				{
					m_stream.Close();
				}
				else if (fileStream != null)
				{
					fileStream.Close();
				}
				throw;
			}
		}

		public override void Emit(ref LogRecord record)
		{
			string value = Format(ref record);
			m_stream.Write(value);
			m_stream.WriteLine();
		}

		public override void Close()
		{
			m_stream.Close();
		}
	}

	public sealed class SimpleFormatter : IFormatter
	{
		private string m_messageFormat;

		[ThreadStatic]
		private static StringBuilder g_stringBuilder;

		[ThreadStatic]
		private static object[] g_args;

		public string MessageFormat
		{
			get
			{
				return m_messageFormat;
			}
			set
			{
				m_messageFormat = value;
			}
		}

		public SimpleFormatter()
		{
			m_messageFormat = "{1}:{2}:{0}";
		}

		public string Format(ref LogRecord record)
		{
			string arg = FormatMessage(record.Format, record.Args);
			string levelName = GetLevelName(record.Level);
			string arg2 = ((record.Name.Length <= 0) ? "root" : record.Name);
			return string.Format(m_messageFormat, arg, levelName, arg2);
		}

		public static string FormatMessage(string format, params object[] args)
		{
			if (!Array.Exists(args, (object obj) => obj is byte[]))
			{
				return string.Format(format, args);
			}
			int num = args.Length;
			try
			{
				if (g_args == null || g_args.Length < num)
				{
					g_args = new object[num * 2];
				}
				for (int i = 0; i < num; i++)
				{
					object obj2 = args[i];
					byte[] array = obj2 as byte[];
					if (array == null)
					{
						g_args[i] = obj2;
						continue;
					}
					if (g_stringBuilder == null)
					{
						g_stringBuilder = new StringBuilder();
					}
					else
					{
						g_stringBuilder.Remove(0, g_stringBuilder.Length);
					}
					HexPPrinter.ByteArrayToString(g_stringBuilder, array);
					g_args[i] = g_stringBuilder.ToString();
				}
				return string.Format(format, g_args);
			}
			finally
			{
				Array.Clear(g_args, 0, num);
			}
		}
	}

	public sealed class NameFilter : IFilter
	{
		private string[] m_filters;

		public NameFilter(params string[] filters)
		{
			m_filters = filters;
		}

		public bool Filter(ref LogRecord record)
		{
			string name = record.Name;
			int num = m_filters.Length;
			for (int i = 0; i < num; i++)
			{
				string text = m_filters[i];
				if (name.StartsWith(text))
				{
					int length = text.Length;
					if (name.Length == length)
					{
						return true;
					}
					if (length == 0 || name[length] == '.')
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	public class SerializationException : Exception
	{
		public SerializationException()
		{
		}

		public SerializationException(string message)
			: base(message)
		{
		}

		public SerializationException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}

	public class RuntimeConfig
	{
		public struct Logger
		{
			public string Name;

			public int Level;

			public bool Propagate;
		}

		[XmlRoot("Logging")]
		public struct XmlData
		{
			[XmlAttribute]
			public int Version;

			[XmlArray("Loggers")]
			[XmlArrayItem("Logger")]
			public XmlLogger[] Loggers;

			public int Disabled;

			public bool DebugLogHandlerEnabled;

			public bool FileHandlerEnabled;

			public bool CaptureUnityLog;
		}

		public struct XmlLogger
		{
			public string Name;

			public int Level;

			public bool Propagate;
		}

		private List<Logger> m_loggers;

		private int m_disabled;

		private bool m_debugLogHandlerEnabled;

		private bool m_fileHandlerEnabled;

		private bool m_captureUnityLog;

		public int LoggerCount
		{
			get
			{
				return m_loggers.Count;
			}
		}

		public int Disabled
		{
			get
			{
				return m_disabled;
			}
			set
			{
				m_disabled = value;
			}
		}

		public bool CaptureUnityLog
		{
			get
			{
				return m_captureUnityLog;
			}
			set
			{
				m_captureUnityLog = value;
			}
		}

		public bool DebugLogHandlerEnabled
		{
			get
			{
				return m_debugLogHandlerEnabled;
			}
			set
			{
				m_debugLogHandlerEnabled = value;
			}
		}

		public bool FileHandlerEnabled
		{
			get
			{
				return m_fileHandlerEnabled;
			}
			set
			{
				m_fileHandlerEnabled = value;
			}
		}

		public RuntimeConfig()
		{
			m_loggers = new List<Logger>();
			Reset();
		}

		public void Reset()
		{
			m_loggers.Clear();
			m_disabled = 0;
			m_captureUnityLog = false;
			m_debugLogHandlerEnabled = true;
			m_fileHandlerEnabled = true;
		}

		public Logger GetLogger(int i)
		{
			return m_loggers[i];
		}

		public void AddLogger(Logger logger)
		{
			m_loggers.Add(logger);
		}

		public void Remove(int i)
		{
			m_loggers.RemoveAt(i);
		}

		public void Load(Stream stream)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(XmlData));
			XmlData data = (XmlData)xmlSerializer.Deserialize(stream);
			Validate(ref data);
			List<Logger> list = new List<Logger>();
			XmlLogger[] loggers = data.Loggers;
			if (loggers != null)
			{
				int num = loggers.Length;
				for (int i = 0; i < num; i++)
				{
					XmlLogger xmlLogger = loggers[i];
					Logger logger = default(Logger);
					logger.Name = xmlLogger.Name;
					logger.Level = xmlLogger.Level;
					logger.Propagate = xmlLogger.Propagate;
					Logger item = logger;
					list.Add(item);
				}
			}
			m_loggers = list;
			m_disabled = data.Disabled;
			m_debugLogHandlerEnabled = data.DebugLogHandlerEnabled;
			m_fileHandlerEnabled = data.FileHandlerEnabled;
			m_captureUnityLog = data.CaptureUnityLog;
		}

		public void Save(Stream stream)
		{
			XmlData data = default(XmlData);
			data.Version = 1;
			int loggerCount = LoggerCount;
			data.Loggers = new XmlLogger[loggerCount];
			for (int i = 0; i < loggerCount; i++)
			{
				Logger logger = GetLogger(i);
				data.Loggers[i] = new XmlLogger
				{
					Name = logger.Name,
					Level = logger.Level,
					Propagate = logger.Propagate
				};
			}
			data.Disabled = Disabled;
			data.DebugLogHandlerEnabled = DebugLogHandlerEnabled;
			data.FileHandlerEnabled = FileHandlerEnabled;
			data.CaptureUnityLog = CaptureUnityLog;
			Validate(ref data);
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.Encoding = Encoding.UTF8;
			xmlWriterSettings.Indent = true;
			using (XmlWriter xmlWriter = XmlWriter.Create(stream, xmlWriterSettings))
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(XmlData));
				xmlSerializer.Serialize(xmlWriter, data);
			}
		}

		private void Validate(ref XmlData data)
		{
			if (data.Version != 1)
			{
				throw new SerializationException(string.Format("Unsupported config version {0}", data.Version));
			}
		}
	}

	private sealed class UnityLogLogger : LoggerSingleton<UnityLogLogger>
	{
		private static int m_lockCount;

		public UnityLogLogger()
		{
			LoggerSingleton<UnityLogLogger>.SetLoggerName("UnityLog");
			m_lockCount = 0;
		}

		public static void RegisterLogCallback()
		{
			Application.RegisterLogCallback(LogCallback);
		}

		public static void Lock()
		{
			m_lockCount++;
		}

		public static void Unlock()
		{
			if (m_lockCount > 0)
			{
				m_lockCount--;
			}
		}

		private static void LogCallback(string condition, string stackTrace, LogType type)
		{
			if (m_lockCount <= 0)
			{
				switch (type)
				{
				case LogType.Log:
					break;
				case LogType.Warning:
					break;
				case LogType.Assert:
					break;
				case LogType.Exception:
					break;
				case LogType.Error:
					break;
				}
			}
		}
	}

	public const int NOTSET = 0;

	public const int DEBUG = 10;

	public const int INFO = 20;

	public const int WARN = 30;

	public const int ERROR = 40;

	public const int CRITICAL = 50;

	public const string DefaultConfigResource = "Conf/logging";

	private static bool g_initialized;

	private static ILogger g_root;

	private static Dictionary<string, ILogger> g_loggers;

	private static int g_disable;

	private static IFormatter g_defaultFormatter;

	private static List<Action> g_destructors;

	private static bool g_registeredUnityLogCallback;

	private static RuntimeConfig g_config;

	public static IFormatter DefaultFormatter
	{
		get
		{
			return g_defaultFormatter;
		}
		set
		{
			g_defaultFormatter = value;
		}
	}

	public static string DefaultConfigPath
	{
		get
		{
			string text = Path.Combine("Assets/Resources", "Conf/logging") + ".xml";
			return text.Replace('\\', '/');
		}
	}

	public static RuntimeConfig Config
	{
		get
		{
			if (g_config == null)
			{
				LoadConfig();
			}
			return g_config;
		}
	}

	public static ILogger GetLogger(string name)
	{
		Initialize();
		if (name == null || name.Length == 0)
		{
			return g_root;
		}
		ILogger value;
		if (g_loggers.TryGetValue(name, out value))
		{
			return value;
		}
		value = new Logger(name);
		int num = name.LastIndexOf('.');
		if (num > 0)
		{
			string name2 = name.Substring(0, num);
			value.Parent = GetLogger(name2);
		}
		else
		{
			value.Parent = g_root;
		}
		g_loggers[name] = value;
		return value;
	}

	public static void Log(int level, string format, params object[] args)
	{
		Initialize();
		g_root.Log(level, format, args);
	}

	public static void Debug(string format, params object[] args)
	{
		Initialize();
		g_root.Debug(format, args);
	}

	public static void Info(string format, params object[] args)
	{
		Initialize();
		g_root.Info(format, args);
	}

	public static void Warn(string format, params object[] args)
	{
		Initialize();
		g_root.Warn(format, args);
	}

	public static void Error(string format, params object[] args)
	{
		Initialize();
		g_root.Error(format, args);
	}

	public static void Critical(string format, params object[] args)
	{
		Initialize();
		g_root.Critical(format, args);
	}

	public static void Log(UnityEngine.Object context, int level, string format, params object[] args)
	{
		Initialize();
		g_root.Log(context, level, format, args);
	}

	public static void Debug(UnityEngine.Object context, string format, params object[] args)
	{
		Initialize();
		g_root.Debug(context, format, args);
	}

	public static void Info(UnityEngine.Object context, string format, params object[] args)
	{
		Initialize();
		g_root.Info(context, format, args);
	}

	public static void Warn(UnityEngine.Object context, string format, params object[] args)
	{
		Initialize();
		g_root.Warn(context, format, args);
	}

	public static void Error(UnityEngine.Object context, string format, params object[] args)
	{
		Initialize();
		g_root.Error(context, format, args);
	}

	public static void Critical(UnityEngine.Object context, string format, params object[] args)
	{
		Initialize();
		g_root.Critical(context, format, args);
	}

	public static int GetDisableLevel()
	{
		Initialize();
		return g_disable;
	}

	public static void Disable(int level)
	{
		Initialize();
		g_disable = level;
	}

	public static IEnumerable<ILogger> GetLoggers()
	{
		Initialize();
		yield return g_root;
		foreach (ILogger value in g_loggers.Values)
		{
			yield return value;
		}
	}

	public static void Initialize()
	{
		if (g_initialized)
		{
			return;
		}
		g_root = null;
		g_loggers = null;
		g_disable = 0;
		g_defaultFormatter = null;
		g_destructors = null;
		g_registeredUnityLogCallback = false;
		IHandler handler = null;
		try
		{
			g_destructors = new List<Action>();
			g_defaultFormatter = new SimpleFormatter();
			g_loggers = new Dictionary<string, ILogger>();
			g_root = new Logger(string.Empty);
			g_root.SetLevel(30);
			g_initialized = true;
			RuntimeConfig config = Config;
			g_disable = config.Disabled;
			if (config.DebugLogHandlerEnabled)
			{
				handler = new DebugLogHandler();
				g_root.AddHandler(handler);
				handler = null;
			}
			if (Application.isPlaying && config.FileHandlerEnabled)
			{
				string temporaryCachePath = Application.temporaryCachePath;
				string text = Path.Combine(temporaryCachePath, "log.txt");
				string destFileName = Path.Combine(temporaryCachePath, "log.bak");
				if (File.Exists(text))
				{
					File.Copy(text, destFileName, true);
				}
				handler = new FileHandler(text, FileMode.Create);
				g_root.AddHandler(handler);
				handler = null;
			}
			int loggerCount = config.LoggerCount;
			for (int i = 0; i < loggerCount; i++)
			{
				RuntimeConfig.Logger logger = config.GetLogger(i);
				ILogger logger2 = GetLogger(logger.Name);
				logger2.SetLevel(logger.Level);
				logger2.Propagate = logger.Propagate;
			}
			if (config.CaptureUnityLog)
			{
				UnityLogLogger.RegisterLogCallback();
				g_registeredUnityLogCallback = true;
			}
		}
		catch (Exception)
		{
			if (handler != null)
			{
				handler.Close();
			}
			DestroyImpl();
			throw;
		}
	}

	public static void Destroy()
	{
		if (g_initialized)
		{
			DestroyImpl();
		}
	}

	public static string GetLevelName(int level)
	{
		switch (level)
		{
		case 0:
			return "NOTSET";
		case 10:
			return "DEBUG";
		case 20:
			return "INFO";
		case 30:
			return "WARN";
		case 40:
			return "ERROR";
		case 50:
			return "CRITICAL";
		default:
			return level.ToString();
		}
	}

	public static void RegisterDestructor(Action dtor)
	{
		if (dtor != null)
		{
			g_destructors.Add(dtor);
		}
	}

	private static void DestroyLogger(ILogger logger)
	{
		logger.Parent = null;
		logger.SetLevel(0);
		int filterCount = logger.GetFilterCount();
		for (int num = filterCount - 1; num >= 0; num--)
		{
			logger.RemoveFilter(logger.GetFilter(num));
		}
		int handlerCount = logger.GetHandlerCount();
		for (int num2 = handlerCount - 1; num2 >= 0; num2--)
		{
			IHandler handler = logger.GetHandler(num2);
			handler.Close();
			logger.RemoveHandler(handler);
		}
	}

	private static void DestroyImpl()
	{
		if (g_registeredUnityLogCallback)
		{
			Application.RegisterLogCallback(null);
			g_registeredUnityLogCallback = false;
		}
		if (g_destructors != null)
		{
			int count = g_destructors.Count;
			count--;
			while (count >= 0)
			{
				try
				{
					while (count >= 0)
					{
						g_destructors[count]();
						count--;
					}
				}
				catch (Exception)
				{
					count--;
				}
			}
			g_destructors = null;
		}
		if (g_loggers != null)
		{
			foreach (ILogger value in g_loggers.Values)
			{
				try
				{
					DestroyLogger(value);
				}
				catch (Exception)
				{
				}
			}
			g_loggers = null;
		}
		if (g_root != null)
		{
			try
			{
				DestroyLogger(g_root);
			}
			catch (Exception)
			{
			}
			g_root = null;
		}
		ReplaceConfig(null);
		g_initialized = false;
		g_disable = 0;
		g_defaultFormatter = null;
	}

	public static void ReplaceConfig(RuntimeConfig newConfig)
	{
		g_config = newConfig;
	}

	public static void LoadConfig()
	{
		RuntimeConfig runtimeConfig = new RuntimeConfig();
		try
		{
			TextAsset textAsset = (TextAsset)Resources.Load("Conf/logging", typeof(TextAsset));
			if (textAsset != null)
			{
				runtimeConfig.Load(new MemoryStream(textAsset.bytes));
			}
		}
		catch (Exception)
		{
		}
		g_config = runtimeConfig;
	}
}
