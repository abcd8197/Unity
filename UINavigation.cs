using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Master Canvas에 들어간다.
public class UINavigation : MonoBehaviour
{
    private List<UIView> lstHistoty = null;
    private string current;

    private void Awake()
    {
        lstHistoty = new List<UIView>();
    }

    public void Push(string name)
    {
        if (current == name) return;

        this.AllHide();

        GameObject viewObj = Instantiate(Resources.Load<GameObject>("UIs/UIViews/" + name), this.transform);
        UIView view = viewObj.GetComponent<UIView>();
        view.Name = name;
        view.Show();
        lstHistoty.Add(view);

        current = name;
    }

    public void Push(UIView view)
    {
        this.AllHide();
        view.Show();

        if (lstHistoty.Contains(view) == false)
            lstHistoty.Add(view);
    }

    public void Pop()
    {
        if (lstHistoty.Count <= 0) return;

        GameObject obj = lstHistoty[lstHistoty.Count - 1].gameObject;

        lstHistoty.RemoveAt(lstHistoty.Count - 1);
        Destroy(obj);

        if (lstHistoty.Count > 0)
        {
            current = lstHistoty[lstHistoty.Count - 1].Name;
            lstHistoty[lstHistoty.Count - 1].Show();
        }
        else
            current = "";
    }

    public void DeleteAll()
    {
        for (int i = lstHistoty.Count - 1; i >= 0; i--)
            Destroy(lstHistoty[i].gameObject);

        lstHistoty.Clear();
        current = "";
    }

    private void AllHide()
    {
        for (int i = 0; i < lstHistoty.Count; i++)
            lstHistoty[i].Hide();
    }
}
