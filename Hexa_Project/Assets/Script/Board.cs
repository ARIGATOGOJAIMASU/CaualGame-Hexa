using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JP
{
    public class Board : MonoBehaviour
    {
        [System.Serializable]
        public struct BoardPosition
        {
            public BoardPosition(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public int x;
            public int y;
        }

        Block myBlock;
        BoardPosition boardPosition = new();
        public List<Board> AroundBoard = new();
        public bool isCain;

        public void SetBlock(Block block)
        {
            myBlock = block;
            myBlock.Move(transform.position + new Vector3(0, 0, -1));
        }

        public Block GetBlock()
        {
            return myBlock;
        }

        public BlockType GetBlockType()
        {
            return myBlock.GetBlockType;
        }

        public BoardPosition MyBoardPosition 
        { 
            get { return boardPosition; }
            set 
            { 
                boardPosition = value;
                //SetAround();
            }
        }


        //�ڽ����� �� ���� �� ��ġ�� �´��� Ȯ��
        public void CheckBlock()
        {

        }

        public void SwapBlock(Board board)
        {
            //�ڽ� �ֺ��� �ִ� Board���� Ȯ��
            if(!aroundCheck(board.MyBoardPosition))
            {
                return;
            }

            Block tempBlock = board.GetBlock();
            board.SetBlock(myBlock);
            SetBlock(tempBlock);

            //4�� �̻��Ͻ� ��ȯ�Ϸ�
            /*if (Cain(0, GetBlockType()) >= 4)
            {

            }
            //���ϸ� �ٽ� ��ü
            else
            {
                tempBlock = myBlock;
                SetBlock(board.GetBlock());
                board.SetBlock(tempBlock);
            }*/
        }

        public bool aroundCheck(BoardPosition boardPosition)
        {
            //���� ���Ͽ� �������� Ȯ��
            if (Mathf.Abs(this.boardPosition.x - boardPosition.x) > 1)
            {
                return false;
            }

            //���� ���Ͽ� �������� Ȯ��
            if(Mathf.Abs(this.boardPosition.y - boardPosition.y) > 1)
            {
                return false;
            }
            
            return true;
        }

        public void CainStart()
        {
            //4�� �̻��Ͻ� ��ȯ�Ϸ�
            if(Cain(0, GetBlockType()) >= 4)
            {

            }
            //���ϸ� �ٽ� ��ü
            else
            {

            }
        }

        public int Cain(int cainNum, BlockType blockType)
        {
            if (blockType == GetBlockType() && isCain == false)
            {
                isCain = true;
                ++cainNum;

                for (int i = 0; i < AroundBoard.Count; ++i)
                {
                    cainNum =+ AroundBoard[i].Cain(cainNum, blockType);
                }
            }

            return cainNum;
        }
    }
}
