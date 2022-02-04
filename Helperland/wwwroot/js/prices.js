// navbar toggle  
document.getElementById("expand_btn").onclick = () => {
  document.getElementById("expand_btn_container").classList.toggle("expand-btn-container-open")
  console.log(document.getElementById("expand_btn_container"));
}

// up scroll button 
window.addEventListener("scroll", () => {
  if (window.scrollY <= 132) {
    document.getElementById("scroll_img_div").style.opacity = 0;
    document.getElementById("scroll_img_div").style.pointerEvents = "none";
  }
  else {
    document.getElementById("scroll_img_div").style.opacity = 1;
    document.getElementById("scroll_img_div").style.pointerEvents = "all";
  }
})


