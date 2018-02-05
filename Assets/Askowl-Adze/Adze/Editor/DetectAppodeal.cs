using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class DetectAppodeal : AddDefineSymbols {
  static DetectAppodeal() {
    if (HasFolder("Appodeal")) {
      AddDefines("AdzeAppodeal");
    }
  }
}
