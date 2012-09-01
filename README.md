# CsTasks

Simple and time-saving Commerce Server task automation.  Do things like deleting, purging and importing promotions.  CsTasks is an easy to use set of functions integrated with the power of the FAKE build system.  

## Authors

Written by [@bentayloruk](http://twitter.com/bentayloruk) of [@enticify](http://twitter.com/enticify) and used in the production of [best discount engine for Commerce Server.](http://www.enticify.com/)

Get your name here.  [Fork and contribute](https://github.com/enticify/CsTasks/fork)!  *Or [just watch](https://github.com/enticify/CsTasks/star) :)*

## Features

* Import discounts, promotion codes and global expressions.
* Delete discounts, direct mail and advertisment campaign items.
* Purge deleted discounts.

## Pre-Requisites

* Microsoft Commerce Server 2007 or Microsoft/Ascentium Commerce Server 2009.

## Installation

To install CsTasks run the following command in the [Nuget Package Manager Console](http://docs.nuget.org/docs/start-here/using-the-package-manager-console)

`PM> install-package CsTasks -pre`
 
*The `-pre` option is required as CsTasks is only available as a [pre-release package](http://nuget.codeplex.com/wikipage?title=Pre-Release%20Packages) at the moment.*

## Documentation

* Create an `.fsx` script that references the FAKE and CsTasks assemblies (located in the nuget installed package folders).  
* Use some of the CsTask functions in your FAKE targets.
* Look at an [example of FAKE with CsTasks usage](https://github.com/enticify/CsTasks/blob/master/src/Enticify.CsTasks/ResetDiscounts.fsx)

## Additional Information

CsTasks uses the following tools and APIs.  

* [Export/Import Discounts Tool](http://archive.msdn.microsoft.com/ExportImportDiscount).  Included in the CsTasks package tools folder.  No installation required.
* [PurgeCommerceData](http://msdn.microsoft.com/en-us/library/cc515165(v=cs.70).aspx#PurgeCommerceData).  Part of the Commerce Server installation.

## Known Issues

None.  [Raise one](https://github.com/enticify/CsTasks/issues).

## Release Notes

* [Initial release to Nuget](http://nuget.org/packages/CsTasks).  More ctp releases will follow.

## License

[MIT](https://github.com/enticify/CsSpy/blob/master/LICENSE.md)
