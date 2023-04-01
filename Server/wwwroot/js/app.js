var redirect = function (url, asLink) {
    if (asLink === void 0) { asLink = true; }
    return asLink ? (window.location.href = url) : window.location.replace(url);
};
function validateInvitationCode(ev) {
    var input = document.getElementById("InvitationCodeInput");
    if (input.value == "GRIT") {
        redirect("sdfgsdfg");
    }
    else {
        input.value = "";
    }
}
//# sourceMappingURL=app.js.map