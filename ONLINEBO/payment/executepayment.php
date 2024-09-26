<?php
session_start();
$paymentID=$_GET['paymentID'];
    
$url=curl_init('https://checkout.pay.bka.sh/v1.2.0-beta/checkout/payment/execute/'.$paymentID);
$header=array(
		'Content-Type:application/json',
		'authorization:'.$_SESSION['token'],
		'x-app-key:11fu85qnntpr5e4kefa7cusc7g');	
		curl_setopt($url,CURLOPT_HTTPHEADER, $header);
		curl_setopt($url,CURLOPT_CUSTOMREQUEST, "POST");
		curl_setopt($url,CURLOPT_RETURNTRANSFER, true);
		curl_setopt($url,CURLOPT_FOLLOWLOCATION, 1);
		$resultdatax=curl_exec($url);
		curl_close($url);
	echo $resultdatax;   
    

?>