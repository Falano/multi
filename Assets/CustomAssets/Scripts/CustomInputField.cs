using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomInputField : MonoBehaviour
{

    Text tx;
    Image img;
    Button butt;
    string defaultText = "<i>Enter your name here</i>";
    public Color txPendingCol = Color.grey;
    Color txActiveCol = Color.white;
    public Color bgPendingCol = Color.red;
    public Color bgEmptyCol = Color.red;
    Color bgActiveCol = Color.black;
    bool isInputActive = false;
    bool clickedButton = false;
    int maxLength = 26;
    IEnumerator waitTillKeyPressed = new WaitUntil(() => Input.anyKeyDown);

    void Start()
    {
        tx = GetComponentInChildren<Text>();
        img = GetComponentInChildren<Image>();
        butt = GetComponentInChildren<Button>();
        tx.text = defaultText;
        tx.color = txPendingCol;
        img.color = bgPendingCol;

        resetInputField();
        if (PlayerPrefs.HasKey("playerName") && PlayerPrefs.GetString("playerName")!="")
        {
            SetInputField();
        }

        if (butt != null) butt.onClick.AddListener(() => // if we click on the button
        {
            StartCoroutine(enterName());
        });
    }

    void resetInputField()
    {
        tx.text = defaultText;
        tx.color = txPendingCol;
        if (isInputActive)
        {
            img.color = bgPendingCol;
        }
        else
        {
            img.color = bgEmptyCol;
        }
    }

    void SetInputField()
    {
        img.color = bgActiveCol;
        tx.text = PlayerPrefs.GetString("playerName");
        tx.color = txActiveCol;
        isInputActive = false;
    }

    IEnumerator enterName()
    {
        isInputActive = true;
        resetInputField();
        while (isInputActive)
        {
            yield return waitTillKeyPressed; // we wait until a key was pressed
            if (tx.text == defaultText) { tx.text = ""; } // so that we don't write after the prompt but in its place
            // I would put the interact and menu custom keys there instead of hardcoded Escape and Enter, but what if they have alphanumeric values? People couldn't use those in their name. It'd be a mess. So I can't. (probably?)
            if (Input.GetKeyDown(KeyCode.Escape)) // if Escape was pressed, actually the previous name was good
            {
                if (PlayerPrefs.HasKey("playerName"))
                {
                    SetInputField();
                }
                else
                {
                    resetInputField();
                }
            }
            foreach (char c in Input.inputString) // I don't know why we need to use the foreach (and I resent it) but if I don't it doesn't work
            {
                if ((c == '\n') || (c == '\r')) // if Enter was pressed, it confirms the name
                {
                    if (tx.text != defaultText)
                    {
                        PlayerPrefs.SetString("playerName", tx.text);
                        SetInputField();
                    }
                    else
                    {
                        resetInputField();
                    }
                }
                else if (c == '\b') // if Backspace was pressed, I made a typo
                {
                    if (tx.text.Length > 0) // we don't want a negative length to our string
                    {
                        tx.text = tx.text.Substring(0, tx.text.Length - 1);
                    }
                }
                else if (tx.text.Length < maxLength) // we don't want a string that's too long either
                {
                    tx.text += c;
                }
            }
            if (tx.text == "")
            {
                resetInputField();
            }
        }
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0) && isInputActive) // if we clicked while writing the name, it confirms the name
        {
            if (tx.text != defaultText)
            {
                PlayerPrefs.SetString("playerName", tx.text);
            }
            SetInputField();
        }
    }
}