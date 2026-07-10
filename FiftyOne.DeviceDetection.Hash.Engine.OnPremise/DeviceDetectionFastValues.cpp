/* *********************************************************************
 * This Original Work is copyright of 51 Degrees Mobile Experts Limited.
 * Copyright 2026 51 Degrees Mobile Experts Limited, Davidson House,
 * Forbury Square, Reading, Berkshire, United Kingdom RG1 3EU.
 *
 * This Original Work is licensed under the European Union Public Licence
 * (EUPL) v.1.2 and is subject to its terms as set out below.
 *
 * If a copy of the EUPL was not distributed with this file, You can obtain
 * one at https://opensource.org/licenses/EUPL-1.2.
 *
 * The 'Compatible Licences' set out in the Appendix to the EUPL (as may be
 * amended by the European Commission) shall be deemed incompatible for
 * the purposes of the Work and the provisions of the compatibility
 * clause in Article 5 of the EUPL shall not apply.
 *
 * If using the Work as, or as part of, a network application, by
 * including the attribution notice(s) required under Article 5 of the EUPL
 * in the end user terms of the application under an appropriate heading,
 * such notice(s) shall fulfill the requirements of that article.
 * ********************************************************************* */

/*
 * Flattened, "by value" scalar property accessors for the .NET on-premise hash
 * engine (issue #524, fix #4).
 *
 * The SWIG-generated getValueAsBool/Integer/Double/String each return a heap
 * `Value<T>` wrapper object (a native object with a finalizer, plus its managed
 * proxy) that the caller must then interrogate with hasValue()/getValue() and
 * dispose - roughly four P/Invokes and two heap allocations to read one value.
 * Property reads are the dominant managed cost of a detection, so these helpers
 * return the value (and its has-value flag) across the P/Invoke boundary in a
 * single call with no wrapper object.
 *
 * This is deliberately a small hand-written translation unit rather than a SWIG
 * %extend + regen: the checked-in SWIG bindings are ~21 months behind the cxx
 * submodule, so regenerating would pull in a large, unrelated binding refresh.
 * The equivalent %extend is documented in hash_csharp.i for the next full regen,
 * which is tracked in issue #823 (this TU is retired once that regen lands).
 *
 * Each function calls the same C++ ResultsHash getters the SWIG path uses, so
 * behaviour (value, no-value) is identical; the managed side falls back to the
 * SWIG path when hasValue is false, to recover the no-value message.
 *
 * Invariant: resultsPtr is ResultsHashSwig.getCPtr(...).Handle, i.e. the pointer
 * SWIG stored at construction, which for the concrete proxy is the original
 * ResultsHash* cast to void*. static_cast<ResultsHash*> is therefore exact with
 * no base-class adjustment. This holds only while the binding stores the derived
 * ResultsHash pointer; it would break if a future regen handed us an upcast base
 * pointer, so the managed side must keep passing the ResultsHash proxy's handle.
 */

#include "device-detection-cxx/src/hash/EngineHash.hpp"
#include <string>
#include <cstring>

#ifndef SWIGEXPORT
#  if defined(_WIN32)
#    define SWIGEXPORT __declspec(dllexport)
#  else
#    define SWIGEXPORT
#  endif
#endif

using namespace FiftyoneDegrees::DeviceDetection::Hash;

// Bool: returns 0/1; *hasValue set to 1 when a value is present, else 0.
extern "C" SWIGEXPORT int fiftyone_hash_get_bool(
    void* resultsPtr, const char* name, int* hasValue) {
    // A disposed managed proxy passes a null pointer; catch(...) would not catch
    // the resulting access violation, so report no-value and let the managed
    // slow-path fallback raise the usual exception.
    if (resultsPtr == nullptr) { *hasValue = 0; return 0; }
    try {
        ResultsHash* results = static_cast<ResultsHash*>(resultsPtr);
        auto value = results->getValueAsBool(name);
        if (value.hasValue()) {
            *hasValue = 1;
            return value.getValue() ? 1 : 0;
        }
    }
    catch (...) {
    }
    *hasValue = 0;
    return 0;
}

// Integer: returns the value; *hasValue set to 1 when present, else 0.
extern "C" SWIGEXPORT int fiftyone_hash_get_int(
    void* resultsPtr, const char* name, int* hasValue) {
    if (resultsPtr == nullptr) { *hasValue = 0; return 0; }
    try {
        ResultsHash* results = static_cast<ResultsHash*>(resultsPtr);
        auto value = results->getValueAsInteger(name);
        if (value.hasValue()) {
            *hasValue = 1;
            return value.getValue();
        }
    }
    catch (...) {
    }
    *hasValue = 0;
    return 0;
}

// Double: returns the value; *hasValue set to 1 when present, else 0.
extern "C" SWIGEXPORT double fiftyone_hash_get_double(
    void* resultsPtr, const char* name, int* hasValue) {
    if (resultsPtr == nullptr) { *hasValue = 0; return 0.0; }
    try {
        ResultsHash* results = static_cast<ResultsHash*>(resultsPtr);
        auto value = results->getValueAsDouble(name);
        if (value.hasValue()) {
            *hasValue = 1;
            return value.getValue();
        }
    }
    catch (...) {
    }
    *hasValue = 0;
    return 0.0;
}

// String: returns 1 if a value is present (0 otherwise). When present, *needed is
// set to the value's byte length and, if it fits (length < bufLen), the value is
// copied into buffer NUL-terminated. If it does not fit, buffer is left untouched
// and the caller retries via the slow path. No SWIG wrapper object is created.
extern "C" SWIGEXPORT int fiftyone_hash_get_string(
    void* resultsPtr, const char* name, char* buffer, int bufLen, int* needed) {
    if (resultsPtr == nullptr) { *needed = 0; return 0; }
    try {
        ResultsHash* results = static_cast<ResultsHash*>(resultsPtr);
        auto value = results->getValueAsString(name);
        if (value.hasValue()) {
            std::string s = value.getValue();
            int len = (int)s.length();
            *needed = len;
            if (buffer != nullptr && len < bufLen) {
                memcpy(buffer, s.c_str(), (size_t)len);
                buffer[len] = '\0';
            }
            return 1;
        }
    }
    catch (...) {
    }
    *needed = 0;
    return 0;
}
