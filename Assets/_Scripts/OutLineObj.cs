using Mirror;
using UnityEngine;

public enum HighLightState
{
    normal = 0,
    freeze
}

public class OutLineObj : NetworkBehaviour
{
    protected Outline m_outline;
    protected HighLightState m_highLightState;
    protected Color m_color;

    protected virtual void Init()
    {
        m_highLightState = HighLightState.normal;

        if (TryGetComponent<Outline>(out m_outline))
        {
            m_outline.enabled = false;
            m_color = m_outline.OutlineColor;
        }
    }


    protected virtual void OnMouseEnter()
    {
        if(m_outline != null && m_highLightState == HighLightState.normal)
            m_outline.enabled = true;
    }

    protected virtual void OnMouseOver()
    {

    }

    protected virtual void OnMouseExit()
    {
        if (m_outline != null && m_highLightState == HighLightState.normal)
            m_outline.enabled = false;
    }


    /// <summary>
    /// 操作前获取权限--位置同步异常竟是权限引发的
    /// </summary>
    public virtual void OnMouseDown()
    {
        //NetworkClient.localPlayer.GetComponent<Player>().GetAuthority(gameObject);

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
            m_highLightState = HighLightState.normal;
            m_outline.OutlineColor = m_color;
            m_outline.enabled = false;
        }
    }


}
