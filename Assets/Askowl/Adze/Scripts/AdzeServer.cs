namespace Adze {
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using Askowl;
  using Decoupled;
  using JetBrains.Annotations;
  using UnityEngine;

  /// <a href=""></a> //#TBD#//
  public abstract class AdzeServer : CustomAsset.Constant.OfType<AdzeServer> {
    /// <a href=""></a> //#TBD#//
    protected string AppKey;
    /// <a href=""></a> //#TBD#//
    protected string AppSignature;
    /// <a href=""></a> //#TBD#//
    protected string Location;

    [SerializeField] private List<Key> appKeys;
    /// <a href=""></a> //#TBD#//
    [SerializeField] protected Mode mode = Mode.Reward;
    /// <a href=""></a> //#TBD#//
    [SerializeField] public int priority = 1;
    /// <a href=""></a> //#TBD#//
    [SerializeField] public int usageBalance = 1;

    private bool enabled;

    /// <a href=""></a> //#TBD#//
    public bool AdActionTaken { get; protected set; }
    /// <a href=""></a> //#TBD#//
    public bool Error { get; protected set; }

    /// <a href=""></a> //#TBD#//
    protected virtual void Destroy() { }

    /// <a href=""></a> //#TBD#//
    protected virtual bool ShowNow() => false;

    /// <a href=""></a> //#TBD#//
    protected virtual bool Dismissed { get; set; }

    /// <a href=""></a> //#TBD#//
    public IEnumerator Show(Mode modeRequested, [CanBeNull] string location) {
      if (!string.IsNullOrEmpty(location)) Location = location;

      AdActionTaken = Error = Dismissed = false;

      if (modeRequested == mode) log(action: "Show", message: "Now");
      Error = !enabled || (modeRequested != mode) || !ShowNow();

      while (!Error && !Dismissed) yield return null;
    }

    /// <a href=""></a> //#TBD#//
    public void OnEnable() {
      name     = GetType().Name;
      Location = "Default";

      foreach (Key appKey in appKeys) {
        if (!(enabled = Application.platform == appKey.Platform)) continue;

        AppKey = appKey.Value;
        string[] separators = {";", " ", ",", ":"};

        string[] parts = appKey.Value.Split(
          separator: separators,
          options: StringSplitOptions.RemoveEmptyEntries);

        AppKey       = parts[0];
        AppSignature = (parts.Length > 1) ? parts[1] : "";
        Location     = (parts.Length > 2) ? parts[2] : "";

        Initialise();
        return;
      }

      if (Application.platform != RuntimePlatform.OSXEditor) {
        Debug.LogWarning(
          "**** No viable platform to enable " + name +
          " on "                               + Application.platform);
      }

      Error = true;
    }

    public void OnDisable() { Destroy(); }

    private Log.MessageRecorder log = Log.Messages();

//    // ReSharper disable once MemberCanBePrivate.Global
//    protected void Log(string action, string result, [NotNull] params object[] more) {
//      log.Event("Adze", action, result, name, Mode, Location, Analytics.More(more));
//    }

//    // ReSharper disable once UnusedMember.Global
//    protected void LogError(string message) {
//      log.Error("Adze", message, name, Mode, "Location=", Location);
//    }

    [Serializable] public struct Key {
      public RuntimePlatform Platform;
      public string          Value;
    }
  }
}