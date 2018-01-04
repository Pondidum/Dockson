using System.Collections.Generic;
using Dockson.Infrastructure;
using Shouldly;
using Xunit;

namespace Dockson.Tests.Infrastructure.ExtensionsTests
{
	public class MedianTests
	{
		public static IEnumerable<object[]> TestCases => new[]
		{
			new object[] { 176m, new double[] { 176 } },
			new object[] { 5.5m, new double[] { 5, 6 } },
			new object[] { 3m, new double[] { 2, 3, 4 } },
			new object[] { 2.5m, new double[] { 1, 2, 3, 4 } },
			new object[] { 3m, new double[] { 1, 2, 3, 4, 5 } },
			new object[] { 15m, new double[] { 10, 20 } },
		};

		[Theory]
		[MemberData(nameof(TestCases))]
		public void When_checking_the_median(double expected, double[] values)
		{
			values.Median().ShouldBe(expected, tolerance: 0.005);
		}
	}
}
