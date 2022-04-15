using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float horizontalMove;
    public float speedCoefficient;
    public float jumpCoefficient;
       

    private Rigidbody2D rb;


    bool isRight; //for checking if character turned to the right


    public Transform PositionOfFeet;
    private bool IsOnTheGround;
    public float DistanceToCheck;
    public LayerMask Ground;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
        IsOnTheGround = Physics2D.OverlapCircle(PositionOfFeet.position, DistanceToCheck, Ground);

        if (IsOnTheGround && Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = Vector2.up * jumpCoefficient;
        }
    }

    void FixedUpdate() 
    {
        horizontalMove = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(horizontalMove * speedCoefficient, rb.velocity.y);
        if (isRight && horizontalMove > 0)
        {
            FaceFlip();

        } else if (!isRight & horizontalMove < 0)
        {
            FaceFlip();
        }
    }

    void FaceFlip()
    {
        isRight = !isRight;
        Vector3 temp = transform.localScale;
        temp.x *= -1;
        transform.localScale = temp;
    }


}
