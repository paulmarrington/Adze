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
    adMobDistributor         = AdzeDistributor.Asset(name: "AdMobDistributor");
    appodealDistributor      = AdzeDistributor.Asset(name: "AppodealDistributor");
    chartboostDistributor    = AdzeDistributor.Asset(name: "ChartboostDistributor");
    unityDistributor         = AdzeDistributor.Asset(name: "UnityDistributor");
    rewardedVideoAllNetworks = AdzeReward.Asset(assetName: "RewardedVideoAllNetworks");
  }

  private void showResults([NotNull] AdzeDistributor distributor) {
    Debug.Log(message: "*** After Show: adShown=" + distributor.AdShown       +
                       ", adActionTaken="         + distributor.AdActionTaken + ", error=" +
                       distributor.Error);
  }

  private IEnumerator Show([NotNull] AdzeDistributor distributor, Mode mode, string location) {
    Debug.Log(message: "*** Showing " + distributor.ServerName + ", mode=" + mode + ", location=" +
                       location);

    // location is optional - defaults to "Default"
    yield return distributor.Show(mode: mode, location: location);

    showResults(distributor: distributor);
  }

  [UsedImplicitly]
  public void ShowAdMobInterstitial() {
    StartCoroutine(routine: Show(distributor: adMobDistributor, mode: Mode.Interstitial,
                                 location: "Startup"));
  }

  [UsedImplicitly]
  public void ShowAdMobRewarded() {
    StartCoroutine(routine: Show(distributor: adMobDistributor, mode: Mode.Reward,
                                 location: "Main Menu"));
  }

  [UsedImplicitly]
  public void ShowUnityInterstitial() {
    StartCoroutine(routine: Show(distributor: unityDistributor, mode: Mode.Interstitial,
                                 location: "video"));
  }

  [UsedImplicitly]
  public void ShowUnityRewarded() {
    StartCoroutine(routine: Show(distributor: unityDistributor, mode: Mode.Reward,
                                 location: "rewardedVideo"));
  }

  [UsedImplicitly]
  public void ShowAppodealInterstitial() {
    StartCoroutine(routine: Show(distributor: appodealDistributor, mode: Mode.Interstitial,
                                 location: "Achievements"));
  }

  [UsedImplicitly]
  public void ShowAppodealRewarded() {
    // location must be Default or a valid Appodeal placement
    StartCoroutine(routine: Show(distributor: appodealDistributor, mode: Mode.Reward,
                                 location: "Default"));
  }

  [UsedImplicitly]
  public void ShowChartboostInterstitial() {
    StartCoroutine(routine: Show(distributor: chartboostDistributor, mode: Mode.Interstitial,
                                 location: "Level Start"));
  }

  [UsedImplicitly]
  public void ShowChartboostRewarded() {
    StartCoroutine(routine: Show(distributor: chartboostDistributor, mode: Mode.Reward,
                                 location: "Level Complete"));
  }

  private IEnumerator RewardedVideoAllNetworksCoroutine(string location) {
    yield return rewardedVideoAllNetworks.Show(location: location);

    Debug.Log(message: "*** After Reward: adRequested=" + rewardedVideoAllNetworks.AdRequested +
                       ", adWatched="                   + rewardedVideoAllNetworks.AdWatched);

    showResults(distributor: rewardedVideoAllNetworks.Distributor);
  }

  [UsedImplicitly]
  public void ShowAllRewarded() {
    StartCoroutine(routine: RewardedVideoAllNetworksCoroutine(location: "Turn Complete"));
  }

  // other locations include Game Over, Leaderboard, IAP Store, Item Store, Settings and Quit
}