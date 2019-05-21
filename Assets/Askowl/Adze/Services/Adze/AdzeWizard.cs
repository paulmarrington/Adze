using System;
using System.IO;
using Askowl;
using UnityEditor;
using UnityEngine;

namespace Decoupler.Services {
  [CreateAssetMenu(menuName = "Decoupled/Adze/Concrete Service Wizard", fileName = "AdzeWizard")]
  public class AdzeWizard : AssetWizard {
    private static AdzeWizard newServiceInputForm;

    [MenuItem("Assets/Create/Decoupled/Adze/Add Concrete Service")]
    internal static void Start() {
      if (newServiceInputForm == null) newServiceInputForm = CreateInstance<AdzeWizard>();
      Selection.activeObject = newServiceInputForm;
    }

    [SerializeField] private string newAdzeName;

    public override void Clear() => newAdzeName = "";

    public override void Create() {
      type  = "Decoupler";
      Label = "BuildNewService";
      CreateAssets(assetName: "Adze", assetType: "Decoupler", basePath: "Assets/Askowl/Adze/Services/Adze");
      BuildAssets.Display();
    }
    protected override void ProcessAllFiles(string textAssetTypes) {
      var serviceFor = $"Assets/Askowl/Adze/Services/Adze/AdzeServiceFor";
      var fileName   = $"{destination}/AdzeServiceFor{newAdzeName}.cs";
      if (File.Exists(fileName)) throw new Exception($"'{fileName}' already exists. Try another name.");
      File.Copy($"{serviceFor}.cs", fileName);
      ProcessFiles("cs", fileName);
    }

    protected override string FillTemplate(Template template, string text) =>
      template.From(text)
              .Substitute("_ConcreteService_", newAdzeName)
              .And(@"/\*\+\+", "").And(@"\+\+\*/", "").Result();
  }
}