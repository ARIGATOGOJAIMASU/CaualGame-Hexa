using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JP
{
    public class MouseHandler : MonoBehaviour
    {
        Board ClickBlock;
        LayerMask layerMask;
        bool isDrag;

        private void Update()
        {
            MouseButtonDownCheck();
            MouseDragCheck();
            MouseButtonUpCheck();
        }      

        void MouseButtonDownCheck()
        {
            if(Input.GetMouseButtonDown(0))
            {
                layerMask = LayerMask.GetMask("Board");

                RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));

                //Board가 아니라면 패스
                if (!hit)
                {
                    return;
                }

                ClickBlock = hit.transform.GetComponent<Board>();
                isDrag = true;
            }
        }

        void MouseDragCheck()
        {
            if (isDrag)
            {
                layerMask = LayerMask.GetMask("Board");
                RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));

                //Board가 아니라면 패스
                if (!hit || ClickBlock.transform == hit.transform)
                {
                    return;
                }

                isDrag = false;
                ClickBlock.SwapBlock(hit.transform.GetComponent<Board>());
                ClickBlock = null;
            }
        }

        void MouseButtonUpCheck()
        {
            if (Input.GetMouseButtonUp(0))
            {
                isDrag = false;
            }
        }
    }
}
