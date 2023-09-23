using Mirror;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using UnityEngine;

public abstract class ContainerObj<T> : OutLineObj, IAttachable where T : DragObject
{
    public readonly SyncList<DragObject> m_contents = new SyncList<DragObject>();

    protected List<Type> ContainTypes = new List<Type>();

    [ToggleLeft]
    public bool CountUnlimitedToggle = false;

    [EnableIf("CountUnlimitedToggle")]
    public GameObject CountUnlimitedPrefab;


    public override void OnStartServer()
    {
        base.OnStartServer();
        Init();
    }


    protected override void Init()
    {
        base.Init();
        AddContainTypes();

        foreach(var subClassType in ContainTypes)
        {
            DragObject dragObject = null;
            if (CountUnlimitedPrefab.TryGetComponent<DragObject>(out dragObject))
            {
                if(!dragObject.GetType().Equals(subClassType))
                {
                    Debug.LogError($"CountUnlimitedPrefab不挂载{subClassType.Name}");
                }
            }
        }

        for(int i = 0; i < 1; i++)
        {
            SupplySingleContent();
        }
    }

    /// <summary>
    /// Example: ContainTypes.Add(typeof(subClass of DragObject));
    /// </summary>
    protected abstract void AddContainTypes();

    public void Attach(DragObject dragObject)
    {
        CmdAdd(dragObject);
    }

    [Command]
    public void CmdAdd(DragObject dragObject)
    {
        if (!ContainTypes.Contains(dragObject.GetType()) || !AddCondition(dragObject))
        {
            print("该容器不装载此物体");
            return;
        }

        m_contents.Add(dragObject);
        dragObject.transform.position = new Vector3(0,0,0);
        dragObject.transform.rotation = Quaternion.identity;
        dragObject.gameObject.SetActive(false);
    }

    protected abstract bool AddCondition(DragObject dragObj);

    [Server]
    public DragObject Get()
    {
        if (m_contents.Count == 0)
        {
            if (CountUnlimitedToggle)
            {
                SupplySingleContent();
            }
            else
            {
                return null;
            }
        }

        print("List count:" + m_contents.Count);
        var res = m_contents[m_contents.Count - 1];
        m_contents.RemoveAt(m_contents.Count - 1);
        res.gameObject.SetActive(true);
        return res;
    }

    /// <summary>
    /// 补充GO
    /// </summary>
    [Server]
    protected virtual void SupplySingleContent()
    {
        var go = Instantiate(CountUnlimitedPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        NetworkServer.Spawn(go, connectionToClient);
        var dragObj = go.GetComponent<DragObject>();
        AfterGenerateHandler(dragObj);
        CmdAdd(dragObj);
    }
    protected abstract void AfterGenerateHandler(DragObject dragObj);


    /// <summary>
    /// ///****************************************
    /// </summary>
    protected uint m_currentNetId = 0;

    public override void OnMouseDown()
    {
        base.OnMouseDown();
        CmdMouseDown();
        //m_currentDragObj?.OnMouseDown();
    }

    [Command]
    public void CmdMouseDown()
    {
        m_currentNetId = Get().netId;

        //NetworkIdentity.s.TryGetValue(netId, out identity);
    }

    public void OnMouseDrag()
    {
        //m_currentDragObj?.OnMouseDrag();
    }

    [Command]
    public void CmdMouseDrag()
    {

    }

    public void OnMouseUp()
    {
        //m_currentDragObj?.OnMouseUp();
        CmdMouseUp();
    }

    [Command]
    public void CmdMouseUp()
    {
        m_currentNetId = 0;
    }
}
