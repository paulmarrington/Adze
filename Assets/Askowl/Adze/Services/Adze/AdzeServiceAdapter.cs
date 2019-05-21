using Askowl;
using UnityEngine;

namespace Decoupler.Services {
  public abstract class AdzeServiceAdapter : Services<AdzeServiceAdapter, AdzeContext>.ServiceAdapter {
    #region Service Support
    // Code that is common to all services belongs here
    protected override void Prepare() { }
    #endregion

    #region Public Interface
    // Methods calling code will use to call a service - over and above concrete interface methods below ones defined below.
    #endregion

    #region Service Entry Points
    // List of virtual interface methods that all concrete service adapters need to implement.

    /************* Show *************/
    public class ShowDto : DelayedCache<ShowDto> {
      public int /*-entryPointRequest-*/                                request;
      public (bool addActionTaken, bool dismissed, string serviceError) response;
    }
    public abstract Emitter Call(Service<ShowDto> service);
    public virtual void Succeed(Service<ShowDto> service) {
      if (service.ErrorMessage != default) {
        if (!string.IsNullOrWhiteSpace(service.ErrorMessage)) Error(service.ErrorMessage);
      } else if (service.Dto.response.dismissed)
        Log("Dismissed", "By Player");
      else
        Log("Action", service.Dto.response.addActionTaken ? "Taken" : "Not Taken");

      service.Dto.response = service.Dto.response;
      service.Succeed();
    }
    #endregion
  }
}