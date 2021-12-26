document.getElementById("expand_btn").addEventListener('click', () => {
    document.getElementById("expand_btn_container").classList.toggle("expand-btn-container-open")
    console.log(document.getElementById("expand_btn_container"));
  })
  
  
  document.getElementById("open_side_manu_btn").addEventListener('click', () => {
    document.getElementById("open_side_manu_id").classList.toggle("open_side_manu_toggle")
  })

  document.getElementById("between_2nd_container_wrap_btn").addEventListener('click', () => {
    document.getElementById("between_2nd_container_data_detail").classList.toggle("between_2nd_container_data_toggle")
  })