using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class DetectChartboost : DefineSymbols {
  static DetectChartboost() {
    bool usable = HasFolder("Chartboost") && Target(iOS, Android);
    AddOrRemoveDefines(usable, "AdzeChartboost");
  }
}
