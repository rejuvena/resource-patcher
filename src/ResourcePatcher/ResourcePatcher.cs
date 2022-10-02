using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Rejuvena.ResourcePatcher.API;
using Rejuvena.ResourcePatcher.Common.Assets;
using Rejuvena.ResourcePatcher.Common.CWT.Data;
using Rejuvena.ResourcePatcher.Common.Patches;
using Rejuvena.ResourcePatcher.Common.UI;
using Rejuvena.ResourcePatcher.Utilities;
using ReLogic.Content.Sources;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace Rejuvena.ResourcePatcher
{
    public class ResourcePatcher : Mod
    {
        public static event Action<ResourcePatcher>? OnLoad;
        public static event Action<ResourcePatcher>? OnUnload;

        private readonly MethodInfo _hasAssetMethod = typeof(ModContent).GetMethod("HasAsset", BindingFlags.Public | BindingFlags.Static)!;

        public override void Load() {
            base.Load();

            On.Terraria.IO.ResourcePack.GetContentSource += AddDataToContentSource;
            IL.Terraria.IO.ResourcePackList.CreatePacksFromSavedJson += ReplaceResourcePackCtor;
            IL.Terraria.IO.ResourcePackList.CreatePacksFromDirectories += ReplaceResourcePackCtor;
            IL.Terraria.IO.ResourcePackList.CreatePacksFromZips += ReplaceResourcePackCtor;
            IL.Terraria.IO.ResourcePackList.CreatePacksFromWorkshopFolders += ReplaceResourcePackCtor;
            On.Terraria.GameContent.UI.Elements.UIResourcePack.DrawSelf += DrawAdditionalIcon;
            IL.Terraria.GameContent.UI.States.UIResourcePackInfoMenu.BuildPage += VersionDisplay;
            On.Terraria.Localization.LanguageManager.UseSources += LanguageManagerUsePatcherSources;

            OnLoad?.Invoke(this);
        }

        public override void Unload() {
            base.Unload();

            OnUnload?.Invoke(this);

            // Release any subscriptions.
            OnLoad = null;
            OnUnload = null;
        }

        #region AddDataToContentSource Detour

        private static IContentSource AddDataToContentSource(On.Terraria.IO.ResourcePack.orig_GetContentSource orig, ResourcePack self) {
            IContentSource source = orig(self);
            source.GetData().Pack = self;
            return source;
        }

        #endregion

        #region ReplaceResourcePackCtor IL

        // public ResourcePack(IServiceProvider services, string path, ResourcePack.BrandingType branding = ResourcePack.BrandingType.None)
        private readonly ConstructorInfo? _resourcePackCtor =
            typeof(ResourcePack).GetConstructor(new[] {typeof(IServiceProvider), typeof(string), typeof(ResourcePack.BrandingType)});

        private readonly MethodInfo? _createResourcePack =
            typeof(ResourcePackFactory).GetMethod(nameof(ResourcePackFactory.CreateResourcePack), BindingFlags.Public | BindingFlags.Static);

        private void ReplaceResourcePackCtor(ILContext il) {
            ILCursor c = new(il);

            while (c.TryGotoNext(MoveType.Before, x => x.MatchNewobj(_resourcePackCtor))) {
                c.Remove(); // TODO: Can we rewrite this WITHOUT removing the opcode?
                c.Emit(OpCodes.Call, _createResourcePack); // Swap out the newobj ctor with a call to our own factory method.
            }
        }

        #endregion

        #region AdditionalIcon Detour

        private static void DrawAdditionalIcon(
            On.Terraria.GameContent.UI.Elements.UIResourcePack.orig_DrawSelf orig,
            UIResourcePack self,
            SpriteBatch spriteBatch
        ) {
            orig(self, spriteBatch);

            // Draw patcher icon.
            if (self.ResourcePack is PatcherResourcePack) {
                Texture2D tex = RpAssets.PatcherResourcePackIcon.Value;
                float xPos = self.GetDimensions().X + self.GetDimensions().Width - tex.Width - 3f;
                xPos -= self.ResourcePack.Branding == ResourcePack.BrandingType.SteamWorkshop ? tex.Width - 3f : 0f;

                spriteBatch.Draw(new BatchDrawData(tex) {Position = new Vector2(xPos, self.GetDimensions().Y + 2f)});
            }
        }

        #endregion

        #region VersionDisplay IL

        private readonly FieldInfo _packField = typeof(UIResourcePackInfoMenu).GetField("_pack", BindingFlags.Instance | BindingFlags.NonPublic)!;

        private readonly Dictionary<ManifestStatus, string> _statusKeys = new()
        {
            {ManifestStatus.Ok, "Mods.ResourcePatcher.UI.ManifestOK"},
            {ManifestStatus.TooNew, "Mods.ResourcePatcher.UI.ManifestTooNew"},
            {ManifestStatus.TooOld, "Mods.ResourcePatcher.UI.ManifestTooOld"},
            {ManifestStatus.Unknown, "Mods.ResourcePatcher.UI.ManifestUnknown"},
        };

        private readonly Dictionary<ManifestStatus, Color> _colorKeys = new()
        {
            {ManifestStatus.Ok, Colors.RarityGreen},
            {ManifestStatus.TooNew, Colors.RarityRed},
            {ManifestStatus.TooOld, Colors.RarityRed},
            {ManifestStatus.Unknown, Colors.RarityTrash},
        };

        private void VersionDisplay(ILContext il) {
            const string textValueAnchor = "UI.Version";
            const float dividerHeight = 52f;
            const float containerY = -74f;
            ILCursor c = new(il);

            c.GotoNext(x => x.MatchLdstr(textValueAnchor)); // Match "UI.Version"
            c.GotoNext(MoveType.After, x => x.MatchCallvirt<UIElement>("Append")); // Jump to after the element is appended.

            int index = c.Index;

            int elementIndex = 0;
            c.GotoPrev(x => x.MatchLdloc(out _));
            c.GotoPrev(x => x.MatchLdloc(out elementIndex)); // Capture index of element to append to.

            c.Index = index;
            c.Emit(OpCodes.Ldarg_0); // this
            c.Emit(OpCodes.Ldfld, _packField); // push _pack
            c.Emit(OpCodes.Ldloc, elementIndex); // element
            c.EmitDelegate((ResourcePack pack, UIElement element) =>
            {
                if (pack is not PatcherResourcePack resPack) return;

                UIRainbowText rpText = new(Language.GetTextValue("Mods.ResourcePatcher.UI.RPName"), 0.9f)
                {
                    HAlign = 0f,
                    VAlign = 1f
                };
                rpText.Top.Set(16f, 0f);
                element.Append(rpText);

                UIText versText = new(Language.GetTextValue(_statusKeys[resPack.Status]), 0.9f)
                {
                    HAlign = 1f,
                    VAlign = 1f,
                    TextColor = _colorKeys[resPack.Status]
                };
                versText.Top.Set(16f, 0f);
                element.Append(versText);
            });

            c.GotoNext(MoveType.After, x => x.MatchLdcR4(dividerHeight));
            c.Emit(OpCodes.Pop);
            c.Emit(OpCodes.Ldarg_0); // this
            c.Emit(OpCodes.Ldfld, _packField); // push _pack
            c.EmitDelegate((ResourcePack pack) => dividerHeight + (pack is PatcherResourcePack ? 24f : 0f));

            c.GotoNext(MoveType.After, x => x.MatchLdcR4(containerY));
            c.Emit(OpCodes.Pop);
            c.Emit(OpCodes.Ldarg_0); // this
            c.Emit(OpCodes.Ldfld, _packField); // push _pack
            c.EmitDelegate((ResourcePack pack) => containerY - (pack is PatcherResourcePack ? 28f : 0f));
        }

        #endregion

        #region LanguageManagerUsePatcherSources

        private readonly MethodInfo _processCopyCommandsInText =
            typeof(LanguageManager).GetMethod("ProcessCopyCommandsInText", BindingFlags.NonPublic | BindingFlags.Instance)!;
        
        private void LanguageManagerUsePatcherSources(
            On.Terraria.Localization.LanguageManager.orig_UseSources orig,
            LanguageManager self,
            List<IContentSource> sourcesFromLowestToHighest
        ) {
            throw new NotImplementedException();
        }

        #endregion
    }
}