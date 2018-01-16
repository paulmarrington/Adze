using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;

namespace Ads {

  public enum Mode {
    interstitial,
    banner,
    reward
  }

  [CreateAssetMenu(
    menuName = "Advertisements/Distributor",
    fileName = "Distributor")]
  public class Advertisement : CustomAsset<Advertisement> {

    public AdServer[] servers;

    [HideInInspector]
    public bool adShown, adActionTaken, error;

    private int currentServer, lastServer;
    private int[] usages;
    private Mode currentMode;

    new public static Advertisement Asset(string name = "Distributor") {
      return CustomAsset<Advertisement>.Asset("Advertisements/" + name);
    }

    public IEnumerator Show(Mode mode) {
      currentMode = mode;
      adShown = adActionTaken = error = false;
      lastServer = currentServer; // set so we only try each server once
      return reshow();
    }

    public override void OnEnable() {
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
            Answers.LogContentView("Advertisement", adActionTaken ? "Ad action taken" : "Ad video watched");
            Debug.Log("**** Advertisement - " + (adActionTaken ? "Ad action taken" : "Ad video watched"));
            break;
          } else if (!prepareNextServer()) {
            Debug.LogWarning("**** Ad servers not responding");
            Answers.LogContentView("Advertisement", "Ad servers not responding");
            yield return After.Realtime.ms(500);
          }
        }
      }
      yield return null;
      Debug.Log("**** error=" + error); //#### DELETE ME! Thu, Dec 14, 2017 9:52
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