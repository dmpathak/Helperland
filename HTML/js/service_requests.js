$(document).ready(function () {
    $('#example').DataTable({
        dom: 'tlp',
        responsive: true,
        'columnDefs': [{ 'orderable': false, 'targets': [5] }],
        "language": {
            "paginate": {
                "previous": '<img src="img/polygon-1-copy-5.png" alt="">',
                "next": '<img src="img/polygon-1-copy-5.png" style="transform:rotate(180deg)" alt="">'
            },
        }
    });
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