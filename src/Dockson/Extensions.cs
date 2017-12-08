using System;
using System.Collections.Generic;
using System.Linq;

namespace Dockson
{
	public static class Extensions
	{
		public static void Each<T>(this IEnumerable<T> self, Action<T> action)
		{
			foreach (var item in self)
				action(item);
		}

		public static bool EqualsIgnore(this string left, string right) => string.Equals(left, right, StringComparison.OrdinalIgnoreCase);

		public static DateTime PreviousMonday(this DateTime self)
		{
			var daysToRemove = ((int)self.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;

			return self.AddDays(-daysToRemove);
		}

		// shamelessly copied from stackoverflow as I cannot maths.
		// https://stackoverflow.com/a/897463/1500
		public static double StandardDeviation(this IEnumerable<double> values)
		{
			double M = 0.0;
			double S = 0.0;
			int k = 1;

			foreach (double value in values)
			{
				var tmpM = M;
				M += (value - tmpM) / k;
				S += (value - tmpM) * (value - M);
				k++;
			}

			var deviation = Math.Sqrt(S / (k - 2));

			return double.IsNaN(deviation)
				? 0
				: deviation;
		}

		// not the most performant version, but honestly, I don't care at this point
		public static double Median(this IEnumerable<double> sequence)
		{
			var list = sequence.ToList();
			list.Sort();

			var mid = list.Count / 2;

			return list.Count % 2 != 0
				? list[mid]
				: (list[mid] + list[mid - 1]) / 2;
		}
	}
}
