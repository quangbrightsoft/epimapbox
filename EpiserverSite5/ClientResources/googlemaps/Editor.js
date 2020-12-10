define([
    "dojo/on", // To connect events
    "dojo/_base/declare", // Used to declare the actual widget
    "dojo/_base/config", // Used to check if client code debugging is enabled
    "dojo/aspect", // Used to attach to events in an aspect-oriented way to inject behaviors
    "dojo/Deferred", // Used to allow code to wait for Google Maps to be loaded

    "dijit/registry", // Used to get access to other dijits in the app
    "dijit/WidgetSet", // To be able to use 'byClass' when querying the dijit registry
    "dijit/_Widget", // Base class for all widgets
    "dijit/_TemplatedMixin", // Widgets will be based on an external template (string literal, external file, or URL request)
    "dijit/_WidgetsInTemplateMixin", // The widget will in itself contain additional widgets

    "epi/epi", // For example to use areEqual to compare property values
    "epi/shell/widget/_ValueRequiredMixin", // In order to check if the property is in a readonly state
    "epi/shell/widget/dialog/LightWeight", // Used to display the help message

    "dojo/i18n!./nls/Labels", // Localization files containing translations

    "./Async",

    'xstyle/css!./WidgetTemplate.css'
],
function (
    on,
    declare,
    config,
    aspect,
    Deferred,

    registry,
    WidgetSet,
    _Widget,
    _TemplatedMixin,
    _WidgetsInTemplateMixin,

    epi,
    _ValueRequiredMixin,
    LightWeight,

    Labels,

    Async
) {
    return declare([_Widget, _TemplatedMixin, _WidgetsInTemplateMixin, _ValueRequiredMixin], {

        // The Google Maps object of this widget instance
        _map: null,

        // The map marker of this widget instance
        _marker: null,

        // Localizations able to be accessed from the template
        _localized: Labels,

        _helpDialog: null,

        _mapLoadPromise: null, // Promise resolved when Google Maps has been loaded

        // Property settings (set by editor descriptor)
        apiKey: null,
        defaultZoom: null,
        defaultCoordinates: null,

        // Load HTML template from the same location as the widget
        templateString: dojo.cache("brightsoftgooglemaps", "WidgetTemplate.html"),

        // Event used to notify EPiServer that the property value has changed
        onChange: function (value) {

        },

        // Dojo event fired after all properties of a widget are defined, but before the fragment itself is added to the main HTML document
        postCreate: function () {

            // Call base implementation of postCreate, passing on any parameters
            this.inherited(arguments);

            // When the editor switches tabs in the UI we trigger a map resize to ensure it aligns properly
            // Note: byClass requires dot notation
            var that = this; // Reduce number of scope binds
            registry.byClass("dijit.layout.TabContainer").forEach(function (tab, i) {
                aspect.after(tab, "selectChild", function () {
                    //that.alignMap();
                });
            });

            // Display help when help icon is clicked
            on(this.helpIcon, "click", function (e) {
                e.preventDefault();

                if (!that._helpDialog) {

                    that._helpDialog = new LightWeight({
                        style: "width: 540px",
                        closeIconVisible: true,
                        showButtonContainer: false,
                        onButtonClose: function () {
                            that._helpDialog.hide();
                        },
                        _endDrag: function () {
                            // HACK Avoid EPiServer bug, "Cannot read property 'userSetTransformId' of null" when close icon is clicked
                        },
                        title: that.localized.help.dialogTitle,
                        content: that.localized.help.dialogHtml
                    });
                }

                if (that._helpDialog.open) {
                    that._helpDialog.hide();
                } else {
                    that._helpDialog.show();
                }
            });
        },

        // Dojo event triggered after 'postCreate', for example when JS resizing needs to be done
        startup: function () {

            if (!this.apiKey) {
                console.warn("Google Maps API key not set, ensure custom editor setting 'apiKey' is set through editor descriptor");
            }

            const googleMapsScriptUrl = "https://maps.googleapis.com/maps/api/js?v=3.exp&libraries=places&key=" + this.apiKey;
            const gljsUrl = "https://api.mapbox.com/mapbox-gl-js/v2.0.0/mapbox-gl.js";
            const mapjsUrl = "https://api.mapbox.com/mapbox.js/v3.3.1/mapbox.js";
            
            this._mapLoadPromise = new Deferred();
            // Load Google Maps script
            Async.load(mapjsUrl, function () {
                this.initializeMap();

                // HACK Give EPiServer UI some time to load the view before resizing the map to ensure it aligns even after being hidden/displayed
                // TODO Replace with event/aspect for when the edit view changes in the UI

                setTimeout(function () {
                    //this.alignMap();
                }.bind(this), 250);

                this._mapScriptAdded = true;

                this._mapLoadPromise.resolve();
            }.bind(this));

        },

        // Dojo event triggered when widget is removed
        destroy: function () {
            this.inherited(arguments); // Important to ensure inherited widgets are destroyed properly, failure to do this risks memory leaks

            // Clean up Google Maps (as much as possible, but there is a known issue with Google Maps: https://code.google.com/p/gmaps-api-issues/issues/detail?id=3803)
            if (this._marker) {
                this._marker.setMap(null);
            }

            if (this._map && this._map.parentNode) {
                google.maps.event.clearListeners(this._map, 'rightclick');
                this._map.parentNode.removeChild(this._map);
                this._map = null;
            }
        },

        // Checks if the current property value is valid (invoked by EPiServer)
        isValid: function () {
            // summary:
            //    Check if widget's value is valid.
            // tags:
            //    protected, override

            if (!this.value || this.value === '' || this.value == undefined || (typeof this.value === "object" && (isNaN(this.value.longitude) || isNaN(this.value.latitude)))) {
                return !this.required; // Making use of _ValueRequiredMixin to check if a property value is required
            }

            if (typeof this.value === "string") {
                var coordinates = this.value.split(',');

                if (coordinates.length != 2) { // Valid value is longitude and latitude, separated by a comma
                    return false;
                }

                for (var i = 0; i < 1; i++) {
                    if (isNaN(coordinates[0]) || coordinates[0].toString().indexOf('.') == -1) {
                        return false; // The coordinate is not a float number
                    }
                }

                return true;
            } else if (typeof this.value === "object") { // Complex type, ensure coordinates are numbers
                var isValidCoordinatesObject = this.value.longitude !== undefined &&
                    this.value.latitude !== undefined &&
                    !isNaN(this.value.longitude) &&
                    !isNaN(this.value.latitude) &&
                    this.value.longitude !== 0 &&
                    this.value.latitude !== 0;

                return isValidCoordinatesObject;
            }

            return false;
        },

        // Checks if the current value is valid coordinates
        hasCoordinates: function () {

            if (!this.isValid() || !this.value || this.valueOf === '' || (typeof this.value === "object" && (isNaN(this.value.longitude) || isNaN(this.value.latitude)))) {
                return false;
            }

            if (typeof this.value === "string") {
                return this.value.split(',').length == 2; // String value with comma-separated coordinates
            } else if (typeof this.value === "object") { // Complex type with separate properties for latitude and longitude
                return this.value.longitude !== undefined &&
                    this.value.latitude !== undefined &&
                    !isNaN(this.value.longitude) &&
                    !isNaN(this.value.latitude) &&
                    this.value.longitude !== 0 &&
                    this.value.latitude !== 0;
            }

            return false;
        },

        // Determine if the property is complex type, such as a local block with separate properties for longitude and latitude, as opposed to a simple string property
        _isComplexType: function (value) {

            var valueToCheck = value;

            if (!valueToCheck) {
                valueToCheck = this.value;
            }

            if (valueToCheck) {
                return typeof valueToCheck === "object";
            }

            if (Array.isArray(this.properties)) {
                return this.properties.length > 0;
            }

            if (this.metadata && Array.isArray(this.metadata.properties)) {
                return this.metadata.properties.length > 0;
            }

            return false;
        },

        // Setter for value property (invoked by EPiServer on load)
        _setValueAttr: function (value) {

            // Skip if the new property value is identical to the current one
            if (value === this.value) {
                return;
            }

            // Update the widget (i.e. property) value
            this._set("value", value);

            // Ensure Google Maps is loaded
            this._mapLoadPromise.then(function () {
                // If the value set is empty then clear the coordinates
                if (!value) {
                    this.clearCoordinates();
                    return;
                }



                var location;
                if (this._isComplexType()) {
                    location = new google.maps.LatLng(value.latitude, value.longitude);
                } else {
                    var coordinates = value.split(",");
                    location = new google.maps.LatLng(parseFloat(coordinates[0]), parseFloat(coordinates[1]));
                }

                this.setMapLocation(location, null, true);
            }.bind(this));

        },

        // Update widget value when a coordinate is changed
        _onCoordinateChanged: function () {

            if (!this._started) {
                return;
            }

            var longitude = this.longitudeTextbox.get('value');
            var latitude = this.latitudeTextbox.get('value');

            if (longitude === undefined || latitude === undefined) {
                return;
            }

            // Get the new value in the correct format
            var value;
            if (this._isComplexType()) {
                value = { latitude: parseFloat(latitude), longitude: parseFloat(longitude) };
            } else {
                value = latitude + "," + longitude;
            }

            // Set the widget (i.e. property) value and trigger change event to notify EPiServer (and possibly others) that the value has changed
            this._set("value", value);
            this.onChange(value);
        },

        // Clears the coordinates, i.e. the property value (the clear button's click event is wired up through a 'data-dojo-attach-event' attribute in the HTML template)
        clearCoordinates: function () {

            // Clear coordinate checkboxes
            this.longitudeTextbox.set('value', '');
            this.latitudeTextbox.set('value', '');

            // Clear search box
            this.searchTextbox.set("value", '');

            // Remove the map marker, if any
            if (this._marker) {
                this._marker.setMap(null);
                this._marker = null;
            }

            // Null the widget (i.e. property) value and trigger change event to notify EPiServer (and possibly others) that the value has changed
            this._set("value", null);
            this.onChange(null);
        },

        // Setup the Google Maps canvas
        initializeMap: function () {

          
            // Center on current coordinates (i.e. property value), or a default location if no coordinates are set
            if (this.hasCoordinates()) {
                if (typeof this.value === "string") {
                    var coordinates = this.value.split(',');
                    defaultCoordinates = new L.LatLng(parseFloat(coordinates[0]), parseFloat(coordinates[1]));
                }
                else if (typeof this.value === "object") {
                    defaultCoordinates = new L.LatLng(this.value.latitude, this.value.longitude);
                }
            }

            // Render the map, but disable interaction if property is readonly
            var mapOptions = {
                zoom: this.defaultZoom,
                disableDefaultUI: true,
                disableDoubleClickZoom: this.readOnly,
                scrollwheel: !this.readOnly,
                draggable: !this.readOnly
            };


            //L.mapbox.accessToken = 'sk.eyJ1IjoibGVxdWFuZzEwMjQiLCJhIjoiY2tpaWxsczc1MDJnOTJwcWx1c3F5OWhpMyJ9.VnPV3Y0ZWLTpZaeK8_Lojg';
            L.mapbox.accessToken = 'pk.eyJ1IjoibGVxdWFuZzEwMjQiLCJhIjoiY2tpaWxpNGNvMGFrYzJyb2QzNjJpOGR0diJ9.fwBENvuqXz1O3zrCzCYLcA';
            this._map = L.mapbox.map('map')
                .setView([this.defaultCoordinates.latitude, this.defaultCoordinates.longitude], 9)
                .addLayer(L.mapbox.styleLayer('mapbox://styles/mapbox/streets-v11'))


            //// Load the map
            //this._map = new google.maps.Map(this.canvas, mapOptions);

            //// Display grayscale map if property is readonly
            //if (this.readOnly) {

            //    var grayStyle = [{
            //        featureType: "all",
            //        elementType: "all",
            //        stylers: [{ saturation: -100 }]
            //    }];

            //    var mapType = new google.maps.StyledMapType(grayStyle, { name: "Grayscale" });

            //    this._map.mapTypes.set('disabled', mapType);

            //    this._map.setMapTypeId('disabled');
            //}

            //// Add a marker to indicate the current coordinates, if any
            //if (this.hasCoordinates()) {
            //    this._marker = new google.maps.Marker({
            //        position: this._map.getCenter(),
            //        map: this._map
            //    });
            //}

            //// Allow user to change coordinates unless property is readonly
            if (!this.readOnly) {

                var that = this;
                this._map.on('click', function (e) {
                    that.setMapLocation(e.latlng, null, false);
                });
                // Update map marker and coordinate textboxes when map is right-clicked
              //L.event.addListener(this._map, "rightclick", function (event) {
              //      that.setMapLocation(event.latLng, null, false);
              //  });

              //  // Add search textbox and when a place is selected, move pin and center map
              //  var searchBox = new google.maps.places.SearchBox(this.searchTextbox.textbox);

              //  // Remove Google Maps Searchbox default placeholder, as it won't recognize the placeholder attribute placed on the Textbox dijit
              //  this.searchTextbox.textbox.setAttribute('placeholder', '');

              // L.event.addListener(searchBox, 'places_changed', function () {
              //      var places = searchBox.getPlaces();

              //      if (places.length == 0) {
              //          return;
              //      }
              //      // Return focus to the textbox to ensure autosave works correctly and to also give a nice editor experience
              //      that.searchTextbox.focus();
              //      that.setMapLocation(places[0].geometry.location, 15, true);
              //  });
            } else {
                // Disable search box and clear button
                this.searchTextbox.set("disabled", true);
                this.clearButton.set("disabled", true);
            }
        },

        // Triggers a map resize, for example when the map is hidden/displayed to ensure it aligns properly
        alignMap: function () {
            var center = this._map.getCenter();
            google.maps.event.trigger(this._map, "resize");
            this._map.setCenter(center);
        },

        // Updates map marker location, centers on it (optional), sets the zoom level (optional) and updates coordinate textboxes for longitude and latitude values
        setMapLocation: function (/* google.maps.LatLng */ location, zoom, center) {

            // Set the values of the coordinate textboxes to longitude and latitude, respectively
            this.longitudeTextbox.set('value', location.lng());
            this.latitudeTextbox.set('value', location.lat());

            // Set the marker's position
            if (!this._marker) { // No marker yet, create one
                this._marker = new L.Marker({
                    map: this._map
                });
            }

            this._marker.setPosition(location);

            // Center on the location (optional)
            if (center) {
                this._map.setCenter(location);
            }

            // Set map zoom level (optional)
            if (zoom) {
                this._map.setZoom(zoom);
            }

            // Trigger event to update the widget (i.e. property) value
            this._onCoordinateChanged();
        }
    });
});