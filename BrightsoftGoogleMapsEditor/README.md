Google Maps editor for EPiServer 7.5
====================================

How to use (detailed): http://tedgustaf.com/blog/2014/10/google-maps-custom-editor-for-episerver-75/

1. Install nuget

2. Add the following to your content type

        public virtual MapPoint Coordinates { get; set; }


Source code: https://github.com/tedgustaf/episerver-google-maps-editor