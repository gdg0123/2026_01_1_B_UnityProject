using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("기본 이동 설정")]
    public float moveSpeed = 5.0f;          //이동 속도 변수 설정
    public float jumpForce = 7.0f;          //점프 힘 변수 설정
    public float turnSpeed = 10f;           //회전 속도

    [Header("점프 개선 설정")]
    public float fallMultiplier = 2.5f;        //하강 중력 배율
    public float lowJumpMultiplier = 2.0f;     //짧은 점프 배율

    [Header("지면 감지 설정")]
    public float coyoteTime = 0.15f;           //지면 관성 시간
    public float coyoteTimeCounter;            //관성 타이머
    public bool realGrouned = true;            //실제 지면 상태

    [Header("글라이더 설정")]
    public GameObject gliderObject;            //글라이더 오브젝트
    public float gliderFallSpeed = 1.0f;       //글라이더 낙하속도
    public float gliderMoveSpeed = 7.0f;       //글라이더 이동 속도
    public float gliderMaxTime = 5.0f;         //최대 사용 시간
    public float gliderTimeLeft;               //남은 사용 시간
    public bool isGliding = false;             //글라이딩 중인지 여부


    public Rigidbody rb;

    public bool isGrounded = true;          //플레이어가 땅에 있는지 여부를 나타내는 변수

    public int coinCount = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        coyoteTimeCounter = 0;

        if(gliderObject != null)                //글라이더 오브젝트 초기화
        {
            gliderObject.SetActive(false);      //시작시 비활성화
        }

        gliderTimeLeft = gliderMaxTime;         //글라이더 시간 초기화
    }

    // Update is called once per frame
    void Update()
    {

        //지면 감지
        UpdateGrounededState();


        float moveHorizontal = Input.GetAxis("Horizontal");  //수평 이동
        float moveVertical = Input.GetAxis("Vertical");      //수직 이동

        //이동 방향 벡터
        Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical);     //이동 방향 감지

        if(movement.magnitude > 0.1f)   //입력이 있을 때만 회전
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);    //이동 방향을 바라보도록 부드럽게 회전
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        //G키로 글라이더 제어 (누르는 동안 활성화)
        if(Input.GetKey(KeyCode.G) && !isGrounded && gliderTimeLeft > 0)      //G키를 누르면서 땅에 있지 않고 글라이더 남은 시간이 있을 떄 (3가지 조건)
        {
            if (!isGliding)                  //글라이더 활성화
            {
                EnableGlider();
            }

            //글라이더 사용 시간 감소
            gliderTimeLeft -= Time.deltaTime;

            //글라이더 시간이 다 되면 비활성화
            if(gliderTimeLeft <= 0)
            {
                DisableGlider();
            }

        }
        else if(isGliding)
        {
            DisableGlider();
        }


        if(isGliding)                              //글라이딩 움직임 처리
        {
            ApplyGliderMovement(moveHorizontal, moveVertical);
        }
        else
        {
            //속도 값으로 직접 이동
            rb.linearVelocity = new Vector3(moveHorizontal * moveSpeed, rb.linearVelocity.y, moveVertical * moveSpeed);

            //착시 점프 높이 구현
            if (rb.linearVelocity.y < 0)
            {
                rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }
            else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
            {
                rb.linearVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
        }


        //점프 입력 처리
        if (Input.GetButtonDown("Jump") && isGrounded)                  //&& 두 값을 만족할 때 -> 스페이스 버튼을 눌렀을 때와 isGrounded가 true일 때
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);    //위쪽으로 설정한 힘만큼 물체에 힘을 준다.
            isGrounded = false;                                        //점프를 한 순간 땅에서 떨어졌기 때문에 false로 한다
            realGrouned = false;
            coyoteTimeCounter = 0;
        }

        //지면에 있으면 ㄱㄹ라이더 시간 회복 및 글라이더 비활성화
        if(isGrounded)
        {
            if (isGliding)
            {
                DisableGlider();
            }
            //지상에 있을때 시간회복
            gliderTimeLeft = gliderMaxTime;
        }

    }



    private void OnCollisionEnter(Collision collision)        //충돌 처리 함수
    {
        if (collision.gameObject.tag == "Ground")
        {
            realGrouned = true;
        }
    }

    private void OnCollisionStay(Collision collision)        //지면과의 충돌이 유지되는지 확인
    {
        if (collision.gameObject.tag == "Ground")            //충돌이 일어난 물체의 Tag가 Ground인 경우
        {
            realGrouned = true;                              //땅과 충돌하면 True로 변경한다
        }
    }

    private void OnCollisionExit(Collision collision)        //지면에서 떨어졌는지 확인
    {
        if (collision.gameObject.tag == "Ground")
        {
            realGrouned = false;
        }
    }

    private void OnTriggerEnter(Collider other)           //트리어 영역 안에 들어왔나를 검사하는 함수
    {
        if (other.CompareTag("Coin"))                      //코인 트리거와 충돌하면
        {
            coinCount++;                                  //코인 변수 1을 올린다
            Destroy(other.gameObject);                     //코인 오브젝트를 파괴한다
        }
    }

    
    //지면 상태 업데이트 함수
    void UpdateGrounededState()
    {
        if(realGrouned)
        {
            coyoteTimeCounter = coyoteTime;
            isGrounded = true;
        }
        else
        {
            //실제로는 지면에 없지만 코요테 타임 내에 있으면 여전히 지면으로 판단
            if(coyoteTimeCounter > 0)
            {
                coyoteTimeCounter -= Time.deltaTime;          //시간을 지속적으로 감소시킨다
                isGrounded = true;
            }
            else
            {
                isGrounded = false;                           //타임이 끝나면 False
            }
        }
    }

    //글라이더 활성화 함수
    void EnableGlider()
    {
        isGliding = true;

        //글라이더 오브젝트 표시
        if (gliderObject != null)
        {
            gliderObject.SetActive(true);
        }

        //하강 속도 초기화
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, -gliderFallSpeed, rb.linearVelocity.z);
    }

    //글라이더 비활성화 함수
    void DisableGlider()
    {
        isGliding = false;

        //글라이더 오브젝트 숨기기
        if (gliderObject != null)
        {
            gliderObject.SetActive(false);
        }

        //즉시 낙하하도록 중력 적용
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
    }

    //글라이더 이동 적용
    void ApplyGliderMovement(float horizontal, float vertical)              //수평과 수직을 함수의 인수로 받는다
    {
        //글라이더 효과  천천히 떨어지고 수평 방향으로 더 빠르게 이동

        Vector3 gliderVelocity = new Vector3(
                horizontal * gliderMoveSpeed,
                -gliderFallSpeed,
                vertical * gliderMoveSpeed
        );

        rb.linearVelocity = gliderVelocity;
    }

}
