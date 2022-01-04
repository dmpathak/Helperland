$(document).ready(function () {
    $('#example').DataTable({
        dom: 'tlp',
        'columnDefs': [{ 'orderable': false, 'targets': [1, 2, 4, 7] }],
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