using EPiServer.Core;

namespace EpiserverSite5.Models.Pages
{
    public interface IHasRelatedContent
    {
        ContentArea RelatedContentArea { get; }
    }
}
