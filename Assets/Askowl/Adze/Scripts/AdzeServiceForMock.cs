// Copyright 2019 (C) paul@marrington.net http://www.askowl.net/unity-packages

using Askowl;
using UnityEngine;

namespace CustomAsset.Services {
  /// <a href=""></a> //#TBD#//
  [CreateAssetMenu(menuName = "Custom Assets/Services/Adze/Service", fileName = "AdzeSelector")]
  public class AdzeServiceAdapterForMock : AdzeServiceAdapter {
    /// <inheritdoc />
    protected override void Prepare() { }

    /// <inheritdoc />
    protected override string Display(Emitter emitter) {
      Log("Mocking", "Display Advertisement");
      emitter.Fire();
      return default;
    }
  }
}