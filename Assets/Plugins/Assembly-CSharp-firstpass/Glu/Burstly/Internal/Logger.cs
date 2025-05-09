namespace Glu.Burstly.Internal
{
	public sealed class Logger : LoggerSingleton<Logger>
	{
		public Logger()
		{
			LoggerSingleton<Logger>.SetLoggerName("Package.Burstly");
		}
	}
}
