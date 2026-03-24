using UnityEngine;

public class MyBall : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name + " 와 충돌");      //이름으로도 접근할 수 있다

        if(collision.gameObject.tag == "Ground")              //충돌한 게임 오브젝트의 태그가 Ground인 경우
        {
            Debug.Log("땅과 충돌");                           //디버그 로그를 쓴다.
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("트리어 안에 들어옴");
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("트리어 밖으로 나감");
    }
}
