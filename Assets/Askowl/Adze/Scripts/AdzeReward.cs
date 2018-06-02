namespace Adze {
  using System;
  using System.Collections;
  using Askowl;
  using Decoupled;
  using JetBrains.Annotations;
  using UnityEngine;

  [CreateAssetMenu(menuName = "Adze/Rewarded", fileName = "AdzeReward")]
  public sealed class AdzeReward : CustomAsset<AdzeReward> {
    [Serializable]
    public struct Prompt {
      // ReSharper disable MemberCanBeInternal
      // ReSharper disable UnassignedField.Global
      [TextArea] public string Message;

      public string AcceptButton;

      public string RefuseButton;
      // ReSharper restore UnassignedField.Global
      // ReSharper restore MemberCanBeInternal
    }

    public Prompt[]        Prompts, Thanks;
    public int             AdsShownBetweenQuotes = 2;
    public AdzeDistributor Distributor;

    private Selector<Prompt> prompt, thank;
    private Quotes           quotes, facts;
    private int              count;

    private Analytics log;

    [HideInInspector] public bool AdWatched, AdRequested;

    
    public new static AdzeReward Asset(string assetName) { return Asset(name: assetName); }

    public void OnEnable() {
      log    = Analytics.Instance;
      prompt = new Selector<Prompt>(choices: Prompts);
      thank  = new Selector<Prompt>(choices: Thanks);
      quotes = new Quotes();
      facts  = new Quotes("facts");
      count  = 0;
    }

    private IEnumerator ShowDialog([NotNull] Dialog dialog, [NotNull] IPick<Prompt> prompter) {
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

        if (((++count % (AdsShownBetweenQuotes + 1)) == 0) && (quotes.Length > 0)) {
          yield return ShowQuote(dialog);

          log.Event(name: "Advertisement", action: "Content", result: "Quote displayed");
        } else if (Distributor == null) {
          yield return ShowQuote(dialog);

          log.Error(name: "Adze", message: "Advertisement server not set");
        } else {
          // ReSharper disable once MustUseReturnValue
          ShowFact(dialog); // don't wait
          yield return Distributor.Show(mode: Mode.Reward, location: location);

          AdWatched = Distributor.AdShown;

          if (AdWatched || Distributor.Error) {
            yield return ShowDialog(dialog: dialog, prompter: thank);
          }
        }
      } else {
        log.Event(name: "Advertisement", action: "Content", result: "Ad not watched by player");
      }
    }
  }
}