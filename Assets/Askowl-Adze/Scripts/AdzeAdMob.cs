﻿#if AdzeAdMob
namespace Adze {
  using System;
  using System.Collections;
  using GoogleMobileAds.Api;
  using JetBrains.Annotations;
  using UnityEngine;

  [CreateAssetMenu(menuName = "Adze/AdMob", fileName = "AdzeAdMob")]
  public sealed class AdzeAdMob : AdzeServer {
    private static bool initialised;

    private bool               complete, loaded;
    private Action             showAd;
    private BannerView         bannerView;
    private InterstitialAd     interstitialAd;
    private RewardBasedVideoAd rewardBasedVideoAd;

    protected override void Initialise() {
      if (!initialised) {
        string[] appKey = AppKey.Split('/');
        MobileAds.Initialize(appId: appKey[0]);
        initialised = true;
      }

      if (Mode == Mode.Reward) {
        rewardBasedVideoAd = RewardBasedVideoAd.Instance;

        rewardBasedVideoAd.OnAdLoaded             += OnAdLoaded;
        rewardBasedVideoAd.OnAdFailedToLoad       += OnAdFailedToLoad;
        rewardBasedVideoAd.OnAdRewarded           += OnAdRewarded;
        rewardBasedVideoAd.OnAdClosed             += OnAdClosed;
        rewardBasedVideoAd.OnAdLeavingApplication += OnAdLeavingApplication;

        showAd = rewardBasedVideoAd.Show;
      }

      LoadNextAd(location: "Default");
    }

    protected override IEnumerator ShowNow(string location) {
      complete = false;

//      if (Error && !loaded) LoadNextAd(location);

      while (!loaded) {
        if (Error) yield break;

        yield return null;
      }

      showAd();

      yield return WaitForResponse();

      LoadNextAd(location);
    }

    private void LoadNextAd(string location) {
      AdRequest
        adRequest = new AdRequest.Builder()
//                   .AddTestDevice(AdRequest.TestDeviceSimulator)
//                   .AddTestDevice(kGADSimulatorID)
                   .AddKeyword(keyword: location).Build();

      loaded = Error = false;

      switch (Mode) {
        case Mode.Interstitial:

          if (interstitialAd != null) interstitialAd.Destroy();

          interstitialAd                        =  new InterstitialAd(adUnitId: AppKey);
          interstitialAd.OnAdLoaded             += OnAdLoaded;
          interstitialAd.OnAdFailedToLoad       += OnAdFailedToLoad;
          interstitialAd.OnAdClosed             += OnAdClosed;
          interstitialAd.OnAdLeavingApplication += OnAdLeavingApplication;
          interstitialAd.LoadAd(request: adRequest);
          showAd = interstitialAd.Show;
          break;

        case Mode.Reward:
          rewardBasedVideoAd.LoadAd(request: adRequest, adUnitId: AppKey);
          break;
      }
    }

    /* ******************************************************************* */
    private void OnAdLoaded(object sender, EventArgs args) { loaded = true; }

    private void OnAdFailedToLoad(object sender, [NotNull] AdFailedToLoadEventArgs args) {
      Error = true;
      Log(action: "Load", result: "Failed", csv: More(args.Message));
    }

    private void OnAdRewarded(object sender, Reward args) { AdActionTaken = true; }

    private void OnAdClosed(object sender, EventArgs args) { complete = true; }

    private void OnAdLeavingApplication(object sender, EventArgs args) { AdActionTaken = true; }
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