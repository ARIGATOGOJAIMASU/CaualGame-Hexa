using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JP
{
    public class BlockPactory : MonoBehaviour
    {
        [SerializeField] Block[] blocks;
        [SerializeField] List<List<Block>> ActiveBlockList;
        [SerializeField] List<Queue<Block>> DisabledBlockList;

        void Awake()
        {
            DisabledBlockList = new();
            ActiveBlockList = new();

            //블록의 수만큼 할당후 15개 정도 생성
            for (BlockType i = 0; i < BlockType.Block_End; ++i)
            {
                DisabledBlockList.Add(new());
                ActiveBlockList.Add(new());

                for (int j = 0; j < 15; ++j)
                {
                    DisabledBlockList[(int)i].Enqueue(CreateBlock((int)i));
                }
            }
        }

        public Block GetBlock(int blockType)
        {
            Block block;

            //활성화 할 수 있는 오브젝트가 없다면 생성
            if (DisabledBlockList[blockType].Count == 0)
            {
                DisabledBlockList[blockType].Enqueue(CreateBlock(blockType));
            }

            block = DisabledBlockList[blockType].Dequeue();
            ActiveBlockList[blockType].Add(block);
            block.gameObject.SetActive(true);

            return block;
        }

        public void CheckBlock(int blockType)
        {
            //해당 블록들을 검사
            for (int i = 0; i < ActiveBlockList[blockType].Count; ++i)
            {
                //비활성화 블록들을 검사
                if(!ActiveBlockList[blockType][i].gameObject.activeSelf)
                {
                    Block block;
                    block = ActiveBlockList[blockType][i];
                    DisabledBlockList[blockType].Enqueue(block);
                    ActiveBlockList[blockType].Remove(block);
                    --i;
                }
            }
        }

        Block CreateBlock(int blockType)
        {
            Block block = Instantiate(blocks[blockType]);
            
            //옵저버 패턴 적용
            block.gameObject.AddComponent<Notice>();
            block.SetNotice(GetComponent<IObserver>());

            block.gameObject.SetActive(false);

            return block;
        }
    }
}
