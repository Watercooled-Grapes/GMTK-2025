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
    [SerializeField] private AudioClip popupSound;


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

        // Instantiate the popup as a child of the canvas
        GameObject instantiatedPopup = Instantiate(popup, transform);

        // Get canvas rect and popup rect
        RectTransform canvasRect = transform.parent.GetComponent<RectTransform>();
        RectTransform popupRect = instantiatedPopup.GetComponent<RectTransform>();

        // Calculate canvas boundaries
        float canvasWidth = canvasRect.rect.width;
        float canvasHeight = canvasRect.rect.height;

        // Calculate random position within canvas boundaries (with margins)
        float marginX = popupRect.rect.width * 0.5f;
        float marginY = popupRect.rect.height * 0.5f;

        // Set position relative to canvas (keep z position as is)
        float randomX = Random.Range(-canvasWidth / 2 + marginX, canvasWidth / 2 - marginX);
        float randomY = Random.Range(-canvasHeight / 2 + marginY, canvasHeight / 2 - marginY);

        GetComponent<AudioSource>().PlayOneShot(popupSound);
        popupRect.localPosition = new Vector3(randomX, randomY, popupRect.localPosition.z);
    }
}
