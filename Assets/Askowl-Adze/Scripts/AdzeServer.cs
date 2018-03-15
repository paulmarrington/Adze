namespace Adze {
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using Decoupled.Analytics;
  using JetBrains.Annotations;
  using UnityEngine;

  public abstract class AdzeServer : CustomAsset<AdzeServer> {
    protected string AppKey;

    [SerializeField] private List<Key> appKeys;

    private bool enabled;

    private                    GameLog log;
    [SerializeField] protected Mode    Mode                    = Mode.Reward;
    [SerializeField] public    int     Priority                = 1;
    [SerializeField] protected int     SecondsTimeoutForAdLoad = 30;
    [SerializeField] public    int     UsageBalance            = 1;

    [NotNull]
    public string Name { get { return GetType().Name; } }

    public bool AdActionTaken { get; protected set; }
    public bool Error         { get; protected set; }

    protected virtual void Initialise() { }
    protected virtual void Destroy()    { }

    protected virtual  bool Prepare() { return true; }
    protected abstract bool ShowNow();

    protected virtual bool Loaded    { get; set; }
    protected virtual bool Dismissed { get; set; }

    protected string Location { get; private set; }

    public IEnumerator Show(Mode modeRequested, string location) {
      try {
        Location = location;
        if (!enabled || (modeRequested != Mode)) throw new Exception();

        AdActionTaken = Error = Dismissed = false;

        if (!Prepare()) throw new Exception();

        if (!Loaded) {
          Log(action: "Show", result: "Not Ready");
          throw new Exception();
        }

        Log(action: "Show", result: "Now");
        if (!ShowNow()) throw new Exception();
      } catch {
        Error = true;
        yield break;
      }

      yield return WaitUntilAdDismissed();
    }

    private IEnumerator WaitUntilAdDismissed() {
      while (!Dismissed && !Error) yield return null;
    }

    public void OnEnable() {
      Location = "default";
      log      = GameLog.Instance;

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
      log.Event(name: "Adze", action: action, result: result, csv: More(Name, Mode, Location, csv));
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