﻿using Rejuvena.ResourcePatcher.Core;

namespace Rejuvena.ResourcePatcher.Common.Util
{
    public static class CwtUtilities
    {
        public static TValue GetDynamicField<TKey, TValue>(this TKey key, string fieldName)
            where TKey : class
            where TValue : class, new() {
            return Cwt<TKey, TValue>.GetField(key, fieldName);
        }
    }
}