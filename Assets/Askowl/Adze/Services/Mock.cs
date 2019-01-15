//// Copyright 2019 (C) paul@marrington.net http://www.askowl.net/unity-packages
//
//using Decoupled.Adze;
//using UnityEngine;
//
//namespace Askowl.Adze {
//  /// <a href=""></a> //#TBD#//
//  [CreateAssetMenu(menuName = "Adze/Mock", fileName = "AdzeMock")]
//  public class Mock : Service {
//    [SerializeField] private bool failOnDisplay   = default;
//    [SerializeField] private bool adActionIsTaken = default;
//    [SerializeField] private bool adDismissed     = default;
//    [SerializeField] private bool adServiceError  = default;
//
//    private Fiber fiber;
//
//    /// <inheritdoc />
//    protected override void Initialise() {
//      base.Initialise();
//      Log("Initialise", $"platform={platform}, mode={mode}, key={key}, signature={signature}, location={location}");
//      fiber = Fiber.Instance.WaitFor(seconds: 0.2f).Do(Displayed);
//    }
//
//    /// <inheritdoc />
//    protected override bool Display() {
//      Log("Display", $"{failOnDisplay}");
//      fiber.Go();
//      return failOnDisplay;
//    }
//    private int counter;
//
//    private void Displayed(Fiber _) {
//      AdActionTaken = adActionIsTaken;
//      Dismissed     = adDismissed;
//      ServiceError  = adServiceError;
//      Emitter.Fire();
//    }
//  }
//}