#if AdzeChartboost
namespace Adze {
  using System;
  using System.Collections;
  using ChartboostSDK;
  using Decoupled.Analytics;
  using JetBrains.Annotations;
  using UnityEngine;

  [CreateAssetMenu(menuName = "Adze/Chartboost", fileName = "Chartboost")]
  public sealed class AdzeChartboost : AdzeServer {
    private bool           complete;
    private GameLog        analytics;
    private Action<string> chartboostShow;
    private string[]       keyPair;

    protected override void Initialise() {
      Chartboost[] chartboosts = GameObject.FindObjectsOfType<Chartboost>();
      Debug.Log("**** Chartboosts=" + chartboosts.Length);

      if (chartboosts.Length > 0) {
        Debug.Log("**** Chartboost before SendMessage");
        chartboosts[0].SendMessage("didInitializeEvent", "true");
        Debug.Log("**** Chartboost after SendMessage");
      }

      // appKey is made up of AppId and AppSignature separated by ; or similar
      string[] separators = {";", " ", ",", ":"};

      keyPair = AppKey.Split(separator: separators, options: StringSplitOptions.RemoveEmptyEntries);

      analytics = GameLog.Instance;

      switch (Mode) {
        case Mode.Interstitial:
          chartboostShow = showInterstitial;
          break;

        case Mode.Reward:
          chartboostShow = showRewardedVideo;
          break;
      }

      SetupDelegates();

      //      Chartboost.Create();
      Chartboost.setAutoCacheAds(true);
    }

    void showRewardedVideo(string location) {
      CBSettings.setAppId(appId: keyPair[0], appSignature: keyPair[1]);
      Chartboost.showRewardedVideo(location: CBLocation.locationFromName(name: location));
    }

    void showInterstitial(string location) {
      CBSettings.setAppId(appId: keyPair[0], appSignature: keyPair[1]);
      Chartboost.showInterstitial(location: CBLocation.locationFromName(name: location));
    }

    protected override void Destroy() { RemoveDelegates(); }

    protected override IEnumerator ShowNow(string location) {
      analytics.Event("Adze", "Show Chartboost", location);

      while (!Chartboost.isInitialized()) {
        yield return null;
      }

      complete = false;
      chartboostShow(location);

      while (!complete) {
        yield return null;
      }
    }

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
      complete = Error = true;
      Loaded   = false;

      analytics.Event("Adze", string.Format(
                        "Chartboost: {0} -- {1} at location {2}", what, errorText, location));
    }

    private void CbDismiss(string which, string location) {
      complete = AdActionTaken = true;
      Loaded   = false;

      analytics.Event(
        "Adze",
        string.Format(format: "Chartboost: {0} dismissed at location {1}", arg0: which,
                      arg1: location));
    }

    private void DidInitialize(bool status) {
      if (!(Error = status)) {
        analytics.Error(message: "Chartboost did not initialise correctly");
      }
    }

    private void DidFailToLoadInterstitial([NotNull] CBLocation location, CBImpressionError error) {
      CbError(what: "Interstitial failed to load", location: location.ToString(),
              errorText: error.ToString());
    }

    private void DidDismissInterstitial([NotNull] CBLocation location) {
      CbDismiss(which: "Interstitial", location: location.ToString());
    }

    private void DidCloseInterstitial(CBLocation location) { complete = true; }

    private void DidClickInterstitial(CBLocation location) { AdActionTaken = true; }

    private void DidCacheInterstitial(CBLocation location) { Loaded = true; }

    private bool ShouldDisplayInterstitial(CBLocation location) { return true; }

    private void DidDisplayInterstitial(CBLocation location) { Loaded = false; }

    private void DidFailToRecordClick([NotNull] CBLocation location, CBClickError error) {
      CbError(what: "Failed to record click", location: location.ToString(),
              errorText: error.ToString());
    }

    private void
      DidFailToLoadRewardedVideo([NotNull] CBLocation location, CBImpressionError error) {
      CbError(what: "Rewarded video failed to load", location: location.ToString(),
              errorText: error.ToString());
    }

    private void DidDismissRewardedVideo([NotNull] CBLocation location) {
      CbDismiss(which: "Rewarded video", location: location.ToString());
    }

    private void DidCloseRewardedVideo(CBLocation location) { complete = true; }

    private void DidClickRewardedVideo(CBLocation location) { AdActionTaken = true; }

    private void DidCacheRewardedVideo(CBLocation location) { Loaded = true; }

    private bool ShouldDisplayRewardedVideo(CBLocation location) { return true; }

    private void DidCompleteRewardedVideo(CBLocation location, int reward) {
      complete = AdActionTaken = true;
    }

    private void DidDisplayRewardedVideo(CBLocation location) { Loaded = false; }

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