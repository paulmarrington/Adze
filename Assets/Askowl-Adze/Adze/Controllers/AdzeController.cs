using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

/*
 * Usage:
namespace Ads {
  public class AnAdServer : AdServer {
    public override void initialise (string key) {}
    public override void show (Mode mode) {}
  }
} */

namespace Adze {
  
  abstract public class AdzeController : CustomAsset<AdzeController> {

    [Serializable]
    public struct Key {
      public RuntimePlatform platform;
      public string value;
    }

    public List<Key> appKeys;

    public Mode mode = Mode.Reward;
    public int priority = 1;
    public int usageBalance = 1;

    [HideInInspector]
    public bool adActionTaken, error, loaded;

    public class Service : Decoupled.Service {

      public IEnumerator showNow() {
        Debug.LogError("No Adze Services registered");
        yield return null;
      }

      public override IEnumerator Initialise() {
        yield return null;
      }

      public override IEnumerator Destroy() {
        yield return null;
      }
    }

    Service services;

    public IEnumerator Show() {
      adActionTaken = error = false;
      return services.showNow();
    }

    public IEnumerator Show(Mode mode) {
      if (this.mode == mode) {
        return Show();
      }
      Debug.LogWarning("*** AdServer mode " + this.mode + "!=" + mode + " expected");
      error = true;
      return null;
    }

    public void OnEnable() {
      string key = null;
      foreach (Key appKey in appKeys) {
        if (Application.platform == appKey.platform) {
          key = appKey.value;
          break;
        }
      }
      initialise(key);
    }
  }
}