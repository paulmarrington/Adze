namespace Adze {
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using Decoupled.Analytics;
  using JetBrains.Annotations;
  using UnityEngine;

  public abstract class AdzeServer : CustomAsset<AdzeServer> {
    [HideInInspector] public bool AdActionTaken, Error, Complete;

    protected string AppKey;

    [SerializeField] private List<Key> appKeys;

    private bool enabled;

    private                    GameLog log;
    [SerializeField] protected Mode    Mode                    = Mode.Reward;
    [SerializeField] public    int     Priority                = 1;
    [SerializeField] private   int     secondsTimeoutForAdLoad = 30;
    [SerializeField] public    int     UsageBalance            = 1;

    [NotNull]
    internal string Name { get { return GetType().Name; } }

    protected virtual void Initialise() { }

    protected virtual void Destroy() { }

    protected abstract void ShowNow(string location);

    protected abstract bool Loaded(string location);

    protected virtual void Prepare(string location) { }

    public IEnumerator Show(Mode modeRequested, string location) {
      Error = true;
      if (!enabled || (modeRequested != Mode)) yield break;

      AdActionTaken = Error = Complete = false;

      Prepare(location);

      yield return WaitUntilAdLoaded(location: location);

      if (Error) yield break;

      Log(action: "Show", result: "Now", csv: More(location));
      ShowNow(location: location);

      if (Error) yield break;

      yield return WaitUntilAdDismissed();
    }

    private IEnumerator WaitUntilAdLoaded(string location) {
      float noMoreTime = Time.realtimeSinceStartup + secondsTimeoutForAdLoad;

      while (!Loaded(location: location) && !Error) {
        if (Time.realtimeSinceStartup > noMoreTime) {
          LogError(message: "Ad did not load in " + secondsTimeoutForAdLoad + " seconds");
          Error = true;
        }

        yield return null;
      }
    }

    private IEnumerator WaitUntilAdDismissed() {
      while (!Complete && !Error) yield return null;
    }

    public void OnEnable() {
      log = GameLog.Instance;

      foreach (Key appKey in appKeys) {
        if (!(enabled = Application.platform == appKey.Platform)) continue;

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

    protected void Log(string action, string result, string csv = "") {
      log.Event(name: "Adze", action: action, result: result, csv: More(Name, Mode, csv));
    }

    [NotNull]
    protected string More([NotNull] params object[] list) { return log.More(list: list); }

    protected void LogError(string message) { log.Error(message: More("Adze", Name, message)); }

    [Serializable]
    public struct Key {
      [UsedImplicitly] public RuntimePlatform Platform;
      [UsedImplicitly] public string          Value;
    }
  }
}