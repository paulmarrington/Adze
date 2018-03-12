namespace Adze {
  using System.Collections;
  using UnityEngine;
  using UnityEngine.Advertisements;

  [CreateAssetMenu(menuName = "Adze/Unity", fileName = "AdzeUnity")]
  public sealed class AdzeUnity : AdzeServer {
    private bool        complete;
    private ShowOptions options;

    protected override void Initialise() {
      Advertisement.Initialize(gameId: AppKey);

      options = new ShowOptions {resultCallback = HandleShowResult};
    }

    protected override IEnumerator ShowNow(string location) {
      while (!Ready) yield return null;

      complete = Error = false;

      Advertisement.Show(placementId: location, showOptions: options);

      yield return WaitForResponse();
    }

    private static bool Ready {
      get { return Advertisement.isInitialized && Advertisement.IsReady(); }
    }

    private void HandleShowResult(ShowResult result) {
      switch (result) {
        case ShowResult.Failed:
          Error = true;
          break;
        case ShowResult.Skipped:
          AdActionTaken = false;
          break;
        case ShowResult.Finished:
          AdActionTaken = true;
          break;
      }

      complete = true;
    }
  }
}