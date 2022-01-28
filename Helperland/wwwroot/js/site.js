// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
const hamburger = document.querySelector(".hamburger");
const nav_ul = document.querySelector(".nav_ul");
const last_div = document.querySelector(".last_div");
const hide_ok = document.querySelector(".last_div_button");
const arrow_div = document.querySelector(".arrow_div");

// for hamburger 
hamburger.addEventListener("click", () => {
    nav_ul.classList.toggle("open")
    hamburger.classList.toggle("open")
})


// for scroll about up arrow 
window.addEventListener("scroll", () => {
    if (window.scrollY <= 132) {
        arrow_div.style.opacity = 0;
        arrow_div.style.pointerEvents = "none";
    }
    else {
        arrow_div.style.opacity = 1;
        arrow_div.style.pointerEvents = "all";
    }
})
// for navbar scroll
window.addEventListener("scroll", () => {
    if (window.scrollY > 10) {
        document.getElementById("header").classList.add("header_scroll")
    }
    else {
        document.getElementById("header").classList.remove("header_scroll")
    }
})


// for last yellow button
hide_ok.addEventListener("click", () => {
    last_div.classList.add('hidden');
})


// script for modal 
if (document.location.search.substring(1).includes('true')) {
    document.querySelector("#login_modal").click()
    if (window.history.pushState) {
        const newURL = new URL(window.location.href);
        newURL.search = '';
        window.history.pushState({ path: newURL.href }, '', newURL.href);
    }
}
