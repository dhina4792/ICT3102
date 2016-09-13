//*Data Analysis Validation Functions*//
//Default run when document loaded

///*Min and Max Input Functions*/
function stripNonNumeric(source) {
    if (source !== undefined) {
        //This will strip out all non-numeric values except decimal point .
        source.value = source.value.replace(/[^0-9.]/g, "");
    }
}

// Don't allow you to type non-numeric values
function isNumber(evt) {
    if (evt !== undefined) {
        evt = (evt) ? evt : window.event;
        var charCode = (evt.which) ? evt.which : evt.keyCode;
        if (charCode > 31 && ((charCode < 48 && charCode != 46) || charCode > 57)) {
            return false;
        }
        return true;
    }
}

// Compare your min and max values
function validateTextBoxes(arg) {
    //Testing
    var id = arg.getAttribute('id');
    var num = Number(id.split("_")[1], 4);

    var firstinputIdNaming = "min_";
    var secondinputIdNaming = "max_";

    var minID = firstinputIdNaming.concat(num); //Final ID Value.
    var maxID = secondinputIdNaming.concat(num); //Final ID Value.

    // Get the values
    var min = parseFloat(document.getElementById(minID).value, 10);
    var max = parseFloat(document.getElementById(maxID).value, 10);

    // Perform your comparison
    if (min > max && max != 0) {
        alert("Min cannot be greater then Max.");
        // Clear out b and give it focus
        document.getElementById(minID).value = '';
        document.getElementById(minID).focus();
    }
    else if (min != null) {
        return true;
    }
}