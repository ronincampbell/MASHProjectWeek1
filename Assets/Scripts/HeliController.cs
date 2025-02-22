using UnityEngine;

public class HeliController : MonoBehaviour
{
    public float speed = 10.0f;
    private int soldierCount = 0;
    private int soldiersRescued = 0;
    public TMPro.TextMeshProUGUI soldierCountText;
    public TMPro.TextMeshProUGUI soldiersRescuedText;
    public TMPro.TextMeshProUGUI winText;
    public TMPro.TextMeshProUGUI loseText;

    private bool isGameOver = false;

    // SFX
    public AudioSource audioSource;
    public AudioClip soldierRescuedSFX;
    public AudioClip soldierPickupSFX;
    public AudioClip LoseSFX;
    public AudioClip WinSFX;

    private Vector2 lastDirection = Vector2.right; // Default direction


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // Get the direction from arrow keys
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Calculate the direction vector
        Vector2 direction = new Vector2(horizontal, vertical).normalized;

        // If no input is detected, use the last direction
        if (direction != Vector2.zero)
        {
            lastDirection = direction;
        }

        // Move the helicopter in the last direction at a constant speed
        transform.Translate(lastDirection * speed * Time.deltaTime);

        // Flip helicopter sprite based on horizontal movement
        if (lastDirection.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (lastDirection.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        // Stop helicopter from moving out of view port
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        pos.x = Mathf.Clamp01(pos.x);
        pos.y = Mathf.Clamp01(pos.y);
        transform.position = Camera.main.ViewportToWorldPoint(pos);

        // Collision detection
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1.0f);
        foreach (Collider2D col in colliders)
        {
            if (col.tag == "Soldier")
            {
                Destroy(col.gameObject);
                soldierCount++;
                soldierCountText.text = "Soldiers: " + soldierCount + "/3";
                audioSource.PlayOneShot(soldierPickupSFX);
            }

            if (col.tag == "Hospital")
            {
                soldiersRescued += soldierCount;
                if (soldierCount > 0)
                {
                    audioSource.PlayOneShot(soldierRescuedSFX);
                }

                soldierCount = 0;
                soldierCountText.text = "Soldiers: " + soldierCount + "/3";
                soldiersRescuedText.text = "Rescued: " + soldiersRescued + "/3";
            }

            if (col.tag == "Tree" && !isGameOver)
            {
                isGameOver = true;
                loseText.gameObject.SetActive(true);
                audioSource.PlayOneShot(LoseSFX);
                Time.timeScale = 0;
            }
        }

        // If soldierCount is 3 then display win message
        if (soldiersRescued == 3 && !isGameOver)
        {
            isGameOver = true;
            winText.gameObject.SetActive(true);
            audioSource.PlayOneShot(WinSFX);
            Time.timeScale = 0;
        }

        // If player presses R key then restart the game
        if (Input.GetKeyDown(KeyCode.R))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            Time.timeScale = 1;
        }
    }
}