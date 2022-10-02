using Rejuvena.ResourcePatcher.Common.Util;
using ReLogic.Content.Sources;
using Terraria.IO;

namespace Rejuvena.ResourcePatcher.Common.Data
{
    public class ContentSourceData
    {
        public ResourcePack? Pack { get; set; }
    }

    public static class ContentSourceDataExtensions
    {
        public static ContentSourceData GetData(this IContentSource contentSource) {
            return contentSource.GetDynamicField<IContentSource, ContentSourceData>("data");
        }
    }
}