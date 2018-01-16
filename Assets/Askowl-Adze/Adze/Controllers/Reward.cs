using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Ads {

  [CreateAssetMenu(menuName = "Advertisements/Rewarded", fileName = "Reward")]
  public class Reward : CustomAsset<Reward> {

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
    private Randomiser<Prompt> prompt, thank;
    private Quotes quotes;
    private int count;

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

    public override void OnEnable() {
      advertisement = Advertisement.Asset(advertisementDistributor);
      prompt = new Randomiser<Prompt> (prompts);
      thank = new Randomiser<Prompt> (thanks);
      quotes = Quotes.Singleton();
      count = 0;
    }

    private IEnumerator showDialog(Dialog dialog, Randomiser<Prompt> prompter) {
      Prompt prompt = prompter.Pick();
      return dialog.Activate(
        prompt.message,
        prompt.refuseButton,
        prompt.acceptButton);
    }

    private IEnumerator showQuote(Dialog dialog) {
      return dialog.Activate(quotes.PickRTF(), "", thank.Pick().acceptButton);
    }


    public IEnumerator Show() {
      Dialog dialog = getDialog();
      adWatched = adRequested = false;
      yield return showDialog(dialog, prompt);
      if (dialog.action.Equals(("Yes"))) {
        adRequested = true;
        if ((++count % (adsShownBetweenQuotes + 1)) == 0) {
          yield return showQuote(dialog);
          Answers.LogContentView("Advertisement", "Quote displayed");
        } else if (advertisement == null) {
          yield return showQuote(dialog);
          Answers.LogContentView("Advertisement", "Ad server not set");
        } else {
          yield return advertisement.Show(Mode.reward);
          if (adWatched = advertisement.adShown) {
            yield return showDialog(dialog, thank);
          } else {
            yield return showQuote(dialog);
          }
        }
      } else {
        Answers.LogContentView("Advertisement", "Ad not watched by player");
      }
    }
  }
}