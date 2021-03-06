// for navbar 
document.getElementById("expand_btn").onclick = () => {
    document.getElementById("expand_btn_container").classList.toggle("expand-btn-container-open")
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


// for accordian
$(document).ready(() => {
    $('.head-symbol').click((e) => {
        $e = e.target
        $($e).toggleClass('rotate90');
        $($e).parent().next().slideToggle(500);
    })
})


// for extra service click image 
$(document).ready(function () {
    // 1st service 
    $(".click_checkbox1").click(function () {
        $(".click_checkbox1").next().children("div").toggleClass("extra_service_photo_active");
        $(".click_checkbox1").next().children().find('img').toggle();
    });

    // 2nd service 
    $(".click_checkbox2").click(function () {
        $(".click_checkbox2").next().children("div").toggleClass("extra_service_photo_active");
        $(".click_checkbox2").next().children().find('img').toggle();
    });

    // 3rd service 
    $(".click_checkbox3").click(function () {
        $(".click_checkbox3").next().children("div").toggleClass("extra_service_photo_active");
        $(".click_checkbox3").next().children().find('img').toggle();
    });

    // 4th service 
    $(".click_checkbox4").click(function () {
        $(".click_checkbox4").next().children("div").toggleClass("extra_service_photo_active");
        $(".click_checkbox4").next().children().find('img').toggle();
    });

    // 5th service 
    $(".click_checkbox5").click(function () {
        $(".click_checkbox5").next().children("div").toggleClass("extra_service_photo_active");
        $(".click_checkbox5").next().children().find('img').toggle();
    });

});



// for navtabs desable and all

tab_ids = ["", "paymentBtn", "detailsBtn", "schedulePlanBtn", "setupServiceBtn"]
function progress_validator(tab_id) {
    let i = 1;
    tab_ids.slice(1, 5).forEach(tabidind => {
        document.getElementById(tabidind).disabled = false;

    });
    while (i < tab_id) {
        document.getElementById(tab_ids[i]).disabled = true;
        document.getElementById(tab_ids[i]).classList.remove("filled")
        i += 1;
    }
}




// sample dict for final data
var clener_details = {
    pincode: "394001",
    datetime: ["date", "time"],
    basic_time: "5",
    service_choosen: [true, true, false, false, true],
    comment: "hello deep !",
    is_pet: true,
    address_id: 5,
    fav_provider: true,
    id_of_fav_provider: 10
}


//final booking deails
var booking_details = {};


function schedule_plan_continue() {
    let checked_services = [];
    let extra_time = 0;

    if (document.getElementById("date").value && document.getElementById("time").value && document.getElementById("basic").value) {

        document.getElementById("date_empty_error").innerHTML = ``;
        document.getElementById("starttime_empty_error").innerHTML = ``;
        document.getElementById("basic_empty_error").innerHTML = ``;

        booking_details.date = document.getElementById("date").value;
        booking_details.time = document.getElementById("time").value;
        booking_details.basic_time = document.getElementById("basic").value;
        ["1", "2", "3", "4", "5"].forEach(tempid => {
            checked_services.push(document.getElementById(`flexCheckDefault${tempid}`).checked)
            if (document.getElementById(`flexCheckDefault${tempid}`).checked) {
                extra_time += 0.5;
            }
        })
        booking_details.extra_services = checked_services;
        booking_details.comments = document.getElementById("comments").value;
        booking_details.have_pets = document.getElementById("is_pet").checked;
        booking_details.total_time = `${Number(document.getElementById("basic").value) + extra_time}`;


        if ((extra_time + Number(document.getElementById("time").value.split(":")[0]) + Number(document.getElementById("basic").value.split(" ")[0])) > 21) {
            document.getElementById("time_exeed").innerText = `your service time is more than 21:00 hr`;
        }
        else {
            document.getElementById("time_exeed").innerhtml = ``;
            document.getElementById("detailsBtn").disabled = false;
            document.getElementById("detailsBtn").click();
            document.getElementById("schedulePlanBtn").classList.add("filled");
        }


    }
    else {

        if (document.getElementById("date").value) {
            document.getElementById("date_empty_error").innerHTML = ``;
        }
        else {
            document.getElementById("date_empty_error").innerHTML = `Please! Enter Service Date`;
        }

        if (document.getElementById("time").value) {
            document.getElementById("starttime_empty_error").innerHTML = ``;
        }
        else {
            document.getElementById("starttime_empty_error").innerHTML = `Please! Enter Service StartTime`;
        }

        if (document.getElementById("basic").value) {
            document.getElementById("basic_empty_error").innerHTML = ``;
        }
        else {
            document.getElementById("basic_empty_error").innerHTML = `Please! Enter Basic Time`;
        }
    }


}


function your_details_continue() {

    if (document.querySelector('input[name="flexRadioDefault"]:checked') != null) {

        booking_details.address_id = Number(document.querySelector('input[name="flexRadioDefault"]:checked').value);
        document.getElementById("paymentBtn").disabled = false;
        document.getElementById("paymentBtn").click();
        document.getElementById("detailsBtn").classList.add("filled");
    }
    else {
        document.getElementById("add_address_error").innerHTML = `Please! Select Any Address`;
    }
}


// for reflect input data to payment card 

// 1st part (datechange on 2nd tab with validation)
function datechange() {
    var date_id = document.getElementById("date");
    var regex = /^([0-9]){2}\/([0-9]){2}\/([0-9]){4}$/;
    if (date_id.value.match(regex)) {


        document.getElementById("date_text").innerHTML = date_id.value;
        document.getElementById("date_text_responsive").innerHTML = date_id.value;
        document.getElementById("wrong_date").innerHTML = "";
        document.getElementById("date_empty_error").innerHTML = ``;
        document.getElementById("gone_date").innerHTML = "";
    }
    else {
        document.getElementById("wrong_date").innerHTML = "Please ! Enter Valid Date Format";

    }

    var today = new Date();
    let entered_date = new Date(Number(date_id.value.split("/")[2]), Number(date_id.value.split("/")[1]) - 1, Number(date_id.value.split("/")[0]), today.getHours(), today.getMinutes(), today.getSeconds())
    if (entered_date.getTime() + 50000 > (new Date()).getTime()) {
        document.getElementById("gone_date").innerHTML = "";
    }
    else {
        document.getElementById("gone_date").innerHTML = "You Can't Peak date before Today";
    }
}

// (timechange on 2nd tab )
function timechange() {
    var time_id = document.getElementById("time");
    document.getElementById("time_text").innerHTML =
        time_id.options[time_id.selectedIndex].text;
    document.getElementById("time_text_responsive").innerHTML =
        time_id.options[time_id.selectedIndex].text;
    if (document.getElementById("time").value) {
        document.getElementById("starttime_empty_error").innerHTML = ``;
    }
}
// (basic staytime change on 2nd tab )
function basicchange() {
    var basic_id = document.getElementById("basic");
    document.getElementById("basic_text").innerHTML =
        basic_id.options[basic_id.selectedIndex].text;
    document.getElementById("basic_text_responsive").innerHTML =
        basic_id.options[basic_id.selectedIndex].text;

    let time = parseFloat(basic_id.options[basic_id.selectedIndex].text);
    [1, 2, 3, 4, 5].forEach((each_id) => {
        if (document.querySelector(`.click_checkbox${each_id}`).checked) {
            time += 0.5
        }
    })
    document.getElementById("total_time").innerHTML =
        time;
    document.getElementById("total_time_responsive").innerHTML =
        time;

    document.getElementById("total_paym").innerHTML =
        time * 18;
    document.getElementById("total_paym_responsive").innerHTML =
        time
        * 18;

    if (document.getElementById("basic").value) {
        document.getElementById("basic_empty_error").innerHTML = ``;
    }
}

// 2nd part (Extra Services)
$(document).ready(function () {

    // SERVICE 1
    $(".click_checkbox1").click(function () {
        $(".cabinet").toggle();
        // FOR ADD AND REMOVE TOTAL TIME BASED ON EXTRA SERVICES
        if (document.querySelector(".click_checkbox1").checked) {
            document.getElementById("total_time").innerHTML = parseFloat(document.getElementById("total_time").innerText) + 0.5;
            document.getElementById("total_time_responsive").innerHTML = parseFloat(document.getElementById("total_time_responsive").innerText) + 0.5;

            document.getElementById("total_paym").innerHTML = (parseFloat(document.getElementById("total_time").innerText)) * 18;
            document.getElementById("total_paym_responsive").innerHTML = (parseFloat(document.getElementById("total_time").innerText)) * 18;
        }
        else {
            document.getElementById("total_time").innerHTML = parseFloat(document.getElementById("total_time").innerText) - 0.5;
            document.getElementById("total_time_responsive").innerHTML = parseFloat(document.getElementById("total_time_responsive").innerText) - 0.5;

            document.getElementById("total_paym").innerHTML = (parseFloat(document.getElementById("total_time").innerText)) * 18;
            document.getElementById("total_paym_responsive").innerHTML = (parseFloat(document.getElementById("total_time").innerText)) * 18;
        }
    });

    // SERVICE 2
    $(".click_checkbox2").click(function () {
        $(".fridge").toggle();
        if (document.querySelector(".click_checkbox2").checked) {
            document.getElementById("total_time").innerText = parseFloat(document.getElementById("total_time").innerText) + 0.5;
            document.getElementById("total_time_responsive").innerHTML = parseFloat(document.getElementById("total_time_responsive").innerText) + 0.5;

            document.getElementById("total_paym").innerHTML = (parseFloat(document.getElementById("total_time").innerText)) * 18;
            document.getElementById("total_paym_responsive").innerHTML = (parseFloat(document.getElementById("total_time").innerText)) * 18;
        }
        else {
            document.getElementById("total_time").innerText = parseFloat(document.getElementById("total_time").innerText) - 0.5;
            document.getElementById("total_time_responsive").innerHTML = parseFloat(document.getElementById("total_time_responsive").innerText) - 0.5;

            document.getElementById("total_paym").innerHTML = (parseFloat(document.getElementById("total_time").innerText)) * 18;
            document.getElementById("total_paym_responsive").innerHTML = (parseFloat(document.getElementById("total_time").innerText)) * 18;
        }
    });

    // SERVICE 3
    $(".click_checkbox3").click(function () {
        $(".oven").toggle();
        if (document.querySelector(".click_checkbox3").checked) {
            document.getElementById("total_time").innerText = parseFloat(document.getElementById("total_time").innerText) + 0.5;
            document.getElementById("total_time_responsive").innerHTML = parseFloat(document.getElementById("total_time_responsive").innerText) + 0.5;

            document.getElementById("total_paym").innerHTML = (parseFloat(document.getElementById("total_time").innerText)) * 18;
            document.getElementById("total_paym_responsive").innerHTML = (parseFloat(document.getElementById("total_time").innerText)) * 18;
        }
        else {
            document.getElementById("total_time").innerText = parseFloat(document.getElementById("total_time").innerText) - 0.5;
            document.getElementById("total_time_responsive").innerHTML = parseFloat(document.getElementById("total_time_responsive").innerText) - 0.5;

            document.getElementById("total_paym").innerHTML = (parseFloat(document.getElementById("total_time").innerText)) * 18;
            document.getElementById("total_paym_responsive").innerHTML = (parseFloat(document.getElementById("total_time").innerText)) * 18;
        }
    });

    // SERVICE 4
    $(".click_checkbox4").click(function () {
        $(".laundry").toggle();
        if (document.querySelector(".click_checkbox4").checked) {
            document.getElementById("total_time").innerText = parseFloat(document.getElementById("total_time").innerText) + 0.5;
            document.getElementById("total_time_responsive").innerHTML = parseFloat(document.getElementById("total_time_responsive").innerText) + 0.5;

            document.getElementById("total_paym").innerHTML = (parseFloat(document.getElementById("total_time").innerText)) * 18;
            document.getElementById("total_paym_responsive").innerHTML = (parseFloat(document.getElementById("total_time").innerText)) * 18;
        }
        else {
            document.getElementById("total_time").innerText = parseFloat(document.getElementById("total_time").innerText) - 0.5;
            document.getElementById("total_time_responsive").innerHTML = parseFloat(document.getElementById("total_time_responsive").innerText) - 0.5;

            document.getElementById("total_paym").innerHTML = (parseFloat(document.getElementById("total_time").innerText)) * 18;
            document.getElementById("total_paym_responsive").innerHTML = (parseFloat(document.getElementById("total_time").innerText)) * 18;
        }
    });

    // SERVICE 5
    $(".click_checkbox5").click(function () {
        $(".window").toggle();
        if (document.querySelector(".click_checkbox5").checked) {
            document.getElementById("total_time").innerText = parseFloat(document.getElementById("total_time").innerText) + 0.5;
            document.getElementById("total_time_responsive").innerHTML = parseFloat(document.getElementById("total_time_responsive").innerText) + 0.5;

            document.getElementById("total_paym").innerHTML = (parseFloat(document.getElementById("total_time").innerText)) * 18;
            document.getElementById("total_paym_responsive").innerHTML = (parseFloat(document.getElementById("total_time").innerText)) * 18;
        }
        else {
            document.getElementById("total_time").innerText = parseFloat(document.getElementById("total_time").innerText) - 0.5;
            document.getElementById("total_time_responsive").innerHTML = parseFloat(document.getElementById("total_time_responsive").innerText) - 0.5;

            document.getElementById("total_paym").innerHTML = (parseFloat(document.getElementById("total_time").innerText)) * 18;
            document.getElementById("total_paym_responsive").innerHTML = (parseFloat(document.getElementById("total_time").innerText)) * 18;
        }
    });
});



//Java script of Fatch API for Check Availability
var postalcode_form = document.querySelector("form#postalcode_form");
var avail_btn = document.querySelector(".avail_btn");

avail_btn.addEventListener("click", (e) => {

    e.preventDefault();
    const formData = new FormData(document.querySelector("form#postalcode_form"));
    const data = {};
    data.MyCheckavail = formData.get("MyCheckavail");
    data.user_id = 2
    fetch("/Book/CheckAvailability", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(data)
    })
        .then(res => res.json()).then(datareturnedfromcontroller => {
            document.getElementById("address_show_radio").innerHTML = "";
            document.getElementById("setupServiceBtn").classList.add("filled")


            if (datareturnedfromcontroller.is_available == true) {
                Array.from(datareturnedfromcontroller.addresses).forEach(eachadd => {
                    document.getElementById("address_show_radio").innerHTML += `

                        <div class="form-check mb-3 existing_address">
                            <input class="form-check-input cust_add" type="radio" name="flexRadioDefault"
                                   id="flexRadioDefault3" value="${eachadd.address_id}">
                            <label class="form-check-label" for="flexRadioDefault3">
                                <b class="second_tab_headings">Address:</b>
                                ${eachadd.address}   <br>
                                <b class="second_tab_headings">Phone:</b> ${eachadd.phone}
                            </label>
                        </div>
                    `;
                })

                if (datareturnedfromcontroller.addresses != false) {
                    document.getElementsByName("flexRadioDefault")[0].checked = true;
                }

                booking_details.pincode = document.getElementById("MyCheckavail").value;
                document.getElementById("schedulePlanBtn").disabled = false;
                document.getElementById("schedulePlanBtn").click();
                document.getElementById("pin_not_found").innerHTML = ""
                document.getElementById("form_postal").value = data.MyCheckavail
                document.getElementById("form_city").value = datareturnedfromcontroller.city


            }
            else {
                document.getElementById("pin_not_found").innerHTML = "There is no any service provider for this Postal Code !!"
                document.getElementById("setupServiceBtn").classList.remove("filled")
            }
        }).catch(err => console.log(err));

})
postalcode_form.addEventListener("submit", (e) => {
    e.preventDefault();
    avail_btn.click()
})


//Java script of Fatch API for save new address form 
// (new address form ma je data nakhyo post request marine and te j post req na response ma submit karyo ej data detail pa6i mokli and te j data ne te page par display karyo address nu radio button banavi ne)

var new_add_form = document.querySelector("form#new_add_form");
var form_save = document.getElementById("form_save");

form_save.addEventListener("click", (event) => {
    event.preventDefault();

    const formData1 = new FormData(document.querySelector("form#new_add_form"));
    const mydata = {};

    mydata.street_name = formData1.get("street_name");
    mydata.house_number = formData1.get("house_number");
    mydata.pincode = document.getElementById("MyCheckavail").value;
    mydata.city = formData1.get("city");
    mydata.phone_number = formData1.get("phone_number");
    fetch("/Book/save_new_address", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(mydata)
    })
        .then(res => res.json()).then(datareturnedfromcontroller => {

            document.getElementById("add_address_error").innerHTML = ``;
            document.getElementById("address_show_radio").innerHTML += `
                    <div class="form-check mb-3 existing_address">
                        <input class="form-check-input cust_add" type="radio" name="flexRadioDefault"
                                id="flexRadioDefault3" value="${datareturnedfromcontroller.address_id}">
                        <label class="form-check-label" for="flexRadioDefault3">
                            <b class="second_tab_headings">Address:</b>
                            ${datareturnedfromcontroller.address}  <br>
                            <b class="second_tab_headings">Phone:</b> ${datareturnedfromcontroller.phone}
                        </label>
                    </div>`;

        }).catch(err => console.log(err));
})


// fetch api for final data submission
var final = document.getElementById("complete_booking");
final.addEventListener("click", (event) => {
    if (document.querySelector('input[name="fav_provider"]:checked') != null) {
        var fav_ids = document.querySelector('input[name="fav_provider"]:checked').id.split("_");
        booking_details.selected_service_pro_id = fav_ids[2];
    }
    fetch("/Book/FinaldataSubmit", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(booking_details)
    }).then(res => res.json()).then(datafromcontroller => {
        if (Object.values(datafromcontroller)[0]) {
            document.getElementById("show_error").classList.add("d-none");
            document.getElementById("booking_success").classList.remove("d-none");
            document.getElementById("service_id").innerHTML = Object.values(datafromcontroller)[1];
        }
        else {
            document.getElementById("show_error").classList.remove("d-none");
            document.getElementById("booking_success").classList.add("d-none");
            document.getElementById("show_error").innerHTML = Object.values(datafromcontroller)[1];

        }
    }).catch(err => console.log(err));
})


function selected_fav_pro(id) {
    if (document.querySelector('input[name="fav_provider"]:checked') != null) {
        if (document.querySelector('input[name="fav_provider"]:checked') == document.getElementById(`fav_radio_${id}`)) {


            document.getElementById("fav_radio_" + id).checked = false;
            document.getElementById(`fav_button_${id}`).innerText = "select";
        }
        else {
            var ids = document.querySelector('input[name="fav_provider"]:checked').id.split("_")
            document.getElementById(`fav_button_${ids[2]}`).innerText = "select";
            document.getElementById("fav_radio_" + id).checked = true;
            document.getElementById(`fav_button_${id}`).innerText = "selected";
        }
    } else {
        document.getElementById("fav_radio_" + id).checked = true;
        document.getElementById(`fav_button_${id}`).innerText = "selected";
    }

}