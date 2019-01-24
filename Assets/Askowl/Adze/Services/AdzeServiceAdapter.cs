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
  [CreateAssetMenu(menuName = "Custom Assets/Services/Adze/Service", fileName = "AdzeSelector")]
  public abstract class AdzeServiceAdapter : Services<AdzeServiceAdapter, AdzeContext>.ServiceAdapter {
    #region Service Support
    // Code that is common to all services belongs here
    /// <a href="">Did the player take an action proposed by the advertisement?</a> //#TBD#//
    [NonSerialized] public bool AdActionTaken;
    /// <a href="">Did the player dismiss the advertisement without watching it?</a> //#TBD#//
    [NonSerialized] public bool Dismissed;
    /// <a href="">Is default for no error, empty for no logging of a message else error message</a> //#TBD#//
    [NonSerialized] public string ServiceError;

    // Registered with Emitter to provide common logging
    protected override void LogOnResponse() {
      if (ServiceError != default) {
        if (!string.IsNullOrEmpty(ServiceError)) Error($"Service Error: {ServiceError}");
      } else if (Dismissed) {
        Log("Dismissed", "By Player");
      } else {
        Log("Action", AdActionTaken ? "Taken" : "Not Taken");
      }
    }
    #endregion

    #region Public Interface
    // Methods calling code will use to call a service - over and above abstract ones defined below.
    /// <a href="">Ask for advert and returns emitter to wait on completion or null on service error</a> //#TBD#//
    public Emitter Show() {
      AdActionTaken = Dismissed = default;
      ServiceError  = default;
      Log(action: "Show", message: "Now");
      var emitter = GetAnEmitter();
      ServiceError = Display(emitter);
      return ServiceError == default ? emitter : null;
    }
    #endregion

    #region Abstract Service Interface Methods
    // List of abstract methods that all concrete service adapters need to implement

    /// <a href="">Display advert, returning default for no error, empty for no logging of a message else error message</a> //#TBD#//
    protected abstract string Display(Emitter emitter);
    #endregion

    #region Service Library Access
    #if AdzeServiceFor
    // Add any code that accesses the service library here
    #endif
    #endregion

    #region Compiler Definition
    [InitializeOnLoadMethod] private static void DetectService() {
      bool usable = DefineSymbols.HasPackage("") || DefineSymbols.HasFolder("");
      DefineSymbols.AddOrRemoveDefines(addDefines: usable, named: "AdzeServiceFor");
    }
    #endregion
  }
}