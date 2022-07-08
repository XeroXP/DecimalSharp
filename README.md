DecimalSharp
============

![](https://img.shields.io/nuget/v/DecimalSharp)
![](https://img.shields.io/nuget/dt/DecimalSharp?color=laim)
![](https://img.shields.io/appveyor/build/XeroXP/decimalsharp/master)
![](https://img.shields.io/appveyor/tests/XeroXP/decimalsharp/master)

Decimal in C#. Port of [decimal.js](https://github.com/MikeMcl/decimal.js/). Public domain.

An arbitrary-precision Decimal type for C#.


Documentation
=============

* [Overview](#overview)
* [Installation](#installation)
* [Use](#use)
* [System requirements](#system-requirements)
* [Development and testing](#development-and-testing)
* [Contributors](#contributors)


Overview
--------

The primary goal of this project is to produce a translation of decimal.js to
C# which is as close as possible to the original implementation.

### Features

- Integers and floats
- Simple but full-featured API
- Also handles hexadecimal, binary and octal values
- No dependencies
- Comprehensive [documentation](../../wiki/) and test set

This library also adds the trigonometric functions, among others, and supports non-integer powers.

For a lighter version of this library without the trigonometric functions use `DecimalSharp.Light`


Installation
------------

You can install DecimalSharp via [NuGet](https://www.nuget.org/):

package manager:

    $ PM> Install-Package DecimalSharp

NET CLI:

	$ dotnet add package DecimalSharp

or [download source code](../../releases).


Use
-----

*In the code examples below, semicolons and `ToString` calls are not shown.*

A BigDecimal number is created from a primitive number, string, or other BigDecimal number.

```csharp
x = new BigDecimal(123.4567)
y = new BigDecimal("123456.7e-3")
z = new BigDecimal(x)
x.Eq(y) && x.Eq(z) && y.Eq(z)          // true
```

If using values with more than a few digits, it is recommended to pass strings rather than numbers to avoid a potential loss of precision.

```csharp
// Precision loss from using numeric literals with more than 15 significant digits.
new BigDecimal(1.0000000000000001)         // "1"
new BigDecimal(88259496234518.57)          // "88259496234518.56"
new BigDecimal(99999999999999999999)       // "100000000000000000000"

// Precision loss from using numeric literals outside the range of simple values.
new BigDecimal(2e+308)                     // "Infinity"
new BigDecimal(1e-324)                     // "0"

// Precision loss from the unexpected result of arithmetic with simple values.
new BigDecimal(0.7 + 0.1)                  // "0.7999999999999999"
```

Strings can contain underscores as separators to improve readability.

```csharp
x = new BigDecimal("2_147_483_647")
```

String values in binary, hexadecimal or octal notation are also accepted if the appropriate prefix is included.

```csharp
x = new BigDecimal("0xff.f")         // "255.9375"
y = new BigDecimal("0b10101100")     // "172"
z = x.Plus(y)                        // "427.9375"

z.ToBinary()                         // "0b110101011.1111"
z.ToBinary(13)                       // "0b1.101010111111p+8"

x = new BigDecimal("0b1.1111111111111111111111111111111111111111111111111111p+1023")
// "1.7976931348623157081e+308"
```


A BigDecimal number is immutable in the sense that it is not changed by its methods.

```csharp
0.3 - 0.1                              // 0.19999999999999998
x = new BigDecimal(0.3)
x.Minus(0.1)                           // "0.2"
x                                      // "0.3"
```

The methods that return a BigDecimal number can be chained.

```csharp
x.Div(y).Plus(z).Times(9).Minus("1.234567801234567e+8").Plus(976.54321).Div("2598.11772")
x.Sqrt().Div(y).Pow(3).Gt(y.Mod(z))    // true
```

Many method names have a shorter alias.

```csharp
x.SquareRoot().DividedBy(y).ToPower(3).Equals(x.Sqrt().Div(y).Pow(3))     // true
x.ComparedTo(y.Modulo(z).Negated() === x.Cmp(y.Mod(z).Neg())              // true
```

And there are `IsNaN` and `IsFinite` methods, as `NaN` and `Infinity` are valid `BigDecimal` values.

```csharp
x = new BigDecimal(double.NaN)                                 // "NaN"
y = new BigDecimal(double.PositiveInfinity)                    // "Infinity"
x.IsNaN() && !y.IsNaN() && !x.IsFinite() && !y.IsFinite()      // true
```

There are `ToExponential`, `ToFixed` and `ToPrecision` methods.

```csharp
x = new BigDecimal(255.5)
x.ToExponential(5)                     // "2.55500e+2"
x.ToFixed(5)                           // "255.50000"
x.ToPrecision(5)                       // "255.50"
```

There is also a `ToFraction` method with an optional *maximum denominator* argument.

```csharp
z = new BigDecimal(355)
pi = z.DividedBy(113)        // "3.1415929204"
pi.ToFraction()              // [ "7853982301", "2500000000" ]
pi.ToFraction(1000)          // [ "355', '113" ]
```

All calculations are rounded according to the number of significant digits and rounding mode specified by the `precision` and `rounding` properties of the BigDecimal constructor.

For advanced usage, multiple BigDecimal constructors can be created, each with their own independent configuration which applies to all BigDecimal numbers created from it.

```csharp
// Set the precision and rounding
var bigDecimalFactory = new BigDecimalFactory(new BigDecimalConfig()
{
	Precision = 5,
	Rounding = RoundingMode.ROUND_HALF_UP
});

// Create another Decimal constructor, optionally passing in a configuration object
var bigDecimalFactory2 = new BigDecimalFactory(new BigDecimalConfig()
{
	Precision = 9,
	Rounding = RoundingMode.ROUND_DOWN
});

x = bigDecimalFactory.BigDecimal(5)
y = bigDecimalFactory2.BigDecimal(5)

x.Div(3)                           // "1.6667"
y.Div(3)                           // "1.66666666"
```

The value of a BigDecimal is stored in a floating point format in terms of its digits, exponent and sign, but these properties should be considered read-only.

```csharp
x = new BigDecimal(-12345.67);
x.c                                    // [ 12345, 6700000 ]    digits (base 10000000)
x.e                                    // 4                     exponent
x.s                                    // -1                    sign
```

For further information see the [API](../../wiki/) reference documentation.


System requirements
-------------------

BNSharp supports:

* Net 6


Development and testing
------------------------

Make sure to rebuild projects every time you change code for testing.

### Testing

To run tests:

    $ dotnet test


Contributors
------------

[XeroXP](../../../).
