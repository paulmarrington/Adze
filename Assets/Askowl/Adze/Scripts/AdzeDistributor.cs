namespace Adze {
  using System;
  using System.Collections;
  using Askowl;
  using Decoupled;
  using UnityEngine;

  public enum Mode {
    //    Banner,
    Interstitial,
    Reward
  }

  [CreateAssetMenu(menuName = "Adze/Distributor", fileName = "AdzeDistributor")]
  public sealed class AdzeDistributor : CustomAsset<AdzeDistributor> {
    public bool         RoundRobin = true;
    public AdzeServer[] Servers;

    [HideInInspector] public bool AdShown, AdActionTaken, Error;

//    [NotNull]
//    public string ServerName { get { return Servers[currentServer].name; } }

    private int       currentServer, lastServer;
    private int[]     usages;
    private Mode      currentMode;
    private Analytics log;

    internal new static AdzeDistributor Asset(string name = "AdzeDistributor") {
      return CustomAsset<AdzeDistributor>.Asset(name: name);
    }

    internal IEnumerator Show(Mode mode, string location = "") {
      Error = true;

      if (Servers.Length == 0) yield break;

      float timeScale = Time.timeScale;
      Time.timeScale = 0;
      currentMode    = mode;
      AdActionTaken  = AdShown = false;

      if (!RoundRobin) currentServer = 0; // alway start with the primary server
      lastServer = currentServer;         // set so we only try each server once

      for (int i = 0; i < 3;) {
        yield return Show(location);

        if (!Error) break;
        if (PrepareNextServer()) break;

        i++;
        yield return new WaitForSecondsRealtime(0.5f);
      }

      if (Error) {
        log.Event("Adze", "Error", "Ad servers not responding", currentMode, location);
      }

      Time.timeScale = timeScale;
      yield return null;
    }

    private IEnumerator Show(string location) {
      yield return Servers[currentServer].Show(currentMode, location);

      Error = Servers[currentServer].Error;

      if (Error) yield break;

      AdShown       = true;
      AdActionTaken = Servers[currentServer].AdActionTaken;

      usages[currentServer] += 1;

      if ((usages[currentServer] % Servers[currentServer].UsageBalance) == 0) {
        PrepareNextServer(); // ready for next time we show an advertisement
      }

      string actionText = AdActionTaken ? "Ad action taken" : "Ad displayed";

      log.Event("Adze", "Player Action", actionText, currentMode, location);
    }

    public void OnEnable() {
      log = Analytics.Instance;

      if (Servers == null) {
        log.Error("Adze", "No advertising servers in Adze Distributor Custom Asset");
        Servers = new AdzeServer[0];
      }

      Array.Sort(array: Servers, comparison: (x, y) => x.Priority.CompareTo(value: y.Priority));

      currentServer = 0;
      usages        = new int[Servers.Length];
    }

    private bool PrepareNextServer() {
      while ((currentServer = (currentServer + 1) % Servers.Length) != lastServer) {
        if (Servers[currentServer].UsageBalance != 0) {
          return true; // only allow if this ad server is meant to serve in the loop
        }
      }

      return false; // will always use the first/only server
    }
  }
}