// Copyright 2019 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using System.Collections;
using CustomAsset;
using CustomAsset.Constant;
using JetBrains.Annotations;
using UnityEngine;

namespace Askowl.Adze {
  /// <a href=""></a> //#TBD#//
  [CreateAssetMenu(menuName = "Adze/Rewarded", fileName = "AdzeReward")]
  public sealed class AdzeReward : Manager {
    /// <a href=""></a> //#TBD#//
    [Serializable] public class Prompt {
      [TextArea, SerializeField] internal string message      = default;
      [SerializeField]           internal string acceptButton = default;
      [SerializeField]           internal string refuseButton = default;
    }

    [SerializeField] private Prompt[]        prompts               = default, thanks = default;
    [SerializeField] private int             adsShownBetweenQuotes = 2;
    [SerializeField] private AdzeDistributor distributor           = default;
    [SerializeField] private Quotes          quotes                = default, facts = default;

    private Selector<Prompt> prompt, thank;
    private int              count;

    private Log.EventRecorder log;
    private Log.EventRecorder error;

    /// <a href=""></a> //#TBD#//
    [HideInInspector] public bool adWatched, adRequested;

    /// <a href=""></a> //#TBD#//
    public AdzeDistributor Distributor => distributor;

    /// <a href=""></a> //#TBD#//
    protected override void OnEnable() {
      base.OnEnable();
      log    = Log.Events("Content");
      error  = Log.Errors();
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