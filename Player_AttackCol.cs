using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_AttackCol : MonoBehaviour
{
    private CircleCollider2D attackCol = null;
    private PlayerControl playerCon = null;

    private void Awake()
    {
        playerCon = this.transform.root.GetComponent<PlayerControl>();
        attackCol = this.GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        if(playerCon.IsFlipX)
        {
            attackCol.offset = new Vector2(-1.0f, 1.0f);
        }
        else
        {
            attackCol.offset = new Vector2(1.0f, 1.0f);
        }
    }
}
