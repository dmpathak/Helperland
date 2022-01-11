const hamburger = document.querySelector(".hamburger");
const nav_ul = document.querySelector(".nav_ul");
const last_div = document.querySelector(".last_div");
const hide_ok = document.querySelector(".last_div_button");
const arrow_div = document.querySelector(".arrow_div");
hamburger.addEventListener("click", () => {
    nav_ul.classList.toggle("open")
    hamburger.classList.toggle("open")
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