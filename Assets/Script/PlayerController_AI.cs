using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController_AI : MonoBehaviour
{
    public float speed = 10f;

    public Text countText;
    public Text winText;
    public Text gameOverText;

    public int win = 12;
    public float fall = -5f;

    public GameObject restartButton;
    public GameObject quitButton;

    private Rigidbody rb;
    private int count = 0;
    private bool isGameOver = false;

    private Color ballColor = Color.white;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (winText != null)
            winText.text = "";

        if (gameOverText != null)
            gameOverText.text = "";

        if (restartButton != null)
            restartButton.SetActive(false);

        if (quitButton != null)
            quitButton.SetActive(true);

        UpdateUI();
    }

    void Update()
    {
        if (isGameOver) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0, v);

        rb.AddForce(move * speed);

        if (transform.position.y < fall)
        {
            GameOver();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isGameOver) return;

        if (other.CompareTag("pick up"))
        {
            Renderer r = other.GetComponent<Renderer>();
            if (r != null)
            {
                ballColor = r.material.color;
                GetComponent<Renderer>().material.color = ballColor;
            }

            other.gameObject.SetActive(false);
            count++;

            UpdateUI();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isGameOver) return;

        if (collision.gameObject.CompareTag("wall"))
        {
            Renderer r = collision.gameObject.GetComponent<Renderer>();
            if (r != null)
            {
                Color wallColor = r.material.color;
           
                if (SameColor(ballColor, wallColor))
                {
                    Physics.IgnoreCollision(GetComponent<Collider>(), collision.collider, true);
                }
            }
        }
    }

    bool SameColor(Color a, Color b)
    {
        return a == b;
    }

    void UpdateUI()
    {
        if (countText != null)
            countText.text = "count: " + count;

        if (count >= win)
        {
            if (winText != null)
                winText.text = "you win <3";

            isGameOver = true;

            if (restartButton != null)
                restartButton.SetActive(true);
        }
    }

    void GameOver()
    {
        isGameOver = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (gameOverText != null)
            gameOverText.text = "game over :(";

        if (restartButton != null)
            restartButton.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}