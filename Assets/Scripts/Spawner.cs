using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject coinPrefabs;              //동전 프리팹
    public GameObject MissilePrefabs;

    [Header("스폰 타이밍 설정")]
    public float minSpawnInterval = 0.5f;       //최소 생성 간격
    public float maxSpawnInterval = 2.0f;       //최대 생성 간격

    public float timer = 0.0f;
    public float nextSpawnTime;                 //다음 생성시간

    [Header("동전 스폰 확률 설정")]
    [Range(0, 100)]
    public int coinSpawnChance = 50;             //동전이 생성될 확률(0~100) => 50%



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetNextSpawnTime();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;               //시간이 0에서 점점 증가

        //생성 시간이 되면 오브젝트 생성
        if(timer > nextSpawnTime)
        {
            SpawnObject();                       //프리팹 생성 함수를 호출한다
            timer = 0.0f;                       //시간을 초기화 시켜준다
            SetNextSpawnTime();                 //다시 함수 실행
        }
    }

    void SpawnObject()
    {
        Transform spawnTransform = transform;                                            //스포너 오브젝트의 위치와 회전 값을 가져온다

        //확률에 따라 동전 또는 미사일 생성
        int randomValue = Random.Range(0, 100);

        if(randomValue < coinSpawnChance)
        {
            Instantiate(coinPrefabs, spawnTransform.position, spawnTransform.rotation);       //코인 프리팹을 해당 위치에 생성한다
        }
        else
        {
            Instantiate(MissilePrefabs, spawnTransform.position, spawnTransform.rotation);      //미사일 프리팹을 해당 위치에 생성한다
        }

    }


    void SetNextSpawnTime()          //최소~최대 사이의 랜덤한 시간 설정
    {
        nextSpawnTime = Random.Range(minSpawnInterval, maxSpawnInterval);
    }
}
