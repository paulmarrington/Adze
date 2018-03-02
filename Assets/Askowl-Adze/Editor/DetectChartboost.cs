using UnityEditor;

[InitializeOnLoad]
public class DetectChartboost : DefineSymbols {
  static DetectChartboost() {
    bool usable = HasFolder(folder: "Chartboost") && Target(iOS, Android);
    AddOrRemoveDefines(addDefines: usable, named: "AdzeChartboost");
  }
}
