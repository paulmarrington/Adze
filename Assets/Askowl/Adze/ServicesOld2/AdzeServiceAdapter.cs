// Copyright 2019 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using Askowl;
using UnityEditor;
using UnityEngine;
//#if AdzeServiceFor.Services
//// Add using statements for service library here
//#endif

namespace Decoupler.Services {
  /// <a href=""></a> //#TBD#//
  [CreateAssetMenu(menuName = "Custom Assets/Services/Adze/Service", fileName = "AdzeServiceAdapter")]
  public class Adze1ServiceAdapter : Services<Adze1ServiceAdapter, Adze1Context>.ServiceAdapter {
    #region Service Support
    // Code that is common to all services belongs here
    /// <a href=""></a> //#TBD#//
    [Serializable] public class Result {
      /// <a href="">Did the player take an action proposed by the advertisement?</a> //#TBD#//
      public bool adActionTaken;
      /// <a href="">Did the player dismiss the advertisement without watching it?</a> //#TBD#//
      public bool dismissed;
      /// <a href="">Is default for no error, empty for no logging of a message else error message</a> //#TBD#//
      public string serviceError;

      internal static Result Instance(Emitter emitter) { return null; } //return Result(emitter); }

      internal Result Clear() {
        adActionTaken = dismissed = default;
        serviceError  = default;
        return this;
      }
    }

    /// <a href=""></a> //#TBD#//
    protected override void Prepare() { }

    // Registered with Emitter to provide common logging
    protected void LogOnResponse(Emitter emitter) {
      var result = Result.Instance(emitter);
      if (result.serviceError != default) {
        if (!string.IsNullOrEmpty(result.serviceError)) Error($"Service Error: {result.serviceError}");
      } else if (result.dismissed) {
        Log("Dismissed", "By Player");
      } else {
        Log("Action", result.adActionTaken ? "Taken" : "Not Taken");
      }
    }
    #endregion

    #region Public Interface
    /// <a href="">Ask for advert and returns emitter to wait on completion or null on service error</a> //#TBD#//
    public Emitter Show() {
      Log(action: "Show", message: "Now");
      var emitter = Emitter.Instance; //GetAnEmitter<Result>();
      var result  = Result.Instance(emitter).Clear();
      Display(emitter, result);
      return result.serviceError == default ? emitter : null;
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