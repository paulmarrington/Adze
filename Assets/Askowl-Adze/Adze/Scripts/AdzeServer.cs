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
  
  abstract public class AdzeServer : CustomAsset<AdzeServer> {

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
    protected string appKey;

    public virtual void Initialise(string key) {
    }

    public virtual void Destroy() {
    }

    abstract public IEnumerator showNow(string location);

    public IEnumerator Show(string location = "Default") {
      adActionTaken = error = false;
      return showNow(location);
    }

    public IEnumerator Show(Mode mode) {
      if (this.mode == mode) {
        return Show();
      }
      Debug.LogWarning("*** AdServer mode " + this.mode + "!=" + mode + " expected");
      error = true;
      return null;
    }

    public virtual void OnEnable() {
      string key = null;
      foreach (Key appKey in appKeys) {
        if (Application.platform == appKey.platform) {
          key = appKey.value;
          break;
        }
      }
      Initialise(this.appKey = key);
    }

    public virtual void OnDisable() {
      Destroy();
    }
  }
}