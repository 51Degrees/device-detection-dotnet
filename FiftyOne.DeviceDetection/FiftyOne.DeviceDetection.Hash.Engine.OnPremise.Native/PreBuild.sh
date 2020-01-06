#!/bin/bash

if command -v swig >/dev/null 2>&1; then
    { echo >&2 "Generating Swig wrapper for Hash."; }
    swig -c++ -csharp -namespace FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Interop -dllimport FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Native.dll -outdir ../FiftyOne.DeviceDetection.Hash.Engine.OnPremise/Interop/Swig -o ../DeviceDetectionHashEngineSwig_csharp.cpp hash_csharp.i
else
    { echo >&2 "Swig is required to generate wrapper but it's not installed."; }
fi
