using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerControler : MonoBehaviour
{
    public float speed = 5f;
    public float jumpSpeed = 6f;
    private float direction = 0f;
    private Rigidbody2D player;

    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask groundLayer;
    private bool isTouchingGround;

    private Animator playerAnimation;

    private Vector3 respawnPoint;
    public GameObject fallDetector;
    public Text scoreText;
    public HealthBar healthBar;

    private float vertical;
    private float climbingSpeed = 5f;
    private bool isLadder;
    private bool isClimbing;

    public GameManagerScript gameManager;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Rigidbody2D>();
        playerAnimation = GetComponent<Animator>();
        respawnPoint = transform.position;
        scoreText.text = "Score: " + Scoring.totalScore;
    }

    // Update is called once per frame
    void Update()
    {
        isTouchingGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        //movin the player left and right
        direction = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if (direction > 0f) //right direction
        {
            player.velocity = new Vector2(direction * speed, player.velocity.y);
            transform.localScale = new Vector2(0.3123609f, 0.3123609f);
        }
        else if (direction < 0f) //left direction
        {
            player.velocity = new Vector2(direction * speed, player.velocity.y);
            transform.localScale = new Vector2(-0.3123609f, 0.3123609f); //flips the player when walking to the left
        }
        else //not moving the character (not pressing anything)
        {
            player.velocity = new Vector2(0, player.velocity.y);
        }

        //making the player jump
        if (Input.GetButtonDown("Jump") && isTouchingGround)
        {
            player.velocity = new Vector2(player.velocity.x, jumpSpeed);
        }
        
        //climbing a ladder
        if (isLadder && Mathf.Abs(vertical) > 0)
        {
            isClimbing = true;
        }

        playerAnimation.SetFloat("Speed", Mathf.Abs(player.velocity.x));
        playerAnimation.SetBool("OnGround", isTouchingGround);

        fallDetector.transform.position = new Vector2(transform.position.x, fallDetector.transform.position.y); //makes the collider folloe the player
    }

    private void FixedUpdate()
    {
        if (isClimbing)
        {
            player.gravityScale = 0f;
            player.velocity = new Vector2(player.velocity.x, vertical * climbingSpeed);
        }
        else
        {
            player.gravityScale = 1f;
        }
    }

    void OnTriggerEnter2D(Collider2D collision) //checks if the player hits the collider => respawns the player, adds checkpoints, portal managment
    {
        if(collision.tag == "FallDetector") 
        {
            healthBar.Damage(0.05f);
            transform.position = respawnPoint;
        }
        else if(collision.tag == "CheckPoint")
        {
            respawnPoint = transform.position;
        }
        else if(collision.tag == "NextLevel")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            respawnPoint = transform.position;
        }
        else if (collision.tag == "PreviousLevel")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
            respawnPoint = transform.position;
        }
        else if(collision.tag == "Crystal")
        {
            collision.gameObject.SetActive(false);
            Scoring.totalScore += 1;
            scoreText.text = "Score: " + Scoring.totalScore;
        }
        else if(collision.tag == "Ladder")
        {
            isLadder = true;
        }
        else if(collision.tag == "WinPortal")
        {
            gameManager.gameOver();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Ladder")
        {
           isLadder = false;
            isClimbing = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Spikes")
        {
            healthBar.Damage(0.002f);
        }
    }
}
