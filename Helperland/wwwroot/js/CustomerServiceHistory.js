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
        dom: 'Btlip',
        responsive: false,
        pagingType: 'full_numbers',
        'columnDefs': [{ 'orderable': false, 'targets': [4, 5] }],
        "language": {
            "paginate": {
                "previous": '<img src="../img/polygon-1-copy-5.png" alt="">',
                "next": '<img src="../img/polygon-1-copy-5.png" style="transform:rotate(180deg)" alt="">',
                "first": '<img src="../img/first-page.png" alt="">',
                "last": '<img src="../img/first-page.png" style="transform:rotate(180deg)" alt="">'
            },
            'info': "Total Record: _MAX_",

        },
        buttons: ["excel"],
    });

    // filter popover 
    document.getElementById("responsive_filter_id").addEventListener("click", () => {
        document.querySelectorAll(".form-check-input").forEach(e => {
            e.addEventListener("click", () => {
                table.order([e.getAttribute('data-dt-col'), e.getAttribute('data-dt-sort')]).draw()
            })
        })
    })

    $(".between_2nd_container_upper_header").insertBefore(".dt-button");
    $(".buttons-excel").html("Export");
});

// for left-right side manu 
document.getElementById("expand_btn").addEventListener('click', () => {
    document.getElementById("expand_btn_container").classList.toggle("expand-btn-container-open")
})
document.getElementById("expand_btn_res").addEventListener('click', () => {
    document.getElementById("expand_btn_container").classList.toggle("expand-btn-container-open")
})

// for popover 
var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'))
var popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
    return new bootstrap.Popover(popoverTriggerEl, { sanitize: false })
})




//  ......1st modal data change.......

function first_modal(id) {

    var myservice_id = {}
    myservice_id.ServiceId = id;

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


// stars Js 
var rating_send = {}

const svgs = document.querySelectorAll(".stars svg path");
svgs.forEach((s, i) => {
    s.addEventListener("click", () => {

        $cover = s.parentNode.parentNode
        var sid = s.getAttribute("data-serviceid");
        document.getElementById("htmlrate1_" + sid).value = (i % 5) + 1;
        $($cover).children("div.cover")[0].style.width = 100 - ((i % 5) + 1) * 20 + "%";
    });
});

const svgs1 = document.querySelectorAll(".stars1 svg path");
svgs1.forEach((s, i) => {
    s.addEventListener("click", () => {

        $cover = s.parentNode.parentNode
        var sid = s.getAttribute("data-serviceid");
        document.getElementById("htmlrate2_" + sid).value = (i % 5) + 1;
        $($cover).children("div.cover1")[0].style.width = 100 - ((i % 5) + 1) * 20 + "%";
    });
});

const svgs2 = document.querySelectorAll(".stars2 svg path");
svgs2.forEach((s, i) => {
    s.addEventListener("click", () => {

        $cover = s.parentNode.parentNode
        var sid = s.getAttribute("data-serviceid");
        document.getElementById("htmlrate3_" + sid).value = (i % 5) + 1;
        $($cover).children("div.cover2")[0].style.width = 100 - ((i % 5) + 1) * 20 + "%";
    });
});

