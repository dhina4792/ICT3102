//*Service Create Functions*//

//Default run when document loaded
$(function () {
    initializeTablePrioritySetting();
    intializeTableCRUD();
});

function initializeTablePrioritySetting() {
    //Setup: Table to be sortable with jquery
    initializeTableSorting();

    //Setup: Up Down Sort Function
    initializeUpDownSorting();
}

//Trigger up and down arrow for sorting
function initializeUpDownSorting() {
    //Up and Down 
    //Up and down sorting of rows and to backend for checking.
    //The rows are dynamically updated in the table DOM hierarchy
    $(document).on('click', '.moveuplink', function () {
        $(this).parents(".sectionsid").insertBefore($(this).parents(".sectionsid").prev());
        renumber_table('#diagnosis_list');
    });

    $(document).on('click', '.movedownlink', function () {
        $(this).parents(".sectionsid").insertAfter($(this).parents(".sectionsid").next());
        renumber_table('#diagnosis_list');
    });

    //Comment For myself
    //Click event only works if the element already exist in html code.
    //It won't consider the new element which is created dynamically after the page loaded.
    //You should use on function to trigger the event on dynamically created elements.
    //
    //$(".moveuplink").click(function () {
    //$(document).on('click', '.moveuplink', function () {
}

//Set up table to be able to sort.
function initializeTableSorting() {
    //Helper function to keep table row from collapsing when being sorted
    var fixHelperModified = function (e, tr) {
        var $originals = tr.children();
        var $helper = tr.clone();
        $helper.children().each(function (index) {
            $(this).width($originals.eq(index).width())
        });
        return $helper;
    };

    //Make diagnosis table sortable
    $("#diagnosis_list tbody").sortable({
        helper: fixHelperModified,
        stop: function (event, ui) { renumber_table('#diagnosis_list') }
    }).disableSelection();
}

//Renumber table rows priority
function renumber_table(tableID) {
    $(tableID + " tr").each(function () {
        count = $(this).parent().children().index($(this)) + 1;
        $(this).find('.priority').html(count);
    });
}

//Find out the highest id in the table
function retrieve_HighestID() {
    var max = 0;
    $(".sectionsid").each(function () {
        num = parseInt(this.id.split("_")[1], 10);
        if (num > max) {
            max = num;
        }
    });
    return max;
}

//Set up CRUD for Table Priority Sorting
function intializeTableCRUD() {
    //Setup: Delete row Function
    delete_row();

    //Setup: Add row Function
    add_row();
}



//Add row to table
function add_row() {
    $("#addBtn").on("click", function () {
        var highestIDvalue = retrieve_HighestID(); // Retrieve ID Highest Value in the table
        var incrementid = parseInt(highestIDvalue) + 1; //Increment the id
        var tableRowIdNaming = "sectionsid_";
        var rowID = tableRowIdNaming.concat(incrementid); //Final ID Value.

        var firstinputIdNaming = "min_";
        var secondinputIdNaming = "max_";
        var minID = firstinputIdNaming.concat(incrementid); //Final ID Value.
        var maxID = secondinputIdNaming.concat(incrementid); //Final ID Value.

        //var cloneSection = document.getElementById('sectionsid_1'),
        //    sectionResult = cloneSection.cloneNode(true); // Clone the previous form with its values

        $("#diagnosis_list tr:last").after($("#diagnosis_list tr:last").clone()); //Clone base on the last row data
        $("#diagnosis_list tr:last").attr("id", "sectionsid_" + incrementid); //Changing the ID for the last row
        $("#diagnosis_list input:last").attr("id", "portnameid_" + incrementid); //Changing the ID for the last

        // Clear out b and give it focus
        document.getElementById("portnameid_" + incrementid).value = '';

        var newRowTest = "" +
            "<tr class='sectionsid' id='" + rowID + "'> " +
            "<td class='priority col-sm-1'>" + incrementid + "</td>" +
            "<td class='col-sm-1'>" +
            "   <a href='javascript:void(0)' class='moveuplink'>" +
            "       <i class='glyphicon glyphicon-arrow-up'></i>" +
            "   </a>" +
            "   <a href='javascript:void(0)' class='movedownlink'>" +
            "       <i class='glyphicon glyphicon-arrow-down'></i>" +
            "   </a>" +
            "</td>" +
            "<td>" + 
            "   <select class='form-control' name='PortCode' id='PortCode'>" +
            "       <option>SIN</option>" +
            "       <option>YAT</option>" +
            "   </select>" +
            "</td>" +
            "<td>" +
            "   <input id='PortName' name='PortName' type='text' pattern='[a-zA-Z0-9-]+' class='form-control'>" +
            "</td>" +
            "<td class='col-sm-3'>" +
            "<select class='form-control' id='FileUpload' name='FileUpload'>"+
            "<option value='true'>Yes</option>" +
            "<option value='false'>No</option>" +
            "</select>"+
            "</td>" +
            "<td class='col-sm-1'>" +
            "   <a class='btn btn-delete btn-danger'><span class='glyphicon glyphicon-trash'></span></a>" +
            "</td>" +
            "</tr>";

        //$("#diagnosis_list").append(sectionResult);
        //Sort the priority number again
        renumber_table('#diagnosis_list')
    });
}

//Delete row from table
function delete_row() {
    //Delete button in table rows
    $('table').on('click', '.btn-delete', function () {
        tableID = '#' + $(this).closest('table').attr('id');
        r = confirm('Delete this item?');
        if (r) {
            $(this).closest('tr').remove();
        }

        //Sort the priority number again
        renumber_table('#diagnosis_list')
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
        url: '/Service/check',
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