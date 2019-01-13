// Copyright 2019 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using Askowl;
using UnityEngine;

namespace Decoupled.Adze {
  /// <a href=""></a> //#TBD#//
  [Serializable] public abstract class AdzeService : Service<AdzeService> {
    /// <a href=""></a> //#TBD#//
    // ReSharper disable MissingXmlDoc
    public enum Mode { Banner, Interstitial, Reward }
    // ReSharper restore MissingXmlDoc

    /// <a href=""></a> //#TBD#//
    [SerializeField] public RuntimePlatform platform;
    /// <a href=""></a> //#TBD#//
    [SerializeField] public Mode mode;
    /// <a href=""></a> //#TBD#//
    [SerializeField] public string key;
    /// <a href=""></a> //#TBD#//
    [SerializeField] public string signature;
    /// <a href=""></a> //#TBD#//
    [SerializeField] public string location;

    /// <a href=""></a> //#TBD#//
    public bool AdActionTaken { get; protected set; }
    /// <a href=""></a> //#TBD#//
    public bool Dismissed { get; protected set; }
    /// <a href=""></a> //#TBD#//
    public bool ServiceError { get; protected set; }

    /// <a href=""></a> //#TBD#//
    protected Log.MessageRecorder Log;
    /// <a href=""></a> //#TBD#//
    protected Log.EventRecorder Error;
    /// <a href=""></a> //#TBD#//
    protected string Location;
    /// <a href=""></a> //#TBD#//
    protected Emitter Emitter;

    private string name;

    /// <a href=""></a> //#TBD#//
    protected override void Initialise() {
      Log      = Askowl.Log.Messages();
      Error    = Askowl.Log.Errors();
      name     = GetType().Name;
      Emitter  = Emitter.Instance;
      Location = "Default";
      Emitter.Subscribe(Messages);
    }

    private void Messages() {
      if (ServiceError) {
        Error("Service Error");
      } else if (Dismissed) {
        Log("Dismissed", "By Player");
      } else {
        Log("Action", AdActionTaken ? "Taken" : "Not Taken");
      }
    }

    /// <a href=""></a> //#TBD#//
    public Emitter Show() {
      AdActionTaken = Dismissed = ServiceError = false;
      Log(action: "Show", message: "Now");
      ServiceError = Display();
      return ServiceError ? Emitter : null;
    }

    /// <a href=""></a> //#TBD#//
    protected abstract bool Display();
  }
}