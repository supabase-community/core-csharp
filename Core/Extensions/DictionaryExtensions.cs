using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Supabase.Core.Extensions
{
    /// <summary>
    /// Extensions for the `Dictionary` Classes
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Merges two dictionaries, allowing overwrite priorities leftward.
        /// 
        /// Works in C#3/VS2008:
        /// Returns a new dictionary of this ... others merged leftward.
        /// Keeps the type of 'this', which must be default-instantiable.
        /// Example: 
        ///   result = map.MergeLeft(other1, other2, ...)
        /// From: https://stackoverflow.com/a/2679857/3629438
        /// </summary>
        /// <param name="me"></param>
        /// <param name="others"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <returns></returns>
        public static T MergeLeft<T, K, V>(this T me, params IDictionary<K, V>[] others)
            where T : IDictionary<K, V>, new()
        {
            T newMap = new T();
            foreach (IDictionary<K, V> src in (new List<IDictionary<K, V>> { me }).Concat(others))
            {
                foreach (KeyValuePair<K, V> p in src)
                {
                    newMap[p.Key] = p.Value;
                }
            }
            return newMap;
        }
    }
}
