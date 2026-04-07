using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;

public class CubeGameUI : MonoBehaviour
{
    public TextMeshProUGUI TimerText;
    public float Timer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime;                                   //타이머 시간이 늘어난다.s
        TimerText.text = "생존 시간: " + Timer.ToString("0.00");   //문자열 형태로 변환하여 보여준다.
    }
}
