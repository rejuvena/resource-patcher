using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace Rejuvena.ResourcePatcher.Common.Assets
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