using System;
using System.Collections;
using UnityEngine;
using TMPro;
using Object = UnityEngine.Object;
using UnityEngine.SceneManagement;

public class TypewriterOnEvent : MonoBehaviour
{
    [SerializeField] GameObject prevObject;

    private TMP_Text _textBox;

    // Basic Typewriter Functionality
    private int _currentVisibleCharacterIndex;
    private Coroutine _typewriterCoroutine;
    private bool _readyForNewText = true;

    private WaitForSeconds _simpleDelay;

    [Header("Typewriter Settings")]
    [SerializeField] private float charactersPerSecond = 20;


    // Event Functionality
    private WaitForSeconds _textboxFullEventDelay;
    [SerializeField][Range(0.1f, 0.5f)] private float sendDoneDelay = 0.25f; 

    public event Action StartGameInput;

    // we gon make this supah cool later with epic screen cracking and guy popping out, but just change scenes for now
    void StartGame()
    {
        SceneManager.LoadScene(sceneName:"Level1");
    } 

    private void Awake()
    {
        ///////
        StartGameInput += StartGame;
        ///////


        prevObject.GetComponent<AppearAfterTypeWriter>().AfterAppearing += StartTyping;

        _textBox = GetComponent<TMP_Text>();

        _simpleDelay = new WaitForSeconds(1 / charactersPerSecond);

        _textboxFullEventDelay = new WaitForSeconds(sendDoneDelay);

        _textBox.maxVisibleCharacters = 0;
    }

    void Update()
    {
        if (Input.anyKey)
        {
            if (_typewriterCoroutine != null) StopCoroutine(_typewriterCoroutine);
            if (_textBox.maxVisibleCharacters > 0) StartGameInput.Invoke();
        }
    }

    private void StartTyping()
    {
        _readyForNewText = false;

        // Stop any existing typewriter
        if (_typewriterCoroutine != null)
            StopCoroutine(_typewriterCoroutine);

        // Reset character visibility
        _textBox.maxVisibleCharacters = 0;
        _currentVisibleCharacterIndex = 0;

        // Start the typewriter
        _typewriterCoroutine = StartCoroutine(Typewriter());
    }

    private void OnEnable()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(PrepareForNewText);
    }

    private void OnDisable()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(PrepareForNewText);
    }

    private void PrepareForNewText(Object obj)
    {
        if (obj != _textBox || !_readyForNewText || _textBox.maxVisibleCharacters >= _textBox.textInfo.characterCount)
            return;

        _readyForNewText = false;

        if (_typewriterCoroutine != null)
            StopCoroutine(_typewriterCoroutine);

        _textBox.maxVisibleCharacters = 0;
        _currentVisibleCharacterIndex = 0;

        // _typewriterCoroutine = StartCoroutine(Typewriter());
    }

    private IEnumerator Typewriter()
    {
        TMP_TextInfo textInfo = _textBox.textInfo;

        while (_currentVisibleCharacterIndex < textInfo.characterCount + 1)
        {
            var lastCharacterIndex = textInfo.characterCount - 1;

            if (_currentVisibleCharacterIndex >= lastCharacterIndex)
            {
                _textBox.maxVisibleCharacters++;
                yield return _textboxFullEventDelay;
                _readyForNewText = true;
                yield break;
            }

            char character = textInfo.characterInfo[_currentVisibleCharacterIndex].character;

            _textBox.maxVisibleCharacters++;

            yield return _simpleDelay;

            _currentVisibleCharacterIndex++;
        }
    }
}
