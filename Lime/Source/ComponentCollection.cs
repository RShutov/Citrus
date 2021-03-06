﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace Lime
{
	public class Component
	{
		private static Dictionary<Type, int> keyMap = new Dictionary<Type, int>();
		private static int keyCounter;

		internal static int GetKeyForType(Type type)
		{
			lock (keyMap) {
				int key;
				if (!keyMap.TryGetValue(type, out key)) {
					key = ++keyCounter;
					keyMap.Add(type, key);
				}
				return key;
			}	
		}

		internal int GetKey() => GetKeyForType(GetType());
	}

	public class ComponentCollection<TComponent> : ICollection<TComponent> where TComponent : Component
	{
		private static Bucket[] emptyBuckets = new Bucket[0];

		protected Bucket[] buckets = emptyBuckets;

		public int Count { get; private set; }

		public bool IsReadOnly => false;

		public bool Contains(TComponent component) => ContainsKey(component.GetKey());
		public bool Contains<T>() where T : TComponent => ContainsKey(ComponentKeyResolver<T>.Key);

		private bool ContainsKey(int key)
		{
			for (var i = 0; i < buckets.Length; i++) {
				var b = buckets[(key + i) & (buckets.Length - 1)];
				if (b.Key == key) {
					return true;
				}
				if (b.Key == 0) {
					break;
				}
			}
			return false;
		}

		public T Get<T>() where T : TComponent
		{
			var key = ComponentKeyResolver<T>.Key;
			for (var i = 0; i < buckets.Length; i++) {
				var b = buckets[(key + i) & (buckets.Length - 1)];
				if (b.Key == key) {
					return (T)b.Component;
				}
				if (b.Key == 0) {
					break;
				}
			}
			return default(T);
		}

		public T GetOrAdd<T>() where T : TComponent, new()
		{
			var c = Get<T>();
			if (c == null) {
				c = new T();
				Add(c);
			}
			return c;
		}

		public virtual void Add(TComponent component)
		{
			if (Contains(component)) {
				throw new InvalidOperationException("Attempt to add a component twice.");
			}
			if (buckets.Length == 0) {
				buckets = new Bucket[1];
			}
			var loadFactor = CalcLoadFactor();
			if (loadFactor >= 0.7f) {
				var newBuckets = new Bucket[buckets.Length * 2];
				foreach (var b in buckets) {
					if (b.Key > 0) {
						AddHelper(newBuckets, b.Component);
					}
				}
				buckets = newBuckets;
			}
			AddHelper(buckets, component);
			Count++;
		}

		private float CalcLoadFactor() => (float)Count / buckets.Length;

		private static void AddHelper(Bucket[] buckets, TComponent component)
		{
			int key = component.GetKey();
			for (var i = 0; i < buckets.Length; i++) {
				var j = (key + i) & (buckets.Length - 1);
				if (buckets[j].Key <= 0) {
					buckets[j].Key = key;
					buckets[j].Component = component;
					return;
				}
			}
			throw new InvalidOperationException();
		}

		public bool Remove<T>() where T : TComponent
		{
			var c = Get<T>();
			return c != null && Remove(c);
		}

		public virtual bool Remove(TComponent component)
		{
			var key = component.GetKey();
			for (var i = 0; i < buckets.Length; i++) {
				var j = (key + i) & (buckets.Length - 1);
				if (buckets[j].Key == key) {
					buckets[j].Key = -1;
					buckets[j].Component = default(TComponent);
					Count--;
					return true;
				}
			}
			return false;
		}

		IEnumerator<TComponent> IEnumerable<TComponent>.GetEnumerator()
		{
			foreach (var b in buckets) {
				if (b.Key > 0) {
					yield return b.Component;
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			foreach (var b in buckets) {
				if (b.Key > 0) {
					yield return b.Component;
				}
			}
		}

		public virtual void Clear()
		{
			for (int i = 0; i < buckets.Length; i++) {
				buckets[i].Key = 0;
				buckets[i].Component = default(TComponent);
			}
			Count = 0;
		}

		public void CopyTo(TComponent[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		private static class ComponentKeyResolver<T> where T : TComponent
		{
			public static readonly int Key = Component.GetKeyForType(typeof(T));
		}

		protected struct Bucket
		{
			// Key special values:
			//  0 - means an empty bucket.
			// -1 - a deleted component.
			public int Key;
			public TComponent Component;
		}
	}
}
