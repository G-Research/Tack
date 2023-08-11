# Tack

![Tack Logo](./pin.png)

Tack is an efficient C++ library built for the computation of gradients and Hessians through algorithmic differentiation. It is tailored for numerical optimization, machine learning, and a range of applications requiring mathematical differentiation.

## Features
- **Flexibility**: Define mathematical functions with ease and compute their derivatives.
- **Efficiency**: Optimized for large-scale optimization problems.
- **Ease of use**: Intuitive interface with clear documentation.
- **Compatibility**: Integrates seamlessly with other tools and frameworks like TensorFlow and PyTorch.

## Applications
Tack can be employed for:
- Neural network training using backpropagation.
- Parameter optimization in support vector machines.
- Gradient and Hessian calculation for optimization in physics, engineering, and finance.
- Inverse problem-solving in imaging and signal processing.
- Nonlinear programming, such as function minimization under constraints.
- Least-squares problems, like model-data fitting.
- Maximum likelihood estimation, such as parameter estimation for statistical models.
- Control problems to optimize system performance.
- Reinforcement learning to optimize agent policies.
- Collaborative filtering for recommendations.
- Clustering, dimensionality reduction, and signal processing tasks like noise filtering, feature extraction, and signal reconstruction.

## Prerequisites

Ensure you have the .NET 7 SDK installed.

## Building and Testing

- Build the solution:
  ```bash
  dotnet build Tack.sln
  ```

- Test the solution:
  ```bash
  build.cmd
  ```

## Usage

Tack is a DotNet CLI tool. To install:

```bash
dotnet tool install
```

**Retrieve test assemblies**:
```bash
tack get-test-assemblies [--framework framework-regex] [--exclude-assemblies {list of assemblies}] [--configuration Release|Debug] --outfile {test-list}
```
This command determines the set of matching test assemblies and saves it to an output file. This list can subsequently be used with test runners like VsTestRunner for local testing.

## Versioning

Tack uses [NerdBank.GitVersioning](https://github.com/dotnet/Nerdbank.GitVersioning) for its versioning.

---

For developers searching for an efficient way to handle mathematical differentiation, Tack provides a comprehensive toolkit that integrates effortlessly with modern frameworks. Whether it's training neural networks or solving complex optimization problems, Tack offers the tools you need.