using EPiServer.Find.Statistics.Api;
using EpiserverSite5.Models.ViewModels;
using System.Collections.Generic;
using System.Web;
using EPiServer.Find.Statistics;
using EpiserverSite5.Models.Pages;

namespace EpiserverSite5.Models.ViewModels
{
    public class GDPRDemoModel : PageViewModel<GDPRApiDemoPage>
    {
        public List<GDPRHitResult> GetHits { get; set; }

        public List<string> PlainTextFilterPatterns { get; set; }

        public List<string> WildcardFilterPatterns { get; set; }

        public List<string> RegexFilterPatterns { get; set; }

        public string DeleteStatus { get; set; }

        public string SaveStatus { get; set; }

        public string GDPRGetQuery
        {
            get { return (HttpContext.Current.Request.QueryString["q"] ?? string.Empty).Trim(); }
        }

        public string GDPRDeleteQuery
        {
            get { return (HttpContext.Current.Request.QueryString["qd"] ?? string.Empty).Trim(); }
        }

        public GDPRDemoModel(GDPRApiDemoPage currentPage) : base(currentPage)
        {
        }
    }
}
