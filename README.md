# Banned API Analyzer for the .NET Compiler Platform

[![NuGet](https://img.shields.io/nuget/v/DotNetAnalyzers.BannedApiAnalyzer.svg)](https://www.nuget.org/packages/DotNetAnalyzers.BannedApiAnalyzer) [![NuGet Beta](https://img.shields.io/nuget/vpre/DotNetAnalyzers.BannedApiAnalyzer.svg)](https://www.nuget.org/packages/DotNetAnalyzers.BannedApiAnalyzer)

[![Join the chat at https://gitter.im/DotNetAnalyzers/BannedApiAnalyzer](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/DotNetAnalyzers/BannedApiAnalyzer?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

[![Build status](https://ci.appveyor.com/api/projects/status/npxe1cc6fo8d6wn6/branch/master?svg=true)](https://ci.appveyor.com/project/sharwell/BannedApiAnalyzer/branch/master)

[![codecov.io](http://codecov.io/github/DotNetAnalyzers/BannedApiAnalyzer/coverage.svg?branch=master)](http://codecov.io/github/DotNetAnalyzers/BannedApiAnalyzer?branch=master)

This repository contains an implementation of .NET banned API rules using the .NET Compiler Platform. Where possible, code fixes are also provided to simplify the process of correcting violations.

## Using BannedApiAnalyzer

The preferable way to use the analyzers is to add the nuget package [DotNetAnalyzers.BannedApiAnalyzer](http://www.nuget.org/packages/DotNetAnalyzers.BannedApiAnalyzer/)
to the project where you want to enforce banned API rules.

The severity of individual rules may be configured using [rule set files](https://docs.microsoft.com/en-us/visualstudio/code-quality/using-rule-sets-to-group-code-analysis-rules)
in Visual Studio 2015 or newer. See [Configuration.md](docs/Configuration.md) for more information.

For documentation and reasoning on the rules themselves, see the [DOCUMENTATION.md](DOCUMENTATION.md).

## Installation

BannedApiAnalyzer requires Visual Studio 2017 version 15.5 or newer, or the equivalent command line compiler tools.

BannedApiAnalyzer can be installed using the NuGet command line or the NuGet Package Manager in Visual Studio 2017.

**Install using the command line:**

```ps
Install-Package DotNetAnalyzers.BannedApiAnalyzer
```

> ⚠ Prereleases of the **DotNetAnalyzers.BannedApiAnalyzer** package use Semantic Versioning 2, which requires NuGet 4.3.0 (Visual Studio 2017 version 15.3) or newer. Users with clients that do not support Semantic Versioning 2 may install prereleases using the **DotNetAnalyzers.BannedApiAnalyzer.Unstable** package instead.

## Team Considerations

If you use older versions of Visual Studio in addition to Visual Studio 2017 or Visual Studio 2019, you may still install these analyzers. They will be automatically disabled when you open the project back up in versions of Visual Studio prior to Visual Studio 2017 version 15.5.

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md)

## Current status

An up-to-date list of which banned API rules are implemented and which have code fixes can be found [here](https://dotnetanalyzers.github.io/BannedApiAnalyzer/).
