using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if AdzeAppodeal
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;

namespace Adze {
  [CreateAssetMenu(menuName = "Adze/Appodeal", fileName = "Appodeal")]
  public class AdzeAppodeal : AdzeServer, IInterstitialAdListener, INonSkippableVideoAdListener {

    private static Dictionary<Mode,int> appodealModes;
    private int appodealMode = -1;
    private bool complete;

    public override void Initialise(string appKey) {

#if UNITY_ANDROID || UNITY_EDITOR || UNITY_IPHONE
      int NON_SKIPPABLE_VIDEO = Appodeal.NON_SKIPPABLE_VIDEO;

#else
      int NON_SKIPPABLE_VIDEO = 256;
#endif
      appodealModes = new Dictionary<Mode,int> () {
        { Mode.Interstitial, Appodeal.INTERSTITIAL },
        { Mode.Banner, Appodeal.BANNER },
        { Mode.Reward, NON_SKIPPABLE_VIDEO },
      };
      appodealMode = appodealModes [mode];
      Appodeal.setAutoCache(appodealMode, true);
      Appodeal.disableLocationPermissionCheck();
      Appodeal.initialize(appKey, appodealMode);
      Appodeal.setTesting(Debug.isDebugBuild);
      Appodeal.setLogLevel(Debug.isDebugBuild ? Appodeal.LogLevel.Verbose : Appodeal.LogLevel.None);
      Appodeal.setInterstitialCallbacks(this);
      Appodeal.setNonSkippableVideoCallbacks(this);
    }

    public override IEnumerator showNow(string location) {
      complete = false;
//      if (!Debug.isDebugBuild && !Appodeal.isLoaded(appodealMode)) {
//        yield return After.Realtime.seconds(1);
//      }
      Appodeal.show(appodealMode, location);
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
    }

    public void onInterstitialShown() {
      loaded = false;
    }

    public void onInterstitialClicked() {
      adActionTaken = true;
    }
  }
}

#else
namespace Adze {
  // so we can create asset and still install Appodeal later
  [CreateAssetMenu(menuName = "Adze/Appodeal", fileName = "Appodeal")]
  public class AdzeAppodeal : AdzeServer {

    public override void Initialise(string appKey) {
      Debug.LogWarning("Install Appodeal unity package from https://www.appodeal.com/sdk/unity2");
    }

    public override IEnumerator showNow() {
      Debug.LogWarning("Show requires Appodeal unity package from https://www.appodeal.com/sdk/unity2");
      yield return null;
    }
  }
}
#endif