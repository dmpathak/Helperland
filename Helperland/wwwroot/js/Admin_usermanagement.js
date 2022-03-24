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


    var user_name = document.getElementById("user_name")
    var user_role = document.getElementById("user_role")
    var mobile = document.getElementById("mobile")
    var zip = document.getElementById("zip")
    var search_data = document.getElementById("search_data")
    var clear_data = document.getElementById("clear_data")
    var from_dt = document.getElementById("from_dt")
    var to_dt = document.getElementById("to_dt")

    search_data.addEventListener("click", (e) => {
        e.preventDefault();
        $.fn.dataTable.ext.search.push(function (settings, data, dataIndex) {
            let userName = data[0].trim();
            let regiDate = data[1].trim();
            let userType = data[2];
            let phoneNumber = data[3];
            const isUserName = user_name.value !== "" ? userName == user_name.value : true;
            const isUserType = user_role.value === "User Role" ? true : userType === user_role.value;
            const isPhoneNumber = mobile.value ? phoneNumber.includes(mobile.value) : true;
            const dateSplited = regiDate.split("/");
            const srdate = new Date(parseInt(dateSplited[2]), parseInt(dateSplited[1]) - 1, parseInt(dateSplited[0]));
            const isDateGreater = from_dt.value ? srdate >= new Date(from_dt.value) : true;
            const isDateSmaller = to_dt.value ? srdate <= new Date(to_dt.value) : true;
            console.log(isUserName, isUserType, isPhoneNumber, isDateGreater, isDateSmaller);
            return isUserName && isUserType && isPhoneNumber && isDateGreater && isDateSmaller;
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




$(document).on('click', ".activate", function (e) {
    $('.popover').popover('hide');
    //alert(e.target.id);

    send = {}
    send.id_for_user = e.target.id;

    fetch("/Admin/activate", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(send)
    }).then(res => res.json()).then(datafromcontroller => {

        //alert(datafromcontroller);
        window.location.reload();


    }).catch(err => console.log(err));



});


$(document).on('click', ".Deactivate", function (e) {
    $('.popover').popover('hide');
    //alert(e.target.id);

    send = {}
    send.id_for_user = e.target.id;

    fetch("/Admin/deactive", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(send)
    }).then(res => res.json()).then(datafromcontroller => {

        //alert(datafromcontroller);
        window.location.reload();

    }).catch(err => console.log(err));



});