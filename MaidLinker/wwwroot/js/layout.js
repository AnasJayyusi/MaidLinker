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
    if (currentCultureCode == "en-US") {
        $("#current-language").text("English")
    }

    if (currentCultureCode == "ar-SA") {
        $("#current-language").text("العربية")
    }

}


