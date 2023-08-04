#!/bin/bash

if [ "$1" = "hash" ]; then
    API=Hash
fi
if [ "$2" = "hash" ]; then
    API=Hash
fi

SCRIPTPATH="$( cd "$(dirname "$0")" ; pwd -P )"

if [ "$API" = "Hash" ]; then
    SRCOUT=$SCRIPTPATH/FiftyOne.DeviceDetection.$API.Engine.OnPremise/Interop/Swig
    RES=$SCRIPTPATH
else
    echo >&2 "No API name supplied. Use hash as an argument."
    exit 1
fi

SRCMAIN=$SCRIPTPATH/device-detection-cxx/src
SRCHASH=$SRCMAIN/hash
SRCCM=$SRCMAIN/common-cxx

# Set common flags
. $SCRIPTPATH/device-detection-cxx/scripts/build_variables.sh

CC_CMD=gcc
CXX_CMD=g++
TH="-D FIFTYONEDEGREES_NO_THREADING"
GCCARGS_SHARED="-c -fPIC -pthread $GCC_COMMON_FLAGS -Wno-strict-aliasing"
GXXARGS_SHARED="-c -fPIC -pthread $GXX_COMMON_FLAGS -Wno-strict-aliasing"
GCCARGS="$GCCARGS_SHARED -Wno-missing-braces -Wno-unused-variable"
GXXARGS="$GXXARGS_SHARED -Wno-missing-braces -Wno-unused-variable"
GXXARGS_WRAPPER=$GXXARGS_SHARED
LDARGS="-shared -O3 -pthread -std=gnu++11"
unameOut="$(uname -s)"
case "${unameOut}" in
    Linux*)
    OS=linux
    LDARGS="$LDARGS -static-libgcc -static-libstdc++ -latomic"
    ;;
    Darwin*)
    OS=mac
    CC_CMD=clang
    CXX_CMD=clang++
    GCCARGS="$GCCARGS -Wno-atomic-alignment"
    ;;
#    CYGWIN*)    OS=Cygwin;;
#    MINGW*)     OS=MinGw;;
    *)          OS="UNKNOWN:${unameOut}"
esac

if [ "$OS" = "UNKNOWN:${unameOut}" ]; then
    { echo >&2 "Operating system is UNKNOWN:${unameOut}. Aborting."; exit 1; }
fi

#
# The C# files generated by swig are done manually when there are changes in the C/C++ layer
# Thus, this step seems to be irrelevant so commented out.
#
#if command -v swig >/dev/null 2>&1; then
#    if [ "$API" = "Hash" ]; then
#        swig -c++ -csharp -namespace FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Interop -dllimport FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Native.dll -outdir $SRCOUT -o $SCRIPTPATH/DeviceDetectionHashEngineSwig_csharp.cpp $SCRIPTPATH/FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Native/hash_csharp.i
#    fi
#else
#    { echo >&2 "Swig is required to generate wrapper but it's not installed."; }
#fi

if [[ -d obj ]]; then
    rm -r obj/
fi
    mkdir obj

# Run the command being passed and check if succeeds
test_execution() {
    echo "Run: $@"
    $@
    if [ $? != 0 ]
    then
        echo >&2 "Failed to execute"
        exit 1
    fi
}

if [ "$API" = "Hash" ]; then

    echo "Building Hash Device Detection library."
    for ARCH in "x86" "x64"
    do
        echo $ARCH
        if [ "$ARCH" = "x86" ]
        then
            if [ "$OS" == "mac" ]
            then
                echo "x86 not supported on MacOS."
                continue
            fi
            M=-m32
        else
            M=-m64
        fi

        # Hash
        test_execution $CXX_CMD $M $GXXARGS $SRCHASH/ComponentMetaDataBuilderHash.cpp -o obj/ComponentMetaDataBuilderHash.o
        test_execution $CXX_CMD $M $GXXARGS $SRCHASH/ComponentMetaDataCollectionHash.cpp -o obj/ComponentMetaDataCollectionHash.o
        test_execution $CXX_CMD $M $GXXARGS $SRCHASH/ConfigHash.cpp -o obj/ConfigHash.o
        test_execution $CXX_CMD $M $GXXARGS $SRCHASH/EngineHash.cpp -o obj/EngineHash.o
        test_execution $CXX_CMD $M $GXXARGS $SRCHASH/MetaDataHash.cpp -o obj/MetaDataHash.o
        test_execution $CXX_CMD $M $GXXARGS $SRCHASH/ProfileMetaDataBuilderHash.cpp -o obj/ProfileMetaDataBuilderHash.o
        test_execution $CXX_CMD $M $GXXARGS $SRCHASH/ProfileMetaDataCollectionHash.cpp -o obj/ProfileMetaDataCollectionHash.o
        test_execution $CXX_CMD $M $GXXARGS $SRCHASH/PropertyMetaDataBuilderHash.cpp -o obj/PropertyMetaDataBuilderHash.o
        test_execution $CXX_CMD $M $GXXARGS $SRCHASH/PropertyMetaDataCollectionForComponentHash.cpp -o obj/PropertyMetaDataForComponentHash.o
        test_execution $CXX_CMD $M $GXXARGS $SRCHASH/PropertyMetaDataCollectionForPropertyHash.cpp -o obj/PropertyMetaDataForPropertyHash.o
        test_execution $CXX_CMD $M $GXXARGS $SRCHASH/PropertyMetaDataCollectionHash.cpp -o obj/PropertyMetaDataCollectionHash.o
        test_execution $CXX_CMD $M $GXXARGS $SRCHASH/ResultsHash.cpp -o obj/ResultsHash.o
        test_execution $CXX_CMD $M $GXXARGS $SRCHASH/ValueMetaDataBuilderHash.cpp -o obj/ValueMetaDataBuilderHash.o
        test_execution $CXX_CMD $M $GXXARGS $SRCHASH/ValueMetaDataCollectionBaseHash.cpp -o obj/ValueMetaDataCollectionBaseHash.o
        test_execution $CXX_CMD $M $GXXARGS $SRCHASH/ValueMetaDataCollectionForProfileHash.cpp -o obj/ValueMetaDataCollectionForProfileHash.o
        test_execution $CXX_CMD $M $GXXARGS $SRCHASH/ValueMetaDataCollectionForPropertyHash.cpp -o obj/ValueMetaDataCollectionForPropertyHash.o
        test_execution $CXX_CMD $M $GXXARGS $SRCHASH/ValueMetaDataCollectionHash.cpp -o obj/ValueMetaDataCollectionHash.o
        test_execution $CC_CMD $M $GCCARGS $SRCHASH/graph.c -o obj/graph.o
        test_execution $CC_CMD $M $GCCARGS $SRCHASH/hash.c -o obj/hash.o

        # Device detection
        test_execution $CXX_CMD $M $GXXARGS $SRCMAIN/ConfigDeviceDetection.cpp -o obj/ConfigDeviceDetection.o
        test_execution $CXX_CMD $M $GXXARGS $SRCMAIN/EngineDeviceDetection.cpp -o obj/EngineDeviceDetection.o
        test_execution $CXX_CMD $M $GXXARGS $SRCMAIN/EvidenceDeviceDetection.cpp -o obj/EvidenceDeviceDetection.o
        test_execution $CXX_CMD $M $GXXARGS $SRCMAIN/ResultsDeviceDetection.cpp -o obj/ResultsDeviceDetection.o
        test_execution $CC_CMD $M $GCCARGS $SRCMAIN/dataset-dd.c -o obj/dataset-dd.o
        test_execution $CC_CMD $M $GCCARGS $SRCMAIN/results-dd.c -o obj/results-dd.o

        # Common
        test_execution $CXX_CMD $M $GXXARGS $SRCCM/CollectionConfig.cpp -o obj/CollectionConfig.o
        test_execution $CXX_CMD $M $GXXARGS $SRCCM/ComponentMetaData.cpp -o obj/ComponentMetaData.o
        test_execution $CXX_CMD $M $GXXARGS $SRCCM/ConfigBase.cpp -o obj/ConfigBase.o
        test_execution $CXX_CMD $M $GXXARGS $SRCCM/Date.cpp -o obj/Date.o
        test_execution $CXX_CMD $M $GXXARGS $SRCCM/EngineBase.cpp -o obj/EngineBase.o
        test_execution $CXX_CMD $M $GXXARGS $SRCCM/EvidenceBase.cpp -o obj/EvidenceBase.o
        test_execution $CXX_CMD $M $GXXARGS $SRCCM/Exceptions.cpp -o obj/Exceptions.o
        test_execution $CXX_CMD $M $GXXARGS $SRCCM/MetaData.cpp -o obj/MetaData.o
        test_execution $CXX_CMD $M $GXXARGS $SRCCM/ProfileMetaData.cpp -o obj/ProfileMetaData.o
        test_execution $CXX_CMD $M $GXXARGS $SRCCM/PropertyMetaData.cpp -o obj/PropertyMetaData.o
        test_execution $CXX_CMD $M $GXXARGS $SRCCM/RequiredPropertiesConfig.cpp -o obj/RequiredPropertiesConfig.o
        test_execution $CXX_CMD $M $GXXARGS $SRCCM/ResultsBase.cpp -o obj/ResultsBase.o
        test_execution $CXX_CMD $M $GXXARGS $SRCCM/ValueMetaData.cpp -o obj/ValueMetaData.o
        test_execution $CC_CMD $M $GCCARGS $SRCCM/cache.c -o obj/cache.o
        test_execution $CC_CMD $M $GCCARGS $SRCCM/collection.c -o obj/collection.o
        test_execution $CC_CMD $M $GCCARGS $SRCCM/component.c -o obj/component.o
        test_execution $CC_CMD $M $GCCARGS $SRCCM/data.c -o obj/data.o
        test_execution $CC_CMD $M $GCCARGS $SRCCM/dataset.c -o obj/dataset.o
        test_execution $CC_CMD $M $GCCARGS $SRCCM/evidence.c -o obj/evidence.o
        test_execution $CC_CMD $M $GCCARGS $SRCCM/exceptionsc.c -o obj/exceptionsc.o
        test_execution $CC_CMD $M $GCCARGS $SRCCM/file.c -o obj/file.o
        test_execution $CC_CMD $M $GCCARGS $SRCCM/float.c -o obj/float.o
        test_execution $CC_CMD $M $GCCARGS $SRCCM/headers.c -o obj/headers.o
        test_execution $CC_CMD $M $GCCARGS $SRCCM/ip.c -o obj/ip.o
        test_execution $CC_CMD $M $GCCARGS $SRCCM/list.c -o obj/list.o
        test_execution $CC_CMD $M $GCCARGS $SRCCM/memory.c -o obj/memory.o
        test_execution $CC_CMD $M $GCCARGS $SRCCM/overrides.c -o obj/overrides.o
        test_execution $CC_CMD $M $GCCARGS $SRCCM/pool.c -o obj/pool.o
        test_execution $CC_CMD $M $GCCARGS $SRCCM/process.c -o obj/process.o
        test_execution $CC_CMD $M $GCCARGS $SRCCM/profile.c -o obj/profile.o
        test_execution $CC_CMD $M $GCCARGS $SRCCM/properties.c -o obj/properties.o
        test_execution $CC_CMD $M $GCCARGS $SRCCM/property.c -o obj/property.o
        test_execution $CC_CMD $M $GCCARGS $SRCCM/pseudoheader.c -o obj/pseudoheader.o
        test_execution $CC_CMD $M $GCCARGS $SRCCM/resource.c -o obj/resource.o
        test_execution $CC_CMD $M $GCCARGS $SRCCM/results.c -o obj/results.o
        test_execution $CC_CMD $M $GCCARGS $SRCCM/status.c -o obj/status.o
        test_execution $CC_CMD $M $GCCARGS $SRCCM/string.c -o obj/string.o
        test_execution $CC_CMD $M $GCCARGS $SRCCM/threading.c -o obj/threading.o
        test_execution $CC_CMD $M $GCCARGS $SRCCM/tree.c -o obj/tree.o
        test_execution $CC_CMD $M $GCCARGS $SRCCM/value.c -o obj/value.o

        mkdir -p $RES/$OS/$ARCH
        # Static Library
        test_execution ar rcvs $RES/$OS/$ARCH/libDeviceDetectionHashEngine-$OS-$ARCH.a obj/*.o

        # SWIG
        test_execution $CXX_CMD $M $GXXARGS_WRAPPER $SCRIPTPATH/DeviceDetectionHashEngineSwig_csharp.cpp -o obj/DeviceDetectionHashEngineSwig_csharp.o

        # Shared Library
        test_execution $CXX_CMD $M obj/*.o -o $RES/$OS/$ARCH/libFiftyOne.DeviceDetection.Hash.Engine.OnPremise.Native.dll -L$RES/$OS/$ARCH -lDeviceDetectionHashEngine-$OS-$ARCH $LDARGS
    done
fi