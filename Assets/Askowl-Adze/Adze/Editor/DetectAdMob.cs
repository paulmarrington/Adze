using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class DetectAdMob : AddDefineSymbols {
  static DetectAdMob() {
    if (HasFolder("GoogleMobileAds")) {
      AddDefines("AdzeAdMob");
    }
  }
}
