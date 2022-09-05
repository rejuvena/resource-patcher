using System;
using Terraria.ModLoader;

namespace Rejuvena.ResourcePatcher
{
    public class ResourcePatcher : Mod
    {
        public static event Action<ResourcePatcher>? OnLoad;
        public static event Action<ResourcePatcher>? OnUnload;

        public override void Load() {
            base.Load();

            OnLoad?.Invoke(this);
        }

        public override void Unload() {
            base.Unload();

            OnUnload?.Invoke(this);

            // Release any subscriptions.
            OnLoad = null;
            OnUnload = null;
        }
    }
}

namespace ResourcePatcher
{
    // Circumvents this stupid and arbitrary decision:
    // https://github.com/tModLoader/tModLoader/blob/58794a48d27a7d2babb7760dd33cbcfe5068305f/patches/tModLoader/Terraria/ModLoader/Core/AssemblyManager.cs#L213
    internal sealed class Dummy
    { }
}