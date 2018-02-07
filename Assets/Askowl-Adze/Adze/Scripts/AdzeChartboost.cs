using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if AdzeChartboost
using ChartboostSDK;

namespace Adze {
  [CreateAssetMenu(menuName = "Adze/Chartboost", fileName = "Chartboost")]
  public class AdzeChartboost : AdzeServer {

    bool complete, initialised;
    Decoupled.Analytics.Play analytics;
    Action<string> chartboostShow;

    public override void Initialise(string appKey) {
      // appKey is made up of AppId and AppSignature separated by ; or similar
      string[] separators = { ";", " ", ",", ":" };
      string[] keyPair = appKey.Split(separators, StringSplitOptions.RemoveEmptyEntries);

      analytics = Decoupled.Analytics.Play.Instance;

      switch (mode) {
        case Mode.Banner:
          analytics.Error("Chartboost does not show banner advertisements");
          chartboostShow = (loc) => complete = initialised = error = true;
          break;
        case Mode.Interstitial:
          chartboostShow = (loc) => Chartboost.showInterstitial(CBLocation.locationFromName(loc));
          break;
        case Mode.Reward:
          chartboostShow = (loc) => Chartboost.showRewardedVideo(CBLocation.locationFromName(loc));
          break;
      }

      SetupDelegates();
      Chartboost.CreateWithAppId(keyPair [0], keyPair [1]);
      Chartboost.setAutoCacheAds(true);
    }

    public override void Destroy() {
      RemoveDelegates();
    }

    public override IEnumerator showNow(string location) {
      analytics.Event("Adze", "Show Chartboost", location);
      while (!initialised) {
        yield return null;
      }
      complete = false;
      chartboostShow(location);
      while (!complete) {
        yield return null;
      }
    }

    void showInterstitial(string location) {
      Chartboost.showInterstitial(CBLocation.locationFromName(location));
    }

    void showRewarded(string location) {
      Chartboost.showRewardedVideo(CBLocation.locationFromName(location));
    }
    /* ******************************************************************* */
    void SetupDelegates() {
      // Listen to all impression-related events
      Chartboost.didInitialize += didInitialize;
      Chartboost.didFailToLoadInterstitial += didFailToLoadInterstitial;
      Chartboost.didDismissInterstitial += didDismissInterstitial;
      Chartboost.didCloseInterstitial += didCloseInterstitial;
      Chartboost.didClickInterstitial += didClickInterstitial;
      Chartboost.didCacheInterstitial += didCacheInterstitial;
      Chartboost.shouldDisplayInterstitial += shouldDisplayInterstitial;
      Chartboost.didDisplayInterstitial += didDisplayInterstitial;
      Chartboost.didFailToRecordClick += didFailToRecordClick;
      Chartboost.didFailToLoadRewardedVideo += didFailToLoadRewardedVideo;
      Chartboost.didDismissRewardedVideo += didDismissRewardedVideo;
      Chartboost.didCloseRewardedVideo += didCloseRewardedVideo;
      Chartboost.didClickRewardedVideo += didClickRewardedVideo;
      Chartboost.didCacheRewardedVideo += didCacheRewardedVideo;
      Chartboost.shouldDisplayRewardedVideo += shouldDisplayRewardedVideo;
      Chartboost.didCompleteRewardedVideo += didCompleteRewardedVideo;
      Chartboost.didDisplayRewardedVideo += didDisplayRewardedVideo;
      Chartboost.didPauseClickForConfirmation += didPauseClickForConfirmation;
      Chartboost.willDisplayVideo += willDisplayVideo;






#if UNITY_IPHONE
      Chartboost.didCompleteAppStoreSheetFlow += didCompleteAppStoreSheetFlow;
#endif
    }

    void RemoveDelegates() {
      Chartboost.didInitialize -= didInitialize;
      Chartboost.didFailToLoadInterstitial -= didFailToLoadInterstitial;
      Chartboost.didDismissInterstitial -= didDismissInterstitial;
      Chartboost.didCloseInterstitial -= didCloseInterstitial;
      Chartboost.didClickInterstitial -= didClickInterstitial;
      Chartboost.didCacheInterstitial -= didCacheInterstitial;
      Chartboost.shouldDisplayInterstitial -= shouldDisplayInterstitial;
      Chartboost.didDisplayInterstitial -= didDisplayInterstitial;
      Chartboost.didFailToRecordClick -= didFailToRecordClick;
      Chartboost.didFailToLoadRewardedVideo -= didFailToLoadRewardedVideo;
      Chartboost.didDismissRewardedVideo -= didDismissRewardedVideo;
      Chartboost.didCloseRewardedVideo -= didCloseRewardedVideo;
      Chartboost.didClickRewardedVideo -= didClickRewardedVideo;
      Chartboost.didCacheRewardedVideo -= didCacheRewardedVideo;
      Chartboost.shouldDisplayRewardedVideo -= shouldDisplayRewardedVideo;
      Chartboost.didCompleteRewardedVideo -= didCompleteRewardedVideo;
      Chartboost.didDisplayRewardedVideo -= didDisplayRewardedVideo;
      Chartboost.didPauseClickForConfirmation -= didPauseClickForConfirmation;
      Chartboost.willDisplayVideo -= willDisplayVideo;






#if UNITY_IPHONE
      Chartboost.didCompleteAppStoreSheetFlow -= didCompleteAppStoreSheetFlow;
#endif
    }

    void cbError(string what, string location, string errorText) {
      complete = error = true;
      loaded = false;
      string msg = string.Format("Chartboost: {0} -- {1} at location {2}", what, errorText, location);
      analytics.Event("Adze", msg);
    }

    void cbDismiss(string which, string location) {
      complete = adActionTaken = true;
      loaded = false;
      analytics.Event("Adze", string.Format("Chartboost: {0} dismissed at location {1}", which, location));
    }

    void didInitialize(bool status) {
      initialised = true;
      if (!(error = status)) {
        analytics.Error("Chartboost did not initialise correctly");
      }
    }

    void didFailToLoadInterstitial(CBLocation location, CBImpressionError error) {
      cbError("Interstitial failed to load", location.ToString(), error.ToString());
    }

    void didDismissInterstitial(CBLocation location) {
      cbDismiss("Interstitial", location.ToString());
    }

    void didCloseInterstitial(CBLocation location) {
      complete = true;
    }

    void didClickInterstitial(CBLocation location) {
      adActionTaken = true;
    }

    void didCacheInterstitial(CBLocation location) {
      loaded = true;
    }

    bool shouldDisplayInterstitial(CBLocation location) {
      return true;
    }

    void didDisplayInterstitial(CBLocation location) {
      loaded = false;
    }

    void didFailToRecordClick(CBLocation location, CBClickError error) {
      cbError("Failed to record click", location.ToString(), error.ToString());
    }

    void didFailToLoadRewardedVideo(CBLocation location, CBImpressionError error) {
      cbError("Rewarded video failed to load", location.ToString(), error.ToString());
    }

    void didDismissRewardedVideo(CBLocation location) {
      cbDismiss("Rewarded video", location.ToString());
    }

    void didCloseRewardedVideo(CBLocation location) {
      complete = true;
    }

    void didClickRewardedVideo(CBLocation location) {
      adActionTaken = true;
    }

    void didCacheRewardedVideo(CBLocation location) {
      loaded = true;
    }

    bool shouldDisplayRewardedVideo(CBLocation location) {
      return true;
    }

    void didCompleteRewardedVideo(CBLocation location, int reward) {
      complete = adActionTaken = true;
    }

    void didDisplayRewardedVideo(CBLocation location) {
      loaded = false;
    }

    void didPauseClickForConfirmation() {
    }






    #if UNITY_IPHONE
    #endif

    void willDisplayVideo(CBLocation location) {
    }







    #if UNITY_IPHONE
    void didCompleteAppStoreSheetFlow() {
    }
#endif
  }
}






#else
namespace Adze {
  // so we can create asset and still install Appodeal later
  [CreateAssetMenu(menuName = "Adze/Chartboost", fileName = "Chartboost")]
  public class AdzeChartboost : AdzeServer {

    public override void Initialise(string appKey) {
      Debug.LogWarning("Install Chartboost unity package from http://www.chartboo.st/sdk/unity");
    }

    public override IEnumerator showNow(string location) {
      Debug.Log("Show Chartboost Advertisement for '" + location + "'");
      Debug.LogWarning("Show requires Chartboost unity package from http://www.chartboo.st/sdk/unity");
      yield return null;
    }
  }
}
#endif
