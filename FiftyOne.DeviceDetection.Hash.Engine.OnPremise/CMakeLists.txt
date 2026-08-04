cmake_minimum_required(VERSION 3.0...3.10 FATAL_ERROR)

set (CMAKE_C_STANDARD 11)
set (CMAKE_CXX_STANDARD 11)

# Include the C API
project(DeviceDetectionHashEngine VERSION 4.3.0 LANGUAGES CXX C)

if (NOT MSVC)
    add_compile_options(-fPIC)
endif()

if (MSVC)
    if (CMAKE_GENERATOR_PLATFORM)
        set (GEN_PLATFORM ${CMAKE_GENERATOR_PLATFORM})
    else()
        set (GEN_PLATFORM ${CMAKE_VS_PLATFORM_NAME_DEFAULT})
    endif()

    if (${GEN_PLATFORM} MATCHES .*64.* AND NOT 32bit)
        set(ARCH x64)
    else()
        set(ARCH x86)
    endif()

else()
    message(STATUS "System Processor: ${CMAKE_SYSTEM_PROCESSOR}")
    if (${CMAKE_SYSTEM_PROCESSOR} MATCHES ^armhf)
        set(ARCH armhf)
    elseif (${CMAKE_SYSTEM_PROCESSOR} MATCHES ^aarch64)
        set(ARCH aarch64)
    elseif (${CMAKE_SYSTEM_PROCESSOR} MATCHES ^arm64)
        set(ARCH aarch64)
    elseif (${CMAKE_SYSTEM_PROCESSOR} MATCHES 64)
        if (32bit)
            message(STATUS "32bit: ${32bit}")
            set (ARCH x86)
        else()
            message(STATUS "32bit: ${32bit}")
            set (ARCH x64)
        endif()
    else()
        set (ARCH x86)
    endif()
endif()
message(STATUS "Arch name: ${ARCH}")
message(STATUS "Generator Platform: ${CMAKE_GENERATOR_PLATFORM}")

if (MSVC)
    set (OS windows)
elseif (UNIX AND NOT APPLE)
    set (OS linux)
elseif (APPLE)
    set (OS macos)
endif()
message(STATUS "OS name: ${OS}")

include(${CMAKE_CURRENT_LIST_DIR}/device-detection-cxx/CMakeLists.txt NO_POLICY_SCOPE)

if (RebuildSwig)
    find_package(SWIG 4.0 COMPONENTS csharp)
    if (NOT SWIG_FOUND)
        message(WARNING "SWIG cannot be found so will not be run")
    elseif (NOT SWIG_csharp_FOUND)
        message(WARNING "SWIG does not include csharp support so will not be run")
    else()
        message(STATUS "Rebuilding SWIG interfaces")
        set(SRCOUT ${CMAKE_CURRENT_LIST_DIR}/Interop/Swig)
        execute_process(COMMAND ${SWIG_EXECUTABLE} -c++ -csharp -namespace FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Interop -dllimport FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Native.dll -outdir ${SRCOUT} -o ${CMAKE_CURRENT_LIST_DIR}/DeviceDetectionHashEngineSwig_csharp.cpp ${CMAKE_CURRENT_LIST_DIR}/hash_csharp.i)
    endif()
endif()

add_library(fiftyone-hash-dotnet SHARED
    ${CMAKE_CURRENT_LIST_DIR}/DeviceDetectionHashEngineSwig_csharp.cpp)

if(UNIX)
    set_target_properties(fiftyone-hash-dotnet
        PROPERTIES
        RUNTIME_OUTPUT_DIRECTORY "."
        LIBRARY_OUTPUT_DIRECTORY "./${CMAKE_BUILD_TYPE}"
        PREFIX ""
        OUTPUT_NAME "FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Native"
        SUFFIX ".dll"
    )
else()
    set_target_properties(fiftyone-hash-dotnet
        PROPERTIES
        RUNTIME_OUTPUT_DIRECTORY "."
        LIBRARY_OUTPUT_DIRECTORY "."
        OUTPUT_NAME "FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Native"
        SUFFIX ".dll"
    )
endif()

target_link_libraries(fiftyone-hash-dotnet fiftyone-hash-cxx)
if (MSVC)
    # /wd4100 needed to disable "unreferenced formal parameter" which comes from the SWIG file
    target_compile_options(fiftyone-hash-dotnet PRIVATE "/D_CRT_SECURE_NO_WARNINGS" "/W4" "/WX" "/wd4100")
else()
    add_compile_options(-fPIC)
    target_compile_options(fiftyone-hash-dotnet INTERFACE "-static-libgcc -static-libstdc++")
    target_link_options(fiftyone-hash-dotnet INTERFACE "-static-libgcc -static-libstdc++")
endif()