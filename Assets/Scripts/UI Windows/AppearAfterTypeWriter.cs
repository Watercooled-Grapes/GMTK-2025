using System;
using System.Collections;
using UnityEngine;
using TMPro;
using Object = UnityEngine.Object;

public class AppearAfterTypeWriter : MonoBehaviour
{
    [SerializeField] GameObject typewriterObject;
    [SerializeField] float secondsBeforeEvent = 1;
    [SerializeField] private float scaleTime = 0.5f;
    public event Action AfterAppearing;

    void Start()
    {
        this.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
        typewriterObject.GetComponent<TypewriterEffect>().CompleteTextRevealed += Open;
    }

    void Open()
    {
        StartCoroutine(GradualScale());
        StartCoroutine(delayBeforeEvent());
    }

    private IEnumerator delayBeforeEvent()
    {
        yield return new WaitForSeconds(secondsBeforeEvent);
        AfterAppearing.Invoke();
    }

    private IEnumerator GradualScale()
{
    RectTransform rectTransform = this.GetComponent<RectTransform>();
    float elapsedTime = 0;
    Vector3 startScale = Vector3.zero;
    Vector3 targetScale = Vector3.one;
    
    // Gradually scale over time
    while (elapsedTime < scaleTime)
    {
        rectTransform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / scaleTime);
        elapsedTime += Time.deltaTime;
        yield return null;
    }
    
    // Ensure we end at exactly the target scale
    rectTransform.localScale = targetScale;
    
    // After scaling completes, start the delay before the event
    StartCoroutine(delayBeforeEvent());
}
}
