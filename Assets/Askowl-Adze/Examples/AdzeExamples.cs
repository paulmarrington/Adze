using System.Collections;
using UnityEngine;
using Adze;
using JetBrains.Annotations;

public sealed class AdzeExamples : MonoBehaviour {
  private AdzeDistributor adMobDistributor;

  //  AdzeDistributor AppodealDistributor;
  //  AdzeDistributor ChartboostDistributor;
  //  AdzeReward RewardedVideoAllNetworks;

  private void Start() {
    adMobDistributor = AdzeDistributor.Asset(name: "AdMobDistributor");

//    AppodealDistributor = AdzeDistributor.Asset("AppodealDistributor");
//    ChartboostDistributor = AdzeDistributor.Asset("ChartboostDistributor");

//    RewardedVideoAllNetworks = AdzeReward.Asset("RewardedVideoAllNetworks");
  }

  void showResults([NotNull] AdzeDistributor distributor) {
    Debug.Log(message: "%%%% After Show: adShown=" + distributor.AdShown       +
              ", adActionTaken="          + distributor.AdActionTaken + ", error=" + distributor.Error);
  }

  IEnumerator Show([NotNull] AdzeDistributor distributor, Mode mode, string location) {
    Debug.Log("%%%% Showing " + distributor.ServerName + ", mode=" + mode + ", location=" + location);
    yield return distributor.Show(mode, location); // location is optional - defaults to "Default"

    showResults(distributor);
  }

  public void ShowAdMobInterstitial() { StartCoroutine(Show(adMobDistributor, Mode.Interstitial, "Startup")); }

  public void ShowAdMobRewarded() { StartCoroutine(Show(adMobDistributor, Mode.Reward, "Main Menu")); }

  public void ShowAppodealInterstitial() {
//    StartCoroutine(Show(AppodealDistributor, Mode.Interstitial, "Achievements"));
  }

  public void ShowAppodealRewarded() {
//    StartCoroutine(Show(AppodealDistributor, Mode.Reward, "Quests"));
  }

  public void ShowChartboostInterstitial() {
//    StartCoroutine(Show(ChartboostDistributor, Mode.Interstitial, "Level Start"));
  }

  public void ShowChartboostRewarded() {
//    StartCoroutine(Show(ChartboostDistributor, Mode.Reward, "Level Complete"));
  }

  private IEnumerator RewardedVideoAllNetworksCoroutine() {
    yield return null; //RewardedVideoAllNetworks.Show();
  }

  private IEnumerator RewardedVideoAllNetworksCoroutine(string location) {
    yield return null; //RewardedVideoAllNetworks.Show(location);

//    Debug.Log("After Reward: adRequested=" + RewardedVideoAllNetworks.adRequested +
//      ", adWatched=" + RewardedVideoAllNetworks.adWatched);
//    showResults(RewardedVideoAllNetworks.distributor);
  }

  public void ShowAllRewarded() {
//    StartCoroutine(RewardedVideoAllNetworksCoroutine("Turn Complete"));
  }

  // other locations include IAP Store, Item Store, Game Over, Leaderboard, Settings and Quit
}