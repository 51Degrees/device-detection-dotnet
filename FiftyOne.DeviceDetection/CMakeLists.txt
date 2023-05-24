cmake_minimum_required(VERSION 3.0...3.10 FATAL_ERROR)
set (CMAKE_C_STANDARD 11)
set (CMAKE_CXX_STANDARD 11)

# Include the C API
project(DeviceDetectionHashEngine VERSION 4.3.0 LANGUAGES CXX C)

if (MSVC)
    add_compile_options(/wd4100) #Disable "unreferenced formal parameter" which comes from the SWIG file
else()
    add_compile_options(-fPIC)
#    if (NOT Apple)
#    endif()
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
    if (${CMAKE_SYSTEM_PROCESSOR} MATCHES ^armhf)
        set(ARCH armhf)
    elseif (${CMAKE_SYSTEM_PROCESSOR} MATCHES ^aarch64)
        set(ARCH aarch64)
    elseif (${CMAKE_SYSTEM_PROCESSOR} MATCHES ^arm64)
        set(ARCH aarch64)
    elseif (${CMAKE_SYSTEM_PROCESSOR} MATCHES .*64.*)
        if (32bit)
            set (ARCH x86)
        else()
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
    set (OS mac)
endif()
message(STATUS "OS name: ${OS}")

include(${CMAKE_CURRENT_LIST_DIR}/device-detection-cxx/CMakeLists.txt NO_POLICY_SCOPE)

set(SRCOUT ${CMAKE_CURRENT_LIST_DIR}/FiftyOne.DeviceDetection.Hash.Engine.OnPremise/Interop/Swig)
set(RES ${CMAKE_CURRENT_LIST_DIR})


option(RebuildSwig "RebuildSwig" OFF)

if (RebuildSwig)
    find_package(SWIG 4.0 COMPONENTS csharp) # TODO is is cs, csharp, or dotnet?
    if (NOT SWIG_FOUND)
        message(WARNING "SWIG executable cannot be found so will not be run")
    elseif (NOT SWIG_csharp_FOUND) # TODO same as above
        message(WARNING "SWIG does not include csharp support so will not be run")
    else()
        message(STATUS "Rebuilding SWIG interfaces")
        execute_process(COMMAND ${SWIG_EXECUTABLE} -c++ -csharp -namespace FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Interop -dllimport FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Native.dll -outdir ${SRCOUT} -o ${CMAKE_CURRENT_LIST_DIR}/DeviceDetectionHashEngineSwig_csharp.cpp ${CMAKE_CURRENT_LIST_DIR}/hash_csharp.i)
    endif()
endif()
message(STATUS "Done SWIG interfaces")

add_library(fiftyone-hash-dotnet SHARED
    ${CMAKE_CURRENT_LIST_DIR}/DeviceDetectionHashEngineSwig_csharp.cpp)

set_target_properties(fiftyone-hash-dotnet
    PROPERTIES
    RUNTIME_OUTPUT_DIRECTORY "${RES}/${OS}/${ARCH}"
    LIBRARY_OUTPUT_DIRECTORY "${RES}/${OS}/${ARCH}"
    RUNTIME_OUTPUT_DIRECTORY_RELEASE "${RES}/${OS}/${ARCH}"
    PREFIX "$<$<PLATFORM_ID:Linux>:lib>"
    OUTPUT_NAME "FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Native"
    SUFFIX ".dll"
)


target_link_libraries(fiftyone-hash-dotnet fiftyone-hash-cxx)
if (NOT MSVC)
    target_compile_options(fiftyone-hash-dotnet INTERFACE "-static-libgcc -static-libstdc++")
    target_link_options(fiftyone-hash-dotnet INTERFACE "-static-libgcc -static-libstdc++")
endif()

if (MSVC)
	target_compile_options(fiftyone-hash-dotnet PRIVATE "/D_CRT_SECURE_NO_WARNINGS" "/W4" "/WX")
else()
	target_compile_options(fiftyone-hash-dotnet PRIVATE ${COMPILE_OPTION_DEBUG} "-Werror") # TODO required?
endif()

# TODO are these defined for linker? -shared -O3 -pthread -std=gnu++11
# TODO are these defined for compiler? -Wno-missing-braces -Wno-unused-variable -Wno-strict-aliasing -c -fPIC -pthread -std=gnu++11 -O3 -Wall -Werror

# TODO there are extra link requiredments in output for libstdc++.so.6 and libgcc_s.so.1

# TODO are similar header includes required for dotnet?
# target_include_directories(fiftyone-hash-java PUBLIC ${JAVA_INCLUDE_PATH})
# if(WIN32)
#     target_include_directories(fiftyone-hash-java PUBLIC ${JAVA_INCLUDE_PATH}/win32)
# endif()
# if(APPLE)
#     target_include_directories(fiftyone-hash-java PUBLIC ${JAVA_INCLUDE_PATH}/darwin)
# endif()
# if(UNIX AND NOT APPLE)
#     target_include_directories(fiftyone-hash-java PUBLIC ${JAVA_INCLUDE_PATH}/linux)
# endif()
