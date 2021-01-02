# BitcoinFallingPriceWarner
First .NETCore Project , Emailwarning if Bitcoin price is falling.


Install:

* Checkout the Repro
* Build the WorkerService
** In Folder-\BitcoinFallingPriceWarner\Release\Service the WindowsService is build
* config the Appsettings.json with your Emailadresse and your SMTP Server
* Config the Timer (good is 15 Minutes), how often the service get new prices
* Config the AmountDifference from when an email will be sent. The last execution value is always compared with the 10th last execution value.
