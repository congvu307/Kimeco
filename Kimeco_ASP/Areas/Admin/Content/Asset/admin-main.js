$('.cash-approve').on("click", function () {
    $('#cash-spending-id').val($(this).data('id'));
    $('#cash-spending-form').submit();
})
$('#Date').datepicker({
    container: '#salaryDate',
    format: 'd/m/yyyy',
    autoclose: true
});