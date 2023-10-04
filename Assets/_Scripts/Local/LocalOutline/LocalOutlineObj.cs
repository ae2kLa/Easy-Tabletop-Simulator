using UnityEngine;

namespace Tabletop.Local
{
    public abstract class LocalOutLineObj : MonoBehaviour
    {
        protected Outline m_outline;
        protected HighLightState m_highLightState;
        protected Color m_color;

        public void Awake()
        {
            if (TryGetComponent(out m_outline))
            {
                OutlineInit();
            }
            Init();
        }

        protected virtual void Init()
        {

        }

        protected virtual void OnMouseEnter()
        {
            if (m_outline != null && m_highLightState == HighLightState.avaliable)
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

        public void FreezeHighlight(Color color)
        {
            if (m_outline != null)
            {
                m_highLightState = HighLightState.freeze;
                m_outline.OutlineColor = color;
                m_outline.enabled = true;
            }
        }

        public void CancelHighlight()
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