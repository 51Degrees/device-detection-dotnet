﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FiftyOne.DeviceDetection {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Messages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Messages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("FiftyOne.DeviceDetection.Messages", typeof(Messages).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to AppleProfileEngine must have a device detection engine after it in the pipeline. This may be &apos;CloudRequestEngine&apos; or &apos;DeviceDetectionHashEngine&apos;&quot;..
        /// </summary>
        internal static string ExceptionAppleEngineConfiguration {
            get {
                return ResourceManager.GetString("ExceptionAppleEngineConfiguration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unexpected error creating device detection engine..
        /// </summary>
        internal static string ExceptionErrorOnStartup {
            get {
                return ResourceManager.GetString("ExceptionErrorOnStartup", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No source for engine data. Use SetFilename or SetEngineData to configure this..
        /// </summary>
        internal static string ExceptionNoEngineData {
            get {
                return ResourceManager.GetString("ExceptionNoEngineData", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Evidence value for key &apos;{0}&apos; is not in the expected format. This should be a base 64 encoded JSON string from a JavaScript call to navigator.userAgentData.getHighEntropyValues..
        /// </summary>
        internal static string ExceptionUachJsUnexpectedFormat {
            get {
                return ResourceManager.GetString("ExceptionUachJsUnexpectedFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unrecognized filename &apos;{0}&apos;. Expected a &apos;*.hash&apos; Hash data file..
        /// </summary>
        internal static string ExceptionUnrecognizedFileExtension {
            get {
                return ResourceManager.GetString("ExceptionUnrecognizedFileExtension", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There is already an evidence value for &apos;{0}&apos;. This may be due to using Apple client-side detection in addition to server-side. One or the other should be removed..
        /// </summary>
        internal static string MessageAppleEvidenceConflict {
            get {
                return ResourceManager.GetString("MessageAppleEvidenceConflict", resourceCulture);
            }
        }
    }
}
