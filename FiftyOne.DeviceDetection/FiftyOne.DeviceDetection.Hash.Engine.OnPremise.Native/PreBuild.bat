@for %%X in (swig.exe) do (set SWIG_EXE=%%~$PATH:X)
@if defined SWIG_EXE (
@echo SWIG auto generated code being rebuilt.
swig -c++ -csharp -namespace FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Interop -dllimport FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Native.dll -outdir ../FiftyOne.DeviceDetection.Hash.Engine.OnPremise/Interop/Swig -o ../DeviceDetectionHashEngineSwig_csharp.cpp hash_csharp.i
) else (
@echo SWIG not found. SWIG auto generated code will not be rebuilt.
)