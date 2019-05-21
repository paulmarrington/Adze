using UnityEngine;

namespace Decoupler.Services {
  /// Services Manager resides in project hierarchy to load and initialise service management
  [CreateAssetMenu(menuName = "Decoupled/Adze/Service Manager", fileName = "AdzeServicesManager")]
  public class AdzeServicesManager : Services<AdzeServiceAdapter, AdzeContext> { }
}