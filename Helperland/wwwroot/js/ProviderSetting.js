// for left-right side manu 
document.getElementById("expand_btn").addEventListener('click', () => {
    document.getElementById("expand_btn_container").classList.toggle("expand-btn-container-open")
    console.log("Open");
})
document.getElementById("expand_btn_res").addEventListener('click', () => {
    document.getElementById("expand_btn_container").classList.toggle("expand-btn-container-open")
    console.log("Return");
})

// for popover 
var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'))
var popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
    return new bootstrap.Popover(popoverTriggerEl, { sanitize: false })
})


//to set database value of select options
$("#tab_1_date").val($("#tab_1_date").attr("value"))
$("#tab_1_month").val($("#tab_1_month").attr("value"))
$("#tab_1_year").val($("#tab_1_year").attr("value"))


//setting tab 1
function mygender(g_id) {
    console.log(g_id);
    document.getElementById("gender_value").value = document.getElementById(g_id).value;
}

function check(myid) {
    var checked = "";
    document.getElementById("avtar_img1").classList.remove("avtar_img_clicked");
    document.getElementById("avtar_img2").classList.remove("avtar_img_clicked");
    document.getElementById("avtar_img3").classList.remove("avtar_img_clicked");
    document.getElementById("avtar_img4").classList.remove("avtar_img_clicked");
    document.getElementById("avtar_img5").classList.remove("avtar_img_clicked");
    document.getElementById("avtar_img6").classList.remove("avtar_img_clicked");
    document.getElementById(myid).nextElementSibling.lastChild.classList.add("avtar_img_clicked");

    checked = myid;


    document.getElementById("heading_avtar_img").src = document.getElementById(myid).nextElementSibling.lastChild.src;

    document.getElementById("heading_avtar_img_value").value = document.getElementById(myid).nextElementSibling.lastChild.src;
    console.log(checked)

}

function passsave(e) {

    e.preventDefault();

    var pass = {};

    pass.oldpassword = document.getElementById("exampleInputPassword1").value;
    pass.newpassword = document.getElementById("exampleInputPassword2").value;
    pass.confirmpassword = document.getElementById("exampleInputPassword3").value;


    //loading(true);
    fetch("/Provider/settingtab2", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(pass)
    }).then(res => res.json()).then(datafromcontroller => {
        //loading(false);

        if (datafromcontroller == "true") {
            document.getElementById("pass_success").style.color = "#198754"
            document.getElementById("pass_success").innerHTML = `Your Password has been changed sucessfully...!`
        }
        if (datafromcontroller == "false") {

            document.getElementById("pass_success").style.color = "#DC3545"
            document.getElementById("pass_success").innerHTML = `Incorrect old password, Your Password has not been changed...! `
        }
        if (datafromcontroller == "datanone") {

            document.getElementById("pass_success").style.color = "#DC3545"
            document.getElementById("pass_success").innerHTML = `Any Field should not be null here...! `
        }

    }).catch(err => console.log(err));

};
document.getElementById("passform").addEventListener("submit", (e) => {
    e.preventDefault();
});