//*Service Setting*//
//Default run when document loaded
var iTableCounter = 1;
var oTable;
var oInnerTable;
var TableHtml;

$(function () {
    // Fix bug dropdown not displaying
    $(".dropdown-toggle").dropdown();

    dataTableSetting();
});

function dataTableSetting() {
    var table = $('#dev-table').DataTable({
        "autoWidth": true,
        "language": {
            infoEmpty: "There are currently no service available.",
            emptyTable: "There are currently no service available.",
            zeroRecords: "There are currently no service available."
        },
        "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
        'iDisplayLength': 10,//Set Row Per Page
        "bFilter": true,//Remove Search - false mean remove
        "bPaginate": true,//Remove Pagination
        "bInfo": true,//Remove Page Info
        "bLengthChange": true,//Show per Page Dropdown Remove
        "columnDefs": [{ "targets": 1, "orderable": false }],//Remove Colum Orderable(Here Col 0 Remove)
        "sPaginationType": "full_numbers"//Full Pagination
    });

    // Add event listener for opening and closing details
    $('#dev-table tbody').on('click', 'div.details-control', function () {
        var tr = $(this).closest('tr');
        var row = table.row(tr);

        var ServiceCode = $(this).closest('td').prev('.service_code').text();
        $.ajax({
            url: '/Service/Retrieve',
            type: "POST",
            data: { "ServiceCode": ServiceCode },
            "success": function (data) {
                if (data != null) {
                    var vdata = data;
                    if (row.child.isShown()) {
                        // This row is already open - close it
                        row.child.hide();
                        tr.removeClass('shown');
                    }
                    else {
                        // Open this row
                        row.child(format(vdata)).show();
                        tr.addClass('shown');
                    }
                }
            }
        });
    });
}

/*Dropdownlist populating*/
function Action(PortCode) {
    var num = 0;

    $("tr").click(function (event) {
        var id = $(this).attr('id');
        num = parseInt(this.id.split("_")[1], 10);
    });

    $.ajax({
        url: '/Service/Retrieve',
        type: "POST",
        data: { "PortCode": PortCode },
        "success": function (data) {
            if (data != null) {
                var vdata = data;
                document.getElementById("portnameid_" + num).value = vdata.PortName;
                //alert(num);
                //$("").val(vdata.PortName);
                //$("#ChainName").val(vdata[0].PortName);
            }
        }
    });
}

/* Formatting function for row details*/
function format(vdata) {
    var numPorts = vdata.PortList.length;

    var table = document.createElement('table'), thead, tbody, th, tr, td, row, cell;
    table.setAttribute('class', '<table table table-striped table-bordered table-list>');
    thead = document.createElement('thead');
    table.appendChild(thead);

    for (row = 1; row <= 2; row++) {
        if (row == 1) {
            tr = document.createElement('tr');
            th = document.createElement('th');
            th.setAttribute('class', '<"col-sm-3">'); //Set Class
            th.colSpan = 4; //Set Colspan
            tr.appendChild(th);
            th.innerHTML = "Service Port Order List";
        }
        else if (row == 2) {
            tr = document.createElement('tr');
            th = document.createElement('th');
            th.setAttribute('class', '<"col-sm-3">');
            tr.appendChild(th);
            th.innerHTML = "Sequence Number";
            th = document.createElement('th');
            th.setAttribute('class', '<"col-sm-3">');
            tr.appendChild(th);
            th.innerHTML = "Port Code";
            th = document.createElement('th');
            th.setAttribute('class', '<"col-sm-3">');
            tr.appendChild(th);
            th.innerHTML = "Port Name";
            th = document.createElement('th');
            th.setAttribute('class', '<"col-sm-3">');
            tr.appendChild(th);
            th.innerHTML = "STIF File to Upload?";
        }
        thead.appendChild(tr);
    }

    tbody = document.createElement('tbody');
    table.appendChild(tbody);

    for (row = 0; row < numPorts; row++)
    {
        tr = document.createElement('tr');
        td = document.createElement('td');
        td.setAttribute('class', '<"col-sm-3">');
        tr.appendChild(td);
        td.innerHTML = vdata.PortList[row].SequenceNo; //Sequence Number
        td = document.createElement('td');
        td.setAttribute('class', '<"col-sm-3">');
        tr.appendChild(td);
        td.innerHTML = vdata.PortList[row].PortCode; //Port Code
        td = document.createElement('td');
        td.setAttribute('class', '<"col-sm-3">');
        tr.appendChild(td);
        td.innerHTML = vdata.PortList[row].PortName; //Port Name
        td = document.createElement('td');
        td.setAttribute('class', '<"col-sm-3">');
        tr.appendChild(td);
        td.innerHTML = (vdata.PortList[row].FileUpload == true) ? "Yes" : "No";// vdata.PortList[row].FileUpload; //File Upload
        tr.appendChild(td);
        tbody.appendChild(tr);
    }
    return table;
}
