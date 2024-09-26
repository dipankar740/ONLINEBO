<?php
session_start();

$amount=$_GET['amount'];
$invoice=$_GET['invoice'];//must be uniqueinvoice
$intent = "sale";

	   $createpaybody=array(
	       'amount'=>$amount,
		   'currency'=>'BDT',
		   'intent'=>$intent,
		   'merchantInvoiceNumber'=>$invoice
		   );	
		
		$url=curl_init('https://checkout.pay.bka.sh/v1.2.0-beta/checkout/payment/create');
		
		$createpaybodyx=json_encode($createpaybody);
		$header=array(
		        'Content-Type:application/json',
				'authorization:'.$_SESSION['token'],
				'x-app-key:11fu85qnntpr5e4kefa7cusc7g');				
				curl_setopt($url,CURLOPT_HTTPHEADER, $header);
				curl_setopt($url,CURLOPT_CUSTOMREQUEST, "POST");
				curl_setopt($url,CURLOPT_RETURNTRANSFER, true);
				curl_setopt($url,CURLOPT_POSTFIELDS, $createpaybodyx);
				curl_setopt($url,CURLOPT_FOLLOWLOCATION, 1);
				$resultdata=curl_exec($url);
				curl_close($url);
				echo $resultdata;
?>