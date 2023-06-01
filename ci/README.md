# API Specific CI/CD Approach
This API complies with the `common-ci` approach with the following exceptions:

## Linux PreBuild

Because x86 architecture is not supported with dotnet on ubuntu, we are building the native binaries in the same job as the x64 binaries. 

## Integration Tests

The integration testing approach differs from the 'general' inversion of control approach outlined in the [Design.md](https://github.com/51Degrees/common-ci/blob/gh-refact/design.md) as the it cannot be generic. 


The integration tests are conducted to verify the proper functioning of the device-detection-dotnet-examples with the newly built packages. In order to do this, the integration tests use a locally built and installed version of the device-detection-dotnet dependency created in the preceding stages of the pipeline. 

The necessary secrets such as Resource Keys and location of the test file are set up in the `setup-environment.ps1` script as environmental variables.  

The below chart shows the process of running tests for the device-detection-dotnet-examples. 

```mermaid
graph TD
    B[Clone Examples Repo]
    B --> C[Set up test files]
    C --> F[Enter device-detection-examples directory]
    F --> G[Install the package to local feed]
    G --> H[Set package dependency version]
    H --> I[Build and Test Examples]
    I --> J[Leave RepoPath]
```

It performs the following steps:

1. Clone Examples Repo: Clone the "device-detection-dotnet-examples" repository into the common-ci directory.
2. Set up test files: Move the TAC-HashV41.hash file and evidence files to the device-detection-dotnet-examples/device-detection-data directory.
3. Enter device-detection-examples directory: Changes the current working directory to the device-detection-dotnet-examples folder.
4. Install nuget packages to local feed: publishes the packages locally to the user home directory.
5. Set package dependency version: Sets the version of the device-detection package dependency for the examples to the specified Version parameter. This will be the version installed in the local repository found in the .nuget folder.
6. Build and Test Examples: Builds the project with updated dependency and runs unit tests in the examples project

## Performance Tests

The script sets up the required directories to store the test results and builds the performance tests project using publish command to produce an executable file that is then passed to the script. If the operating system is Linux, it installs the APR library needed for the tests. It then proceeds to build the performance tests using CMake and generates the necessary executable.

From the generated "summary.json" file, relevant data is extracted to create a performance summary in JSON format. This summary includes DetectionsPerSecond and MsPerDetection metrics.
