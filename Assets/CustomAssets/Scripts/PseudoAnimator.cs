using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PseudoAnimator : MonoBehaviour
{
    public Sprite[] sprites;

    Image image;
    WaitForSeconds wait;
    [SerializeField]
    float animSpeed = .5f; // how many seconds between each image
    int i = 0;

    void Awake()
    {
        image = GetComponent<Image>();
        wait = new WaitForSeconds(animSpeed);
    }

    public void changeImage()
    {
        image.sprite = sprites[i];
        i++;
        if (i >= sprites.Length) i = 0;
    }

    public void changeImage(int k)
    {
        image.sprite = sprites[k];
    }

    public IEnumerator pseudoAnim()
    {
        while (true)
        {
        changeImage();
        yield return wait;
        }
    }

    public void StopAnim()
    {
        StopAllCoroutines();
    }
    public void StartAnim()
    {
        StartCoroutine(pseudoAnim());
    }
}