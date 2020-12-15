using EPiServer.Core;
using EPiServer.PlugIn;

namespace BrightsoftGoogleMapsEditor
{
    [PropertyDefinitionTypePlugIn]
    public class MapCoordinatesProperty : PropertyList<string>
    {
        public override PropertyData ParseToObject(string value)
        {
            throw new System.NotImplementedException();
        }

        protected override string ParseItem(string value)
        {
            throw new System.NotImplementedException();
        }
    }
}