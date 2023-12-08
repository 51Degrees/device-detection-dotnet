//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (https://www.swig.org).
// Version 4.1.1
//
// Do not make changes to this file unless you know what you are doing - modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------

namespace FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Interop {

public class RequiredPropertiesConfigSwig : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal RequiredPropertiesConfigSwig(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(RequiredPropertiesConfigSwig obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  internal static global::System.Runtime.InteropServices.HandleRef swigRelease(RequiredPropertiesConfigSwig obj) {
    if (obj != null) {
      if (!obj.swigCMemOwn)
        throw new global::System.ApplicationException("Cannot release ownership as memory is not owned");
      global::System.Runtime.InteropServices.HandleRef ptr = obj.swigCPtr;
      obj.swigCMemOwn = false;
      obj.Dispose();
      return ptr;
    } else {
      return new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
    }
  }

  ~RequiredPropertiesConfigSwig() {
    Dispose(false);
  }

  public void Dispose() {
    Dispose(true);
    global::System.GC.SuppressFinalize(this);
  }

  protected virtual void Dispose(bool disposing) {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          DeviceDetectionHashEngineModulePINVOKE.delete_RequiredPropertiesConfigSwig(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }
  }

  public RequiredPropertiesConfigSwig() : this(DeviceDetectionHashEngineModulePINVOKE.new_RequiredPropertiesConfigSwig__SWIG_0(), true) {
    if (DeviceDetectionHashEngineModulePINVOKE.SWIGPendingException.Pending) throw DeviceDetectionHashEngineModulePINVOKE.SWIGPendingException.Retrieve();
  }

  public RequiredPropertiesConfigSwig(VectorStringSwig properties) : this(DeviceDetectionHashEngineModulePINVOKE.new_RequiredPropertiesConfigSwig__SWIG_1(VectorStringSwig.getCPtr(properties)), true) {
    if (DeviceDetectionHashEngineModulePINVOKE.SWIGPendingException.Pending) throw DeviceDetectionHashEngineModulePINVOKE.SWIGPendingException.Retrieve();
  }

  public RequiredPropertiesConfigSwig(string properties) : this(DeviceDetectionHashEngineModulePINVOKE.new_RequiredPropertiesConfigSwig__SWIG_2(properties), true) {
    if (DeviceDetectionHashEngineModulePINVOKE.SWIGPendingException.Pending) throw DeviceDetectionHashEngineModulePINVOKE.SWIGPendingException.Retrieve();
  }

  public VectorStringSwig getProperties() {
    VectorStringSwig ret = new VectorStringSwig(DeviceDetectionHashEngineModulePINVOKE.RequiredPropertiesConfigSwig_getProperties(swigCPtr), true);
    if (DeviceDetectionHashEngineModulePINVOKE.SWIGPendingException.Pending) throw DeviceDetectionHashEngineModulePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

}

}