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

    }

    [EditorDescriptorRegistration(TargetType = typeof(string), UIHint = "MapboxEditor",
        EditorDescriptorBehavior = EditorDescriptorBehavior.OverrideDefault)]
    public class CustomGoogleMapsEditorDescriptor : MapboxEditor
    {
        public override void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
        {
            base.ModifyMetadata(metadata, attributes);
        }
    }

    public abstract class MapboxEditor : EditorDescriptor
    {
        private Injected<IMapsEditorRepository> MapsEditorRepository { get; set; }

        public override void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
        {
            base.ModifyMetadata(metadata, attributes);
            var savedConfig = MapsEditorRepository.Service.GetAllMapsEditorData().FirstOrDefault();
            switch (savedConfig?.Service)
            {
                case null:
                case MapsEditorService.Mapbox:
                    ClientEditingClass = "brss/Editor";
                    break;
                case MapsEditorService.GoogleMaps:
                    ClientEditingClass = "brss/GoogleMapsEditor";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            metadata.EditorConfiguration.Add("apiKey", savedConfig?.ApiKey ??
                                                       "pk.eyJ1IjoibGVxdWFuZzEwMjQiLCJhIjoiY2tpaWxpNGNvMGFrYzJyb2QzNjJpOGR0diJ9.fwBENvuqXz1O3zrCzCYLcA");
            metadata.EditorConfiguration.Add("defaultZoom",
                savedConfig?.ZoomLevel ?? 5);
            metadata.EditorConfiguration.Add("styleUrl", savedConfig?.StyleUrl ?? "mapbox://styles/mapbox/light-v10");
            metadata.EditorConfiguration.Add("defaultCoordinates", new
            {
                latitude =
                    savedConfig?.DefaultLatitude ?? 21.002449485238547,
                longitude = savedConfig?.DefaultLongitude ?? 105.80183683128283
            });
        }
    }
}