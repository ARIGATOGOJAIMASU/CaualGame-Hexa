using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JP
{
    public enum BlockType { Red, Blue, Green, Yellow, Orange, Purple }

    public struct NeviInfo
    {
        public NeviInfo(Vector3 targetPosition, BoardPosition boardPosition)
        {
            this.targetPosition = targetPosition;
            this.boardPosition = boardPosition;
        }

        public Vector3 targetPosition;
        public BoardPosition boardPosition;
    }

    public class Block : MonoBehaviour
    {
        [SerializeField] BlockType myType;
        [SerializeField] float MoveSpeed;

        Board currentBoard;
        public Board preveBoard;
        Transform trans;
        Notice notice;

        public BlockType GetBlockType { get {return myType; }  }

        private void Start()
        {
            trans = GetComponent<Transform>();
        }

        public void SetNotice(Notice notice)
        {
            this.notice = notice;
        }

        public void SetBoard(Board board)
        {
            preveBoard = currentBoard;
            currentBoard = board;
        }

        public void Move(Vector3 position)
        {
            StartCoroutine(MoveBlock(position));
        }

        IEnumerator MoveBlock(Vector3 position)
        {
            while (true)
            {
                yield return null;

                if(trans.position != position)
                {
                    trans.position = Vector3.MoveTowards(trans.position, position, MoveSpeed * Time.deltaTime);
                }
                else
                {
                    currentBoard.ArriveBlock();
                    notice.notifyObserver("MoveArrive", currentBoard.MyBoardPosition);
                    yield break;
                }
            }
        }
    }
}
