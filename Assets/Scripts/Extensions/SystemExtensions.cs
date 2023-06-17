using System;
using System.Collections.Generic;

namespace SilgonGalaxy.Extensions {
	public static class SystemExtensions {
		public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> action) {
			foreach (var item in enumerable) {
				action(item);
				yield return item;
			}
		}
	}
}