using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if AdzeAppodeal
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
#endif

namespace Adze {
  [CreateAssetMenu(menuName = "Adze/Appodeal", fileName = "Appodeal")]
  #if AdzeAppodeal
  public class AdzeAppodeal : AdzeServer, IInterstitialAdListener, INonSkippableVideoAdListener, IBannerAdListener {
    int appodealMode = -1;
    #else
  public class AdzeAppodeal : AdzeServer {
    static bool first = true;
    #endif

    static Dictionary<Mode,int> appodealModes;
    bool complete;
    Decoupled.Analytics.GameLog log;

    public override void Initialise(string appKey) {
      log = Decoupled.Analytics.GameLog.Instance;
      #if AdzeAppodeal
      #if (UNITY_ANDROID || UNITY_IPHONE)
      int NON_SKIPPABLE_VIDEO = Appodeal.NON_SKIPPABLE_VIDEO;
      #else
      int NON_SKIPPABLE_VIDEO = 256;
      #endif
      appodealModes = new Dictionary<Mode,int> () {
        { Mode.Interstitial, Appodeal.INTERSTITIAL },
        { Mode.Reward, NON_SKIPPABLE_VIDEO },
      };
      appodealMode = appodealModes [mode];

      disableNetworks();
      Appodeal.setAutoCache(appodealMode, true);
//      Appodeal.disableLocationPermissionCheck();
      Appodeal.initialize(appKey, appodealMode);
      Appodeal.setTesting(Debug.isDebugBuild);
      Appodeal.setLogLevel(Debug.isDebugBuild ? Appodeal.LogLevel.Verbose : Appodeal.LogLevel.None);
      Appodeal.setBannerCallbacks(this);
      Appodeal.setInterstitialCallbacks(this);
      Appodeal.setNonSkippableVideoCallbacks(this);
      #else
      if (first) {
        Debug.LogWarning("Install Appodeal unity package from https://www.appodeal.com/sdk/unity2");
        first = false;
      }
      #endif
    }

    public override IEnumerator showNow(string location) {
      #if AdzeAppodeal
      log.Event("Adze", "Show", "Appodeal " + location);
      complete = false;
//      if (!Debug.isDebugBuild && !Appodeal.isLoaded(appodealMode)) {
//        yield return After.Realtime.seconds(1);
//      }
      if (location == "Default" || Appodeal.canShow(appodealMode, location)) {
        Appodeal.show(appodealMode, location);
      } else {
        log.Event("Adze", "Appodeal ad not shown for location '" + location + "'");
        complete = true;
      }

      #else
      Debug.Log("Show Appodeal Advertisement for '" + location + "'");
      Debug.LogWarning("Show requires Appodeal unity package from https://www.appodeal.com/sdk/unity2");
      complete = true;
      #endif

      while (!complete) {
        yield return null;
      }
    }

    public void onNonSkippableVideoLoaded() {
      loaded = true;
    }

    public void onNonSkippableVideoFailedToLoad() {
      complete = error = true;
      loaded = false;
      log.Event("Adze", "Appodeal Non-Skippable Video Failed To Load");
    }

    public void onNonSkippableVideoShown() {
      loaded = false;
    }

    public void onNonSkippableVideoFinished() {
      adActionTaken = true;
    }

    public void onNonSkippableVideoClosed(bool finished) {
      error = !finished;
      complete = true;
    }

    public void onInterstitialClosed() {
      complete = true;
    }

    public void onInterstitialLoaded(bool isPrecache) {
      loaded = true;
    }

    public void onInterstitialFailedToLoad() {
      loaded = false;
      complete = error = true;
      log.Event("Adze", "Appodeal Interstitial Failed To Load");
    }

    public void onInterstitialShown() {
      loaded = false;
    }

    public void onInterstitialClicked() {
      adActionTaken = true;
    }

    public void onBannerLoaded(bool isPrecache) {
      loaded = true;
    }

    public void onBannerFailedToLoad() {
      loaded = false;
      complete = error = true;
      log.Event("Adze", "Appodeal Interstitial Failed To Load");
    }

    public void onBannerShown() {
      loaded = false;
    }

    public void onBannerClicked() {
      adActionTaken = true;
    }

    public enum Network {
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
    }

    public Network[] disabledNetworks = new Network[] { Network.pubnative };

    void disableNetworks() {
      foreach (Network disabledNetwork in disabledNetworks) {
        #if AdzeAppodeal
        Appodeal.disableNetwork(disabledNetwork.ToString());
        #endif
      }
    }
  }
}