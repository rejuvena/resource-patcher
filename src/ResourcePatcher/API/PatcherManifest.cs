using System;
using System.IO;
using Newtonsoft.Json;
using Terraria.ModLoader;

namespace Rejuvena.ResourcePatcher.API
{
    public class PatcherManifest
    {
        [JsonProperty("version")]
        public string Version { get; set; }

        public static PatcherManifest? FromFile(string path, out ManifestStatus status) {
            status = ManifestStatus.Unknown;

            if (!File.Exists(path)) return null;

            PatcherManifest? manifest;
            try {
                manifest = JsonConvert.DeserializeObject<PatcherManifest>(File.ReadAllText(path), settings: null);
                if (manifest is null) return null;
                if (!System.Version.TryParse(manifest.Version, out Version? vers)) return manifest;

                Version modVer = ModContent.GetInstance<ResourcePatcher>().Version;
                status = modVer.Major > vers.Major ? ManifestStatus.TooOld : modVer.Major < vers.Major ? ManifestStatus.TooNew : ManifestStatus.OK;
            }
            catch {
                manifest = null;
            }

            return manifest;
        }
    }
}