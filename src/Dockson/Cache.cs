using System;
using System.Collections;
using System.Collections.Generic;

namespace Dockson
{
	public class Cache<TKey, TValue> : IEnumerable<TValue>
	{
		private readonly object _locker = new object();
		private readonly IDictionary<TKey, TValue> _values;
		private readonly Func<TKey, TValue> _onMissing;

		public Cache(Func<TKey, TValue> onMissing)
		{
			_values = new Dictionary<TKey, TValue>();
			_onMissing = onMissing;
		}

		public Cache(IEqualityComparer<TKey> comparer, Func<TKey, TValue> onMissing)
		{
			_values = new Dictionary<TKey, TValue>(comparer);
			_onMissing = onMissing;
		}

		public TValue this[TKey key]
		{
			get
			{
				Fill(key, _onMissing);

				return _values[key];
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<TValue>)this).GetEnumerator();
		public IEnumerator<TValue> GetEnumerator() => _values.Values.GetEnumerator();
		public IDictionary<TKey, TValue> ToDictionary() => new Dictionary<TKey, TValue>(_values);

		private void Fill(TKey key, Func<TKey, TValue> onMissing)
		{
			if (!_values.ContainsKey(key))
			{
				lock (_locker)
				{
					if (!_values.ContainsKey(key))
					{
						var value = onMissing(key);
						_values.Add(key, value);
					}
				}
			}
		}
	}
}
