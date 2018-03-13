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
  using Decoupled.Analytics;
  using JetBrains.Annotations;
  using UnityEngine;

  public abstract class AdzeServer : CustomAsset<AdzeServer> {
    [Serializable]
    public struct Key {
      [UsedImplicitly] public RuntimePlatform Platform;
      [UsedImplicitly] public string          Value;
    }

    [SerializeField] private   List<Key> appKeys;
    [SerializeField] protected Mode      Mode         = Mode.Reward;
    [SerializeField] public    int       Priority     = 1;
    [SerializeField] public    int       UsageBalance = 1;

    [HideInInspector] public bool AdActionTaken, Error, Complete;

    protected string AppKey;

    private bool enabled;

    [NotNull]
    internal string Name { get { return GetType().Name; } }

    protected virtual void Initialise() { }

    protected virtual void Destroy() { }

    protected abstract IEnumerator ShowNow(string location);

    protected abstract bool Loaded(string location);

    public IEnumerator Show(Mode modeRequested, string location) {
      if (enabled && (modeRequested == Mode)) {
        AdActionTaken = false;

        while (!Loaded(location) && !Error) {
          yield return null;
        }

        if (Error) yield break;

        Log(action: "Show", result: "Now", csv: More(location));
        Complete = Error = false;
        yield return ShowNow(location);
      } else {
        Error = true;
        yield return null;
      }
    }

    protected IEnumerator WaitForResponse() {
      while (!Complete && !Error) yield return null;
    }

    public void OnEnable() {
      log = GameLog.Instance;

      foreach (Key appKey in appKeys) {
        if (!(enabled = (Application.platform == appKey.Platform))) continue;

        AppKey = appKey.Value;
        Initialise();
        return;
      }

      if (Application.platform != RuntimePlatform.OSXEditor) {
        Debug.LogWarning(message: "**** No viable platform to enable " + Name +
                                  " on "                               + Application.platform);
      }

      Error = true;
    }

    public void OnDisable() { Destroy(); }

    private GameLog log;

    protected void Log(string action, string result, string csv = "") {
      log.Event(name: "Adze", action: action, result: result, csv: More(Name, Mode, csv));
    }

    [NotNull]
    protected string More([NotNull] params object[] list) { return log.More(list); }

    protected void LogError(string message) { log.Error(message: More("Adze", Name, message)); }
  }
}