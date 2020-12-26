using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 게임의 전반적인 처리를 위한 Manager
// 전투 관련은 여기서 처리 할 예정  작성일 : 2020 - 12 - 15

public class GameManager : MonoBehaviour
{
    private static GameManager sInstance = null;
    public static GameManager Instance
    {
        get
        {
            if (sInstance == null)
            {
                GameObject newObj = new GameObject("_GameManager");
                sInstance = newObj.AddComponent<GameManager>();
            }
            return sInstance;
        }
    }

    private PlayerControl playerCon = null;
    public PlayerControl PlayerCon { get => playerCon; }
    private UINavigation uiNavigation = null;
    public UINavigation UINavi { get => uiNavigation; }

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        playerCon = GameObject.Find("Player").GetComponent<PlayerControl>();
        uiNavigation = GameObject.Find("Master_Canvas").GetComponent<UINavigation>();

        Application.targetFrameRate = 60;

    }

    // Public Functions --------------------------------------
    public void PlayerHit(int attack)
    {
        if (playerCon != null && playerCon.IsHit == false)
            playerCon.PlayerHit();
    }

    // 열고자 하는 UIWindow 이름
    public void Push(string name)
    {
        uiNavigation.Push(name);
    }

    // Test
    public void Push()
    {
        uiNavigation.Push("UIView_Home");
    }

    public void Pop()
    {
        uiNavigation.Pop();
    }

    public void Delete()
    {
        uiNavigation.DeleteAll();
    }
}
