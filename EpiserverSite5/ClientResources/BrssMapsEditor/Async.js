/** @license
 * RequireJS plugin for async dependency load like JSONP and Google Maps
 * Author: Miller Medeiros
 * Version: 0.1.2 (2014/02/24)
 * Released under the MIT license
 * https://github.com/millermedeiros/requirejs-plugins/blob/master/src/async.js
 */

/* Modified by Ted Nyberg (@tednyberg) to be used in Dojo widgets ('config' parameter excluded as Dojo doesn't call modules this way) */

define(function () {

    var _uid = 0;

    function injectScript(src,callback) {
        var s, t;
        s = document.createElement('script');
        s.type = 'text/javascript';
        s.async = true;
        s.onload = callback;
        s.src = src;
        
        t = document.getElementsByTagName('script')[0];
        t.parentNode.insertBefore(s, t);
    }

    function uid() {
        _uid += 1;
        return '__async_req_' + _uid + '__';
    }

    return {
        load: function (url, callback) {
            var id = uid();
            //create a global variable that stores onLoad so callback
            //function can define new module after async load
            window[id] = callback;

            // Keep track of whether this script has already been loaded
            if (window._asyncScripts && window._asyncScripts[url]) { // Already loaded
                callback();
            }
            else {
                // Add to list of loaded scripts and then add script tag
                if (!window._asyncScripts) {
                    window._asyncScripts = {};
                }

                window._asyncScripts[url] = true;

                injectScript(url, window[id]);
            }
        }
    };
});