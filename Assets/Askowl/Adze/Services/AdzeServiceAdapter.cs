// Copyright 2019 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using Askowl;
using UnityEditor;
using UnityEngine;
#if AdzeServiceFor
// Add using statements for service library here
#endif

namespace CustomAsset.Services {
  /// <a href=""></a> //#TBD#//
  [CreateAssetMenu(menuName = "Custom Assets/Services/Adze/Service", fileName = "AdzeServiceAdapter")]
  public class AdzeServiceAdapter : Services<AdzeServiceAdapter, AdzeContext>.ServiceAdapter {
    #region Service Support
    // Code that is common to all services belongs here
    public class Result {
      /// <a href="">Did the player take an action proposed by the advertisement?</a> //#TBD#//
      public bool AdActionTaken;
      /// <a href="">Did the player dismiss the advertisement without watching it?</a> //#TBD#//
      public bool Dismissed;
      /// <a href="">Is default for no error, empty for no logging of a message else error message</a> //#TBD#//
      public string ServiceError;

      internal static Result Instance(Emitter emitter) => Result<Result>(emitter);

      internal Result Clear() {
        AdActionTaken = Dismissed = default;
        ServiceError  = default;
        return this;
      }
    }

    /// <a href=""></a> //#TBD#//
    protected override void Prepare() { }

    // Registered with Emitter to provide common logging
    protected override void LogOnResponse(Emitter emitter) {
      var result = Result.Instance(emitter);
      if (result.ServiceError != default) {
        if (!string.IsNullOrEmpty(result.ServiceError)) Error($"Service Error: {result.ServiceError}");
      } else if (result.Dismissed) {
        Log("Dismissed", "By Player");
      } else {
        Log("Action", result.AdActionTaken ? "Taken" : "Not Taken");
      }
    }
    #endregion

    #region Public Interface
    /// <a href="">Ask for advert and returns emitter to wait on completion or null on service error</a> //#TBD#//
    public Emitter Show() {
      Log(action: "Show", message: "Now");
      var emitter = GetAnEmitter<Result>();
      var result  = Result.Instance(emitter).Clear();
      Display(emitter, result);
      return result.ServiceError == default ? emitter : null;
    }
    #endregion

    #region Service Interface Methods
    /// <a href="">Display advert, returning default for no error, empty for no logging of a message else error message</a> //#TBD#//
    protected virtual string Display(Emitter emitter, Result result) => throw new NotImplementedException();
    #endregion

    #region Compiler Definition
    #if AdzeServiceFor
    public override bool IsExternalServiceAvailable() => true;
    #else
    public override bool IsExternalServiceAvailable() => false;
    #endif

    [InitializeOnLoadMethod] private static void DetectService() {
      bool usable = DefineSymbols.HasPackage("") || DefineSymbols.HasFolder("");
      DefineSymbols.AddOrRemoveDefines(addDefines: usable, named: "AdzeServiceFor");
    }
    #endregion
  }
}