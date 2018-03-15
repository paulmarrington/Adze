using System.Collections;
using UnityEngine;
using System;

namespace Adze {
  using JetBrains.Annotations;

  [CreateAssetMenu(menuName = "Adze/Rewarded", fileName = "Reward")]
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

    private Selector<Prompt>            prompt, thank;
    private Quotes                      quotes;
    private int                         count;
    private Decoupled.Analytics.GameLog log;

    [HideInInspector] public bool AdWatched, AdRequested;

    [UsedImplicitly]
    public new static AdzeReward Asset(string assetName) {
      return CustomAsset<AdzeReward>.Asset(name: assetName);
    }

    public void OnEnable() {
      log    = Decoupled.Analytics.GameLog.Instance;
      prompt = new Selector<Prompt>(choices: Prompts);
      thank  = new Selector<Prompt>(choices: Thanks);
      quotes = new Quotes();
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

    [UsedImplicitly]
    public IEnumerator Show(string location = "Default") {
      /*
       * When I try and cache dialog loaded in OnEnable, the reference becomes destroyed.
       * Peculiar since it has the same ID. Probably something to do with it being a prefab.
       * The solution/workaround I chose was to find it when I need it.
       */
      Dialog dialog           = Dialog.Instance(gameObjectName: "Reward");
      AdWatched = AdRequested = false;
      if (dialog == null) yield break;

      yield return ShowDialog(dialog: dialog, prompter: prompt);

      if (dialog.Action.Equals(value: "Yes")) {
        AdRequested = true;

        if ((++count % (AdsShownBetweenQuotes + 1)) == 0) {
          yield return ShowQuote(dialog: dialog);

          log.Event(name: "Advertisement", action: "Content", result: "Quote displayed");
        } else if (Distributor == null) {
          yield return ShowQuote(dialog: dialog);

          log.Error("Advertisement server not set");
        } else {
          yield return Distributor.Show(mode: Mode.Reward, location: location);

          AdWatched = Distributor.AdActionTaken;

          if (AdWatched) {
            yield return ShowDialog(dialog: dialog, prompter: thank);
          } else {
            yield return ShowQuote(dialog: dialog);
          }
        }
      } else {
        log.Event("Advertisement", "Content", "Ad not watched by player");
      }
    }
  }
}