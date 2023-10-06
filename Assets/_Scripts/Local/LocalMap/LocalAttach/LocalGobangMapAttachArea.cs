using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tabletop.Local
{
    public class LocalGobangMapAttachArea : LocalMapAttachArea
    {
        public override void Attach(LocalDragObj dragObject)
        {
            //TODO:这个方法届时当下沉到子类
            var piece = dragObject as LocalGoChessPiece;
            if (piece is null)
            {
                print($"所拖拽物体并非围棋棋子");
                return;
            }
            else if (piece.VirtualColor != Map.CurrentColor.Value)
            {
                if (Map.CurrentColor.Value == GoChessColor.Black)
                    print($"当前是黑子回合，白子落子无效");
                else
                    print($"当前是白子回合，黑子落子无效");

                //落子无效时自动将棋子移回棋篓
                StartCoroutine(piece.RecycleDragObject());
                return;
            }

            if (Grid.Occupied) return;
            Grid.Occupied = true;
            Grid.DragObject = dragObject;

            //棋子还没移动到目标点时不允许在棋盘上落子
            var currentColor = Map.CurrentColor.Value;
            Map.CurrentColor.Value = GoChessColor.Unknown;
            StartCoroutine(piece.ApplyAttachTransform(transform, () =>
            {
                //高亮最后一次落子
                Map.LastOutlineObj.Value?.CancelHighlight();
                Map.LastOutlineObj.Value = piece;

                var rb = piece.transform.GetComponent<Rigidbody>();
                rb.constraints = RigidbodyConstraints.FreezeAll;
                rb.freezeRotation = true;

                //TODO:这个方法届时当下沉到子类
                if (Map.GameReferee.CheckWin(piece.VirtualColor, Grid, Grids))
                {
                    print("检测到五子连成一线");

                    //通知某方胜利
                    LocalPracticeController.Instance.WinEvent.Trigger(piece.VirtualColor);
                    Map.CurrentColor.Value = GoChessColor.Unknown;
                    return;
                }

                //回合转换
                if (currentColor == GoChessColor.Black)
                {
                    Map.CurrentColor.Value = GoChessColor.White;
                }
                else if (currentColor == GoChessColor.White)
                {
                    Map.CurrentColor.Value = GoChessColor.Black;
                }
            }));
        }
    }

}
