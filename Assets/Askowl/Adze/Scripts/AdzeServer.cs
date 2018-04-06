﻿namespace Adze {
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using Askowl;
  using Decoupled;
  using JetBrains.Annotations;
  using UnityEngine;

  public abstract class AdzeServer : CustomAsset<AdzeServer> {
    protected string AppKey;

    [SerializeField] private   List<Key> appKeys;
    [SerializeField] protected Mode      Mode         = Mode.Reward;
    [SerializeField] public    int       Priority     = 1;
    [SerializeField] public    int       UsageBalance = 1;

    private bool enabled;

    public    bool   AdActionTaken { get; protected set; }
    public    bool   Error         { get; protected set; }
    protected string Location      { get; private set; }

    protected virtual void Initialise() { }

    // ReSharper disable once VirtualMemberNeverOverridden.Global
    protected virtual void Destroy() { }

    protected virtual bool ShowNow() { return false; }

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    // ReSharper disable once VirtualMemberNeverOverridden.Global
    protected virtual bool Dismissed { get; set; }

    public IEnumerator Show(Mode modeRequested, string location) {
      Location      = location;
      AdActionTaken = Error = Dismissed = false;

      if (modeRequested == Mode) Log(action: "Show", result: "Now");
      Error = !enabled || (modeRequested != Mode) || !ShowNow();

      while (!Error && !Dismissed) yield return null;
    }

    public void OnEnable() {
      name     = GetType().Name;
      Location = "default";

      foreach (Key appKey in appKeys) {
        if (!(enabled = Application.platform == appKey.Platform)) continue;

        AppKey = appKey.Value;
        Initialise();
        return;
      }

      if (Application.platform != RuntimePlatform.OSXEditor) {
        Debug.LogWarning("**** No viable platform to enable " + name +
                         " on "                               + Application.platform);
      }

      Error = true;
    }

    public void OnDisable() { Destroy(); }

    // ReSharper disable once MemberCanBePrivate.Global
    protected void Log(string action, string result, [NotNull] params object[] more) {
      Analytics.Instance.Event("Adze", action, result, name, Mode, Location, Analytics.More(more));
    }

    // ReSharper disable once UnusedMember.Global
    protected void LogError(string message) {
      Analytics.Instance.Error("Adze", message, name, Mode, "Location=", Location);
    }

    [Serializable]
    public struct Key {
      [UsedImplicitly] public RuntimePlatform Platform;
      [UsedImplicitly] public string          Value;
    }
  }
}