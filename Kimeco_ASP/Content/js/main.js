$(document).ready(function () {
    $('.owl-carousel').owlCarousel({
        loop: true,
        autoplay: true,
        autoplayTimeout: 5000,
        margin: 10,
        responsiveClass: true,
        responsive: {
            0: {
                items: 1,
                nav: true
            },
            600: {
                items: 3,
                nav: false
            },
            1000: {
                items: 5,
                nav: true,
                loop: false
            }
        }
    })
    setInterval(function () {
        if (!$(".timeline-nav-button--next").is(":disabled")) {
            $('.timeline-nav-button--next').click();
        }
        else {
            $('.timeline-nav-button--prev').click();
        }
    }, 3000)
    $(window).scroll(function () {
        if ($(this).scrollTop() > 100) {
            $('#headerNav .nav-item .nav-link').addClass('text-white');
            $("#headerNav").addClass('fixed-top');
            
            $("#headerNav").addClass('padding0-tb');
            $("#headerNav").addClass('background-blue-0p7');
            $("#navbarSupportedContent").addClass('padding0-tb');
        } else {
            console.log($(window).width);
            $('#headerNav .nav-item .nav-link').removeClass('text-white');
            if (window.matchMedia('(max-width: 991px)').matches) {
                $("#headerNav").removeClass('fixed-top');
            }
            $('#headerNav').removeClass('padding0-tb');
            $("#headerNav").removeClass('background-blue-0p7');
            $('#navbarSupportedContent').removeClass('padding0-tb');
        }
    });
    // scroll body to 0px on click
    $('#back-to-top').click(function () {
        $('body,html').animate({
            scrollTop: 0
        }, 2000);
        return false;
    });
    $('#back-to-bottom').click(function () {
        $('body,html').animate({
            scrollTop: $(document).height()
        }, 2000);
        return false;
    });
    $(".scrollTo").click(function () {

        var target = $(this).data("scroll");
        $('body, html').animate({
            scrollTop: $(target).offset().top
        }, 1000);
    });
    loadGallery(true, 'a.thumbnail');

    //This function disables buttons when needed
    function disableButtons(counter_max, counter_current) {
        $('#show-previous-image, #show-next-image').show();
        if (counter_max == counter_current) {
            $('#show-next-image').hide();
        } else if (counter_current == 1) {
            $('#show-previous-image').hide();
        }
    }

    function loadGallery(setIDs, setClickAttr) {
        var current_image,
            selector,
            counter = 0;

        $('#show-next-image, #show-previous-image').click(function () {
            if ($(this).attr('id') == 'show-previous-image') {
                current_image--;
            } else {
                current_image++;
            }

            selector = $('[data-image-id="' + current_image + '"]');
            updateGallery(selector);
        });

        function updateGallery(selector) {
            var $sel = selector;
            current_image = $sel.data('image-id');
            $('#image-gallery-caption').text($sel.data('caption'));
            $('#image-gallery-title').text($sel.data('title'));
            $('#image-gallery-image').attr('src', $sel.data('image'));
            disableButtons(counter, $sel.data('image-id'));
        }

        if (setIDs == true) {
            $('[data-image-id]').each(function () {
                counter++;
                $(this).attr('data-image-id', counter);
            });
        }
        $(setClickAttr).on('click', function () {
            updateGallery($(this));
        });
    }
});
(function ($) {

    /**
     * Copyright 2012, Digital Fusion
     * Licensed under the MIT license.
     * http://teamdf.com/jquery-plugins/license/
     *
     * @author Sam Sehnert
     * @desc A small plugin that checks whether elements are within
     *     the user visible viewport of a web browser.
     *     only accounts for vertical position, not horizontal.
     */

    $.fn.visible = function (partial) {

        var $t = $(this),
            $w = $(window),
            viewTop = $w.scrollTop(),
            viewBottom = viewTop + $w.height(),
            _top = $t.offset().top,
            _bottom = _top + $t.height(),
            compareTop = partial === true ? _bottom : _top,
            compareBottom = partial === true ? _top : _bottom;

        return ((compareBottom <= viewBottom) && (compareTop >= viewTop));

    };

})(jQuery);
$(window).scroll(function (event) {

    $(".module").each(function (i, el) {
        var el = $(el);
        if (el.visible(true)) {
            el.addClass("come-in");
        }
    });

});
var win = $(window);
var allMods = $(".module");

// Already visible modules
allMods.each(function (i, el) {
    var el = $(el);
    if (el.visible(true)) {
        el.addClass("already-visible");
    }
});

win.scroll(function (event) {

    allMods.each(function (i, el) {
        var el = $(el);
        if (el.visible(true)) {
            el.addClass("come-in");
        }
    });

});
timeline(document.querySelectorAll('.timeline'), {
    forceVerticalMode: 700,
    mode: 'horizontal',
    verticalStartPosition: 'left',
    visibleItems: 3,
    forceVerticalMode: 600
});
$(function () {
    $('[data-toggle="tooltip"]').tooltip()
})