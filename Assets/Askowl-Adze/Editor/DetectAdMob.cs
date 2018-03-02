using UnityEditor;

[InitializeOnLoad]
public class DetectAdMob : DefineSymbols {
  static DetectAdMob() {
    bool usable = HasFolder(folder: "GoogleMobileAds") && Target(iOS, Android);
    AddOrRemoveDefines(addDefines: usable, named: "AdzeAdMob");
  }
}
