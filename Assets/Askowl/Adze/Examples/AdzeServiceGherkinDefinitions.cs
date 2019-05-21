// Copyright 2019 (C) paul@marrington.net http://www.askowl.net/unity-packages
#if !ExcludeAskowlTests

using System;
using System.Collections;
using Askowl.Gherkin;
using Decoupler.Services;
using NUnit.Framework;
using UnityEngine.TestTools;
// ReSharper disable MissingXmlDoc

namespace Askowl.Adze.Examples {
  [Serializable] public class AdzeServiceGherkinDefinitions : PlayModeTests {
    private string              serviceDirectory = "Assets/Askowl/Adze/Services";
    private AssetDb             assetDb          = AssetDb.Instance;
    private AssetEditor         assetEditor      = AssetEditor.Instance;
    private AdzeServicesManager adzeServicesManager;
    private string[]            mocks;

    private IEnumerator ServiceTest(string label) {
      yield return Feature.Go("AdzeServiceGherkinDefinitions", featureFile: "AdzeService", label).AsCoroutine();
    }

    [UnityTest] public IEnumerator RoundRobinAllPass() { yield return ServiceTest("@RoundRobinAllPass"); }

    [Step(@"^(\d+) services available$")] public void ServicesAvailable(string[] matches) {
      int available = int.Parse(matches[0]);
      Assert.IsTrue(available < 10, "Maximum of 10 mock services allowed");
      assetEditor.Load("AdzeServicesManager", "Decoupler.Services", serviceDirectory);
      adzeServicesManager = assetEditor.Asset("AdzeServicesManager") as AdzeServicesManager;
      Assert.NotNull(adzeServicesManager);
      string mockIndexes = "";
      using (var list = Fifo<string>.Instance) {
        foreach (AdzeServiceAdapter service in adzeServicesManager.ServiceList) {
          var mock = service as AdzeServiceForMock;
          if (mock != null) {
            var name = mock.GetType().Name;
            list.Push(name);
            assetEditor.Load(name, "Decoupler.Services", serviceDirectory);
            mockIndexes += name[name.Length - 1];
          }
        }
        for (int i = 1; i <= available; i++) {
          if (!mockIndexes.Contains(i.ToString())) {
            var assetName = $"AdzeServiceForMock {i}";
            assetEditor.Load(assetName, "Decoupler.Services", serviceDirectory);
            list.Push(assetName);
            assetEditor.InsertIntoArrayField("AdzeServicesManager", "services", assetName);
          }
        }
        mocks = list.ToArray();
      }
    }
    [Step(@"^they are ordered  as ""(.*)""$")] public void ServiceOrder(string[] matches) {
      assetEditor.SetField("AdzeServicesManager", "order", matches[0]);
      var serialized = assetEditor.SerialisedAsset("AdzeServicesManager").FindProperty("order");
      var option     = matches[0];
      var options    = serialized.enumNames;
      for (int i = 0; i < options.Length; i++) {
        if (options[i] == option) {
          serialized.enumValueIndex = i;
          return;
        }
      }
      Assert.Fail($"No option '{option}'");
    }
    [Step(@"^that the all services work$")] public void AllWork() {
      foreach (var mock in mocks) {
        assetEditor.SerialisedAsset(mock).FindProperty("addActionTaken").stringValue = null;
      }
      assetEditor.Save();
    }
    [Step(@"^I ask for an advertisement$")] public Emitter AskForAdvertisement() {
      var show = Service<AdzeServiceAdapter.ShowDto>.Instance;
      return Fiber.Start.WaitFor(_ => adzeServicesManager.CallService(show)).OnComplete;
    }
    [Step(@"^I get the service (\d+)$")] public void WhichService(string[] matches) {
      int expected = int.Parse(matches[0]);
      Assert.AreEqual(expected, adzeServicesManager.selector.CycleIndex);
    }
  }
}
#endif