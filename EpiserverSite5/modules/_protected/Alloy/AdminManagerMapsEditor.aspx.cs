// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using EPiServer;
using EPiServer.PlugIn;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EpiserverSite5.Services;

namespace EpiserverSite5.modules._protected.Alloy
{
    [GuiPlugIn(Area = PlugInArea.AdminMenu,
        DisplayName = "Map editor Settings",
        Description = "Map editor Settings",
        UrlFromModuleFolder = "episerver/Alloy/AdminManagerMapsEditor.aspx",
        RequiredAccess = AccessLevel.Administer)]
    public partial class AdminManageSitemap : SimplePage
    {
        public Injected<IMapsEditorRepository> MapsEditorRepository { get; set; }

        public object SiteHosts { get; private set; }

        protected MapsEditorData CurrentMapsEditorData
        {
            get { return GetDataItem() as MapsEditorData; }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            var ddl = GeneralSettings.FindControl("availableServices") as DropDownList;
            var apiKey = GeneralSettings.FindControl("apiKey") as TextBox;
            var styleUrl = GeneralSettings.FindControl("styleUrl") as TextBox;
            var savedValues = MapsEditorRepository.Service.GetAllMapsEditorData().FirstOrDefault() ??
                              new MapsEditorData();

            //var isLongValid = double.TryParse(defaultLongitude.Text,out var longitude);
            //var isLatValid = double.TryParse(defaultLatitude.Text,out var latitude);
            //if (!isLongValid || !isLatValid)
            //{
            //    ValidationSummary.te
            //}

            savedValues.Service = (MapsEditorService) Enum.Parse(typeof(MapsEditorService), ddl.SelectedValue);
            savedValues.ApiKey = apiKey.Text;
            savedValues.StyleUrl = styleUrl.Text;
            savedValues.ZoomLevel = int.Parse(zoomLevel.Text);
            savedValues.DefaultLatitude = double.Parse(defaultLatitude.Text);
            savedValues.DefaultLongitude = double.Parse(defaultLongitude.Text);

            MapsEditorRepository.Service.Save(savedValues);
            BindList();
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            MasterPageFile = ResolveUrlFromUI("MasterPages/EPiServerUI.master");
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            PopulateServiceListControl();
            PopulateStyleUrlSuggestionControl();
            if (!IsPostBack)
            {
                BindList();
            }

            SystemPrefixControl.Heading = "Bright software Editor settings";
        }

        private void BindList()
        {
            var savedValues = MapsEditorRepository.Service.GetAllMapsEditorData().FirstOrDefault();
            if (savedValues == default) return;
            availableServices.SelectedValue = savedValues?.Service.ToString() ?? "";
            styleUrl.Text = savedValues.StyleUrl;
            apiKey.Text = savedValues.ApiKey;
            zoomLevel.Text = savedValues.ZoomLevel.ToString();
            defaultLongitude.Text = savedValues.DefaultLongitude.ToString(CultureInfo.InvariantCulture);
            defaultLatitude.Text = savedValues.DefaultLatitude.ToString(CultureInfo.InvariantCulture);
            if (availableServices.SelectedValue == MapsEditorService.Mapbox.ToString())
            {
                StyleUrlRow.Visible = true;
            }
        }

        private void PopulateServiceListControl()
        {
            var ddl = GeneralSettings.FindControl("availableServices") as DropDownList;

            if (ddl != null)
            {
                ddl.DataSource = new List<string> {"GoogleMaps", "Mapbox"};
                ddl.DataBind();
                ddl.Visible = true;
            }
        }

        private void PopulateStyleUrlSuggestionControl()
        {
            var ddl = GeneralSettings.FindControl("styleUrlSuggestion") as DropDownList;

            if (ddl != null)
            {
                ddl.DataSource = new List<string>
                {
                    "mapbox://styles/mapbox/streets-v11",
                    "mapbox://styles/mapbox/outdoors-v11",
                    "mapbox://styles/mapbox/light-v10",
                    "mapbox://styles/mapbox/dark-v10",
                    "mapbox://styles/mapbox/satellite-v9",
                    "mapbox://styles/mapbox/satellite-streets-v11"
                };
                ddl.DataBind();
                ddl.Visible = true;
            }
        }

        protected void myListDropDown_Change(object sender, EventArgs e)
        {
            var ddlListFind = (DropDownList)sender;
            styleUrl.Text = ddlListFind.Text;
        }

        protected void availableServices_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            var dropDownList = (DropDownList) sender;
            if (dropDownList.SelectedItem.Value == MapsEditorService.Mapbox.ToString())
            {
                StyleUrlRow.Visible = true;
            }
        }
    }
}