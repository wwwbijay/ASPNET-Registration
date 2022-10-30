// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    const hideClass = 'hideForm';
    let solo = $('#solo');
    let group = $('#group');
    let mix = $('#solo,#group');

    solo.addClass(hideClass);
    group.addClass(hideClass);
    

    $('#PerformanceType').on('change', function () {
        
        let performance_type = $('#PerformanceType').val();
        
        if (performance_type == '0') {
           mix.addClass(hideClass);
        } else if (performance_type=='solo') {
            solo.removeClass(hideClass);
            group.addClass(hideClass);
        } else {
            solo.addClass(hideClass);
            group.removeClass(hideClass);
        }
    });

    $("#check_payment_status").click(function () {

        let tid = $(this).data('assigned-id');
        let requestUrl = 'Account/CheckPaymentStatus/?transaction_id='+ tid;
     
        console.log(requestUrl);
        
        $.get(requestUrl)
            .done(function (responsedata) {
                console.log("success");
                $("#partialViewContent").html(responsedata);
                $('#paymentStatusModal').modal('show')
            })
            .fail(function () {
                $("#partialViewContent").html("Sorry. No Data available.");
                $('#paymentStatusModal').modal('show')
              
            })
            .always(function () {
                // console.log("finished");
            });
    });
    
});