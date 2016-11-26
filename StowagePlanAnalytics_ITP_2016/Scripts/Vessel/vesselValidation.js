//*Vessel Validation Functions*//

//Default run when document loaded
$(function () {
});

//Validate PortCode Input
function validatePortCode(arg) {
    console.time("validatePortCode");
    console.profile("validatePortCode");
    var pattern;
    var id = arg.getAttribute('id');

    if (id == "vesselcode") //portcode
    {
        pattern = new RegExp(/[@~`!#$%\^&*+=\-\[\]\\';,/{}|\\":<>\?]/); //unacceptable chars
    }
    else //portname
    {
        pattern = new RegExp(/[0-9@~`!#$%\^&*+=\-\[\]\\';,/{}|\\":<>\?]/); //unacceptable chars and number
    }

    var inputValue = document.getElementById(id).value;

    //Check whether there is white space / tab in the input value.
    if (hasWhiteSpace(inputValue)) {
        if (id != "vesselname") {
            clearInputValue(id);
            alert("Empty space or tab are not allowed.");
            return false;
        }
        else
        {
            upsizeLetter(id, inputValue);
            return true; //valid inputs value
        }
    }
    else {

        if (pattern.test(inputValue)) {
            if (id == "vesselcode") {
                clearInputValue(id);
                alert("Please only use standard alphanumerics without special characters.");
                return false;
            }
            else if (id == "vesselname") {
                clearInputValue(id);
                alert("Please only use standard alpha without special characters or numbers");
                return false;
            }
        }
        upsizeLetter(id, inputValue);
        return true; //valid inputs value
    }
    console.profileEnd("validatePortCode");
    console.timeEnd("validatePortCode");
}

/*Check for White Space*/
function hasWhiteSpace(s) {
    console.time("hasWhiteSpace");
    console.profile("hasWhiteSpace");
    return /\s/g.test(s);
    console.profileEnd("hasWhiteSpace");
    console.timeEnd("hasWhiteSpace");
}

/*Clear Invalid InputValue*/
function clearInputValue(id)
{
    console.time("clearInputValue");
    console.profile("clearInputValue");
    document.getElementById(id).value = '';
    document.getElementById(id).focus();
    console.profileEnd("clearInputValue");
    console.timeEnd("clearInputValue");
}

///*Strip out all non-numeric values except decimal point Functions*/
function stripNonNumeric(source) {
    console.time("stripNonNumeric");
    console.profile("stripNonNumeric");
    if (source !== undefined) {
        //This will strip out all non-numeric values except decimal point .
        source.value = source.value.replace(/[^0-9]/g, "");
    }
    console.profileEnd("stripNonNumeric");
    console.timeEnd("stripNonNumeric");
}

// Don't allow you to type non-numeric values
// charCode == 46 (Remove decimal point)
function isNumber(evt) {
    console.time("isNumber");
    console.profile("isNumber");
    if (evt !== undefined) {
        evt = (evt) ? evt : window.event;
        var charCode = (evt.which) ? evt.which : evt.keyCode;
        if (charCode > 31 && ((charCode < 48 && charCode == 46) || charCode > 57)) {
            return false;
        }
        return true;
    }
    console.profileEnd("isNumber");
    console.timeEnd("isNumber");
}

/*Convert lower case letter to upper case*/
function upsizeLetter(id, value)
{
    console.time("upsizeLetter");
    console.profile("upsizeLetter");
    var resValue = value.toUpperCase();
    document.getElementById(id).value = resValue;
    console.profileEnd("upsizeLetter");
    console.timeEnd("upsizeLetter");
}

