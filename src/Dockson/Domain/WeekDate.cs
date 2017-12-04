using System;

namespace Dockson.Domain
{
	public class WeekDate : IEquatable<WeekDate>
	{
		private readonly DateTime _fromDate;
		private readonly DateTime _toDate;

		public WeekDate(DateTime fromDate)
		{
			_fromDate = fromDate.PreviousMonday().Date;
			_toDate = _fromDate.AddDays(6);
		}

		public DateTime Start => _fromDate;
		public DateTime Finish => _toDate;

		public bool Equals(WeekDate other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;

			return _fromDate.Equals(other._fromDate);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;

			return Equals((WeekDate)obj);
		}

		public override int GetHashCode() => _fromDate.GetHashCode();
		public override string ToString() => _fromDate.ToShortDateString();
	}
}
