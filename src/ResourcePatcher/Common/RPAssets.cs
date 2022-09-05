using Microsoft.Xna.Framework.Graphics;
using Rejuvena.ResourcePatcher.Common.Assets;
using Terraria.ModLoader;

namespace Rejuvena.ResourcePatcher.Common
{
    public class RPAssets : ILoadable
    {
        public static DeferredAsset<Texture2D> PatcherResourcePackIcon = new("ResourcePatcher/Assets/UI/PatcherResourcePackIcon");

        void ILoadable.Load(Mod mod) { }

        void ILoadable.Unload() {
            PatcherResourcePackIcon = null!;
        }
    }
}