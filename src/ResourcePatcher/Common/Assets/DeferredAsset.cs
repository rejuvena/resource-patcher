using System;
using ReLogic.Content;
using Terraria.ModLoader;

namespace Rejuvena.ResourcePatcher.Common.Assets
{
    // TODO: Implement IAsset when possible.
    public sealed class DeferredAsset<T> : IDisposable
        where T : class
    {
        public Asset<T> Asset => BackingAsset ??= ModContent.Request<T>(Path);
        public T Value => Asset.Value;

        private Asset<T>? BackingAsset;
        private readonly string Path;

        public DeferredAsset(string path) {
            Path = path;
        }

        #region IDisposable Impl

        public void Dispose() {
            Asset.Dispose();
        }

        #endregion
    }
}