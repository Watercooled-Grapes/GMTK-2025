using UnityEngine;
using Unity.Collections;
using TMPro;

public class CodeLineManager : MonoBehaviour
{
    private TMP_Text tmp;
    private string[] lines;
    private int pos = 0;

    public int PLACEHOLDERCOUNTER
    {
        get
        {
            return pos;
        }
        set
        {
            pos = value;
            updateCode();
        }
    } 

    void Awake()
    {
        tmp = this.GetComponent<TMP_Text>();

        lines = new string[] { "def helloWorld()", "    this is some supah cool code", "    man i sure do hope i dont get hightlighted", "end" };

        updateCode();
    }

    private void updateCode()
    {
        tmp.text = "";
        for (int i = 0; i < lines.Length; i++)
        {
            if (i == pos)
            {
                string highlightedText = lines[i].Insert(0, "<mark=#ffff00aa>");
                highlightedText = highlightedText.Insert(lines[i].Length + "<mark=#ffff00aa>".Length, "</mark>");

                tmp.text += highlightedText + "\n";
            }
            tmp.text += lines[i] + "\n";
        }
    }
}
