#if AdzeAdMob
namespace Adze {
  using System;
  using UnityEngine;
  using JetBrains.Annotations;
  using GoogleMobileAds.Api;

  [CreateAssetMenu(menuName = "Adze/AdMob", fileName = "AdzeAdMob")]
  public sealed class AdzeAdMob : AdzeServer {
    private static bool initialised;

    private Action         showAd;
    private Func<bool>     isLoaded;
    private BannerView     bannerView;
    private InterstitialAd interstitialAd;

    private RewardBasedVideoAd rewardBasedVideoAd;

    protected override void Initialise() {
      if (!initialised) {
        string[] appKey = AppKey.Split('/');
        MobileAds.Initialize(appId: appKey[0]);
        initialised = true;
      }

      if (Mode == Mode.Reward) {
        rewardBasedVideoAd = RewardBasedVideoAd.Instance;

        rewardBasedVideoAd.OnAdLoaded += OnAdLoaded;
        rewardBasedVideoAd.OnAdFailedToLoad += OnAdFailedToLoad;
        rewardBasedVideoAd.OnAdRewarded += OnAdRewarded;
        rewardBasedVideoAd.OnAdClosed += OnAdClosed;
        rewardBasedVideoAd.OnAdLeavingApplication += OnAdLeavingApplication;

        showAd = rewardBasedVideoAd.Show;
        isLoaded = rewardBasedVideoAd.IsLoaded;
      }

      LoadNextAd();
    }

    private bool Prepare() {
      if (isLoaded()) return true;

      LoadNextAd();
      return false; // no more callbacks if ad never loads
    }

    protected override bool ShowNow() {
      if (Prepare()) {
        showAd();
        return true;
      }

      return false;
    }

    private bool dismissed;

    protected override Boolean Dismissed {
      get { return dismissed; }
      set {
        dismissed = value;
        if (dismissed) Prepare();
      }
    }

    private void LoadNextAd() {
      AdRequest adRequest = new AdRequest.Builder()
//                           .AddTestDevice(AdRequest.TestDeviceSimulator)
                           .AddKeyword(keyword: Location).Build();

      switch (Mode) {
        case Mode.Interstitial:
          if (interstitialAd != null) interstitialAd.Destroy();

          interstitialAd = new InterstitialAd(adUnitId: AppKey);
          interstitialAd.OnAdLoaded += OnAdLoaded;
          interstitialAd.OnAdFailedToLoad += OnAdFailedToLoad;
          interstitialAd.OnAdClosed += OnAdClosed;
          interstitialAd.OnAdLeavingApplication += OnAdLeavingApplication;
          interstitialAd.LoadAd(request: adRequest);
          showAd = interstitialAd.Show;
          isLoaded = interstitialAd.IsLoaded;
          break;

        case Mode.Reward:
          rewardBasedVideoAd.LoadAd(request: adRequest, adUnitId: AppKey);
          break;
      }
    }

    /* ******************************************************************* */
    private void OnAdLoaded(object sender, EventArgs args) { }

    private void OnAdFailedToLoad(object sender, [NotNull] AdFailedToLoadEventArgs args) {
      Error = true;
      Log(action: "Load", result: "Failed", more: args.Message);
    }

    private void OnAdRewarded(object sender, Reward args) { AdActionTaken = true; }

    private void OnAdClosed(object sender, EventArgs args) { Dismissed = true; }

    private void OnAdLeavingApplication(object sender, EventArgs args) { AdActionTaken = true; }
  }
}

#else
namespace Adze {
  using UnityEngine;

  // so we can create asset and still install Appodeal later
  /// <a href=""></a> //#TBD#//
  [CreateAssetMenu(menuName = "Adze/AdMob", fileName = "AdMob")]
  public sealed class AdzeAdMob : AdzeServer {
    protected override void Initialise() =>
      Debug.LogWarning(
        "*** Install AdMob unity package from" +
        " https://github.com/googleads/googleads-mobile-plugins/releases/latest");
  }
}
#endif