#if AdzeAppodeal
#endif

namespace Adze {
  using System.Collections;
  using System.Collections.Generic;
  using AppodealAds.Unity.Api;
  using AppodealAds.Unity.Common;
  using JetBrains.Annotations;
  using UnityEngine;

  [CreateAssetMenu(menuName = "Adze/Appodeal", fileName = "Appodeal")]
#if AdzeAppodeal
  public sealed class AdzeAppodeal : AdzeServer, IInterstitialAdListener,
                                     INonSkippableVideoAdListener, IBannerAdListener {
    private static bool initialised;
    private        int  appodealMode = -1;
#else
  public sealed class AdzeAppodeal : AdzeServer {
    private static bool first = true;
#endif

    private static Dictionary<Mode, int> appodealModes;
    private        bool                  complete;

    protected override void Initialise() {
#if AdzeAppodeal
#if (UNITY_ANDROID || UNITY_IPHONE)
      int NON_SKIPPABLE_VIDEO = Appodeal.NON_SKIPPABLE_VIDEO;
#else
      int NON_SKIPPABLE_VIDEO = 256;
#endif
      appodealModes = new Dictionary<Mode, int>() {
        {Mode.Interstitial, Appodeal.INTERSTITIAL},
        {Mode.Reward, NON_SKIPPABLE_VIDEO},
      };

      appodealMode = appodealModes[key: Mode];
      InitialiseAppodealOnce(modes: Appodeal.INTERSTITIAL | NON_SKIPPABLE_VIDEO);

      switch (Mode) {
        case Mode.Interstitial:
          Appodeal.setInterstitialCallbacks(listener: this);
          break;
        case Mode.Reward:
          Appodeal.setNonSkippableVideoCallbacks(listener: this);
          break;
      }
#else
      if (first) {
        Debug.LogWarning(message: "Install Appodeal unity package from https://www.appodeal.com/sdk/unity2");
        first = false;
      }
#endif
    }

    private void InitialiseAppodealOnce(int modes) {
      if (initialised) return;

      initialised = true;
      DisableNetworksWeDoNotWantToUse();
      Appodeal.setAutoCache(adTypes: modes, autoCache: true);
//      Appodeal.disableLocationPermissionCheck();
      Appodeal.initialize(appKey: AppKey, adTypes: modes);
      Appodeal.setTesting(test: Debug.isDebugBuild);
    }

    protected override IEnumerator ShowNow(string location) {
#if AdzeAppodeal
      complete = Error = false;

      if ((location == "Default") || Appodeal.canShow(adTypes: appodealMode, placement: location)) {
        Appodeal.show(adTypes: appodealMode, placement: location);
      } else {
        Log(action: "Show", result: "Unavailable", csv: location);
        complete = Error = true;
      }
#else
      Debug.Log(message: "Show Appodeal Advertisement for '" + location + "'");
      Debug.LogWarning(message: "Show requires Appodeal unity package from https://www.appodeal.com/sdk/unity2");
      complete = true;
#endif
      while (!complete) {
        yield return null;
      }
    }

#if AdzeAppodeal
    public void onNonSkippableVideoLoaded() { }

    public void onNonSkippableVideoFailedToLoad() { complete = Error = true; }

    public void onNonSkippableVideoShown() { }

    public void onNonSkippableVideoFinished() { AdActionTaken = true; }

    public void onNonSkippableVideoClosed(bool finished) {
      Error    = !finished;
      complete = true;
    }

    public void onInterstitialClosed() { complete = true; }

    public void onInterstitialLoaded(bool isPrecache) { }

    public void onInterstitialFailedToLoad() {
      complete = Error = true;
      Log(action: "Load", result: "Failed");
    }

    public void onInterstitialShown() { }

    public void onInterstitialClicked() { AdActionTaken = true; }

    public void onBannerLoaded(bool isPrecache) { }

    public void onBannerFailedToLoad() {
      complete = Error = true;
      Log(action: "Load", result: "Failed");
    }

    public void onBannerShown() { }

    public void onBannerClicked() { AdActionTaken = true; }
#endif

    public enum Network {
      // ReSharper disable InconsistentNaming
      // ReSharper disable UnusedMember.Global

      adcolony,
      admob,
      amazon_ads,
      appgrowth,
      applovin,
      axonix,
      avocarrot,
      chartboost,
      clickky,
      facebook,
      flurry,
      fractional,
      inmobi,
      inneractive,
      ironsource,
      liquidm,
      mobvista,
      mopub,
      mytarget,
      ogury,
      openx,
      pubnative,
      smaato,
      startapp,
      tapjoy,
      unity_ads,
      vungle,
      yandex

      // ReSharper restore UnusedMember.Global
      // ReSharper restore InconsistentNaming
    }

    public Network[] DisabledNetworks = {Network.pubnative};

    [UsedImplicitly]
    private void DisableNetworksWeDoNotWantToUse() {
      // ReSharper disable once UnusedVariable
      foreach (Network disabledNetwork in DisabledNetworks) {
#if AdzeAppodeal
        Appodeal.disableNetwork(network: disabledNetwork.ToString());
#endif
      }
    }
  }
}