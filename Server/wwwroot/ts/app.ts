import { createApp } from 'vue';

const redirect = (url, asLink = true) =>
    asLink ? (window.location.href = url) : window.location.replace(url);

function validateInvitationCode(ev) {
    let input = (<HTMLInputElement>document.getElementById("InvitationCodeInput"));
    if (input.value == "GRIT") {
        alert("Greetigns!");
        redirect("Identity/Account/Register");
    }
    else {
        input.value = "";
    }
}

const bookApp = createApp({
    data() {
        return {
            Title: "",
            AuthorFirst: "",
            AuthorLast: "",
            Description: "",
            Pages: 0,
            ISBN: "",
            ASIN: "",
            Published: ""
        }
    }
})

bookApp.mount('#bookAppMount');
