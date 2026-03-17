using UnityEngine;
using UnityEngine.UI;

public class MyJump : MonoBehaviour
{
    public Rigidbody rigidbody;             //강체(형태와 크기가 고정된 고체) 물리 현상이 동작하게 해주는 컴포넌트
    public float power = 20f;        //변수 힘을 선언함
    public float timer = 0;
    public Text TextUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer = timer + Time.deltaTime;             //타이머를 상승시킨다.
        TextUI.text = timer.ToString();              //타이머 숫자를 문자열 변수로 변경한 후 표시한다.

        if (Input.GetKeyDown(KeyCode.Space))       //스페이스 키를 눌렀을 때
        {
            power = power + Random.Range(-100, 200);   //Power를 랜덤으로 변경시킨다. (-100, 200)사이의 값을 더한다.
            rigidbody.AddForce(transform.up * power);  //변수(power)의 위쪽으로 힘을 준다.
        }

        if (this.gameObject.transform.position.y > 5 || this.gameObject.transform.position.y < -3)
        {
            Destroy(this.gameObject);
        }
    }
}
