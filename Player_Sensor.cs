using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Sensor : MonoBehaviour
{
    // 외부 읽기를 위한 프로퍼티
    [SerializeField]
    private bool isCollided = false;
    public bool IsCollided { get => isCollided; }

    // 충돌 시 효과음이 인스펙터 창에서 설정 되었다면 효과음을 재생한다.
    [SerializeField]
    private Player_SFX sfx = Player_SFX.None;

    // Inspector창에서 지정한 레이더들과만 충돌처리가 가능하도록 만든다.
    [SerializeField]
    private int[] layers;

    //  Sensor이기 때문에 모든 충돌은 트리거 충돌
    private void OnTriggerEnter2D(Collider2D collision)
    {
        for(int i = 0; i < layers.Length; i++)
        {
            if (collision.gameObject.layer == layers[i])
            {
                // 지정한 효과음이 있다면 그 효과음을 SoundManager를 통해 재생 ( Audio Source는 플레이어의 하위에 들어있다)
                if (sfx != Player_SFX.None)
                    SoundManager.Instance.PlayPSFX(sfx);

                isCollided = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        for (int i = 0; i < layers.Length; i++)
        {
            if (collision.gameObject.layer == layers[i])
                isCollided = false;
        }
    }
}
