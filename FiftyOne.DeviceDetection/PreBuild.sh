#!/bin/bash


if [ "$1" = "pattern" ]; then
    API=Pattern
fi
if [ "$2" = "pattern" ]; then
    API=Pattern
fi
if [ "$1" = "hash" ]; then
    API=Hash
fi
if [ "$2" = "hash" ]; then
    API=Hash
fi

SCRIPTPATH="$( cd "$(dirname "$0")" ; pwd -P )"

if [ "$API" = "Pattern" ]; then
  SRCOUT=$SCRIPTPATH/FiftyOne.DeviceDetection.$API.Engine.OnPremise/Interop/Swig
  RES=$SCRIPTPATH/dlls
fi

if [ "$API" = "Hash" ]; then
  SRCOUT=$SCRIPTPATH/FiftyOne.DeviceDetection.$API.Engine.OnPremise/Interop/Swig
  RES=$SCRIPTPATH/dlls
fi

SRCMAIN=$SCRIPTPATH/device-detection-cxx/src
SRCPAT=$SRCMAIN/pattern
SRCHASH=$SRCMAIN/hash
SRCCM=$SRCMAIN/common-cxx
TH="-D FIFTYONEDEGREES_NO_THREADING"
GCCARGS="-c -std=c11 -fPIC -pthread -O2"
GXXARGS="-c -std=c++11 -fPIC -pthread -O2"
LDARGS="-shared -O2 -pthread -std=c++11"
unameOut="$(uname -s)"
case "${unameOut}" in
    Linux*)
    OS=linux
    LDARGS="$LDARGS -static-libgcc -static-libstdc++"
    ;;
    Darwin*)
    OS=mac
    ;;
#    CYGWIN*)    OS=Cygwin;;
#    MINGW*)     OS=MinGw;;
    *)          OS="UNKNOWN:${unameOut}"
esac

if [ "$OS" = "UNKNOWN:${unameOut}" ]; then
  { echo >&2 "Operating system is UNKNOWN:${unameOut}. Aborting."; exit 1; }
fi

if command -v swig >/dev/null 2>&1; then
    if [ "$API" = "Hash" ]; then
        swig -c++ -csharp -namespace FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Interop -dllimport FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Native.dll -outdir $SRCOUT -o $SCRIPTPATH/DeviceDetectionHashEngineSwig_csharp.cpp $SCRIPTPATH/FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Native/hash_csharp.i
    fi
    if [ "$API" = "Pattern" ]; then
        swig -c++ -csharp -namespace FiftyOne.DeviceDetection.Pattern.OnPremise.Interop -dllimport FiftyOne.DeviceDetection.Pattern.Engine.OnPremise.Native.dll -outdir $SRCOUT -o $SCRIPTPATH/DeviceDetectionPatternEngineSwig_csharp.cpp $SCRIPTPATH/FiftyOne.DeviceDetection.Pattern.Engine.OnPremise.Native/pattern_csharp.i
    fi
else
    { echo >&2 "Swig is required to generate wrapper but it's not installed."; }
fi

rm -r obj/
mkdir obj

if [ "$API" = "Pattern" ]; then
    echo "Building Pattern Device Detection library."
    for ARCH in "x86" "x64"
    do
        echo $ARCH
        if [ "$ARCH" = "x86" ]
        then
            M=-m32
        else
            M=-m64
        fi

        # SWIG
        g++ $M $GXXARGS $SCRIPTPATH/DeviceDetectionPatternEngineSwig_csharp.cpp -o obj/DeviceDetectionPatternEngineSwig_csharp.o

        # Pattern
        g++ $M $GXXARGS $SRCPAT/ComponentMetaDataBuilderPattern.cpp -o obj/ComponentMetaDataBuilderPattern.o
        g++ $M $GXXARGS $SRCPAT/ComponentMetaDataCollectionPattern.cpp -o obj/ComponentMetaDataCollectionPattern.o
        g++ $M $GXXARGS $SRCPAT/ConfigPattern.cpp -o obj/ConfigPattern.o
        g++ $M $GXXARGS $SRCPAT/EnginePattern.cpp -o obj/EnginePattern.o
        g++ $M $GXXARGS $SRCPAT/MetaDataPattern.cpp -o obj/MetaDataPattern.o
        g++ $M $GXXARGS $SRCPAT/ProfileMetaDataBuilderPattern.cpp -o obj/ProfileMetaDataBuilderPattern.o
        g++ $M $GXXARGS $SRCPAT/ProfileMetaDataCollectionPattern.cpp -o obj/ProfileMetaDataCollectionPattern.o
        g++ $M $GXXARGS $SRCPAT/PropertyMetaDataBuilderPattern.cpp -o obj/PropertyMetaDataBuilderPattern.o
        g++ $M $GXXARGS $SRCPAT/PropertyMetaDataCollectionPattern.cpp -o obj/PropertyMetaDataCollectionPattern.o
        g++ $M $GXXARGS $SRCPAT/PropertyMetaDataCollectionForComponentPattern.cpp -o obj/PropertyMetaDataCollectionForComponentPattern.o
        g++ $M $GXXARGS $SRCPAT/ResultsPattern.cpp -o obj/ResultsPattern.o
        g++ $M $GXXARGS $SRCPAT/ValueMetaDataBuilderPattern.cpp -o obj/ValueMetaDataBuilderPattern.o
        g++ $M $GXXARGS $SRCPAT/ValueMetaDataCollectionBasePattern.cpp -o obj/ValueMetaDataCollectionBasePattern.o
        g++ $M $GXXARGS $SRCPAT/ValueMetaDataCollectionForProfilePattern.cpp -o obj/ValueMetaDataCollectionForProfilePattern.o
        g++ $M $GXXARGS $SRCPAT/ValueMetaDataCollectionForPropertyPattern.cpp -o obj/ValueMetaDataCollectionForPropertyPattern.o
        g++ $M $GXXARGS $SRCPAT/ValueMetaDataCollectionPattern.cpp -o obj/ValueMetaDataCollectionPattern.o
        gcc $M $GCCARGS $SRCPAT/node.c -o obj/node.o
        gcc $M $GCCARGS $SRCPAT/pattern.c -o obj/pattern.o
        gcc $M $GCCARGS $SRCPAT/signature.c -o obj/signature.o

        # City hash
        gcc $M $GCCARGS $SRCMAIN/cityhash/city.c -o obj/city.o

        # Device detection
        g++ $M $GXXARGS $SRCMAIN/ConfigDeviceDetection.cpp -o obj/ConfigDeviceDetection.o
        g++ $M $GXXARGS $SRCMAIN/EngineDeviceDetection.cpp -o obj/EngineDeviceDetection.o
        g++ $M $GXXARGS $SRCMAIN/EvidenceDeviceDetection.cpp -o obj/EvidenceDeviceDetection.o
        g++ $M $GXXARGS $SRCMAIN/ResultsDeviceDetection.cpp -o obj/ResultsDeviceDetection.o
        gcc $M $GCCARGS $SRCMAIN/dataset-dd.c -o obj/dataset-dd.o
        gcc $M $GCCARGS $SRCMAIN/results-dd.c -o obj/results-dd.o

        # Common
        g++ $M $GXXARGS $SRCCM/CollectionConfig.cpp -o obj/CollectionConfig.o
        g++ $M $GXXARGS $SRCCM/ComponentMetaData.cpp -o obj/ComponentMetaData.o
        g++ $M $GXXARGS $SRCCM/ConfigBase.cpp -o obj/ConfigBase.o
        g++ $M $GXXARGS $SRCCM/Date.cpp -o obj/Date.o
        g++ $M $GXXARGS $SRCCM/EngineBase.cpp -o obj/EngineBase.o
        g++ $M $GXXARGS $SRCCM/EvidenceBase.cpp -o obj/EvidenceBase.o
        g++ $M $GXXARGS $SRCCM/Exceptions.cpp -o obj/Exceptions.o
        g++ $M $GXXARGS $SRCCM/MetaData.cpp -o obj/MetaData.o
        g++ $M $GXXARGS $SRCCM/ProfileMetaData.cpp -o obj/ProfileMetaData.o
        g++ $M $GXXARGS $SRCCM/PropertyMetaData.cpp -o obj/PropertyMetaData.o
        g++ $M $GXXARGS $SRCCM/RequiredPropertiesConfig.cpp -o obj/RequiredPropertiesConfig.o
        g++ $M $GXXARGS $SRCCM/ResultsBase.cpp -o obj/ResultsBase.o
        g++ $M $GXXARGS $SRCCM/ValueMetaData.cpp -o obj/ValueMetaData.o
        gcc $M $GCCARGS $SRCCM/cache.c -o obj/cache.o
        gcc $M $GCCARGS $SRCCM/collection.c -o obj/collection.o
        gcc $M $GCCARGS $SRCCM/component.c -o obj/component.o
        gcc $M $GCCARGS $SRCCM/data.c -o obj/data.o
        gcc $M $GCCARGS $SRCCM/dataset.c -o obj/dataset.o
        gcc $M $GCCARGS $SRCCM/evidence.c -o obj/evidence.o
        gcc $M $GCCARGS $SRCCM/exceptionsc.c -o obj/exceptionsc.o
        gcc $M $GCCARGS $SRCCM/file.c -o obj/file.o
        gcc $M $GCCARGS $SRCCM/headers.c -o obj/headers.o
        gcc $M $GCCARGS $SRCCM/list.c -o obj/list.o
        gcc $M $GCCARGS $SRCCM/memory.c -o obj/memory.o
        gcc $M $GCCARGS $SRCCM/overrides.c -o obj/overrides.o
        gcc $M $GCCARGS $SRCCM/pool.c -o obj/pool.o
        gcc $M $GCCARGS $SRCCM/profile.c -o obj/profile.o
        gcc $M $GCCARGS $SRCCM/properties.c -o obj/properties.o
        gcc $M $GCCARGS $SRCCM/property.c -o obj/property.o
        gcc $M $GCCARGS $SRCCM/resource.c -o obj/resource.o
        gcc $M $GCCARGS $SRCCM/results.c -o obj/results.o
        gcc $M $GCCARGS $SRCCM/status.c -o obj/status.o
        gcc $M $GCCARGS $SRCCM/string.c -o obj/string.o
        gcc $M $GCCARGS $SRCCM/threading.c -o obj/threading.o
        gcc $M $GCCARGS $SRCCM/tree.c -o obj/tree.o
        gcc $M $GCCARGS $SRCCM/value.c -o obj/value.o

        mkdir -p $RES/$OS/$ARCH

        # Shared Library
        g++ $M $LDARGS obj/*.o -o $RES/$OS/$ARCH/libFiftyOne.DeviceDetection.Pattern.Engine.OnPremise.Native.dll
    done
fi

rm obj/*.o

if [ "$API" = "Hash" ]; then

    echo "Building Hash Device Detection library."
    for ARCH in "x86" "x64"
    do
        echo $ARCH
        if [ "$ARCH" = "x86" ]
        then
            M=-m32
        else
            M=-m64
        fi

        # SWIG
        g++ $M $GXXARGS $SCRIPTPATH/DeviceDetectionHashEngineSwig_csharp.cpp -o obj/DeviceDetectionHashEngineSwig_csharp.o

        # Hash
        g++ $M $GXXARGS $SRCHASH/ComponentMetaDataCollectionHash.cpp -o obj/ComponentMetaDataCollectionHash.o
        g++ $M $GXXARGS $SRCHASH/ComponentMetaDataCollectionHashGenerated.cpp -o obj/ComponentMetaDataCollectionHashGenerated.o
        g++ $M $GXXARGS $SRCHASH/ConfigHash.cpp -o obj/ConfigHash.o
        g++ $M $GXXARGS $SRCHASH/EngineHash.cpp -o obj/EngineHash.o
        g++ $M $GXXARGS $SRCHASH/MetaDataHash.cpp -o obj/MetaDataHash.o
        g++ $M $GXXARGS $SRCHASH/PropertyMetaDataCollectionForComponentHash.cpp -o obj/PropertyMetaDataForComponentHash.o
        g++ $M $GXXARGS $SRCHASH/PropertyMetaDataCollectionForComponentHashGenerated.cpp -o obj/PropertyMetaDataForComponentHashGenerated.o
        g++ $M $GXXARGS $SRCHASH/PropertyMetaDataCollectionHash.cpp -o obj/PropertyMetaDataCollectionHash.o
        g++ $M $GXXARGS $SRCHASH/PropertyMetaDataCollectionHashGenerated.cpp -o obj/PropertyMetaDataCollectionHashGenerated.o
        g++ $M $GXXARGS $SRCHASH/ResultsHash.cpp -o obj/ResultsHash.o
        gcc $M $GCCARGS $SRCHASH/hash.c -o obj/hash.o

        # Device detection
        g++ $M $GXXARGS $SRCMAIN/ConfigDeviceDetection.cpp -o obj/ConfigDeviceDetection.o
        g++ $M $GXXARGS $SRCMAIN/EngineDeviceDetection.cpp -o obj/EngineDeviceDetection.o
        g++ $M $GXXARGS $SRCMAIN/EvidenceDeviceDetection.cpp -o obj/EvidenceDeviceDetection.o
        g++ $M $GXXARGS $SRCMAIN/ResultsDeviceDetection.cpp -o obj/ResultsDeviceDetection.o
        gcc $M $GCCARGS $SRCMAIN/dataset-dd.c -o obj/dataset-dd.o
        gcc $M $GCCARGS $SRCMAIN/results-dd.c -o obj/results-dd.o

        # Common
        g++ $M $GXXARGS $SRCCM/CollectionConfig.cpp -o obj/CollectionConfig.o
        g++ $M $GXXARGS $SRCCM/ComponentMetaData.cpp -o obj/ComponentMetaData.o
        g++ $M $GXXARGS $SRCCM/ConfigBase.cpp -o obj/ConfigBase.o
        g++ $M $GXXARGS $SRCCM/Date.cpp -o obj/Date.o
        g++ $M $GXXARGS $SRCCM/EngineBase.cpp -o obj/EngineBase.o
        g++ $M $GXXARGS $SRCCM/EvidenceBase.cpp -o obj/EvidenceBase.o
        g++ $M $GXXARGS $SRCCM/Exceptions.cpp -o obj/Exceptions.o
        g++ $M $GXXARGS $SRCCM/MetaData.cpp -o obj/MetaData.o
        g++ $M $GXXARGS $SRCCM/ProfileMetaData.cpp -o obj/ProfileMetaData.o
        g++ $M $GXXARGS $SRCCM/PropertyMetaData.cpp -o obj/PropertyMetaData.o
        g++ $M $GXXARGS $SRCCM/RequiredPropertiesConfig.cpp -o obj/RequiredPropertiesConfig.o
        g++ $M $GXXARGS $SRCCM/ResultsBase.cpp -o obj/ResultsBase.o
        g++ $M $GXXARGS $SRCCM/ValueMetaData.cpp -o obj/ValueMetaData.o
        gcc $M $GCCARGS $SRCCM/cache.c -o obj/cache.o
        gcc $M $GCCARGS $SRCCM/collection.c -o obj/collection.o
        gcc $M $GCCARGS $SRCCM/component.c -o obj/component.o
        gcc $M $GCCARGS $SRCCM/data.c -o obj/data.o
        gcc $M $GCCARGS $SRCCM/dataset.c -o obj/dataset.o
        gcc $M $GCCARGS $SRCCM/evidence.c -o obj/evidence.o
        gcc $M $GCCARGS $SRCCM/exceptionsc.c -o obj/exceptionsc.o
        gcc $M $GCCARGS $SRCCM/file.c -o obj/file.o
        gcc $M $GCCARGS $SRCCM/headers.c -o obj/headers.o
        gcc $M $GCCARGS $SRCCM/list.c -o obj/list.o
        gcc $M $GCCARGS $SRCCM/memory.c -o obj/memory.o
        gcc $M $GCCARGS $SRCCM/overrides.c -o obj/overrides.o
        gcc $M $GCCARGS $SRCCM/pool.c -o obj/pool.o
        gcc $M $GCCARGS $SRCCM/profile.c -o obj/profile.o
        gcc $M $GCCARGS $SRCCM/properties.c -o obj/properties.o
        gcc $M $GCCARGS $SRCCM/property.c -o obj/property.o
        gcc $M $GCCARGS $SRCCM/resource.c -o obj/resource.o
        gcc $M $GCCARGS $SRCCM/results.c -o obj/results.o
        gcc $M $GCCARGS $SRCCM/status.c -o obj/status.o
        gcc $M $GCCARGS $SRCCM/string.c -o obj/string.o
        gcc $M $GCCARGS $SRCCM/threading.c -o obj/threading.o
        gcc $M $GCCARGS $SRCCM/tree.c -o obj/tree.o
        gcc $M $GCCARGS $SRCCM/value.c -o obj/value.o

        mkdir -p $RES/$OS/$ARCH

        # Shared Library
        g++ $M $LDARGS obj/*.o -o $RES/$OS/$ARCH/libFiftyOne.DeviceDetection.Hash.Engine.OnPremise.Native.dll
    done
fi
