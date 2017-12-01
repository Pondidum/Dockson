using System;
using System.Collections.Generic;
using System.Linq;

namespace Dockson
{
	public static class Extensions
	{
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

		// likewise
		// https://stackoverflow.com/a/22702269/1500

		/// <summary>
		/// Partitions the given list around a pivot element such that all elements on left of pivot are <= pivot
		/// and the ones at thr right are > pivot. This method can be used for sorting, N-order statistics such as
		/// as median finding algorithms.
		/// Pivot is selected ranodmly if random number generator is supplied else its selected as last element in the list.
		/// Reference: Introduction to Algorithms 3rd Edition, Corman et al, pp 171
		/// </summary>
		private static int Partition<T>(this IList<T> list, int start, int end, Random rnd = null) where T : IComparable<T>
		{
			if (rnd != null)
				list.Swap(end, rnd.Next(start, end + 1));

			var pivot = list[end];
			var lastLow = start - 1;
			for (var i = start; i < end; i++)
			{
				if (list[i].CompareTo(pivot) <= 0)
					list.Swap(i, ++lastLow);
			}

			list.Swap(end, ++lastLow);
			return lastLow;
		}

		/// <summary>
		/// Returns Nth smallest element from the list. Here n starts from 0 so that n=0 returns minimum, n=1 returns 2nd smallest element etc.
		/// Note: specified list would be mutated in the process.
		/// Reference: Introduction to Algorithms 3rd Edition, Corman et al, pp 216
		/// </summary>
		private static T NthOrderStatistic<T>(this IList<T> list, int n, Random rnd = null) where T : IComparable<T>
		{
			return NthOrderStatistic(list, n, 0, list.Count - 1, rnd);
		}

		private static T NthOrderStatistic<T>(this IList<T> list, int n, int start, int end, Random rnd) where T : IComparable<T>
		{
			while (true)
			{
				var pivotIndex = list.Partition(start, end, rnd);
				if (pivotIndex == n)
					return list[pivotIndex];

				if (n < pivotIndex)
					end = pivotIndex - 1;
				else
					start = pivotIndex + 1;
			}
		}

		private static void Swap<T>(this IList<T> list, int i, int j)
		{
			if (i == j) //This check is not required but Partition function may make many calls so its for perf reason
				return;
			var temp = list[i];
			list[i] = list[j];
			list[j] = temp;
		}

		public static double Median(this IEnumerable<double> sequence)
		{
			var list = sequence.ToList();
			var mid = (list.Count - 1) / 2;

			return list.NthOrderStatistic(mid);
		}
	}
}
