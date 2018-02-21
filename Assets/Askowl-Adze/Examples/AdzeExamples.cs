using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Adze;

public class AdzeExamples : MonoBehaviour {

  AdzeDistributor AdMobDistributor;
  AdzeDistributor AppodealDistributor;
  AdzeDistributor ChartboostDistributor;
  AdzeReward RewardedVideoAllNetworks;

  void Start() {
    AdMobDistributor = AdzeDistributor.Asset("AdMobDistributor");
    AppodealDistributor = AdzeDistributor.Asset("AppodealDistributor");
    ChartboostDistributor = AdzeDistributor.Asset("ChartboostDistributor");

    RewardedVideoAllNetworks = AdzeReward.Asset("RewardedVideoAllNetworks");
  }

  void showResults(AdzeDistributor distributor) {
    Debug.Log("After Show: adShown=" + distributor.adShown +
      ", adActionTaken=" + distributor.adActionTaken + ", error=" + distributor.error);
  }

  IEnumerator Show(AdzeDistributor distributor, Mode mode, string location) {
    Debug.Log("Showing " + distributor.serverName + ", mode=" + mode + ", location=" + location); 
    yield return distributor.Show(mode, location); // location is optional - defaults to "Default"
    showResults(distributor);
  }
    
  public void ShowAdMobInterstitial() {
    StartCoroutine(Show(AdMobDistributor, Mode.Interstitial, "Startup"));
  }

  public void ShowAdMobRewarded() {    
    StartCoroutine(Show(AdMobDistributor, Mode.Reward, "Main Menu"));
  }

  public void ShowAppodealInterstitial() {
    StartCoroutine(Show(AppodealDistributor, Mode.Interstitial, "Achievements"));
  }

  public void ShowAppodealRewarded() {    
    StartCoroutine(Show(AppodealDistributor, Mode.Reward, "Quests"));
  }

  public void ShowChartboostInterstitial() {
    StartCoroutine(Show(ChartboostDistributor, Mode.Interstitial, "Level Start"));
  }

  public void ShowChartboostRewarded() {    
    StartCoroutine(Show(ChartboostDistributor, Mode.Reward, "Level Complete"));
  }

  IEnumerator RewardedVideoAllNetworksCoroutine() {
    yield return RewardedVideoAllNetworks.Show();
  }

  IEnumerator RewardedVideoAllNetworksCoroutine(string location) {
    yield return RewardedVideoAllNetworks.Show(location);
    Debug.Log("After Reward: adRequested=" + RewardedVideoAllNetworks.adRequested +
      ", adWatched=" + RewardedVideoAllNetworks.adWatched);
    showResults(RewardedVideoAllNetworks.distributor);
  }

  public void ShowAllRewarded() {
    StartCoroutine(RewardedVideoAllNetworksCoroutine("Turn Complete"));
  }

  // other locations include IAP Store, Item Store, Game Over, Leaderboard, Settings and Quit
}
