#if AdzeChartboost
namespace Adze {
  using System;
  using ChartboostSDK;
  using JetBrains.Annotations;
  using UnityEngine;

  [CreateAssetMenu(menuName = "Adze/Chartboost", fileName = "AdzeChartboost")]
  public sealed class AdzeChartboost : AdzeServer {
    private Action     chartboostShow, chartboostCache;
    private Func<bool> isLoaded;
    private CBLocation cbLocation;

    protected override void Initialise() {
      // appKey is made up of AppId and AppSignature separated by ; or similar
      SetChartboostData();

      switch (Mode) {
        case Mode.Interstitial:
          chartboostShow  = () => Chartboost.showInterstitial(cbLocation);
          isLoaded        = () => Chartboost.hasInterstitial(cbLocation);
          chartboostCache = () => Chartboost.cacheInterstitial(cbLocation);
          SetupInterstitialDelegates();
          break;

        case Mode.Reward:
          chartboostShow  = () => Chartboost.showRewardedVideo(cbLocation);
          isLoaded        = () => Chartboost.hasRewardedVideo(cbLocation);
          chartboostCache = () => Chartboost.cacheRewardedVideo(cbLocation);
          SetupRewardDelegates();
          break;
      }

      Chartboost.Create(); // loads a gameObject with script into the scene
      Chartboost.setAutoCacheAds(true);
      Prepare();
    }

    private void SetChartboostData() {
      if (string.IsNullOrEmpty(Location)) Location = "Default";
      cbLocation = CBLocation.locationFromName(name: Location);
      CBSettings.setAppId(AppKey, AppSignature);
    }

    private bool Prepare() {
      SetChartboostData();
      if (isLoaded()) return true;

      chartboostCache();
      return false;
    }

    protected override bool ShowNow() {
      if (!Prepare()) return false;

      chartboostShow();
      return true;
    }

    private bool dismissed;

    protected override bool Dismissed {
      get { return dismissed; }
      set {
        dismissed = value;
        if (dismissed) chartboostCache();
      }
    }

    protected override void Destroy() { RemoveDelegates(); }

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
      switch (Mode) {
        case Mode.Interstitial:
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

        case Mode.Reward:
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

    private void CbError(string what, string location, string errorText) {
      Dismissed = Error = true;
      Log("Error", what, location, errorText);
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

    private void DidDismissInterstitial([NotNull] CBLocation location) { }

    private void DidCloseInterstitial(CBLocation location) { Dismissed = true; }

    private void DidClickInterstitial(CBLocation location) { AdActionTaken = true; }

    private void DidCacheInterstitial(CBLocation location) { }

    private bool ShouldDisplayInterstitial(CBLocation location) { return true; }

    private void DidFailToRecordClick([NotNull] CBLocation location, CBClickError error) {
      CbError(what: "Failed to record click", location: location.ToString(),
              errorText: error.ToString());
    }

    private void
      DidFailToLoadRewardedVideo([NotNull] CBLocation location, CBImpressionError error) {
      CbError(what: "Rewarded video failed to load", location: location.ToString(),
              errorText: error.ToString());
    }

    private void DidDismissRewardedVideo([NotNull] CBLocation location) { }

    private void DidCloseRewardedVideo(CBLocation location) { Dismissed = true; }

    private void DidClickRewardedVideo(CBLocation location) { AdActionTaken = true; }

    private void DidCacheRewardedVideo(CBLocation location) { }

    private bool ShouldDisplayRewardedVideo(CBLocation location) { return true; }

    private void DidCompleteRewardedVideo(CBLocation location, int reward) { }

    private void DidPauseClickForConfirmation() { }

    private void WillDisplayVideo(CBLocation location) { }

    private void DidCompleteAppStoreSheetFlow() { }
  }
}

#else
namespace Adze {
  using UnityEngine;

  // so we can create asset and still install Appodeal later
  [CreateAssetMenu(menuName = "Adze/Chartboost", fileName = "AdzeChartboost")]
  public sealed class AdzeChartboost : AdzeServer {
    protected override void Initialise() {
      Debug.LogWarning("*** Install Chartboost unity package from " +
                       "http://www.chartboo.st/sdk/unity");
    }
  }
}
#endif