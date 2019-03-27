// Copyright 2019 (C) paul@marrington.net http://www.askowl.net/unity-packages

using UnityEngine;
//#if TemplateServiceFor.Services
//// Add using statements for service library here
//#endif

namespace Decoupler.Services {
  /// <a href=""></a><inheritdoc /> //#TBD#//
  [CreateAssetMenu(menuName = "Custom Assets/Services/Adze/ServiceFor", fileName = "AdzeServiceFor")]
  public abstract class Adze1ServiceFor : Adze1ServiceAdapter {
    #if TemplateServiceFor
    protected override void Prepare() => base.Prepare();
    protected override void LogOnResponse() => base.LogOnResponse();

    protected override string Display(Emitter emitter, Result result) => throw new NotImplementedException();
    #endif
  }
}