namespace Rejuvena.ResourcePatcher.API
{
    /// <summary>
    ///     The version status of a <see cref="PatcherManifest"/> instance.
    /// </summary>
    public enum ManifestStatus
    {
        /// <summary>
        ///     The manifest is of an expected version.
        /// </summary>
        Ok,
        
        /// <summary>
        ///     The manifest version is too new, and won't work with this version of the mod.
        /// </summary>
        TooNew,
        
        /// <summary>
        ///     The manifest version is too old, and won't work with this version of the mod.
        /// </summary>
        TooOld,
        
        /// <summary>
        ///     The manifest version is unknown - something went wrong.
        /// </summary>
        Unknown
    }
}