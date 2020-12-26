using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Animation_Setting : MonoBehaviour
{
    private Animator animator = null;

    // 현재 애니메이션을 저장하는 문자열
    private string currentState;
    public string CurrentState { get => currentState; }

    private PlayerControl playerCon = null;

    // WallSlide 연출용 Prefeb
    [SerializeField]
    private GameObject Slide_Dust = null;

    [SerializeField]
    private AnimationClip[] AttackClips = null;

    [SerializeField]
    private AnimationClip HitClip = null;
    public float HitAnimLength { get => HitClip.length; }

    [SerializeField]
    private AnimationClip RollClip = null;
    public float RollAnimLength { get => RollClip.length; }

    // Block Start -> Block Idle로 넘어가기위해 구분하는 Boolean value
    private bool isReadyToBlockIdle = false;

    // 빠른 제어를 위해서 상수로 정의한다. ( 애니메이션 추가 시 추가된다)
    const string PLAYER_IDLE = "Idle";
    const string PLAYER_RUN = "Run";
    const string PLAYER_JUMP = "Jump";
    const string PLAYER_FALL = "Fall";
    const string PLAYER_ATTACK1 = "Attack1";
    const string PLAYER_ATTACK2 = "Attack2";
    const string PLAYER_ATTACK3 = "Attack3";
    const string PLAYER_ROLL = "Roll";
    const string PLAYER_WALLSLIDE = "WallSlide";
    const string PLAYER_HIT = "Hit";
    const string PLAYER_BLOCK = "Block";
    const string PLAYER_BLOCKIDLE = "BlockIdle";

    private void Awake()
    {
        animator = this.GetComponent<Animator>();
        playerCon = this.GetComponent<PlayerControl>();
    }

    // Script로 제어를 하기 위해 Animator에 Animation을 바꿔주는 함수
    private void ChangeAnimationState(string nextState)
    {
        if (currentState == nextState) return;

        animator.Play(nextState);

        currentState = nextState;
    }

    private void FixedUpdate()
    {
        // 땅에 붙어있는지
        if (playerCon.IsGround)
        {
            if (playerCon.IsHit)
                ChangeAnimationState(PLAYER_HIT);
            else if (playerCon.IsBlock)
            {
                if (currentState != PLAYER_BLOCK && currentState != PLAYER_BLOCKIDLE)
                    ChangeAnimationState(PLAYER_BLOCK);
                else if (isReadyToBlockIdle)
                    ChangeAnimationState(PLAYER_BLOCKIDLE);
            }
            else if (playerCon.IsRoll)
                ChangeAnimationState(PLAYER_ROLL);
            else if (playerCon.AttackForAnim == 1)
                ChangeAnimationState(PLAYER_ATTACK1);
            else if (playerCon.AttackForAnim == 2)
                ChangeAnimationState(PLAYER_ATTACK2);
            else if (playerCon.AttackForAnim == 3)
                ChangeAnimationState(PLAYER_ATTACK3);
            else if (playerCon.IsRun)
                ChangeAnimationState(PLAYER_RUN);
            else
            {
                ChangeAnimationState(PLAYER_IDLE);

                isReadyToBlockIdle = false;
            }
        }
        else
        {
            isReadyToBlockIdle = false;

            if (playerCon.Rigid2D.velocity.y > 0.0f)
                ChangeAnimationState(PLAYER_JUMP);
            else if (playerCon.Rigid2D.velocity.y < 0.0f)
            {
                if (playerCon.IsWallSlide)
                    ChangeAnimationState(PLAYER_WALLSLIDE);
                else
                    ChangeAnimationState(PLAYER_FALL);
            }
        }
    }

    // PlayerControl에서 ComboAttack 코루틴에서 사용하는 함수
    public float GetAttackAnimLength(int index)
    {
        if (index < 0 || index >= AttackClips.Length) return 0.0f;

        return AttackClips[index].length;
    }

    // PlayerControl에서 구르기를 판단하는 함수
    public bool IsAbleToRoll()
    {
        if (currentState == PLAYER_ATTACK1 ||
            currentState == PLAYER_ATTACK2 ||
            currentState == PLAYER_ATTACK3 ||
            currentState == PLAYER_IDLE ||
            currentState == PLAYER_RUN)
            return true;

        return false;
    }

    // PlayerControl 스크립트에서 연속공격 구분을 위한 함수
    public int AttackState()
    {
        if (currentState == PLAYER_ATTACK1)
            return 1;
        else if (currentState == PLAYER_ATTACK2)
            return 2;
        else if (currentState == PLAYER_ATTACK3)
            return 3;
        else if (currentState == PLAYER_IDLE || currentState == PLAYER_RUN)
            return 0;
        else
            return -1;
    }

    // 구르기 종료를 위한 Animation Event 함수
    private void StopRoll()
    {
        playerCon.IsRoll = false;
        playerCon.Rigid2D.velocity = Vector2.zero;
    }

    // Block Start -> Block Idle로 바뀌기 위한 Animation Event 함수
    private void ToBlockIdle()
    {
        isReadyToBlockIdle = true;
    }

    // WallSlide의 SlideDust 연출 위한 Animation Event 함수
    private void SlideDust()
    {
        if (Slide_Dust != null)
        {
            Vector2 pos = this.transform.position;
            pos.y += 1.5f;
            if (playerCon.IsFlipX)
                pos.x -= 0.3f;
            else
                pos.x += 0.3f;

            GameObject dust = Instantiate(Slide_Dust, pos, transform.rotation);
            dust.GetComponent<SpriteRenderer>().flipX = playerCon.IsFlipX;
        }
    }
}
