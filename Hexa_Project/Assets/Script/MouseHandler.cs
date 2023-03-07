using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JP
{
    public class MouseHandler : MonoBehaviour
    {
        Board firstClickBlock;
        Board secondClickBlock;
        Vector2 firstBoardPosition;
        Vector2 secondBoardPosition;

        bool isDrag;

        public void MouseUpdate()
        {
            MouseButtonDownCheck();
            MouseDragCheck();
            MouseButtonUpCheck();
        }

        void MouseButtonDownCheck()
        {
            if(Input.GetMouseButtonDown(0))
            {
                RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));

                //Board가 아니라면 패스
                if (!hit)
                {
                    return;
                }

                firstClickBlock = hit.transform.GetComponent<Board>();
                firstBoardPosition = firstClickBlock.MyBoardPosition;
                isDrag = true;
            }
        }

        void MouseDragCheck()
        {
            if (isDrag)
            {
                RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));

                //Board가 아니라면 패스
                if (!hit || firstClickBlock.transform == hit.transform)
                {
                    return;
                }

                isDrag = false;

                secondClickBlock = hit.transform.GetComponent<Board>();
                secondBoardPosition = secondClickBlock.MyBoardPosition;

                firstClickBlock.SwapBlock(secondClickBlock);
                firstClickBlock = null;
            }
        }

        void MouseButtonUpCheck()
        {
            if (Input.GetMouseButtonUp(0))
            {
                isDrag = false;
            }
        }

        public Vector2 GetFirstBoardPosition()
        {
            return firstBoardPosition;
        }

        public Vector2 GetSecondBoardPosition()
        {
            return secondBoardPosition;
        }
    }
}
