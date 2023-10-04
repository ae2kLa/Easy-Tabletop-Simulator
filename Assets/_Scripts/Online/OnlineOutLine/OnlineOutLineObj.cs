using Mirror;
using UnityEngine;

namespace Tabletop.Online
{
    public class OnlineOutLineObj : NetworkBehaviour
    {
        protected Outline m_outline;
        protected HighLightState m_highLightState;
        protected Color m_color;

        public override void OnStartClient()
        {
            if (TryGetComponent(out m_outline))
            {
                OutlineInit();
            }
        }

        protected virtual void Init()
        {

        }

        protected virtual void OnMouseEnter()
        {
            if(m_outline != null && m_highLightState == HighLightState.avaliable)
                m_outline.enabled = true;
        }

        protected virtual void OnMouseOver()
        {

        }

        protected virtual void OnMouseExit()
        {
            if (m_outline != null && m_highLightState == HighLightState.avaliable)
                m_outline.enabled = false;
        }

        public virtual void OnMouseDown()
        {

        }

        public void OutlineInit()
        {
            m_outline.enabled = false;
            m_color = m_outline.OutlineColor;
            m_highLightState = HighLightState.avaliable;
        }

        [ClientRpc]
        public void RpcFreezeHighlight(Color color)
        {
            if (m_outline != null)
            {
                m_highLightState = HighLightState.freeze;
                m_outline.OutlineColor = color;
                m_outline.enabled = true;
            }
        }

        [ClientRpc]
        public void RpcCancelHighlight()
        {
            if (m_outline != null)
            {
                m_highLightState = HighLightState.avaliable;
                m_outline.OutlineColor = m_color;
                m_outline.enabled = false;
            }
        }


    }
}