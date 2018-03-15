namespace Adze {
  using System;
  using System.Collections;
  using Decoupled.Analytics;
  using JetBrains.Annotations;
  using UnityEngine;

  public enum Mode {
    //    Banner,
    Interstitial,
    Reward
  }

  [CreateAssetMenu(menuName = "Adze/Distributor", fileName = "Distributor")]
  public sealed class AdzeDistributor : CustomAsset<AdzeDistributor> {
    public bool         RoundRobin = true;
    public AdzeServer[] Servers;

    [HideInInspector] public bool AdShown, AdActionTaken, Error;

    [NotNull]
    internal string ServerName { get { return Servers[currentServer].Name; } }

    private int     currentServer, lastServer;
    private int[]   usages;
    private Mode    currentMode;
    private GameLog log;

    internal new static AdzeDistributor Asset(string name = "Distributor") {
      return CustomAsset<AdzeDistributor>.Asset(name: name);
    }

    internal IEnumerator Show(Mode mode, string location = "Default") {
      currentMode   = mode;
      AdActionTaken = AdShown = Error = false;

      if (!RoundRobin) {
        currentServer = 0; // alway start with the primary server
      }

      lastServer = currentServer; // set so we only try each server once

      Error = true;

      if (Servers.Length != 0) {
        for (int i = 0; i < 3; i++) {
          yield return Servers[currentServer].Show(modeRequested: currentMode, location: location);

          if (!(Error = Servers[currentServer].Error)) {
            AdShown       = true;
            AdActionTaken = Servers[currentServer].AdActionTaken;

            usages[currentServer] += 1;

            if ((usages[currentServer] % Servers[currentServer].UsageBalance) == 0) {
              PrepareNextServer(); // ready for next time we show an advertisement
            }

            string actionText = AdActionTaken ? "Ad action taken" : "Ad displayed";

            log.Event(name: "Adze", action: "Player Action", result: actionText,
                      csv: log.More(currentMode, location));

            break;
          } else if (!PrepareNextServer()) {
            log.Event(name: "Adze", action: "Error", result: "Ad servers not responding",
                      csv: log.More(currentMode, location));

            yield return new WaitForSecondsRealtime(0.5f);
          }
        }
      }

      yield return null;
    }

    public void OnEnable() {
      log = GameLog.Instance;

      if (Servers == null) {
        log.Error(message: "Set advertising servers in Adze Distributor Custom Asset");
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