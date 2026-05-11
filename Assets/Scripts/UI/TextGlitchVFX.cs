using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TextGlitchVFX : MonoBehaviour
{
    public TMP_Text mainText;
    
    [Header("Settings")]
    public float glitchDuration = 0.25f;
    public float jitterIntensity = 4.0f;
    public string glitchChars = "01!@#$%^&*";

    [Header("Chromatic Aberration")]
    public bool useChromaticAberration = true;
    public Color colorR = new Color(1, 0, 0, 0.5f); // Ghost Red
    public Color colorB = new Color(0, 1, 1, 0.5f); // Ghost Cyan

    private string originalString;
    private Color originalColor;
    private TMP_Text ghostR;
    private TMP_Text ghostB;

    void Start()
    {
        if (mainText == null) mainText = GetComponent<TMP_Text>();
        originalString = mainText.text;
        originalColor = mainText.color;

        if (useChromaticAberration) CreateGhostTexts();
    }

    void CreateGhostTexts()
    {
        // Spawns hidden copies of the text for the RGB split effect
        ghostR = Instantiate(mainText, mainText.transform.parent);
        ghostB = Instantiate(mainText, mainText.transform.parent);

        // Remove this script from clones to avoid infinite loops
        Destroy(ghostR.GetComponent<TextGlitchVFX>());
        Destroy(ghostB.GetComponent<TextGlitchVFX>());

        ghostR.color = colorR;
        ghostB.color = colorB;
        
        ghostR.gameObject.SetActive(false);
        ghostB.gameObject.SetActive(false);
    }

    public void TriggerGlitch()
    {
        if (this.gameObject.activeInHierarchy)
        {
            StopAllCoroutines();
            StartCoroutine(GlitchSequence());
        }
    }

    IEnumerator GlitchSequence()
    {
        float elapsed = 0f;
        if(useChromaticAberration) { ghostR.gameObject.SetActive(true); ghostB.gameObject.SetActive(true); }

        while (elapsed < glitchDuration)
        {
            //1 Scramle Text Chunks
            char[] currentChars = originalString.ToCharArray();
            for (int i = 0; i < Random.Range(1, 3); i++)
                currentChars[Random.Range(0, currentChars.Length)] = glitchChars[Random.Range(0, glitchChars.Length)];
            
            string scrambled = new string(currentChars);
            mainText.text = scrambled;
            if(useChromaticAberration) { ghostR.text = scrambled; ghostB.text = scrambled; }

            //2 Vertex Jitter and Chromatic Split
            ApplyJitter(mainText, jitterIntensity);
            if(useChromaticAberration) 
            {
                ApplyJitter(ghostR, jitterIntensity * 1.5f); // Shift further for "split" look
                ApplyJitter(ghostB, jitterIntensity * 0.8f);
            }

            elapsed += 0.04f;
            yield return new WaitForSeconds(0.04f);
        }

        //Reset
        ResetText(mainText);
        if(useChromaticAberration) { ghostR.gameObject.SetActive(false); ghostB.gameObject.SetActive(false); }
    }

    void ApplyJitter(TMP_Text textComp, float intensity)
    {
        textComp.ForceMeshUpdate();
        TMP_TextInfo textInfo = textComp.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;
            int meshIndex = textInfo.characterInfo[i].materialReferenceIndex;
            int vIndex = textInfo.characterInfo[i].vertexIndex;
            Vector3[] vertices = textInfo.meshInfo[meshIndex].vertices;

            Vector3 offset = new Vector3(Random.Range(-intensity, intensity), Random.Range(-intensity, intensity), 0);
            for (int j = 0; j < 4; j++) vertices[vIndex + j] += offset;
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            textComp.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }

    void ResetText(TMP_Text textComp)
    {
        textComp.text = originalString;
        textComp.color = originalColor;
        textComp.ForceMeshUpdate();
    }
}
