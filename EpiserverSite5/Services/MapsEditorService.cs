using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Data;
using EPiServer.Data.Dynamic;
using EPiServer.ServiceLocation;

namespace EpiserverSite5.Services
{
    [EPiServerDataStore(AutomaticallyCreateStore = true, AutomaticallyRemapStore = true)]
    public class MapsEditorData
    {
        [EPiServerDataIndex]
        public Identity Id { get; set; }
        public MapsEditorService Service { get; set; }
        public string ApiKey { get; set; }
        public string StyleUrl { get; set; }
        public int ZoomLevel { get; set; }
        public double DefaultLatitude { get; set; }
        public double DefaultLongitude { get; set; }
    }

    public enum MapsEditorService
    {
        Mapbox,
        GoogleMaps
    }

    public interface IMapsEditorLoader
    {
        void Delete(Identity id);
        MapsEditorData GetMapsEditorData(Identity id);
        IList<MapsEditorData> GetAllMapsEditorData();
        void Save(MapsEditorData MapsEditorData);
    }

    [ServiceConfiguration(typeof(IMapsEditorLoader))]
    public class MapsEditorLoader : IMapsEditorLoader
    {
        private static DynamicDataStore MapsEditorStore
        {
            get
            {
                return typeof(MapsEditorData).GetStore();
            }
        }

        public void Delete(Identity id)
        {
            MapsEditorStore.Delete(id);
        }

        public MapsEditorData GetMapsEditorData(Identity id)
        {
            return MapsEditorStore.Items<MapsEditorData>().FirstOrDefault();
        }

        public virtual IList<MapsEditorData> GetAllMapsEditorData()
        {
            return MapsEditorStore.Items<MapsEditorData>().ToList();
        }

        public void Save(MapsEditorData MapsEditorData)
        {
            if (MapsEditorData == null)
            {
                return;
            }
            if (MapsEditorStore.Items().Count() > 1)
            {
                MapsEditorStore.DeleteAll();
            }
            MapsEditorStore.Save(MapsEditorData);
        }
    }
    public interface IMapsEditorRepository
    {
        void Delete(Identity id);

        IList<MapsEditorData> GetAllMapsEditorData();

        void Save(MapsEditorData MapsEditorData);
    }

    [ServiceConfiguration(typeof(IMapsEditorRepository))]
    public class MapsEditorRepository : IMapsEditorRepository
    {
        private readonly IMapsEditorLoader _MapsEditorLoader;


        public MapsEditorRepository(IMapsEditorLoader MapsEditorLoader)
        {
            if (MapsEditorLoader == null) throw new ArgumentNullException(nameof(MapsEditorLoader));

            _MapsEditorLoader = MapsEditorLoader;
        }

        public void Delete(Identity id)
        {
            _MapsEditorLoader.Delete(id);
        }

        public IList<MapsEditorData> GetAllMapsEditorData()
        {
            return _MapsEditorLoader.GetAllMapsEditorData();
        }

        public void Save(MapsEditorData MapsEditorData)
        {
            _MapsEditorLoader.Save(MapsEditorData);
        }
    }
}
