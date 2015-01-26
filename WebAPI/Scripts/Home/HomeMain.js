$(document).ready(function () {

    $("#login").click(function () {
        var LoginName = $("#loginName").val();
        var pwd = $("#pwd").val();

        if(LoginName == 'undefined' || LoginName == '')
        {
            $("#errMsg").html("用户名为空");
            return;
        }
        if (pwd == 'undefined' || pwd == '') {
            $("#errMsg").html("密码为空");
            return;
        }

        var param = {};
        param.user_account = LoginName;
        param.user_pwd = pwd;

        Login(param);


        $("#valForm").validate();

    })
})

function Login(param) {
    $.ajax({
        url: '/Home/AdminLogin/',
        type: 'post',
        data:param,
        success:function(data){
            if (data.result)
                window.location.href = "/Home/Main";

            $("#errMsg").html(data.reason);
        }
    });
}

function search() {
    var phone = $("#phone").val();
    var param={};
    param.phone=phone;
    $("#userTable").bootstrapTable("showLoading");
    $.ajax({
        type: "get",
        url: "/Home/GetUserDetails",
        data: param,
        success: function (data) {
            if (data.rows == "undefined")
            {
                data.rows = [];
            }
            $("#userTable").bootstrapTable("load", data.rows);
            $("#userTable").bootstrapTable("hideLoading");
        }
    });
}

function setMoney() {
    var selection = $("#userTable").bootstrapTable("getSelections");

    if (selection.length < 1)
    {
        $("#warningModel").modal("show");
        return;
    }

    $("#nowMoney").val(selection[0].nowmoney);
    $("#nowSonMoney").val(selection[0].nowsonmoney);
    
    $("#myModel").modal("show");
}

function saveChange() {

    var selection = $("#userTable").bootstrapTable("getSelections");
    var phone = selection[0].phone;
    var beforeNowMoney = selection[0]["nowmoney"];
    var beforeNowSonmoney = selection[0]["nowsonmoney"];

    var param = {};
    param.phone = phone;
    param.nowMoney = $("#nowMoney").val();
    param.nowSonMoney = $("#nowSonMoney").val();

    if (beforeNowMoney < param.nowMoney) {
        alert("自己赚未到流量币减少数值不能大于" + beforeNowMoney);
        return;
    }
    if (beforeNowSonmoney < param.nowSonMoney)
    {
        alert("推荐赚的未到账流量币减少数值不能大于" + beforeNowSonmoney);
       
        return;
    }

    $.ajax({
        url: "/Home/SetMoney",
        data: param,
        type: "get",
        success: function (data) {
            $("#saveChange").popover({
                content: data.reason,
                delay: { "show": 3000, "hide": 3000 },
                placement: "top"
            });
            $("#saveChange").popover("show");
        }
    })
}