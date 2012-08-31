# CsTasks

Commerce Server functions for use with the FAKE build system.  Automate tasks like deleting and importing promotions.

Written by [@bentayloruk](http://twitter.com/bentayloruk) of [@enticify](http://twitter.com/enticify) and used in the production of [best discount engine for Commerce Server.](http://www.enticify.com/)

## Features

* Import discounts, promotion codes and global expressions.
* Delete all discounts.
* Purge all discount data.

## Installation from Nuget Package Manager Console

CsTasks is installed using Nuget.  Everything you need to use CsTasks will be installed (including FAKE).

Use the following Nuget command:  

`install-package CsTasks -pre` 

*The -pre is required as this is a CTP release.  Proceed at your own risk.*

## Documentation

* Create an `.fsx` script that references the FAKE and CsTasks assemblies (located in the nuget installed package folders).  
* Use some of the CsTask functions in your FAKE targets.
* Look at an [example of FAKE with CsTasks usage](https://github.com/enticify/CsTasks/blob/master/src/Enticify.CsTasks/ResetDiscounts.fsx)

## Known Issues

None.

## Release Notes

* [Initial release to Nuget.](http://nuget.org/packages/CsTasks).  More ctp releases will follow.

## License

[MIT](https://github.com/enticify/CsSpy/blob/master/LICENSE.md)
