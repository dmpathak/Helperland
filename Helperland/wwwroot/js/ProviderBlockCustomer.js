$(document).ready(function () {
    var table = $('#example').DataTable({
        dom: 'btlip',
        responsive: false,
        pagingType: 'full_numbers',
        'columnDefs': [{ 'orderable': false, 'targets': [2] }],
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


function block(id) {
    data = {}

    data.UserId = id;

    fetch("/Provider/block", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(data)
    }).then(res => res.json()).then(datafromcontroller => {

        //alert("Customer Blocked..!!");
        document.getElementById("block_" + id).value = "Unblock";
        document.getElementById("block_" + id).classList.remove("action_button_block");
        document.getElementById("block_" + id).classList.add("action_button_unblock");

        window.location.reload();
    }).catch(err => console.log(err));

}
function unblock(id) {
    data = {}

    data.UserId = id;

    fetch("/Provider/unblock", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(data)
    }).then(res => res.json()).then(datafromcontroller => {

        //alert("Customer Blocked..!!");
        document.getElementById("unblock_" + id).value = "block";
        document.getElementById("unblock_" + id).classList.add("action_button_block");
        document.getElementById("unblock_" + id).classList.remove("action_button_unblock");

        window.location.reload();
    }).catch(err => console.log(err));

}