var pageRefreshFlag = false;

$(function () {
    $.ajaxSetup({ cache: false });
    //$(".popup").on("click", function (e) {
    $("a[data-modal]").on("click", function (e) {
        $('#myModalContent').load(this.href, function () {
            $('#myModal').modal({
                keyboard: true
            }, 'show');
            // Setup listener for modal on hide
            $('#myModal').on('hidden.bs.modal', function () {
                if (pageRefreshFlag) {
                    pageRefreshFlag = false;
                    location.reload();
                }
            })
            bindForm(this);
        });
        
        return false;
    });
});

function bindForm(dialog) {
    $('form', dialog).submit(function () {
        //var formData = $(this).serialize();
        $('#progress').show();
        if (this.enctype == "multipart/form-data") {
            //formData = new FormData(this);
            return true;    //Allow submit button to work normally (No ajax)
        }
        else {
            $.ajax({
                url: this.action,
                type: this.method,
                //contentType: this.enctype,
                //data: formData,
                data: $(this).serialize(),
                //data: new FormData(this),
                //dataType: "json",
                success: function (result) {
                    //if (result.success) {
                    //    $('#myModal').modal('hide');
                    //    pageRefreshFlag = true;
                    //    $('#progress').hide();
                    //    if (result.message != null) {
                    //        alert(result.message);
                    //    }
                    //    location.reload();
                    //    //alert("Your password has been successfully changed.");
                    //} else {
                    //    $('#progress').hide();
                    //    $('#myModalContent').html(result);

                    //    bindForm();
                    //}

                    // Set refresh page flag
                    pageRefreshFlag = true;
                    $('#myModalContent').html(result);
                    bindForm();
                }
            });
        }
        
        return false;    // Prevent submit button default action
    });
}
