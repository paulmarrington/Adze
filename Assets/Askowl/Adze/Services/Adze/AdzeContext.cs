using UnityEngine;

namespace Decoupler.Services {
  [CreateAssetMenu(menuName = "Decoupled/Adze/Add Context", fileName = "AdzeContext")]
  public class AdzeContext : Services<AdzeServiceAdapter, AdzeContext>.Context{
    #region Service Validity Fields

    [SerializeField] public RuntimePlatform platform;

    public enum Mode { Banner, Interstitial, Reward }
    [SerializeField] public Mode mode = Mode.Reward;


    /// <a href="">Equality is used to decide if a service is valid in this context</a> //#TBD#//
    protected bool Equals(AdzeContext other) =>
      base.Equals(other)  && Equals(platform, other.platform) && Equals(mode, other.mode);
    #endregion

    #region Other Context Fields
    [SerializeField] public string key       = default;
    [SerializeField] public string signature = default;
    [SerializeField] public string location  = "Default";
    #endregion
  }
}