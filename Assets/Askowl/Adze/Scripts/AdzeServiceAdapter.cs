// Copyright 2019 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using Askowl;
using UnityEngine;

namespace CustomAsset.Services {
  /// <a href=""></a> //#TBD#//
  [CreateAssetMenu(menuName = "Custom Assets/Services/Adze/Service", fileName = "AdzeSelector")]
  public abstract class AdzeServiceAdapter : Services<AdzeServiceAdapter, AdzeContext>.ServiceAdapter {
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

    /// <a href="">Ask for advert and returns emitter to wait on completion or null on service error</a> //#TBD#//
    public Emitter Show() {
      AdActionTaken = Dismissed = default;
      ServiceError  = default;
      Log(action: "Show", message: "Now");
      var emitter = GetAnEmitter();
      ServiceError = Display(emitter);
      return ServiceError == default ? emitter : null;
    }

    /// <a href="">Display advert, returning default for no error, empty for no logging of a message else error message</a> //#TBD#//
    protected abstract string Display(Emitter emitter);
  }
}