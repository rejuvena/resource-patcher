using System.Text;
using Microsoft.Xna.Framework;
using Terraria;

namespace Rejuvena.ResourcePatcher.Utilities
{
    public static class StringExtensions
    {
        // https://github.com/chi-rei-den/Localizer/blob/eddcaa798702a177c0cb03bb6ebc62042c39e2e9/Localizer/Helpers/Utils.cs#L288
        public static string AsRainbow(this string text, int frameCount) {
            StringBuilder rainbow = new();

            float hueUnit = 4f / text.Length;
            float baseHue = (frameCount % 300) / 300f;

            for (int i = 0; i < text.Length; i++) {
                float colorHue = baseHue + hueUnit * i / text.Length;
                Color color = Main.hslToRgb(1.5f - colorHue, 1, 0.7f);

                rainbow.Append($"[c/{color.R:X2}{color.G:X2}{color.B:X2}:{text[i]}]");
            }

            return rainbow.ToString();
        }
    }
}