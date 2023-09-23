using Mirror;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ContainerObj : OutLineObj, IAttachable
{
    public List<DragObject> Contents = new List<DragObject>();

    protected List<Type> ContainTypes = new List<Type>();

    [ToggleLeft]
    public bool CountUnlimitedToggle = false;

    [EnableIf("CountUnlimitedToggle")]
    public GameObject CountUnlimitedPrefab;

    public DragObject CurrentDragObj = null;

    public override void OnStartServer()
    {
        base.OnStartServer();
        Init();
    }


    protected override void Init()
    {
        base.Init();

        AddContainTypes();

        foreach (var subClassType in ContainTypes)
        {
            DragObject dragObject = null;
            if (CountUnlimitedPrefab.TryGetComponent<DragObject>(out dragObject))
            {
                if (!dragObject.GetType().Equals(subClassType))
                {
                    Debug.LogError($"CountUnlimitedPrefab不挂载{subClassType.Name}");
                }
            }
        }
    }

    /// <summary>
    /// Example: ContainTypes.Add(typeof(subClass of DragObject));
    /// </summary>
    protected abstract void AddContainTypes();

    protected abstract bool AddCondition(DragObject dragObj);

    public void Attach(DragObject dragObject)
    {
        Add(dragObject);
    }

    public void Add(DragObject dragObject)
    {
        if (!ContainTypes.Contains(dragObject.GetType()) || !AddCondition(dragObject))
        {
            print("该容器不装载此物体");
            return;
        }

        Contents.Add(dragObject);
        dragObject.gameObject.SetActive(false);
        dragObject.transform.position = this.transform.position + Vector3.up * 10f;
        dragObject.transform.rotation = Quaternion.identity;
    }


    public override void OnMouseDown()
    {
        //base.OnMouseDown();//位置同步异常竟是权限引发的

        CmdGet(Contents);
    }

    [Command(requiresAuthority = false)]
    public void CmdGet(List<DragObject> contents)
    {
        if (contents.Count == 0)
        {
            if (CountUnlimitedToggle)
            {
                var go = Instantiate(CountUnlimitedPrefab,
                    transform.position + Vector3.up * 10f, Quaternion.identity);
                NetworkServer.Spawn(go, connectionToClient);
                CurrentDragObj = go.GetComponent<DragObject>();
                RpcAfterGenerateHandler(CurrentDragObj);
            }
            else
            {
                print("容器是空的");
                return;
            }
        }
        else
        {
            CurrentDragObj = contents[contents.Count - 1];
            contents.RemoveAt(contents.Count - 1);
        }

        CurrentDragObj.OnMouseDown();
        CurrentDragObj.gameObject.SetActive(true);
    }

    [ClientRpc]
    protected virtual void RpcAfterGenerateHandler(DragObject dragObj)
    {

    }

    public void OnMouseDrag()
    {
        CmdDrag();
    }

    [Command(requiresAuthority = false)]
    public void CmdDrag()
    {
        CurrentDragObj?.OnMouseDrag();
    }

    public void OnMouseUp()
    {
        CmdMouseUp();
    }

    [Command(requiresAuthority = false)]
    public void CmdMouseUp()
    {
        CurrentDragObj?.OnMouseUp();
    }
}
