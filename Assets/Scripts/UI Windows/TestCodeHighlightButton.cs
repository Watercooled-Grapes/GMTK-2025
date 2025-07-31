using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestCodeHighlightButton : MonoBehaviour
{
    public GameObject codeTmp;

    void Awake()
    {
        this.GetComponent<Button>().onClick.AddListener(updateCounter);
    }

    public void updateCounter()
    {
        codeTmp.GetComponent<CodeLineManager>().PLACEHOLDERCOUNTER += 1;
    }
}
