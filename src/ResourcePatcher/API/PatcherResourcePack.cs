using System;
using System.IO;
using Terraria.IO;

namespace Rejuvena.ResourcePatcher.API
{
    /// <summary>
    ///     The backbone of Resource Patcher functionality.
    /// </summary>
    public class PatcherResourcePack : ResourcePack
    {
        /// <summary>
        ///     The manifest version status.
        /// </summary>
        public readonly ManifestStatus Status;
        
        /// <summary>
        ///     The manifest of this resource pack.
        /// </summary>
        public readonly PatcherManifest? Manifest;
        
        public PatcherResourcePack(IServiceProvider services, string path, BrandingType branding = BrandingType.None) : base(services, path, branding) {
            Manifest = PatcherManifest.FromFile(Path.Combine(path, "patcher.json"), out Status);
        }
    }
}