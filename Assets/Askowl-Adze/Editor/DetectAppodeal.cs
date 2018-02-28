using UnityEditor;

[InitializeOnLoad]
public class DetectAppodeal : DefineSymbols {
  static DetectAppodeal() {
    bool usable = HasFolder("Appodeal") && Target(iOS, Android);
    AddOrRemoveDefines(usable, "AdzeAppodeal");
  }
}
