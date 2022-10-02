using Microsoft.Xna.Framework.Graphics;
using Rejuvena.ResourcePatcher.Common.Util;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;

namespace Rejuvena.ResourcePatcher.Common.UI
{
    public class UIRainbowText : UIText
    {
        private int _frameCount;
        
        public UIRainbowText(string text, float textScale = 1, bool large = false) : base(text, textScale, large) { }
        public UIRainbowText(LocalizedText text, float textScale = 1, bool large = false) : base(text, textScale, large) { }

        protected override void DrawSelf(SpriteBatch spriteBatch) {
            using ImmutableCache<string> _ = new(Text, SetText);
            SetText(Text.AsRainbow(_frameCount++));
            base.DrawSelf(spriteBatch);
        }
    }
}