using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tabletop.Local
{
    public abstract class LocalContainerObj : LocalOutLineObj, LocalIAttachable
    {
        protected Collider m_collider;
        public Collider Collider => m_collider;

        protected List<LocalDragObj> Contents = new List<LocalDragObj>();
        protected List<Type> ContainTypes = new List<Type>();

        [ToggleLeft]
        public bool CountUnlimitedToggle = false;
        [EnableIf("CountUnlimitedToggle")]
        public GameObject CountUnlimitedPrefab;

        [HideInInspector] public LocalDragObj CurrentDragObj = null;


        protected override void Init()
        {
            base.Init();

            m_collider = transform.Find("model").GetComponent<Collider>();

            AddContainTypes();
            foreach (var subClassType in ContainTypes)
            {
                if (CountUnlimitedPrefab.TryGetComponent(out LocalDragObj dragObject))
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
        protected abstract bool AddCondition(LocalDragObj dragObj);

        public void Attach(LocalDragObj dragObject)
        {
            Add(dragObject);
        }

        public void Add(LocalDragObj dragObject)
        {
            if (!ContainTypes.Contains(dragObject.GetType()) || !AddCondition(dragObject))
            {
                print("该容器不装载此物体");
                return;
            }

            dragObject.BeAdd();
            Contents.Add(dragObject);
        }

        /// <summary>
        /// 为真时可以操作
        /// </summary>
        /// <returns></returns>
        protected abstract bool CheckHandleAddition(GoChessColor goChessColor);

        public override void OnMouseDown()
        {
            Get(LocalPracticeController.Instance.PlayerColor);
        }

        public LocalDragObj Get(GoChessColor goChessColor)
        {
            if (!CheckHandleAddition(goChessColor))
            {
                print("你不能使用对方的棋篓");
                return null;
            }

            if (Contents.Count == 0)
            {
                if (CountUnlimitedToggle)
                {
                    var go = Instantiate(CountUnlimitedPrefab,
                        transform.position + Vector3.up * 1f, Quaternion.identity);
                    CurrentDragObj = go.GetComponent<LocalDragObj>();
                    CurrentDragObj.Container = this;
                    AfterGenerate(CurrentDragObj);
                }
                else
                {
                    print("容器是空的");
                    return null;
                }
            }
            else
            {
                CurrentDragObj = Contents[Contents.Count - 1];
                Contents.RemoveAt(Contents.Count - 1);
            }

            CurrentDragObj.transform.position = transform.position + Vector3.up * 1f;
            CurrentDragObj.MouseDown();
            CurrentDragObj.BeGet();
            return CurrentDragObj;
        }

        protected abstract void AfterGenerate(LocalDragObj dragObj);

        public void OnMouseDrag()
        {
            Vector3 hitPos;
            if (Vector3Utils.GetClosetPoint(Input.mousePosition, transform.position, out hitPos))
            {
                MouseDrag(LocalPracticeController.Instance.PlayerColor, hitPos);
            }
        }

        public void MouseDrag(GoChessColor goChessColor, Vector3 hitPos)
        {
            if (!CheckHandleAddition(goChessColor))
            {
                print("你不能使用对方的棋篓");
                return;
            }

            CurrentDragObj?.MouseDrag(hitPos);
        }

        public void OnMouseUp()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            MouseUp(LocalPracticeController.Instance.PlayerColor, ray);
        }

        public void MouseUp(GoChessColor goChessColor, Ray ray)
        {
            if (!CheckHandleAddition(goChessColor))
            {
                print("你不能使用对方的棋篓");
                return;
            }
            CurrentDragObj?.MouseUp(ray);
        }

    }
}