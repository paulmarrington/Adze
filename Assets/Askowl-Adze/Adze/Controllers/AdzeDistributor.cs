using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;

namespace Adze {

  public enum Mode {
    Banner,
    Interstitial,
    Reward
  }

  [CreateAssetMenu(menuName = "Adze/Distributor", fileName = "Distributor")]
  public class AdzeDistributor : CustomAsset<AdzeDistributor> {

    public AdServer[] servers;

    [HideInInspector]
    public bool adShown, adActionTaken, error;

    int currentServer, lastServer;
    int[] usages;
    Mode currentMode;
    Decoupled.Analytics.Play analytics;

    new public static Advertisement Asset(string name = "Distributor") {
      return CustomAsset<Advertisement>.Asset("Adze/" + name);
    }

    public IEnumerator Show(Mode mode) {
      currentMode = mode;
      adShown = adActionTaken = error = false;
      lastServer = currentServer; // set so we only try each server once
      return reshow();
    }

    public void OnEnable() {
      analytics = Decoupled.Analytics.Play.Instance;
      if (servers == null) {
        servers = new AdServer[0];
      }
      Array.Sort(servers, (x, y) => x.priority.CompareTo(y.priority));

      currentServer = 0;
      usages = new int[servers.Length];
    }

    IEnumerator reshow() {
      error = true;
      if (servers.Length != 0) {
        for (int i = 0; i < 3; i++) {
          yield return servers [currentServer].Show(currentMode);
          if (!(error = servers [currentServer].error)) {
            adShown = true;
            adActionTaken = servers [currentServer].adActionTaken;
            usages [currentServer] += 1;
            if ((usages [currentServer] % servers [currentServer].usageBalance) == 0) {
              prepareNextServer(); // ready for next time we show an advertisement
            }
            string actionText = adActionTaken ? "Adze action taken" : "Adze video watched";
            analytics.Event("Adze", "Action", actionText);
            Debug.Log("**** Adze - " + actionText);
            break;
          } else if (!prepareNextServer()) {
            Debug.LogWarning("**** Adze Ad servers not responding");
            analytics.Event("Adze", "Error", "Ad servers not responding");
            yield return After.Realtime.ms(500);
          }
        }
      }
      yield return null;
    }

    bool prepareNextServer() {
      while ((currentServer = (currentServer + 1) % servers.Length) != lastServer) {
        if (servers [currentServer].usageBalance != 0) {
          return true; // only allow if this ad server is meant to serve in the loop
        }
      }
      return false; // will always use the first/only server
    }
  }
}