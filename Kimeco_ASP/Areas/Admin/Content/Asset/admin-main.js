$('.cash-approve').on("click", function () {
    $('#cash-spending-id').val($(this).data('id'));
    $('#cash-spending-form').submit();
})
$('#Date').datepicker({
    container: '#salaryDate',
    format: 'd/m/yyyy',
    autoclose: true
});
function formatMoney(n, c, d, t) {
    var c = isNaN(c = Math.abs(c)) ? 2 : c,
        d = d == undefined ? "." : d,
        t = t == undefined ? "," : t,
        s = n < 0 ? "-" : "",
        i = String(parseInt(n = Math.abs(Number(n) || 0).toFixed(c))),
        j = (j = i.length) > 3 ? j % 3 : 0;

    return s + (j ? i.substr(0, j) + t : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + t) + (c ? d + Math.abs(n - i).toFixed(c).slice(2) : "");
};
$(document).ready(function () {
    //$(".currency").each(function () {
    //    console.log($(this).text());
    //});
    //var listCurrency = $('.currency');
    //$.each(listCurrency, function (i, val) {
    //    $(this).html(formatMoney($(val).html()));
    //})
    //$('.fg-button').on('click',function () {
    //    $.each($('.currency'), function (i, val) {
    //        console.log($(val).html());
    //        $(this).html(formatMoney($(val).html()));
    //    })
    //})
})
