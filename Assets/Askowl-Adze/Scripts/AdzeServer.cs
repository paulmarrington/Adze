

/*
 * Usage:
namespace Ads {
  public class AnAdServer : AdServer {
    public override void initialise (string key) {}
    public override void show (Mode mode) {}
  }
} */

namespace Adze {
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using JetBrains.Annotations;
  using UnityEngine;

  public abstract class AdzeServer : CustomAsset<AdzeServer> {
    [Serializable]
    public struct Key {
      [UsedImplicitly] public RuntimePlatform Platform;
      [UsedImplicitly] public string          Value;
    }

    public List<Key> AppKeys;

    public Mode Mode         = Mode.Reward;
    public int  Priority     = 1;
    public int  UsageBalance = 1;

    [HideInInspector] public bool   AdActionTaken, Error, Loaded;
    protected                string AppKey;

    private bool enabled;

    [NotNull]
    internal string Name { get { return GetType().Name; } }

    protected virtual void Initialise() { }

    protected virtual void Destroy() { }

    protected abstract IEnumerator ShowNow(string location);

    internal IEnumerator Show(Mode modeRequested, string location) {
      if (enabled && (modeRequested == Mode)) {
        AdActionTaken = false;
        yield return ShowNow(location);
      } else {
        Error = true;
        yield return null;
      }
    }

    public void OnEnable() {
      foreach (Key appKey in AppKeys) {
        enabled = (Application.platform == appKey.Platform);

        if (enabled) {
          AppKey = appKey.Value;
          Initialise();
          return;
        }
      }

      if (Application.platform != RuntimePlatform.OSXEditor) {
        Debug.LogWarning(message: "**** No viable platform to enable " + Name +
                                  " on "                               + Application.platform);
      }

      Error = true;
    }

    public void OnDisable() { Destroy(); }
  }
}