using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class PopupManager : MonoBehaviour
{
    [SerializeField] private GameObject popup;
    [SerializeField] private string[] titles;
    [SerializeField] private string[] contents;

    public void Update()
    {
        if (Random.Range(0, 100) == 1)
        {
            int textIndex = Random.Range(0, titles.Length);
            popup.transform.GetChild(0).GetComponent<DragWindow>().canvas = transform.parent.GetComponent<Canvas>();
            popup.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = titles[textIndex];
            popup.transform.GetChild(1).GetComponent<TMP_Text>().text = contents[textIndex];
            Instantiate(popup, transform); 
        }
    
    }
}
