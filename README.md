# Tack

<img src="./pin.png" width="300px" />

A DotNet tool which can used to get filter projects and associated output assemblies from solutions. 

Used in conjunction to with other tools this can enable running of tests from local checkout using the standard CI build tools.

### Prerequisites

* DotnetCore 3.1 Sdk

### Build the solution.

```dotnet build Tack.sln```

### Testing

To build and run the associated tests run:

```build.cmd```

## Usage

Tack builds as a DotNet command line tool and can be installed using `dotnet tool install`

### Get test assemblies

```tack get-test-assemblies [--framework framework-regex] [--exclude-assemblies {list of assemblies}] [--configuration Release|Debug] --outfile {test-list}```

Determines the matching set of test assemblies and writes to output file. This list can be passed into one of the test runners e.g. VsTestRunner to enable running of tests from local checkout.

## Versioning

Versioned using NerdBank.GitVersioning

