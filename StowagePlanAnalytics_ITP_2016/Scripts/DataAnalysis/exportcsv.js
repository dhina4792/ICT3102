$(document).ready(function () {

    function exportTableToCSV(filename) {
        console.time("exporttoCSV");
        console.profile("export");
        var csvContent = "";

        // Header row for the csv file
        var insertRow = "VovageNo.,\tTEU Capacity (TEU),\tTEU Onboard (TEU),\tTEU Remaining (TEU),\tTEU Utilisation (%)," +
            "\tWeight Capacity (MT),\tWeight Onboard (MT),\tWeight Remaining (MT),\tWeight Utilisation (%)," +
            "\tIMO Onboard(units),\tIMO Utilisation (%)," +
            "\tOOG Onboard(Units),\tOOG Utilisation(%)," +
            "\tReefer Capacity (Units),\tReefer Onboard (Units),\tReefer Remaining (Units),\tReefer Utilisation(%)," +
            "\tMove Count: Loaded (Moves),\tMove Count: Discharged (Moves),\tMove Count: Re-stows (Moves),\tMove Count: Re-shifting (Moves),\tMove Count: Total (Moves)," +
            "\tBallast (m3),\tBunker Oil HFO (m3),\tBunker Oil MGOMDO (m3)," +
            "\tOwner,\t";
        csvContent += insertRow + "\n\n";

        // Get all tables that the user wants to export (4 sets of table per entry)
        var VoyageIds = document.getElementsByClassName("isCheckedVoyage");
        var MainTables = document.getElementsByClassName("isCheckedMain");
        var MoveCountTables = document.getElementsByClassName("isCheckedMoveCount");
        var TankTables = document.getElementsByClassName("isCheckedTank");
        var OwnerTables = document.getElementsByClassName("isCheckedOwner");
        console.log(MainTables.length);
        for (j = 0; j < MainTables.length; j++) {

            var VoyageId = VoyageIds[j].textContent;
            var MainTable = MainTables[j].textContent.split("\n");
            var MoveCountTable = MoveCountTables[j].textContent.split("\n");
            var TankTable = TankTables[j].textContent.split("\n");
            var OwnerTable = OwnerTables[j].textContent.split("\n");

            // Remove empty element from the array
            MainTable = MainTable.filter(function (x) {
                return /\S/.test(x);
            });

            MoveCountTable = MoveCountTable.filter(function (x) {
                return /\S/.test(x);
            });

            TankTable = TankTable.filter(function (x) {
                return /\S/.test(x);
            });

            OwnerTable = OwnerTable.filter(function (x) {
                return /\S/.test(x);
            });

            // Trim all values in the OwnerTable
            for (i = 2; i < OwnerTable.length; i++) {
                OwnerTable[i] = OwnerTable[i].trim();
            }

            // Match the owner with the number of containers on the vessel
            var secondRowOfTable = OwnerTable.indexOf("Container") - 1;
            var newOwnerList = new Array();
            for (i = 2; i < OwnerTable.length; i++) {
                if (OwnerTable[i] != "Container") {
                    newOwnerList.push(OwnerTable[i] + ": " + OwnerTable[i + secondRowOfTable]);
                } else {
                    break;
                }
            }

            // Data row for the csv file
            var insertRow = VoyageId + ",\t" + MainTable[8].trim() + ",\t" + MainTable[14].trim() + ",\t" + MainTable[20].trim() + ",\t" + MainTable[27].trim() +
                ",\t" + MainTable[9].trim() + ",\t" + MainTable[15].trim() + ",\t" + MainTable[21].trim() + ",\t" + MainTable[29].trim() +
                ",\t" + MainTable[16].trim() + ",\t" + MainTable[31].trim() +
                ",\t" + MainTable[17].trim() + ",\t" + MainTable[33].trim() +
                ",\t" + MainTable[12].trim() + ",\t" + MainTable[18].trim() + ",\t" + MainTable[24].trim() + ",\t" + MainTable[35].trim() +
                ",\t" + MoveCountTable[8].trim() + ",\t" + MoveCountTable[9].trim() + ",\t" + MoveCountTable[10].trim() + ",\t" + MoveCountTable[11].trim() + ",\t" + MoveCountTable[12].trim() +
                ",\t" + TankTable[6].trim() + ",\t" + TankTable[7].trim() + ",\t" + TankTable[8].trim();

            for (i = 0; i < newOwnerList.length; i++) {
                insertRow = insertRow + ",\t" + newOwnerList[i];
            }

            csvContent += insertRow + "\n";
        }

        csvData = 'data:application/csv;charset=utf-8,' + encodeURIComponent(csvContent);
        // Configure the file to be downloaded
        $(this)
            .attr({
                'download': "Export_" + timeStamp() + ".csv",
                'href': csvData,
                'target': '_blank'
            });
        console.profileEnd("export");
        console.timeEnd("exporttoCSV");
    }

    $(".export").click(function () {
        exportTableToCSV.apply(this);
    });

    $(".referencecheckbox").click(function () {
        var VoyageId = $("#voyage-" + this.id);
        var MainTable = $("#table-main-" + this.id);
        var MoveCountTable = $("#table-movecount-" + this.id);
        var TankTable = $("#table-tank-" + this.id);
        var OwnerTable = $("#table-owner-" + this.id);

        //console.log(VoyageId);
        //console.log(MainTable);
        //console.log(MoveCountTable);
        //console.log(TankTable);
        //console.log(OwnerTable);

        if (this.checked) {
            // "isChecked" is used to identity the table that will be exported out
            VoyageId.addClass("isCheckedVoyage");
            MainTable.addClass("isCheckedMain");
            MoveCountTable.addClass("isCheckedMoveCount");
            TankTable.addClass("isCheckedTank");
            OwnerTable.addClass("isCheckedOwner");
        } else {
            VoyageId.removeClass("isCheckedVoyage");
            MainTable.removeClass("isCheckedMain");
            MoveCountTable.removeClass("isCheckedMoveCount");
            TankTable.removeClass("isCheckedTank");
            OwnerTable.removeClass("isCheckedOwner");
        }
    })

    function timeStamp() {
        // Create a date object with the current time
        var now = new Date();

        // Create an array with the current month, day and time
        var date = [now.getMonth() + 1, now.getDate(), now.getFullYear()];

        // Create an array with the current hour, minute and second
        var time = [now.getHours(), now.getMinutes(), now.getSeconds()];

        // If seconds and minutes are less than 10, add a zero
        for (var i = 1; i < 3; i++) {
            if (time[i] < 10) {
                time[i] = "0" + time[i];
            }
        }

        // Return the formatted string
        return date.join("/") + "_" + time.join(":");
    }

});
