using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PseudoAnimator : MonoBehaviour
{
    public Sprite[] sprites;
    public PseudoAnimator[] tutoScripts;
    private int activeTutoScreen = 0;

    Image image;
    WaitForSeconds wait;
    float animSpeed = 1; // how many seconds between each image
    int i = 0;

    void Awake()
    {
        image = GetComponent<Image>();
        wait = new WaitForSeconds(animSpeed);
    }

    private IEnumerator animLoop()
    { // changes the image every animSpeed seconds, creating some kind of pseudo animation
        while (true)
        {
            image.sprite = sprites[i];
            i++;
            if (i >= sprites.Length)
            {
                i = 0;
            }
            yield return wait;
        }
    }
    
    public void Toggle (int value)
    {
        tutoScripts[activeTutoScreen].StopAnim();
        activeTutoScreen += value;
        tutoScripts[activeTutoScreen].StartAnim();
    }

    public void StartAnim()
    {
        image.enabled = true;
        StartCoroutine(animLoop());
    }

    public void StopAnim()
    {
        image.enabled = false;
        StopAllCoroutines();
    }
}