
document.getElementById("expand_btn").onclick = () => {
  document.getElementById("expand_btn_container").classList.toggle("expand-btn-container-open")
  console.log(document.getElementById("expand_btn_container"));
}

// between 
document.getElementById("for_cust_btn").onclick = () => {
  document.getElementById("for_cust_btn").classList.toggle("selected")
  document.getElementById("for_provide_btn").classList.toggle("selected")
}
document.getElementById("for_provide_btn").onclick = () => {
  document.getElementById("for_provide_btn").classList.toggle("selected")
  document.getElementById("for_cust_btn").classList.toggle("selected")
}

$(document).ready(()=>{
  $('.head-symbol').click((e)=>{
    $e=e.target     
    $($e).toggleClass('rotate90');
    $($e).parent().next().slideToggle(500);
})
})