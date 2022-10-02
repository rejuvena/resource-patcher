using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Terraria.ModLoader;

namespace Rejuvena.ResourcePatcher.API
{
    /// <summary>
    ///     Contains core manifest data for a resource pack that uses Resource Patcher.
    /// </summary>
    public class PatcherManifest
    {
        /// <summary>
        ///     The Resource Patcher this resource pack was made for.
        /// </summary>
        [JsonRequired]
        [JsonProperty("version")]
        public string Version { get; set; } = "";

        /// <summary>
        ///     Maps mod names to directories.
        /// </summary>
        [JsonProperty("nameMap")]
        public Dictionary<string, (string modName, string modDir)> NameMap { get; set; } = new();

        public string GetTerrariaDirectory() {
            return NameMap.ContainsKey("Terraria") ? NameMap["Terraria"].modName : "";
        }

        public string GetTerrariaPath(string path) {
            return Path.Combine(GetTerrariaDirectory(), path);
        }

        public static PatcherManifest? FromFile(string path, out ManifestStatus status) {
            status = ManifestStatus.Unknown;

            if (!File.Exists(path)) return null;

            PatcherManifest? manifest;
            try {
                manifest = JsonConvert.DeserializeObject<PatcherManifest>(File.ReadAllText(path), settings: null);
                if (manifest is null) return null;
                if (!System.Version.TryParse(manifest.Version, out Version? vers)) return manifest;

                Version modVer = ModContent.GetInstance<ResourcePatcher>().Version;
                status = modVer.Major > vers.Major ? ManifestStatus.TooOld : modVer.Major < vers.Major ? ManifestStatus.TooNew : ManifestStatus.Ok;
            }
            catch {
                manifest = null;
            }

            return manifest;
        }
    }
}