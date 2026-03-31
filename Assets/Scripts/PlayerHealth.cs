using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxLives = 3;        //최대 생명 수
    public int curentLives;          //현재 생명 수

    public float invincibleTime = 1.0f;   //피격 후 무적 시간
    public bool isinvincible = false;          //무적 상태 여부


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        curentLives = maxLives;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Missile"))
        {
            curentLives--;
            Destroy(other.gameObject);

            if(curentLives <= 0)
            {
                GameOver();
            }
        }
    }


    void GameOver()
    {
        gameObject.SetActive(false);
        Invoke("RestartGame", 3.0f);
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
