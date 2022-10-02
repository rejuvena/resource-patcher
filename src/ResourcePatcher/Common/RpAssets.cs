using Microsoft.Xna.Framework.Graphics;
using Rejuvena.ResourcePatcher.Common.Util;
using Terraria.ModLoader;

namespace Rejuvena.ResourcePatcher.Common
{
    public sealed class RpAssets : ILoadable
    {
        public static DeferredAsset<Texture2D> PatcherResourcePackIcon = new("ResourcePatcher/Assets/UI/PatcherResourcePackIcon");

        void ILoadable.Load(Mod mod) { }

        void ILoadable.Unload() {
            PatcherResourcePackIcon = null!;
        }
    }
}