# Tack

<img src="./pin.png" width="300px" />

## Overview

Tack is a powerful DotNet command-line tool designed to filter projects and associated output assemblies from solutions. It streamlines the process of managing test assemblies in your CI/CD pipeline, enabling seamless test execution from local checkouts using standard CI build tools.

## Features

- Filter and manage projects within .NET solutions
- Extract and filter test assemblies based on configurable criteria
- Support for framework-specific filtering
- Configurable assembly exclusion
- Compatible with popular test runners like VsTestRunner
- Flexible output configuration (Release/Debug)

## Prerequisites

* .NET 7 SDK

## Installation

Tack can be installed as a DotNet command-line tool using:

```bash
dotnet tool install --global Tack
```

## Building from Source

To build the solution:

```bash
dotnet build Tack.sln
```

## Testing

To build and run the associated tests:

```bash
build.cmd
```

## Usage

### Get Test Assemblies

The main functionality of Tack is to extract test assemblies from your solution. Use the following command:

```bash
tack get-test-assemblies [--framework framework-regex] [--exclude-assemblies {list of assemblies}] [--configuration Release|Debug] --outfile {test-list}
```

#### Parameters:
- `--framework`: (Optional) Regex pattern to filter frameworks
- `--exclude-assemblies`: (Optional) List of assemblies to exclude
- `--configuration`: (Optional) Build configuration (Release or Debug)
- `--outfile`: (Required) Output file path for the test assembly list

The generated test assembly list can be used with test runners like VsTestRunner for local test execution.

## Contributing

We welcome contributions from the community! Please see our [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines on how to:
- Submit bug reports and feature requests
- Set up your development environment
- Submit pull requests
- Follow our coding standards

## Versioning

This project uses NerdBank.GitVersioning for version management.

## Security

For information about reporting security vulnerabilities, please refer to our [security policy](https://github.com/G-Research/Tack/blob/main/SECURITY.md).

## Support

If you encounter any issues or have questions:
1. Check the existing [GitHub Issues](https://github.com/G-Research/Tack/issues)
2. Create a new issue if your problem hasn't been reported
3. Provide as much detail as possible, including version numbers and reproduction steps

## License

This project is licensed under the Apache 2.0 license. See the [LICENSE](LICENSE) file for details.