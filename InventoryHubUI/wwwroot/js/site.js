$("#page-header-user-dropdown").on("click", function () {
    if ($("#profile_menu").attr('hidden') !== undefined) {
        $("#profile_menu").removeAttr("hidden");
    } else {
        $("#profile_menu").attr("hidden",true);
    }

})