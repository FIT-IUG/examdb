
$(window).on('load', function () {
    // animate login & signup fields placeholder
    $('.login_form, .signup_form, .upload_file_modal, .profile_modal').find('input:not([type="submit"]), textarea, select').on('keyup  blur focus', function (e) {

        var $this = $(this),
            label = $this.prev('label');
        if (e.type === 'keyup' || e.type === 'keydown') {
            if ($this.val() === '') {
                $(this).css({ "height": "44px", "padding": "5px 12px" });
                label.removeClass('active highlight');
            } else {
                $(this).css({ "height": "65px", "padding": "20px 12px 5px 12px" });
                label.addClass('active highlight');
            }
        } else if (e.type === 'blur') {
            if ($this.val() === '') {
                $(this).css({ "height": "44px", "padding": "5px 12px" });
                label.removeClass('active highlight');
            } else {
                $(this).css({ "height": "65px", "padding": "20px 12px 5px 12px" });
                label.removeClass('highlight');
            }
        } else if (e.type === 'focus') {
            if ($this.val() === '') {
                $(this).css({ "height": "44px", "padding": "5px 12px" });
                label.removeClass('highlight');
            }
            else if ($this.val() !== '') {
                $(this).css({ "height": "65px", "padding": "20px 12px 5px 12px" });
                label.addClass('highlight');
            }
        }
    });

    // animate login & signup fields placeholder (fix for reload) 
    $('.login_form, .signup_form, .profile_modal').find('input:not([type="submit"]), textarea, select').each(function () {
        var $this = $(this),
            label = $this.prev('label');
        if ($this.val() === '') {
            $(this).css({ "height": "44px", "padding": "5px 12px" });
            label.removeClass('active highlight');
        } else {
            $(this).css({ "height": "65px", "padding": "20px 12px 5px 12px" });
            label.addClass('active highlight');
        }
    });


    /* Open & Close Navigation */
    $('.navbar .bars, .login_page_navigation .bars').on("click", function () {
        $('.navbar .bars .bar, .login_page_navigation .bars .bar').toggleClass('bar-close');
        $('.navbar_circle').toggleClass('navbar_circle_show');
        
        if ($('html').css('direction') == 'rtl') {
            $('.navbar_circle ol li#navbar_animate_1').toggleClass('animated fadeInLeft');
            $('.navbar_circle ol li#navbar_animate_2').toggleClass('animated fadeInLeft delay-hs');
            $('.navbar_circle ol li#navbar_animate_3').toggleClass('animated fadeInLeft delay-s');
        } else {
            $('.navbar_circle ol li#navbar_animate_1').toggleClass('animated fadeInRight');
            $('.navbar_circle ol li#navbar_animate_2').toggleClass('animated fadeInRight delay-hs');
            $('.navbar_circle ol li#navbar_animate_3').toggleClass('animated fadeInRight delay-s');
        }
        
    });

    

    $(document).mouseup(function (e) {
        var container = $(".navbar_circle");
        var closeBtn = $('.navbar .bars, .login_page_navigation .bars');
        var isOpen = $('.navbar_circle').hasClass('navbar_circle_show');
        // if the target of the click isn't the container nor a descendant of the container
        if (!container.is(e.target) && container.has(e.target).length === 0 && isOpen && !closeBtn.is(e.target) && closeBtn.has(e.target).length === 0) {
            $('.navbar .bars, .login_page_navigation .bars').click();
        }
    });


    /* Open & Close side-nav in small screen */
    $('.homepage_contant .bars').on("click", function () {
        $('.homepage_contant .bars .bar').toggleClass('bar-close');
        if ($('html').css('direction') == 'rtl') {
            $('.side_nav_container').toggleClass(' fadeInRight d-none');

        } else {
            $('.side_nav_container').toggleClass(' fadeInLeft d-none');

        }
    });

    //if (window.innerWidth <= 991.98) {
    //    $('.side_nav_container').addClass('animated fadeOutLeft');
    //} else {
    //    $('.side_nav_container').removeClass('fadeOutLeft');
    //}



    /* Search filter button */
    $('.search_container .filter_btn').on("click", function () {
        $(this).toggleClass('avtive_color');
        $('.search_filters_container').toggleClass('search_filters_container_show');
        $('.search_filters_container').toggleClass('animated flipInX faster');
    });

    
    
});

/* Success Toster */
function successToaster(text) {
    var node = document.createElement("div");
    node.className += " successToaster animated fadeInDown";
    var textnode = document.createTextNode(text);
    node.appendChild(textnode);
    document.body.appendChild(node);
    setTimeout(function () { node.className += " fadeOutUp"; }, 3000);
    setTimeout(function () { node.remove(); }, 4000);
}
function errorToaster(text) {
    var node = document.createElement("div");
    node.className += " errorToaster animated fadeInDown";
    var textnode = document.createTextNode(text);
    node.appendChild(textnode);
    document.body.appendChild(node);
    setTimeout(function () { node.className += " fadeOutUp"; }, 3000);
    setTimeout(function () { node.remove(); }, 4000);
}
