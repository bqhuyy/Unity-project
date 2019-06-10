using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Kirby_KinematicControl : MonoBehaviour
{
    private Rigidbody2D myRigidbody;
    private Animator myAnimator;

    [SerializeField]
    private float movementSpeed = 1.0f;

    private bool facingRight;

    [SerializeField]
    private Transform[] groundPoints;

    [SerializeField]
    private float groundRadius;

    [SerializeField]
    private LayerMask whatIsGround;

    private bool isGrounded;

    private bool jump;

    [SerializeField]
    private bool airControl;

    [SerializeField]
    private float jumpForce;

    const int airLayer = 1;

    private string[] monster = new string[] {
        "UmbrellaOrange",
        "ElectricOrange",
        "Sword"
    };

    // Start is called before the first frame update
    void Start()
    {
        facingRight = true;
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        if (gameObject)
        {
            HandleInput();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (gameObject)
        {
            float horizontal = Input.GetAxis("Horizontal");
            isGrounded = IsGrounded();

            HandleMovement(horizontal);
            Flip(horizontal);
            HandleLayers();
            ResetValues();
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump = true;
        }
    }

    private void ResetValues()
    {
        jump = false;
    }

    private void HandleMovement(float horizontal)
    {
        if (myRigidbody.velocity.y < 0)
        {
            myAnimator.SetBool("land", true);
        }

        if (isGrounded && jump)
        {
            isGrounded = false;
            myRigidbody.AddForce(new Vector2(0, jumpForce));
            myAnimator.SetTrigger("jump");
        }

        myRigidbody.AddForce(new Vector2(horizontal * movementSpeed, 0));
        myAnimator.SetFloat("speed", Mathf.Abs(horizontal));
    }

    private void Flip(float horizontal)
    {
        if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight)
        {
            facingRight = !facingRight;
            Vector3 theScale = transform.localScale;

            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }

    private bool IsGrounded()
    {
        if (myRigidbody.velocity.y <= 0)
        {
            foreach (Transform point in groundPoints)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(point.position, groundRadius, whatIsGround);
                
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].gameObject != gameObject)
                    {
                        myAnimator.ResetTrigger("jump");
                        myAnimator.SetBool("land", false);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private void HandleLayers()
    {
        if (!isGrounded)
        {
            myAnimator.SetLayerWeight(airLayer, 1);
        }
        else
        {
            myAnimator.SetLayerWeight(airLayer, 0);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (monster.Contains(collision.gameObject.name))
        {
            Destroy(gameObject);
        }
    }
}
