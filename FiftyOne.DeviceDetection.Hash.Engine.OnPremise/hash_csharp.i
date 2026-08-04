// Needed to map the data byte array paramater in EngineHash constructor and
// refreshData method in EngineHash.i to C#.
// See https://www.swig.org/Doc4.0/CSharp.html#CSharp_arrays
%include "arrays_csharp.i"
%apply unsigned char INPUT[] {unsigned char data[]}

// -----------------------------------------------------------------------------
// Fix #4 (issue #524): flattened "by value" scalar property accessors.
//
// The generated getValueAsBool/Integer/Double/String each return a heap Value<T>
// wrapper (BoolValueSwig etc.) that the caller then interrogates and disposes -
// several P/Invokes and heap allocations per property read, which dominate the
// managed cost of a detection.
//
// The %extend below adds tryGet* methods to ResultsHashSwig that return the
// value plus its has-value flag in one call with no wrapper object. They were
// first shipped (PR #822) as a hand-written translation unit plus manual
// P/Invokes because the checked-in bindings were then ~21 months stale and a
// regen was out of scope. That regen has now landed (issue #823), so the fast
// paths are SWIG-generated and regen-safe. See DeviceDataHash for the managed
// slow-path fallback that recovers the no-value message.
//
// Semantics (kept identical to the hand-written version):
// - a disposed managed proxy passes a null pointer: report no-value and let the
//   managed slow-path fallback raise the usual exception. catch(...) would not
//   catch the access violation a null deref causes, hence the explicit guard.
// - any native exception maps to no-value, the slow path then surfaces it.
// - string: the value's byte length is always reported via needed. The bytes
//   are copied into buffer only when they fit (needed < bufferLength).
//   Too-long values leave buffer untouched and the caller falls back to the
//   slow path for the full value.
// -----------------------------------------------------------------------------
%include "typemaps.i"

%apply bool *OUTPUT { bool *fastValue };
%apply int *OUTPUT { int *fastValue };
%apply double *OUTPUT { double *fastValue };
%apply int *OUTPUT { int *needed };
%apply unsigned char OUTPUT[] { unsigned char *buffer };

%{
#include <cstring>
%}

// Note: the class is extended by its SWIG-side name (the .i files redeclare
// ResultsHash in the global namespace); the generated C++ resolves it via the
// using-namespace directives hash.i injects.
%extend ResultsHash {

    bool tryGetBool(const char *propertyName, bool *fastValue) {
        *fastValue = false;
        if (self == NULL) { return false; }
        try {
            Value<bool> value =
                self->getValueAsBool(propertyName);
            if (value.hasValue()) {
                *fastValue = value.getValue();
                return true;
            }
        }
        catch (...) {
        }
        return false;
    }

    bool tryGetInt(const char *propertyName, int *fastValue) {
        *fastValue = 0;
        if (self == NULL) { return false; }
        try {
            Value<int> value =
                self->getValueAsInteger(propertyName);
            if (value.hasValue()) {
                *fastValue = value.getValue();
                return true;
            }
        }
        catch (...) {
        }
        return false;
    }

    bool tryGetDouble(const char *propertyName, double *fastValue) {
        *fastValue = 0.0;
        if (self == NULL) { return false; }
        try {
            Value<double> value =
                self->getValueAsDouble(propertyName);
            if (value.hasValue()) {
                *fastValue = value.getValue();
                return true;
            }
        }
        catch (...) {
        }
        return false;
    }

    bool tryGetString(const char *propertyName, unsigned char *buffer,
        int bufferLength, int *needed) {
        *needed = 0;
        if (self == NULL) { return false; }
        try {
            Value<std::string> value =
                self->getValueAsString(propertyName);
            if (value.hasValue()) {
                std::string s = value.getValue();
                int length = (int)s.length();
                *needed = length;
                if (buffer != NULL && length < bufferLength) {
                    memcpy(buffer, s.c_str(), (size_t)length);
                }
                return true;
            }
        }
        catch (...) {
        }
        return false;
    }
}

%include "./device-detection-cxx/src/hash/hash.i";
