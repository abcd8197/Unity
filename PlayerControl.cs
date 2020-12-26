using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private Rigidbody2D rigid = null;
    public Rigidbody2D Rigid2D { get => rigid; }

    // 좌우 반전이 되었는지 확인하기 위한 FlipX 프로퍼티
    private SpriteRenderer spRenderer = null;
    public bool IsFlipX { get => spRenderer.flipX; }

    // About Raycast for Movement
    [SerializeField]
    private LayerMask mask;
    [SerializeField]
    private float RayDistance = 1.0f;

    // For AttacckButton
    private Player_Animation_Setting animSet = null;

    private int attackForAnim = 0;
    public int AttackForAnim { get => attackForAnim; }

    // Sensors
    private Player_Sensor GroundSensor = null;
    private Player_Sensor GroundSensorR = null;
    private Player_Sensor GroundSensorL = null;
    private Player_Sensor WallSensorR = null;
    private Player_Sensor WallSensorL = null;

    // 맞았는지 여부
    private bool isHit = false;
    public bool IsHit { get { return isHit; } set { isHit = value; } }
    private float hitCount = 0.0f;

    // 방어중 여부
    private bool isBlock = false;
    public bool IsBlock { get => isBlock; }

    // 착지 여부
    private bool isGround = false;
    public bool IsGround { get => isGround; }

    // 벽 슬라이드 여부
    private bool isWallSlide = false;
    public bool IsWallSlide { get => isWallSlide; }

    // 벽 슬라이드 간격
    private float WallJumpCounter = 0.0f;

    // 구르기 여부
    private bool isRoll = false;
    public bool IsRoll { get { return isRoll; } set { isRoll = value; } }
    private float rollCount = 0.0f;

    // Abillity Values
    [SerializeField]
    private float moveSpeed = 1.0f;
    private float baseMoveSpeed;
    public float MoveSpeed { get => moveSpeed; }

    private bool isPressedMoveL = false;
    private bool isPressedMoveR = false;

    // 이동중 여부
    [SerializeField]
    private bool isRun = false;
    public bool IsRun { get => isRun; }

    // 점프하는 힘
    [SerializeField]
    private float jumpPower = 1.0f;
    public float JumpPower { get => jumpPower; }
    // 벽점프 하는 힘
    [SerializeField]
    private float wallJumpPower = 1.0f;
    public float WallJumpPower { get => wallJumpPower; }
    // 구르는 힘
    [SerializeField]
    private float rollPower = 1.0f;
    public float RollPower { get => rollPower; }

    private void Awake()
    {
        rigid = this.GetComponent<Rigidbody2D>();
        spRenderer = this.GetComponent<SpriteRenderer>();
        animSet = this.GetComponent<Player_Animation_Setting>();

        GroundSensor = this.transform.Find("Ground_Sensor").GetComponent<Player_Sensor>();
        GroundSensorR = this.transform.Find("Ground_Sensor_R").GetComponent<Player_Sensor>();
        GroundSensorL = this.transform.Find("Ground_Sensor_L").GetComponent<Player_Sensor>();
        WallSensorR = this.transform.Find("Wall_Sensor_R").GetComponent<Player_Sensor>();
        WallSensorL = this.transform.Find("Wall_Sensor_L").GetComponent<Player_Sensor>();

        // 공격중, 방어중 일 때 속도 감소를 위해 baseMoveSpeed를 저장
        baseMoveSpeed = moveSpeed;
    }

    private void Update()
    {
        // GroundSensor가 셋중 하나 이상이 충돌하고 떨어지는 중일 때에만 점프 가능
        if ((GroundSensor.IsCollided || GroundSensorR.IsCollided || GroundSensorL.IsCollided) &&
            rigid.velocity.y <= 0.0f)
            isGround = true;
        else
            isGround = false;

        // WallSlide 때 점프를 하고 난 뒤 컨트롤 간격을 주기 위한 Count
        if (WallJumpCounter > 0.0f)
            WallJumpCounter -= Time.deltaTime;

        this.WallSlide();
        this.RollCount();
        this.HitCount();
    }

    // 좌, 우의 WallSensor 충돌된 상태이고(Trigger Enter) 떨어지는 중이라면 WallSlide
    private void WallSlide()
    {
        if ((WallSensorL.IsCollided || WallSensorR.IsCollided) &&
            rigid.velocity.y < 0.0f)
            isWallSlide = true;
        else
            isWallSlide = false;
    }

    // For UI =======================================================

    // UI - MoveButton의 Press상태 일 때 사용하는 함수
    public void MoveMentButton(float Rate)
    {
        if (isRoll || WallJumpCounter > 0.0f) return;

        if (AttackForAnim != 0) moveSpeed = baseMoveSpeed * 0.5f;
        else if (isBlock) moveSpeed = baseMoveSpeed * 0.25f;
        else moveSpeed = baseMoveSpeed;

        if (Rate < 0.0f)
            spRenderer.flipX = true;
        else
            spRenderer.flipX = false;

        Vector2 rayOrigin = this.transform.position;
        rayOrigin.y += 0.75f;
        if (Physics2D.Raycast(rayOrigin, Vector2.right * Rate, 0.55f, mask.value)) return;

        this.transform.Translate(Vector3.right * moveSpeed * Rate * Time.deltaTime);
    }
    public void MoveMentCheck(bool b)
    {
        isRun = b;
    }


    // UI - Jump Button용 함수
    public void JumpButton()
    {
        if (isRoll) return;

        // 기본 점프
        if (isGround)
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }
        // 벽점프
        else if (isWallSlide && WallJumpCounter <= 0.0f)
        {
            rigid.velocity = Vector2.zero;

            if (spRenderer.flipX)    //Left
                rigid.AddForce(new Vector2(0.5f, 1.0f) * wallJumpPower, ForceMode2D.Impulse);
            else
                rigid.AddForce(new Vector2(-0.5f, 1.0f) * wallJumpPower, ForceMode2D.Impulse);

            // 벽점프 후 0.2초 동안은 좌우 컨트롤 불가능
            WallJumpCounter = 0.2f;
            spRenderer.flipX = !spRenderer.flipX;
        }
    }

    // UI - Attack Button용 함수 -------------------------------
    public void AttackButton()
    {
        int state = animSet.AttackState();

        if (state == -1 || isRoll) return;

        // 0 : Idel or Move  --> 공격 시작 가능
        // 1, 2 : Attack1, Attack2 --> 다음 연속 공격 가능
        if (state == 0)
        {
            StartCoroutine(ComboAttack(0));
            SoundManager.Instance.PlayAttackSound(0);
            isRoll = false;
        }
        else if (state == 1)
        {
            StartCoroutine(ComboAttack(1));
            SoundManager.Instance.PlayAttackSound(1);
        }
        else if (state == 2)
        {
            StartCoroutine(ComboAttack(2));
            SoundManager.Instance.PlayAttackSound(2);
        }
    }

    // ComboAttack을 위한 코루틴
    private IEnumerator ComboAttack(int comboNum)
    {
        attackForAnim = comboNum + 1;   // 1, 2, 3 이어야 Attack을 실행하기 때문에 1을 더해준다.

        int preNum = attackForAnim; // 터치가 또 눌렸는지 비교하기위한 값

        // Length를 얻어오기 위해서는 1을 더하지 않고 인덱스 번호로 넘긴다.(Clip배열에서 찾아옴)
        yield return new WaitForSeconds(animSet.GetAttackAnimLength(comboNum) * 0.9f);

        if (attackForAnim == preNum)
            attackForAnim = 0;
    }
    //-------------- Attack Button ----------------------------------

    // UI - Roll Button용 함수
    public void RollButton()
    {
        // Animatin을 관리하는 스크립트에서 현재 애니메이션을 판별
        if (animSet.IsAbleToRoll() == false) return;

        if (isRoll == false)
        {
            // 구르기를 시작할 때 다른 물리적용을 없애기 위해
            rigid.velocity = Vector2.zero;

            if (spRenderer.flipX)
                rigid.AddForce(Vector2.left * rollPower, ForceMode2D.Impulse);
            else
                rigid.AddForce(Vector2.right * rollPower, ForceMode2D.Impulse);

            isRoll = true;
        }
    }

    // 구르기 끝을 위한 변수
    private void RollCount()
    {
        if (isRoll)
        {
            rollCount += Time.deltaTime;

            if (rollCount >= animSet.RollAnimLength * 0.9f)
            {
                rigid.velocity = Vector2.zero;
                rollCount = 0.0f;
                isRoll = false;
            }
        }
    }

    // UI - Block Button용 함수
    public void BlockButton()
    {
        if (isRoll) return;
        isBlock = true;
    }

    // Block Button 조작 없을 때 호출되는 함수
    public void BlockNone()
    {
        isBlock = false;
    }

    // GameManaer 호출용 함수 (전투 시 GameMager 통해 호출하도록)
    public void PlayerHit()
    {
        isHit = true;
        hitCount = animSet.HitAnimLength * 0.9f;
    }

    // Hit 애니메이션 통제를 위한 함수
    private void HitCount()
    {
        if (hitCount > 0.0f)
            hitCount -= Time.deltaTime;
        else
        {
            isHit = false;
            hitCount = 0.0f;
        }
    }
}