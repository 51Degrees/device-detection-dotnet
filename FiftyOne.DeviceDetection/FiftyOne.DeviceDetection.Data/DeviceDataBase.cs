/* *********************************************************************
 * This Original Work is copyright of 51 Degrees Mobile Experts Limited.
 * Copyright 2019 51 Degrees Mobile Experts Limited, 5 Charlotte Close,
 * Caversham, Reading, Berkshire, United Kingdom RG4 7BY.
 *
 * This Original Work is licensed under the European Union Public Licence (EUPL) 
 * v.1.2 and is subject to its terms as set out below.
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

using FiftyOne.DeviceDetection.Shared.Data;
using FiftyOne.Pipeline.Core.Data;
using FiftyOne.Pipeline.Core.Data.Types;
using FiftyOne.Pipeline.Engines.Data;
using FiftyOne.Pipeline.Engines.FlowElements;
using FiftyOne.Pipeline.Engines.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
namespace FiftyOne.DeviceDetection.Shared
{
    public abstract class DeviceDataBase : AspectDataBase, IDeviceData
    {
        protected DeviceDataBase(
            ILogger<AspectDataBase> logger,
            IFlowData flowData,
            IAspectEngine engine,
            IMissingPropertyService missingPropertyService)
            : base(logger, flowData, engine, missingPropertyService) { }

        public virtual IAspectPropertyValue<IReadOnlyList<string>> UserAgents { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("UserAgents"); } }
        public virtual IAspectPropertyValue<string> DeviceId { get { return GetAs<IAspectPropertyValue<string>>("DeviceId"); } }
        public IAspectPropertyValue<double> BackCameraMegaPixels { get { return GetAs<IAspectPropertyValue<double>>("BackCameraMegaPixels"); } }
        public IAspectPropertyValue<int> BatteryCapacity { get { return GetAs<IAspectPropertyValue<int>>("BatteryCapacity"); } }
        public IAspectPropertyValue<int> BitsPerPixel { get { return GetAs<IAspectPropertyValue<int>>("BitsPerPixel"); } }
        public IAspectPropertyValue<IReadOnlyList<string>> CameraTypes { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("CameraTypes"); } }
        public IAspectPropertyValue<string> ContrastRatio { get { return GetAs<IAspectPropertyValue<string>>("ContrastRatio"); } }
        public IAspectPropertyValue<string> CPU { get { return GetAs<IAspectPropertyValue<string>>("CPU"); } }
        public IAspectPropertyValue<int> CPUCores { get { return GetAs<IAspectPropertyValue<int>>("CPUCores"); } }
        public IAspectPropertyValue<string> CPUDesigner { get { return GetAs<IAspectPropertyValue<string>>("CPUDesigner"); } }
        public IAspectPropertyValue<double> CPUMaximumFrequency { get { return GetAs<IAspectPropertyValue<double>>("CPUMaximumFrequency"); } }
        public IAspectPropertyValue<IReadOnlyList<string>> DeviceCertifications { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("DeviceCertifications"); } }
        public IAspectPropertyValue<int> DeviceRAM { get { return GetAs<IAspectPropertyValue<int>>("DeviceRAM"); } }
        public IAspectPropertyValue<IReadOnlyList<string>> DeviceRAMVariants { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("DeviceRAMVariants"); } }
        public IAspectPropertyValue<string> DeviceType { get { return GetAs<IAspectPropertyValue<string>>("DeviceType"); } }
        public IAspectPropertyValue<IReadOnlyList<string>> Durability { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("Durability"); } }
        public IAspectPropertyValue<string> DynamicContrastRatio { get { return GetAs<IAspectPropertyValue<string>>("DynamicContrastRatio"); } }
        public IAspectPropertyValue<int> EnergyConsumptionPerYear { get { return GetAs<IAspectPropertyValue<int>>("EnergyConsumptionPerYear"); } }
        public IAspectPropertyValue<int> ExpansionSlotMaxSize { get { return GetAs<IAspectPropertyValue<int>>("ExpansionSlotMaxSize"); } }
        public IAspectPropertyValue<IReadOnlyList<string>> ExpansionSlotType { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("ExpansionSlotType"); } }
        public IAspectPropertyValue<double> FrontCameraMegaPixels { get { return GetAs<IAspectPropertyValue<double>>("FrontCameraMegaPixels"); } }
        public IAspectPropertyValue<string> GPU { get { return GetAs<IAspectPropertyValue<string>>("GPU"); } }
        public IAspectPropertyValue<string> GPUDesigner { get { return GetAs<IAspectPropertyValue<string>>("GPUDesigner"); } }
        public IAspectPropertyValue<IReadOnlyList<string>> HardwareAudioCodecsDecode { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("HardwareAudioCodecsDecode"); } }
        public IAspectPropertyValue<IReadOnlyList<string>> HardwareAudioCodecsEncode { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("HardwareAudioCodecsEncode"); } }
        public IAspectPropertyValue<string> HardwareCarrier { get { return GetAs<IAspectPropertyValue<string>>("HardwareCarrier"); } }
        public IAspectPropertyValue<string> HardwareFamily { get { return GetAs<IAspectPropertyValue<string>>("HardwareFamily"); } }
        public IAspectPropertyValue<IReadOnlyList<string>> HardwareImages { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("HardwareImages"); } }
        public IAspectPropertyValue<string> HardwareModel { get { return GetAs<IAspectPropertyValue<string>>("HardwareModel"); } }
        public IAspectPropertyValue<IReadOnlyList<string>> HardwareModelVariants { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("HardwareModelVariants"); } }
        public IAspectPropertyValue<IReadOnlyList<string>> HardwareName { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("HardwareName"); } }
        public IAspectPropertyValue<string> HardwareProfileSource { get { return GetAs<IAspectPropertyValue<string>>("HardwareProfileSource"); } }
        public IAspectPropertyValue<int> HardwareRank { get { return GetAs<IAspectPropertyValue<int>>("HardwareRank"); } }
        public IAspectPropertyValue<string> HardwareVendor { get { return GetAs<IAspectPropertyValue<string>>("HardwareVendor"); } }
        public IAspectPropertyValue<IReadOnlyList<string>> HardwareVideoCodecsDecode { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("HardwareVideoCodecsDecode"); } }
        public IAspectPropertyValue<IReadOnlyList<string>> HardwareVideoCodecsEncode { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("HardwareVideoCodecsEncode"); } }
        public IAspectPropertyValue<bool> Has3DCamera { get { return GetAs<IAspectPropertyValue<bool>>("Has3DCamera"); } }
        public IAspectPropertyValue<bool> Has3DScreen { get { return GetAs<IAspectPropertyValue<bool>>("Has3DScreen"); } }
        public IAspectPropertyValue<bool> HasCamera { get { return GetAs<IAspectPropertyValue<bool>>("HasCamera"); } }
        public IAspectPropertyValue<bool> HasClickWheel { get { return GetAs<IAspectPropertyValue<bool>>("HasClickWheel"); } }
        public IAspectPropertyValue<bool> HasKeypad { get { return GetAs<IAspectPropertyValue<bool>>("HasKeypad"); } }
        public IAspectPropertyValue<bool> HasNFC { get { return GetAs<IAspectPropertyValue<bool>>("HasNFC"); } }
        public IAspectPropertyValue<bool> HasQwertyPad { get { return GetAs<IAspectPropertyValue<bool>>("HasQwertyPad"); } }
        public IAspectPropertyValue<bool> HasRemovableBattery { get { return GetAs<IAspectPropertyValue<bool>>("HasRemovableBattery"); } }
        public IAspectPropertyValue<bool> HasTouchScreen { get { return GetAs<IAspectPropertyValue<bool>>("HasTouchScreen"); } }
        public IAspectPropertyValue<bool> HasTrackPad { get { return GetAs<IAspectPropertyValue<bool>>("HasTrackPad"); } }
        public IAspectPropertyValue<bool> HasVirtualQwerty { get { return GetAs<IAspectPropertyValue<bool>>("HasVirtualQwerty"); } }
        public IAspectPropertyValue<IReadOnlyList<string>> InternalStorageVariants { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("InternalStorageVariants"); } }
        public IAspectPropertyValue<bool> IsConsole { get { return GetAs<IAspectPropertyValue<bool>>("IsConsole"); } }
        public IAspectPropertyValue<bool> IsEReader { get { return GetAs<IAspectPropertyValue<bool>>("IsEReader"); } }
        public IAspectPropertyValue<bool> IsHardwareGroup { get { return GetAs<IAspectPropertyValue<bool>>("IsHardwareGroup"); } }
        public IAspectPropertyValue<bool> IsMediaHub { get { return GetAs<IAspectPropertyValue<bool>>("IsMediaHub"); } }
        public IAspectPropertyValue<bool> IsMobile { get { return GetAs<IAspectPropertyValue<bool>>("IsMobile"); } }
        public IAspectPropertyValue<bool> IsSmallScreen { get { return GetAs<IAspectPropertyValue<bool>>("IsSmallScreen"); } }
        public IAspectPropertyValue<bool> IsSmartPhone { get { return GetAs<IAspectPropertyValue<bool>>("IsSmartPhone"); } }
        public IAspectPropertyValue<bool> IsSmartWatch { get { return GetAs<IAspectPropertyValue<bool>>("IsSmartWatch"); } }
        public IAspectPropertyValue<bool> IsTablet { get { return GetAs<IAspectPropertyValue<bool>>("IsTablet"); } }
        public IAspectPropertyValue<bool> IsTv { get { return GetAs<IAspectPropertyValue<bool>>("IsTv"); } }
        public IAspectPropertyValue<JavaScript> JavascriptHardwareProfile { get { return GetAs<IAspectPropertyValue<JavaScript>>("JavascriptHardwareProfile"); } }
        public IAspectPropertyValue<double> MaxInternalStorage { get { return GetAs<IAspectPropertyValue<double>>("MaxInternalStorage"); } }
        public IAspectPropertyValue<int> MaxNumberOfSIMCards { get { return GetAs<IAspectPropertyValue<int>>("MaxNumberOfSIMCards"); } }
        public IAspectPropertyValue<int> MaxStandbyTime { get { return GetAs<IAspectPropertyValue<int>>("MaxStandbyTime"); } }
        public IAspectPropertyValue<int> MaxTalkTime { get { return GetAs<IAspectPropertyValue<int>>("MaxTalkTime"); } }
        public IAspectPropertyValue<int> MaxUsageTime { get { return GetAs<IAspectPropertyValue<int>>("MaxUsageTime"); } }
        public IAspectPropertyValue<IReadOnlyList<string>> NativeBrand { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("NativeBrand"); } }
        public IAspectPropertyValue<IReadOnlyList<string>> NativeDevice { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("NativeDevice"); } }
        public IAspectPropertyValue<IReadOnlyList<string>> NativeModel { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("NativeModel"); } }
        public IAspectPropertyValue<IReadOnlyList<string>> NativeName { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("NativeName"); } }
        public IAspectPropertyValue<string> NativePlatform { get { return GetAs<IAspectPropertyValue<string>>("NativePlatform"); } }
        public IAspectPropertyValue<string> OEM { get { return GetAs<IAspectPropertyValue<string>>("OEM"); } }
        public IAspectPropertyValue<int> OnPowerConsumption { get { return GetAs<IAspectPropertyValue<int>>("OnPowerConsumption"); } }
        public IAspectPropertyValue<string> Popularity { get { return GetAs<IAspectPropertyValue<string>>("Popularity"); } }
        public IAspectPropertyValue<string> PriceBand { get { return GetAs<IAspectPropertyValue<string>>("PriceBand"); } }
        public IAspectPropertyValue<int> RefreshRate { get { return GetAs<IAspectPropertyValue<int>>("RefreshRate"); } }
        public IAspectPropertyValue<int> ReleaseAge { get { return GetAs<IAspectPropertyValue<int>>("ReleaseAge"); } }
        public IAspectPropertyValue<string> ReleaseMonth { get { return GetAs<IAspectPropertyValue<string>>("ReleaseMonth"); } }
        public IAspectPropertyValue<int> ReleaseYear { get { return GetAs<IAspectPropertyValue<int>>("ReleaseYear"); } }
        public IAspectPropertyValue<IReadOnlyList<string>> SatelliteNavigationTypes { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("SatelliteNavigationTypes"); } }
        public IAspectPropertyValue<double> ScreenInchesDiagonal { get { return GetAs<IAspectPropertyValue<double>>("ScreenInchesDiagonal"); } }
        public IAspectPropertyValue<int> ScreenInchesDiagonalRounded { get { return GetAs<IAspectPropertyValue<int>>("ScreenInchesDiagonalRounded"); } }
        public IAspectPropertyValue<double> ScreenInchesHeight { get { return GetAs<IAspectPropertyValue<double>>("ScreenInchesHeight"); } }
        public IAspectPropertyValue<int> ScreenInchesSquare { get { return GetAs<IAspectPropertyValue<int>>("ScreenInchesSquare"); } }
        public IAspectPropertyValue<double> ScreenInchesWidth { get { return GetAs<IAspectPropertyValue<double>>("ScreenInchesWidth"); } }
        public IAspectPropertyValue<double> ScreenMMDiagonal { get { return GetAs<IAspectPropertyValue<double>>("ScreenMMDiagonal"); } }
        public IAspectPropertyValue<int> ScreenMMDiagonalRounded { get { return GetAs<IAspectPropertyValue<int>>("ScreenMMDiagonalRounded"); } }
        public IAspectPropertyValue<double> ScreenMMHeight { get { return GetAs<IAspectPropertyValue<double>>("ScreenMMHeight"); } }
        public IAspectPropertyValue<int> ScreenMMSquare { get { return GetAs<IAspectPropertyValue<int>>("ScreenMMSquare"); } }
        public IAspectPropertyValue<double> ScreenMMWidth { get { return GetAs<IAspectPropertyValue<double>>("ScreenMMWidth"); } }
        public IAspectPropertyValue<int> ScreenPixelsHeight { get { return GetAs<IAspectPropertyValue<int>>("ScreenPixelsHeight"); } }
        public IAspectPropertyValue<int> ScreenPixelsWidth { get { return GetAs<IAspectPropertyValue<int>>("ScreenPixelsWidth"); } }
        public IAspectPropertyValue<string> ScreenType { get { return GetAs<IAspectPropertyValue<string>>("ScreenType"); } }
        public IAspectPropertyValue<string> SecondBackCameraMegaPixels { get { return GetAs<IAspectPropertyValue<string>>("SecondBackCameraMegaPixels"); } }
        public IAspectPropertyValue<string> SecondFrontCameraMegaPixels { get { return GetAs<IAspectPropertyValue<string>>("SecondFrontCameraMegaPixels"); } }
        public IAspectPropertyValue<string> SoC { get { return GetAs<IAspectPropertyValue<string>>("SoC"); } }
        public IAspectPropertyValue<string> SoCDesigner { get { return GetAs<IAspectPropertyValue<string>>("SoCDesigner"); } }
        public IAspectPropertyValue<string> SoCModel { get { return GetAs<IAspectPropertyValue<string>>("SoCModel"); } }
        public IAspectPropertyValue<string> SpecificAbsorbtionRateEU { get { return GetAs<IAspectPropertyValue<string>>("SpecificAbsorbtionRateEU"); } }
        public IAspectPropertyValue<int> SpecificAbsorbtionRateUS { get { return GetAs<IAspectPropertyValue<int>>("SpecificAbsorbtionRateUS"); } }
        public IAspectPropertyValue<double> SuggestedImageButtonHeightMms { get { return GetAs<IAspectPropertyValue<double>>("SuggestedImageButtonHeightMms"); } }
        public IAspectPropertyValue<double> SuggestedImageButtonHeightPixels { get { return GetAs<IAspectPropertyValue<double>>("SuggestedImageButtonHeightPixels"); } }
        public IAspectPropertyValue<double> SuggestedLinkSizePixels { get { return GetAs<IAspectPropertyValue<double>>("SuggestedLinkSizePixels"); } }
        public IAspectPropertyValue<double> SuggestedLinkSizePoints { get { return GetAs<IAspectPropertyValue<double>>("SuggestedLinkSizePoints"); } }
        public IAspectPropertyValue<IReadOnlyList<string>> SupportedBearers { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("SupportedBearers"); } }
        public IAspectPropertyValue<string> SupportedBluetoothVersion { get { return GetAs<IAspectPropertyValue<string>>("SupportedBluetoothVersion"); } }
        public IAspectPropertyValue<IReadOnlyList<string>> SupportedCameraFeatures { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("SupportedCameraFeatures"); } }
        public IAspectPropertyValue<IReadOnlyList<string>> SupportedChargerTypes { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("SupportedChargerTypes"); } }
        public IAspectPropertyValue<IReadOnlyList<string>> SupportedIO { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("SupportedI/O"); } }
        public IAspectPropertyValue<IReadOnlyList<string>> SupportedSensorTypes { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("SupportedSensorTypes"); } }
        public IAspectPropertyValue<IReadOnlyList<string>> SupportedSIMCardTypes { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("SupportedSIMCardTypes"); } }
        public IAspectPropertyValue<bool> Supports24p { get { return GetAs<IAspectPropertyValue<bool>>("Supports24p"); } }
        public IAspectPropertyValue<bool> SupportsPhoneCalls { get { return GetAs<IAspectPropertyValue<bool>>("SupportsPhoneCalls"); } }
        public IAspectPropertyValue<bool> SupportsWiDi { get { return GetAs<IAspectPropertyValue<bool>>("SupportsWiDi"); } }
        public IAspectPropertyValue<string> WeightWithBattery { get { return GetAs<IAspectPropertyValue<string>>("WeightWithBattery"); } }
        public IAspectPropertyValue<string> WeightWithoutBattery { get { return GetAs<IAspectPropertyValue<string>>("WeightWithoutBattery"); } }
        public IAspectPropertyValue<IReadOnlyList<string>> CcppAccept { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("CcppAccept"); } }
        public IAspectPropertyValue<double> CLDC { get { return GetAs<IAspectPropertyValue<double>>("CLDC"); } }
        public IAspectPropertyValue<double> MIDP { get { return GetAs<IAspectPropertyValue<double>>("MIDP"); } }
        public IAspectPropertyValue<int> PlatformDiscontinuedAge { get { return GetAs<IAspectPropertyValue<int>>("PlatformDiscontinuedAge"); } }
        public IAspectPropertyValue<string> PlatformDiscontinuedMonth { get { return GetAs<IAspectPropertyValue<string>>("PlatformDiscontinuedMonth"); } }
        public IAspectPropertyValue<string> PlatformDiscontinuedYear { get { return GetAs<IAspectPropertyValue<string>>("PlatformDiscontinuedYear"); } }
        public IAspectPropertyValue<string> PlatformName { get { return GetAs<IAspectPropertyValue<string>>("PlatformName"); } }
        public IAspectPropertyValue<int> PlatformPreviewAge { get { return GetAs<IAspectPropertyValue<int>>("PlatformPreviewAge"); } }
        public IAspectPropertyValue<string> PlatformPreviewMonth { get { return GetAs<IAspectPropertyValue<string>>("PlatformPreviewMonth"); } }
        public IAspectPropertyValue<string> PlatformPreviewYear { get { return GetAs<IAspectPropertyValue<string>>("PlatformPreviewYear"); } }
        public IAspectPropertyValue<int> PlatformRank { get { return GetAs<IAspectPropertyValue<int>>("PlatformRank"); } }
        public IAspectPropertyValue<int> PlatformReleaseAge { get { return GetAs<IAspectPropertyValue<int>>("PlatformReleaseAge"); } }
        public IAspectPropertyValue<string> PlatformReleaseMonth { get { return GetAs<IAspectPropertyValue<string>>("PlatformReleaseMonth"); } }
        public IAspectPropertyValue<string> PlatformReleaseYear { get { return GetAs<IAspectPropertyValue<string>>("PlatformReleaseYear"); } }
        public IAspectPropertyValue<string> PlatformVendor { get { return GetAs<IAspectPropertyValue<string>>("PlatformVendor"); } }
        public IAspectPropertyValue<string> PlatformVersion { get { return GetAs<IAspectPropertyValue<string>>("PlatformVersion"); } }
        public IAspectPropertyValue<IReadOnlyList<string>> SoftwareAudioCodecsDecode { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("SoftwareAudioCodecsDecode"); } }
        public IAspectPropertyValue<IReadOnlyList<string>> SoftwareAudioCodecsEncode { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("SoftwareAudioCodecsEncode"); } }
        public IAspectPropertyValue<IReadOnlyList<string>> SoftwareVideoCodecsDecode { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("SoftwareVideoCodecsDecode"); } }
        public IAspectPropertyValue<IReadOnlyList<string>> SoftwareVideoCodecsEncode { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("SoftwareVideoCodecsEncode"); } }
        public IAspectPropertyValue<IReadOnlyList<string>> StreamingAccept { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("StreamingAccept"); } }
        public IAspectPropertyValue<string> AjaxRequestType { get { return GetAs<IAspectPropertyValue<string>>("AjaxRequestType"); } }
        public IAspectPropertyValue<bool> AnimationTiming { get { return GetAs<IAspectPropertyValue<bool>>("AnimationTiming"); } }
        public IAspectPropertyValue<bool> BlobBuilder { get { return GetAs<IAspectPropertyValue<bool>>("BlobBuilder"); } }
        public IAspectPropertyValue<int> BrowserDiscontinuedAge { get { return GetAs<IAspectPropertyValue<int>>("BrowserDiscontinuedAge"); } }
        public IAspectPropertyValue<string> BrowserDiscontinuedMonth { get { return GetAs<IAspectPropertyValue<string>>("BrowserDiscontinuedMonth"); } }
        public IAspectPropertyValue<string> BrowserDiscontinuedYear { get { return GetAs<IAspectPropertyValue<string>>("BrowserDiscontinuedYear"); } }
        public IAspectPropertyValue<string> BrowserName { get { return GetAs<IAspectPropertyValue<string>>("BrowserName"); } }
        public IAspectPropertyValue<int> BrowserPreviewAge { get { return GetAs<IAspectPropertyValue<int>>("BrowserPreviewAge"); } }
        public IAspectPropertyValue<string> BrowserPreviewMonth { get { return GetAs<IAspectPropertyValue<string>>("BrowserPreviewMonth"); } }
        public IAspectPropertyValue<string> BrowserPreviewYear { get { return GetAs<IAspectPropertyValue<string>>("BrowserPreviewYear"); } }
        public IAspectPropertyValue<string> BrowserPropertySource { get { return GetAs<IAspectPropertyValue<string>>("BrowserPropertySource"); } }
        public IAspectPropertyValue<int> BrowserRank { get { return GetAs<IAspectPropertyValue<int>>("BrowserRank"); } }
        public IAspectPropertyValue<int> BrowserReleaseAge { get { return GetAs<IAspectPropertyValue<int>>("BrowserReleaseAge"); } }
        public IAspectPropertyValue<string> BrowserReleaseMonth { get { return GetAs<IAspectPropertyValue<string>>("BrowserReleaseMonth"); } }
        public IAspectPropertyValue<int> BrowserReleaseYear { get { return GetAs<IAspectPropertyValue<int>>("BrowserReleaseYear"); } }
        public IAspectPropertyValue<string> BrowserVendor { get { return GetAs<IAspectPropertyValue<string>>("BrowserVendor"); } }
        public IAspectPropertyValue<string> BrowserVersion { get { return GetAs<IAspectPropertyValue<string>>("BrowserVersion"); } }
        public IAspectPropertyValue<bool> Canvas { get { return GetAs<IAspectPropertyValue<bool>>("Canvas"); } }
        public IAspectPropertyValue<bool> CookiesCapable { get { return GetAs<IAspectPropertyValue<bool>>("CookiesCapable"); } }
        public IAspectPropertyValue<bool> CssBackground { get { return GetAs<IAspectPropertyValue<bool>>("CssBackground"); } }
        public IAspectPropertyValue<bool> CssBorderImage { get { return GetAs<IAspectPropertyValue<bool>>("CssBorderImage"); } }
        public IAspectPropertyValue<bool> CssCanvas { get { return GetAs<IAspectPropertyValue<bool>>("CssCanvas"); } }
        public IAspectPropertyValue<bool> CssColor { get { return GetAs<IAspectPropertyValue<bool>>("CssColor"); } }
        public IAspectPropertyValue<bool> CssColumn { get { return GetAs<IAspectPropertyValue<bool>>("CssColumn"); } }
        public IAspectPropertyValue<bool> CssFlexbox { get { return GetAs<IAspectPropertyValue<bool>>("CssFlexbox"); } }
        public IAspectPropertyValue<bool> CssFont { get { return GetAs<IAspectPropertyValue<bool>>("CssFont"); } }
        public IAspectPropertyValue<bool> CssImages { get { return GetAs<IAspectPropertyValue<bool>>("CssImages"); } }
        public IAspectPropertyValue<bool> CssMediaQueries { get { return GetAs<IAspectPropertyValue<bool>>("CssMediaQueries"); } }
        public IAspectPropertyValue<bool> CssMinMax { get { return GetAs<IAspectPropertyValue<bool>>("CssMinMax"); } }
        public IAspectPropertyValue<bool> CssOverflow { get { return GetAs<IAspectPropertyValue<bool>>("CssOverflow"); } }
        public IAspectPropertyValue<bool> CssPosition { get { return GetAs<IAspectPropertyValue<bool>>("CssPosition"); } }
        public IAspectPropertyValue<bool> CssText { get { return GetAs<IAspectPropertyValue<bool>>("CssText"); } }
        public IAspectPropertyValue<bool> CssTransforms { get { return GetAs<IAspectPropertyValue<bool>>("CssTransforms"); } }
        public IAspectPropertyValue<bool> CssTransitions { get { return GetAs<IAspectPropertyValue<bool>>("CssTransitions"); } }
        public IAspectPropertyValue<bool> CssUI { get { return GetAs<IAspectPropertyValue<bool>>("CssUI"); } }
        public IAspectPropertyValue<bool> DataSet { get { return GetAs<IAspectPropertyValue<bool>>("DataSet"); } }
        public IAspectPropertyValue<bool> DataUrl { get { return GetAs<IAspectPropertyValue<bool>>("DataUrl"); } }
        public IAspectPropertyValue<bool> DeviceOrientation { get { return GetAs<IAspectPropertyValue<bool>>("DeviceOrientation"); } }
        public IAspectPropertyValue<bool> FileReader { get { return GetAs<IAspectPropertyValue<bool>>("FileReader"); } }
        public IAspectPropertyValue<bool> FileSaver { get { return GetAs<IAspectPropertyValue<bool>>("FileSaver"); } }
        public IAspectPropertyValue<bool> FileWriter { get { return GetAs<IAspectPropertyValue<bool>>("FileWriter"); } }
        public IAspectPropertyValue<bool> FormData { get { return GetAs<IAspectPropertyValue<bool>>("FormData"); } }
        public IAspectPropertyValue<bool> Fullscreen { get { return GetAs<IAspectPropertyValue<bool>>("Fullscreen"); } }
        public IAspectPropertyValue<bool> GeoLocation { get { return GetAs<IAspectPropertyValue<bool>>("GeoLocation"); } }
        public IAspectPropertyValue<bool> History { get { return GetAs<IAspectPropertyValue<bool>>("History"); } }
        public IAspectPropertyValue<bool> Html5 { get { return GetAs<IAspectPropertyValue<bool>>("Html5"); } }
        public IAspectPropertyValue<IReadOnlyList<string>> Html5Audio { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("Html5Audio"); } }
        public IAspectPropertyValue<IReadOnlyList<string>> Html5Video { get { return GetAs<IAspectPropertyValue<IReadOnlyList<string>>>("Html5Video"); } }
        public IAspectPropertyValue<bool> HtmlMediaCapture { get { return GetAs<IAspectPropertyValue<bool>>("Html-Media-Capture"); } }
        public IAspectPropertyValue<double> HtmlVersion { get { return GetAs<IAspectPropertyValue<double>>("HtmlVersion"); } }
        public IAspectPropertyValue<string> HttpLiveStreaming { get { return GetAs<IAspectPropertyValue<string>>("HttpLiveStreaming"); } }
        public IAspectPropertyValue<bool> Iframe { get { return GetAs<IAspectPropertyValue<bool>>("Iframe"); } }
        public IAspectPropertyValue<bool> IndexedDB { get { return GetAs<IAspectPropertyValue<bool>>("IndexedDB"); } }
        public IAspectPropertyValue<string> InVRMode { get { return GetAs<IAspectPropertyValue<string>>("InVRMode"); } }
        public IAspectPropertyValue<bool> IsDataMinimising { get { return GetAs<IAspectPropertyValue<bool>>("IsDataMinimising"); } }
        public IAspectPropertyValue<bool> IsEmailBrowser { get { return GetAs<IAspectPropertyValue<bool>>("IsEmailBrowser"); } }
        public IAspectPropertyValue<string> IsEmulatingDesktop { get { return GetAs<IAspectPropertyValue<string>>("IsEmulatingDesktop"); } }
        public IAspectPropertyValue<bool> IsEmulatingDevice { get { return GetAs<IAspectPropertyValue<bool>>("IsEmulatingDevice"); } }
        public IAspectPropertyValue<string> IsWebApp { get { return GetAs<IAspectPropertyValue<string>>("IsWebApp"); } }
        public IAspectPropertyValue<bool> Javascript { get { return GetAs<IAspectPropertyValue<bool>>("Javascript"); } }
        public IAspectPropertyValue<bool> JavascriptCanManipulateCSS { get { return GetAs<IAspectPropertyValue<bool>>("JavascriptCanManipulateCSS"); } }
        public IAspectPropertyValue<bool> JavascriptCanManipulateDOM { get { return GetAs<IAspectPropertyValue<bool>>("JavascriptCanManipulateDOM"); } }
        public IAspectPropertyValue<bool> JavascriptGetElementById { get { return GetAs<IAspectPropertyValue<bool>>("JavascriptGetElementById"); } }
        public IAspectPropertyValue<string> JavascriptImageOptimiser { get { return GetAs<IAspectPropertyValue<string>>("JavascriptImageOptimiser"); } }
        public IAspectPropertyValue<string> JavascriptPreferredGeoLocApi { get { return GetAs<IAspectPropertyValue<string>>("JavascriptPreferredGeoLocApi"); } }
        public IAspectPropertyValue<bool> JavascriptSupportsEventListener { get { return GetAs<IAspectPropertyValue<bool>>("JavascriptSupportsEventListener"); } }
        public IAspectPropertyValue<bool> JavascriptSupportsEvents { get { return GetAs<IAspectPropertyValue<bool>>("JavascriptSupportsEvents"); } }
        public IAspectPropertyValue<bool> JavascriptSupportsInnerHtml { get { return GetAs<IAspectPropertyValue<bool>>("JavascriptSupportsInnerHtml"); } }
        public IAspectPropertyValue<string> JavascriptVersion { get { return GetAs<IAspectPropertyValue<string>>("JavascriptVersion"); } }
        public IAspectPropertyValue<string> jQueryMobileSupport { get { return GetAs<IAspectPropertyValue<string>>("jQueryMobileSupport"); } }
        public IAspectPropertyValue<bool> Json { get { return GetAs<IAspectPropertyValue<bool>>("Json"); } }
        public IAspectPropertyValue<string> LayoutEngine { get { return GetAs<IAspectPropertyValue<string>>("LayoutEngine"); } }
        public IAspectPropertyValue<bool> Masking { get { return GetAs<IAspectPropertyValue<bool>>("Masking"); } }
        public IAspectPropertyValue<bool> Meter { get { return GetAs<IAspectPropertyValue<bool>>("Meter"); } }
        public IAspectPropertyValue<bool> PostMessage { get { return GetAs<IAspectPropertyValue<bool>>("PostMessage"); } }
        public IAspectPropertyValue<bool> Progress { get { return GetAs<IAspectPropertyValue<bool>>("Progress"); } }
        public IAspectPropertyValue<string> Promise { get { return GetAs<IAspectPropertyValue<string>>("Promise"); } }
        public IAspectPropertyValue<bool> Prompts { get { return GetAs<IAspectPropertyValue<bool>>("Prompts"); } }
        public IAspectPropertyValue<JavaScript> ScreenPixelsHeightJavaScript { get { return GetAs<IAspectPropertyValue<JavaScript>>("ScreenPixelsHeightJavaScript"); } }
        public IAspectPropertyValue<JavaScript> ScreenPixelsWidthJavaScript { get { return GetAs<IAspectPropertyValue<JavaScript>>("ScreenPixelsWidthJavaScript"); } }
        public IAspectPropertyValue<bool> Selector { get { return GetAs<IAspectPropertyValue<bool>>("Selector"); } }
        public IAspectPropertyValue<bool> SupportsTlsSsl { get { return GetAs<IAspectPropertyValue<bool>>("SupportsTls/Ssl"); } }
        public IAspectPropertyValue<bool> SupportsWebGL { get { return GetAs<IAspectPropertyValue<bool>>("SupportsWebGL"); } }
        public IAspectPropertyValue<bool> Svg { get { return GetAs<IAspectPropertyValue<bool>>("Svg"); } }
        public IAspectPropertyValue<bool> TouchEvents { get { return GetAs<IAspectPropertyValue<bool>>("TouchEvents"); } }
        public IAspectPropertyValue<bool> Track { get { return GetAs<IAspectPropertyValue<bool>>("Track"); } }
        public IAspectPropertyValue<bool> Video { get { return GetAs<IAspectPropertyValue<bool>>("Video"); } }
        public IAspectPropertyValue<bool> Viewport { get { return GetAs<IAspectPropertyValue<bool>>("Viewport"); } }
        public IAspectPropertyValue<bool> WebWorkers { get { return GetAs<IAspectPropertyValue<bool>>("WebWorkers"); } }
        public IAspectPropertyValue<bool> Xhr2 { get { return GetAs<IAspectPropertyValue<bool>>("Xhr2"); } }
        public IAspectPropertyValue<string> CrawlerName { get { return GetAs<IAspectPropertyValue<string>>("CrawlerName"); } }
        public IAspectPropertyValue<bool> IsCrawler { get { return GetAs<IAspectPropertyValue<bool>>("IsCrawler"); } }
    }
}
