// Copyright 2019 (C) paul@marrington.net http://www.askowl.net/unity-packages

#if UNITY_EDITOR && Adze
using System.Collections;
using Askowl.Adze;
using JetBrains.Annotations;
using UnityEngine;

// ReSharper disable MissingXmlDoc
namespace Askowl.Examples {
  public sealed class AdzeExamples : MonoBehaviour {
    [SerializeField] private AdzeDistributor adMobDistributor         = default;
    [SerializeField] private AdzeDistributor appodealDistributor      = default;
    [SerializeField] private AdzeDistributor chartboostDistributor    = default;
    [SerializeField] private AdzeDistributor unityDistributor         = default;
    [SerializeField] private AdzeReward      rewardedVideoAllNetworks = default;

    private void showResults([NotNull] AdzeDistributor distributor) =>
      Debug.Log(
        message: "**** >>> After Show: adShown=" + distributor.AdShown       +
                 ", adActionTaken="              + distributor.AdActionTaken + ", error=" +
                 distributor.Error);

    private IEnumerator Show([NotNull] AdzeDistributor distributor, Mode mode, string location) {
      Debug.Log($"Show '{distributor}, {mode}, {location}'"); //#DM#// 
      // location is optional - defaults to "Default"
      yield return distributor.Show(mode: mode, location: location);

      showResults(distributor: distributor);
    }

    public void ShowAdMobInterstitial() =>
      StartCoroutine(
        routine: Show(
          distributor: adMobDistributor, mode: Mode.Interstitial,
          location: "Startup"));

    public void ShowAdMobRewarded() =>
      StartCoroutine(
        routine: Show(
          distributor: adMobDistributor, mode: Mode.Reward,
          location: "Main Menu"));

    public void ShowUnityInterstitial() =>
      StartCoroutine(
        routine: Show(
          distributor: unityDistributor, mode: Mode.Interstitial,
          location: "video"));

    public void ShowUnityRewarded() =>
      StartCoroutine(
        routine: Show(
          distributor: unityDistributor, mode: Mode.Reward,
          location: "rewardedVideo"));

    public void ShowAppodealInterstitial() =>
      StartCoroutine(
        routine: Show(
          distributor: appodealDistributor, mode: Mode.Interstitial,
          location: "Achievements"));

    public void ShowAppodealRewarded() =>
      StartCoroutine(
        routine: Show(
          distributor: appodealDistributor, mode: Mode.Reward,
          location: "Default"));

    public void ShowChartboostInterstitial() =>
      StartCoroutine(
        routine: Show(
          distributor: chartboostDistributor, mode: Mode.Interstitial,
          location: "Level Start"));

    public void ShowChartboostRewarded() =>
      StartCoroutine(
        routine: Show(
          distributor: chartboostDistributor, mode: Mode.Reward,
          location: "Level Dismissed"));

    private IEnumerator RewardedVideoAllNetworksCoroutine(string location) {
      yield return rewardedVideoAllNetworks.Show(location: location);

      Debug.Log(
        message: "**** >>> After Reward: adRequested=" +
                 rewardedVideoAllNetworks.adRequested  + ", adWatched=" +
                 rewardedVideoAllNetworks.adWatched);

      showResults(distributor: rewardedVideoAllNetworks.Distributor);
    }

    public void ShowAllRewarded() =>
      StartCoroutine(routine: RewardedVideoAllNetworksCoroutine(location: "rewardedVideo"));

    // other locations include Turn Dismissed, Game Over, Leaderboard, IAP Store, Item Store, Settings and Quit
  }
}
#endif