#if UNITY_EDITOR
using System.Collections;
using Adze;
using JetBrains.Annotations;
using UnityEngine;

public sealed class AdzeExamples : MonoBehaviour {
  private AdzeDistributor adMobDistributor;
  private AdzeDistributor appodealDistributor;
  private AdzeDistributor chartboostDistributor;
  private AdzeDistributor unityDistributor;
  private AdzeReward      rewardedVideoAllNetworks;

  private void Start() {
    adMobDistributor         = AdzeDistributor.Instance("AdMobDistributor");
    appodealDistributor      = AdzeDistributor.Instance("AppodealDistributor");
    chartboostDistributor    = AdzeDistributor.Instance("ChartboostDistributor");
    unityDistributor         = AdzeDistributor.Instance("UnityDistributor");
    rewardedVideoAllNetworks = AdzeReward.Instance("RewardedVideoAllNetworks");
  }

  private void showResults([NotNull] AdzeDistributor distributor) {
    Debug.Log(message: "**** >>> After Show: adShown=" + distributor.AdShown       +
                       ", adActionTaken="              + distributor.AdActionTaken + ", error=" +
                       distributor.Error);
  }

  private IEnumerator Show([NotNull] AdzeDistributor distributor, Mode mode, string location) {
    // location is optional - defaults to "Default"
    yield return distributor.Show(mode: mode, location: location);

    showResults(distributor: distributor);
  }


  public void ShowAdMobInterstitial() {
    StartCoroutine(routine: Show(distributor: adMobDistributor, mode: Mode.Interstitial,
                                 location: "Startup"));
  }


  public void ShowAdMobRewarded() {
    StartCoroutine(routine: Show(distributor: adMobDistributor, mode: Mode.Reward,
                                 location: "Main Menu"));
  }


  public void ShowUnityInterstitial() {
    // locations must be this unless a new position is defined
    StartCoroutine(routine: Show(distributor: unityDistributor, mode: Mode.Interstitial,
                                 location: "video"));
  }


  public void ShowUnityRewarded() {
    // locations must be this unless a new position is defined
    StartCoroutine(routine: Show(distributor: unityDistributor, mode: Mode.Reward,
                                 location: "rewardedVideo"));
  }


  public void ShowAppodealInterstitial() {
    StartCoroutine(routine: Show(distributor: appodealDistributor, mode: Mode.Interstitial,
                                 location: "Achievements"));
  }


  public void ShowAppodealRewarded() {
    // location must be Default or a valid Appodeal placement
    StartCoroutine(routine: Show(distributor: appodealDistributor, mode: Mode.Reward,
                                 location: "Default"));
  }


  public void ShowChartboostInterstitial() {
    StartCoroutine(routine: Show(distributor: chartboostDistributor, mode: Mode.Interstitial,
                                 location: "Level Start"));
  }


  public void ShowChartboostRewarded() {
    StartCoroutine(routine: Show(distributor: chartboostDistributor, mode: Mode.Reward,
                                 location: "Level Dismissed"));
  }

  private IEnumerator RewardedVideoAllNetworksCoroutine(string location) {
    yield return rewardedVideoAllNetworks.Show(location: location);

    Debug.Log(message: "**** >>> After Reward: adRequested=" +
                       rewardedVideoAllNetworks.AdRequested  + ", adWatched=" +
                       rewardedVideoAllNetworks.AdWatched);

    showResults(distributor: rewardedVideoAllNetworks.Distributor);
  }


  public void ShowAllRewarded() {
    // Appodeal will fail because it expects a location of 'Default'
    // Admob will fail because it does not like to coexist with Appodeal
    // So we should either get Chartboost or Unity ads
    StartCoroutine(routine: RewardedVideoAllNetworksCoroutine(location: "rewardedVideo"));
  }

  // other locations include Turn Dismissed, Game Over, Leaderboard, IAP Store, Item Store, Settings and Quit
}
#endif