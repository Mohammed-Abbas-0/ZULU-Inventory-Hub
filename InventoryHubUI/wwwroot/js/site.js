$("#page-header-user-dropdown").on("click", function (e) {
    e.stopPropagation();
    if ($("#profile_menu").attr('hidden') !== undefined) {
        $("#profile_menu").removeAttr("hidden");
    } else {
        $("#profile_menu").attr("hidden",true);
    }

})

$(document).click(function () {
    $('#profile_menu').attr('hidden', true);  // إضافة سمة hidden
});

