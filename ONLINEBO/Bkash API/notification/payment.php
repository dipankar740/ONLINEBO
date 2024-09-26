<html>
<head>
<script src="https://code.jquery.com/jquery-3.3.1.min.js" integrity="sha256-FgpCb/KJQlLNfOu91ta32o/NMZxltwRo8QtmkMRdAu8="
crossorigin="anonymous"></script>
<script src="BKASH_SCRIPT_URL (View Page Source)"></script>
</head>

<body>
<button id="bKash_button">Pay With bKash</button>


<script type="text/javascript">
    $(document).ready(function () {


        var paymentID;
        var paymentConfig;

        scriptUrlFetch()
            .done(function(url){
                console.log('Finished Loading Script URL :: '+url);

                url=url.replace("XXX",$('#version').text());
                console.log('Finished replacing XXX with version number :: Script URL :: '+url);
                scriptFetch(url)
                    .done(function(script){
                        console.log('Finished Loading Script :: '+url);
                        merchantBackendURLFetch()
                            .done(function(merchantBackendObj){

                                console.log('Finished Loading Merchant Backend URL :: merchantBackendObj :: ');

                                console.log(merchantBackendObj);

                                getApiGWVersion($('#version').text())
                                    .done(function(apiGWVersion){


                                        $("#apiVersion").text(apiGWVersion);

                                        console.log('Target api version for '+$('#version').text()+' is '+apiGWVersion);
                                        for (var key in merchantBackendObj) {
//                                             console.log(key, merchantBackendObj[key]);
                                            merchantBackendObj[key]=merchantBackendObj[key].replace("XXX", 'v'+apiGWVersion);
                                        }

                                        console.log('Replacing XXX with proper version number :: merchantBackendObj :: ');
                                        console.log(merchantBackendObj);


                                        paymentConfig=merchantBackendObj;

                                        $('#isAggregrator').prop('checked', false);
                                        document.getElementById("merchantAssociationInfo").value = "";
                                        //simulating sample amount value
                                        $('#amount').text((Math.random() * 100).toFixed(2));


                                        var paymentRequest;
                                        if ($('input[name=paymentType]:checked').val() == 'immediate') {
                                            paymentRequest = { amount: $('#amount').html(), intent: 'sale' };

                                        }
                                        else if ($('input[name=paymentType]:checked').val() == 'authNcapture') {
                                            paymentRequest = { amount: $('#amount').html(), intent: 'authorization' };

                                        };



                                        bKash.init({


                                            //options - 1) 'checkout'
                                            //1) 'checkout' : Performs a single checkout.
                                            paymentMode: 'checkout',
                                            paymentRequest: paymentRequest,


                                            createRequest: function (request) {

                                                console.log('=> createRequest (request) :: ');
                                                console.log(request);


                                                $.ajax({
                                                    url: paymentConfig.createCheckoutURL,
                                                    type: 'POST',
                                                    contentType: 'application/json',
                                                    data: JSON.stringify(request),

                                                    success: function (data) {

                                                        // alert('inside success : create mandate() :; bKash-direct-old.js');
                                                        console.log('got data from create  ..');
                                                        console.log('data ::=>');
                                                        console.log(JSON.stringify(data));

                                                        if (data && data.paymentID != null) {
                                                            paymentID = data.paymentID;
                                                            bKash.create().onSuccess(data);

                                                        } else {
                                                            bKash.create().onError();//run clean up code
                                                            alert(data.errorMessage+" Tag should be 2 digit, Length should be 2 digit, Value should be number of character mention in Length, ex. MI041234 , supported tags are MI, MW, RF");

                                                        }

                                                    },
                                                    error: function () {
                                                        bKash.create().onError();//run clean up code
                                                        //alert(data.errorMessage);

                                                    }
                                                });

                                            },
                                            executeRequestOnAuthorization: function () {

                                                console.log('=> executeRequestOnAuthorization');
                                                $.ajax({
                                                    url: paymentConfig.executeCheckoutURL,
                                                    type: 'POST',
                                                    contentType: 'application/json',
                                                    data: JSON.stringify({ "paymentID": paymentID }),

                                                    success: function (data) {

                                                        console.log('got data from execute  ..');
                                                        console.log('data ::=>');
                                                        console.log(JSON.stringify(data));

                                                        if (data && data.paymentID != null) {
                                                            // executeRequestOnAuthorization executed successfully
                                                            // redirect to your(merchant) success page
                                                            alert('[SUCCESS] data : ' + JSON.stringify(data));
                                                            window.location.href = "../../success/checkout";

                                                        } else {
                                                            alert('[ERROR] data : ' + JSON.stringify(data));

                                                            bKash.execute().onError();//run clean up code

                                                        }

                                                    },
                                                    error: function () {
                                                        alert('An alert has occured during execute');

                                                        bKash.execute().onError();//run clean up code

                                                    }
                                                });
                                            },

                                            onClose : function () {
                                                alert('User has clicked the close button');
                                            }
                                        });


                                        $('#bKash_button').removeAttr('disabled');


                                        $('input[type=radio][name=paymentType]').change(function () {
                                            var merchantAssociationInfo;
                                            if (this.value == 'immediate') {
                                                if ($("#isAggregrator").is(":checked")) {
                                                    merchantAssociationInfo = document.getElementById("merchantAssociationInfo").value
                                                    if(merchantAssociationInfo!=null || !merchantAssociationInfo.isEmptyObject()) {
                                                        bKash.reconfigure({
                                                            paymentRequest: {
                                                                amount: $('#amount').html(),
                                                                intent: 'sale',
                                                                merchantAssociationInfo: document.getElementById("merchantAssociationInfo").value
                                                            }
                                                        });
                                                    } else {
                                                        bKash.reconfigure({
                                                            paymentRequest: { amount: $('#amount').html(), intent: 'sale'}
                                                        });
                                                    }
                                                } else {
                                                    bKash.reconfigure({
                                                        paymentRequest: { amount: $('#amount').html(), intent: 'sale'}
                                                    });

                                                }

                                            } else if (this.value == 'authNcapture') {
                                                merchantAssociationInfo = document.getElementById("merchantAssociationInfo").value
                                                if(merchantAssociationInfo!=null || !merchantAssociationInfo.isEmptyObject()) {
                                                    bKash.reconfigure({
                                                        paymentRequest: {
                                                            amount: $('#amount').html(),
                                                            intent: 'authorization',
                                                            merchantAssociationInfo: document.getElementById("merchantAssociationInfo").value
                                                        }
                                                    });
                                                } else {
                                                    bKash.reconfigure({
                                                        paymentRequest: { amount: $('#amount').html(), intent: 'authorization'}
                                                    });
                                                }
                                            }
                                        });

                                    })

                                    .fail(function(error){
                                        alert("Error while fetching dropdown - apiGW Version mapping");

                                    })

                            })
                            .fail(function(error){
                                alert("Error while fetching merchant backend URL");

                            })
                    })
                    .fail(function(error){
                        alert("Error while fetching script");

                    })
            })
            .fail(function(error){
                alert("Error while fetching script URL");
            })
    });



    $(function () {
        $("#isAggregrator").click(function () {
            if ($(this).is(":checked")) {
                $("#dvAggregratorInfo").show();
            } else {
                document.getElementById("merchantAssociationInfo").value = "";
                $("#dvAggregratorInfo").hide();
            }
        });
    });

    function getAggretaroInfo(){
        var merchantAssociationInfo;
        if ($('input[type=radio][name=paymentType]').val() == 'immediate') {
            if ($("#isAggregrator").is(":checked")) {
                merchantAssociationInfo = document.getElementById("merchantAssociationInfo").value
                if(merchantAssociationInfo!=null || !merchantAssociationInfo.isEmptyObject()) {
                    bKash.reconfigure({
                        paymentRequest: {
                            amount: $('#amount').html(),
                            intent: 'sale',
                            merchantAssociationInfo: document.getElementById("merchantAssociationInfo").value
                        }
                    });
                } else {
                    bKash.reconfigure({
                        paymentRequest: { amount: $('#amount').html(), intent: 'sale'}
                    });
                }

            } else if (this.value == 'authNcapture') {
                merchantAssociationInfo = document.getElementById("merchantAssociationInfo").value
                if(merchantAssociationInfo!=null || !merchantAssociationInfo.isEmptyObject()) {
                    bKash.reconfigure({
                        paymentRequest: {
                            amount: $('#amount').html(),
                            intent: 'authorization',
                            merchantAssociationInfo: document.getElementById("merchantAssociationInfo").value
                        }
                    });
                } else {
                    bKash.reconfigure({
                        paymentRequest: { amount: $('#amount').html(), intent: 'authorization'}
                    });
                }
            }

        }
    }



</script>

</body>
</html>