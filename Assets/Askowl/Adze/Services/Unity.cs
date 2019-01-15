// Copyright 2019 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using Decoupled.Adze;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if AdzeUnity
using UnityEngine.Advertisements;

#endif

namespace Askowl.Adze {
  /// <a href=""></a> //#TBD#//
  [CreateAssetMenu(menuName = "Adze/Unity", fileName = "AdzeUnity")]
  public class Unity : Service {
    #if AdzeUnity
    private ShowOptions options;

    /// <inheritdoc />
    protected override void Initialise() {
      base.Initialise();
      Advertisement.Initialize(gameId: key, Application.isEditor);
      options = new ShowOptions {resultCallback = HandleShowResult};

      if (string.IsNullOrEmpty(Location)) {
        switch (mode) {
          case Mode.Interstitial:
            Location = "rewardedVideo";
            break;
          case Mode.Reward:
            Location = "video";
            break;
          default: throw new ArgumentOutOfRangeException();
        }
      }
    }

    /// <inheritdoc />
    protected override bool Display() {
      Advertisement.Show(placementId: Location, showOptions: options);
      return true;
    }

    private void HandleShowResult(ShowResult result) {
      switch (result) {
        case ShowResult.Failed:
          ServiceError = true;
          break;
        case ShowResult.Skipped:
          AdActionTaken = false;
          break;
        case ShowResult.Finished:
          AdActionTaken = true;
          break;
      }

      Dismissed = true;
      Emitter.Fire();
    }
    #else
    /// <inheritdoc />
    protected override bool Display() => true;
    #endif

    #if UNITY_EDITOR
    /// <a href=""></a> //#TBD#//
    [InitializeOnLoad] public sealed class DetectAdMob : DefineSymbols {
      static DetectAdMob() {
        bool usable = HasPackage("com.unity.ads");
        AddOrRemoveDefines(addDefines: usable, named: "AdzeUnity");
      }
    }
    #endif
  }
}