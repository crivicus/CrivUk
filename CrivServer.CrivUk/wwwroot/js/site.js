// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Blazy code - for image lazy loading
(function () {
    // Initialize
    var bLazy = new Blazy({
        breakpoints: [{
            width: 420 // max-width
            , src: 'data-src-small'
        }
            , {
            width: 768 // max-width
            , src: 'data-src-medium'
        }]
    });
})();