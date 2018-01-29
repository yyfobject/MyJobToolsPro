//Url 为分页Ajax方式的访问地址。如果为空则认识是跳转分页，页面会刷新。
//pdata.setExtend(@Html.Raw(pageModel.GetScriptJson()));
var ishavedata = 0
var PageData = {
    defaults: {
        Url: '',
        ExpUrl: '',
        ExpUrlNpoi: '',
        Index: 1,
        PageSize: 10,
        Data: function () { return {}; }, //参数
        HtmlData: function (data) { },
        PagerID: '',
        FirstPageText: '首页',
        LastPageText: '尾页',
        PrePageText: '上一页',
        NextPageText: '下一页',
        RowsCount: 0
    },
    settings: {},
    init: function (options) {
        $.extend(this.settings, this.defaults, options);
        this.HtmlPage(this.settings.RowsCount, this.settings, null);
    },
    setExtend: function (options) {
        $.extend(this.settings, options);
        this.HtmlPage(this.settings.RowsCount, this.settings, null);
    },
    GetData: function () {
        para = this.settings.Data();
        para["page"] = this.settings.Index;
        para["pagesize"] = this.settings.PageSize;
        var HtmlData = this.settings.HtmlData;
        var HtmlPage = this.HtmlPage;
        var settings = this.settings;
        HtmlPage(1, settings, null);
        if (!this.settings.Url || this.settings.Url == '') {
            var myurl = new UrlPager.URL();//new UrlPager.URL(window.location.href);
            myurl.set("page", this.settings.Index);
            myurl.set("pagesize", this.settings.PageSize);
            for (var o in para) {
                if (para[o] && para[o] != "") {
                    myurl.set(o, para[o]);
                }
                else {
                    myurl.remove(o);
                }
            }
            window.location.href = myurl.url();
            $.fn.jqLoading({ height: 100, width: 240, text: "正在加载中，请耐心等待...." });
            return false;
        }
        $.ajax({
            url: this.settings.Url,
            //contentType:"application/x-www-form-urlencoded;charset=UTF-8",
            type: "post",
            data: para,
            dataType: "json",
            cache: false,
            beforeSend: function () {
                $.fn.jqLoading({ height: 100, width: 240, text: "正在加载中，请耐心等待...." });
            },
            complete: function (data) {
                $.fn.jqLoading("destroy");
            },
            success: function (data) {
                if (data.result == 100) {
                    ishavedata = data.recordcount;
                    HtmlData(data);
                    HtmlPage(data.recordcount, settings, data);
                }
                else if (data.result == 101) {
                    alert("登录超时，请重新登录！");
                    top.location.href = "../login.asp";
                }
                else if (data.result == 103) {
                    ishavedata = data.recordcount;
                    HtmlData(data);
                    HtmlPage(data.recordcount, settings, data);
                    alert(data.message);
                }
                else if (data.result == 102) {
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert("发生错误。系统参数：" + XMLHttpRequest.status + "," + XMLHttpRequest.readyState + "," + textStatus);
            }
        });
    },
    ExportToExcelDispose: function () {
        para = this.settings.Data();
        para["page"] = 0;
        var settings = this.settings;
        if (ishavedata == 0) {
            alert('没有数据要导出！');
        } else {
            $.ajax({
                url: this.settings.ExpUrl,
                type: "post",
                data: para,
                dataType: "json",
                cache: false,
                timeout: 3000000,
                beforeSend: function () {
                    $.fn.jqLoading({ height: 100, width: 240, text: "正在加载中，请耐心等待...." });
                },
                complete: function (data) {
                    $.fn.jqLoading("destroy");
                },
                success: function (data) {
                    if (data.result == 100) {
                        window.location.href = data.message;
                    }
                    else if (data.result == 101) {
                        alert("登录超时，请重新登录！");
                        top.location.href = "../login.aspx";
                    }
                    else {
                        alert(data.message);
                    }
                }, error: function () {
                    //alert("数据有误！");
                }
            });
        }
    },
    ExportToExcelDisposeNpoi: function () {
        para = this.settings.Data();
        para["page"] = 0;
        var settings = this.settings;
        if (ishavedata == 0) {
            alert('没有数据要导出！');
        } else {
            //$.fn.jqLoading({ height: 100, width: 240, text: "正在加载中，请耐心等待...." });

            //var expiframe = document.createElement("iframe");
            //expiframe.name = "expiframe";
            //expiframe.style.display = "none";
            //document.body.appendChild(expiframe);
            //expiframe.load = function () {
            //    $.fn.jqLoading("destroy");
            //    document.body.removeChild(expiframe);
            //};
            

            var tempForm = document.createElement("form");
            tempForm.id = "tempForm1";
            tempForm.method = "post";
            tempForm.action = this.settings.ExpUrlNpoi;
            tempForm.target = "_blank";
	    tempForm.acceptCharset= "UTF-8";
            for (var key in para) {
                var hideInput = document.createElement("input");
                hideInput.type = "hidden";
                hideInput.name = key
                hideInput.value = para[key];
                tempForm.appendChild(hideInput);
            }

            document.body.appendChild(tempForm);
            tempForm.submit();
            document.body.removeChild(tempForm);
        }
    },
    HtmlPage: function (count, settings, data) {
        var size = parseInt(settings.PageSize);
        var index = parseInt(settings.Index);
        var PageCount = count % size == 0 ? count / size : Math.floor(count / size) + 1;
        //填充数据
        if (data != null) {
            $("#pspan").text(data.data.length);
        }
        else {
            $("#pspan").text(settings.RowsCount);
        }
        $("#tspan").text(count);
        $("#totcountpage").val(PageCount);
        $("#txtcurrpage").val(index);
        if (index <= 1) {
            $("#firstpage").addClass("disable");
            $("#prevpage").addClass("disable");
            $("#nextpage").removeClass("disable");
            $("#lastpage").removeClass("disable");
        }
        else if (index >= PageCount) {
            $("#nextpage").addClass("disable");
            $("#lastpage").addClass("disable");
            $("#firstpage").removeClass("disable");
            $("#prevpage").removeClass("disable");
        }
        else {
            $("#nextpage").removeClass("disable");
            $("#lastpage").removeClass("disable");
            $("#firstpage").removeClass("disable");
            $("#prevpage").removeClass("disable");
        }
        if (PageCount == 0) {
            $("#nextpage").addClass("disable");
            $("#lastpage").addClass("disable");
            $("#firstpage").addClass("disable");
            $("#prevpage").addClass("disable");
        }
        //事件
        $("#pagesizeselect").unbind("change");
        $("#pagesizeselect").change(function () {
            PageData.settings.PageSize = $(this).val();
            PageData.settings.Index = 1;
            PageData.GetData();
        })

        $("#pagego").unbind("click");
        $("#pagego").unbind("change");
        $("#pagego").click(function () {
            PageData.settings.Index = $("#txtcurrpage").val();
            PageData.GetData();
        });

        $("#firstpage").unbind("click");
        $("#prevpage").unbind("click");
        if (index > 1) {
            $("#firstpage").click(function () {
                PageData.settings.Index = 1;
                PageData.GetData();
            })
            $("#prevpage").click(function () {
                PageData.settings.Index -= 1;
                PageData.GetData();
            })
        }
        $("#nextpage").unbind("click");
        $("#lastpage").unbind("click");
        if (index < PageCount) {
            $("#nextpage").click(function () {
                PageData.settings.Index += 1;
                PageData.GetData();
            })
            $("#lastpage").click(function () {
                PageData.settings.Index = PageCount;
                PageData.GetData();
            })
        }
    }
};
//时间
function DayNumOfMonth(startTimeStr) {
    var startTime = new Date(startTimeStr);
    Year = startTime.getYear();
    Month = startTime.getMonth();
    var d = new Date(Year, Month, 0);
    return d.getDate();
}
function GetDateDiff(startTime, endTime, diffType) {
    startTime = startTime.replace(/\-/g, "/");
    endTime = endTime.replace(/\-/g, "/");
    diffType = diffType.toLowerCase();
    var sTime = new Date(startTime);
    var eTime = new Date(endTime);
    var divNum = 1;

    switch (diffType) {
        case "second":
            divNum = 1000;
            break;
        case "minute":
            divNum = 1000 * 60;
            break;
        case "hour":
            divNum = 1000 * 3600;
            break;
        case "day":
            divNum = 1000 * 3600 * 24;
            break;
        default:
            break;
    }
    return parseInt((eTime.getTime() - sTime.getTime()) / parseInt(divNum));
}

function buildRowsByTemplate(tempId, dataItem) {
    var htmlObj = $("#" + tempId).html();
    $.each(dataItem, function (i, val) {
        var showObj;
        //if (typeof (val) == "datetime") {

        //}
        if (!val) {
            val = "";
        }
        var rObj = "{{model." + i + "}}";
        var regObj = new RegExp(rObj, "g");
        htmlObj = htmlObj.replace(regObj, val);
    });
    return htmlObj;
}

function buildTableByTemplate(tempId, tableId, dataRows) {
    $.each(dataRows, function (i, dataItem) {
        var htmlObj = buildRowsByTemplate(tempId, dataItem);
        $("#" + tableId).append(htmlObj);
    });
}

//2017/10/13,yangyifeng,新增url操作对象
var UrlPager = (function (urlPager) {
    var objURL = function (url) {
        this.ourl = url || window.location.href;
        this.href = "";//?前面部分
        this.params = {};//url参数对象
        this.jing = "";//#及后面部分
        this.init();
    }
    //分析url,得到?前面存入this.href,参数解析为this.params对象，#号及后面存入this.jing
    objURL.prototype.init = function () {
        var str = this.ourl;
        var index = str.indexOf("#");
        if (index > 0) {
            this.jing = str.substr(index);
            str = str.substring(0, index);
        }
        index = str.indexOf("?");
        if (index > 0) {
            this.href = str.substring(0, index);
            str = str.substr(index + 1);
            var parts = str.split("&");
            for (var i = 0; i < parts.length; i++) {
                var kv = parts[i].split("=");
                this.params[kv[0]] = kv[1];
            }
        }
        else {
            this.href = this.ourl;
            this.params = {};
        }
    }
    //只是修改this.params
    objURL.prototype.set = function (key, val) {
        this.params[key] = val;
    }
    //只是设置this.params
    objURL.prototype.remove = function (key) {
        this.params[key] = undefined;
    }
    //根据三部分组成操作后的url
    objURL.prototype.url = function () {
        var strurl = this.href;
        var objps = [];//这里用数组组织,再做join操作
        for (var k in this.params) {
            if (this.params[k]) {
                objps.push(k + "=" + this.params[k]);
            }
        }
        if (objps.length > 0) {
            strurl += "?" + objps.join("&");
        }
        if (this.jing.length > 0) {
            strurl += this.jing;
        }
        return strurl;
    }
    //得到参数值
    objURL.prototype.get = function (key) {
        return this.params[key];
    }
    urlPager.URL = objURL;
    return urlPager;
}(UrlPager || {}));
//var myurl=new LG.URL("url");
//var myurl=new LG.URL(); //本页面url
//myurl.set("a","hello");
//myurl.get("a");
//myurl.remove("a");
//myurl.url(); //获取url