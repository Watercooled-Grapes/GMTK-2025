using UnityEngine;
using Unity.Collections;
using TMPro;
using System.Collections.Generic;

public class CodeLineManager : MonoBehaviour
{
    private TMP_Text tmp;
    private List<string> lines;

    void Awake()
    {
        tmp = this.GetComponent<TMP_Text>();

        lines = new List<string>() { "while (true)", "continue" };
    }

    public void Init(int numTurns)
    {
        for (int i = 0; i < numTurns; i++) lines.Insert(1, "   move();");
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
                string highlightedText = lines[i].Insert(0, "<mark=#ffff00aa>");
                printText = highlightedText.Insert(lines[i].Length + "<mark=#ffff00aa>".Length, "</mark>");
            }
            tmp.text += i + 1 + "|   " + printText + "\n";
        }
    }

    public void addLines(int n)
    {
        for (int i = 0; i < n; i++) lines.Insert(1, "   move();");
    }
}
