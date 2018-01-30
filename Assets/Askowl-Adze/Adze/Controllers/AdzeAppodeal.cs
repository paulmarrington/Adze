using UnityEngine;
using System.Collections;

using AppodealAds.Unity.Api;
using System.Collections.Generic;

using AppodealAds.Unity.Common;
using System.Net.Configuration;


namespace Adze {
  [CreateAssetMenu(menuName = "Adze/Appodeal", fileName = "Appodeal")]
  public class AdzeAppodeal : AdServer, IInterstitialAdListener, INonSkippableVideoAdListener {

    private static Dictionary<Mode,int> appodealModes;
    private int appodealMode = -1;
    private bool complete;

    public override void initialise(string appKey) {
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

    public override IEnumerator showNow() {
      complete = false;
      if (!Debug.isDebugBuild && !Appodeal.isLoaded(appodealMode)) {
        yield return After.Realtime.seconds(1);
      }
      Appodeal.show(appodealMode);
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