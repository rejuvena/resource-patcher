using System;
using System.IO;
using Terraria.IO;

namespace Rejuvena.ResourcePatcher.API
{
    public class PatcherResourcePack : ResourcePack
    {
        public readonly ManifestStatus Status;
        public readonly PatcherManifest? Manifest;
        
        public PatcherResourcePack(IServiceProvider services, string path, BrandingType branding = BrandingType.None) : base(services, path, branding) {
            Manifest = PatcherManifest.FromFile(Path.Combine(path, "patcher.json"), out Status);
        }
    }
}