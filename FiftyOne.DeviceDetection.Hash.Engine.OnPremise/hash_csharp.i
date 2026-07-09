// Needed to map the data byte array paramater in EngineHash constructor and
// refreshData method in EngineHash.i to C#.
// See https://www.swig.org/Doc4.0/CSharp.html#CSharp_arrays
%include "arrays_csharp.i"
%apply unsigned char INPUT[] {unsigned char data[]}

%include "./device-detection-cxx/src/hash/hash.i";

// -----------------------------------------------------------------------------
// Fix #4 (issue #524): flattened "by value" scalar property accessors.
//
// The generated getValueAsBool/Integer/Double/String each return a heap Value<T>
// wrapper (BoolValueSwig etc.) that the caller then interrogates and disposes -
// several P/Invokes and heap allocations per property read, which dominate the
// managed cost of a detection.
//
// The accessors below return the value plus its has-value flag in one call with
// no wrapper object. They are CURRENTLY IMPLEMENTED as a small hand-written
// translation unit (DeviceDetectionFastValues.cpp) + managed P/Invokes in
// ResultsSwigWrapper, because the checked-in SWIG bindings are ~21 months behind
// the device-detection-cxx submodule and regenerating would pull in a large,
// unrelated binding refresh (see _docs/swig-marshaling-524). The regen is tracked
// in issue #823.
//
// WHEN THE BINDINGS ARE NEXT REGENERATED, replace the hand-written .cpp and the
// managed P/Invokes with the %extend below (uncomment it), so the fast paths are
// SWIG-generated and regen-safe:
//
// %extend FiftyoneDegrees::DeviceDetection::Hash::ResultsHash {
//     bool tryGetBool(const char* name, bool* OUTPUT) { /* getValueAsBool -> hasValue/value */ }
//     bool tryGetInt(const char* name, int* OUTPUT) { /* getValueAsInteger */ }
//     bool tryGetDouble(const char* name, double* OUTPUT) { /* getValueAsDouble */ }
//     // string: return the value into a caller buffer, or use %csmethodmodifiers
//     //         with an out string typemap.
// }
// -----------------------------------------------------------------------------