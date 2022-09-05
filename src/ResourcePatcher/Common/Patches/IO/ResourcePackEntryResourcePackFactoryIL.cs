using System;
using System.Reflection;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria.IO;
using Terraria.ModLoader;

namespace Rejuvena.ResourcePatcher.Common.Patches.IO
{
    public class ResourcePackEntryResourcePackFactoryIL : ILoadable
    {
        // public ResourcePack(IServiceProvider services, string path, ResourcePack.BrandingType branding = ResourcePack.BrandingType.None)
        private readonly ConstructorInfo? ResourcePackCtor =
            typeof(ResourcePack).GetConstructor(new[] {typeof(IServiceProvider), typeof(string), typeof(ResourcePack.BrandingType)});

        private readonly MethodInfo? CreateResourcePack =
            typeof(ResourcePackFactory).GetMethod(nameof(ResourcePackFactory.CreateResourcePack), BindingFlags.Public | BindingFlags.Static);

        void ILoadable.Load(Mod mod) {
            // These IL edits aren't super important, so logging the errors is fine... maybe.
            if (ResourcePackCtor is null) {
                mod.Logger.Error("Failed to find ResourcePack constructor!");
                return;
            }

            if (CreateResourcePack is null) {
                mod.Logger.Error("Failed to find ResourcePackFactory.CreateResourcePack method!");
                return;
            }

            IL.Terraria.IO.ResourcePackList.CreatePacksFromSavedJson += ReplaceResourcePackCtor;
            IL.Terraria.IO.ResourcePackList.CreatePacksFromDirectories += ReplaceResourcePackCtor;
            IL.Terraria.IO.ResourcePackList.CreatePacksFromZips += ReplaceResourcePackCtor;
            IL.Terraria.IO.ResourcePackList.CreatePacksFromWorkshopFolders += ReplaceResourcePackCtor;
        }

        void ILoadable.Unload() {
            IL.Terraria.IO.ResourcePackList.CreatePacksFromSavedJson -= ReplaceResourcePackCtor;
            IL.Terraria.IO.ResourcePackList.CreatePacksFromDirectories -= ReplaceResourcePackCtor;
            IL.Terraria.IO.ResourcePackList.CreatePacksFromZips -= ReplaceResourcePackCtor;
            IL.Terraria.IO.ResourcePackList.CreatePacksFromWorkshopFolders -= ReplaceResourcePackCtor;
        }

        private void ReplaceResourcePackCtor(ILContext il) {
            ILCursor c = new(il);

            while (c.TryGotoNext(MoveType.Before, x => x.MatchNewobj(ResourcePackCtor))) {
                c.Remove(); // TODO: Can we rewrite this WITHOUT removing the opcode?
                c.Emit(OpCodes.Call, CreateResourcePack); // Swap out the newobj ctor with a call to our own factory method.
            }
        }
    }
}