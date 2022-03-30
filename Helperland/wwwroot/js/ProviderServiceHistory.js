$(document).ready(function () {
    var table = $('#example').DataTable({
        dom: 'Btlip',
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




// for map 
function for_map_show(id) {

    var postalcode = document.getElementById("mappostal_" + id).value;

    //alert(postalcode);
    fetch(`https://api.mapbox.com/geocoding/v5/mapbox.places/` +
        postalcode +
        `.json?country=de&limit=1&types=postcode&access_token=pk.eyJ1IjoiY2hpbnRhbjgxNjkiLCJhIjoiY2tvZWNiaTdhMDljeDJwbGoxdTV6eW9ocyJ9.ZTVOwDvOJqnfEKpBWgUvbg`,
        {
        method: "GET",
        }).then(res => res.json()).then(result => {

        var coordinates = result.features[0].geometry.coordinates;
            document.getElementById("second_part_modal_"+id).innerHTML = `<iframe src="https://maps.google.com/maps?q=` + coordinates[1] + `,` + coordinates[0] + `&z=16&output=embed" height="100%" width="100%" style="border:0px; min-height:400px;"></iframe>`

    }).catch(err => console.log(err));
};