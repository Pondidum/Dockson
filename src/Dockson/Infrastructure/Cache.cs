using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dockson.Infrastructure
{
	[JsonConverter(typeof(CacheJsonConverter))]
	public class Cache<TKey, TValue> : IEnumerable<TValue>
	{
		private readonly object _locker = new object();
		private IDictionary<TKey, TValue> _values;
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
		public void FromDictionary(IDictionary<TKey, TValue> dictionary) => _values = dictionary;

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

	public class CacheJsonConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var method = value.GetType().GetMethod(nameof(Cache<int, int>.ToDictionary));
			var dictionary = method.Invoke(value, null);

			serializer.Serialize(writer, dictionary);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var dt = typeof(Dictionary<,>).MakeGenericType(existingValue.GetType().GetGenericArguments());

			var method = existingValue.GetType().GetMethod(nameof(Cache<int, int>.FromDictionary));
			var dictionary = serializer.Deserialize(reader, dt);

			method.Invoke(existingValue, new[] { dictionary });

			return existingValue;
		}

		public override bool CanConvert(Type objectType)
		{
			return true;
		}
	}
}
