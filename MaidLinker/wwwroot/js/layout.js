    $(document).ready(function () {
            var currentCultureCode = getCookie("CultureInfo");
    console.log(currentCultureCode);
    setCurrentLanguageOnUI(currentCultureCode)
        });

    function getCookie(name) {
        function escape(s) { return s.replace(/([.*+?\^$(){}|\[\]\/\\])/g, '\\$1'); }
            var match = document.cookie.match(RegExp('(?:^|;\\s*)' + escape(name) + '=([^;]*)'));
    return match ? match[1] : null;
        }


    function setCurrentLanguageOnUI(currentCultureCode) {
            if (currentCultureCode == "en-US"){
        // Smell Code Should be deleted
        //$('html').attr('dir', "ltr");
        //$('html').attr('lang', "en");
        //$("html").children().css("direction", "ltr");
        $("#current-language").text("English")
    }

    if (currentCultureCode == "ar-SA"){
        // Smell Code Should be deleted
        $("#current-language").text("العربية")
        //$('html').attr('dir', "rtl");
        //$('html').attr('lang', "ar");
        //$("html").children().css("direction", "rtl");

        //var align = document.querySelectorAll("h1");
        //for (var i = 0; i < align.length; i++) {
        //    // Set the 'direction' property to 'ltr'
        //    align[i].style.textAlign = "right";
        //}
        //align = document.querySelectorAll("p");
        //for (var i = 0; i < align.length; i++) {
        //    // Set the 'direction' property to 'ltr'
        //    align[i].style.textAlign = "right";
        //}

        //align = document.querySelectorAll("select");
        //for (var i = 0; i < align.length; i++) {
        //    // Set the 'direction' property to 'ltr'
        //    align[i].style.textAlign = "inherit";
        //}

        //align = document.querySelectorAll("li");
        //for (var i = 0; i < align.length; i++) {
        //    // Set the 'direction' property to 'ltr'
        //    align[i].style.textAlign = "right";
        //}

        // align = document.querySelectorAll("form");
        //for (var i = 0; i < align.length; i++) {
        //    // Set the 'direction' property to 'ltr'
        //    align[i].style.textAlign = "right";
        //}

        //align = document.querySelectorAll("select");
        //for (var i = 0; i < align.length; i++) {
        //    // Set the 'direction' property to 'ltr'
        //    align[i].style.textAlign = "center";
        //}
    }
                
        }


