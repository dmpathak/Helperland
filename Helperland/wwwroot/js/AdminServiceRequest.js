$(document).ready(function () {
    var dt = $('#example').DataTable({
        dom: 'tlip',
        responsive: false,
        pagingType: 'full_numbers',
        'columnDefs': [{ 'orderable': false, 'targets': [6] }],
        "language": {
            "paginate": {
                "previous": '<img src="../img/polygon-1-copy-5.png" alt="">',
                "next": '<img src="../img/polygon-1-copy-5.png" style="transform:rotate(180deg)" alt="">'
            },
            'info': "Total Record: _MAX_",
        }
    });


    var sid = document.getElementById("selected_sid")
    var s_cust = document.getElementById("selected_cust")
    var s_pro = document.getElementById("selected_pro")
    var s_status = document.getElementById("selected_status")
    var s_fromdt = document.getElementById("from_dt")
    var s_todt = document.getElementById("to_dt")
    var search_data = document.getElementById("search_data")
    var clear_data = document.getElementById("clear_data")


    search_data.addEventListener("click", (e) => {
        e.preventDefault();
        $.fn.dataTable.ext.search.push(function (settings, data, dataIndex) {
            let serviceId = data[0];
            let serviceDate = data[1]
                .match(/[0-9]{2}\/[0-9]{2}\/[0-9]{4}/)[0];
            let custDetails = data[2];
            let spDetails = data[4];
            let status = data[5];
            const isServiceId = sid.value ? serviceId === sid.value : true;
            const isCustomer = s_cust.value === "" ? true : custDetails.includes(s_cust.value);
            const isSp = s_pro.value === "" ? true : spDetails.includes(s_pro.value);
            const isStatus = s_status.value === "" ? true : status === s_status.value;
            const dateSplited = serviceDate.split("/");
            const srdate = new Date(parseInt(dateSplited[2]), parseInt(dateSplited[1]) - 1, parseInt(dateSplited[0]));
            const isDateGreater = s_fromdt.value ? srdate >= new Date(s_fromdt.value) : true;
            const isDateSmaller = s_todt.value ? srdate <= new Date(s_todt.value) : true;
            return isServiceId && isCustomer && isSp && isStatus && isDateGreater && isDateSmaller;
        });
        dt.draw();
    });
    clear_data.addEventListener("click", (e) => {
        $.fn.dataTableExt.afnFiltering.length = 0;
        dt.draw();
    });
});



$("#from_dt").datepicker({
    changeMonth: true,
    changeYear: true,
    showButtonPanel: true,
    showAnim: "slideDown",
    dateFormat: "mm/dd/yy",
});

$("#to_dt").datepicker({
    changeMonth: true,
    changeYear: true,
    showButtonPanel: true,
    showAnim: "slideDown",
    dateFormat: "mm/dd/yy",
});






// for popover 
var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'))
var popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
    return new bootstrap.Popover(popoverTriggerEl)
})


// left toggle 

document.getElementById("pg_10_left_toggler").addEventListener('click', () => {
    document.getElementById("main_contant_1").classList.toggle("main_contant_1_toggler")
    document.getElementById("pg_10_left_toggler").classList.toggle("background_change")
})


//popover modal open for dynamic id (edit and reschedule)
$(function () {
    $("[data-toggle=popover]").popover({
        html: true,
        content: function () {
            return $('#popover-content').html();
        }
    });

    $(document).on('click', ".for_getting_id", function (e) {
        $('#for_edit_' + e.target.id).modal('show');
        $('.popover').popover('hide');
        //alert(e.target.id);

        //to set database value of select options
        console.log($("#start_time_" + e.target.id).val())
        $("#start_time_" + e.target.id).val($("#start_time_" + e.target.id).attr("value"));
        $("#city_" + e.target.id).val($("#city_" + e.target.id).attr("value"));

    });
});


//on date change
function datechange(id) {
    var date_id = document.getElementById("date_" + id);
    var regex = /^([0-9]){2}\/([0-9]){2}\/([0-9]){4}$/;
    if (date_id.value.match(regex)) {

        document.getElementById("errordate_" + id).innerHTML = ``;
        document.getElementById("wrong_date_" + id).innerHTML = "";
        document.getElementById("gone_date_" + id).innerHTML = "";
    }
    else {
        document.getElementById("wrong_date_" + id).innerHTML = "Please ! Enter Valid Date Format";
    }

    var today = new Date();
    let entered_date = new Date(Number(date_id.value.split("/")[2]), Number(date_id.value.split("/")[1]) - 1, Number(date_id.value.split("/")[0]), today.getHours(), today.getMinutes(), today.getSeconds())
    if (entered_date.getTime() + 50000 > (new Date()).getTime()) {
        document.getElementById("gone_date_" + id).innerHTML = "";
        document.getElementById("wrong_date_" + id).innerHTML = "";
        document.getElementById("gone_date_" + id).innerHTML = "";
    }
    else {
        document.getElementById("gone_date_" + id).innerHTML = "You Can't Peak date before Today";
    }
}



//edited data submit of above opened modal
function update(e, id) {
    if (document.getElementById("date_" + id).value && document.getElementById("start_time_" + id).value && document.getElementById("street_" + id).value && document.getElementById("house_" + id).value && document.getElementById("postal_" + id).value && document.getElementById("city_" + id).value) {

        send = {}

        send.id_for_service = document.getElementById("id_" + id).value;
        send.date = document.getElementById("date_" + id).value;
        send.time = document.getElementById("start_time_" + id).value;
        send.street = document.getElementById("street_" + id).value;
        send.house = document.getElementById("house_" + id).value;
        send.postal = document.getElementById("postal_" + id).value;
        send.city = document.getElementById("city_" + id).value;
        send.comment = document.getElementById("textarea_" + id).value;

        console.log(send);

        fetch("/Admin/edit_data", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(send)
        }).then(res => res.json()).then(datafromcontroller => {

            if (datafromcontroller == true) {
                alert("Your Data has been saved Successfully...!!");

            }
            if (datafromcontroller == false) {
                alert("You have service on this time, You Can't peak this one...!!");

            }

            window.location.reload();
        }).catch(err => console.log(err));
    }
    else {
        document.getElementById("errordate_" + id).innerHTML = ``;
        document.getElementById("errortime_" + id).innerHTML = ``;
        document.getElementById("errorstreet_" + id).innerHTML = ``;
        document.getElementById("errorhouse_" + id).innerHTML = ``;
        document.getElementById("errorpostal_" + id).innerHTML = ``;
        document.getElementById("errorcity_" + id).innerHTML = ``;

        if (!document.getElementById("date_" + id).value) {
            document.getElementById("errordate_" + id).innerHTML = `Please Enter Date`;
        }
        if (!document.getElementById("start_time_" + id).value) {
            document.getElementById("errortime_" + id).innerHTML = `Please Enter time`;
        }
        if (!document.getElementById("street_" + id).value) {
            document.getElementById("errorstreet_" + id).innerHTML = `Please Enter street`;
        }
        if (!document.getElementById("house_" + id).value) {
            document.getElementById("errorhouse_" + id).innerHTML = `Please Enter house`;
        }
        if (!document.getElementById("postal_" + id).value) {
            document.getElementById("errorpostal_" + id).innerHTML = `Please Enter postal`;
        }
        if (!document.getElementById("city_" + id).value) {
            document.getElementById("errorcity_" + id).innerHTML = `Please Enter city`;
        }
    }
};



//popover modal open for dynamic id (refund)
$(function () {
    $("[data-toggle=popover]").popover({
        html: true,
        content: function () {
            return $('#popover-content').html();
        }
    });

    $(document).on('click', ".refund_id_get", function (e) {
        $('#for_refund_' + e.target.id).modal('show');
        $('.popover').popover('hide');
        //alert(e.target.id);
        if (document.getElementById("paid_amount_" + e.target.id).innerHTML == document.getElementById("refunded_amount_" + e.target.id).innerHTML) {
            document.getElementById("fully_refunded_" + e.target.id).innerHTML = `You have refunded all your paid amount`;
            document.getElementById("refund_btn_" + e.target.id).disabled = true;
        }


    });
});


// show calculate data
function calculate_btn(id) {
    document.getElementById("erroramount_" + id).innerHTML = ``;
    if (document.getElementById("amount_method_" + id).value == "Linear") {

        document.getElementById("calculate_text_" + id).value = document.getElementById("amount_" + id).value;
    }
    if (document.getElementById("amount_method_" + id).value == "Percentage") {

        document.getElementById("calculate_text_" + id).value = (document.getElementById("balance_amount_" + id).innerHTML * document.getElementById("amount_" + id).value) / 100;
    }
}


//refund data submit of above opened modal
function refund(e, id) {
    document.getElementById("erroramount_" + id).innerHTML = ``;
    if (document.getElementById("amount_" + id).value) {
        document.getElementById("erroramount_" + id).innerHTML = ``;

        if (document.getElementById("amount_method_" + id).value == "Linear" && parseFloat(document.getElementById("amount_" + id).value) <= parseFloat(document.getElementById("balance_amount_" + id).innerHTML) || document.getElementById("amount_method_" + id).value == "Percentage" && parseFloat(document.getElementById("amount_" + id).value) <= parseFloat(100)) {


            document.getElementById("erroramount_" + id).innerHTML = ``;
            send = {}

            send.id_for_service = document.getElementById("my_refund_id_" + id).value;
            send.refund_amount = document.getElementById("calculate_text_" + id).value;

            console.log(send);

            fetch("/Admin/refund", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(send)
            }).then(res => res.json()).then(datafromcontroller => {

                if (datafromcontroller) {
                    alert("You have been refunded Successfully...!!");

                }

                window.location.reload();
            }).catch(err => console.log(err));
        }
        else {
            document.getElementById("erroramount_" + id).innerHTML = `Please Enter valid amount for refund`;
        }
    }
    else {

        document.getElementById("erroramount_" + id).innerHTML = `Please Enter amount for refund`;
    }
};


function change_pin(id) {

    var data = {}
    data.postal = document.getElementById("postal_" + id).value;
    fetch("/Admin/change_postal", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(data)
    }).then(res => res.json()).then(datafromcontroller => {

        console.log(datafromcontroller.city);
        document.getElementById("city_" + id).value = datafromcontroller.city;

    }).catch(err => console.log(err));

}