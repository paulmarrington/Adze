using System.Collections;
using UnityEngine;
using Adze;
using JetBrains.Annotations;

public sealed class AdzeExamples : MonoBehaviour {
  private AdzeDistributor adMobDistributor;

  private AdzeDistributor appodealDistributor;
  private AdzeDistributor chartboostDistributor;
  private AdzeReward      rewardedVideoAllNetworks;

  private void Start() {
    adMobDistributor = AdzeDistributor.Asset(name: "AdMobDistributor");

    appodealDistributor   = AdzeDistributor.Asset(name: "AppodealDistributor");
    chartboostDistributor = AdzeDistributor.Asset(name: "ChartboostDistributor");

    rewardedVideoAllNetworks = AdzeReward.Asset(assetName: "RewardedVideoAllNetworks");
  }

  void showResults([NotNull] AdzeDistributor distributor) {
    Debug.Log(message: "*** After Show: adShown=" + distributor.AdShown       +
                       ", adActionTaken="         + distributor.AdActionTaken + ", error=" +
                       distributor.Error);
  }

  private IEnumerator Show([NotNull] AdzeDistributor distributor, Mode mode, string location) {
    Debug.Log(message: "*** Showing " + distributor.ServerName + ", mode=" + mode + ", location=" +
                       location);

    yield return
      distributor.Show(mode: mode,
                       location: location); // location is optional - defaults to "Default"

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
  public void ShowAppodealInterstitial() {
    StartCoroutine(routine: Show(distributor: appodealDistributor, mode: Mode.Interstitial,
                                 location: "Achievements"));
  }

  [UsedImplicitly]
  public void ShowAppodealRewarded() {
    StartCoroutine(routine: Show(distributor: appodealDistributor, mode: Mode.Reward,
                                 location: "Quests"));
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

  // other locations include IAP Store, Item Store, Game Over, Leaderboard, Settings and Quit
}