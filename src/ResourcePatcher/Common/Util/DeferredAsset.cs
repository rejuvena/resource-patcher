using System;
using ReLogic.Content;
using Terraria.ModLoader;

namespace Rejuvena.ResourcePatcher.Common.Util
{
    // TODO: Implement IAsset when possible.
    public sealed class DeferredAsset<T> : IDisposable
        where T : class
    {
        public Asset<T> Asset => _backingAsset ??= ModContent.Request<T>(_path);
        public T Value => Asset.Value;

        private Asset<T>? _backingAsset;
        private readonly string _path;

        public DeferredAsset(string path) {
            _path = path;
        }

        #region IDisposable Impl

        public void Dispose() {
            Asset.Dispose();
        }

        #endregion
    }
}