
//$(document).ready(function () {

//    $("#bankNameList").empty();
//    //$("#bankroutinglist").empty();
//    //$("#bankroutinglist").append($("<option />").val("Routing Number").text("Routing Number"))
//    //$("#bankNameList").click(function () {

        
//        //$("#bankroutinglist").empty();
//        //$("#bankroutinglist").append($("<option />").val("Routing Number").text("Routing Number"))
//        //var selecteDistrict = { bankName: $("#bankNameList").val() };
//        //alert(selecteDistrict.bankName);
//        $.ajax({

//            url: "/Branch/GetBankName",
//            type: 'Post',
//            contentType: 'application/json',
//            dataType: 'json',
//            data: JSON.stringify(),
//            success: function (data) {
//                //$("#bankDistrictList").empty();

//                $("#bankNameList").append($("<option />").val("Select Your Bank").text("Select Your Bank"));
//                $.each(data, function (value) {
//                    //alert(data[value].BANKDISTRICT);
//                    $("#bankNameList").append($("<option />").val(data[value].BANKNAME).text(data[value].BANKNAME));
//                });
//                //alert("End");

//            }
//        });
//    //});
//});


$(document).ready(function () {
    
        $("#bankroutinglist").empty();
        $("#bankroutinglist").append($("<option />").val("Routing Number").text("Routing Number"))
        $("#bankNameList").change(function () {
            $("#bankroutinglist").empty();
            $("#bankroutinglist").append($("<option />").val("Routing Number").text("Routing Number"))
            var selecteDistrict = { bankName: $("#bankNameList").val() };
            //alert(selecteDistrict.bankName);
            $.ajax({
               
                url: "/Client/GetBankDistrict",
                type: 'Post',
                contentType: 'application/json',
                dataType: 'json',
                data: JSON.stringify(selecteDistrict),
                success: function (data) {
                    $("#bankDistrictList").empty();

                    $("#bankDistrictList").append($("<option />").val("Select Your Bank District").text("Select Your Bank District"));
                    $.each(data, function (value) {
                        //alert(data[value].BANKDISTRICT);
                        $("#bankDistrictList").append($("<option />").val(data[value].BANKDISTRICT).text(data[value].BANKDISTRICT));
                    });
                    //alert("End");

                }
            });
        });
    });

//get BAnk Branch
$(document).ready(function () {
    $("#bankDistrictList").change(function () {
        $("#bankroutinglist").empty();
        $("#bankroutinglist").append($("<option />").val("Routing Number").text("Routing Number"))
        //var v_data = {};
        //v_data.name = $.trim($("[id*=txtUserID]").val());
        //v_data.age = $.trim($("[id*=txtEmailAddress]").val());
        var selecteBank = { bankName: $("#bankNameList").val(), districtName: $("#bankDistrictList").val() };
        //var selecteDistrict = {districtName: $("#bankDistrictList").val() };
        //alert(selecteDistrict.bankName);
        $.ajax({
            url: "/Client/GetBankBranch",
            type: 'Post',
            contentType: 'application/json',
            dataType: 'json',
            data: JSON.stringify(selecteBank),
            success: function (data) {
                $("#bankbranchlist").empty();
                $("#bankbranchlist").append($("<option />").val("Select Your Bank Branch").text("Select Your Bank Branch"));
                //alert("Start");
                $.each(data, function (value) {
                    //alert(data[bb].BANKDISTRICT);
                    $("#bankbranchlist").append($("<option />").val(data[value].BANKBRANCH).text(data[value].BANKBRANCH));
                    //document.getElementById("#bankDistrictList").value = data[bb].BANKDISTRICT;
                });
                //alert("End");

            }
        });
    });
});


//get BAnk Routing
$(document).ready(function () {
    $("#bankbranchlist").change(function () {
        var selecteBank = { bankName: $("#bankNameList").val(), districtName: $("#bankDistrictList").val(), bankBranch: $("#bankbranchlist").val() };

        //alert(selecteDistrict.bankName);
        $.ajax({
            url: "/Client/GetBankRouting",
            type: 'Post',
            contentType: 'application/json',
            dataType: 'json',
            data: JSON.stringify(selecteBank),
            success: function (data) {
                //alert("Start");
                $.each(data, function (value) {
                    $("#bankroutinglist").empty();
                    //alert(data[bb].BANKDISTRICT);
                    $("#bankroutinglist").append($("<option />").val(data[value].BANKROUTING).text(data[value].BANKROUTING));

                    
                    //document.getElementById("#bankDistrictList").value = data[bb].BANKDISTRICT;
                });
                //alert("End");

            }
        });
    });
});
