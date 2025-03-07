using EPiServer.DataAnnotations;

namespace EpiserverSite5.Models
{
    /// <summary>
    /// Attribute used for site content types to set default attribute values
    /// </summary>
    public class SiteContentType : ContentTypeAttribute
    {
        public SiteContentType()
        {
            GroupName = Global.GroupNames.Default;
        }
    }
}
