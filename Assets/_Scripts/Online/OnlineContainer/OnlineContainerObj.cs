using Mirror;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tabletop.Online
{
    public abstract class OnlineContainerObj : OnlineOutLineObj, OnlineIAttachable
    {
        protected Collider m_collider;
        public Collider Collider => m_collider;

        public List<OnlineDragObj> Contents = new List<OnlineDragObj>();
        protected List<Type> ContainTypes = new List<Type>();

        [ToggleLeft]
        public bool CountUnlimitedToggle = false;
        [EnableIf("CountUnlimitedToggle")]
        public GameObject CountUnlimitedPrefab;

        public OnlineDragObj CurrentDragObj = null;

        public override void OnStartServer()
        {
            base.OnStartServer();
            Init();
        }

        protected override void Init()
        {
            base.Init();

            m_collider = transform.Find("model").GetComponent<Collider>();

            AddContainTypes();
            foreach (var subClassType in ContainTypes)
            {
                if (CountUnlimitedPrefab.TryGetComponent(out OnlineDragObj dragObject))
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
        protected abstract bool AddCondition(OnlineDragObj dragObj);

        public void Attach(uint playerNid, OnlineDragObj dragObject)
        {
            Add(playerNid, dragObject);
        }

        public void Add(uint playerNid, OnlineDragObj dragObject)
        {
            if (!ContainTypes.Contains(dragObject.GetType()) || !AddCondition(dragObject))
            {
                OnlinePlayerManager.Instance.SendMsg(playerNid, "该容器不装载此物体");
                return;
            }

            dragObject.ServerBeAdd();
            dragObject.RpcBeAdd();
            Contents.Add(dragObject);
        }

        /// <summary>
        /// 为真时可以操作
        /// </summary>
        /// <returns></returns>
        [Server]
        protected virtual bool CheckHandleAddition(uint playerNid)
        {
            return false;
        }

        public override void OnMouseDown()
        {
             CmdGet(NetworkClient.localPlayer.netId);
        }


        [Command(requiresAuthority = false)]
        public void CmdGet(uint playerNid)
        {
            if (!CheckHandleAddition(playerNid))
            {
                OnlinePlayerManager.Instance.SendMsg(playerNid, "你不能使用对方的棋篓");
                return;
            }

            if (Contents.Count == 0)
            {
                if (CountUnlimitedToggle)
                {
                    var go = Instantiate(CountUnlimitedPrefab,
                        transform.position + Vector3.up * 1f, Quaternion.identity);
                    NetworkServer.Spawn(go, connectionToClient);
                    CurrentDragObj = go.GetComponent<OnlineDragObj>();
                    CurrentDragObj.Container = this;
                    AfterGenerate(CurrentDragObj);
                }
                else
                {
                    OnlinePlayerManager.Instance.SendMsg(playerNid, "容器是空的");
                    return;
                }
            }
            else
            {
                CurrentDragObj = Contents[Contents.Count - 1];
                Contents.RemoveAt(Contents.Count - 1);
            }

            CurrentDragObj.transform.position = transform.position + Vector3.up * 1f;
            CurrentDragObj.MouseDown(OnlinePlayerManager.Instance.GetConn(playerNid));

            CurrentDragObj.ServerBeGet();
            CurrentDragObj.RpcBeGet();
        }

        protected virtual void AfterGenerate(OnlineDragObj dragObj)
        {

        }

        public void OnMouseDrag()
        {
            Vector3 hitPos;
            if (Vector3Utils.GetClosetPoint(Input.mousePosition, transform.position, out hitPos))
            {
                CmdMouseDrag(NetworkClient.localPlayer.netId, hitPos);
            }
        }

        [Command(requiresAuthority = false)]
        public void CmdMouseDrag(uint playerNid, Vector3 hitPos)
        {
            if (!CheckHandleAddition(playerNid))
            {
                OnlinePlayerManager.Instance.SendMsg(playerNid, "你不能使用对方的棋篓");
                return;
            }

            CurrentDragObj?.MouseDrag(hitPos);
        }

        public void OnMouseUp()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            CmdMouseUp(NetworkClient.localPlayer.netId, ray);
        }

        [Command(requiresAuthority = false)]
        public void CmdMouseUp(uint playerNid, Ray ray)
        {
            if (!CheckHandleAddition(playerNid))
            {
                OnlinePlayerManager.Instance.SendMsg(playerNid, "你不能使用对方的棋篓");
                return;
            }
            CurrentDragObj?.MouseUp(OnlinePlayerManager.Instance.GetConn(playerNid), playerNid, ray);
        }
    }
}