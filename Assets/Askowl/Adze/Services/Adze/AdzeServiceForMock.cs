using Askowl;
using UnityEngine;

namespace Decoupler.Services {
  [CreateAssetMenu(menuName = "Decoupled/Adze/ServiceForMock", fileName = "AdzeServiceForMock")]
  public class AdzeServiceForMock : AdzeServiceAdapter {
    [SerializeField] private bool   addActionTaken = default;
    [SerializeField] private bool   dismissed      = default;
    [SerializeField] private string serviceError   = default;

    public override Emitter Call(Service<ShowDto> service) {
      Debug.Log($"*** Mock Call '{GetType().Name}' '{typeof(ShowDto).Name}', '{service.Dto.request}");
      service.Dto.response = (addActionTaken, dismissed, serviceError);
      return null;
    }

    public override bool IsExternalServiceAvailable() => true;
  }
}