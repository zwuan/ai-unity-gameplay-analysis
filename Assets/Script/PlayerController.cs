// ======================================================================================
// MANDATORY HUMAN DECISION NODE
// AI proposed: A Unity player controller script that applies movement force in Update()
// and compares colors using direct equality (a == b) for gameplay logic.
// I rejected/modified because: applying physics in Update() caused frame-dependent
// acceleration and unstable movement, and exact color equality failed in practice
// because visually identical colors are not always numerically identical in Unity.
// My decision: I moved physics-based movement to FixedUpdate() to align with Unity’s
// physics system, and replaced exact color equality with a tolerance-based comparison
// to ensure consistent gameplay behavior that matches player perception.
// ======================================================================================

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public Text countText;
    public Text winText;
    public Text gameOverText;

    private Rigidbody rb;
    private int count;
    private bool isGameOver = false;

    public int win = 12;
    public float fall = -5f;

    private Renderer ballRenderer;
    private Color ballColor = Color.white;

    public GameObject restartButton;
    public GameObject quitButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ballRenderer = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();

        count = 0;
        SetCountText();
        winText.text = "";
        if (gameOverText != null)
            gameOverText.text = "";

        SetPlayerColor(ballColor);

        if (restartButton != null)
            restartButton.SetActive(false);

        if (quitButton != null)
            quitButton.SetActive(true);
    }

    void FixedUpdate()
    {
        if (isGameOver) return;

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rb.AddForce(movement * speed);
    }

    private void Update()
    {
        if (isGameOver) return;

        if (transform.position.y < fall)
            GameOver();              
    }

    void OnTriggerEnter(Collider other)
    {
        if (isGameOver) return;

        //Destroy(other.gameObject);
        if (other.gameObject.CompareTag("pick up"))
        {
            Renderer pickupRenderer = other.GetComponent<Renderer>();
            if (pickupRenderer != null)
                SetPlayerColor(pickupRenderer.material.color);

            other.gameObject.SetActive(false);
            count++;
            SetCountText();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isGameOver) return;

        if (collision.gameObject.CompareTag("wall"))
        {
            Renderer wallRenderer = collision.gameObject.GetComponent<Renderer>();
            if(wallRenderer != null)
            {
                Color wallcolor = wallRenderer.material.color;

                if(SameColor(ballColor, wallcolor))
                {
                    Collider ballCollider = GetComponent<Collider>();
                    Collider wallCollider = collision.collider;

                    Physics.IgnoreCollision(ballCollider, wallCollider, true);
                    Invoke(nameof(ResetWallCollision), 0.8f);
                }
            }
        }
    }

    void ResetWallCollision()
    {
        Collider ballCollider = GetComponent<Collider> ();

        GameObject[] walls = GameObject.FindGameObjectsWithTag("wall");
        foreach (var wall in walls)
        {
            Collider wallCollider = wall.GetComponent<Collider>();
            if (wallCollider != null)
                Physics.IgnoreCollision (ballCollider, wallCollider, false);
        }
    }

    void SetPlayerColor(Color c)
    { 
        ballColor = c;
        if (ballRenderer != null)
            ballRenderer.material.color = c;
    }

    bool SameColor(Color a, Color b)
    {
        float tolerance = 0.05f;
        return Mathf.Abs(a.r - b.r) < tolerance && Mathf.Abs(a.g - b.g) < tolerance && Mathf.Abs(a.b - b.b) < tolerance;
    }

    void SetCountText() 
    {
        countText.text = "count: " + count.ToString();

        if (count >= win) 
        {
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

    public void QuitGame()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
    public void RestartGame()
    {
        if (!isGameOver)
            return;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
