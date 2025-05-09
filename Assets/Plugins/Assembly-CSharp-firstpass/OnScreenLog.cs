using System.Collections.Generic;
using System.Text;
using UnityEngine;

public sealed class OnScreenLog : MonoBehaviour
{
	private class OnScreenLogHandler : Logging.HandlerBase
	{
		private OnScreenLog m_log;

		public OnScreenLogHandler(OnScreenLog log)
		{
			m_log = log;
		}

		public override void Emit(ref Logging.LogRecord record)
		{
			string s = Format(ref record);
			m_log.Log(s);
		}
	}

	public bool Visible;

	public Rect WindowRect;

	public bool Draggable = true;

	public bool ShowOpenButton = true;

	public bool ShowCloseButton = true;

	public bool AutoScroll = true;

	public bool DontDestroy = true;

	public string AttachToLogger = string.Empty;

	public int MaxMessageCount = 100;

	private Logging.ILogger m_loggerAttachedTo;

	private OnScreenLogHandler m_handler;

	private GUIStyle m_style;

	private Vector2 m_scroll;

	private Queue<string> m_messages;

	private StringBuilder m_stringBuilder;

	private bool m_dirty;

	private void Awake()
	{
		m_messages = new Queue<string>(MaxMessageCount);
		m_stringBuilder = new StringBuilder();
		m_handler = new OnScreenLogHandler(this);
		m_loggerAttachedTo = Logging.GetLogger(AttachToLogger);
		m_loggerAttachedTo.AddHandler(m_handler);
		if (DontDestroy)
		{
			Object.DontDestroyOnLoad(this);
		}
		if (WindowRect.width == 0f || WindowRect.height == 0f)
		{
			WindowRect.x = 8f;
			WindowRect.width = (float)Screen.width - 2f * WindowRect.x;
			WindowRect.height = Screen.height / 3;
			WindowRect.y = (float)Screen.height - WindowRect.height;
		}
	}

	private void OnDestroy()
	{
		m_loggerAttachedTo.RemoveHandler(m_handler);
	}

	private void OnGUI()
	{
		if (m_style == null)
		{
			InitializeGUI();
		}
		if (Visible)
		{
			WindowRect = GUI.Window(0, WindowRect, WindowProc, "Log");
		}
		else if (ShowOpenButton && GUILayout.Button("Log"))
		{
			Visible = true;
		}
	}

	private void WindowProc(int id)
	{
		if (m_dirty)
		{
			m_stringBuilder.Remove(0, m_stringBuilder.Length);
			foreach (string message in m_messages)
			{
				m_stringBuilder.AppendLine(message);
			}
			int length = m_stringBuilder.Length;
			if (length > 0)
			{
				m_stringBuilder.Remove(length - 1, 1);
			}
			m_dirty = false;
		}
		RectOffset border = GUI.skin.window.border;
		Rect rect = new Rect(0f, 0f, WindowRect.width, WindowRect.height);
		rect = border.Remove(rect);
		Event current2 = Event.current;
		EventType type = current2.type;
		if (AutoScroll)
		{
			m_scroll.y = 2.1474836E+09f;
		}
		m_scroll = GUILayout.BeginScrollView(m_scroll);
		if (type == EventType.MouseDrag && current2.type == EventType.Used)
		{
			AutoScroll = false;
		}
		GUILayout.Label(m_stringBuilder.ToString());
		GUILayout.EndScrollView();
		if (ShowCloseButton)
		{
			Rect position = new Rect(15f, 2f, 30f, border.top - 4);
			if (GUI.Button(position, GUIContent.none))
			{
				Visible = false;
			}
		}
		if (Draggable)
		{
			GUI.DragWindow();
		}
	}

	private void InitializeGUI()
	{
		m_style = new GUIStyle(GUI.skin.box);
		m_style.alignment = TextAnchor.UpperLeft;
		m_style.wordWrap = true;
	}

	private void Log(string s)
	{
		if (m_messages.Count >= MaxMessageCount)
		{
			if (MaxMessageCount <= 1)
			{
				m_messages.Clear();
			}
			else
			{
				do
				{
					m_messages.Dequeue();
				}
				while (m_messages.Count >= MaxMessageCount);
			}
			m_dirty = true;
		}
		if (m_messages.Count < MaxMessageCount)
		{
			m_messages.Enqueue(s);
			m_dirty = true;
		}
	}
}
