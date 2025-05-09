using System;
using System.Runtime.Serialization;

namespace Glu.Burstly
{
	[Serializable]
	public class BurstlyException : Exception
	{
		public BurstlyException()
		{
		}

		public BurstlyException(string message)
			: base(message)
		{
		}

		public BurstlyException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected BurstlyException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
