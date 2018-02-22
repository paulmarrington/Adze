using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Decoupled.Analytics;

#if AdzeAdMob
using GoogleMobileAds.Api;

namespace Adze {
  [CreateAssetMenu(menuName = "Adze/AdMob", fileName = "AdMob")]
  public class AdzeAdMob : AdzeServer {
    bool complete, initialised;
    Decoupled.Analytics.GameLog analytics;
    Action showAd;

    BannerView bannerView;
    InterstitialAd interstitialAd;
    RewardBasedVideoAd rewardBasedVideoAd;

    public override void Initialise(string appKey) {
      analytics = Decoupled.Analytics.GameLog.Instance;
      MobileAds.Initialize(appKey);

      if (mode == Mode.Reward) {
        rewardBasedVideoAd = RewardBasedVideoAd.Instance;
        rewardBasedVideoAd.OnAdLoaded += OnAdLoaded;
        rewardBasedVideoAd.OnAdFailedToLoad += OnAdFailedToLoad;
        rewardBasedVideoAd.OnAdOpening += OnAdOpening;
        rewardBasedVideoAd.OnAdStarted += OnAdStarted;
        rewardBasedVideoAd.OnAdRewarded += OnAdRewarded;
        rewardBasedVideoAd.OnAdClosed += OnAdClosed;
        rewardBasedVideoAd.OnAdLeavingApplication += OnAdLeavingApplication;
        showAd = rewardBasedVideoAd.Show;
      }

      loadNextAd("Default");
    }

    public override void Destroy() {
    }

    public override IEnumerator showNow(string location) {
      analytics.Event("Adze", "Show AdMob", location);
      complete = false;
      while (!loaded && !error) {
        yield return null;
      }
      showAd();
      while (!complete) {
        yield return null;
      }
      loadNextAd(location);
    }

    void loadNextAd(string location) {
      AdRequest adRequest = new AdRequest.Builder ().AddKeyword(location).Build();
      loaded = error = false;

      switch (mode) {
        case Mode.Interstitial:
          if (interstitialAd != null) {
            interstitialAd.Destroy();
          }
          interstitialAd = new InterstitialAd (appKey);
          interstitialAd.OnAdLoaded += OnAdLoaded;
          interstitialAd.OnAdFailedToLoad += OnAdFailedToLoad;
          interstitialAd.OnAdOpening += OnAdOpening;
          interstitialAd.OnAdClosed += OnAdClosed;
          interstitialAd.OnAdLeavingApplication += OnAdLeavingApplication;
          interstitialAd.LoadAd(adRequest);
          showAd = interstitialAd.Show;
          break;
        case Mode.Reward:
          rewardBasedVideoAd.LoadAd(adRequest, appKey);
          break;
      }
    }
    /* ******************************************************************* */
    void OnAdLoaded(object sender, EventArgs args) {
      loaded = true;
    }

    void OnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args) {
      error = true;
      analytics.Event("Adze", "AdMob: Failed To Load -- " + args.Message);
    }

    void OnBannerAdOpening(object sender, EventArgs args) {
      adActionTaken = true;
    }

    void OnAdOpening(object sender, EventArgs args) {
    }

    void OnAdStarted(object sender, EventArgs args) {
    }

    void OnAdRewarded(object sender, Reward args) {
      adActionTaken = true;
    }

    void OnAdClosed(object sender, EventArgs args) {
      complete = true;
    }

    void OnAdLeavingApplication(object sender, EventArgs args) {
      adActionTaken = true;
    }
  }
}






#else
namespace Adze {
  // so we can create asset and still install Appodeal later
  [CreateAssetMenu(menuName = "Adze/AdMob", fileName = "AdMob")]
  public class AdzeAdMob : AdzeServer {

    static bool first = true;

    public override void Initialise(string appKey) {
      if (first) {
        Debug.LogWarning("Install AdMob unity package from https://github.com/googleads/googleads-mobile-plugins/releases/latest");
        first = false;
      }
    }

    public override IEnumerator showNow(string location) {
      Debug.Log("Show AdMob Advertisement for '" + location + "'");
      Debug.LogWarning("Show requires AdMob unity package from https://github.com/googleads/googleads-mobile-plugins/releases/latest");
      adActionTaken = true;
      yield return null;
    }
  }
}
#endif
