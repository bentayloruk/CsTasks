# CsTasks

Simple and time-saving Commerce Server task automation library.  Do things like deleting, purging and importing promotions.  CsTasks provides a consistent .NET API around existing tools and APIs.  

## Authors

Written by [@bentayloruk](http://twitter.com/bentayloruk) of [@enticify](http://twitter.com/enticify) and used in the production of [best discount engine for Commerce Server.](http://www.enticify.com/)

Get your name here.  Fork and contribute!  *Or show some love and Star us.*

## Features

* Import discounts, promotion codes and global expressions.
* Delete discounts, direct mail and advertisment campaign items.
* Purge marketing data, baskets, purchase orders and catalog data.

## Pre-Requisites

* Microsoft Commerce Server 2007 or Microsoft/Ascentium Commerce Server 2009.

## Installation

To install CsTasks run the following command in the [Nuget Package Manager Console](http://docs.nuget.org/docs/start-here/using-the-package-manager-console)

`PM> install-package CsTasks -pre`
 
*The `-pre` option is required as CsTasks is only available as a [pre-release package](http://nuget.codeplex.com/wikipage?title=Pre-Release%20Packages) at the moment.*

## Documentation

CsTasks is written in F# and we use it from `.fsx` build scripts.  However, it is a normal .NET assembly so you can use it from any .NET language.  Look at an [example of FAKE with CsTasks usage](https://github.com/enticify/CsTasks/blob/master/src/Enticify.CsTasks/ResetDiscounts.fsx)

*More documentation to come.  We are in pre-release mode so you'll have to find your own way.*

## Additional Information

CsTasks uses the following tools and APIs.  

* [Export/Import Discounts Tool](http://archive.msdn.microsoft.com/ExportImportDiscount).  Included in the CsTasks package tools folder.  No installation required.
* [PurgeCommerceData](http://msdn.microsoft.com/en-us/library/cc515165.aspx#PurgeCommerceData).  Part of the Commerce Server installation.

## Known Issues

None.  [Raise one](https://github.com/enticify/CsTasks/issues).

## Release Notes

### 0.1.4

* Removed dependency on FAKE.
* Breaking:  Replaced PurgeCommerceDataTool functions with PurgeCommerceDataTool a class.

## License

[MIT](https://github.com/enticify/CsSpy/blob/master/LICENSE.md)
