using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.ServiceLocation;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using EpiserverSite5.Services;

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
        public Injected<IMapsEditorRepository> MapsEditorRepository { get; set; }

        public override void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
        {
            base.ModifyMetadata(metadata, attributes);
            var savedConfig = MapsEditorRepository.Service.GetAllMapsEditorData().FirstOrDefault();
            metadata.EditorConfiguration["apiKey"] = savedConfig.ApiKey ?? "pk.eyJ1IjoibGVxdWFuZzEwMjQiLCJhIjoiY2tpaWxpNGNvMGFrYzJyb2QzNjJpOGR0diJ9.fwBENvuqXz1O3zrCzCYLcA";
            metadata.EditorConfiguration["defaultZoom"] = savedConfig.ZoomLevel != default ? savedConfig.ZoomLevel : 5;
            metadata.EditorConfiguration["styleUrl"] = savedConfig.StyleUrl ?? "mapbox://styles/mapbox/light-v10";
            metadata.EditorConfiguration["defaultCoordinates"] = new
            {
                latitude = savedConfig.DefaultLatitude != default ? savedConfig.DefaultLatitude : 21.002449485238547,
                longitude = savedConfig.DefaultLongitude != default ? savedConfig.DefaultLongitude : 105.80183683128283
            };
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
