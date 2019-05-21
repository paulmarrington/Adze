using System;
using Askowl;
using UnityEditor;
using UnityEngine; // Do not remove
#if AdzeServiceFor_ConcreteService_
// Add using statements for service library here
#endif

namespace Decoupler.Services {
  /*++[CreateAssetMenu(
    menuName = "Decoupled/Adze/ServiceFor_ConcreteService_", fileName = "AdzeServiceFor_ConcreteService_")]++*/
  public class AdzeServiceFor_ConcreteService_ : AdzeServiceAdapter {
    [InitializeOnLoadMethod] private static void DetectService() {
      bool usable = DefineSymbols.HasPackage("_ConcreteService_") || DefineSymbols.HasFolder("_ConcreteService_");
      DefineSymbols.AddOrRemoveDefines(addDefines: usable, named: "AdzeServiceFor_ConcreteService_");
    }

    #region Service Entry Points
    #if AdzeServiceFor_ConcreteService_
    protected override void Prepare() { base.Prepare(); }

    public override Emitter Call(Service<ShowDto> service) => ShowFiber.Go(service).OnComplete;

    private class ShowFiber : Fiber.Closure<ShowFiber, Service<ShowDto>> {
      protected override void Activities(Fiber fiber) =>
        Emitter callExternalService(Fiber _) { // Could return an inner Fiber if needed
          var emitter = Emitter.SingleFireInstance;
          // Use Scope.Dto.request and fire emitter when a response has been received
          return emitter;
        }
        fiber.WaitFor(callExternalService).Fire(Scope.Emitter);
    } /** or use service.Emitter && service.Emitter.Fire() if another fiber is not needed **/

    public override  void    OnResponse(Service<ShowDto> service) { base.OnResponse(service); }
    #endif
    #endregion

    #region Service Entry Points
    #if !AdzeServiceFor_ConcreteService_

    public override Emitter Call(Service<ShowDto> service) => throw new NotImplementedException("ShowDto");

    #endif
    #endregion

    // One service override per service method
    // Access the external service here. Save and call dti.Emitter.Fire when service call completes
    // or set dto.ErrorMessage if the service call fails to initialise

    #region Compiler Definition
    #if AdzeServiceFor_ConcreteService_
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void RegisterService() { }
    public override bool IsExternalServiceAvailable() => true;
    #else
    public override bool IsExternalServiceAvailable() => false;
    #endif
    #endregion
  }
}