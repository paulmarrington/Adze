using UnityEngine;
using System;
using System.Collections;

namespace Adze {
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

    internal string ServerName { get { return Servers[currentServer].Name; } }

    private int                         currentServer, lastServer;
    private int[]                       usages;
    private Mode                        currentMode;
    private Decoupled.Analytics.GameLog log;

    internal new static AdzeDistributor Asset(string name = "Distributor") {
      return CustomAsset<AdzeDistributor>.Asset(name: name);
    }

    internal IEnumerator Show(Mode mode, string location = "Default") {
      currentMode = mode;
      AdShown     = AdActionTaken = Error = false;

      if (!RoundRobin) {
        currentServer = 0; // alway start with the primary server
      }

      lastServer = currentServer; // set so we only try each server once

      Error = true;

      if (Servers.Length != 0) {
        for (int i = 0; i < 3; i++) {
          yield return Servers[currentServer].Show(modeRequested: currentMode, location: location);

          if (!(Error = Servers[currentServer].Error)) {
            AdShown               =  true;
            AdActionTaken         =  Servers[currentServer].AdActionTaken;
            usages[currentServer] += 1;

            if ((usages[currentServer] % Servers[currentServer].UsageBalance) == 0) {
              PrepareNextServer(); // ready for next time we show an advertisement
            }

            string actionText = AdActionTaken ? "Ad action taken" : "Ad video watched";
            log.Event("Adze", "Action", actionText);
            Debug.Log(message: "**** Adze - " + actionText);
            break;
          } else if (!PrepareNextServer()) {
            Debug.LogWarning(message: "**** Adze servers not responding");
            log.Event("Adze", "Error", "Ad servers not responding");
            yield return After.Realtime.ms(ms: 500);
          }
        }
      }

      yield return null;
    }

    public void OnEnable() {
      log = Decoupled.Analytics.GameLog.Instance;

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