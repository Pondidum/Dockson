using System;
using Dockson.Infrastructure;
using Shouldly;
using Xunit;

namespace Dockson.Tests.ExtensionsTests
{
	public class PreviousMondayTests
	{
		[Theory]
		[InlineData("2017-12-03", "2017-11-27")] //sunday => monday
		[InlineData("2017-11-27", "2017-11-27")] //monday => monday
		[InlineData("2017-11-29", "2017-11-27")] //wednesday => monday
		public void When_calculating_previous_monday(string inputDate, string expectedDate)
		{
			var input = DateTime.Parse(inputDate);
			var expected = DateTime.Parse(expectedDate);

			var actual = input.PreviousMonday();

			actual.ShouldSatisfyAllConditions(
				() => actual.DayOfWeek.ShouldBe(DayOfWeek.Monday),
				() => actual.Date.ShouldBe(expected.Date)
			);
		}
	}
}
