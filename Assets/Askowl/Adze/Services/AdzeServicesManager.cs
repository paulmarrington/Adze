// Copyright 2019 (C) paul@marrington.net http://www.askowl.net/unity-packages

using UnityEngine;

namespace CustomAsset.Services {
  /// <inheritdoc />
  [CreateAssetMenu(menuName = "Custom Assets/Services/Adze/ServicesManager", fileName = "AdzeServicesManager")]
  public class AdzeServicesManager : Services<AdzeServiceAdapter, AdzeContext> { }
}