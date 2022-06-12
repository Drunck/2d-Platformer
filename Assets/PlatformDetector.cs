using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformDetector : MonoBehaviour
{
    public bool IsOnTheGround;
    public bool check;
    public Transform Player;
    public float DistanceToCheck;
    public Transform PositionOfFeet;
    public LayerMask Ground;



    // Update is called once per frame
    void Update()
    {
        IsOnTheGround = Physics2D.OverlapCircle(PositionOfFeet.position, DistanceToCheck, Ground);
        if (IsOnTheGround != true)
        {
            check = false;
            if(Input.GetAxisRaw("Horizontal") > .25f || Input.GetAxisRaw("Horizontal") < -.25f)
            {
                Player.SetParent(null);
            }
        }

        if(check != true)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, .125f);

            if(hit.collider != null)
            {
                if(hit.collider.CompareTag("MovingPlatform"))
                {
                    Player.SetParent(hit.transform);
                }
                else
                {
                    Player.SetParent(null);
                }
                check = true;
            }
        }

    }
}
