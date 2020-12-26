using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIView : MonoBehaviour
{
    [SerializeField]
    private List<Image> lstImages = null;   // WindowPanel 하위에 모든 Image 담기 위한 List
    [SerializeField]
    private List<Text> lstTxts = null;         // WindowPanel 하위에 모든 Text 담기 위한 List

    private string name;
    public string Name { get { return name; } set { name = value; } }
    public delegate void Event();
    public static event Event Events;

    private void Awake()
    {
        // 자신의 하위에 모든 Image와 Text를 담는다.
        if (lstImages.Count == 0)
        {
            lstImages = new List<Image>();
            lstImages.AddRange(this.GetComponentsInChildren<Image>());
        }
        if (lstTxts.Count == 0)
        {
            lstTxts = new List<Text>();
            lstTxts.AddRange(this.GetComponentsInChildren<Text>());
        }
    }

    // UINavigation을 통해 호출되며, 다른 UI창으로 전환 될 때 호출한다.
    public void Hide()
    {
        for (int i = 0; i < lstImages.Count; i++)
        {
            lstImages[i].enabled = false;
            lstImages[i].raycastTarget = false;
        }
        for (int i = 0; i < lstTxts.Count; i++)
            lstTxts[i].enabled = false;
    }

    // UINavigation을 통해 호출되며, 열려있던 UI창이 꺼지고 마지막으로 열렸던 창이 열리게된다.
    public void Show()
    {
        for (int i = 0; i < lstImages.Count; i++)
        {
            lstImages[i].enabled = true;
            lstImages[i].raycastTarget = true;
        }
        for (int i = 0; i < lstTxts.Count; i++)
            lstTxts[i].enabled = true;
    }
}
