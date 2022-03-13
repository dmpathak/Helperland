const body = document.querySelector("body")
const loading = (isloading) => {
    if (isloading) {
        body.classList.add("loading");
    }
    else {
        body.classList.remove("loading");
    }

}

$(document).ready(function () {
    var table = $('#example').DataTable({
        dom: 'btlip',
        responsive: false,
        pagingType: 'full_numbers',
        'columnDefs': [{ 'orderable': false, 'targets': [4] }],
        buttons: [
            {
                extend: "excel",
                text: "Export",
            },
        ],

        "language": {
            "paginate": {
                "previous": '<img src="../img/polygon-1-copy-5.png" alt="">',
                "next": '<img src="../img/polygon-1-copy-5.png" style="transform:rotate(180deg)" alt="">',
                "first": '<img src="../img/first-page.png" alt="">',
                "last": '<img src="../img/first-page.png" style="transform:rotate(180deg)" alt="">'
            },
            'info': "Total Record: _MAX_",
        }
    });

    // filter popover 
    document.getElementById("responsive_filter_id").addEventListener("click", () => {
        document.querySelectorAll(".form-check-input").forEach(e => {
            e.addEventListener("click", () => {
                table.order([e.getAttribute('data-dt-col'), e.getAttribute('data-dt-sort')]).draw()
            })
        })
    })
});

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





// modal for service request detail
obj1 = {}
obj1.date = document.getElementById("date");
obj1.service_provide_name = document.getElementById("service_provide_name")
obj1.rating = document.getElementById("rate")
obj1.ServiceId = document.getElementById("ServiceId")
obj1.date = document.getElementById("date")
obj1.start_time = document.getElementById("start_time")
obj1.end_time = document.getElementById("end_time")
obj1.service_duration = document.getElementById("")
obj1.extras = document.getElementById("")
obj1.total_cost = document.getElementById("")
obj1.street_name = document.getElementById("")
obj1.house_number = document.getElementById("")
obj1.city = document.getElementById("")
obj1.phone_number = document.getElementById("")
obj1.email = document.getElementById("")
obj1.have_pets = document.getElementById("")


//  ......1st modal data change.......

function first_modal(id) {

    var myservice_id = {}
    myservice_id.ServiceId = id;
    /*console.log(id);*/

    loading(true);
    fetch("/Customer/CustomerDashboard1", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(myservice_id)
    }).then(res => res.json()).then(datafromcontroller => {
        loading(false);

        const m = document.querySelector("#servicedetail_" + id)

        m.querySelector("#date_modal").innerHTML = datafromcontroller.service_date;
        m.querySelector("#starttime_modal").innerHTML = datafromcontroller.service_start_time;
        m.querySelector("#endtime_modal").innerHTML = datafromcontroller.service_end_date;
        m.querySelector("#duration_modal").innerHTML = datafromcontroller.duration;
        m.querySelector("#serviceid_modal").innerHTML = datafromcontroller.service_id;
        m.querySelector("#amount_modal").innerHTML = datafromcontroller.payment;
        m.querySelector("#extras_modal").innerHTML = datafromcontroller.extras;
        m.querySelector("#address_modal").innerHTML = datafromcontroller.serviceaddres;
        m.querySelector("#phone_modal").innerHTML = datafromcontroller.servicephone;
        m.querySelector("#mail_modal").innerHTML = datafromcontroller.serviceemail;
        if (datafromcontroller.servicepets == true) {
            m.querySelector("#pet").innerHTML = "I have pets at home";
            m.querySelector("#for_pet_img").src = "/img/success_tick.png";
        }
        else {
            m.querySelector("#pet").innerHTML = "I don't have pets at home";
        }

    }).catch(err => console.log(err));

};


//  ..... 2nd modal data change.......

function reschedule(e, id) {
    e.preventDefault();
    var reschedule_data = {}
    reschedule_data.ServiceId = id;
    reschedule_data.date = document.getElementById("reschedule_data1_" + id).value;
    reschedule_data.start_time = document.getElementById("reschedule_data2_" + id).value;

    loading(true);
    fetch("/Customer/CustomerDashboard2", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(reschedule_data)
    }).then(res => res.json()).then(datafromcontroller => {
        loading(false);
        document.getElementById("reschedule_success_" + id).innerHTML = ``;
        document.getElementById("reschedule_error_" + id).innerHTML = ``;

        if (datafromcontroller) {
            document.getElementById("reschedule_success_" + id).innerHTML = `service has been rescheduled..!!`;
            setTimeout(() => {
                window.location.reload();
            },2500)
        }
        else {
            document.getElementById("reschedule_error_" + id).innerHTML = `service not rescheduled..!!`;
        }

    }).catch(err => console.log(err));
};