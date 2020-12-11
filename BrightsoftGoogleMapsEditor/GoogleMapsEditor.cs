using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;
using System;
using System.Collections.Generic;

namespace EPiServer.GoogleMapsEditor
{
    [EditorDescriptorRegistration(TargetType = typeof(string), UIHint = UIHint, EditorDescriptorBehavior = EditorDescriptorBehavior.OverrideDefault)]
    public abstract class MapboxEditor : EditorDescriptor
    {
        public const string UIHint = "MapboxEditor";

        protected MapboxEditor() { }

        public override void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
        {
            ClientEditingClass = "epimapbox/Editor";
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