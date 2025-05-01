/* *********************************************************************
 * This Original Work is copyright of 51 Degrees Mobile Experts Limited.
 * Copyright 2025 51 Degrees Mobile Experts Limited, Davidson House,
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
using FiftyOne.Pipeline.Core.Data.Types;
using FiftyOne.Pipeline.Core.Data;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.Data;
using FiftyOne.Pipeline.Engines.FlowElements;
using FiftyOne.Pipeline.Engines.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
namespace FiftyOne.DeviceDetection.Shared
{
	/// <summary>
	/// Abstract base class for properties relating to a device.
	/// This includes the hardware, operating system and browser as
	/// well as crawler details if the request actually came from a 
	/// bot or other automated system.
	/// </summary>
	public abstract class DeviceDataBase : AspectDataBase, IDeviceData
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="logger">
		/// The logger for this instance to use.
		/// </param>
		/// <param name="pipeline">
		/// The Pipeline this data instance has been created by.
		/// </param>
		/// <param name="engine">
		/// The engine this data instance has been created by.
		/// </param>
		/// <param name="missingPropertyService">
		/// The missing property service to use when a requested property
		/// does not exist.
		/// </param>
		protected DeviceDataBase(
			ILogger<AspectDataBase> logger,
			IPipeline pipeline,
			IAspectEngine engine,
			IMissingPropertyService missingPropertyService)
			: base(logger, pipeline, engine, missingPropertyService) { }

		/// <summary>
		/// Dictionary of property value types, keyed on the string
		/// name of the type.
		/// </summary>
		protected static readonly IReadOnlyDictionary<string, Type> PropertyTypes =
			new Dictionary<string, Type>()
			{
				{ "AjaxRequestType", typeof(IAspectPropertyValue<string>) },
				{ "AnimationTiming", typeof(IAspectPropertyValue<bool>) },
				{ "BackCameraMegaPixels", typeof(IAspectPropertyValue<double>) },
				{ "BatteryCapacity", typeof(IAspectPropertyValue<int>) },
				{ "BitsPerPixel", typeof(IAspectPropertyValue<int>) },
				{ "BlobBuilder", typeof(IAspectPropertyValue<bool>) },
				{ "BrowserAudioCodecsDecode", typeof(IAspectPropertyValue<IReadOnlyList<string>>) },
				{ "BrowserAudioCodecsEncode", typeof(IAspectPropertyValue<IReadOnlyList<string>>) },
				{ "BrowserDiscontinuedAge", typeof(IAspectPropertyValue<int>) },
				{ "BrowserDiscontinuedMonth", typeof(IAspectPropertyValue<string>) },
				{ "BrowserDiscontinuedYear", typeof(IAspectPropertyValue<int>) },
				{ "BrowserFamily", typeof(IAspectPropertyValue<string>) },
				{ "BrowserLogos", typeof(IAspectPropertyValue<IReadOnlyList<string>>) },
				{ "BrowserName", typeof(IAspectPropertyValue<string>) },
				{ "BrowserPreviewAge", typeof(IAspectPropertyValue<int>) },
				{ "BrowserPreviewMonth", typeof(IAspectPropertyValue<string>) },
				{ "BrowserPreviewYear", typeof(IAspectPropertyValue<int>) },
				{ "BrowserPropertySource", typeof(IAspectPropertyValue<string>) },
				{ "BrowserRank", typeof(IAspectPropertyValue<int>) },
				{ "BrowserReleaseAge", typeof(IAspectPropertyValue<int>) },
				{ "BrowserReleaseMonth", typeof(IAspectPropertyValue<string>) },
				{ "BrowserReleaseYear", typeof(IAspectPropertyValue<int>) },
				{ "BrowserSourceProject", typeof(IAspectPropertyValue<string>) },
				{ "BrowserSourceProjectVersion", typeof(IAspectPropertyValue<string>) },
				{ "BrowserSupportsPrivacySandbox", typeof(IAspectPropertyValue<string>) },
				{ "BrowserVendor", typeof(IAspectPropertyValue<string>) },
				{ "BrowserVersion", typeof(IAspectPropertyValue<string>) },
				{ "BrowserVideoCodecsDecode", typeof(IAspectPropertyValue<IReadOnlyList<string>>) },
				{ "BrowserVideoCodecsEncode", typeof(IAspectPropertyValue<IReadOnlyList<string>>) },
				{ "CameraTypes", typeof(IAspectPropertyValue<IReadOnlyList<string>>) },
				{ "Canvas", typeof(IAspectPropertyValue<bool>) },
				{ "CcppAccept", typeof(IAspectPropertyValue<IReadOnlyList<string>>) },
				{ "CLDC", typeof(IAspectPropertyValue<double>) },
				{ "ContrastRatio", typeof(IAspectPropertyValue<string>) },
				{ "CookiesCapable", typeof(IAspectPropertyValue<bool>) },
				{ "CPU", typeof(IAspectPropertyValue<string>) },
				{ "CPUCores", typeof(IAspectPropertyValue<int>) },
				{ "CPUDesigner", typeof(IAspectPropertyValue<string>) },
				{ "CPUMaximumFrequency", typeof(IAspectPropertyValue<double>) },
				{ "CrawlerName", typeof(IAspectPropertyValue<string>) },
				{ "CssBackground", typeof(IAspectPropertyValue<bool>) },
				{ "CssBorderImage", typeof(IAspectPropertyValue<bool>) },
				{ "CssCanvas", typeof(IAspectPropertyValue<bool>) },
				{ "CssColor", typeof(IAspectPropertyValue<bool>) },
				{ "CssColumn", typeof(IAspectPropertyValue<bool>) },
				{ "CssFlexbox", typeof(IAspectPropertyValue<bool>) },
				{ "CssFont", typeof(IAspectPropertyValue<bool>) },
				{ "CssGrid", typeof(IAspectPropertyValue<bool>) },
				{ "CssImages", typeof(IAspectPropertyValue<bool>) },
				{ "CssMediaQueries", typeof(IAspectPropertyValue<bool>) },
				{ "CssMinMax", typeof(IAspectPropertyValue<bool>) },
				{ "CssOverflow", typeof(IAspectPropertyValue<bool>) },
				{ "CssPosition", typeof(IAspectPropertyValue<bool>) },
				{ "CssText", typeof(IAspectPropertyValue<bool>) },
				{ "CssTransforms", typeof(IAspectPropertyValue<bool>) },
				{ "CssTransitions", typeof(IAspectPropertyValue<bool>) },
				{ "CssUI", typeof(IAspectPropertyValue<bool>) },
				{ "DataSet", typeof(IAspectPropertyValue<bool>) },
				{ "DataUrl", typeof(IAspectPropertyValue<bool>) },
				{ "DeviceCertifications", typeof(IAspectPropertyValue<IReadOnlyList<string>>) },
				{ "DeviceId", typeof(IAspectPropertyValue<string>) },
				{ "DeviceOrientation", typeof(IAspectPropertyValue<bool>) },
				{ "DeviceRAM", typeof(IAspectPropertyValue<int>) },
				{ "DeviceRAMVariants", typeof(IAspectPropertyValue<IReadOnlyList<string>>) },
				{ "DeviceType", typeof(IAspectPropertyValue<string>) },
				{ "Difference", typeof(IAspectPropertyValue<int>) },
				{ "Drift", typeof(IAspectPropertyValue<int>) },
				{ "Durability", typeof(IAspectPropertyValue<IReadOnlyList<string>>) },
				{ "DynamicContrastRatio", typeof(IAspectPropertyValue<string>) },
				{ "EnergyConsumptionPerYear", typeof(IAspectPropertyValue<int>) },
				{ "ExpansionSlotMaxSize", typeof(IAspectPropertyValue<int>) },
				{ "ExpansionSlotType", typeof(IAspectPropertyValue<IReadOnlyList<string>>) },
				{ "Fetch", typeof(IAspectPropertyValue<bool>) },
				{ "FileReader", typeof(IAspectPropertyValue<bool>) },
				{ "FileSaver", typeof(IAspectPropertyValue<bool>) },
				{ "FileWriter", typeof(IAspectPropertyValue<bool>) },
				{ "FormData", typeof(IAspectPropertyValue<bool>) },
				{ "FrequencyBands", typeof(IAspectPropertyValue<IReadOnlyList<string>>) },
				{ "FrontCameraMegaPixels", typeof(IAspectPropertyValue<double>) },
				{ "Fullscreen", typeof(IAspectPropertyValue<bool>) },
				{ "GeoLocation", typeof(IAspectPropertyValue<bool>) },
				{ "GPU", typeof(IAspectPropertyValue<string>) },
				{ "GPUDesigner", typeof(IAspectPropertyValue<string>) },
				{ "HardwareAudioCodecsDecode", typeof(IAspectPropertyValue<IReadOnlyList<string>>) },
				{ "HardwareAudioCodecsEncode", typeof(IAspectPropertyValue<IReadOnlyList<string>>) },
				{ "HardwareCarrier", typeof(IAspectPropertyValue<string>) },
				{ "HardwareFamily", typeof(IAspectPropertyValue<string>) },
				{ "HardwareImages", typeof(IAspectPropertyValue<IReadOnlyList<string>>) },
				{ "HardwareModel", typeof(IAspectPropertyValue<string>) },
				{ "HardwareModelVariants", typeof(IAspectPropertyValue<IReadOnlyList<string>>) },
				{ "HardwareName", typeof(IAspectPropertyValue<IReadOnlyList<string>>) },
				{ "HardwareProfileSource", typeof(IAspectPropertyValue<string>) },
				{ "HardwareRank", typeof(IAspectPropertyValue<int>) },
				{ "HardwareVendor", typeof(IAspectPropertyValue<string>) },
				{ "HardwareVideoCodecsDecode", typeof(IAspectPropertyValue<IReadOnlyList<string>>) },
				{ "HardwareVideoCodecsEncode", typeof(IAspectPropertyValue<IReadOnlyList<string>>) },
				{ "Has3DCamera", typeof(IAspectPropertyValue<bool>) },
				{ "Has3DScreen", typeof(IAspectPropertyValue<bool>) },
				{ "HasCamera", typeof(IAspectPropertyValue<bool>) },
				{ "HasClickWheel", typeof(IAspectPropertyValue<bool>) },
				{ "HasKeypad", typeof(IAspectPropertyValue<bool>) },
				{ "HasNFC", typeof(IAspectPropertyValue<bool>) },
				{ "HasQwertyPad", typeof(IAspectPropertyValue<bool>) },
				{ "HasRemovableBattery", typeof(IAspectPropertyValue<bool>) },
				{ "HasTouchScreen", typeof(IAspectPropertyValue<bool>) },
				{ "HasTrackPad", typeof(IAspectPropertyValue<bool>) },
				{ "HasVirtualQwerty", typeof(IAspectPropertyValue<bool>) },
				{ "History", typeof(IAspectPropertyValue<bool>) },
				{ "Html-Media-Capture", typeof(IAspectPropertyValue<bool>) },
				{ "Html5", typeof(IAspectPropertyValue<bool>) },
				{ "Html5Audio", typeof(IAspectPropertyValue<IReadOnlyList<string>>) },
				{ "Html5Video", typeof(IAspectPropertyValue<IReadOnlyList<string>>) },
				{ "HtmlVersion", typeof(IAspectPropertyValue<double>) },
				{ "Http2", typeof(IAspectPropertyValue<bool>) },
				{ "HttpLiveStreaming", typeof(IAspectPropertyValue<string>) },
				{ "Iframe", typeof(IAspectPropertyValue<bool>) },
				{ "IndexedDB", typeof(IAspectPropertyValue<bool>) },
				{ "InternalStorageVariants", typeof(IAspectPropertyValue<IReadOnlyList<string>>) },
				{ "InVRMode", typeof(IAspectPropertyValue<bool>) },
				{ "IsArtificialIntelligence", typeof(IAspectPropertyValue<string>) },
				{ "IsConsole", typeof(IAspectPropertyValue<bool>) },
				{ "IsCrawler", typeof(IAspectPropertyValue<bool>) },
				{ "IsDataMinimising", typeof(IAspectPropertyValue<bool>) },
				{ "IsEmailBrowser", typeof(IAspectPropertyValue<bool>) },
				{ "IsEmulatingDesktop", typeof(IAspectPropertyValue<bool>) },
				{ "IsEmulatingDevice", typeof(IAspectPropertyValue<bool>) },
				{ "IsEReader", typeof(IAspectPropertyValue<bool>) },
				{ "IsHardwareGroup", typeof(IAspectPropertyValue<bool>) },
				{ "IsMediaHub", typeof(IAspectPropertyValue<bool>) },
				{ "IsMobile", typeof(IAspectPropertyValue<bool>) },
				{ "IsScreenFoldable", typeof(IAspectPropertyValue<bool>) },
				{ "IsSmallScreen", typeof(IAspectPropertyValue<bool>) },
				{ "IsSmartPhone", typeof(IAspectPropertyValue<bool>) },
				{ "IsSmartWatch", typeof(IAspectPropertyValue<bool>) },
				{ "IsTablet", typeof(IAspectPropertyValue<bool>) },
				{ "IsTv", typeof(IAspectPropertyValue<bool>) },
				{ "IsWebApp", typeof(IAspectPropertyValue<bool>) },
				{ "Iterations", typeof(IAspectPropertyValue<int>) },
				{ "Javascript", typeof(IAspectPropertyValue<bool>) },
				{ "JavaScriptBrowserOverride", typeof(IAspectPropertyValue<JavaScript>) },
				{ "JavascriptCanManipulateCSS", typeof(IAspectPropertyValue<bool>) },
				{ "JavascriptCanManipulateDOM", typeof(IAspectPropertyValue<bool>) },
				{ "JavascriptGetElementById", typeof(IAspectPropertyValue<bool>) },
				{ "JavascriptGetHighEntropyValues", typeof(IAspectPropertyValue<JavaScript>) },
				{ "JavascriptHardwareProfile", typeof(IAspectPropertyValue<JavaScript>) },
				{ "JavascriptImageOptimiser", typeof(IAspectPropertyValue<JavaScript>) },
				{ "JavascriptPreferredGeoLocApi", typeof(IAspectPropertyValue<string>) },
				{ "JavascriptSupportsEventListener", typeof(IAspectPropertyValue<bool>) },
				{ "JavascriptSupportsEvents", typeof(IAspectPropertyValue<bool>) },
				{ "JavascriptSupportsInnerHtml", typeof(IAspectPropertyValue<bool>) },
				{ "JavascriptVersion", typeof(IAspectPropertyValue<string>) },
				{ "Jpeg2000", typeof(IAspectPropertyValue<bool>) },
				{ "jQueryMobileSupport", typeof(IAspectPropertyValue<string>) },
				{ "Json", typeof(IAspectPropertyValue<bool>) },
				{ "LayoutEngine", typeof(IAspectPropertyValue<string>) },
				{ "Masking", typeof(IAspectPropertyValue<bool>) },
				{ "MatchedNodes", typeof(IAspectPropertyValue<int>) },
				{ "MaxInternalStorage", typeof(IAspectPropertyValue<double>) },
				{ "MaxNumberOfSIMCards", typeof(IAspectPropertyValue<int>) },
				{ "MaxStandbyTime", typeof(IAspectPropertyValue<int>) },
				{ "MaxTalkTime", typeof(IAspectPropertyValue<int>) },
				{ "MaxUsageTime", typeof(IAspectPropertyValue<int>) },
				{ "Meter", typeof(IAspectPropertyValue<bool>) },
				{ "Method", typeof(IAspectPropertyValue<string>) },
				{ "MIDP", typeof(IAspectPropertyValue<double>) },
				{ "NativeBrand", typeof(IAspectPropertyValue<IReadOnlyList<string>>) },
				{ "NativeDevice", typeof(IAspectPropertyValue<IReadOnlyList<string>>) },
				{ "NativeModel", typeof(IAspectPropertyValue<IReadOnlyList<string>>) },
				{ "NativeName", typeof(IAspectPropertyValue<IReadOnlyList<string>>) },
				{ "NativePlatform", typeof(IAspectPropertyValue<string>) },
				{ "NumberOfScreens", typeof(IAspectPropertyValue<int>) },
				{ "OEM", typeof(IAspectPropertyValue<string>) },
				{ "OnPowerConsumption", typeof(IAspectPropertyValue<int>) },
				{ "PixelRatio", typeof(IAspectPropertyValue<double>) },
				{ "PixelRatioJavascript", typeof(IAspectPropertyValue<JavaScript>) },
				{ "PlatformDiscontinuedAge", typeof(IAspectPropertyValue<int>) },
				{ "PlatformDiscontinuedMonth", typeof(IAspectPropertyValue<string>) },
				{ "PlatformDiscontinuedYear", typeof(IAspectPropertyValue<int>) },
				{ "PlatformLogos", typeof(IAspectPropertyValue<IReadOnlyList<string>>) },
				{ "PlatformName", typeof(IAspectPropertyValue<string>) },
				{ "PlatformPreviewAge", typeof(IAspectPropertyValue<int>) },
				{ "PlatformPreviewMonth", typeof(IAspectPropertyValue<string>) },
				{ "PlatformPreviewYear", typeof(IAspectPropertyValue<int>) },
				{ "PlatformRank", typeof(IAspectPropertyValue<int>) },
				{ "PlatformReleaseAge", typeof(IAspectPropertyValue<int>) },
				{ "PlatformReleaseMonth", typeof(IAspectPropertyValue<string>) },
				{ "PlatformReleaseYear", typeof(IAspectPropertyValue<int>) },
				{ "PlatformVendor", typeof(IAspectPropertyValue<string>) },
				{ "PlatformVersion", typeof(IAspectPropertyValue<string>) },
				{ "PostMessage", typeof(IAspectPropertyValue<bool>) },
				{ "Preload", typeof(IAspectPropertyValue<bool>) },
				{ "PriceBand", typeof(IAspectPropertyValue<string>) },
				{ "Progress", typeof(IAspectPropertyValue<bool>) },
				{ "Promise", typeof(IAspectPropertyValue<string>) },
				{ "Prompts", typeof(IAspectPropertyValue<bool>) },
				{ "ProtectedAudienceAPIEnabled", typeof(IAspectPropertyValue<string>) },
				{ "ProtectedAudienceAPIEnabledJavaScript", typeof(IAspectPropertyValue<JavaScript>) },
				{ "RefreshRate", typeof(IAspectPropertyValue<int>) },
				{ "ReleaseAge", typeof(IAspectPropertyValue<int>) },
				{ "ReleaseMonth", typeof(IAspectPropertyValue<string>) },
				{ "ReleaseYear", typeof(IAspectPropertyValue<int>) },
				{ "SatelliteNavigationTypes", typeof(IAspectPropertyValue<IReadOnlyList<string>>) },
				{ "ScreenInchesDiagonal", typeof(IAspectPropertyValue<double>) },
				{ "ScreenInchesDiagonalRounded", typeof(IAspectPropertyValue<int>) },
				{ "ScreenInchesHeight", typeof(IAspectPropertyValue<double>) },
				{ "ScreenInchesSquare", typeof(IAspectPropertyValue<int>) },
				{ "ScreenInchesWidth", typeof(IAspectPropertyValue<double>) },
				{ "ScreenMMDiagonal", typeof(IAspectPropertyValue<double>) },
				{ "ScreenMMDiagonalRounded", typeof(IAspectPropertyValue<int>) },
				{ "ScreenMMHeight", typeof(IAspectPropertyValue<double>) },
				{ "ScreenMMSquare", typeof(IAspectPropertyValue<int>) },
				{ "ScreenMMWidth", typeof(IAspectPropertyValue<double>) },
				{ "ScreenPixelsHeight", typeof(IAspectPropertyValue<int>) },
				{ "ScreenPixelsHeightJavaScript", typeof(IAspectPropertyValue<JavaScript>) },
				{ "ScreenPixelsPhysicalHeight", typeof(IAspectPropertyValue<int>) },
				{ "ScreenPixelsPhysicalWidth", typeof(IAspectPropertyValue<int>) },
				{ "ScreenPixelsWidth", typeof(IAspectPropertyValue<int>) },
				{ "ScreenPixelsWidthJavaScript", typeof(IAspectPropertyValue<JavaScript>) },
				{ "ScreenType", typeof(IAspectPropertyValue<string>) },
				{ "SecondBackCameraMegaPixels", typeof(IAspectPropertyValue<double>) },
				{ "SecondFrontCameraMegaPixels", typeof(IAspectPropertyValue<double>) },
				{ "SecondScreenInchesDiagonal", typeof(IAspectPropertyValue<double>) },
				{ "SecondScreenInchesDiagonalRounded", typeof(IAspectPropertyValue<int>) },
				{ "SecondScreenInchesHeight", typeof(IAspectPropertyValue<double>) },
				{ "SecondScreenInchesSquare", typeof(IAspectPropertyValue<int>) },
				{ "SecondScreenInchesWidth", typeof(IAspectPropertyValue<double>) },
				{ "SecondScreenMMDiagonal", typeof(IAspectPropertyValue<double>) },
				{ "SecondScreenMMDiagonalRounded", typeof(IAspectPropertyValue<int>) },
				{ "SecondScreenMMHeight", typeof(IAspectPropertyValue<double>) },
				{ "SecondScreenMMSquare", typeof(IAspectPropertyValue<int>) },
				{ "SecondScreenMMWidth", typeof(IAspectPropertyValue<double>) },
				{ "SecondScreenPixelsHeight", typeof(IAspectPropertyValue<int>) },
				{ "SecondScreenPixelsWidth", typeof(IAspectPropertyValue<int>) },
				{ "Selector", typeof(IAspectPropertyValue<bool>) },
				{ "SetHeaderBrowserAccept-CH", typeof(IAspectPropertyValue<string>) },
				{ "SetHeaderHardwareAccept-CH", typeof(IAspectPropertyValue<string>) },
				{ "SetHeaderPlatformAccept-CH", typeof(IAspectPropertyValue<string>) },
				{ "SharedStorageAPIEnabled", typeof(IAspectPropertyValue<string>) },
				{ "SharedStorageAPIEnabledJavaScript", typeof(IAspectPropertyValue<JavaScript>) },
				{ "SoC", typeof(IAspectPropertyValue<string>) },
				{ "SoCDesigner", typeof(IAspectPropertyValue<string>) },
				{ "SoCModel", typeof(IAspectPropertyValue<string>) },
				{ "SoftwareAudioCodecsDecode", typeof(IAspectPropertyValue<IReadOnlyList<string>>) },
				{ "SoftwareAudioCodecsEncode", typeof(IAspectPropertyValue<IReadOnlyList<string>>) },
				{ "SoftwareVideoCodecsDecode", typeof(IAspectPropertyValue<IReadOnlyList<string>>) },
				{ "SoftwareVideoCodecsEncode", typeof(IAspectPropertyValue<IReadOnlyList<string>>) },
				{ "SpecificAbsorbtionRateEU", typeof(IAspectPropertyValue<double>) },
				{ "SpecificAbsorbtionRateUS", typeof(IAspectPropertyValue<double>) },
				{ "StreamingAccept", typeof(IAspectPropertyValue<IReadOnlyList<string>>) },
				{ "SuggestedImageButtonHeightMms", typeof(IAspectPropertyValue<double>) },
				{ "SuggestedImageButtonHeightPixels", typeof(IAspectPropertyValue<double>) },
				{ "SuggestedLinkSizePixels", typeof(IAspectPropertyValue<double>) },
				{ "SuggestedLinkSizePoints", typeof(IAspectPropertyValue<double>) },
				{ "SupportedBearers", typeof(IAspectPropertyValue<IReadOnlyList<string>>) },
				{ "SupportedBluetooth", typeof(IAspectPropertyValue<double>) },
				{ "SupportedBluetoothProfiles", typeof(IAspectPropertyValue<IReadOnlyList<string>>) },
				{ "SupportedBluetoothVersion", typeof(IAspectPropertyValue<string>) },
				{ "SupportedCameraFeatures", typeof(IAspectPropertyValue<IReadOnlyList<string>>) },
				{ "SupportedChargerTypes", typeof(IAspectPropertyValue<IReadOnlyList<string>>) },
				{ "SupportedI/O", typeof(IAspectPropertyValue<IReadOnlyList<string>>) },
				{ "SupportedSensorTypes", typeof(IAspectPropertyValue<IReadOnlyList<string>>) },
				{ "SupportedSIMCardTypes", typeof(IAspectPropertyValue<IReadOnlyList<string>>) },
				{ "Supports24p", typeof(IAspectPropertyValue<bool>) },
				{ "SupportsPhoneCalls", typeof(IAspectPropertyValue<bool>) },
				{ "SupportsTls/Ssl", typeof(IAspectPropertyValue<bool>) },
				{ "SupportsWebGL", typeof(IAspectPropertyValue<bool>) },
				{ "SupportsWiDi", typeof(IAspectPropertyValue<bool>) },
				{ "Svg", typeof(IAspectPropertyValue<bool>) },
				{ "TAC", typeof(IAspectPropertyValue<IReadOnlyList<string>>) },
				{ "TopicsAPIEnabled", typeof(IAspectPropertyValue<string>) },
				{ "TopicsAPIEnabledJavaScript", typeof(IAspectPropertyValue<JavaScript>) },
				{ "TouchEvents", typeof(IAspectPropertyValue<bool>) },
				{ "Track", typeof(IAspectPropertyValue<bool>) },
				{ "UserAgents", typeof(IAspectPropertyValue<IReadOnlyList<string>>) },
				{ "Video", typeof(IAspectPropertyValue<bool>) },
				{ "Viewport", typeof(IAspectPropertyValue<bool>) },
				{ "WebP", typeof(IAspectPropertyValue<bool>) },
				{ "WebWorkers", typeof(IAspectPropertyValue<bool>) },
				{ "WeightWithBattery", typeof(IAspectPropertyValue<double>) },
				{ "WeightWithoutBattery", typeof(IAspectPropertyValue<double>) },
				{ "Xhr2", typeof(IAspectPropertyValue<bool>) }
			};

		/// <summary>
		/// Indicates if the device's primary data connection is wireless and the device is designed to operate mostly by battery power (e.g. mobile phone, smartphone or tablet). This property does not indicate if the device is a mobile phone or not. Laptops are not classified as mobile devices under this definition and so 'IsMobile' will be 'False'.
		/// </summary>
		public IAspectPropertyValue<bool> IsMobile { get { return GetAs<IAspectPropertyValue<bool>>("IsMobile"); } }
		/// <summary>
		/// Indicates if the device is primarily marketed as a tablet or phablet and has a screen size equal to or greater than 7 inches.
		/// </summary>
		public IAspectPropertyValue<bool> IsTablet { get { return GetAs<IAspectPropertyValue<bool>>("IsTablet"); } }
		/// <summary>
		/// Indicates the width of the device's screen in pixels. This property is not applicable for a device that does not have a screen. For devices such as tablets or TV which are predominantly used in landscape mode, the pixel width will be the larger value compared to the pixel height.
		/// </summary>
		public IAspectPropertyValue<int> ScreenPixelsWidth { get { return GetAs<IAspectPropertyValue<int>>("ScreenPixelsWidth"); } }
		/// <summary>
		/// Indicates the height of the device's screen in pixels.This property is not applicable for a device that does not have a screen. For devices such as tablets or TV which are predominantly used in landscape mode, the pixel height will be the smaller value compared to the pixel width.
		/// </summary>
		public IAspectPropertyValue<int> ScreenPixelsHeight { get { return GetAs<IAspectPropertyValue<int>>("ScreenPixelsHeight"); } }
		/// <summary>
		/// Indicates if the device has a touch screen. This property will return 'False' for a device that does not have an integrated screen.
		/// </summary>
		public IAspectPropertyValue<bool> HasTouchScreen { get { return GetAs<IAspectPropertyValue<bool>>("HasTouchScreen"); } }
		/// <summary>
		/// Indicates if the device has a physical qwerty keyboard.
		/// </summary>
		public IAspectPropertyValue<bool> HasQwertyPad { get { return GetAs<IAspectPropertyValue<bool>>("HasQwertyPad"); } }
		/// <summary>
		/// Indicates the name of the company that manufactures the device or primarily sells it, e.g. Samsung.
		/// </summary>
		public IAspectPropertyValue<string> HardwareVendor { get { return GetAs<IAspectPropertyValue<string>>("HardwareVendor"); } }
		/// <summary>
		/// Indicates the model name or number used primarily by the hardware vendor to identify the device, e.g.SM-T805S. When a model identifier is not available the HardwareName will be used.
		/// </summary>
		public IAspectPropertyValue<string> HardwareModel { get { return GetAs<IAspectPropertyValue<string>>("HardwareModel"); } }
		/// <summary>
		/// Indicates the common marketing names associated with the device, e.g. Xperia Z5.
		/// </summary>
		public IAspectPropertyValue<IReadOnlyList<string>> HardwareName { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("HardwareName"); } }
		/// <summary>
		/// Indicates if the device is primarily a game console, such as an Xbox or Playstation.
		/// </summary>
		public IAspectPropertyValue<bool> IsConsole { get { return GetAs<IAspectPropertyValue<bool>>("IsConsole"); } }
		/// <summary>
		/// Indicates the name of the company that developed the operating system.
		/// </summary>
		public IAspectPropertyValue<string> PlatformVendor { get { return GetAs<IAspectPropertyValue<string>>("PlatformVendor"); } }
		/// <summary>
		/// Indicates the name of the operating system the device is using.
		/// </summary>
		public IAspectPropertyValue<string> PlatformName { get { return GetAs<IAspectPropertyValue<string>>("PlatformName"); } }
		/// <summary>
		/// Indicates the version or subversion of the software platform.
		/// </summary>
		public IAspectPropertyValue<string> PlatformVersion { get { return GetAs<IAspectPropertyValue<string>>("PlatformVersion"); } }
		/// <summary>
		/// Refers to the name of the embedded technology the browser uses to display formatted content on the screen.
		/// </summary>
		public IAspectPropertyValue<string> LayoutEngine { get { return GetAs<IAspectPropertyValue<string>>("LayoutEngine"); } }
		/// <summary>
		/// Indicates the name of the company which created the browser.
		/// </summary>
		public IAspectPropertyValue<string> BrowserVendor { get { return GetAs<IAspectPropertyValue<string>>("BrowserVendor"); } }
		/// <summary>
		/// Indicates the name of the browser. Many mobile browsers, by default, come with an operating system (OS). Unless specifically named, these browsers are named after the accompanying OS and/or the layout engine. 
		/// </summary>
		public IAspectPropertyValue<string> BrowserName { get { return GetAs<IAspectPropertyValue<string>>("BrowserName"); } }
		/// <summary>
		/// Refers to the screen width of the device in millimetres. This property will return 'Unknown' for desktops or for devices which do not have an integrated screen. For devices such as tablets or TV which are predominantly used in landscape mode, the screen height will be the smaller value compared to the screen width.
		/// </summary>
		public IAspectPropertyValue<double> ScreenMMWidth { get { return GetAs<IAspectPropertyValue<double>>("ScreenMMWidth"); } }
		/// <summary>
		/// Refers to the screen height of the device in millimetres. This property will return 'Unknown' for desktops or for devices which do not have an integrated screen. For devices such as tablets or TV which are predominantly used in landscape mode, the screen height will be the smaller value compared to the screen width.
		/// </summary>
		public IAspectPropertyValue<double> ScreenMMHeight { get { return GetAs<IAspectPropertyValue<double>>("ScreenMMHeight"); } }
		/// <summary>
		/// Indicates the number of bits used to describe the colour of each individual pixel, also known as bit depth or colour depth.
		/// </summary>
		public IAspectPropertyValue<int> BitsPerPixel { get { return GetAs<IAspectPropertyValue<int>>("BitsPerPixel"); } }
		/// <summary>
		/// Indicates the version or subversion of the browser.
		/// </summary>
		public IAspectPropertyValue<string> BrowserVersion { get { return GetAs<IAspectPropertyValue<string>>("BrowserVersion"); } }
		/// <summary>
		/// Indicates the official name of the CPU within the SoC, e.g. ARM Cortex A9 or Krait (Qualcomm).
		/// </summary>
		public IAspectPropertyValue<string> CPU { get { return GetAs<IAspectPropertyValue<string>>("CPU"); } }
		/// <summary>
		/// Stands for Composite Capability/Preference Profiles.  Refers to the list of MIME types supported by the operating system. The list does not include MIME types that are only enabled through the use of 3rd party applications.
		/// </summary>
		public IAspectPropertyValue<IReadOnlyList<string>> CcppAccept { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("CcppAccept"); } }
		/// <summary>
		/// Refers to the latest version of HyperText Markup Language (HTML) supported by the browser.
		/// </summary>
		public IAspectPropertyValue<double> HtmlVersion { get { return GetAs<IAspectPropertyValue<double>>("HtmlVersion"); } }
		/// <summary>
		/// Indicates if the browser supports JavaScript.
		/// </summary>
		public IAspectPropertyValue<bool> Javascript { get { return GetAs<IAspectPropertyValue<bool>>("Javascript"); } }
		/// <summary>
		/// Indicates which JavaScript version the browser uses. The number refers to JavaScript versioning, not ECMAscript or Jscript. If the browser doesn't support JavaScript then 'NotSupported' value is returned.
		/// </summary>
		public IAspectPropertyValue<string> JavascriptVersion { get { return GetAs<IAspectPropertyValue<string>>("JavascriptVersion"); } }
		/// <summary>
		/// Indicates the list of wireless data technologies supported by the device, including Bluetooth and Wi-Fi. For example, 4G cellular network technologies includes 'LTE' (Long Term Evolution), and 5G technologies includes 'NR' (New Radio). If the device supports phone calls, the SMS value is also returned.
		/// </summary>
		public IAspectPropertyValue<IReadOnlyList<string>> SupportedBearers { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("SupportedBearers"); } }
		/// <summary>
		/// This Property is no longer being supported. Please see Properties, SupportedBluetooth and SupportedBluetoothProfiles for the relevant data.
		/// </summary>
		public IAspectPropertyValue<string> SupportedBluetoothVersion { get { return GetAs<IAspectPropertyValue<string>>("SupportedBluetoothVersion"); } }
		/// <summary>
		/// Indicates the maximum frequency of the CPU of the device in gigahertz (GHz).
		/// </summary>
		public IAspectPropertyValue<double> CPUMaximumFrequency { get { return GetAs<IAspectPropertyValue<double>>("CPUMaximumFrequency"); } }
		/// <summary>
		/// Indicates the year in which the device was released or the year in which the device was first seen by 51Degrees (if the release date cannot be identified).
		/// </summary>
		public IAspectPropertyValue<int> ReleaseYear { get { return GetAs<IAspectPropertyValue<int>>("ReleaseYear"); } }
		/// <summary>
		/// Indicates the month in which the device was released or the month in which the device was first seen by 51Degrees (if the release date cannot be identified).
		/// </summary>
		public IAspectPropertyValue<string> ReleaseMonth { get { return GetAs<IAspectPropertyValue<string>>("ReleaseMonth"); } }
		/// <summary>
		/// Indicates if the browser supports http Cookies. However, the user may have disabled Cookies in their own configuration. Where data cannot be validated, it is assumed that the browser supports cookies.
		/// </summary>
		public IAspectPropertyValue<bool> CookiesCapable { get { return GetAs<IAspectPropertyValue<bool>>("CookiesCapable"); } }
		/// <summary>
		/// A list of MIME types the device can stream. The list does not include MIME types that are only supported through the use of 3rd party applications.
		/// </summary>
		public IAspectPropertyValue<IReadOnlyList<string>> StreamingAccept { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("StreamingAccept"); } }
		/// <summary>
		/// Indicates if the device has a virtual qwerty keyboard capability.
		/// </summary>
		public IAspectPropertyValue<bool> HasVirtualQwerty { get { return GetAs<IAspectPropertyValue<bool>>("HasVirtualQwerty"); } }
		/// <summary>
		/// Indicates if the device has a physical numeric keypad.
		/// </summary>
		public IAspectPropertyValue<bool> HasKeypad { get { return GetAs<IAspectPropertyValue<bool>>("HasKeypad"); } }
		/// <summary>
		/// Indicates if the device has a camera.
		/// </summary>
		public IAspectPropertyValue<bool> HasCamera { get { return GetAs<IAspectPropertyValue<bool>>("HasCamera"); } }
		/// <summary>
		/// Indicates the resolution of the device's back camera in megapixels. For a device that has a rotating camera the same value is returned for front and back megapixels properties.
		/// </summary>
		public IAspectPropertyValue<double> BackCameraMegaPixels { get { return GetAs<IAspectPropertyValue<double>>("BackCameraMegaPixels"); } }
		/// <summary>
		/// Indicates the expansion slot type the device can support.
		/// </summary>
		public IAspectPropertyValue<IReadOnlyList<string>> ExpansionSlotType { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("ExpansionSlotType"); } }
		/// <summary>
		/// Indicates the maximum amount of memory in gigabytes (GB) the expansion slot of the device can support.
		/// </summary>
		public IAspectPropertyValue<int> ExpansionSlotMaxSize { get { return GetAs<IAspectPropertyValue<int>>("ExpansionSlotMaxSize"); } }
		/// <summary>
		/// Indicates the maximum volatile RAM capacity of the device in megabytes (MB). Where a device has different RAM capacity options, the largest option available is returned.
		/// </summary>
		public IAspectPropertyValue<int> DeviceRAM { get { return GetAs<IAspectPropertyValue<int>>("DeviceRAM"); } }
		/// <summary>
		/// Indicates the maximum amount of internal persistent storage (ROM capacity) with which the device is supplied in gigabytes (GB), including the space used by the device's Operating System and bundled applications. This could also be referred to as "Electrically Erasable Programmable Read-Only Memory (EEPROM)" or "Non Volatile Random Access Memory (NVRAM)". Where a device has different internal storage options, the largest option available is returned.
		/// </summary>
		public IAspectPropertyValue<double> MaxInternalStorage { get { return GetAs<IAspectPropertyValue<double>>("MaxInternalStorage"); } }
		/// <summary>
		/// Refers to the suggested optimum height of a button in millimetres. Ensures the button is touchable on a touch screen and not too large on a non-touch screen. 
		/// </summary>
		public IAspectPropertyValue<double> SuggestedImageButtonHeightPixels { get { return GetAs<IAspectPropertyValue<double>>("SuggestedImageButtonHeightPixels"); } }
		/// <summary>
		/// Refers to the suggested optimum height of a button in millimetres. Ensures the button is touchable on a touch screen and not too large on a non-touch screen. Assumes the actual device DPI (Dots Per Inch) is being used. 
		/// </summary>
		public IAspectPropertyValue<double> SuggestedImageButtonHeightMms { get { return GetAs<IAspectPropertyValue<double>>("SuggestedImageButtonHeightMms"); } }
		/// <summary>
		/// Refers to the suggested optimum height of a hyperlink in pixels. Ensures the link is touchable on a touch screen and not too large on a non-touch screen. Assumes the actual device DPI is being used.
		/// </summary>
		public IAspectPropertyValue<double> SuggestedLinkSizePixels { get { return GetAs<IAspectPropertyValue<double>>("SuggestedLinkSizePixels"); } }
		/// <summary>
		/// Refers to the suggested optimum height of a hyperlink in points. Ensures the link is touchable on a touch screen and not too large on a non-touch screen. 
		/// </summary>
		public IAspectPropertyValue<double> SuggestedLinkSizePoints { get { return GetAs<IAspectPropertyValue<double>>("SuggestedLinkSizePoints"); } }
		/// <summary>
		/// Indicates if the device has a trackpad or trackball. Examples of devices that support this property are the Nexus One and Blackberry Curve.
		/// </summary>
		public IAspectPropertyValue<bool> HasTrackPad { get { return GetAs<IAspectPropertyValue<bool>>("HasTrackPad"); } }
		/// <summary>
		/// Indicates if the device has a click wheel such as found on Apple iPod devices.
		/// </summary>
		public IAspectPropertyValue<bool> HasClickWheel { get { return GetAs<IAspectPropertyValue<bool>>("HasClickWheel"); } }
		/// <summary>
		/// Indicates if the device is primarily advertised as an e-reader. If the device type is EReader then the device is not classified as a tablet.
		/// </summary>
		public IAspectPropertyValue<bool> IsEReader { get { return GetAs<IAspectPropertyValue<bool>>("IsEReader"); } }
		/// <summary>
		/// Indicates if the browser supports the JavaScript that can manipulate the Document Object Model on the browser's web page.
		/// </summary>
		public IAspectPropertyValue<bool> JavascriptCanManipulateDOM { get { return GetAs<IAspectPropertyValue<bool>>("JavascriptCanManipulateDOM"); } }
		/// <summary>
		/// Indicates if the browser supports the JavaScript that can manipulate CSS on the browser's web page.
		/// </summary>
		public IAspectPropertyValue<bool> JavascriptCanManipulateCSS { get { return GetAs<IAspectPropertyValue<bool>>("JavascriptCanManipulateCSS"); } }
		/// <summary>
		/// Indicates if the browser allows registration of event listeners on event targets by using the addEventListener() method.
		/// </summary>
		public IAspectPropertyValue<bool> JavascriptSupportsEventListener { get { return GetAs<IAspectPropertyValue<bool>>("JavascriptSupportsEventListener"); } }
		/// <summary>
		/// Indicates if the browser supports the JavaScript events 'onload', 'onclick' and 'onselect'. 
		/// </summary>
		public IAspectPropertyValue<bool> JavascriptSupportsEvents { get { return GetAs<IAspectPropertyValue<bool>>("JavascriptSupportsEvents"); } }
		/// <summary>
		/// Indicates if the browser supports JavaScript that is able to access HTML elements from their ID using the getElementById method.
		/// </summary>
		public IAspectPropertyValue<bool> JavascriptGetElementById { get { return GetAs<IAspectPropertyValue<bool>>("JavascriptGetElementById"); } }
		/// <summary>
		/// Indicates what ajax request format should be used.
		/// </summary>
		public IAspectPropertyValue<string> AjaxRequestType { get { return GetAs<IAspectPropertyValue<string>>("AjaxRequestType"); } }
		/// <summary>
		/// Indicates if the browser supports the JavaScript that is able to insert HTML into a DIV tag.
		/// </summary>
		public IAspectPropertyValue<bool> JavascriptSupportsInnerHtml { get { return GetAs<IAspectPropertyValue<bool>>("JavascriptSupportsInnerHtml"); } }
		/// <summary>
		/// Indicates which GeoLoc API JavaScript the browser supports. If a browser supports a feature to acquire the user's geographical location, another property called 'GeoLocation' will be set to True.
		/// </summary>
		public IAspectPropertyValue<string> JavascriptPreferredGeoLocApi { get { return GetAs<IAspectPropertyValue<string>>("JavascriptPreferredGeoLocApi"); } }
		/// <summary>
		/// Refers to the diagonal size of the screen of the device in millimetres. This property will return 'Unknown' for desktops or for devices which do not have an integrated screen.
		/// </summary>
		public IAspectPropertyValue<double> ScreenMMDiagonal { get { return GetAs<IAspectPropertyValue<double>>("ScreenMMDiagonal"); } }
		/// <summary>
		/// Lists what video formats, if any, the browser supports using the HTLM5 <![CDATA[<video>]]> tag.
		/// </summary>
		public IAspectPropertyValue<IReadOnlyList<string>> Html5Video { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("Html5Video"); } }
		/// <summary>
		/// Lists what audio formats, if any, the browser supports using the HTML5 <![CDATA[<audio>]]> tag.
		/// </summary>
		public IAspectPropertyValue<IReadOnlyList<string>> Html5Audio { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("Html5Audio"); } }
		/// <summary>
		/// Indicates if the browser supports CSS3 columns for setting column- width and column-count.
		/// </summary>
		public IAspectPropertyValue<bool> CssColumn { get { return GetAs<IAspectPropertyValue<bool>>("CssColumn"); } }
		/// <summary>
		/// Indicates if the browser supports CSS3 transitions elements, used for animating changes to properties.
		/// </summary>
		public IAspectPropertyValue<bool> CssTransitions { get { return GetAs<IAspectPropertyValue<bool>>("CssTransitions"); } }
		/// <summary>
		/// Indicates if the device has a camera capable of taking 3D images.
		/// </summary>
		public IAspectPropertyValue<bool> Has3DCamera { get { return GetAs<IAspectPropertyValue<bool>>("Has3DCamera"); } }
		/// <summary>
		/// Indicates if the device has a screen capable of displaying 3D images.
		/// </summary>
		public IAspectPropertyValue<bool> Has3DScreen { get { return GetAs<IAspectPropertyValue<bool>>("Has3DScreen"); } }
		/// <summary>
		/// Indicates if the source of the web traffic identifies itself as operating without human interaction for the purpose of monitoring the availability or performance of a web site, retrieving a response for inclusion in a search engine or is requesting structured data such as via an API. Such sources are often referred to as crawlers, bots, robots, spiders, probes, monitors or HTTP services among other terms. Where the source pretends to be a device operating with human interaction, such as a smartphone or tablet, this property will return, 'False'.
		/// </summary>
		public IAspectPropertyValue<bool> IsCrawler { get { return GetAs<IAspectPropertyValue<bool>>("IsCrawler"); } }
		/// <summary>
		/// Indicates the crawler name when applicable. Returns NotCrawler when the device is not a crawler.
		/// </summary>
		public IAspectPropertyValue<string> CrawlerName { get { return GetAs<IAspectPropertyValue<string>>("CrawlerName"); } }
		/// <summary>
		/// Refers to the grade of the level the device has with the jQuery Mobile Framework, as posted by jQuery.
		/// </summary>
		public IAspectPropertyValue<string> jQueryMobileSupport { get { return GetAs<IAspectPropertyValue<string>>("jQueryMobileSupport"); } }
		/// <summary>
		/// Indicates the list of camera types the device has. If the device has a rotating camera, this property refers to both front and back facing cameras.
		/// </summary>
		public IAspectPropertyValue<IReadOnlyList<string>> CameraTypes { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("CameraTypes"); } }
		/// <summary>
		/// Indicates the number of physical CPU cores the device has.
		/// </summary>
		public IAspectPropertyValue<int> CPUCores { get { return GetAs<IAspectPropertyValue<int>>("CPUCores"); } }
		/// <summary>
		/// Indicates if the browser supports 'window.requestAnimationFrame()' method.
		/// </summary>
		public IAspectPropertyValue<bool> AnimationTiming { get { return GetAs<IAspectPropertyValue<bool>>("AnimationTiming"); } }
		/// <summary>
		/// Indicates if the browser fully supports BlobBuilder, containing a BlobBuilder interface, a FileSaver interface, a FileWriter interface, and a FileWriterSync interface.
		/// </summary>
		public IAspectPropertyValue<bool> BlobBuilder { get { return GetAs<IAspectPropertyValue<bool>>("BlobBuilder"); } }
		/// <summary>
		/// Indicates if the browser supports CSS3 background properties (such as background-image, background-color, etc.) that allow styling of the border and the background of an object, and create a shadow effect.
		/// </summary>
		public IAspectPropertyValue<bool> CssBackground { get { return GetAs<IAspectPropertyValue<bool>>("CssBackground"); } }
		/// <summary>
		/// Indicates if the browser supports border images, allowing decoration of the border around an object.
		/// </summary>
		public IAspectPropertyValue<bool> CssBorderImage { get { return GetAs<IAspectPropertyValue<bool>>("CssBorderImage"); } }
		/// <summary>
		/// Indicates if the browser can draw CSS images into a Canvas.
		/// </summary>
		public IAspectPropertyValue<bool> CssCanvas { get { return GetAs<IAspectPropertyValue<bool>>("CssCanvas"); } }
		/// <summary>
		/// Indicates if the browser supports CSS3 Color, allowing author control of the foreground colour and opacity of an element.
		/// </summary>
		public IAspectPropertyValue<bool> CssColor { get { return GetAs<IAspectPropertyValue<bool>>("CssColor"); } }
		/// <summary>
		/// Indicates if the browser supports flexbox, allowing the automatic reordering of elements on the page when accessed from devices with different screen sizes.
		/// </summary>
		public IAspectPropertyValue<bool> CssFlexbox { get { return GetAs<IAspectPropertyValue<bool>>("CssFlexbox"); } }
		/// <summary>
		/// Indicates if the browser supports CSS3 fonts, including non-standard fonts, e.g. @font-face.
		/// </summary>
		public IAspectPropertyValue<bool> CssFont { get { return GetAs<IAspectPropertyValue<bool>>("CssFont"); } }
		/// <summary>
		/// Indicates if the browser supports CSS3 images, allowing for fall-back images, gradients and other effects.
		/// </summary>
		public IAspectPropertyValue<bool> CssImages { get { return GetAs<IAspectPropertyValue<bool>>("CssImages"); } }
		/// <summary>
		/// Indicates if the browser supports MediaQueries for dynamic CSS that uses the @media rule.
		/// </summary>
		public IAspectPropertyValue<bool> CssMediaQueries { get { return GetAs<IAspectPropertyValue<bool>>("CssMediaQueries"); } }
		/// <summary>
		/// Indicates if the browser supports the CSS 'min-width' and 'max-width' element.
		/// </summary>
		public IAspectPropertyValue<bool> CssMinMax { get { return GetAs<IAspectPropertyValue<bool>>("CssMinMax"); } }
		/// <summary>
		/// Indicates if the browser supports overflowing of clipped blocks.
		/// </summary>
		public IAspectPropertyValue<bool> CssOverflow { get { return GetAs<IAspectPropertyValue<bool>>("CssOverflow"); } }
		/// <summary>
		/// Indicates if the browser supports CSS position, allowing for different box placement algorithms, e.g. static, relative, absolute, fixed and initial.
		/// </summary>
		public IAspectPropertyValue<bool> CssPosition { get { return GetAs<IAspectPropertyValue<bool>>("CssPosition"); } }
		/// <summary>
		/// Indicates if the browser supports all CSS3 text features including: text-overflow, word-wrap and word-break.
		/// </summary>
		public IAspectPropertyValue<bool> CssText { get { return GetAs<IAspectPropertyValue<bool>>("CssText"); } }
		/// <summary>
		/// Indicates if the browser supports 2D transformations in CSS3 including rotating, scaling, etc. This property includes support for both transform and transform-origin properties.
		/// </summary>
		public IAspectPropertyValue<bool> CssTransforms { get { return GetAs<IAspectPropertyValue<bool>>("CssTransforms"); } }
		/// <summary>
		/// Indicates if the browser supports CSS UI stylings, including text-overflow, css3-boxsizing and pointer properties.
		/// </summary>
		public IAspectPropertyValue<bool> CssUI { get { return GetAs<IAspectPropertyValue<bool>>("CssUI"); } }
		/// <summary>
		/// Indicates if the browser has the ability to embed custom data attributes on all HTML elements using the 'data-' prefix.
		/// </summary>
		public IAspectPropertyValue<bool> DataSet { get { return GetAs<IAspectPropertyValue<bool>>("DataSet"); } }
		/// <summary>
		/// Indicates if the browser allows encoded data to be contained in a URL.
		/// </summary>
		public IAspectPropertyValue<bool> DataUrl { get { return GetAs<IAspectPropertyValue<bool>>("DataUrl"); } }
		/// <summary>
		/// Indicates if the browser supports DOM events for device orientation, e.g. 'deviceorientation', 'devicemotion' and 'compassneedscalibration'.
		/// </summary>
		public IAspectPropertyValue<bool> DeviceOrientation { get { return GetAs<IAspectPropertyValue<bool>>("DeviceOrientation"); } }
		/// <summary>
		/// Indicates if the browser supports file reading with events to show progress and errors.
		/// </summary>
		public IAspectPropertyValue<bool> FileReader { get { return GetAs<IAspectPropertyValue<bool>>("FileReader"); } }
		/// <summary>
		/// Indicates if the browser allows Blobs to be saved to client machines with events to show progress and errors. The End-User may opt to decline these files.
		/// </summary>
		public IAspectPropertyValue<bool> FileSaver { get { return GetAs<IAspectPropertyValue<bool>>("FileSaver"); } }
		/// <summary>
		/// Indicates if the browser allows files to be saved to client machines with events to show progress and errors. The End-User may opt to decline these files.
		/// </summary>
		public IAspectPropertyValue<bool> FileWriter { get { return GetAs<IAspectPropertyValue<bool>>("FileWriter"); } }
		/// <summary>
		/// Indicates if the browser supports the 'FormData' object. This property also refers to XMLHttpRequest. If the browser supports 'xhr2', the 'FormData' element will be also supported. 
		/// </summary>
		public IAspectPropertyValue<bool> FormData { get { return GetAs<IAspectPropertyValue<bool>>("FormData"); } }
		/// <summary>
		/// Indicates if the browser supports requests from a video or canvas element to be displayed in full-screen mode.
		/// </summary>
		public IAspectPropertyValue<bool> Fullscreen { get { return GetAs<IAspectPropertyValue<bool>>("Fullscreen"); } }
		/// <summary>
		/// Indicates if the browser supports a feature to acquire the geographical location. For information on which GeoLoc API the browser supports, refer to another property called JavaScriptPreferredGeoLocApi.
		/// </summary>
		public IAspectPropertyValue<bool> GeoLocation { get { return GetAs<IAspectPropertyValue<bool>>("GeoLocation"); } }
		/// <summary>
		/// Indicates if the browser stores the session history for a web page that contains the URLs visited by the browser's user.
		/// </summary>
		public IAspectPropertyValue<bool> History { get { return GetAs<IAspectPropertyValue<bool>>("History"); } }
		/// <summary>
		/// Indicates if the browser is able to use media inputs, e.g. webcam and microphone, in a script and as an input for forms, e.g. '&lt;input type="file" accept="image/*" id="capture"&gt;' would prompt image- capturing software to open.
		/// </summary>
		public IAspectPropertyValue<bool> HtmlMediaCapture { get { return GetAs<IAspectPropertyValue<bool>>("Html-Media-Capture"); } }
		/// <summary>
		/// Indicates if the browser supports the new markup in HTML 5 that also refers to 'New Semantic Elements' such as <![CDATA[<header>, <nav>, <section>, <aside>,<footer>]]> etc.
		/// </summary>
		public IAspectPropertyValue<bool> Html5 { get { return GetAs<IAspectPropertyValue<bool>>("Html5"); } }
		/// <summary>
		/// Indicates if the browser supports the 'Iframe' element, used to embed another document within a current HTML document.
		/// </summary>
		public IAspectPropertyValue<bool> Iframe { get { return GetAs<IAspectPropertyValue<bool>>("Iframe"); } }
		/// <summary>
		/// Indicates if the browser supports an indexed local database.
		/// </summary>
		public IAspectPropertyValue<bool> IndexedDB { get { return GetAs<IAspectPropertyValue<bool>>("IndexedDB"); } }
		/// <summary>
		/// Indicates if the browser supports the 'JSON' object. This property may need a vendor prefix, e.g. webkit, moz, etc.
		/// </summary>
		public IAspectPropertyValue<bool> Json { get { return GetAs<IAspectPropertyValue<bool>>("Json"); } }
		/// <summary>
		/// Indicates if the browser supports messages between different documents.
		/// </summary>
		public IAspectPropertyValue<bool> PostMessage { get { return GetAs<IAspectPropertyValue<bool>>("PostMessage"); } }
		/// <summary>
		/// Indicates if the browser supports progress reports, such as with HTTP requests. The progress element can be used to display the progress of the task. This property doesn't represent a scalar measurement. If the browser supports a gauge, the meter property should be used.
		/// </summary>
		public IAspectPropertyValue<bool> Progress { get { return GetAs<IAspectPropertyValue<bool>>("Progress"); } }
		/// <summary>
		/// Indicates if the browser supports simple dialogues (window.alert, window.confirm and window.prompt).
		/// </summary>
		public IAspectPropertyValue<bool> Prompts { get { return GetAs<IAspectPropertyValue<bool>>("Prompts"); } }
		/// <summary>
		/// Indicates if the browser supports the querySelector() method that returns the first element matching a specified CSS selector(s) in the document.
		/// </summary>
		public IAspectPropertyValue<bool> Selector { get { return GetAs<IAspectPropertyValue<bool>>("Selector"); } }
		/// <summary>
		/// Indicates if the browser supports SVG (scalable vector graphics), useful for 2D animations and applications where all objects within the SVG can be accessed via the DOM and can have assigned event listener elements.
		/// </summary>
		public IAspectPropertyValue<bool> Svg { get { return GetAs<IAspectPropertyValue<bool>>("Svg"); } }
		/// <summary>
		/// Indicates if the browser supports the method of registering and interpreting finder (or stylus) activity on touch screens or trackpads.
		/// </summary>
		public IAspectPropertyValue<bool> TouchEvents { get { return GetAs<IAspectPropertyValue<bool>>("TouchEvents"); } }
		/// <summary>
		/// Indicates if the browser supports a method of tracking text being played with media, e.g. subtitles and captions.
		/// </summary>
		public IAspectPropertyValue<bool> Track { get { return GetAs<IAspectPropertyValue<bool>>("Track"); } }
		/// <summary>
		/// Indicates if the browser supports the 'Video' element for playing videos on web pages without requiring a plug-in.
		/// </summary>
		public IAspectPropertyValue<bool> Video { get { return GetAs<IAspectPropertyValue<bool>>("Video"); } }
		/// <summary>
		/// Indicates if the browser supports Viewport, to give control over view for different screen sizes and resolutions of devices accessing a website.
		/// </summary>
		public IAspectPropertyValue<bool> Viewport { get { return GetAs<IAspectPropertyValue<bool>>("Viewport"); } }
		/// <summary>
		/// Indicates if the browser supports background workers in JavaScript.
		/// </summary>
		public IAspectPropertyValue<bool> WebWorkers { get { return GetAs<IAspectPropertyValue<bool>>("WebWorkers"); } }
		/// <summary>
		/// Indicates if the browser supports client-to-server communication with XmlHttpRequests. If the browser supports 'Xhr2' will also support 'DataForm' element. This property may need a vendor prefix, e.g. webkit, moz, etc.
		/// </summary>
		public IAspectPropertyValue<bool> Xhr2 { get { return GetAs<IAspectPropertyValue<bool>>("Xhr2"); } }
		/// <summary>
		/// Indicates if the browser supports the CSS-mask element that allows users to alter the visibility of an item by either partially or fully hiding the item.
		/// </summary>
		public IAspectPropertyValue<bool> Masking { get { return GetAs<IAspectPropertyValue<bool>>("Masking"); } }
		/// <summary>
		/// Indicates if the browser supports the canvas element, useful for drawing graphics via scripting (usually JavaScript).
		/// </summary>
		public IAspectPropertyValue<bool> Canvas { get { return GetAs<IAspectPropertyValue<bool>>("Canvas"); } }
		/// <summary>
		/// Indicates if the browser supports TLS or SSL, essential for secure protocols such as HTTPS.
		/// </summary>
		public IAspectPropertyValue<bool> SupportsTlsSsl { get { return GetAs<IAspectPropertyValue<bool>>("SupportsTls/Ssl"); } }
		/// <summary>
		/// Indicates which version of the Connected Limited Device Configuration the device supports for use with Java ME.
		/// </summary>
		public IAspectPropertyValue<double> CLDC { get { return GetAs<IAspectPropertyValue<double>>("CLDC"); } }
		/// <summary>
		/// Indicates which version of Mobile Information Device Profile the device supports, used with Java ME and CLDC.
		/// </summary>
		public IAspectPropertyValue<double> MIDP { get { return GetAs<IAspectPropertyValue<double>>("MIDP"); } }
		/// <summary>
		/// Indicates the diagonal size of the device's screen in inches, to a maximum of two decimal points. Where screens have curved corners, the actual viewable area may be less.
		/// </summary>
		public IAspectPropertyValue<double> ScreenInchesDiagonal { get { return GetAs<IAspectPropertyValue<double>>("ScreenInchesDiagonal"); } }
		/// <summary>
		/// Indicates if the device is a TV running on a smart operating system e.g. Android.
		/// </summary>
		public IAspectPropertyValue<bool> IsTv { get { return GetAs<IAspectPropertyValue<bool>>("IsTv"); } }
		/// <summary>
		/// Refers to the height of the device's screen in inches. This property will return 'Unknown' for desktops or for devices which do not have an integrated screen.
		/// </summary>
		public IAspectPropertyValue<double> ScreenInchesHeight { get { return GetAs<IAspectPropertyValue<double>>("ScreenInchesHeight"); } }
		/// <summary>
		/// Refers to the width of the device's screen in inches. This property will return the value 'Unknown' for desktop or for devices which do not have an integrated screen.
		/// </summary>
		public IAspectPropertyValue<double> ScreenInchesWidth { get { return GetAs<IAspectPropertyValue<double>>("ScreenInchesWidth"); } }
		/// <summary>
		/// Indicates whether the device can make and receive phone calls, has a screen size greater than or equal to 2.5 inches, runs a modern operating system (Android, iOS, Windows Phone, BlackBerry etc.), is not designed to be a wearable technology and is marketed by the vendor as a Smartphone.
		/// </summary>
		public IAspectPropertyValue<bool> IsSmartPhone { get { return GetAs<IAspectPropertyValue<bool>>("IsSmartPhone"); } }
		/// <summary>
		/// Indicates if the device is a mobile with a screen size less than 2.5 inches even where the device is marketed as a Smartphone.
		/// </summary>
		public IAspectPropertyValue<bool> IsSmallScreen { get { return GetAs<IAspectPropertyValue<bool>>("IsSmallScreen"); } }
		/// <summary>
		/// A list of images associated with the device. The string contains the caption, followed by the full image URL separated with a tab character.
		/// </summary>
		public IAspectPropertyValue<IReadOnlyList<string>> HardwareImages { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("HardwareImages"); } }
		/// <summary>
		/// Indicates if the device has embedded NFC (Near Field Communication) wireless technology.
		/// </summary>
		public IAspectPropertyValue<bool> HasNFC { get { return GetAs<IAspectPropertyValue<bool>>("HasNFC"); } }
		/// <summary>
		/// Indicates the name of a group of devices that only differ by model or region but are marketed under the same name, e.g. Galaxy Tab S 10.5.
		/// </summary>
		public IAspectPropertyValue<string> HardwareFamily { get { return GetAs<IAspectPropertyValue<string>>("HardwareFamily"); } }
		/// <summary>
		/// Indicates if the application is an email browser (Outlook, Gmail, YahooMail, etc.) that is primarily used to access and manage emails (usually from mobile devices).
		/// </summary>
		public IAspectPropertyValue<bool> IsEmailBrowser { get { return GetAs<IAspectPropertyValue<bool>>("IsEmailBrowser"); } }
		/// <summary>
		/// Indicates the name of the company that manufactures the device.
		/// </summary>
		public IAspectPropertyValue<string> OEM { get { return GetAs<IAspectPropertyValue<string>>("OEM"); } }
		/// <summary>
		/// Indicates the list of charger types supported by the device. For devices that operate via mains power only, e.g. TVs, MediaHubs (which technically aren't being charged) this property is not applicable.
		/// </summary>
		public IAspectPropertyValue<IReadOnlyList<string>> SupportedChargerTypes { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("SupportedChargerTypes"); } }
		/// <summary>
		/// Indicates if the device has a removable battery. This property is not applicable for devices that do not have batteries. Unless otherwise stated this property will return a 'False' value for tablets.
		/// </summary>
		public IAspectPropertyValue<bool> HasRemovableBattery { get { return GetAs<IAspectPropertyValue<bool>>("HasRemovableBattery"); } }
		/// <summary>
		/// Indicates the capacity of the device's standard battery in mAh. This property is not applicable for a device that does not have a battery.
		/// </summary>
		public IAspectPropertyValue<int> BatteryCapacity { get { return GetAs<IAspectPropertyValue<int>>("BatteryCapacity"); } }
		/// <summary>
		/// Indicates  the device's supported satellite navigation types.
		/// </summary>
		public IAspectPropertyValue<IReadOnlyList<string>> SatelliteNavigationTypes { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("SatelliteNavigationTypes"); } }
		/// <summary>
		/// Indicates the list of sensors supported by the device. This property may be not applicable for devices without sensors, such as most feature phones and media hubs.
		/// </summary>
		public IAspectPropertyValue<IReadOnlyList<string>> SupportedSensorTypes { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("SupportedSensorTypes"); } }
		/// <summary>
		/// Indicates the device's Ingress Protection Rating against dust and water (http://en.wikipedia.org/wiki/IP_Code).
		/// </summary>
		public IAspectPropertyValue<IReadOnlyList<string>> Durability { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("Durability"); } }
		/// <summary>
		/// Indicates the maximum number of "Universal Integrated Circuit Cards (UICC - more commonly known as, SIM)" the device can support including both removable and embedded. If the device doesn't support any UICC then a value of '0' is returned.
		/// </summary>
		public IAspectPropertyValue<int> MaxNumberOfSIMCards { get { return GetAs<IAspectPropertyValue<int>>("MaxNumberOfSIMCards"); } }
		/// <summary>
		/// Indicates whether the "Universal Integrated Circuit Card (UICC - more commonly known as, SIM)" is removable or embedded. If removable, the form factor of the UICC is returned.
		/// </summary>
		public IAspectPropertyValue<IReadOnlyList<string>> SupportedSIMCardTypes { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("SupportedSIMCardTypes"); } }
		/// <summary>
		/// Indicates the official name of the graphical chip within the SoC.
		/// </summary>
		public IAspectPropertyValue<string> GPU { get { return GetAs<IAspectPropertyValue<string>>("GPU"); } }
		/// <summary>
		/// Indicates the list of features the device's camera supports.
		/// </summary>
		public IAspectPropertyValue<IReadOnlyList<string>> SupportedCameraFeatures { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("SupportedCameraFeatures"); } }
		/// <summary>
		/// Indicates the maximum talk time of the device in minutes. This property is not applicable for a device that does not have a battery or support phone calls.
		/// </summary>
		public IAspectPropertyValue<int> MaxTalkTime { get { return GetAs<IAspectPropertyValue<int>>("MaxTalkTime"); } }
		/// <summary>
		/// Indicates the maximum standby time of the device in hours. This property is not applicable for a device without a battery.
		/// </summary>
		public IAspectPropertyValue<int> MaxStandbyTime { get { return GetAs<IAspectPropertyValue<int>>("MaxStandbyTime"); } }
		/// <summary>
		/// Indicates the dynamic contrast ratio of the device's screen.
		/// </summary>
		public IAspectPropertyValue<string> DynamicContrastRatio { get { return GetAs<IAspectPropertyValue<string>>("DynamicContrastRatio"); } }
		/// <summary>
		/// Indicates if the device supports Wireless Display Technology.
		/// </summary>
		public IAspectPropertyValue<bool> SupportsWiDi { get { return GetAs<IAspectPropertyValue<bool>>("SupportsWiDi"); } }
		/// <summary>
		/// Indicates the annual energy consumption of the device per year in kWh.
		/// </summary>
		public IAspectPropertyValue<int> EnergyConsumptionPerYear { get { return GetAs<IAspectPropertyValue<int>>("EnergyConsumptionPerYear"); } }
		/// <summary>
		/// Indicates if the device supports 24p; a video format that operates at 24 frames per second.
		/// </summary>
		public IAspectPropertyValue<bool> Supports24p { get { return GetAs<IAspectPropertyValue<bool>>("Supports24p"); } }
		/// <summary>
		/// Indicates the list of input and output communications the device can support, for example 3.5mm jack, micro-USB etc.
		/// </summary>
		public IAspectPropertyValue<IReadOnlyList<string>> SupportedIO { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("SupportedI/O"); } }
		/// <summary>
		/// Indicates the maximum number of frames per second of the output image of the device in Hertz.
		/// </summary>
		public IAspectPropertyValue<int> RefreshRate { get { return GetAs<IAspectPropertyValue<int>>("RefreshRate"); } }
		/// <summary>
		/// Indicates the screen type of the device. This property is not applicable for a device that does not have an integrated screen, e.g. a media hub. If the device manufacturer or vendor does not specify what the screen type of the device is then it is assumed the device has an LCD screen.
		/// </summary>
		public IAspectPropertyValue<string> ScreenType { get { return GetAs<IAspectPropertyValue<string>>("ScreenType"); } }
		/// <summary>
		/// Indicates the power consumption of the device while switched on.
		/// </summary>
		public IAspectPropertyValue<int> OnPowerConsumption { get { return GetAs<IAspectPropertyValue<int>>("OnPowerConsumption"); } }
		/// <summary>
		/// Indicates the contrast ratio of the device.
		/// </summary>
		public IAspectPropertyValue<string> ContrastRatio { get { return GetAs<IAspectPropertyValue<string>>("ContrastRatio"); } }
		/// <summary>
		/// Indicates the maximum general usage time of the device in minutes. This property is not applicable for a device without a battery.
		/// </summary>
		public IAspectPropertyValue<int> MaxUsageTime { get { return GetAs<IAspectPropertyValue<int>>("MaxUsageTime"); } }
		/// <summary>
		/// Indicates the Semiconductor Company that designed the CPU.
		/// </summary>
		public IAspectPropertyValue<string> CPUDesigner { get { return GetAs<IAspectPropertyValue<string>>("CPUDesigner"); } }
		/// <summary>
		/// Indicates the Semiconductor Company that designed the GPU.
		/// </summary>
		public IAspectPropertyValue<string> GPUDesigner { get { return GetAs<IAspectPropertyValue<string>>("GPUDesigner"); } }
		/// <summary>
		/// Indicates the resolution of the device's front camera in megapixels. For a device that has a rotating camera the same value is returned for front and back megapixels' properties.
		/// </summary>
		public IAspectPropertyValue<double> FrontCameraMegaPixels { get { return GetAs<IAspectPropertyValue<double>>("FrontCameraMegaPixels"); } }
		/// <summary>
		/// Indicates the primary marketing name of the System on Chip (chipset) which includes the CPU, GPU and modem. e.g. Snapdragon S4
		/// </summary>
		public IAspectPropertyValue<string> SoC { get { return GetAs<IAspectPropertyValue<string>>("SoC"); } }
		/// <summary>
		/// Indicates the Semiconductor Company that designed the System on Chip (chipset) e.g. Qualcomm, Intel or Mediatek.
		/// </summary>
		public IAspectPropertyValue<string> SoCDesigner { get { return GetAs<IAspectPropertyValue<string>>("SoCDesigner"); } }
		/// <summary>
		/// Indicates the official model of the System on Chip (chipset) e.g. MSM8625, MT8312.
		/// </summary>
		public IAspectPropertyValue<string> SoCModel { get { return GetAs<IAspectPropertyValue<string>>("SoCModel"); } }
		/// <summary>
		/// Indicates if the device is a media hub or set top box that requires an external display(s).
		/// </summary>
		public IAspectPropertyValue<bool> IsMediaHub { get { return GetAs<IAspectPropertyValue<bool>>("IsMediaHub"); } }
		/// <summary>
		/// JavaScript that can override the profile found by the server using information on the client device. This property is applicable for Apple devices which do not provide information about the model in the User-Agent string.
		/// </summary>
		public IAspectPropertyValue<JavaScript> JavascriptHardwareProfile { get { return GetAs<IAspectPropertyValue<JavaScript>>("JavascriptHardwareProfile"); } }
		/// <summary>
		/// Refers to the JavaScript snippet used to determine the response times and bandwidth to monitor the performance of the website.
		/// </summary>
		public IAspectPropertyValue<JavaScript> JavascriptBandwidth { get { return GetAs<IAspectPropertyValue<JavaScript>>("JavascriptBandwidth"); } }
		/// <summary>
		/// Refers to the JavaScript snippet used to optimise images.
		/// </summary>
		public IAspectPropertyValue<JavaScript> JavascriptImageOptimiser { get { return GetAs<IAspectPropertyValue<JavaScript>>("JavascriptImageOptimiser"); } }
		/// <summary>
		/// Refers to the list of audio codecs in specific formats supported for Decode by the Web Browser. This list of codecs is supported for playback on a basic browser installation.
		/// </summary>
		public IAspectPropertyValue<IReadOnlyList<string>> BrowserAudioCodecsDecode { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("BrowserAudioCodecsDecode"); } }
		/// <summary>
		/// Refers to the list of video codecs in specific formats supported for Decode by the Web Browser. This list of codecs is supported for playback on a basic browser installation.
		/// </summary>
		public IAspectPropertyValue<IReadOnlyList<string>> BrowserVideoCodecsDecode { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("BrowserVideoCodecsDecode"); } }
		/// <summary>
		/// Indicates a price range describing the recommended retail price of the device at the date of release, inclusive of tax (where applicable).  Prices are in United States Dollars (USD); if the price is not originally in USD it will be converted to USD using the relevant exchange rate at the time of launch. Prices are for the SIM-free version of the device (if applicable). In cases where there are several versions of the same model of the device, the price will reflect the device that was used to populate the specifications.
		/// </summary>
		public IAspectPropertyValue<string> PriceBand { get { return GetAs<IAspectPropertyValue<string>>("PriceBand"); } }
		/// <summary>
		/// Indicates the area of the device's screen in square millimetres rounded to the nearest whole number. This property will return the value  'Unknown' for desktop or for devices which do not have an integrated screen.
		/// </summary>
		public IAspectPropertyValue<int> ScreenMMSquare { get { return GetAs<IAspectPropertyValue<int>>("ScreenMMSquare"); } }
		/// <summary>
		/// Indicate the diagonal size of the device's screen in millimetres rounded to the nearest whole number. This property will return the value 'Unknown' for desktop or for devices which do not have an integrated screen.
		/// </summary>
		public IAspectPropertyValue<int> ScreenMMDiagonalRounded { get { return GetAs<IAspectPropertyValue<int>>("ScreenMMDiagonalRounded"); } }
		/// <summary>
		/// Indicates the area of the device's screen in square inches rounded to the nearest whole number. This property will return the value 'Unknown' for desktop or for devices which do not have an integrated screen.
		/// </summary>
		public IAspectPropertyValue<int> ScreenInchesSquare { get { return GetAs<IAspectPropertyValue<int>>("ScreenInchesSquare"); } }
		/// <summary>
		/// Indicates the diagonal size of the device's screen in inches rounded to the nearest whole number. This property will return the value 'Unknown' for desktop or for devices which do not have an integrated screen.
		/// </summary>
		public IAspectPropertyValue<int> ScreenInchesDiagonalRounded { get { return GetAs<IAspectPropertyValue<int>>("ScreenInchesDiagonalRounded"); } }
		/// <summary>
		/// Indicates the type of the device based on values set in other properties, such as IsMobile, IsTablet, IsSmartphone, IsSmallScreen etc.
		/// </summary>
		public IAspectPropertyValue<string> DeviceType { get { return GetAs<IAspectPropertyValue<string>>("DeviceType"); } }
		/// <summary>
		/// Indicates the source from which browser properties have been validated. Primary browser data are retrieved from the internal test and populated manually, then they might be validated against an external source such as Caniuse or RingMark. 
		/// </summary>
		public IAspectPropertyValue<string> BrowserPropertySource { get { return GetAs<IAspectPropertyValue<string>>("BrowserPropertySource"); } }
		/// <summary>
		/// Indicates if the browser supports WebGL technology to generate hardware-accelerated 3D graphics.
		/// </summary>
		public IAspectPropertyValue<bool> SupportsWebGL { get { return GetAs<IAspectPropertyValue<bool>>("SupportsWebGL"); } }
		/// <summary>
		/// Indicates if the mobile device accessing a web page emulates a desktop computer. This property is not applicable for desktops, media hubs, TVs and consoles.
		/// </summary>
		public IAspectPropertyValue<bool> IsEmulatingDesktop { get { return GetAs<IAspectPropertyValue<bool>>("IsEmulatingDesktop"); } }
		/// <summary>
		/// Indicates if the device can receive and make telephone calls using available bearers without any additional software such as VoIP. Devices that support voice calls do not necessarily support phone calls.
		/// </summary>
		public IAspectPropertyValue<bool> SupportsPhoneCalls { get { return GetAs<IAspectPropertyValue<bool>>("SupportsPhoneCalls"); } }
		/// <summary>
		/// Indicates if a web page is accessed from an application whose main function is not browsing the World Wide Web or managing emails, e.g. the Facebook App. The application must be downloaded and installed onto the device from an app marketplace such as Apple's App Store or the Google Play Store, or via a third party as an .apk file or similar. This property will return a 'False' value for mobile browsers such as Chrome Mobile or email browsers (such as Hotmail).
		/// </summary>
		public IAspectPropertyValue<bool> IsWebApp { get { return GetAs<IAspectPropertyValue<bool>>("IsWebApp"); } }
		/// <summary>
		/// Indicates if the browser supports a meter element that represents a scalar measurement within a known range or fractional value. This property does not indicate whether the browser supports the progress bar indication. For this purpose, the progress property should be used.
		/// </summary>
		public IAspectPropertyValue<bool> Meter { get { return GetAs<IAspectPropertyValue<bool>>("Meter"); } }
		/// <summary>
		/// Indicates if the device is a web enabled computerised wristwatch with other capabilities beyond timekeeping, such as push notifications. It runs on a Smart Operating System i.e. Android, WatchOS, Tizen, Ubuntu Touch and is designed to be wearable technology.
		/// </summary>
		public IAspectPropertyValue<bool> IsSmartWatch { get { return GetAs<IAspectPropertyValue<bool>>("IsSmartWatch"); } }
		/// <summary>
		/// Indicates the name of the mobile operating system (iOS, Android) for which an application program has been developed to be used by a device.
		/// </summary>
		public IAspectPropertyValue<string> NativePlatform { get { return GetAs<IAspectPropertyValue<string>>("NativePlatform"); } }
		/// <summary>
		/// Refers to the 'Retail Branding' value returned for Android Google Play native applications, when the android.os.Build.BRAND javascript is used to display the class. This property is not applicable for hardware running on operating systems other than Android.
		/// </summary>
		public IAspectPropertyValue<IReadOnlyList<string>> NativeBrand { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("NativeBrand"); } }
		/// <summary>
		/// Refers to the 'Device' value returned for Android Google Play native applications, when the android.os.Build.DEVICE javascript is used to display the class. This property is not applicable for hardware running on operating systems other than Android.
		/// </summary>
		public IAspectPropertyValue<IReadOnlyList<string>> NativeDevice { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("NativeDevice"); } }
		/// <summary>
		/// Refers to the 'Model' value returned for Android Google Play native applications, when the android.os.Build.MODEL javascript is used to display the class. For Apple devices this property refers to the device identifier which appears in the native application from the developer usage log, for example 'iPad5,4'.
		/// </summary>
		public IAspectPropertyValue<IReadOnlyList<string>> NativeModel { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("NativeModel"); } }
		/// <summary>
		/// NativeName Refers to the 'Marketing Name' value that a device is registered with on the Google Play service. This property is not applicable for hardware running on operating systems other than Android.
		/// </summary>
		public IAspectPropertyValue<IReadOnlyList<string>> NativeName { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("NativeName"); } }
		/// <summary>
		/// JavaScript that can override the property value found by the server using information on the client device. This property is applicable for browsers that support screen pixels height cookie.
		/// </summary>
		public IAspectPropertyValue<JavaScript> ScreenPixelsHeightJavaScript { get { return GetAs<IAspectPropertyValue<JavaScript>>("ScreenPixelsHeightJavaScript"); } }
		/// <summary>
		/// JavaScript that can override the property value found by the server using information on the client device. This property is applicable for browsers that support screen pixels width cookie. 
		/// </summary>
		public IAspectPropertyValue<JavaScript> ScreenPixelsWidthJavaScript { get { return GetAs<IAspectPropertyValue<JavaScript>>("ScreenPixelsWidthJavaScript"); } }
		/// <summary>
		/// Indicates what certifications apply to this device.
		/// </summary>
		public IAspectPropertyValue<IReadOnlyList<string>> DeviceCertifications { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("DeviceCertifications"); } }
		/// <summary>
		/// Indicates the resolution of the device's second back camera in megapixels.
		/// </summary>
		public IAspectPropertyValue<double> SecondBackCameraMegaPixels { get { return GetAs<IAspectPropertyValue<double>>("SecondBackCameraMegaPixels"); } }
		/// <summary>
		/// Refers to the list of audio codecs supported for decoding by a Chipset. An audio codec is a program used to playback digital audio files. The values of this property are the codec's common name.
		/// </summary>
		public IAspectPropertyValue<IReadOnlyList<string>> HardwareAudioCodecsDecode { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("HardwareAudioCodecsDecode"); } }
		/// <summary>
		/// Refers to the list of video codecs supported for decoding by a Chipset. An video codec is a program used to playback digital video files. The values of this property are the codec's common name. 
		/// </summary>
		public IAspectPropertyValue<IReadOnlyList<string>> HardwareVideoCodecsDecode { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("HardwareVideoCodecsDecode"); } }
		/// <summary>
		/// Refers to the list of audio codecs supported by an operating system. This list of codecs is supported for playback on a  basic software installation. The values of this property are the codec's common name.
		/// </summary>
		public IAspectPropertyValue<IReadOnlyList<string>> SoftwareAudioCodecsDecode { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("SoftwareAudioCodecsDecode"); } }
		/// <summary>
		/// Refers to the list of audio codecs supported by an operating system. This list of codecs is supported for capture on a basic software installation. The values of this property are the codec's common name.
		/// </summary>
		public IAspectPropertyValue<IReadOnlyList<string>> SoftwareAudioCodecsEncode { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("SoftwareAudioCodecsEncode"); } }
		/// <summary>
		/// Refers to the list of video codecs supported by an operating system. This list of codecs is supported for playback on a  basic software installation. The values of this property are the codec's common name.
		/// </summary>
		public IAspectPropertyValue<IReadOnlyList<string>> SoftwareVideoCodecsDecode { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("SoftwareVideoCodecsDecode"); } }
		/// <summary>
		/// Refers to the list of video codecs supported by an operating system. This list of codecs is supported for capture on a basic software installation. The values of this property are the codec's common name.
		/// </summary>
		public IAspectPropertyValue<IReadOnlyList<string>> SoftwareVideoCodecsEncode { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("SoftwareVideoCodecsEncode"); } }
		/// <summary>
		/// The year in which the platform version was officially released to users by the platform vendor. This version is called the stable version as any bugs or difficulties highlighted in the Beta/Developer Version will have been fixed for this release.
		/// </summary>
		public IAspectPropertyValue<int> PlatformReleaseYear { get { return GetAs<IAspectPropertyValue<int>>("PlatformReleaseYear"); } }
		/// <summary>
		/// The month in which the platform version was officially released to users by the platform vendor. This version is called the stable version as any bugs or difficulties highlighted in the Beta/Developer version will have been fixed for this release.
		/// </summary>
		public IAspectPropertyValue<string> PlatformReleaseMonth { get { return GetAs<IAspectPropertyValue<string>>("PlatformReleaseMonth"); } }
		/// <summary>
		/// The month in which the platform version was originally released as a Beta/Developer version by the platform vendor. This is before it is officially released as a stable version, to ensure wider testing by the community can take place.
		/// </summary>
		public IAspectPropertyValue<string> PlatformPreviewMonth { get { return GetAs<IAspectPropertyValue<string>>("PlatformPreviewMonth"); } }
		/// <summary>
		/// The year in which the platform version was originally released as a Beta/Developer version by the platform vendor. This is before it is officially released as a stable version, to ensure wider testing by the community can take place.
		/// </summary>
		public IAspectPropertyValue<int> PlatformPreviewYear { get { return GetAs<IAspectPropertyValue<int>>("PlatformPreviewYear"); } }
		/// <summary>
		/// The month in which further development for the platform version is stopped by the platform vendor. This occurs when a new stable version of the platform is released.
		/// </summary>
		public IAspectPropertyValue<string> PlatformDiscontinuedMonth { get { return GetAs<IAspectPropertyValue<string>>("PlatformDiscontinuedMonth"); } }
		/// <summary>
		/// The year in which further development for the platform version is stopped by the platform vendor. This occurs when a new stable version of the platform is released.
		/// </summary>
		public IAspectPropertyValue<int> PlatformDiscontinuedYear { get { return GetAs<IAspectPropertyValue<int>>("PlatformDiscontinuedYear"); } }
		/// <summary>
		/// The month in which the browser version is originally released as a Beta/Developer version by the browser vendor. This is before it is officially released as a stable version, to ensure wider testing by the community can take place.
		/// </summary>
		public IAspectPropertyValue<string> BrowserPreviewMonth { get { return GetAs<IAspectPropertyValue<string>>("BrowserPreviewMonth"); } }
		/// <summary>
		/// The year in which the browser version is originally released as a Beta/Developer version by the browser vendor. This is before it is officially released as a stable version, to ensure wider testing by the community can take place.
		/// </summary>
		public IAspectPropertyValue<int> BrowserPreviewYear { get { return GetAs<IAspectPropertyValue<int>>("BrowserPreviewYear"); } }
		/// <summary>
		/// The month in which the browser version is officially released to users by the browser vendor. This version is called the stable version as any bugs or difficulties highlighted in the Beta/Developer Version will have been fixed for this release.
		/// </summary>
		public IAspectPropertyValue<string> BrowserReleaseMonth { get { return GetAs<IAspectPropertyValue<string>>("BrowserReleaseMonth"); } }
		/// <summary>
		/// The year in which the browser version is officially released to users by the browser vendor. This version is called the stable version as any bugs or difficulties highlighted in the Beta/Developer Version will have been fixed for this release.
		/// </summary>
		public IAspectPropertyValue<int> BrowserReleaseYear { get { return GetAs<IAspectPropertyValue<int>>("BrowserReleaseYear"); } }
		/// <summary>
		/// The month in which further development of the browser version is stopped by the browser vendor. This occurs when a new stable version of the browser is released.
		/// </summary>
		public IAspectPropertyValue<string> BrowserDiscontinuedMonth { get { return GetAs<IAspectPropertyValue<string>>("BrowserDiscontinuedMonth"); } }
		/// <summary>
		/// The year in which further development of the browser version is stopped by the browser vendor. This occurs when a new stable version of the browser is released.
		/// </summary>
		public IAspectPropertyValue<int> BrowserDiscontinuedYear { get { return GetAs<IAspectPropertyValue<int>>("BrowserDiscontinuedYear"); } }
		/// <summary>
		/// Refers to the list of audio codecs supported for encoding by a Chipset. An audio codec is a program used to capture digital audio files. The values of this property are the codec's common name.
		/// </summary>
		public IAspectPropertyValue<IReadOnlyList<string>> HardwareAudioCodecsEncode { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("HardwareAudioCodecsEncode"); } }
		/// <summary>
		/// Refers to the list of video codecs supported for encoding by a Chipset. An video codec is a program used to capture digital video files. The values of this property are the codec's common name. 
		/// </summary>
		public IAspectPropertyValue<IReadOnlyList<string>> HardwareVideoCodecsEncode { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("HardwareVideoCodecsEncode"); } }
		/// <summary>
		/// Indicates a browser that does not correctly identify the physical hardware device and instead reports an emulated device.
		/// </summary>
		public IAspectPropertyValue<bool> IsEmulatingDevice { get { return GetAs<IAspectPropertyValue<bool>>("IsEmulatingDevice"); } }
		/// <summary>
		/// Indicates if the browser may be optimised for low bandwidth. A true value indicates the browser supports a feature that can improve performance on low bandwidth connections, either via the removal of elements, features, a proxy or other methods.
		/// </summary>
		public IAspectPropertyValue<bool> IsDataMinimising { get { return GetAs<IAspectPropertyValue<bool>>("IsDataMinimising"); } }
		/// <summary>
		/// Indicates the resolution of the device's second front camera in megapixels.
		/// </summary>
		public IAspectPropertyValue<double> SecondFrontCameraMegaPixels { get { return GetAs<IAspectPropertyValue<double>>("SecondFrontCameraMegaPixels"); } }
		/// <summary>
		/// Refers to the list of audio codecs in specific formats supported for Encode by the Web Browser. This list of codecs is supported for capture on a basic browser installation.
		/// </summary>
		public IAspectPropertyValue<IReadOnlyList<string>> BrowserAudioCodecsEncode { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("BrowserAudioCodecsEncode"); } }
		/// <summary>
		/// Refers to the list of video codecs in specific formats supported for Encode by the Web Browser. This list of codecs is supported for capture on a basic browser installation.
		/// </summary>
		public IAspectPropertyValue<IReadOnlyList<string>> BrowserVideoCodecsEncode { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("BrowserVideoCodecsEncode"); } }
		/// <summary>
		/// The Specific Absorbtion Rate (SAR) is a measure of the rate at which energy is absorbed by the human body when exposed by a radio frequency electromagnetic field. This property contains values in Watts per Kilogram (W/kg) in accordance with the Federal Communications Commission (FCC).
		/// </summary>
		public IAspectPropertyValue<double> SpecificAbsorbtionRateUS { get { return GetAs<IAspectPropertyValue<double>>("SpecificAbsorbtionRateUS"); } }
		/// <summary>
		/// The Specific Absorbtion Rate (SAR) is a measure of the rate at which energy is absorbed by the human body when exposed by a radio frequency electromagnetic field. This property contains values in Watts per Kilogram (W/kg) in accordance with the European Committee for Electrotechnical Standardization (CENELEC).
		/// </summary>
		public IAspectPropertyValue<double> SpecificAbsorbtionRateEU { get { return GetAs<IAspectPropertyValue<double>>("SpecificAbsorbtionRateEU"); } }
		/// <summary>
		/// Indicates the weight of the device with battery in grams.
		/// </summary>
		public IAspectPropertyValue<double> WeightWithBattery { get { return GetAs<IAspectPropertyValue<double>>("WeightWithBattery"); } }
		/// <summary>
		/// Indicates the weight of the device without battery in grams.
		/// </summary>
		public IAspectPropertyValue<double> WeightWithoutBattery { get { return GetAs<IAspectPropertyValue<double>>("WeightWithoutBattery"); } }
		/// <summary>
		/// Indicates if the browser supports HTTP Live Streaming, also known as HLS.
		/// </summary>
		public IAspectPropertyValue<string> HttpLiveStreaming { get { return GetAs<IAspectPropertyValue<string>>("HttpLiveStreaming"); } }
		/// <summary>
		/// Indicates all model numbers used by the hardware vendor to identify the device. This property compliments 'HardwareModel', e.g. Hardware Model Variants A1660 and A1778 correlate to the Hardware Model - iPhone 7.
		/// </summary>
		public IAspectPropertyValue<IReadOnlyList<string>> HardwareModelVariants { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("HardwareModelVariants"); } }
		/// <summary>
		/// Indicates the carrier when the device is sold by the HardwareVendor on a single carrier or as indicated via device User-Agent.
		/// </summary>
		public IAspectPropertyValue<string> HardwareCarrier { get { return GetAs<IAspectPropertyValue<string>>("HardwareCarrier"); } }
		/// <summary>
		/// A measure of the popularity of this device model. All models are ordered by the number of events associated with that model that occurred in the sampling period. The device with the most events is ranked 1, the second 2 and so on. 
		/// </summary>
		public IAspectPropertyValue<int> HardwareRank { get { return GetAs<IAspectPropertyValue<int>>("HardwareRank"); } }
		/// <summary>
		/// A measure of the popularity of this software platform (i.e. OS and version). All platforms are ordered by the number of events associated with that platform that occurred in the sampling period. The platform with the most events is ranked 1, the second 2 and so on.
		/// </summary>
		public IAspectPropertyValue<int> PlatformRank { get { return GetAs<IAspectPropertyValue<int>>("PlatformRank"); } }
		/// <summary>
		/// Indicates the age in months of the operating system since the PlatformPreviewYear and PlatformPreviewMonth.
		/// </summary>
		public IAspectPropertyValue<int> PlatformPreviewAge { get { return GetAs<IAspectPropertyValue<int>>("PlatformPreviewAge"); } }
		/// <summary>
		/// Indicates the age in months of the operating system since the PlatformReleaseYear and PlatformReleaseMonth.
		/// </summary>
		public IAspectPropertyValue<int> PlatformReleaseAge { get { return GetAs<IAspectPropertyValue<int>>("PlatformReleaseAge"); } }
		/// <summary>
		/// Indicates the age in months of the operating system since the PlatformReleaseYear and PlatformReleaseMonth.
		/// </summary>
		public IAspectPropertyValue<int> PlatformDiscontinuedAge { get { return GetAs<IAspectPropertyValue<int>>("PlatformDiscontinuedAge"); } }
		/// <summary>
		/// A measure of the popularity of this browser version. All browsers are ordered by the number of events associated with that browser that occurred in the sampling period. The browser with the most events is ranked 1, the second 2 and so on.
		/// </summary>
		public IAspectPropertyValue<int> BrowserRank { get { return GetAs<IAspectPropertyValue<int>>("BrowserRank"); } }
		/// <summary>
		/// Indicates the age in months of the browser since the BrowserPreviewYear and BrowserPreviewMonth.
		/// </summary>
		public IAspectPropertyValue<int> BrowserPreviewAge { get { return GetAs<IAspectPropertyValue<int>>("BrowserPreviewAge"); } }
		/// <summary>
		/// Indicates the age in months of the browser since the BrowserReleaseYear and BrowserReleaseMonth.
		/// </summary>
		public IAspectPropertyValue<int> BrowserReleaseAge { get { return GetAs<IAspectPropertyValue<int>>("BrowserReleaseAge"); } }
		/// <summary>
		/// Indicates the age in months of the browser since the BrowserDiscontinuedYear and BrowserDiscontinuedMonth.
		/// </summary>
		public IAspectPropertyValue<int> BrowserDiscontinuedAge { get { return GetAs<IAspectPropertyValue<int>>("BrowserDiscontinuedAge"); } }
		/// <summary>
		/// Indicates the age in months of the device since the ReleaseYear and ReleaseMonth.
		/// </summary>
		public IAspectPropertyValue<int> ReleaseAge { get { return GetAs<IAspectPropertyValue<int>>("ReleaseAge"); } }
		/// <summary>
		/// Indicates a profile which contains more than a single hardware device. When this is true all returned properties represent the default value or lowest given specification of all grouped devices. E.g. the profile representing unknown Windows 10 tablets will return true. Apple devices detected through JavascriptHardwareProfile that do not uniquely identify a device will also return true, and HardwareModelVariants will return a list of model numbers associated with that device group.
		/// </summary>
		public IAspectPropertyValue<bool> IsHardwareGroup { get { return GetAs<IAspectPropertyValue<bool>>("IsHardwareGroup"); } }
		/// <summary>
		/// Indicates the internal persistent storage (ROM capacity) options the device can be supplied with in gigabytes (GB), including the device's Operating System and bundled applications. This could also be referred to as "Electrically Erasable Programmable Read-Only Memory (EEPROM)" or "Non Volatile Random Access Memory (NVRAM)". If no variants are found, then the value returned will be the same as "MaxInternalStorage".
		/// </summary>
		public IAspectPropertyValue<IReadOnlyList<string>> InternalStorageVariants { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("InternalStorageVariants"); } }
		/// <summary>
		/// Indicates the level of support for the Promise object. The Promise object represents the eventual completion (or failure) of an asynchronous operation, and its resulting value.
		/// </summary>
		public IAspectPropertyValue<string> Promise { get { return GetAs<IAspectPropertyValue<string>>("Promise"); } }
		/// <summary>
		/// Indicates the source of the profile's specifications. This property will return 'Manufacturer' value if the profile data was obtained from the manufacturer of the device or the device itself. This property will return 'Authoritative' value if the profile data was not obtained from the manufacturer or the device itself but other third party sources (this may include retailers, social media, carriers, etc). This property will return 'Legacy' value if the profile data was obtained prior to 51degrees differentiating between Manufacturer and Authoritative. This property will return 'N/A' value if the profile data was not obtained due to unidentifiable User-Agent. The example profiles are: Generic Android Unknown, Unknown Tablet, etc.
		/// </summary>
		public IAspectPropertyValue<string> HardwareProfileSource { get { return GetAs<IAspectPropertyValue<string>>("HardwareProfileSource"); } }
		/// <summary>
		/// Indicates the volatile RAM capacity options for the device in megabytes (MB). If no variants are found, then the value returned will be the same as "DeviceRAM".
		/// </summary>
		public IAspectPropertyValue<IReadOnlyList<string>> DeviceRAMVariants { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("DeviceRAMVariants"); } }
		/// <summary>
		/// Indicates the list of frequency bands supported by the device.
		/// </summary>
		public IAspectPropertyValue<IReadOnlyList<string>> FrequencyBands { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("FrequencyBands"); } }
		/// <summary>
		/// Indicates if a web page is accessed through a VR headset.
		/// </summary>
		public IAspectPropertyValue<bool> InVRMode { get { return GetAs<IAspectPropertyValue<bool>>("InVRMode"); } }
		/// <summary>
		/// Indicates whether the device screen is foldable or not. If the device does not have a screen or the screen is not foldable, 'False' is returned.
		/// </summary>
		public IAspectPropertyValue<bool> IsScreenFoldable { get { return GetAs<IAspectPropertyValue<bool>>("IsScreenFoldable"); } }
		/// <summary>
		/// Indicates the diagonal size of the device's second screen in inches. This property is not applicable for a device that does not have a second screen.
		/// </summary>
		public IAspectPropertyValue<double> SecondScreenInchesDiagonal { get { return GetAs<IAspectPropertyValue<double>>("SecondScreenInchesDiagonal"); } }
		/// <summary>
		/// Refers to the width of the device's second screen in inches. This property will return the value 'N/A' for desktop or for devices which do not have a second screen.
		/// </summary>
		public IAspectPropertyValue<double> SecondScreenInchesWidth { get { return GetAs<IAspectPropertyValue<double>>("SecondScreenInchesWidth"); } }
		/// <summary>
		/// Indicates the width of the device's second screen in pixels. This property is not applicable for a device that does not have a second screen.
		/// </summary>
		public IAspectPropertyValue<int> SecondScreenPixelsWidth { get { return GetAs<IAspectPropertyValue<int>>("SecondScreenPixelsWidth"); } }
		/// <summary>
		/// Indicates the diagonal size of the device's second screen in inches rounded to the nearest whole number. This property will return the value 'N/A' for desktop or for devices which do not have a second screen.
		/// </summary>
		public IAspectPropertyValue<int> SecondScreenInchesDiagonalRounded { get { return GetAs<IAspectPropertyValue<int>>("SecondScreenInchesDiagonalRounded"); } }
		/// <summary>
		/// Indicates the area of the device's second screen in square inches rounded to the nearest whole number. This property will return the value 'N/A' for desktop or for devices which do not have a second screen.
		/// </summary>
		public IAspectPropertyValue<int> SecondScreenInchesSquare { get { return GetAs<IAspectPropertyValue<int>>("SecondScreenInchesSquare"); } }
		/// <summary>
		/// Refers to the height of the device's second screen in inches. This property will return 'N/A' for desktops or for devices which do not have a second screen.
		/// </summary>
		public IAspectPropertyValue<double> SecondScreenInchesHeight { get { return GetAs<IAspectPropertyValue<double>>("SecondScreenInchesHeight"); } }
		/// <summary>
		/// Refers to the diagonal size of the second screen of the device in millimetres. This property will return 'N/A' for desktops or for devices which do not have a second screen.
		/// </summary>
		public IAspectPropertyValue<double> SecondScreenMMDiagonal { get { return GetAs<IAspectPropertyValue<double>>("SecondScreenMMDiagonal"); } }
		/// <summary>
		/// Indicate the diagonal size of the device's second screen in millimetres rounded to the nearest whole number. This property will return the value 'N/A' for desktop or for devices which do not have a second screen.
		/// </summary>
		public IAspectPropertyValue<int> SecondScreenMMDiagonalRounded { get { return GetAs<IAspectPropertyValue<int>>("SecondScreenMMDiagonalRounded"); } }
		/// <summary>
		/// Refers to the second screen height of the device in millimetres. This property will return 'N/A' for desktops or for devices which do not have a second screen.
		/// </summary>
		public IAspectPropertyValue<double> SecondScreenMMHeight { get { return GetAs<IAspectPropertyValue<double>>("SecondScreenMMHeight"); } }
		/// <summary>
		/// Indicates the area of the device's second screen in square millimetres rounded to the nearest whole number. This property will return the value  'N/A' for desktop or for devices which do not have a second screen.
		/// </summary>
		public IAspectPropertyValue<int> SecondScreenMMSquare { get { return GetAs<IAspectPropertyValue<int>>("SecondScreenMMSquare"); } }
		/// <summary>
		/// Refers to the second screen width of the device in millimetres. This property will return 'N/A' for desktops or for devices which do not have a second screen.
		/// </summary>
		public IAspectPropertyValue<double> SecondScreenMMWidth { get { return GetAs<IAspectPropertyValue<double>>("SecondScreenMMWidth"); } }
		/// <summary>
		/// Indicates the height of the device's second screen in pixels. This property is not applicable for a device that does not have a second screen.
		/// </summary>
		public IAspectPropertyValue<int> SecondScreenPixelsHeight { get { return GetAs<IAspectPropertyValue<int>>("SecondScreenPixelsHeight"); } }
		/// <summary>
		/// Indicates the Type Allocation Code (TAC) for devices supporting GSM/3GPP networks which come from multiple sources. This property will return 'N/A' if we cannot determine the device TAC authenticy.
		/// </summary>
		public IAspectPropertyValue<IReadOnlyList<string>> TAC { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("TAC"); } }
		/// <summary>
		/// Indicates if the browser supports all CSS grid properties.
		/// </summary>
		public IAspectPropertyValue<bool> CssGrid { get { return GetAs<IAspectPropertyValue<bool>>("CssGrid"); } }
		/// <summary>
		/// Indicates if the browser supports the Fetch API.
		/// </summary>
		public IAspectPropertyValue<bool> Fetch { get { return GetAs<IAspectPropertyValue<bool>>("Fetch"); } }
		/// <summary>
		/// Indicates if the browser supports the WebP image format.
		/// </summary>
		public IAspectPropertyValue<bool> WebP { get { return GetAs<IAspectPropertyValue<bool>>("WebP"); } }
		/// <summary>
		/// Indicates the number of screens the device has. This property is not applicable for a device that does not have a screen.
		/// </summary>
		public IAspectPropertyValue<int> NumberOfScreens { get { return GetAs<IAspectPropertyValue<int>>("NumberOfScreens"); } }
		/// <summary>
		/// Indicates if the browser supports HTTP version 2.
		/// </summary>
		public IAspectPropertyValue<bool> Http2 { get { return GetAs<IAspectPropertyValue<bool>>("Http2"); } }
		/// <summary>
		/// Indicates if the browser can prefetch resources without executing them.
		/// </summary>
		public IAspectPropertyValue<bool> Preload { get { return GetAs<IAspectPropertyValue<bool>>("Preload"); } }
		/// <summary>
		/// Indicates the browser supports JPEG 2000 image format.
		/// </summary>
		public IAspectPropertyValue<bool> Jpeg2000 { get { return GetAs<IAspectPropertyValue<bool>>("Jpeg2000"); } }
		/// <summary>
		/// Indicates the name of the browser without the default OS or layout engine.
		/// </summary>
		public IAspectPropertyValue<string> BrowserFamily { get { return GetAs<IAspectPropertyValue<string>>("BrowserFamily"); } }
		/// <summary>
		/// The ratio of the resolution in physical pixels to the resolution in CSS pixels. This is approximated by screen resolution and screen size when the value is not known.
		/// </summary>
		public IAspectPropertyValue<double> PixelRatio { get { return GetAs<IAspectPropertyValue<double>>("PixelRatio"); } }
		/// <summary>
		/// JavaScript that can override the property value found by the server using information on the client device. This property is applicable for browsers that support pixel ratio cookie.
		/// </summary>
		public IAspectPropertyValue<JavaScript> PixelRatioJavascript { get { return GetAs<IAspectPropertyValue<JavaScript>>("PixelRatioJavascript"); } }
		/// <summary>
		/// Contains the Accept-CH HTTP header values to add to the HTTP response for the browser component. UACH values Sec-CH-UA, and Sec-CH-UA-Full-Version are relevant. The default value is Unknown if the browser does not fully support UACH.
		/// </summary>
		public IAspectPropertyValue<string> SetHeaderBrowserAcceptCH { get { return GetAs<IAspectPropertyValue<string>>("SetHeaderBrowserAccept-CH"); } }
		/// <summary>
		/// Contains the Accept-CH HTTP header values to add to the HTTP response for the hardware component. UACH values Sec-CH-UA-Model, and Sec-CH-UA-Mobile are relevant. The default value is Unknown if the browser does not fully support UACH.
		/// </summary>
		public IAspectPropertyValue<string> SetHeaderHardwareAcceptCH { get { return GetAs<IAspectPropertyValue<string>>("SetHeaderHardwareAccept-CH"); } }
		/// <summary>
		/// Contains the Accept-CH HTTP header values to add to the HTTP response for the platform component. UACH values Sec-CH-UA-Platform, and Sec-CH-UA-Platform-Version are relevant. The default value is Unknown if the browser does not fully support UACH.
		/// </summary>
		public IAspectPropertyValue<string> SetHeaderPlatformAcceptCH { get { return GetAs<IAspectPropertyValue<string>>("SetHeaderPlatformAccept-CH"); } }
		/// <summary>
		/// Contains Javascript to get high entropy values.
		/// </summary>
		public IAspectPropertyValue<JavaScript> JavascriptGetHighEntropyValues { get { return GetAs<IAspectPropertyValue<JavaScript>>("JavascriptGetHighEntropyValues"); } }
		/// <summary>
		/// JavaScript that checks for browser specific features and overrides the ProfileID.
		/// </summary>
		public IAspectPropertyValue<JavaScript> JavaScriptBrowserOverride { get { return GetAs<IAspectPropertyValue<JavaScript>>("JavaScriptBrowserOverride"); } }
		/// <summary>
		/// Name of the underlying browser source project.
		/// </summary>
		public IAspectPropertyValue<string> BrowserSourceProject { get { return GetAs<IAspectPropertyValue<string>>("BrowserSourceProject"); } }
		/// <summary>
		/// Indicates the version or subversion of the underlying browser source project.
		/// </summary>
		public IAspectPropertyValue<string> BrowserSourceProjectVersion { get { return GetAs<IAspectPropertyValue<string>>("BrowserSourceProjectVersion"); } }
		/// <summary>
		/// Refers to the experimental Privacy Sandbox Topics API proposal from Google. Indicates if the API caller has observed one or more topics for a user and checks whether the website has not blocked the Topics API using a Permissions Policy.
		/// </summary>
		public IAspectPropertyValue<string> TopicsAPIEnabled { get { return GetAs<IAspectPropertyValue<string>>("TopicsAPIEnabled"); } }
		/// <summary>
		/// Refers to the experimental Privacy Sandbox Shared Storage API proposal from Google. Indicates whether the API caller can access "Shared Storage" and checks whether the website has not blocked the Shared Storage API using a Permissions Policy.
		/// </summary>
		public IAspectPropertyValue<string> SharedStorageAPIEnabled { get { return GetAs<IAspectPropertyValue<string>>("SharedStorageAPIEnabled"); } }
		/// <summary>
		/// Refers to the experimental Privacy Sandbox Protected Audience API proposal from Google. Indicates whether the API caller can register an "AdInterestGroup" and checks whether the website has not blocked the Protected Audience API using a Permissions Policy. Please be aware we have observed latency issues when interacting with the API.
		/// </summary>
		public IAspectPropertyValue<string> ProtectedAudienceAPIEnabled { get { return GetAs<IAspectPropertyValue<string>>("ProtectedAudienceAPIEnabled"); } }
		/// <summary>
		/// A list of logos associated with the Browser. The string contains the caption, followed by the full image URL separated with a tab character.
		/// </summary>
		public IAspectPropertyValue<IReadOnlyList<string>> BrowserLogos { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("BrowserLogos"); } }
		/// <summary>
		/// A list of logos associated with the Software. The string contains the caption, followed by the full image URL separated with a tab character.
		/// </summary>
		public IAspectPropertyValue<IReadOnlyList<string>> PlatformLogos { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("PlatformLogos"); } }
		/// <summary>
		/// Indicates if the browser supports the experimental Privacy Sandbox API proposals from Google.
		/// </summary>
		public IAspectPropertyValue<string> BrowserSupportsPrivacySandbox { get { return GetAs<IAspectPropertyValue<string>>("BrowserSupportsPrivacySandbox"); } }
		/// <summary>
		/// JavaScript that overrides the property value for the TopicsAPIEnabled property.
		/// </summary>
		public IAspectPropertyValue<JavaScript> TopicsAPIEnabledJavaScript { get { return GetAs<IAspectPropertyValue<JavaScript>>("TopicsAPIEnabledJavaScript"); } }
		/// <summary>
		/// JavaScript that overrides the property value for the SharedStorageAPIEnabled property.
		/// </summary>
		public IAspectPropertyValue<JavaScript> SharedStorageAPIEnabledJavaScript { get { return GetAs<IAspectPropertyValue<JavaScript>>("SharedStorageAPIEnabledJavaScript"); } }
		/// <summary>
		/// JavaScript that overrides the property value for the ProtectedAudienceAPIEnabled property.
		/// </summary>
		public IAspectPropertyValue<JavaScript> ProtectedAudienceAPIEnabledJavaScript { get { return GetAs<IAspectPropertyValue<JavaScript>>("ProtectedAudienceAPIEnabledJavaScript"); } }
		/// <summary>
		/// Indicates the height of the device's screen in physical pixels. This property is not applicable for a device that does not have a screen. For devices such as tablets or TV which are predominantly used in landscape mode, the pixel height will be the smaller value compared to the pixel width. 
		/// </summary>
		public IAspectPropertyValue<int> ScreenPixelsPhysicalHeight { get { return GetAs<IAspectPropertyValue<int>>("ScreenPixelsPhysicalHeight"); } }
		/// <summary>
		/// Indicates the width of the device's screen in physical pixels. This property is not applicable for a device that does not have a screen. For devices such as tablets or TV which are predominantly used in landscape mode, the pixel width will be the larger value compared to the pixel height.
		/// </summary>
		public IAspectPropertyValue<int> ScreenPixelsPhysicalWidth { get { return GetAs<IAspectPropertyValue<int>>("ScreenPixelsPhysicalWidth"); } }
		/// <summary>
		/// Indicates the highest version of Bluetooth the device supports.
		/// </summary>
		public IAspectPropertyValue<double> SupportedBluetooth { get { return GetAs<IAspectPropertyValue<double>>("SupportedBluetooth"); } }
		/// <summary>
		/// Indicates the Bluetooth profiles the device supports.
		/// </summary>
		public IAspectPropertyValue<IReadOnlyList<string>> SupportedBluetoothProfiles { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("SupportedBluetoothProfiles"); } }
		/// <summary>
		/// Indicates whether the crawler is confirmed by the crawler controller to be used to train artificial intelligence.
		/// </summary>
		public IAspectPropertyValue<string> IsArtificialIntelligence { get { return GetAs<IAspectPropertyValue<string>>("IsArtificialIntelligence"); } }
		/// <summary>
		/// Indicates the number of hash nodes matched within the evidence.
		/// </summary>
		public IAspectPropertyValue<int> MatchedNodes { get { return GetAs<IAspectPropertyValue<int>>("MatchedNodes"); } }
		/// <summary>
		/// Used when detection method is not Exact or None. This is an integer value and the larger the value the less confident the detector is in this result.
		/// </summary>
		public IAspectPropertyValue<int> Difference { get { return GetAs<IAspectPropertyValue<int>>("Difference"); } }
		/// <summary>
		/// Total difference in character positions where the substrings hashes were found away from where they were expected.
		/// </summary>
		public IAspectPropertyValue<int> Drift { get { return GetAs<IAspectPropertyValue<int>>("Drift"); } }
		/// <summary>
		/// Consists of four components separated by a hyphen symbol: Hardware-Platform-Browser-IsCrawler where each Component represents an ID of the corresponding Profile.
		/// </summary>
		public IAspectPropertyValue<string> DeviceId { get { return GetAs<IAspectPropertyValue<string>>("DeviceId"); } }
		/// <summary>
		/// The matched User-Agents.
		/// </summary>
		public IAspectPropertyValue<IReadOnlyList<string>> UserAgents { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("UserAgents"); } }
		/// <summary>
		/// The number of iterations carried out in order to find a match. This is the number of nodes in the graph which have been visited.
		/// </summary>
		public IAspectPropertyValue<int> Iterations { get { return GetAs<IAspectPropertyValue<int>>("Iterations"); } }
		/// <summary>
		/// The method used to determine the match result.
		/// </summary>
		public IAspectPropertyValue<string> Method { get { return GetAs<IAspectPropertyValue<string>>("Method"); } }
	}
}
