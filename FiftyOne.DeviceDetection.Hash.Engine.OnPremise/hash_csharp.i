// Needed to map the data byte array paramater in EngineHash constructor and
// refreshData method in EngineHash.i to C#.
// See https://www.swig.org/Doc4.0/CSharp.html#CSharp_arrays
%include "arrays_csharp.i"
%apply unsigned char INPUT[] {unsigned char data[]}

%include "./device-detection-cxx/src/hash/hash.i";