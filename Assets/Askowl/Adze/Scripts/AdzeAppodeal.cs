// Copyright 2019 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System.Collections.Generic;
using UnityEngine;

namespace Askowl.Adze {
  #if AdzeAppodeal
  using AppodealAds.Unity.Api;
  using AppodealAds.Unity.Common;
  #endif

  /// <a href=""></a> //#TBD#//
  [CreateAssetMenu(menuName = "Adze/Appodeal", fileName = "AdzeAppodeal")]
  #if AdzeAppodeal
  public sealed class AdzeAppodeal : AdzeServer, IInterstitialAdListener,
                                     INonSkippableVideoAdListener, IBannerAdListener {
    private static bool initialised;
    private        int  appodealMode = -1;
  #else
  public sealed class AdzeAppodeal : AdzeServer {
    #endif

    private static Dictionary<Mode, int> appodealModes;

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
      Debug.LogWarning(
        "*** Install Appodeal unity package from" +
        " https://www.appodeal.com/sdk/unity2");
      #endif
    }

    #if AdzeAppodeal
    private void InitialiseAppodealOnce(int modes) {
      if (initialised) return;

      initialised = true;
      DisableNetworksWeDoNotWantToUse();
      Appodeal.setAutoCache(adTypes: modes, autoCache: true);
//      Appodeal.disableLocationPermissionCheck();
      Appodeal.initialize(appKey: AppKey, adTypes: modes);
      Appodeal.setTesting(test: Debug.isDebugBuild);
    }

    protected override bool ShowNow() {
      if (!Appodeal.isLoaded(adTypes: appodealMode)) return false;

      if ((Location == "Default") || Appodeal.canShow(adTypes: appodealMode, placement: Location)) {
        Appodeal.show(adTypes: appodealMode, placement: Location);
        return true;
      } else {
        Log("Show", "Unavailable", Location);
        return false;
      }
    }

    public void onNonSkippableVideoLoaded() { }

    public void onNonSkippableVideoFailedToLoad() { Dismissed = Error = true; }

    public void onNonSkippableVideoShown() { }

    public void onNonSkippableVideoFinished() { }

    public void onNonSkippableVideoClosed(bool finished) {
      Error = !finished;
      Dismissed = true;
    }

    public void onInterstitialClosed() { Dismissed = true; }

    public void onInterstitialLoaded(bool isPrecache) { }

    public void onInterstitialFailedToLoad() {
      Dismissed = Error = true;
      Log(action: "Load", result: "Failed");
    }

    public void onInterstitialShown() { }

    public void onInterstitialClicked() { AdActionTaken = true; }

    public void onBannerLoaded(bool isPrecache) { }

    public void onBannerFailedToLoad() {
      Dismissed = Error = true;
      Log(action: "Load", result: "Failed");
    }

    public void onBannerShown() { }

    public void onBannerClicked() { AdActionTaken = true; }
    #endif

    /// <a href=""></a> //#TBD#//
    public enum Network {
      // ReSharper disable UnusedMember.Global
      // ReSharper disable MissingXmlDoc
      adcolony, admob, amazon_ads, appgrowth, applovin
    , axonix, avocarrot, chartboost, clickky, facebook
    , flurry, fractional, inmobi, inneractive, ironsource
    , liquidm, mobvista, mopub, mytarget, ogury
    , openx, pubnative, smaato, startapp, tapjoy
    , unity_ads, vungle, yandex
      // ReSharper restore MissingXmlDoc
      // ReSharper restore UnusedMember.Global
    }

    /// <a href=""></a> //#TBD#//
    public Network[] disabledNetworks = {
      Network.pubnative
    };

// ReSharper disable once UnusedMember.Local
    private void DisableNetworksWeDoNotWantToUse() {
      // ReSharper disable once UnusedVariable
      foreach (Network disabledNetwork in disabledNetworks) {
        #if AdzeAppodeal
        Appodeal.disableNetwork(network: disabledNetwork.ToString());
        #endif
      }
    }
  }
}