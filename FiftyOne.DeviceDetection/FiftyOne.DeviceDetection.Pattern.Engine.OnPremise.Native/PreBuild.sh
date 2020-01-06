#!/bin/bash

if command -v swig >/dev/null 2>&1; then
    { echo >&2 "Generating Swig wrapper for Pattern."; }
    swig -c++ -csharp -namespace FiftyOne.DeviceDetection.Pattern.Engine.OnPremise.Interop -dllimport FiftyOne.DeviceDetection.Pattern.Engine.OnPremise.Native.dll -outdir ../FiftyOne.DeviceDetection.Pattern.Engine.OnPremise/Interop/Swig -o ../DeviceDetectionPatternEngineSwig_csharp.cpp pattern_csharp.i
else
    { echo >&2 "Swig is required to generate wrapper but it's not installed."; }
fi
