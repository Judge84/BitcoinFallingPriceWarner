# BitcoinFallingPriceWarner
First .NETCore Project , Emailwarning if Bitcoin price is falling.


Install:

* Checkout the Repro
* Build the WorkerService
* In Folder-\BitcoinFallingPriceWarner\Release\Service the WindowsService is build
* config the Appsettings.json with your Emailadresse and your SMTP Server
* Config the Timer (good is 15 Minutes), how often the service get new prices
* Config the AmountDifference from when an email will be sent. The last execution value is always compared with the 10th last execution value.

* For Installing the Service in Windows:
* cmd --> as Administrator
* sc create BitcoinFallingPriceWarnerWorkerService binPath="C:\Projects\BitcoinFallingPriceWarnerWorkerService\BitcoinFallingPriceWarnerWorkerService.exe"
* Start the Service: "sc start BitcoinFallingPriceWarnerWorkerService"
* Stop the Service: "sc start BitcoinFallingPriceWarnerWorkerService"
* Stop the Service: "sc delete BitcoinFallingPriceWarnerWorkerService"
* if the service not startet, check if .NET Core 3.1 is installed.
https://dotnet.microsoft.com/download/dotnet-core/3.1
