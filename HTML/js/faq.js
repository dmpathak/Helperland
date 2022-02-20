
document.getElementById("expand_btn").onclick = () => {
  document.getElementById("expand_btn_container").classList.toggle("expand-btn-container-open")
  console.log(document.getElementById("expand_btn_container"));
}

// between 
document.getElementById("for_cust_btn").onclick = () => {
  document.getElementById("for_cust_btn").classList.add("selected")
  document.getElementById("for_provide_btn").classList.remove("selected")

  document.getElementById("info_container_customer").classList.remove("hidd")
  document.getElementById("info_container_provider").classList.add("hidd")

}
document.getElementById("for_provide_btn").onclick = () => {
  document.getElementById("for_provide_btn").classList.add("selected")
  document.getElementById("for_cust_btn").classList.remove("selected")

  document.getElementById("info_container_customer").classList.add("hidd")
  document.getElementById("info_container_provider").classList.remove("hidd")
}

$(document).ready(() => {
  $('.head-symbol').click((e) => {
    $e = e.target
    $($e).toggleClass('rotate90');
    $($e).parent().next().slideToggle(500);
  })
})  