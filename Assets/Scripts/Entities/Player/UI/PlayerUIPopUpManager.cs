using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading;

public class PlayerUIPopUpManager : MonoBehaviour
{
    [Header("YOU DIED Pop-Up")]
    [SerializeField] GameObject youDiedPopUpGameObject;
    [SerializeField] TextMeshProUGUI youDiedPopUpBackgroundText;
    [SerializeField] TextMeshProUGUI youDiedPopUpText;
    
    //Allows us to set the alpha to fade over time
    [SerializeField] CanvasGroup youDiedPopUpCanvasGroup;   

    //This function can be repeated with other kinds of pop ups such as "Great Enemy/Boss Defeated", "Save Point Unlocked", etc.
    public void SendYouDiedPopUp() {
        //Activate Post-Processing Effects here e.g. black/white shader, blur, etc

        //Turn on the PopUp object so it's visible
        youDiedPopUpGameObject.SetActive(true);

        //Initialize Spacing incase the player has died previously this play session
        youDiedPopUpBackgroundText.characterSpacing = 0;

        //Stretch out the Pop-Up
        StartCoroutine(StretchPopUpTextOverTime(youDiedPopUpBackgroundText, 8f, 19f));

        //Fade in the Pop-Up
        StartCoroutine(FadeInPopUpOverTime(youDiedPopUpCanvasGroup, 5f));

        //Wait, then fade out the Pop-Up
        StartCoroutine(WaitThenFadeOutPopUpOverTime(youDiedPopUpCanvasGroup, 2f, 5f));

    }

    private IEnumerator StretchPopUpTextOverTime(TextMeshProUGUI text, float duration, float stretchAmount) {
        if (duration > 0f) {
            //Resets Character Spacing
            text.characterSpacing = 0;
            float timer = 0f;

            yield return null;  

            while (timer < duration) {
                timer = timer + Time.deltaTime;
                text.characterSpacing = Mathf.Lerp(text.characterSpacing, stretchAmount, duration * (Time.deltaTime / 20));

                yield return null;
            }
        }
    }

    private IEnumerator FadeInPopUpOverTime(CanvasGroup canvas, float duration) {
        if (duration > 0) {
            canvas.alpha = 0;
            float timer = 0;

            yield return null;

            while (timer < duration) {
                timer = timer + Time.deltaTime;
                canvas.alpha = Mathf.Lerp(canvas.alpha, 1, duration * Time.deltaTime);

                yield return null;
            }
        }

        canvas.alpha = 1;
        yield return null;
    }

    private IEnumerator WaitThenFadeOutPopUpOverTime(CanvasGroup canvas, float duration, float delay) {
        if (duration > 0) {
            while (delay > 0) {
                delay = delay - Time.deltaTime;
                yield return null;
            }

            canvas.alpha = 1;
            float timer = 0;

            yield return null;

            while (timer < duration) {
                timer = timer + Time.deltaTime;
                canvas.alpha = Mathf.Lerp(canvas.alpha, 0, duration * Time.deltaTime);

                yield return null;
            }
        }

        canvas.alpha = 0;

        yield return null;
    }

}
