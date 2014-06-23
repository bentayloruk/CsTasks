# CsTasks

Simple and time-saving Commerce Server task automation library.  Do things like deleting, purging and importing promotions.  CsTasks provides a consistent .NET API around existing tools and APIs.  

## Aims

* Collect useful Commerce Server tools in a single Nuget package (licenses permitting).
    * Consistent, single location makes it easier to share scripts.
    * No need to add tools to source control (as can use Nuget package restore).
    * Get new features and fixes without effort!
* Provide common administration tasks via a simple, easy to use .NET API.
    * Probably safer than hacking command line scripts.
    * Enable script sharing and re-use.
* Support scripting from F# and Powershell.
    * Integrate tasks into build and environment scripts.

## Authors

* CsTasks written by [@bentayloruk](http://twitter.com/bentayloruk) for use in building [Enticify](http://www.enticify.com/) (discount engine for Commerce Server).
* Included [ExportImportPromotion.exe tool](http://archive.msdn.microsoft.com/ExportImportDiscount) written by [ccbeloy](http://archive.msdn.microsoft.com/UserAccount/UserProfile.aspx?UserName=ccbeloy)

Get your name here too.  Fork and contribute!

## Features

* Import discounts, promotion codes and global expressions.
* Delete discounts, direct mail and advertisment campaign items.
* Purge marketing data, baskets, purchase orders and catalog data.

## Example - Delete, Purge and Import Discounts

This example does the following:

- Exports existing discounts to the Temp folder.
- Deletes all the discounts.
- Purges the deleted discounts.
- Deletes all the expressions.
- Reseeds expression and campaign item ids.
- Imports discounts, expressions and promo codes from XML.

The code is written as an F# script:

```fsharp	
open CsTasks
open System.IO

//Setup our site environment.
let siteName = "StarterSite"
let marketingWebServiceUrl = @"""http://localhost/MarketingWebService/MarketingWebService.asmx""" 
let tempPath = Path.GetTempPath()
let purgeTool = PurgeCommerceDataTool(siteName)
let marketingDataStore = MarketingDataStore(siteName)

//Do a safety export to Temp (using an CsTasks F# function).
ExportDiscounts (fun defaultArgs ->
    { defaultArgs with
        DiscountExportArgs.MarketingWebServiceUrl = marketingWebServiceUrl
        ExportDirectoryPath = tempPath })

//Delete all campaign items.
marketingDataStore.DeleteAllCampaignItems()

//Purge marketing data (so we can delete expressions)
let retCode = purgeTool.PurgeAllMarketingData()

//Delete expressions.
marketingDataStore.DeleteAllExpressions()

//Reseed all Ids so our imported discounts get the same IDs each time.
marketingDataStore.ReseedCampaignItemIds(0)
marketingDataStore.ReseedExpressionIds(0)

//Do the discount import.
ImportDiscounts (fun defaultArgs -> { defaultArgs with DiscountImportArgs.MarketingWebServiceUrl = marketingWebServiceUrl})
```

## Pre-Requisites

* Commerce Server.  Version 2007 or greater.

## Installation

To install [CsTasks from Nuget.org](https://nuget.org/packages/CsTasks/) run the following command in the Nuget Package Manager Console.

`PM> install-package CsTasks`
 
## Additional Information

CsTasks uses the following tools and APIs.  

* [Export/Import Discounts Tool](http://archive.msdn.microsoft.com/ExportImportDiscount).  Included in the CsTasks package tools folder.  No installation required.
* [PurgeCommerceData](http://msdn.microsoft.com/en-us/library/cc515165.aspx#PurgeCommerceData).  Part of the Commerce Server installation.

CsTasks is written in F# and we use it from `.fsx` build scripts.  However, it is a normal .NET assembly so you can use it from any .NET language.

## Known Issues

None.  [Raise one](https://github.com/enticify/CsTasks/issues).

## Release Notes

### 0.4.2

* More bug fixes for CS10 support :).  Code and schema namespace changes are a pain in the butt!

### 0.4.1

* Bug fixes for CS10 support.

### 0.4.0

* Support for CS10+.
* Generate two lib assemblies.  `CsTasks.dll` for CS10+ and `CsTasksMS.dll` for CS2007 and CS2009.

### 0.3.0

* Add support for i_SearchResultsLimit so that ALL campaign items are deleted, even when paged.
* Addition of very basic test discount generator (used for volume testing).

### 0.1.7

* New: DeleteAllPromoCodeDefinitions added to MarketingDataStore.

### 0.1.6

* Removed CTP extension.  No longer a PRE package.
* Added ReseedCampaignIds to MarketingDataSource.

### 0.1.5

* Breaking change:  Rolled marketing functions into MarketingDataStore class.  Should be easier to use for most.
* New:  Added reseeding for expressions and campaign items.

### 0.1.4

* Removed dependency on FAKE.
* Breaking:  Replaced PurgeCommerceDataTool functions with PurgeCommerceDataTool class.

## License

[MIT](https://github.com/enticify/CsSpy/blob/master/LICENSE.md)
