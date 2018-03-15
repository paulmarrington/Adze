namespace Adze {
  using UnityEngine;
  using UnityEngine.Advertisements;

  [CreateAssetMenu(menuName = "Adze/Unity", fileName = "AdzeUnity")]
  public sealed class AdzeUnity : AdzeServer {
    private ShowOptions options;

    protected override void Initialise() {
      Advertisement.Initialize(gameId: AppKey);

      options = new ShowOptions {resultCallback = HandleShowResult};
    }

    protected override bool ShowNow() {
      Advertisement.Show(placementId: Location, showOptions: options);
      return true;
    }

    protected override bool Loaded {
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

      Dismissed = true;
    }
  }
}