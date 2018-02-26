makecert -sv JWTTest.pvk -n "cn=JWT Test" JWTTest.cer -b 01/01/2018 -e 01/01/2019 -r -pe -a sha512

pvk2pfx -pvk JWTTest.pvk -spc JWTTest.cer -pfx JWTTest.pfx