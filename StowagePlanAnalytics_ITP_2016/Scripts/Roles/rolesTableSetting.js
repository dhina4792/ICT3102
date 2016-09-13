//*Port Setting*//
//Default run when document loaded
$(function () {
    // Fix bug dropdown not displaying
    $(".dropdown-toggle").dropdown();
    dataTableSetting();


});

function dataTableSetting() {
    $('#dev-table').DataTable({
        "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
        'iDisplayLength': 10,//Set Row Per Page
        "bFilter": true,//Remove Search - false mean remove
        "bPaginate": true,//Remove Pagination
        "bInfo": true,//Remove Page Info
        "bLengthChange": true,//Show per Page Dropdown Remove
        "columnDefs": [{ "targets": 0, "orderable": false }],//Remove Colum Orderable(Here Col 0 Remove)
        "sPaginationType": "full_numbers"//Full Pagination
    });
}