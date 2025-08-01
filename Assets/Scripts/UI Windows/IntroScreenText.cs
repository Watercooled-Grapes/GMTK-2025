using UnityEngine;
using TMPro;
using System.Collections;
using System;

public class IntroScreenText : MonoBehaviour
{
    private TMP_Text _textBox;
    [SerializeField] private float delay = 0.1f;
    private int pos = 8;
    [SerializeField] private GameObject typewriter;

    void Start()
    {
        _textBox = this.GetComponent<TMP_Text>();
        _textBox.text = "running...";
        typewriter.GetComponent<TypewriterOnEvent>().StartGameInput += DisplayBroke;

        StartCoroutine(RunningText());
    }

    private IEnumerator RunningText()
    {
        yield return new WaitForSeconds(delay);
        _textBox.maxVisibleCharacters = pos;
        pos++;
        if (pos > 10) pos = 8;
        StartCoroutine(RunningText());
    }

    void DisplayBroke()
    {
        StopAllCoroutines();
        _textBox.maxVisibleCharacters = 9999;
        _textBox.text = "you weren't supposed to do that...";
    }
}
