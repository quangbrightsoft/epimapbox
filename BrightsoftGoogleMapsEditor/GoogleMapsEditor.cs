using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;
using System;
using System.Collections.Generic;

namespace BrightsoftGoogleMapsEditor
{
    public abstract class MapboxEditor : EditorDescriptor
    {
        public const string UIHint = "MapboxEditor";

        public override void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
        {
            ClientEditingClass = "brss-map-editor/Editor";
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