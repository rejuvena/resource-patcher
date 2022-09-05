using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rejuvena.ResourcePatcher.API;
using Rejuvena.ResourcePatcher.Common.Drawing;
using Rejuvena.ResourcePatcher.Common.UI;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.IO;
using Terraria.ModLoader;

namespace Rejuvena.ResourcePatcher.Common.Patches.UI
{
    public sealed class UIResourcePackDrawAdditionalIconDetour : ILoadable
    {
        void ILoadable.Load(Mod mod) {
            On.Terraria.GameContent.UI.Elements.UIResourcePack.DrawSelf += DrawAdditionalIcon;
        }

        void ILoadable.Unload() {
            On.Terraria.GameContent.UI.Elements.UIResourcePack.DrawSelf -= DrawAdditionalIcon;
        }

        private static void DrawAdditionalIcon(
            On.Terraria.GameContent.UI.Elements.UIResourcePack.orig_DrawSelf orig,
            UIResourcePack self,
            SpriteBatch spriteBatch
        ) {
            orig(self, spriteBatch);

            // Draw patcher icon.
            if (self.ResourcePack is PatcherResourcePack) {
                Texture2D tex = RPAssets.PatcherResourcePackIcon.Value;
                float xPos = self.GetDimensions().X + self.GetDimensions().Width - tex.Width - 3f;
                xPos -= self.ResourcePack.Branding == ResourcePack.BrandingType.SteamWorkshop ? tex.Width - 3f : 0f;

                spriteBatch.Draw(new BatchDrawData(tex) {Position = new Vector2(xPos, self.GetDimensions().Y + 2f)});
            }
        }
    }
}