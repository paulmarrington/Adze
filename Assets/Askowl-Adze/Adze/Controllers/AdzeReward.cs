﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Adze {

  [CreateAssetMenu(menuName = "Advertisements/Rewarded", fileName = "Reward")]
  public class AdzeReward : CustomAsset<Reward> {

    [Serializable]
    public struct Prompt {
      [TextArea]
      public string message;
      public string acceptButton;
      public string refuseButton;
    }

    public Prompt[] prompts, thanks;
    public int adsShownBetweenQuotes = 2;
    public string advertisementDistributor = "Distributor";

    private Advertisement advertisement;
    private Selector<Prompt> prompt, thank;
    private Quotes quotes;
    private int count;
    Decoupled.Analytics.Play analytics;


    [HideInInspector]
    public bool adWatched, adRequested;

    public static Reward Asset() {
      return CustomAsset<Reward>.Asset("Advertisements/Reward");
    }

    /*
     * When I try and cache dialog loaded in OnEnable, the reference becomes destroyed.
     * Peculiar since it has the same ID. Probably something to do with it being a prefab.
     * The solution/workaround I chose was to find it when I need it.
     */
    Dialog getDialog() {
      GameObject go = GameObject.Find("Reward");
      if (go == null) {
        Debug.LogError("Drag Marrington/Ads/Reward prefab into the scene");
      }
      return go.GetComponent<Dialog>();
    }

    public void OnEnable() {
      analytics = Decoupled.Analytics.Play.Instance;
      advertisement = Advertisement.Asset(advertisementDistributor);
      prompt = new Selector<Prompt> (prompts);
      thank = new Selector<Prompt> (thanks);
      quotes = new Quotes ();
      count = 0;
    }

    private IEnumerator showDialog(Dialog dialog, Pick<Prompt> prompter) {
      Prompt prompt = prompter.Pick();
      return dialog.Activate(
        prompt.message,
        prompt.refuseButton,
        prompt.acceptButton);
    }

    private IEnumerator showQuote(Dialog dialog) {
      return dialog.Activate(quotes.Pick(), "", thank.Pick().acceptButton);
    }


    public IEnumerator Show() {
      Dialog dialog = getDialog();
      adWatched = adRequested = false;
      yield return showDialog(dialog, prompt);
      if (dialog.action.Equals(("Yes"))) {
        adRequested = true;
        if ((++count % (adsShownBetweenQuotes + 1)) == 0) {
          yield return showQuote(dialog);
          analytics.Event("Advertisement", "Content", "Quote displayed");
        } else if (advertisement == null) {
          yield return showQuote(dialog);
          analytics.Error("Advertisement server not set");
        } else {
          yield return advertisement.Show(Mode.Reward);
          if (adWatched = advertisement.adShown) {
            yield return showDialog(dialog, thank);
          } else {
            yield return showQuote(dialog);
          }
        }
      } else {
        analytics.Event("Advertisement", "Content", "Ad not watched by player");
      }
    }
  }
}