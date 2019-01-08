using System;
using System.Collections;
using CustomAsset.Constant;
using UnityEngine;

namespace Adze {
  public enum Mode {
    //    Banner,
    Interstitial,
    Reward
  }

  [CreateAssetMenu(menuName = "Adze/Distributor", fileName = "AdzeDistributor")]
  public sealed class AdzeDistributor : OfType<AdzeDistributor> {
    [SerializeField] private Mode         defaultMode = Mode.Reward;
    [SerializeField] private bool         roundRobin  = true;
    [SerializeField] private AdzeServer[] servers;

    public bool AdShown       { get; private set; }
    public bool AdActionTaken { get; private set; }
    public bool Error         { get; private set; }

    private int       currentServer, lastServer;
    private int[]     usages;
    private Mode      currentMode;
    private Analytics log;

    public IEnumerator Show(string location = "") { yield return Show(defaultMode, location); }

    public IEnumerator Show(Mode mode, string location = "") {
      Error = true;

      if (servers.Length == 0) yield break;

      float timeScale = Time.timeScale;
      Time.timeScale = 0;
      currentMode    = mode;
      AdActionTaken  = AdShown = false;

      if (!roundRobin) currentServer = 0; // alway start with the primary server
      lastServer = currentServer;         // set so we only try each server once

      for (int i = 0; i < 3;) {
        yield return ShowOnCurrentServer(location);

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

    private IEnumerator ShowOnCurrentServer(string location) {
      yield return servers[currentServer].Show(currentMode, location);

      Error = servers[currentServer].Error;

      if (Error) yield break;

      AdShown       = true;
      AdActionTaken = servers[currentServer].AdActionTaken;

      usages[currentServer] += 1;

      if ((usages[currentServer] % servers[currentServer].UsageBalance) == 0) {
        PrepareNextServer(); // ready for next time we show an advertisement
      }

      string actionText = AdActionTaken ? "Ad action taken" : "Ad displayed";

      log.Event("Adze", "Player Action", actionText, currentMode, location);
    }

    public void OnEnable() {
      log = Analytics.Instance;

      if (servers == null) {
        log.Error("Adze", "No advertising servers in Adze Distributor Custom Asset");
        servers = new AdzeServer[0];
      }

      Array.Sort(array: servers, comparison: (x, y) => x.Priority.CompareTo(value: y.Priority));

      currentServer = 0;
      usages        = new int[servers.Length];
    }

    private bool PrepareNextServer() {
      while ((currentServer = (currentServer + 1) % servers.Length) != lastServer) {
        if (servers[currentServer].UsageBalance != 0) {
          return true; // only allow if this ad server is meant to serve in the loop
        }
      }

      return false; // will always use the first/only server
    }
  }
}