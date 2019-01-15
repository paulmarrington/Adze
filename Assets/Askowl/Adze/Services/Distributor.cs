//// Copyright 2019 (C) paul@marrington.net http://www.askowl.net/unity-packages
//
//using System;
//using Askowl;
//using UnityEngine;
//
//namespace Decoupled.Adze {
//  /// <a href=""></a> //#TBD#//
//  [CreateAssetMenu(menuName = "Adze/Distributor", fileName = "AdzeDistributor")]
//  public class Distributor : Distributor<Service, State> { }
//
//  /// <a href=""></a> //#TBD#//
//  public struct State { }
//
//  /// <a href=""></a> //#TBD#//
//  [Serializable] public class Service : Service<State> {
//    /// <a href=""></a> //#TBD#//
//    // ReSharper disable MissingXmlDoc
//    public enum Mode { Banner, Interstitial, Reward }
//    // ReSharper restore MissingXmlDoc
//
//    /// <a href=""></a> //#TBD#//
//    [SerializeField] public RuntimePlatform platform;
//    /// <a href=""></a> //#TBD#//
//    [SerializeField] public Mode mode = Mode.Reward;
//    /// <a href=""></a> //#TBD#//
//    [SerializeField] public string key;
//    /// <a href=""></a> //#TBD#//
//    [SerializeField] public string signature;
//    /// <a href=""></a> //#TBD#//
//    [SerializeField] public string location;
//
//    /// <a href=""></a> //#TBD#//
//    [NonSerialized] public bool AdActionTaken;
//    /// <a href=""></a> //#TBD#//
//    [NonSerialized] public bool Dismissed;
//    /// <a href=""></a> //#TBD#//
//    [NonSerialized] public bool ServiceError;
//
//    /// <a href=""></a> //#TBD#//
//    [NonSerialized] protected string Location;
//
//    /// <a href=""></a> //#TBD#//
//    protected override void Initialise() {
////      name     = GetType().Name;
//      Emitter  = Emitter.Instance;
//      Location = "Default";
//      Emitter.Subscribe(Messages);
//    }
//
//    private void Messages() {
//      if (ServiceError) {
//        Error("Service Error");
//      } else if (Dismissed) {
//        Log("Dismissed", "By Player");
//      } else {
//        Log("Action", AdActionTaken ? "Taken" : "Not Taken");
//      }
//    }
//
//    /// <a href=""></a> //#TBD#//
//    public Emitter Show() {
//      AdActionTaken = Dismissed = ServiceError = false;
//      Log(action: "Show", message: "Now");
//      ServiceError = Display();
//      return ServiceError ? Emitter : null;
//    }
//
//    /// <a href=""></a> //#TBD#//
//    public override bool Has(State state) => throw new NotImplementedException();
//
//    /// <a href=""></a> //#TBD#//
//    protected virtual bool Display() => throw new NotImplementedException();
//  }
//}