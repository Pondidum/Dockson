using System;

namespace Dockson.Tests
{
	public static class Extensions
	{
		public static TimeSpan Hour(this int value) => TimeSpan.FromHours(value);
		public static TimeSpan Hours(this int value) => TimeSpan.FromHours(value);

		public static TimeSpan Minute(this int value) => TimeSpan.FromMinutes(value);
		public static TimeSpan Minutes(this int value) => TimeSpan.FromMinutes(value);
	}
}
