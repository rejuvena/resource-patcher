using System;

namespace Rejuvena.ResourcePatcher.Utilities
{
    public sealed class ImmutableCache<T> : IDisposable
    {
        private readonly T _value;
        private readonly Action<T> _setter;
        
        public ImmutableCache(T value, Action<T> setter) {
            _value = value;
            _setter = setter;
        }
        
        public void Dispose() {
            _setter(_value);
        }
    }
}