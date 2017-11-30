using System;
using System.Collections.Generic;
using System.Linq;
using Dockson.Projections;
using Shouldly;
using Xunit;

namespace Dockson.Tests.Projections
{
	public class MasterLeadTimeProjectionTests
	{
		private readonly MasterLeadTimeView _view;
		private readonly MasterLeadTimeProjection _projection;
		private readonly DateTime _now;

		public MasterLeadTimeProjectionTests()
		{
			_view = new MasterLeadTimeView();
			_projection = new MasterLeadTimeProjection(_view);
			_now = new DateTime(2017, 11, 30, 11, 47, 00);
		}

		[Fact]
		public void When_projecting_one_commit()
		{
			_projection.Project(CreateCommit(TimeSpan.Zero, TimeSpan.FromHours(1)), message => { });

			_view.ShouldSatisfyAllConditions(
				() => _view.Days.ShouldBe(new[] { _now.Date }),
				() => _view.Medians.ShouldContainKeyAndValue(_now.Date, TimeSpan.FromHours(1).TotalMinutes),
				() => _view.StandardDeviations.ShouldContainKeyAndValue(_now.Date, 0)
			);
		}

		[Fact]
		public void When_projecting_two_commits()
		{
			_projection.Project(CreateCommit(TimeSpan.Zero, TimeSpan.FromHours(1)), message => { });
			_projection.Project(CreateCommit(TimeSpan.FromHours(4), TimeSpan.FromHours(5)), message => { });
		}

		private MasterCommit CreateCommit(TimeSpan featureTime, TimeSpan masterTime) => new MasterCommit(
			CreateNotification(masterTime, "master"),
			CreateNotification(featureTime, "feature-whatever")
		);

		private Notification CreateNotification(TimeSpan offset, string branch) => new Notification
		{
			Type = Stages.Commit,
			TimeStamp = _now.Add(offset),
			Source = "github",
			Name = "SomeService",
			Version = "1.0.0",
			Tags = new Dictionary<string, string>
			{
				{ "commit", Guid.NewGuid().ToString() },
				{ "branch", branch }
			}
		};
	}

	public class MasterLeadTimeProjection
	{
		private readonly MasterLeadTimeView _view;
		private readonly Dictionary<DateTime, List<TimeSpan>> _source;

		public MasterLeadTimeProjection(MasterLeadTimeView view)
		{
			_view = view;
			_source = new Dictionary<DateTime, List<TimeSpan>>();
		}

		public void Project(MasterCommit message, Action<object> dispatch)
		{
			var key = message.TimeStamp.Date;
			var leadTime = message.LeadTime;

			if (_source.ContainsKey(key) == false)
				_source[key] = new List<TimeSpan>();

			_source[key].Add(leadTime);

			_view.StandardDeviations[key] = _source[key].Select(spans => spans.TotalMinutes).StandardDeviation();
			_view.Medians[key] = _source[key].Select(spans => spans.TotalMinutes).Median();
			_view.Days.Add(key);
		}
	}

	public class MasterLeadTimeView
	{
		public HashSet<DateTime> Days { get; set; }
		public Dictionary<DateTime, double> Medians { get; set; }
		public Dictionary<DateTime, double> StandardDeviations { get; set; }

		public MasterLeadTimeView()
		{
			Days = new HashSet<DateTime>();
			Medians = new Dictionary<DateTime, double>();
			StandardDeviations = new Dictionary<DateTime, double>();
		}
	}

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
