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

    public string Name { get { return this.GetType().Name; } }

    public virtual void Initialise() {
    }

    public virtual void Destroy() {
    }

    abstract public IEnumerator showNow(string location);

    public IEnumerator Show(Mode modeRequested, string location) {
      if (modeRequested == mode) {
        adActionTaken = error = false;
        yield return showNow(location);
      } else {
        error = true;
        yield return null;
      }
    }

    public virtual void OnEnable() {
      foreach (Key appKey in appKeys) {
        if (Application.platform == appKey.platform) {
          Initialise(this.appKey = appKey.value);
          return;
        }
      }
    }

    public virtual void OnDisable() {
      Destroy();
    }
  }
}