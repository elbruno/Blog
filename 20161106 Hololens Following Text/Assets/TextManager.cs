using System;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    private GameObject _textSample;
    private GameObject _textSample2;

    void Start()
    {
        if (_textSample == null) _textSample = GameObject.Find("TextSample");
        if (_textSample2 == null) _textSample2 = GameObject.Find("TextSample2");
    }

    void Update()
    {
        if (_textSample == null ||_textSample2 == null) return;

        var camPos = Camera.main.transform.position + Camera.main.transform.forward;
        _textSample.transform.position = camPos;
        _textSample.transform.localScale = Vector3.one * 0.025f;

        var difference = new Vector3(camPos.x + 0.07f, camPos.y + 0.07f, camPos.z);
        _textSample2.transform.position = difference;
        _textSample2.transform.localScale = Vector3.one * 0.025f;
    }
}
