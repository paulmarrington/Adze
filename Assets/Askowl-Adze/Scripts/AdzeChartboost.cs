﻿#if AdzeChartboost
namespace Adze {
  using System;
  using System.Collections;
  using ChartboostSDK;
  using JetBrains.Annotations;
  using UnityEngine;

  [CreateAssetMenu(menuName = "Adze/Chartboost", fileName = "AdzeChartboost")]
  public sealed class AdzeChartboost : AdzeServer {
    private Action<string>     chartboostShow;
    private Func<string, bool> isLoaded;
    private string[]           keyPair;

    protected override void Initialise() {
      // appKey is made up of AppId and AppSignature separated by ; or similar
      string[] separators = {";", " ", ",", ":"};

      keyPair = AppKey.Split(separator: separators, options: StringSplitOptions.RemoveEmptyEntries);

      switch (Mode) {
        case Mode.Interstitial:
          chartboostShow = ShowInterstitial;
          isLoaded       = HasInterstitial;
          break;

        case Mode.Reward:
          chartboostShow = showRewardedVideo;
          isLoaded       = HasRewardedVideo;
          break;
      }

      SetupDelegates();

      //      Chartboost.Create();
      Chartboost.setAutoCacheAds(true);
    }

    protected override void Prepare(string location) {
      CBLocation cbLocation = CBLocation.locationFromName(name: location);
      CBSettings.setAppId(appId: keyPair[0], appSignature: keyPair[1]);
      Chartboost.cacheRewardedVideo(cbLocation);
    }

    private bool HasRewardedVideo(string location) {
      return Chartboost.hasRewardedVideo(CBLocation.locationFromName(name: location));
    }

    private void showRewardedVideo(string location) {
      CBLocation cbLocation = CBLocation.locationFromName(name: location);
      Chartboost.showRewardedVideo(cbLocation);
      Chartboost.cacheRewardedVideo(cbLocation);
    }

    private bool HasInterstitial(string location) {
      return Chartboost.hasRewardedVideo(CBLocation.locationFromName(name: location));
    }

    private void ShowInterstitial(string location) {
      CBLocation cbLocation = CBLocation.locationFromName(name: location);

      CBSettings.setAppId(appId: keyPair[0], appSignature: keyPair[1]);
      Chartboost.showInterstitial(cbLocation);
      Chartboost.cacheInterstitial(cbLocation);
    }

    protected override void Destroy() { RemoveDelegates(); }

    protected override void ShowNow(string location) { chartboostShow(location); }

    protected override bool Loaded(string location) { return isLoaded(location); }

    /* ******************************************************************* */
    private void SetupDelegates() {
      // Listen to all impression-related events
      Chartboost.didInitialize                += DidInitialize;
      Chartboost.didFailToLoadInterstitial    += DidFailToLoadInterstitial;
      Chartboost.didDismissInterstitial       += DidDismissInterstitial;
      Chartboost.didCloseInterstitial         += DidCloseInterstitial;
      Chartboost.didClickInterstitial         += DidClickInterstitial;
      Chartboost.didCacheInterstitial         += DidCacheInterstitial;
      Chartboost.shouldDisplayInterstitial    += ShouldDisplayInterstitial;
      Chartboost.didDisplayInterstitial       += DidDisplayInterstitial;
      Chartboost.didFailToRecordClick         += DidFailToRecordClick;
      Chartboost.didFailToLoadRewardedVideo   += DidFailToLoadRewardedVideo;
      Chartboost.didDismissRewardedVideo      += DidDismissRewardedVideo;
      Chartboost.didCloseRewardedVideo        += DidCloseRewardedVideo;
      Chartboost.didClickRewardedVideo        += DidClickRewardedVideo;
      Chartboost.didCacheRewardedVideo        += DidCacheRewardedVideo;
      Chartboost.shouldDisplayRewardedVideo   += ShouldDisplayRewardedVideo;
      Chartboost.didCompleteRewardedVideo     += DidCompleteRewardedVideo;
      Chartboost.didDisplayRewardedVideo      += DidDisplayRewardedVideo;
      Chartboost.didPauseClickForConfirmation += DidPauseClickForConfirmation;
      Chartboost.willDisplayVideo             += WillDisplayVideo;

#if UNITY_IPHONE
      Chartboost.didCompleteAppStoreSheetFlow += DidCompleteAppStoreSheetFlow;
#endif
    }

    private void RemoveDelegates() {
      Chartboost.didInitialize                -= DidInitialize;
      Chartboost.didFailToLoadInterstitial    -= DidFailToLoadInterstitial;
      Chartboost.didDismissInterstitial       -= DidDismissInterstitial;
      Chartboost.didCloseInterstitial         -= DidCloseInterstitial;
      Chartboost.didClickInterstitial         -= DidClickInterstitial;
      Chartboost.didCacheInterstitial         -= DidCacheInterstitial;
      Chartboost.shouldDisplayInterstitial    -= ShouldDisplayInterstitial;
      Chartboost.didDisplayInterstitial       -= DidDisplayInterstitial;
      Chartboost.didFailToRecordClick         -= DidFailToRecordClick;
      Chartboost.didFailToLoadRewardedVideo   -= DidFailToLoadRewardedVideo;
      Chartboost.didDismissRewardedVideo      -= DidDismissRewardedVideo;
      Chartboost.didCloseRewardedVideo        -= DidCloseRewardedVideo;
      Chartboost.didClickRewardedVideo        -= DidClickRewardedVideo;
      Chartboost.didCacheRewardedVideo        -= DidCacheRewardedVideo;
      Chartboost.shouldDisplayRewardedVideo   -= ShouldDisplayRewardedVideo;
      Chartboost.didCompleteRewardedVideo     -= DidCompleteRewardedVideo;
      Chartboost.didDisplayRewardedVideo      -= DidDisplayRewardedVideo;
      Chartboost.didPauseClickForConfirmation -= DidPauseClickForConfirmation;
      Chartboost.willDisplayVideo             -= WillDisplayVideo;

#if UNITY_IPHONE
      Chartboost.didCompleteAppStoreSheetFlow -= DidCompleteAppStoreSheetFlow;
#endif
    }

    private void CbError(string what, string location, string errorText) {
      Complete = Error = true;

      Log(action: "Error", result: what, csv: More(location, errorText));
    }

    private void DidInitialize(bool status) {
      if (status) {
        AdActionTaken = true; // only way we can tell that I could see
      } else {
        Error = true;
        LogError(message: "Did not initialise correctly");
      }
    }

    private void DidFailToLoadInterstitial([NotNull] CBLocation location, CBImpressionError error) {
      CbError(what: "Interstitial failed to load", location: location.ToString(),
              errorText: error.ToString());
    }

    private void DidDismissInterstitial([NotNull] CBLocation location) { Complete = true; }

    private void DidCloseInterstitial(CBLocation location) { }

    private void DidClickInterstitial(CBLocation location) { AdActionTaken = true; }

    private void DidCacheInterstitial(CBLocation location) { }

    private bool ShouldDisplayInterstitial(CBLocation location) { return true; }

    private void DidDisplayInterstitial(CBLocation location) { }

    private void DidFailToRecordClick([NotNull] CBLocation location, CBClickError error) {
      CbError(what: "Failed to record click", location: location.ToString(),
              errorText: error.ToString());
    }

    private void
      DidFailToLoadRewardedVideo([NotNull] CBLocation location, CBImpressionError error) {
      CbError(what: "Rewarded video failed to load", location: location.ToString(),
              errorText: error.ToString());
    }

    private void DidDismissRewardedVideo([NotNull] CBLocation location) { Complete = true; }

    private void DidCloseRewardedVideo(CBLocation location) { }

    private void DidClickRewardedVideo(CBLocation location) { AdActionTaken = true; }

    private void DidCacheRewardedVideo(CBLocation location) { }

    private bool ShouldDisplayRewardedVideo(CBLocation location) { return true; }

    private void DidCompleteRewardedVideo(CBLocation location, int reward) { }

    private void DidDisplayRewardedVideo(CBLocation location) { }

    private void DidPauseClickForConfirmation() { }

    private void WillDisplayVideo(CBLocation location) { }

    private void DidCompleteAppStoreSheetFlow() { }
  }
}

#else
namespace Adze {
  // so we can create asset and still install Appodeal later
  [CreateAssetMenu(menuName = "Adze/Chartboost", fileName = "Chartboost")]
  public sealed class AdzeChartboost : AdzeServer {
    private static bool first = true;

    protected override void Initialise() {
      if (first) {
        Debug.LogWarning(
          message: "Install Chartboost unity package from http://www.chartboo.st/sdk/unity");

        first = false;
      }
    }

    protected override IEnumerator ShowNow(string location) {
      Debug.Log(message: "Show Chartboost Advertisement for '" + location + "'");

      Debug.LogWarning(
        message: "Show requires Chartboost unity package from http://www.chartboo.st/sdk/unity");

      yield return null;
    }
  }
}
#endif