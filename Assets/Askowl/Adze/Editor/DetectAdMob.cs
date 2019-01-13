using UnityEditor;

namespace Askowl {
  /// <a href=""></a> //#TBD#//
  [InitializeOnLoad] public sealed class DetectAdMob : DefineSymbols {
    static DetectAdMob() {
      bool usable = HasFolder(folder: "GoogleMobileAds") && Target(iOS, Android);
      AddOrRemoveDefines(addDefines: usable, named: "AdzeAdMob");
    }
  }
}