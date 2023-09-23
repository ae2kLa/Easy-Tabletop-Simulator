using Mirror;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachArea : OutLineObj, IAttachable
{
    public GridData Grid;

    public override void OnStartServer()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
    }

    [Server]
    public void Attach(DragObject dragObject)
    {
        if (Grid.Occupied) return;

        Grid.Occupied = true;
        Grid.OccupiedGO = dragObject.gameObject;
        StartCoroutine(dragObject.ApplyAttachTransform(transform, () =>
        {
            var rb = dragObject.transform.GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeAll;
            rb.freezeRotation = true;
        }));

        //TODO:‘⁄¥À≈–∂œ §∏∫
    }

}
