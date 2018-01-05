using System;
using System.ComponentModel;
using System.Globalization;

namespace Dockson.Domain
{
	[TypeConverter(typeof(DayDateTypeConverter))]
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

	public class DayDateTypeConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string == false)
				return base.ConvertFrom(context, culture, value);

			return new DayDate(DateTime.Parse((string)value));
		}
	}
}
