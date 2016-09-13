function FillDepartList() {
    var serviceCode = $('#service-ddl').val();
    var departCode = $('#DepartHidden').val();
    var destCode = $('#DestHidden').val();
    if (serviceCode != "All") {
        $.ajax({
            url: '/DataAnalysis/FillDepartPort',
            type: "GET",
            dataType: "JSON",
            data: { serviceCode: serviceCode },
            success: function (ports) {
                $("#depart-ddl").html("");
                $("#dest-ddl").html("");
                $("#depart-ddl").append(
                    $('<option></option>').val("All").html("All"));
                $("#dest-ddl").append(
                    $('<option></option>').val("All").html("All"));
                $.each(ports, function (i, port) {
                    $("#depart-ddl").append(
                        $('<option></option>').val(port).html(port));
                    $("#dest-ddl").append(
                        $('<option></option>').val(port).html(port));
                })
                /*To set the previously selected ports*/
                if (departCode != "" && destCode != "") {
                    $('#depart-ddl option[value=' + departCode + ']').prop("selected", true);
                    $('#dest-ddl option[value=' + destCode + ']').prop("selected", true);
                }
            }
        })
    }
    else {
        $.ajax({
            url: '/DataAnalysis/RetrieveAllPort',
            type: "GET",
            dataType: "JSON",
            success: function (ports) {
                $("#depart-ddl").html("");
                $("#dest-ddl").html("");
                $("#depart-ddl").append(
                    $('<option></option>').val("All").html("All"));
                $("#dest-ddl").append(
                    $('<option></option>').val("All").html("All"));
                $.each(ports, function (i, port) {
                    $("#depart-ddl").append(
                        $('<option></option>').val(port).html(port));
                    $("#dest-ddl").append(
                        $('<option></option>').val(port).html(port));
                })
                /*To set the previously selected ports*/
                if (departCode != "" && destCode != "") {
                    $('#depart-ddl option[value=' + departCode + ']').prop("selected", true);
                    $('#dest-ddl option[value=' + destCode + ']').prop("selected", true);
                }
            }
        })
    }


}


$(document).ready(function () {
    console.log($('#service-ddl').val());
    if ($('#service-ddl').val() != "") {
        FillDepartList();
    }
});