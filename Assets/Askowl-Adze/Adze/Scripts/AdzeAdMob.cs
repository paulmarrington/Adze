﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//#if AdzeAdMob
using GoogleMobileAds.Api;

namespace Adze {
  [CreateAssetMenu(menuName = "Adze/AdMob", fileName = "AdMob")]
  public class AdzeAdMob : AdzeServer {
    bool complete, initialised;
    Decoupled.Analytics.Play analytics;
    Action showAd;


    BannerView bannerView;
    InterstitialAd interstitialAd;
    RewardBasedVideoAd rewardBasedVideoAd;

    public override void Initialise(string appKey) {
      analytics = Decoupled.Analytics.Play.Instance;
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

      loadNextAd();
    }

    public override void Destroy() {
    }

    public override IEnumerator showNow(string location) {
      complete = false;
      while (!loaded && !error) {
        yield return null;
      }
      showAd();
      while (!complete) {
        yield return null;
      }
      loadNextAd();
    }

    void loadNextAd() {
      AdRequest adRequest = new AdRequest.Builder ().Build();
      loaded = error = false;

      switch (mode) {
        case Mode.Banner:
          if (bannerView != null) {
            bannerView.Destroy();
          }
          bannerView = new BannerView (appKey, AdSize.SmartBanner, AdPosition.Top);
          bannerView.OnAdLoaded += OnAdLoaded;
          bannerView.OnAdFailedToLoad += OnAdFailedToLoad;
          bannerView.OnAdOpening += OnBannerAdOpening;
          bannerView.OnAdClosed += OnAdClosed;
          bannerView.OnAdLeavingApplication += OnAdLeavingApplication;
          bannerView.LoadAd(adRequest);
          showAd = bannerView.Show;
          break;
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
      analytics.Error("AdMob did not initialise correctly: " + args.Message);
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
//
//
//#else
//namespace Adze {
//  // so we can create asset and still install Appodeal later
//  [CreateAssetMenu(menuName = "Adze/AdMob", fileName = "AdMob")]
//  public class AdMobController : AdzeServer {
//
//    public override void Initialise(string appKey) {
//      Debug.LogWarning("Install AdMob unity package from https://github.com/googleads/googleads-mobile-plugins/releases/latest");
//    }
//
//    public override IEnumerator showNow() {
//      Debug.LogWarning("Show requires AdMob unity package from https://github.com/googleads/googleads-mobile-plugins/releases/latest");
//      yield return null;
//    }
//  }
//}
//#endif
