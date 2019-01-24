// Copyright 2019 (C) paul@marrington.net http://www.askowl.net/unity-packages

using UnityEngine;

namespace CustomAsset.Services {
  /// <a href=""></a> //#TBD#//
  [CreateAssetMenu(menuName = "Custom Assets/Services/Adze/Context", fileName = "AdzeContext")]
  public class AdzeContext : Services<AdzeServiceAdapter, AdzeContext>.Context {
    #region Context Equality
    /// <a href=""></a> //#TBD#//
    // ReSharper disable MissingXmlDoc
    public enum Mode { Banner, Interstitial, Reward }
    // ReSharper restore MissingXmlDoc

    /// <a href=""></a> //#TBD#//
    [SerializeField] public RuntimePlatform platform = default;
    /// <a href=""></a> //#TBD#//
    [SerializeField] public Mode mode = Mode.Reward;
    #endregion

    #region Connection Data
    /// <a href=""></a> //#TBD#//
    [SerializeField] public string key = default;
    /// <a href=""></a> //#TBD#//
    [SerializeField] public string signature = default;
    /// <a href=""></a> //#TBD#//
    [SerializeField] public string location = "Default";
    #endregion

    /// <a href=""></a> //#TBD#//
    protected bool Equals(AdzeContext other) =>
      base.Equals(other) && Equals(platform, other.platform) && Equals(mode, other.mode);
  }
}