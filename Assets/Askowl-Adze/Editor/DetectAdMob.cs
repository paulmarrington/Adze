﻿using UnityEditor;

[InitializeOnLoad]
public class DetectAdMob : DefineSymbols {
  static DetectAdMob() {
    bool usable = HasFolder("GoogleMobileAds") && Target(iOS, Android);
    AddOrRemoveDefines(usable, "AdzeAdMob");
  }
}
