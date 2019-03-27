// Copyright 2019 (C) paul@marrington.net http://www.askowl.net/unity-packages

using UnityEngine;

namespace Decoupler.Services {
  /// <inheritdoc />
  [CreateAssetMenu(menuName = "Custom Assets/Services/Adze/ServicesManager", fileName = "AdzeServicesManager")]
  public class Adze1ServicesManager : Services<Adze1ServiceAdapter, Adze1Context> { }
}