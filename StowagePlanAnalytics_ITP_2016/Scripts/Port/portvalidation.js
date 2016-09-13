//*Port Validation Functions*//

//Default run when document loaded
$(function () {
});

//Validate PortCode Input
function validatePortCode(arg) {
    var pattern;
    var id = arg.getAttribute('id');

    if (id == "portcode") //portcode
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
        clearInputValue(id);
        alert("Empty space or tab are not allowed.");
        return false;
    }
    else {

        if (pattern.test(inputValue)) {
            if (id == "portcode") {
                clearInputValue(id);
                alert("Please only use standard alphanumerics without special characters.");
                return false;
            }
            else if (id == "portname") {
                clearInputValue(id);
                alert("Please only use standard alpha without special characters or numbers");
                return false;
            }
        }
        upsizeLetter(id, inputValue);
        return true; //valid inputs value
    }
}

/*Check for White Space*/
function hasWhiteSpace(s) {
    return /\s/g.test(s);
}

/*Clear Invalid InputValue*/
function clearInputValue(id)
{
    document.getElementById(id).value = '';
    document.getElementById(id).focus();
}

///*Strip out all non-numeric values except decimal point Functions*/
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

/*Convert lower case letter to upper case*/
function upsizeLetter(id, value)
{
    var resValue = value.toUpperCase();
    document.getElementById(id).value = resValue;
    //document.getElementById(id).focus();
}

