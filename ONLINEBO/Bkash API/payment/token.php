<?php

    session_start();
    
	$request_token=bkash_Get_Token();
	$idtoken=$request_token['id_token'];
	
	$_SESSION['token']=$idtoken;
	
	echo $idtoken;
	
	function bkash_Get_Token(){
	$post_token=array(
	       'app_key'=>'11fu85qnntpr5e4kefa7cusc7g',
		   'app_secret'=>'jh5pj7c7he2t5jcnaa9ng2muckks26k5u7aibs4gfvusfkfmcap'
	);	
	
		
		$url=curl_init('https://checkout.pay.bka.sh/v1.2.0-beta/checkout/token/grant');
		
		$posttoken=json_encode($post_token);
		$header=array(
		        'Content-Type:application/json',
				'password:R@09LrPw1Q7',
				'username:ROYALCAPITAL');	
				
				curl_setopt($url,CURLOPT_HTTPHEADER, $header);
				curl_setopt($crl, CURLOPT_SSL_VERIFYHOST, 0);
				curl_setopt($crl, CURLOPT_SSL_VERIFYPEER, 0);
				curl_setopt($url,CURLOPT_CUSTOMREQUEST, "POST");
				curl_setopt($url,CURLOPT_RETURNTRANSFER, true);
				curl_setopt($url,CURLOPT_POSTFIELDS, $posttoken);
				curl_setopt($url,CURLOPT_FOLLOWLOCATION, 1);
				$resultdata=curl_exec($url);
				
				
				if ($resultdata === false)
				{
					// throw new Exception('Curl error: ' . curl_error($crl));
					print_r('Curl error: ' . curl_error($url));
				}

				
				curl_close($url);
				return json_decode($resultdata, true);
				
	}

?>