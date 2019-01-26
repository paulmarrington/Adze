// Copyright 2019 (C) paul@marrington.net http://www.askowl.net/unity-packages

using Askowl;
using UnityEngine;

namespace CustomAsset.Services {
  /// <a href=""></a> //#TBD#//
  [CreateAssetMenu(menuName = "Custom Assets/Services/Adze/ServiceForMock", fileName = "AdzeServiceForMock")]
  public class AdzeServiceForMock : AdzeServiceAdapter {
    /// <a href=""></a> //#TBD#//
    [SerializeField] public Result mockResult;
    [SerializeField] private float secondsDelay = 0.1f;
    /// <inheritdoc />
    protected override string Display(Emitter emitter, Result result) {
      Log("Mocking", "Display Advertisement");
      result.dismissed     = mockResult.dismissed;
      result.adActionTaken = mockResult.adActionTaken;
      result.serviceError  = mockResult.serviceError;
      Fiber.Start.WaitFor(secondsDelay).Fire(emitter);
      return default;
    }

    /// <inheritdoc />
    public override bool IsExternalServiceAvailable() => true;
  }
}