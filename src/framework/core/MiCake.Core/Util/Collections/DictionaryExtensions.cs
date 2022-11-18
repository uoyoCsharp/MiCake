namespace MiCake.Core.Util.Collections
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Finds if a value is included in the dictionary.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool HasValue<TKey, TValue>(this IDictionary<TKey, TValue> dic, TValue value)
        {
            return dic.Values.Any(v => v!.Equals(value));
        }

        /// <summary>
        /// Get key by value.
        /// <para>
        ///     Will return a result list which include all keys.
        /// </para>
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static List<TKey> GetKeyByValue<TKey, TValue>(this IDictionary<TKey, TValue> dic, TValue value)
        {
            return dic.Where(v => v.Equals(value)).Select(k => k.Key).ToList();
        }

        /// <summary>
        /// Gets the first key that meets the criteria.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TKey? GetFirstKeyByValue<TKey, TValue>(this IDictionary<TKey, TValue> dic, TValue value)
        {
            return dic.Where(v => v!.Equals(value)).Select(k => k!.Key).FirstOrDefault();
        }
    }
}
