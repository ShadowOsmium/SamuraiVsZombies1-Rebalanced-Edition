public sealed class AssertionLogger : LoggerSingleton<AssertionLogger>
{
	public AssertionLogger()
	{
		LoggerSingleton<AssertionLogger>.SetLoggerName("Assertion");
	}
}
