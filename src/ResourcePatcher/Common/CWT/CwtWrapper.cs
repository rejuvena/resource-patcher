using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Rejuvena.ResourcePatcher.Common.CWT
{
    public static class CwtWrapper<TKey, TValue>
        where TKey : class
        where TValue : class, new()
    {
        static CwtWrapper() {
            // This is implicitly unsubscribed during the unloading process, so nothing to fear.
            ResourcePatcher.OnUnload += patcher => { Data = null!; };
        }

        private static Dictionary<string, ConditionalWeakTable<TKey, TValue>> Data = new();

        public static TValue GetField(TKey key, string fieldName) {
            return GetFieldTable(fieldName).GetValue(key, _ => new TValue());
        }

        public static ConditionalWeakTable<TKey, TValue> GetFieldTable(string field) {
            return Data.ContainsKey(field) ? Data[field] : Data[field] = new ConditionalWeakTable<TKey, TValue>();
        }
    }

    public static class CwtExtensions
    {
        public static TValue GetDynamicField<TKey, TValue>(this TKey key, string fieldName)
            where TKey : class
            where TValue : class, new() {
            return CwtWrapper<TKey, TValue>.GetField(key, fieldName);
        }
    }
}