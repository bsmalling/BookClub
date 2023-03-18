function validateInvitationCode(event) {
    let input = document.getElementById('InvitationCodeInput');
    if (input.innerText == 'grit') {
        window.location.href = 'Identity/Account/Register';
    }
    else {
        input.innerText = '';
    }
}
