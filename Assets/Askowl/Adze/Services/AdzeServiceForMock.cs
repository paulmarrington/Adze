// Copyright 2019 (C) paul@marrington.net http://www.askowl.net/unity-packages

using Askowl;
using UnityEngine;

namespace CustomAsset.Services {
  /// <a href=""></a> //#TBD#//
  [CreateAssetMenu(menuName = "Custom Assets/Services/Adze/ServiceForMock", fileName = "AdzeServiceForMock")]
  public class AdzeServiceForMock : AdzeServiceAdapter {
    /// <inheritdoc />
    protected override string Display(Emitter emitter, Result result) {
      Log("Mocking", "Display Advertisement");
      emitter.Fire();
      return default;
    }

    /// <a href="">Prepare the mock service for operations</a> //#TBD#//
    protected override void Prepare() => base.Prepare();

    /// <a href="">Use Log and Error to record analytics based on service responses</a> //#TBD#//
    protected override void LogOnResponse(Emitter emitter) => base.LogOnResponse(emitter);

    /// <inheritdoc />
    public override bool IsExternalServiceAvailable() => true;
  }
}