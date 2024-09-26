<html>
	<head>
	<script src="https://code.jquery.com/jquery-3.3.1.min.js" integrity="sha256-FgpCb/KJQlLNfOu91ta32o/NMZxltwRo8QtmkMRdAu8=" crossorigin="anonymous"></script>	
	<!--<script src="https://scripts.sandbox.bka.sh/versions/1.2.0-beta/checkout/bKash-checkout-sandbox.js"></script>-->

	<script src="https://scripts.pay.bka.sh/versions/1.2.0-beta/checkout/bKash-checkout.js"></script>
	
    <link href="https://merchantdemo.sandbox.bka.sh/frontend/resource/css/bootstrap.min.css" rel="stylesheet" />
    <link href="https://merchantdemo.sandbox.bka.sh/frontend/resource/css/myapp.css" rel="stylesheet" />
	
    <meta name="viewport" content="width=device-width" , initial-scale=1.0/>
    <style>
        body {
            padding-top: 20px;
        }

        .container {
            text-align: center;
        }

        .container pre {
            max-height: 30em;
            overflow: auto;

        }

        button {
            width: 10em;
        }
    </style>
	</head>

<body class="text-center">
	<button id="bKash_button" disabled="disabled" >Deposit With bKash</button>

<script type="text/javascript">
$(document).ready(function () {
            //Token
            $.ajax({
                        url: "token.php",
                        type: 'POST',
                        contentType: 'application/json',
                        success: function (data) {
                            console.log('got data from token  ..');
                        }
                    });

            var paymentConfig= {
            createCheckoutURL: "createpayment.php",
            executeCheckoutURL: "executepayment.php",
            };


            var paymentRequest;
			//var payamount=alert($('#tamount').val());//$('#tamount').val();
			//alert(payamount);
			paymentRequest = {amount: '<?php echo $_GET['amount'] ?>', invoice: '<?php echo $_GET['rcode'] ?>' };
			//console.log(paymentRequest.amount, paymentRequest.invoice);
			$('#bKash_button').removeAttr('disabled');
            bKash.init({
				
                paymentMode: 'checkout',

                paymentRequest: paymentRequest,

                createRequest: function (request) {
					
                    console.log('=> createRequest (request) :: ');
                    console.log(request);


				$.ajax({
                        url: paymentConfig.createCheckoutURL+"?amount="+paymentRequest.amount+"&invoice="+paymentRequest.invoice,
                        type: 'GET',
                        contentType: 'application/json',
                        success: function (data) {
                            console.log('got data from create  ..');
                            console.log('data ::=>');
                            //console.log(JSON.parse(data).paymentID);
                            data = JSON.parse(data);
                            if (data && data.paymentID != null) {
                                paymentID = data.paymentID;
                                bKash.create().onSuccess(data);  
								
                            } else {
								alert(data.errorMessage+" Tag should be 2 digit, Length should be 2 digit, Value should be number of character mention in Length, ex. MI041234 , supported tags are MI, MW, RF");
                                bKash.create().onError();
                            }
                        },
                        error: function () {
                            bKash.create().onError();

                        }
                    });

                },
                executeRequestOnAuthorization: function () {

                    console.log('=> executeRequestOnAuthorization');
                    $.ajax({
                        url: paymentConfig.executeCheckoutURL+"?paymentID="+paymentID,
                        type: 'GET',
                        contentType: 'application/json',
                        success: function (data) {

                            console.log('got data from execute  ..');
                            console.log('data ::=>');
                            console.log(JSON.stringify(data));
                            data = JSON.parse(data);
                            if (data && data.paymentID != null) {
								alert('[SUCCESS] data : ' + JSON.stringify(data));
								//console.log('[SUCCESS] data : ' + JSON.stringify(data));
								//alert("Payment Successfull...");
                                window.location.href = "success.html";//your success page

                            } else {
								alert('[ERROR] data : ' + JSON.stringify(data));
                                bKash.execute().onError();
                            }
                        },
                        error: function () {
                            bKash.execute().onError();
                        }
                    });
                }
            });
        });


</script>
</body>

</html>