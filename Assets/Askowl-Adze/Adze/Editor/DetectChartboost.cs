using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class DetectChartboost : AddDefineSymbols {
  static DetectChartboost() {
    if (HasFolder("Chartboost")) {
      AddDefines("AdzeChartboost");
    }
  }
}
