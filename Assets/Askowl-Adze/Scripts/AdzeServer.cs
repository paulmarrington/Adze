using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Usage:
namespace Ads {
  public class AnAdServer : AdServer {
    public override void initialise (string key) {}
    public override void show (Mode mode) {}
  }
} */

namespace Adze {
  using JetBrains.Annotations;

  public abstract class AdzeServer : CustomAsset<AdzeServer> {
    [Serializable]
    public struct Key {
      #pragma warning disable 649
      internal RuntimePlatform Platform;
      internal string          Value;
      #pragma warning restore 649
    }

    public List<Key> AppKeys;

    public Mode Mode         = Mode.Reward;
    public int  Priority     = 1;
    public int  UsageBalance = 1;

    [HideInInspector] public bool   AdActionTaken, Error, Loaded;
    protected                string AppKey;

    [NotNull]
    internal string Name { get { return GetType().Name; } }

    protected virtual void Initialise() { }

    protected virtual void Destroy() { }

    protected abstract IEnumerator ShowNow(string location);

    internal IEnumerator Show(Mode modeRequested, string location) {
      if (modeRequested == Mode) {
        AdActionTaken = Error = false;
        yield return ShowNow(location);
      } else {
        Error = true;
        yield return null;
      }
    }

    public void OnEnable() {
      foreach (Key appKey in AppKeys) {
        if (Application.platform == appKey.Platform) {
          AppKey = appKey.Value;
          Initialise();
          return;
        }
      }
    }

    public void OnDisable() { Destroy(); }
  }
}