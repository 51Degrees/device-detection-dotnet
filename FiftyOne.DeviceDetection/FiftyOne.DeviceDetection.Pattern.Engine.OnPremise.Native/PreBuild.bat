@for %%X in (swig.exe) do (set SWIG_EXE=%%~$PATH:X)
@if defined SWIG_EXE (
@echo SWIG auto generated code being rebuilt.
swig -c++ -csharp -namespace FiftyOne.DeviceDetection.Pattern.Engine.OnPremise.Interop -dllimport FiftyOne.DeviceDetection.Pattern.Engine.OnPremise.Native.dll -outdir ../FiftyOne.DeviceDetection.Pattern.Engine.OnPremise/Interop/Swig -o ../DeviceDetectionPatternEngineSwig_csharp.cpp pattern_csharp.i
) else (
@echo SWIG not found. SWIG auto generated code will not be rebuilt.
)