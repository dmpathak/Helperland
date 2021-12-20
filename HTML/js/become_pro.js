const hamburger = document.querySelector(".hamburger");
const nav_ul = document.querySelector(".nav_ul");
const last_div = document.querySelector(".last_div");
const hide_ok = document.querySelector(".last_div_button");
const arrow_div = document.querySelector(".arrow_div");
hamburger.addEventListener("click", () => {
    nav_ul.classList.toggle("open")
    hamburger.classList.toggle("open")
})

hide_ok.addEventListener("click", () => {
    last_div.classList.add('hidden');
})

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