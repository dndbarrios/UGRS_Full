using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UGRS.Core.Extensions {
    public static class ObjectExtension {
        public static object GetPropertyValue(this object @object, string property) => @object.GetType().GetProperty(property).GetValue(@object, null);
        public static void SetPropertyValue<T>(this object @object, string property, T value) => @object.GetType().GetProperty(property).SetValue(@object, value);
        public static T GetPropertyValue<T>(this object @object, string property) => (T)@object.GetType().GetProperty(property).GetValue(@object, null);
        public static object GetPropertyValueByAttribute<T>(this object @object) where T : Attribute => @object.GetPropertyByAttribute<T>().GetValue(@object, null);
        public static PropertyInfo GetPropertyByAttribute<T>(this object @object) where T : Attribute => @object.GetType().GetProperties().Where(p => p.IsDefined(typeof(T), false)).FirstOrDefault();

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
         (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source) {
                if (seenKeys.Add(keySelector(element))) {
                    yield return element;
                }
            }
        }
    }
}



