# MVC-Call-AML-WS-in-parallel
This articel is to show you how we can call multiple Azure Machine Learning web services in parallel from a C# MVC application. 

For calling the a number of AML web services in parallel, we create a method in Controller class in this prototype:
 async Task<T> PerformAMLWSCall(string url, string key, <your data>) in this method an HttpClient is created and used locally.
Then in action method, public async Task<ActionResult> Search(<your data>), we call the method PerformAMLWSCall with different url, key and your data. 

Also in web.config file we commented out one line:
<!--<add name="ExtensionlessUrlHandler-Integrated-4.0" path="*." verb="GET,HEAD,POST,DEBUG,PUT,DELETE,PATCH,OPTIONS" type="System.Web.Handlers.TransferRequestHandler" preCondition="integratedMode,runtimeVersionv4.0" />-->
Otherwise the app works in VS but not in Azure App Service.
 
