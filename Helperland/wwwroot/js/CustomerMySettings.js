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



$("#tab_1_date").val($("#tab_1_date").attr("value"))
$("#tab_1_month").val($("#tab_1_month").attr("value"))
$("#tab_1_year").val($("#tab_1_year").attr("value"))


// 1st tab data

document.querySelector("#my_datail_save").addEventListener("click", (e) => {

    const validator = $("#tab1_form").validate();
    if (validator.form()) {

        e.preventDefault()

        var details = {}

        details.first_name = document.getElementById("tab_1_f_name").value;
        details.last_name = document.getElementById("tab_1_l_name").value;
        details.email = document.getElementById("tab_1_email_add").value;
        details.mobile = document.getElementById("tab_1_phone_no").value;
        details.date = document.getElementById("tab_1_date").value;
        details.month = document.getElementById("tab_1_month").value;
        details.year = document.getElementById("tab_1_year").value;
        details.language = document.getElementById("lang").value;

        console.log(details);


        fetch("/Customer/settingtab1", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(details)
        }).then(res => res.json()).then(datafromcontroller => {

            document.getElementById("tab_1_f_name").value = datafromcontroller.fname;
            document.getElementById("tab_1_l_name").value = datafromcontroller.lname;
            document.getElementById("tab_1_phone_no").value = datafromcontroller.mob;
            document.getElementById("tab_1_date").value = datafromcontroller.date;
            document.getElementById("tab_1_month").value = datafromcontroller.month;
            document.getElementById("tab_1_year").value = datafromcontroller.year;
            //document.getElementById("lang").value = datafromcontroller.lang;

            alert("Your Data Saved SuccessFully...!!")
        }).catch(err => console.log(err));


    }

});
document.querySelector("#tab1_form").addEventListener("submit", e => {
    e.preventDefault()

})


// 2nd tab 
function edit(event, id) {

    event.preventDefault()

    var details = {}
    details.id = id;
    details.street = document.getElementById("strt_" + id).value;
    details.house = document.getElementById("house_" + id).value;
    details.postcode = document.getElementById("pin_" + id).value;
    details.city = document.getElementById("cty_" + id).value;
    details.mobile = document.getElementById("phn_" + id).value;

    console.log(details);

    fetch("/Customer/settingtab2_edit", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(details)
    }).then(res => res.json()).then(datafromcontroller => {

        document.getElementById("address_" + id).innerHTML = datafromcontroller.address;
        document.getElementById("ph_" + id).innerHTML = datafromcontroller.phone;


        const modalhtml = document.querySelector("#edit_" + id)

        var modal = bootstrap.Modal.getOrCreateInstance(modalhtml)
        modal.hide();

    }).catch(err => console.log(err));
}
function openmodal(id) {

    const modalhtml = document.querySelector("#edit_" + id)

    var modal = bootstrap.Modal.getOrCreateInstance(modalhtml)
    modal.show();
}

// 2nd tab new addresss 

function add(event) {

    event.preventDefault()

    var new_data = {}
    new_data.street = document.getElementById("new1").value;
    new_data.house = document.getElementById("new2").value;
    new_data.postcode = document.getElementById("new3").value;
    new_data.city = document.getElementById("new4").value;
    new_data.mobile = document.getElementById("new5").value;

    console.log(new_data)

    fetch("/Customer/settingtab2_new", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(new_data)
    }).then(res => res.json()).then(datafromcontroller => {


        document.getElementById("for_innerhtml").innerHTML += `

<tr class="table_view_body" data-address_id="${datafromcontroller.id}">
                                    <td>

                                        <p><b>Address: </b><span id="address_${datafromcontroller.id}">${datafromcontroller.address}</span></p>
                                        <p><b>Phone number: </b><span id="ph_${datafromcontroller.id}">${datafromcontroller.phone}</span></p>

                                    </td>
                                    <td>
                                        <img onclick="openmodal(${datafromcontroller.id})" src="/img/edit.png" alt="pic1">
                                        <img src="/img/delete-icon.png" alt="pic2">


                                        <!-- Modal -->
                                    <div class="modal fade" id="edit_${datafromcontroller.id}" tabindex="-1" aria-labelledby="exampleModalLabel" aria-hidden="true">
                                            <div class="modal-dialog modal-dialog-centered">
                                                <div class="modal-content">
                                                    <div class="modal-header">
                                                        <h5 class="modal-title" id="exampleModalLabel">Edit Address</h5>
                                                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                                                    </div>
                                                    <div class="modal-body">
                                                        <form>

                                                            <input asp-for="setting2viewmodel.id" type="text" class="form-control d-none" value="${datafromcontroller.id}" id="adid_${datafromcontroller.id}">
                                                            <div class="row row-cols-2">
                                                                <div class="col mb-3">
                                                                    <label class="control-label"><b>Street Name</b></label>
                                                                    <input asp-for="setting2viewmodel.street" type="text" class="form-control" value="${new_data.street}" id="strt_${datafromcontroller.id}" placeholder="Street name">
                                                                    <span asp-validation-for="setting2viewmodel.street" class="text-danger"></span>
                                                                </div>
                                                                <div class="col mb-3">
                                                                    <label class="control-label"><b>House number</b></label>
                                                                    <input asp-for="setting2viewmodel.house" type="text" class="form-control" value="${new_data.house}" id="house_${datafromcontroller.id}" placeholder="House number">
                                                                    <span asp-validation-for="setting2viewmodel.house" class="text-danger"></span>
                                                                </div>
                                                                <div class="col mb-3">
                                                                    <label class="control-label"><b>Postal code</b></label>
                                                                    <input asp-for="setting2viewmodel.postcode" type="text" class="form-control" value="${new_data.postcode}" id="pin_${datafromcontroller.id}" placeholder="pin code">
                                                                    <span asp-validation-for="setting2viewmodel.postcode" class="text-danger"></span>
                                                                </div>
                                                                <div class="col mb-3">
                                                                    <label class="control-label"><b>City</b></label>
                                                                    <input asp-for="setting2viewmodel.city" type="text" class="form-control" value="${new_data.city}" id="cty_${datafromcontroller.id}" placeholder="city">
                                                                    <span asp-validation-for="setting2viewmodel.city" class="text-danger"></span>
                                                                </div>

                                                                <div class="col mb-3">
                                                                    <label class="control-label"><b>Phone Number</b></label>
                                                                    <div class="input-group flex-nowrap">
                                                                        <span class="input-group-text" id="addon-wrapping">+91</span>
                                                                        <input asp-for="setting2viewmodel.mobile" type="text" class="form-control" id="phn_${datafromcontroller.id}" value="${new_data.mobile}" aria-describedby="addon-wrapping">
                                                                    </div>
                                                                    <span asp-validation-for="setting2viewmodel.mobile" class="text-danger"></span>
                                                                </div>
                                                            </div>
                                                            <button onclick="edit(event, ${datafromcontroller.id});" class="rate_submit w-100">Edit</button>
                                                        </form>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </td>
                                </tr>`
        var modalhtml = document.getElementById("new_form");
        var modal = bootstrap.Modal.getOrCreateInstance(modalhtml)
        modal.hide();

    }).catch(err => console.log(err));
};
document.getElementById("new_form").addEventListener("submit", (e) => {
    e.preventDefault();
})


// tab3 

document.getElementById("psssubmit").addEventListener("click", (e) => {

    const validator = $("#passform").validate();
    if (validator.form()) {
        e.preventDefault();

        var pass = {};

        pass.oldpassword = document.getElementById("exampleInputPassword1").value;
        pass.newpassword = document.getElementById("exampleInputPassword2").value;
        pass.confirmpassword = document.getElementById("exampleInputPassword3").value;

        console.log(pass);

        fetch("/Customer/settingtab3", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(pass)
        }).then(res => res.json()).then(datafromcontroller => {
            if (datafromcontroller == "true") {
                document.getElementById("pass_success").style.color = "#198754"
                document.getElementById("pass_success").innerHTML += `Your Password has been changed sucessfully...!`
            }
            if (datafromcontroller == "false") {

                document.getElementById("pass_success").style.color = "#DC3545"
                document.getElementById("pass_success").innerHTML += `Incorrect old password, Your Password has not been changed...! `
            }



        }).catch(err => console.log(err));
    }
});

document.getElementById("passform").addEventListener("submit", (e) => {
    e.preventDefault();
});