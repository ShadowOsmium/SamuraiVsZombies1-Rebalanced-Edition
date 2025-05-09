using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace Glu.Burstly.Internal.API
{
	public class NativeBurstlyAPI : IBurstlyAPI
	{
		private class Data
		{
			private byte[] buffer;

			public IntPtr Peer { get; set; }

			public byte[] GetBuffer(int capacity)
			{
				int num = ((buffer != null) ? buffer.Length : 0);
				if (num < capacity)
				{
					buffer = new byte[capacity * 2];
				}
				return buffer;
			}

			public void Close()
			{
				Peer = IntPtr.Zero;
				buffer = null;
			}
		}

		private const string importName = "burstlyplugin";

		public object CreateManager(string publisherId, string zoneId)
		{
			Data data = new Data();
			data.Peer = Glu_Burstly_CreateManager(publisherId, zoneId);
			return data;
		}

		public void DestroyManager(object peer)
		{
			Data data = (Data)peer;
			Glu_Burstly_DestroyManager(data.Peer);
			data.Close();
		}

		public void RefreshAd(object peer)
		{
			Glu_Burstly_RefreshAd(((Data)peer).Peer);
		}

		public void PrecacheAd(object peer)
		{
			Glu_Burstly_PrecacheAd(((Data)peer).Peer);
		}

		public bool IsPrecached(object peer)
		{
			return Glu_Burstly_IsPrecached(((Data)peer).Peer);
		}

		public bool IsVisible(object peer)
		{
			return Glu_Burstly_IsVisible(((Data)peer).Peer);
		}

		public void SetVisible(object peer, bool visible)
		{
			Glu_Burstly_SetVisible(((Data)peer).Peer, visible);
		}

		public bool IsPaused(object peer)
		{
			return Glu_Burstly_IsPaused(((Data)peer).Peer);
		}

		public void SetPaused(object peer, bool paused)
		{
			Glu_Burstly_SetPaused(((Data)peer).Peer, paused);
		}

		public int GetBannerAlignment(object peer)
		{
			return Glu_Burstly_GetBannerAlignment(((Data)peer).Peer);
		}

		public void SetBannerAlignment(object peer, int anchor)
		{
			Glu_Burstly_SetBannerAlignment(((Data)peer).Peer, anchor);
		}

		public Vector2 GetBannerOffset(object peer)
		{
			int x;
			int y;
			Glu_Burstly_GetBannerOffset(((Data)peer).Peer, out x, out y);
			return new Vector2(x, y);
		}

		public void SetBannerOffset(object peer, Vector2 point)
		{
			Glu_Burstly_SetBannerOffset(((Data)peer).Peer, (int)point.x, (int)point.y);
		}

		public int GetDefaultSessionLife(object peer)
		{
			return Glu_Burstly_GetDefaultSessionLife(((Data)peer).Peer);
		}

		public void SetDefaultSessionLife(object peer, int timeout)
		{
			Glu_Burstly_SetDefaultSessionLife(((Data)peer).Peer, timeout);
		}

		public void SetPublisherTargetingData(object peer, string data)
		{
			byte[] array = null;
			int num = 0;
			Data data2 = (Data)peer;
			if (data != null)
			{
				num = Encoding.UTF8.GetByteCount(data);
				array = data2.GetBuffer(num);
				Encoding.UTF8.GetBytes(data, 0, data.Length, array, 0);
			}
			Glu_Burstly_SetPublisherTargetingData(data2.Peer, num, array);
		}

		public bool IsFullScreen(object peer)
		{
			return Glu_Burstly_IsFullScreen(((Data)peer).Peer);
		}

		public void Update(object peer)
		{
		}

		public string GetSDKVersion()
		{
			int num = 256;
			byte[] array = new byte[num];
			int count = Glu_Burstly_GetSDKVersion(num, array);
			return Encoding.UTF8.GetString(array, 0, count);
		}

		public void SetServerURL(string url)
		{
			Glu_Burstly_SetServerURL(url);
		}

		[DllImport("burstlyplugin")]
		private static extern IntPtr Glu_Burstly_CreateManager(string publisherId, string zoneId);

		[DllImport("burstlyplugin")]
		private static extern void Glu_Burstly_DestroyManager(IntPtr peer);

		[DllImport("burstlyplugin")]
		private static extern void Glu_Burstly_RefreshAd(IntPtr peer);

		[DllImport("burstlyplugin")]
		private static extern void Glu_Burstly_PrecacheAd(IntPtr peer);

		[DllImport("burstlyplugin")]
		private static extern bool Glu_Burstly_IsPrecached(IntPtr peer);

		[DllImport("burstlyplugin")]
		private static extern void Glu_Burstly_SetVisible(IntPtr peer, bool visible);

		[DllImport("burstlyplugin")]
		private static extern bool Glu_Burstly_IsVisible(IntPtr peer);

		[DllImport("burstlyplugin")]
		private static extern void Glu_Burstly_SetPaused(IntPtr peer, bool visible);

		[DllImport("burstlyplugin")]
		private static extern bool Glu_Burstly_IsPaused(IntPtr peer);

		[DllImport("burstlyplugin")]
		private static extern void Glu_Burstly_SetBannerAlignment(IntPtr peer, int anchor);

		[DllImport("burstlyplugin")]
		private static extern int Glu_Burstly_GetBannerAlignment(IntPtr peer);

		[DllImport("burstlyplugin")]
		private static extern void Glu_Burstly_SetBannerOffset(IntPtr peer, int x, int y);

		[DllImport("burstlyplugin")]
		private static extern void Glu_Burstly_GetBannerOffset(IntPtr peer, out int x, out int y);

		[DllImport("burstlyplugin")]
		private static extern void Glu_Burstly_SetDefaultSessionLife(IntPtr peer, int timeoutMs);

		[DllImport("burstlyplugin")]
		private static extern int Glu_Burstly_GetDefaultSessionLife(IntPtr peer);

		[DllImport("burstlyplugin")]
		private static extern void Glu_Burstly_SetPublisherTargetingData(IntPtr peer, int size, [In][MarshalAs(UnmanagedType.LPArray)] byte[] utf8Data);

		[DllImport("burstlyplugin")]
		private static extern bool Glu_Burstly_IsFullScreen(IntPtr peer);

		[DllImport("burstlyplugin")]
		private static extern int Glu_Burstly_PeekMessage(IntPtr peer, int size, [Out][MarshalAs(UnmanagedType.LPArray)] byte[] utf8Message);

		[DllImport("burstlyplugin")]
		private static extern void Glu_Burstly_SetServerURL(string url);

		[DllImport("burstlyplugin")]
		private static extern int Glu_Burstly_GetSDKVersion(int size, [Out][MarshalAs(UnmanagedType.LPArray)] byte[] utf8Ver);
	}
}
