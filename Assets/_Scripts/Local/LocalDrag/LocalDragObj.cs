using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Tabletop.Local
{
    public abstract class LocalDragObj : LocalOutLineObj
    {
        /// <summary>
        /// 可拖拽的物体一定有碰撞体
        /// </summary>
        protected Rigidbody m_rigidbody;
        protected Collider m_collider;
        protected GameObject m_currentClone = null;
        protected int m_cloneX = -1;
        protected int m_cloneZ = -1;

        protected BindableProperty<DragObjState> m_dragState;

        [HideInInspector] public LocalContainerObj Container;

        /// <summary>
        /// 拖动时更新的向量
        /// </summary>
        protected Vector3 m_currentVector;

        /// <summary>
        /// 点击后在Y轴有一段偏移，即抬高m_yOffsetToTable个距离
        /// </summary>
        [Range(0.1f, 3f)]
        public float m_yOffsetToTable = 0.1f;
        protected Vector3 m_offset;
        protected float m_yTarget;

        protected override void Init()
        {
            base.Init();

            m_rigidbody = GetComponent<Rigidbody>();
            m_collider = transform.Find("model").GetComponent<Collider>();
            m_dragState = new BindableProperty<DragObjState>(DragObjState.Available);
        }

        /// <summary>
        /// 为真时可以操作
        /// </summary>
        /// <returns></returns>
        protected abstract bool CheckHandleAddition(GoChessColor goChessColor);

        public override void OnMouseDown()
        {
            CmdMouseDown(LocalPracticeController.Instance.PlayerColor);
        }

        public void CmdMouseDown(GoChessColor goChessColor)
        {
            if (!CheckHandleAddition(goChessColor))
            {
                print("你不能使用对方的棋子");
                return;
            }
            MouseDown();
        }

        public void MouseDown()
        {
            if (m_dragState.Value != DragObjState.Available)
            {
                return;
            }
            m_rigidbody.isKinematic = true;
            m_collider.isTrigger = true;
            m_dragState.Value = DragObjState.Moving;
            ChangeLayer("IgnoreRaycast");
        }

        public void OnMouseDrag()
        {
            Vector3 hitPos;
            if (Vector3Utils.GetClosetPoint(Input.mousePosition, transform.position, out hitPos))
            {
                CmdMouseDrag(LocalPracticeController.Instance.PlayerColor, hitPos);
            }
        }

        public void CmdMouseDrag(GoChessColor goChessColor, Vector3 hitPos)
        {
            if (!CheckHandleAddition(goChessColor))
            {
                print("你不能使用对方的棋子");
                return;
            }
            MouseDrag(hitPos);
        }

        public void MouseDrag(Vector3 hitPos)
        {
            if (m_dragState.Value != DragObjState.Moving)
            {
                return;
            }

            var yOffset = new Vector3(0, m_collider.bounds.center.y - m_collider.bounds.min.y, 0);
            transform.position = hitPos + yOffset;
        }


        public void OnMouseUp()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            CmdMouseUp(LocalPracticeController.Instance.PlayerColor, ray);
        }

        public void CmdMouseUp(GoChessColor goChessColor, Ray ray)
        {
            if (!CheckHandleAddition(goChessColor))
            {
                print("你不能使用对方的棋子");
                return;
            }
            MouseUp(ray);
        }


        public void MouseUp(Ray ray)
        {
            if (m_dragState.Value != DragObjState.Moving)
            {
                return;
            }

            m_dragState.Value = DragObjState.Available;
            m_rigidbody.isKinematic = false;
            m_collider.isTrigger = false;
            ChangeLayer("Raycast");

            LocalIAttachable attachObj = RaycastContanier(ray);
            if (attachObj != null)
            {
                attachObj.Attach(this);
            }
        }

        protected LocalIAttachable RaycastContanier(Ray ray)
        {
            RaycastHit[] hits = Physics.RaycastAll(ray);
            foreach (RaycastHit hit in hits)
            {
                //Debug.Log($"检测到AttachObj: {hit.collider.transform.name}");

                Transform hitObjTrans = hit.collider.transform;
                LocalIAttachable attachObj = null;
                if (hitObjTrans.TryGetComponent(out attachObj))
                {
                    return attachObj;
                }
                else if (hitObjTrans.parent != null && hitObjTrans.parent.TryGetComponent(out attachObj))
                {
                    return attachObj;
                }
            }
            return null;
        }


        public virtual IEnumerator ApplyAttachTransform(Transform attachTrans, UnityAction callback = null)
        {
            #region 旧方法看起来不够灵活
            //Vector3 originPos = transform.position;
            //Quaternion originRot = transform.rotation;
            //float t = 0f;
            //while(t < 1f)
            //{
            //    yield return null;
            //    t += Time.deltaTime;
            //    t = t < 1f ? t : 1f;
            //    transform.position = Vector3.Lerp(originPos, m_currentClone.transform.position, t);
            //    transform.rotation = Quaternion.Lerp(originRot, m_currentClone.transform.rotation, t);
            //}
            #endregion

            //应用旋转
            m_dragState.Value = DragObjState.Freeze;
            transform.rotation = attachTrans.rotation;

            //插值应用坐标
            Vector3 targetPos = attachTrans.position +
                new Vector3(0, m_collider.bounds.center.y - m_collider.bounds.min.y, 0);
            while (Vector3.Distance(transform.position, targetPos) >= 0.01f)
            {
                yield return null;
                transform.position = Vector3.Lerp(transform.position, targetPos, 0.05f);
            }
            transform.position = targetPos;

            //重置一些状态变量
            m_currentClone.DestroySelf();
            m_currentClone = null;
            m_rigidbody.isKinematic = false;
            m_collider.isTrigger = false;

            if (callback != null)
                callback();
        }


        protected virtual IEnumerator RecycleDragObject(UnityAction callback = null)
        {
            //回收时取消高光显示
            CancelHighlight();
            m_dragState.Value = DragObjState.Freeze;
            m_rigidbody.isKinematic = true;
            m_collider.isTrigger = true;
            m_currentClone.DestroySelf();
            m_currentClone = null;

            //插值应用坐标
            Vector3 targetPos = Container.transform.position + new Vector3(0, 1f, 0);
            while (Vector3.Distance(transform.position, targetPos) >= 0.01f)
            {
                yield return null;
                transform.position = Vector3.Lerp(transform.position, targetPos, 0.05f);
            }
            transform.position = targetPos;

            //重置一些状态变量
            m_dragState.Value = DragObjState.Available;
            m_rigidbody.isKinematic = false;
            m_collider.isTrigger = false;

            //自动放回棋篓
            Container.Attach(this);

            if (callback != null)
                callback();
        }

        private void ChangeLayer(string layerName)
        {
            Transform[] transforms = GetComponentsInChildren<Transform>();
            foreach (var child in transforms)
            {
                child.gameObject.layer = LayerMask.NameToLayer(layerName);
            }
        }

        public void BeAdd()
        {
            gameObject.SetActive(false);
            transform.position = transform.position + Vector3.up * 1f;
            transform.rotation = Quaternion.identity;
        }

        public void BeGet()
        {
            gameObject.SetActive(true);
        }

        public void RecycleFromContainer()
        {
            StartCoroutine(RecycleDragObject());
        }
    }
}