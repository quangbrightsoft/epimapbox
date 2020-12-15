using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EpiserverSite5.Models.Pages
{
    /// <summary>
    /// Used for the pages mainly consisting of manually created content such as text, images, and blocks
    /// </summary>
    [SiteContentType(GUID = "9CCC8A41-5C8C-4BE0-8E73-520FF3DE8267")]
    [SiteImageUrl(Global.StaticGraphicsFolderPath + "page-type-thumbnail-standard.png")]
    public class StandardPage : SitePageData
    {
        [Display(
            GroupName = SystemTabNames.Content,
            Order = 310)]
        [CultureSpecific]
        public virtual XhtmlString MainBody { get; set; }

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 320)]
        public virtual ContentArea MainContentArea { get; set; }

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 320)]
        [UIHint("MapboxEditor")]
        public virtual string Coordinates { get; set; }
    }

    [EditorDescriptorRegistration(TargetType = typeof(string), UIHint = UIHint, EditorDescriptorBehavior = EditorDescriptorBehavior.OverrideDefault)]
    public class CustomGoogleMapsEditorDescriptor : MapboxEditor
    {
        public override void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
        {
            base.ModifyMetadata(metadata, attributes);
            // API key for the Google Maps JavaScript API
            metadata.EditorConfiguration["apiKey"] = "AIzaSyBq1SPFg-CEcwMmBS03NS6Ofsk68gPQgsE";

            // Default zoom level from 1 (least) to 20 (most)
            metadata.EditorConfiguration["defaultZoom"] = 5;

            // Default coordinates when no property value is set
            metadata.EditorConfiguration["defaultCoordinates"] = new { latitude = 21.002449485238547, longitude = 105.80183683128283 };
        }
    }
    public abstract class MapboxEditor : EditorDescriptor
    {
        public const string UIHint = "MapboxEditor";

        public override void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
        {
            ClientEditingClass = "brss/Editor";
            metadata.EditorConfiguration.Add("apiKey", "AIzaSyCCkZI3w943tyqyZCkbbEB2Pl1W0eH1WPc");
            metadata.EditorConfiguration.Add("defaultZoom", 5);
            metadata.EditorConfiguration.Add("defaultCoordinates", new
            {
                latitude = 59.336,
                longitude = 18.063
            });
            base.ModifyMetadata(metadata, attributes);
        }
    }
}
