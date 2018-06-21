using CustomAsset.Constant;

namespace Adze {
  using System;
  using System.Collections;
  using Askowl;
  using Decoupled;
  using JetBrains.Annotations;
  using UnityEngine;

  [CreateAssetMenu(menuName = "Adze/Rewarded", fileName = "AdzeReward")]
  public sealed class AdzeReward : CustomAsset.Constant.OfType<AdzeReward> {
    [Serializable]
    public struct Prompt {
      [TextArea, SerializeField] internal string Message;

      [SerializeField] internal string AcceptButton;

      [SerializeField] internal string RefuseButton;
    }

    [SerializeField] private Prompt[]        prompts, thanks;
    [SerializeField] private int             adsShownBetweenQuotes = 2;
    [SerializeField] private AdzeDistributor distributor;
    [SerializeField] private Quotes          quotes, facts;

    private Selector<Prompt> prompt, thank;
    private int              count;

    private Analytics log;

    [HideInInspector] public bool AdWatched, AdRequested;

    public AdzeDistributor Distributor { get { return distributor; } }

    public static AdzeReward Instance(string assetName) { return Instance<AdzeReward>(assetName); }

    public void OnEnable() {
      log    = Analytics.Instance;
      prompt = new Selector<Prompt>(choices: prompts);
      thank  = new Selector<Prompt>(choices: thanks);
      count  = 0;
    }

    private IEnumerator ShowDialog(Dialog dialog, Pick<Prompt> prompter) {
      Prompt dialogContent = prompter.Pick();

      return dialog.Activate(
        dialogContent.Message,
        dialogContent.RefuseButton,
        dialogContent.AcceptButton);
    }

    private IEnumerator ShowQuote([NotNull] Dialog dialog) {
      return dialog.Activate(quotes.Pick(), "", thank.Pick().AcceptButton);
    }

    // ReSharper disable once UnusedMethodReturnValue.Local
    private IEnumerator ShowFact([NotNull] Dialog dialog) {
      return dialog.Activate(text: facts.Pick());
    }

    public IEnumerator Show(string location = "Default") {
      /*
       * When I try and cache dialog loaded in OnEnable, the reference becomes destroyed.
       * Peculiar since it has the same ID. Probably something to do with it being a prefab.
       * The solution/workaround I chose was to find it when I need it.
       */
      Dialog dialog           = Dialog.Instance(gameObjectName: "AdzeReward");
      AdWatched = AdRequested = false;
      if (dialog == null) yield break;

      yield return ShowDialog(dialog: dialog, prompter: prompt);

      if (dialog.Action.Equals(value: "Yes")) {
        AdRequested = true;

        if (((++count % (adsShownBetweenQuotes + 1)) == 0) && (quotes.Count > 0)) {
          yield return ShowQuote(dialog);

          log.Event(name: "Advertisement", action: "Content", result: "Quote displayed");
        } else if (distributor == null) {
          yield return ShowQuote(dialog);

          log.Error(name: "Adze", message: "Advertisement server not set");
        } else {
          // ReSharper disable once MustUseReturnValue
          ShowFact(dialog); // don't wait
          yield return distributor.Show(mode: Mode.Reward, location: location);

          AdWatched = distributor.AdShown;

          if (AdWatched || distributor.Error) {
            yield return ShowDialog(dialog: dialog, prompter: thank);
          }
        }
      } else {
        log.Event(name: "Advertisement", action: "Content", result: "Ad not watched by player");
      }
    }
  }
}