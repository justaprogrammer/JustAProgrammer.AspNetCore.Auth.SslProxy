# ASP.NET Core OpenId through a SSL Proxy Sample

This sample shows a few things things:

* How to use docker to simulate a SSL Proxy in front of your application
* How to use ForwardedHeaderOptions so that The OpenIdConnect library (and probably all of the ASP.NET Core Auth packages) will properly redirect through a reverse-proxy.
* The wrong way to rewrite redirect URLS with 

## TODO

* Get SSL On a different port working the right way
* Figure out why running docker from console.exe only gives us info level logs while the Debug console gets us trace and debug.
* Wire up ProxyOptions
* Unit tests