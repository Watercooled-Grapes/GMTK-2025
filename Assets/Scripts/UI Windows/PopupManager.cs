using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static MainCharacter;
using Random = UnityEngine.Random;

public class PopupManager : MonoBehaviour
{
    [SerializeField] private GameObject popup;
    [SerializeField] private string[] titles;
    [SerializeField] private string[] contents;
    [SerializeField] private Sprite[] sprites;

    public void SpawnPopup(PopupTypes type)
    {
        popup.transform.GetChild(0).GetComponent<DragWindow>().canvas = transform.parent.GetComponent<Canvas>();
        popup.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = titles[Random.Range(0, titles.Length)];
        switch (type)
        {
            case (PopupTypes.Str):
                popup.GetComponent<Image>().sprite = null;
                popup.GetComponent<Image>().color = new Color(0, 0, 0, 0.45f);
                popup.transform.GetChild(1).GetComponent<TMP_Text>().text = contents[Random.Range(0, contents.Length)];
                break;
            case (PopupTypes.Img):
                popup.GetComponent<Image>().sprite = sprites[Random.Range(0, sprites.Length)];
                popup.GetComponent<Image>().color = new Color(255, 255, 255, 1);
                popup.transform.GetChild(1).GetComponent<TMP_Text>().text = "";
                break;
        }
        popup.GetComponent<RectTransform>().position += new Vector3(Random.Range(-800, 800), Random.Range(-250, 250), 0);
        Instantiate(popup, transform);


    }
}
