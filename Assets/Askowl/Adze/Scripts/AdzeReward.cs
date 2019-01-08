using System;
using System.Collections;
using Askowl;
using CustomAsset.Constant;
using JetBrains.Annotations;
using UnityEngine;

namespace Adze {
  /// <a href=""></a> //#TBD#//
  [CreateAssetMenu(menuName = "Adze/Rewarded", fileName = "AdzeReward")]
  public sealed class AdzeReward : OfType<AdzeReward> {
    /// <a href=""></a> //#TBD#//
    [Serializable] public struct Prompt {
      [TextArea, SerializeField] internal string message;
      [SerializeField]           internal string acceptButton;
      [SerializeField]           internal string refuseButton;
    }

    [SerializeField] private Prompt[]        prompts, thanks;
    [SerializeField] private int             adsShownBetweenQuotes = 2;
    [SerializeField] private AdzeDistributor distributor;
    [SerializeField] private Quotes          quotes, facts;

    private Selector<Prompt> prompt, thank;
    private int              count;

    private readonly Log.EventRecorder log   = Log.Events("Content");
    private readonly Log.EventRecorder error = Log.Errors();

    /// <a href=""></a> //#TBD#//
    [HideInInspector] public bool adWatched, adRequested;

    /// <a href=""></a> //#TBD#//
    public AdzeDistributor Distributor => distributor;

    /// <a href=""></a> //#TBD#//
    public new static AdzeReward Instance(string assetName) => Instance<AdzeReward>(assetName);

    /// <a href=""></a> //#TBD#//
    protected override void OnEnable() {
      prompt = new Selector<Prompt> {Choices = prompts};
      thank  = new Selector<Prompt> {Choices = thanks};
      count  = 0;
    }

    private IEnumerator ShowDialog(Dialog dialog, Pick<Prompt> prompter) {
      Prompt dialogContent = prompter.Pick();

      return dialog.Activate(
        dialogContent.message,
        dialogContent.refuseButton,
        dialogContent.acceptButton);
    }

    private IEnumerator ShowQuote([NotNull] Dialog dialog) =>
      dialog.Activate(quotes.Pick(), "", thank.Pick().acceptButton);

    // ReSharper disable once UnusedMethodReturnValue.Local
    private IEnumerator ShowFact([NotNull] Dialog dialog) => dialog.Activate(text: facts.Pick());

    /// <a href=""></a> //#TBD#//
    public IEnumerator Show(string location = "Default") {
      /*
       * When I try and cache dialog loaded in OnEnable, the reference becomes destroyed.
       * Peculiar since it has the same ID. Probably something to do with it being a prefab.
       * The solution/workaround I chose was to find it when I need it.
       */
      Dialog dialog           = Dialog.Instance(gameObjectName: "AdzeReward");
      adWatched = adRequested = false;
      if (dialog == null) yield break;

      yield return ShowDialog(dialog: dialog, prompter: prompt);

      if (dialog.Action.Equals(value: "Yes")) {
        adRequested = true;

        if (((++count % (adsShownBetweenQuotes + 1)) == 0) && (quotes.Count > 0)) {
          yield return ShowQuote(dialog);

          log("Quote");
        } else if (distributor == null) {
          yield return ShowQuote(dialog);

          error("Advertisement server not set");
        } else {
          yield return ShowFact(dialog); // don't wait
          yield return distributor.Show(mode: Mode.Reward, location: location);

          adWatched = distributor.AdShown;
          log(
            adWatched         ? "Watched" :
            distributor.Error ? "Distributor Error" : "Skipped");

          if (adWatched || distributor.Error) {
            yield return ShowDialog(dialog: dialog, prompter: thank);
          }
        }
      } else {
        log("Rejected");
      }
    }
  }
}