# CsTasks

Commerce Server functions for use with the FAKE build system.  Automate tasks like deleting and importing promotions.

Written by [@bentayloruk](http://twitter.com/bentayloruk) of [@enticify](http://twitter.com/enticify) and used in the production of [best discount engine for Commerce Server.](http://www.enticify.com/)

## Features

* Import discounts, promotion codes and global expressions.
* Delete all discounts.
* Purge all discount data.

## Installation

**Currently CTP release.  Proceed at your own risk.**

`install-package CsTasks -pre` 

## Documentation

* Create an `.fsx` script that references the FAKE and CsTasks assemblies (located in the nuget installed package folders).  [Example](https://github.com/enticify/CsTasks/blob/master/src/Enticify.CsTasks/ResetDiscounts.fsx#L1)
* Use some of the CsTask functions in your FAKE targets.  [Example]([Example](https://github.com/enticify/CsTasks/blob/master/src/Enticify.CsTasks/ResetDiscounts.fsx)

## Known Issues

None.

## Release Notes

* [Version 0.1.3 is up on Nuget](http://nuget.org/packages/CsTasks).  More ctp releases will follow.

## License

[MIT](https://github.com/enticify/CsSpy/blob/master/LICENSE.md)
