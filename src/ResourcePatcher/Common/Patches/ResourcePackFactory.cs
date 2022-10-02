using System;
using System.IO;
using Rejuvena.ResourcePatcher.API;
using Rejuvena.ResourcePatcher.Common.Data;
using Rejuvena.ResourcePatcher.Common.Util;
using Terraria.IO;

namespace Rejuvena.ResourcePatcher.Common.Patches
{
    internal static class ResourcePackFactory
    {
        public static ResourcePack CreateResourcePack(IServiceProvider services, string path, ResourcePack.BrandingType brandingType) {
            ResourcePack pack = File.Exists(Path.Combine(path, "patcher.json"))
                ? new PatcherResourcePack(services, path, brandingType)
                : new ResourcePack(services, path, brandingType);
            ResourcePackData data = pack.GetDynamicField<ResourcePack, ResourcePackData>("packData");

            return pack;
        }
    }
}