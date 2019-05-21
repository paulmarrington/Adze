using System;
using Askowl;
using UnityEditor;
using UnityEngine; // Do not remove
#if AdzeServiceForChartboost
using ChartboostSDK;
#endif

namespace Decoupler.Services {
  [CreateAssetMenu(menuName = "Decoupled/Adze/ServiceForChartboost", fileName = "AdzeServiceForChartboost")]
  public class AdzeServiceForChartboost : AdzeServiceAdapter {
    [InitializeOnLoadMethod] private static void DetectService() {
      bool usable = DefineSymbols.HasPackage("Chartboost") || DefineSymbols.HasFolder("Chartboost");
      DefineSymbols.AddOrRemoveDefines(addDefines: usable, named: "AdzeServiceForChartboost");
    }

    #region Service Entry Points
    #if AdzeServiceForChartboost
    private Service<ShowDto> runningService;
    public override Emitter Call(Service<ShowDto> service) {
      if (!PrepareAdvertisement()) {
        service.ErrorMessage = "Chartboost advertisement already running";
        return null;
      }
      service.Dto.response = (addActionTaken: false, dismissed: false, serviceError: "");
      runningService       = service;
      chartboostShow();
      return service.Emitter;
    }

    #region Chartboost Implementation
    private CBLocation cbLocation;
    private Action     chartboostShow, chartboostCache;
    private Func<bool> isLoaded;
    private void SetChartboostData() {
      cbLocation = CBLocation.locationFromName(context.location);
      CBSettings.setAppId(context.key, context.signature);
    }

    protected override void OnEnable() {
      base.OnEnable();
      if (string.IsNullOrEmpty(context.location)) context.location = "Default";
      SetChartboostData();
      SetModeAndDelegates();
      Chartboost.Create(); // loads a gameObject with script into the scene
      Chartboost.setAutoCacheAds(true);
      PrepareAdvertisement();
    }
    protected override void OnDisable() {
      RemoveDelegates();
      base.OnDisable();
    }

    private bool PrepareAdvertisement() {
      SetChartboostData();
      if (isLoaded()) return true;
      chartboostCache();
      return false;
    }
    private void SetModeAndDelegates() {
      switch (context.mode) {
        case AdzeContext.Mode.Interstitial:
          chartboostShow  = () => Chartboost.showInterstitial(cbLocation);
          isLoaded        = () => Chartboost.hasInterstitial(cbLocation);
          chartboostCache = () => Chartboost.cacheInterstitial(cbLocation);
          SetupInterstitialDelegates();
          break;

        case AdzeContext.Mode.Reward:
          chartboostShow  = () => Chartboost.showRewardedVideo(cbLocation);
          isLoaded        = () => Chartboost.hasRewardedVideo(cbLocation);
          chartboostCache = () => Chartboost.cacheRewardedVideo(cbLocation);
          SetupRewardDelegates();
          break;
      }
    }
    /* ******************************************************************* */
    private void SetupInterstitialDelegates() {
      // Listen to all impression-related events
      Chartboost.didInitialize                += DidInitialize;
      Chartboost.didFailToLoadInterstitial    += DidFailToLoadInterstitial;
      Chartboost.didDismissInterstitial       += DidDismissInterstitial;
      Chartboost.didCloseInterstitial         += DidCloseInterstitial;
      Chartboost.didClickInterstitial         += DidClickInterstitial;
      Chartboost.didCacheInterstitial         += DidCacheInterstitial;
      Chartboost.shouldDisplayInterstitial    += ShouldDisplayInterstitial;
      Chartboost.didFailToRecordClick         += DidFailToRecordClick;
      Chartboost.didPauseClickForConfirmation += DidPauseClickForConfirmation;
      Chartboost.willDisplayVideo             += WillDisplayVideo;

      #if UNITY_IPHONE
      Chartboost.didCompleteAppStoreSheetFlow += DidCompleteAppStoreSheetFlow;
      #endif
    }

    private void SetupRewardDelegates() {
      // Listen to all impression-related events
      Chartboost.didInitialize                += DidInitialize;
      Chartboost.didFailToRecordClick         += DidFailToRecordClick;
      Chartboost.didFailToLoadRewardedVideo   += DidFailToLoadRewardedVideo;
      Chartboost.didDismissRewardedVideo      += DidDismissRewardedVideo;
      Chartboost.didCloseRewardedVideo        += DidCloseRewardedVideo;
      Chartboost.didClickRewardedVideo        += DidClickRewardedVideo;
      Chartboost.didCacheRewardedVideo        += DidCacheRewardedVideo;
      Chartboost.shouldDisplayRewardedVideo   += ShouldDisplayRewardedVideo;
      Chartboost.didCompleteRewardedVideo     += DidCompleteRewardedVideo;
      Chartboost.didPauseClickForConfirmation += DidPauseClickForConfirmation;
      Chartboost.willDisplayVideo             += WillDisplayVideo;

      #if UNITY_IPHONE
      Chartboost.didCompleteAppStoreSheetFlow += DidCompleteAppStoreSheetFlow;
      #endif
    }

    private void RemoveDelegates() {
      switch (context.mode) {
        case AdzeContext.Mode.Interstitial:
          Chartboost.didInitialize                -= DidInitialize;
          Chartboost.didFailToLoadInterstitial    -= DidFailToLoadInterstitial;
          Chartboost.didDismissInterstitial       -= DidDismissInterstitial;
          Chartboost.didCloseInterstitial         -= DidCloseInterstitial;
          Chartboost.didClickInterstitial         -= DidClickInterstitial;
          Chartboost.didCacheInterstitial         -= DidCacheInterstitial;
          Chartboost.shouldDisplayInterstitial    -= ShouldDisplayInterstitial;
          Chartboost.didFailToRecordClick         -= DidFailToRecordClick;
          Chartboost.didPauseClickForConfirmation -= DidPauseClickForConfirmation;
          Chartboost.willDisplayVideo             -= WillDisplayVideo;
          break;

        case AdzeContext.Mode.Reward:
          Chartboost.didInitialize                -= DidInitialize;
          Chartboost.didFailToLoadRewardedVideo   -= DidFailToLoadRewardedVideo;
          Chartboost.didDismissRewardedVideo      -= DidDismissRewardedVideo;
          Chartboost.didCloseRewardedVideo        -= DidCloseRewardedVideo;
          Chartboost.didClickRewardedVideo        -= DidClickRewardedVideo;
          Chartboost.didCacheRewardedVideo        -= DidCacheRewardedVideo;
          Chartboost.shouldDisplayRewardedVideo   -= ShouldDisplayRewardedVideo;
          Chartboost.didCompleteRewardedVideo     -= DidCompleteRewardedVideo;
          Chartboost.didFailToRecordClick         -= DidFailToRecordClick;
          Chartboost.didPauseClickForConfirmation -= DidPauseClickForConfirmation;
          Chartboost.willDisplayVideo             -= WillDisplayVideo;
          break;
      }

      #if UNITY_IPHONE
      Chartboost.didCompleteAppStoreSheetFlow -= DidCompleteAppStoreSheetFlow;
      #endif
    }

    private void CbError(string what, string location, string errorText) =>
      Fail(runningService, $"{what},{location},{errorText}");

    private void DidInitialize(bool status) {
      if (status) {
        runningService.Dto.response.addActionTaken = true; // only way we can tell that I could see
      } else {
        Fail(runningService, "Did not initialise correctly");
      }
    }

    private void DidFailToLoadInterstitial(CBLocation location, CBImpressionError cbImpressionError) =>
      CbError(
        what: "Interstitial failed to load", location: location.ToString(),
        errorText: cbImpressionError.ToString());

    private void DidDismissInterstitial(CBLocation location) { }

    private void DidCloseInterstitial(CBLocation location) => Succeed(runningService);

    private void DidClickInterstitial(CBLocation location) => runningService.Dto.response.addActionTaken = true;

    private void DidCacheInterstitial(CBLocation location) { }

    private bool ShouldDisplayInterstitial(CBLocation location) => true;

    private void DidFailToRecordClick(CBLocation location, CBClickError cbClickError) =>
      CbError(
        what: "Failed to record click", location: location.ToString(),
        errorText: cbClickError.ToString());

    private void
      DidFailToLoadRewardedVideo(CBLocation location, CBImpressionError cbImpressionError) =>
      CbError(
        what: "Rewarded video failed to load", location: location.ToString(),
        errorText: cbImpressionError.ToString());

    private void DidDismissRewardedVideo(CBLocation location) { }

    private void DidCloseRewardedVideo(CBLocation location) => Succeed(runningService);

    private void DidClickRewardedVideo(CBLocation location) => runningService.Dto.response.addActionTaken = true;

    private void DidCacheRewardedVideo(CBLocation location) { }

    private bool ShouldDisplayRewardedVideo(CBLocation location) => true;

    private void DidCompleteRewardedVideo(CBLocation location, int reward) { }

    private void DidPauseClickForConfirmation() { }

    private void WillDisplayVideo(CBLocation location) { }

    // ReSharper disable once UnusedMember.Local
    private void DidCompleteAppStoreSheetFlow() { }
    #endregion

    #else
    public override Emitter Call(Service<ShowDto> service) => throw new NotImplementedException("ShowDto");
    #endif
    #endregion

    // One service override per service method
    // Access the external service here. Save and call dti.Emitter.Fire when service call completes
    // or set dto.ErrorMessage if the service call fails to initialise

    #region Compiler Definition
    #if AdzeServiceForChartboost
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void RegisterService() { }
    public override bool IsExternalServiceAvailable() => true;
    #else
    public override bool IsExternalServiceAvailable() => false;
    #endif
    #endregion
  }
}