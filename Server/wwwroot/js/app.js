function validateInvitationCode(event) {
    var input = document.getElementById('InvitationCodeInput');
    if (input.innerText == 'grit') {
        window.location.href = 'Identity/Account/Register';
    }
    else {
        input.innerText = '';
    }
}
//# sourceMappingURL=app.js.map