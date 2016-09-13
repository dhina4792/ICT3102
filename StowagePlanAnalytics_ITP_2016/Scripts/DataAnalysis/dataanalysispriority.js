//*Data Analysis Priority Functions*//
//Default run when document loaded
$(function () {
    initializeTablePrioritySetting();
    intializeTableCRUD();

    //For the export button to fade in
    if ($(this).scrollTop() == 0) {
        $('#exportBtn').fadeIn();
    }
});

function initializeTablePrioritySetting() {
    //Setup: Table to be sortable with jquery
    initializeTableSorting();

    //Setup: Up Down Sort Function
    initializeUpDownSorting();
}

//Set up CRUD for Table Priority Sorting
function intializeTableCRUD() {
    //Setup: Delete row Function
    delete_row();

    //Setup: Add row Function
    add_row();
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

//Add row to table
function add_row() {
    $("#addBtn").on("click", function () {
        var highestIDvalue = retrieve_HighestID(); // Retrieve ID Highest Value in the table
        var incrementid = parseInt(highestIDvalue) + 1; //Increment the id
        var tableRowIdNaming = "sectionsid_";
        var rowID = tableRowIdNaming.concat(incrementid); //Final ID Value.

        var selectedDDLValueNaming = "selectoptionid_"
        var dropdownID = selectedDDLValueNaming.concat(incrementid); //Final ID Value.

        var inputMinValueNaming = "inputMinID_"
        var inputMinID = inputMinValueNaming.concat(incrementid); //Final ID Value.

        var inputMaxValueNaming = "inputMaxID_"
        var inputMaxID = inputMaxValueNaming.concat(incrementid); //Final ID Value.

        var firstinputIdNaming = "min_";
        var secondinputIdNaming = "max_";
        var minID = firstinputIdNaming.concat(incrementid); //Final ID Value.
        var maxID = secondinputIdNaming.concat(incrementid); //Final ID Value.

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
            "<td class='col-sm-3'>" +
            "   <select class='form-control' name='" + dropdownID + "' id='exampleSelect1'>" +
            "       <option value='CIPlanned' >Crane Intensity</option>" +
            "       <option value='Ballast' >Ballast(mt)</option>" +
            "       <option value='TotalMoves' >Move Count</option>" +
            "   </select>" +
            "</td>" +
            "<td>" +
            "   <input type='number' class='form-control' value='0' min='0'  name='" + inputMinID + "' id='" + minID + "' step='any'" +
            "       onblur='stripNonNumeric(this)' onkeyup='validateTextBoxes(this);' onkeypress='return isNumber(event)'>" +
            "</td>" +
            "<td>" +
            "   <input type='number' class='form-control' value='0' min='0' name='" + inputMaxID + "'  id='" + maxID + "' step='any'" +
            "       onblur='stripNonNumeric(this)' onkeyup='validateTextBoxes(this);' onkeypress='return isNumber(event)'>" +
            "</td>" +
            "<td class='col-sm-1'>" +
            "   <a class='btn btn-delete btn-danger btn-block'><span class='glyphicon glyphicon-trash'></span></a>" +
            "</td>" +
            "</tr>";
        $("#diagnosis_list").append(newRowTest);
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





