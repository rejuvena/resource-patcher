using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rejuvena.ResourcePatcher.Common.Util;
using ReLogic.Content;

namespace Rejuvena.ResourcePatcher.Utilities
{
    public class BatchDrawData
    {
        public readonly record struct TextureData
        {
            private readonly Asset<Texture2D>? _textureAsset = null;
            private readonly Texture2D? _fallbackTexture = null;

            public TextureData(Asset<Texture2D> textureAsset) {
                _textureAsset = textureAsset;
            }

            public TextureData(Texture2D fallbackTexture) {
                _fallbackTexture = fallbackTexture;
            }

            public Texture2D GetTexture() => _textureAsset?.Value ?? _fallbackTexture!;

            public static implicit operator TextureData(Asset<Texture2D> asset) => new(asset);
            public static implicit operator TextureData(Texture2D texture) => new(texture);
            public static implicit operator TextureData(DeferredAsset<Texture2D> deferred) => new(deferred.Asset);
        }

        public readonly record struct ScaleData
        {
            private readonly Vector2 _scale;

            public ScaleData(Vector2 scale) {
                _scale = scale;
            }

            public ScaleData(float scale) : this(new Vector2(scale, scale)) { }

            public Vector2 GetScale() => _scale;

            public static implicit operator ScaleData(Vector2 scale) => new(scale);
            public static implicit operator ScaleData(float scale) => new(scale);
        }

        public TextureData Texture { get; set; }

        public Vector2 Position { get; set; } = Vector2.Zero;

        public Rectangle? SourceRectangle { get; set; } = null;

        public Color Color { get; set; } = Color.White;

        public float Rotation { get; set; } = 0f;

        public Vector2 Origin { get; set; } = Vector2.Zero;

        public ScaleData Scale { get; set; } = Vector2.One;

        public SpriteEffects Effects { get; set; } = SpriteEffects.None;

        public float LayerDepth { get; set; } = 0f;

        public BatchDrawData(TextureData texture) {
            Texture = texture;
        }
    }

    public static class BatchDrawDataExtensions
    {
        public static void Draw(this SpriteBatch sb, BatchDrawData data) {
            sb.Draw(
                data.Texture.GetTexture(),
                data.Position,
                data.SourceRectangle,
                data.Color,
                data.Rotation,
                data.Origin,
                data.Scale.GetScale(),
                data.Effects,
                data.LayerDepth
            );
        }
    }
}