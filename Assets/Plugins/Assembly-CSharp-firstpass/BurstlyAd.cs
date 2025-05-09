using System;
using System.Collections.Generic;
using Glu.Burstly;
using Glu.Burstly.Internal.API;
using UnityEngine;

public class BurstlyAd : MonoBehaviour
{
	[SerializeField]
	private string publisherId;

	[SerializeField]
	private string zoneId;

	[SerializeField]
	private bool showOnStart;

	[SerializeField]
	private BannerAlignment initialBannerAlignment = BannerAlignment.TopLeft;

	[SerializeField]
	private Vector2 initialBannerOffset = Vector2.zero;

	[SerializeField]
	private bool initialPaused;

	[SerializeField]
	private int initialDefaultSessionLife = 20000;

	private bool initialized;

	private IBurstlyAPI api;

	private object peer;

	private IDictionary<string, object> targetingData;

	private Action<bool> presentFullScreenCallback;

	public string PublisherID
	{
		get
		{
			return publisherId;
		}
		set
		{
			if (peer != null)
			{
				throw new BurstlyException("Can't change publisher ID once ad is created");
			}
			publisherId = value;
		}
	}

	public string ZoneID
	{
		get
		{
			return zoneId;
		}
		set
		{
			if (peer != null)
			{
				throw new BurstlyException("Can't change zone ID once ad is created");
			}
			zoneId = value;
		}
	}

	public bool Precached
	{
		get
		{
			if (initialized)
			{
				return api.IsPrecached(peer);
			}
			return false;
		}
	}

	public bool Visible
	{
		get
		{
			if (!initialized)
			{
				return false;
			}
			return api.IsVisible(peer);
		}
		set
		{
			if (initialized)
			{
				api.SetVisible(peer, value);
			}
			else
			{
				showOnStart = value;
			}
		}
	}

	public bool Paused
	{
		get
		{
			if (!initialized)
			{
				return initialPaused;
			}
			return api.IsPaused(peer);
		}
		set
		{
			if (initialized)
			{
				api.SetPaused(peer, value);
			}
			else
			{
				initialPaused = value;
			}
		}
	}

	public bool FullScreen
	{
		get
		{
			if (!initialized)
			{
				return false;
			}
			return api.IsFullScreen(peer);
		}
	}

	public BannerAlignment BannerAlignment
	{
		get
		{
			if (!initialized)
			{
				return initialBannerAlignment;
			}
			return (BannerAlignment)api.GetBannerAlignment(peer);
		}
		set
		{
			if (value < (BannerAlignment)0 || value > BannerAlignment.Center)
			{
				throw new ArgumentException(string.Format("Invalid banner alignment {0}", value));
			}
			if (initialized)
			{
				api.SetBannerAlignment(peer, (int)value);
			}
			else
			{
				initialBannerAlignment = value;
			}
		}
	}

	public Vector2 BannerOffset
	{
		get
		{
			if (!initialized)
			{
				return initialBannerOffset;
			}
			return api.GetBannerOffset(peer);
		}
		set
		{
			if (initialized)
			{
				api.SetBannerOffset(peer, value);
			}
			else
			{
				initialBannerOffset = value;
			}
		}
	}

	public int DefaultSessionLife
	{
		get
		{
			if (!initialized)
			{
				return initialDefaultSessionLife;
			}
			return api.GetDefaultSessionLife(peer);
		}
		set
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException("DefaultSessionLife can't be negative");
			}
			if (initialized)
			{
				api.SetDefaultSessionLife(peer, value);
			}
			else
			{
				initialDefaultSessionLife = value;
			}
		}
	}

	private void Awake()
	{
		api = BurstlyAPI.Instance;
	}

	private void Start()
	{
		Initialize();
	}

	private void OnDestroy()
	{
		api.DestroyManager(peer);
		peer = null;
		initialized = false;
	}

	private void Update()
	{
		api.Update(peer);
	}

	private void Initialize()
	{
		if (initialized)
		{
			return;
		}
		if (string.IsNullOrEmpty(publisherId))
		{
			throw new BurstlyException("Publisher ID must be set");
		}
		if (string.IsNullOrEmpty(zoneId))
		{
			throw new BurstlyException("Zone ID must be set");
		}
		object obj = null;
		try
		{
			obj = api.CreateManager(publisherId, zoneId);
			api.SetBannerAlignment(obj, (int)initialBannerAlignment);
			api.SetBannerOffset(obj, initialBannerOffset);
			api.SetPaused(obj, initialPaused);
			api.SetDefaultSessionLife(obj, initialDefaultSessionLife);
			if (showOnStart)
			{
				api.SetVisible(obj, true);
			}
		}
		catch (Exception)
		{
			if (obj != null)
			{
				api.DestroyManager(obj);
			}
			throw;
		}
		peer = obj;
		initialized = true;
	}

	public void RefreshAd()
	{
		Initialize();
		api.RefreshAd(peer);
	}

	public void PrecacheAd()
	{
		Initialize();
		api.PrecacheAd(peer);
	}

	public IDictionary<string, object> GetPublisherTargetingData()
	{
		if (object.ReferenceEquals(targetingData, null))
		{
			return null;
		}
		return new Dictionary<string, object>(targetingData);
	}

	public void SetPublisherTargetingData(IDictionary<string, object> data)
	{
		if (data == null)
		{
			api.SetPublisherTargetingData(peer, string.Empty);
			targetingData = null;
			return;
		}
		string[] array = new string[data.Count];
		int num = 0;
		foreach (KeyValuePair<string, object> datum in data)
		{
			string key = datum.Key;
			if (object.ReferenceEquals(key, null))
			{
				throw new ArgumentException("Key can't be null");
			}
			object value = datum.Value;
			if (object.ReferenceEquals(value, null))
			{
				array[num] = string.Format("{0}=", key);
			}
			else
			{
				Type type = value.GetType();
				if (type == typeof(int))
				{
					array[num] = string.Format("{0}={1}", key, value);
				}
				else
				{
					array[num] = string.Format("{0}='{1}'", key, value);
				}
			}
			num++;
		}
		targetingData = new Dictionary<string, object>(data);
		if (initialized)
		{
			api.SetPublisherTargetingData(peer, string.Join(",", array));
		}
	}
}
