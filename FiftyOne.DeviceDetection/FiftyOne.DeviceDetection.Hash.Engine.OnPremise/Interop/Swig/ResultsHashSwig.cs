//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (http://www.swig.org).
// Version 4.0.0
//
// Do not make changes to this file unless you know what you are doing--modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------

namespace FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Interop {

internal class ResultsHashSwig : ResultsDeviceDetectionSwig {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;

  internal ResultsHashSwig(global::System.IntPtr cPtr, bool cMemoryOwn) : base(DeviceDetectionHashEngineModulePINVOKE.ResultsHashSwig_SWIGUpcast(cPtr), cMemoryOwn) {
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(ResultsHashSwig obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  protected override void Dispose(bool disposing) {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          DeviceDetectionHashEngineModulePINVOKE.delete_ResultsHashSwig(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
      base.Dispose(disposing);
    }
  }

  public override string getDeviceId() {
    string ret = DeviceDetectionHashEngineModulePINVOKE.ResultsHashSwig_getDeviceId__SWIG_0(swigCPtr);
    if (DeviceDetectionHashEngineModulePINVOKE.SWIGPendingException.Pending) throw DeviceDetectionHashEngineModulePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public string getDeviceId(uint resultIndex) {
    string ret = DeviceDetectionHashEngineModulePINVOKE.ResultsHashSwig_getDeviceId__SWIG_1(swigCPtr, resultIndex);
    if (DeviceDetectionHashEngineModulePINVOKE.SWIGPendingException.Pending) throw DeviceDetectionHashEngineModulePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int getDifference() {
    int ret = DeviceDetectionHashEngineModulePINVOKE.ResultsHashSwig_getDifference__SWIG_0(swigCPtr);
    if (DeviceDetectionHashEngineModulePINVOKE.SWIGPendingException.Pending) throw DeviceDetectionHashEngineModulePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int getDifference(uint resultIndex) {
    int ret = DeviceDetectionHashEngineModulePINVOKE.ResultsHashSwig_getDifference__SWIG_1(swigCPtr, resultIndex);
    if (DeviceDetectionHashEngineModulePINVOKE.SWIGPendingException.Pending) throw DeviceDetectionHashEngineModulePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int getMethod() {
    int ret = DeviceDetectionHashEngineModulePINVOKE.ResultsHashSwig_getMethod__SWIG_0(swigCPtr);
    if (DeviceDetectionHashEngineModulePINVOKE.SWIGPendingException.Pending) throw DeviceDetectionHashEngineModulePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int getMethod(uint resultIndex) {
    int ret = DeviceDetectionHashEngineModulePINVOKE.ResultsHashSwig_getMethod__SWIG_1(swigCPtr, resultIndex);
    if (DeviceDetectionHashEngineModulePINVOKE.SWIGPendingException.Pending) throw DeviceDetectionHashEngineModulePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int getDrift() {
    int ret = DeviceDetectionHashEngineModulePINVOKE.ResultsHashSwig_getDrift__SWIG_0(swigCPtr);
    if (DeviceDetectionHashEngineModulePINVOKE.SWIGPendingException.Pending) throw DeviceDetectionHashEngineModulePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int getDrift(uint resultIndex) {
    int ret = DeviceDetectionHashEngineModulePINVOKE.ResultsHashSwig_getDrift__SWIG_1(swigCPtr, resultIndex);
    if (DeviceDetectionHashEngineModulePINVOKE.SWIGPendingException.Pending) throw DeviceDetectionHashEngineModulePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int getMatchedNodes() {
    int ret = DeviceDetectionHashEngineModulePINVOKE.ResultsHashSwig_getMatchedNodes(swigCPtr);
    if (DeviceDetectionHashEngineModulePINVOKE.SWIGPendingException.Pending) throw DeviceDetectionHashEngineModulePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int getIterations() {
    int ret = DeviceDetectionHashEngineModulePINVOKE.ResultsHashSwig_getIterations(swigCPtr);
    if (DeviceDetectionHashEngineModulePINVOKE.SWIGPendingException.Pending) throw DeviceDetectionHashEngineModulePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public override string getUserAgent(uint resultIndex) {
    string ret = DeviceDetectionHashEngineModulePINVOKE.ResultsHashSwig_getUserAgent(swigCPtr, resultIndex);
    if (DeviceDetectionHashEngineModulePINVOKE.SWIGPendingException.Pending) throw DeviceDetectionHashEngineModulePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public override int getUserAgents() {
    int ret = DeviceDetectionHashEngineModulePINVOKE.ResultsHashSwig_getUserAgents(swigCPtr);
    if (DeviceDetectionHashEngineModulePINVOKE.SWIGPendingException.Pending) throw DeviceDetectionHashEngineModulePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

}

}
