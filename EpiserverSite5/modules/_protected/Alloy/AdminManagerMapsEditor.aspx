<%@ Page Language="C#" AutoEventWireup="False" CodeBehind="AdminManagerMapsEditor.aspx.cs" EnableViewState="true" Inherits="EpiserverSite5.modules._protected.Alloy.AdminManageSitemap" %>

<%@ Register TagPrefix="EPiServerUI" Namespace="EPiServer.UI.WebControls" Assembly="EPiServer.UI" %>

<%--<%@ Import Namespace="Google" %>--%>

<asp:Content ContentPlaceHolderID="FullRegion" runat="server">
    <div class="epi-contentContainer epi-padding">
        <div class="epi-contentArea">
            <EPiServerUI:SystemPrefix ID="SystemPrefixControl" runat="server"/>
            <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="EP-validationSummary" ForeColor="Black"/>
            <%= AntiForgery.GetHtml() %>
        </div>
        <style type="text/css">
            a.add-button {
                color: black;
            }

            table.sitemaps th {
                padding: 4px;
            }

            table.sitemaps td {
                padding: 7px;
            }

                table.sitemaps td input[type=text] {
                    width: 100%;
                }

                table.sitemaps td.sitemap-name input[type=text] {
                    width: 50%;
                }

            div.help {
                padding-top: 20px;
                padding-bottom: 10px;
            }

            div.toolbar {
                padding-bottom: 10px;
                height: 30px;
            }

            div.bottom-text {
                padding-top: 15px;
            }

            span.nb-text {
                font-weight: bold;
            }
        </style>

        <div>Maps editor service configuration</div>

        <div class="help">
            <div>
                <span class="nb-text">Service:</span>
                The Map editor UI can be one of 2 services: Mapbox and Google maps
            </div>

        </div>

        <asp:Panel ID="TabView" runat="server">
            <asp:Panel ID="GeneralSettings" runat="server">
                <div class="epi-formArea epi-padding">
                    <div class="epi-size15">
                        <div class="epi-indent">
                        </div>
                        <div>
                            <asp:Label runat="server" AssociatedControlID="availableServices" Text="Service"/>
                            <asp:DropDownList ID="availableServices" AutoPostBack="True" runat="server" OnSelectedIndexChanged="availableServices_OnSelectedIndexChanged"/>
                            <asp:RequiredFieldValidator ID="availableServicesRequired" ControlToValidate="availableServices" Text="*" EnableClientScript="true" runat="server"/>
                        </div>

                        <div>
                            <asp:Label runat="server" AssociatedControlID="apiKey" Text="Api key"/>
                            <asp:TextBox runat="server" ID="apiKey" Text='<%# CurrentMapsEditorData.ApiKey %>'/>
                            <asp:RequiredFieldValidator ID="apiKeyRequired" ControlToValidate="apiKey" Text="*" EnableClientScript="true" runat="server"/>
                        </div>
                        <div runat="server" id="StyleUrlRow" Visible="false">
                            <asp:Label runat="server" AssociatedControlID="styleUrl" Text="Style Url"/>
                            <asp:TextBox runat="server" ID="styleUrl" Text='<%# CurrentMapsEditorData.StyleUrl %>'/>
                            <asp:DropDownList AutoPostBack="true" ID="styleUrlSuggestion" runat="server" OnSelectedIndexChanged="myListDropDown_Change"/>
                        </div>
                        <div>
                            <asp:Label runat="server" AssociatedControlID="defaultLatitude" Text="Default latitude" type="number" step="any"/>
                            <asp:TextBox runat="server" ID="defaultLatitude" Text='<%# CurrentMapsEditorData.DefaultLatitude %>'/>
                            <asp:RequiredFieldValidator ID="defaultLatitudeRequired" ControlToValidate="defaultLatitude" Text="*" EnableClientScript="true" runat="server"/>
                        </div>
                        <div>
                            <asp:Label runat="server" AssociatedControlID="defaultLongitude" Text="Default longitude" type="number" step="any"/>
                            <asp:TextBox runat="server" ID="defaultLongitude" Text='<%# CurrentMapsEditorData.DefaultLongitude %>'/>
                            <asp:RequiredFieldValidator ID="defaultLongitudeRequired" ControlToValidate="defaultLongitude" Text="*" EnableClientScript="true" runat="server"/>
                        </div>
                        <div>
                            <asp:Label runat="server" AssociatedControlID="zoomLevel" Text="Zoom level"/>
                            <asp:TextBox runat="server" ID="zoomLevel" Text='<%# CurrentMapsEditorData.ZoomLevel %>' type="number"/>
                            <asp:RequiredFieldValidator ID="zoomLevelRequired" ControlToValidate="zoomLevel" Text="*" EnableClientScript="true" runat="server"/>
                        </div>
                        <div>
                            <asp:Label runat="server" AssociatedControlID="markerIconUrl" Text="Marker icon Url"/>
                            <asp:TextBox runat="server" ID="markerIconUrl" Text='<%# CurrentMapsEditorData.MarkerIconUrl %>' type="text"/>
                            <asp:RegularExpressionValidator
                                ID="markerUrlValidator"
                                runat="server"
                                ValidationExpression="http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?"
                                ControlToValidate="markerIconUrl"
                                ErrorMessage="Input valid Internet URL!">
                            </asp:RegularExpressionValidator>
                        </div>
                        <div>
                            <asp:Button ID="btnNew" runat="server" Text="Save" OnClick="btnSave_Click"
                                        CssClass="add-button epi-cmsButton-text epi-cmsButton-tools epi-cmsButton-Add"/>
                        </div>
                    </div>
                </div>
                <div class="epi-buttonContainer">
                </div>
            </asp:Panel>

        </asp:Panel>
    </div>
</asp:Content>