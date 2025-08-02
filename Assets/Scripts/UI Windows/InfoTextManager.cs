using UnityEngine;
using TMPro;

public class InfoTextManager : MonoBehaviour
{
    private TMP_Text tmp;
    private string[] lines;
    [SerializeField] private string baseText;

    void Awake()
    {
        tmp = this.GetComponent<TMP_Text>();

        UpdateTurnLoopInfo(1, 1);
    }

    public void UpdateTurnLoopInfo(int turnsLeft, int loopsLeft)
    {
        tmp.text = "player@GTMK-2025:$ " + baseText + "\n";
        tmp.text += "turns left: " + turnsLeft + "\n";
        tmp.text += "loops left: " + loopsLeft;
    }
}
