

function getRadioVal(name) {
    var str = "";
    $("input[name='" + name + "']:radio").each(function () {
        if ($(this).attr("checked")) {
            str += $(this).val() + ","
        }
    })
    if (str.length > 0) {
        var reg = /,$/gi;
        str = str.replace(reg, "");
    }
    return str;
}
/*表单数据合法性验证*/
function checkCombox() {
    $(".combo-text").focus(function () {
        this.select();
    })
    var combobox = $(this).parent();
    $(".combo-text").blur(function () {
        if ($(this).attr("readonly") != undefined) return;
        whname = $(this).val();
        ifEmpty = true
        $(this).parent().prev().find("option").each(function (index) {
            if ($(this).val() == whname) {
                ifEmpty = false;
                return false;
            }
        })
        if (ifEmpty) {
            $(this).val("请从下拉框中选择数据");
            $(this).siblings(".combo-value:last").val("");
        };
    })
}


function goDefault() {
    parent.location.href = "/home/";
}

// head + 123,456.00
// 将数值四舍五入(保留2位小数)后格式化成金额形式  
// @param num 数值(Number或者String)  
// @return 金额格式的字符串,如'1,234,567.45'  
// @type String  
function formatCurrency(num, head) {
    num = num.toString().replace(/\$|\,/g, '');
    if (isNaN(num))
        num = "0";
    sign = (num == (num = Math.abs(num)));
    num = Math.floor(num * 100 + 0.50000000001);
    cents = num % 100;
    num = Math.floor(num / 100).toString();
    if (cents < 10)
        cents = "0" + cents;
    for (var i = 0; i < Math.floor((num.length - (1 + i)) / 3) ; i++)
        num = num.substring(0, num.length - (4 * i + 3)) + ',' +
    num.substring(num.length - (4 * i + 3));
    return head + (((sign) ? '' : '-') + num + '.' + cents);
}

function trim(str) { //删除左右两端的空格
    return str.replace(/(^\s*)|(\s*$)/g, "");
}
function ltrim(str) { //删除左边的空格
    return str.replace(/(^\s*)/g, "");
}
function rtrim(str) { //删除右边的空格
    return str.replace(/(\s*$)/g, "");
}
function isNumber(str) { //是否是数字
    var reg = /^[0-9]*$/;
    return reg.test(str);
}


