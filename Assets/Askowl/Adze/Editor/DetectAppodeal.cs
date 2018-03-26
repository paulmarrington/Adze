namespace Askowl {
  using UnityEditor;

  [InitializeOnLoad]
  public sealed class DetectAppodeal : DefineSymbols {
    static DetectAppodeal() {
      bool usable = HasFolder(folder: "Appodeal") && Target(iOS, Android);
      AddOrRemoveDefines(addDefines: usable, named: "AdzeAppodeal");
    }
  }
}