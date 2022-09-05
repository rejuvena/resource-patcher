using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Rejuvena.ResourcePatcher.API;
using Rejuvena.ResourcePatcher.Common.UI;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace Rejuvena.ResourcePatcher.Common.Patches.UI
{
    public class UIResourcePackInfoMenuResourcePatcherVersionDisplayIL : ILoadable
    {
        private readonly FieldInfo _packField = typeof(UIResourcePackInfoMenu).GetField("_pack", BindingFlags.Instance | BindingFlags.NonPublic)!;

        private readonly Dictionary<ManifestStatus, string> _statusKeys = new()
        {
            {ManifestStatus.OK, "Mods.ResourcePatcher.UI.ManifestOK"},
            {ManifestStatus.TooNew, "Mods.ResourcePatcher.UI.ManifestTooNew"},
            {ManifestStatus.TooOld, "Mods.ResourcePatcher.UI.ManifestTooOld"},
            {ManifestStatus.Unknown, "Mods.ResourcePatcher.UI.ManifestUnknown"},
        };
        
        private readonly Dictionary<ManifestStatus, Color> _colorKeys = new()
        {
            {ManifestStatus.OK, Colors.RarityGreen},
            {ManifestStatus.TooNew, Colors.RarityRed},
            {ManifestStatus.TooOld, Colors.RarityRed},
            {ManifestStatus.Unknown, Colors.RarityTrash},
        };

        void ILoadable.Load(Mod mod) {
            IL.Terraria.GameContent.UI.States.UIResourcePackInfoMenu.BuildPage += VersionDisplay;
        }

        void ILoadable.Unload() {
            IL.Terraria.GameContent.UI.States.UIResourcePackInfoMenu.BuildPage -= VersionDisplay;
        }

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
    }
}