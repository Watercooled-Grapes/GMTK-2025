using UnityEngine;
using Unity.Collections;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class CodeLineManager : MonoBehaviour
{
    [SerializeField] private string[] randomCode;
    private TMP_Text tmp;
    private List<string> lines;

    void Awake()
    {
        tmp = this.GetComponent<TMP_Text>();

        lines = new List<string>() { "while (true)", "continue" };
    }

    public void Init(int numTurns)
    {
        for (int i = 0; i < numTurns; i++) lines.Insert(1, "   " + randomCode[Random.Range(0,randomCode.Length)]);
        UpdateCode(1);
    }

    public void UpdateCode(int pos)
    {
        tmp.text = "";
        for (int i = 0; i < lines.Count; i++)
        {
            string printText = lines[i];
            if (i == pos)
            {
                string highlightedText = lines[i].Insert(0, "<mark=#ffffff50>");
                printText = highlightedText.Insert(lines[i].Length + "<mark=#ffffff50>".Length, "</mark>");
            }
            // tmp.text += i + 1 + "|   " + printText + "\n";
            tmp.text += printText + "\n";
        }
    }

    public void GoCrazy()
    {
        StartCoroutine(SweepThroughCode());
    }

    private IEnumerator SweepThroughCode()
    {
        for (int i = 1; i < lines.Count; i++)
        {
            yield return new WaitForSeconds(0.1f);
            UpdateCode(i);
        }
        StartCoroutine(SweepThroughCode());
    }

    public void addLines(int n)
    {
        for (int i = 0; i < n; i++) lines.Insert(1, "   " + randomCode[Random.Range(0,randomCode.Length)]);
    }
}
