using System;

namespace Dockson.Domain
{
	public class DayDate
	{
		private readonly DateTime _fromDate;

		public DayDate(DateTime fromDate)
		{
			_fromDate = fromDate.Date;
		}

		public bool Includes(DateTime timestamp) => _fromDate == timestamp.Date;

		public bool Equals(DayDate other)
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

			return Equals((DayDate)obj);
		}

		public override int GetHashCode() => _fromDate.GetHashCode();
		public override string ToString() => _fromDate.ToShortDateString();
	}
}
