using UnityEngine;
using System;
using System.Collections;
#if AdzeAdMob
using GoogleMobileAds.Api;

namespace Adze {
  using JetBrains.Annotations;

  [CreateAssetMenu(menuName = "Adze/AdMob", fileName = "AdMob")]
  public sealed class AdzeAdMob : AdzeServer {
    private bool                        complete, initialised;
    private Decoupled.Analytics.GameLog analytics;
    private Action                      showAd;
    private BannerView                  bannerView;
    private InterstitialAd              interstitialAd;
    private RewardBasedVideoAd          rewardBasedVideoAd;

    public override void Initialise() {
      analytics = Decoupled.Analytics.GameLog.Instance;
      MobileAds.Initialize(appId: appKey);

      if (mode == Mode.Reward) {
        rewardBasedVideoAd                        =  RewardBasedVideoAd.Instance;
        rewardBasedVideoAd.OnAdLoaded             += OnAdLoaded;
        rewardBasedVideoAd.OnAdFailedToLoad       += OnAdFailedToLoad;
        rewardBasedVideoAd.OnAdRewarded           += OnAdRewarded;
        rewardBasedVideoAd.OnAdClosed             += OnAdClosed;
        rewardBasedVideoAd.OnAdLeavingApplication += OnAdLeavingApplication;
        showAd                                    =  rewardBasedVideoAd.Show;
      }

      LoadNextAd(location: "Default");
    }

    public override void Destroy() { }

    public override IEnumerator showNow(string location) {
      analytics.Event("Adze", "Show AdMob", location);
      complete = false;
      Debug.Log(message: "#### showNow loaded=" + loaded + ", error=" + error);

      while (!loaded && !error) {
        yield return null;
      }

      showAd();

      while (!complete) {
        yield return null;
      }

      LoadNextAd(location: location);
    }

    private void LoadNextAd(string location) {
      AdRequest adRequest = new AdRequest.Builder().AddKeyword(keyword: location).Build();
      loaded = error      = false;

      switch (mode) {
        case Mode.Interstitial:

          if (interstitialAd != null) {
            interstitialAd.Destroy();
          }

          interstitialAd                        =  new InterstitialAd(adUnitId: appKey);
          interstitialAd.OnAdLoaded             += OnAdLoaded;
          interstitialAd.OnAdFailedToLoad       += OnAdFailedToLoad;
          interstitialAd.OnAdClosed             += OnAdClosed;
          interstitialAd.OnAdLeavingApplication += OnAdLeavingApplication;
          interstitialAd.LoadAd(request: adRequest);
          showAd = interstitialAd.Show;
          break;
        case Mode.Reward:
          rewardBasedVideoAd.LoadAd(request: adRequest, adUnitId: appKey);
          break;
      }
    }

    /* ******************************************************************* */
    private void OnAdLoaded(object sender, EventArgs args) {
      Debug.Log(message: "######## Loaded for " + mode);
      loaded = true;
    }

    private void OnAdFailedToLoad(object sender, [NotNull] AdFailedToLoadEventArgs args) {
      Debug.Log(message: "######## Error for " + mode);
      error = true;
      analytics.Event("Adze", "AdMob: Failed To Load -- " + args.Message);
    }

    private void OnAdRewarded(object sender, Reward args) { adActionTaken = true; }

    private void OnAdClosed(object sender, EventArgs args) { complete = true; }

    private void OnAdLeavingApplication(object sender, EventArgs args) { adActionTaken = true; }
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