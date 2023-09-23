using Mirror;
using UnityEngine;

public class OutLineObj : NetworkBehaviour
{
    protected Outline m_outline;

    protected virtual void Init()
    {
        if (TryGetComponent<Outline>(out m_outline))
            m_outline.enabled = false;
    }


    protected virtual void OnMouseEnter()
    {
        if(m_outline != null)
            m_outline.enabled = true;
    }

    protected virtual void OnMouseOver()
    {

    }

    protected virtual void OnMouseExit()
    {
        if (m_outline != null)
            m_outline.enabled = false;
    }


    /// <summary>
    /// 操作前获取权限
    /// </summary>
    public virtual void OnMouseDown()
    {
         NetworkClient.localPlayer.GetComponent<Player>().GetAuthority(gameObject);

    }




}
