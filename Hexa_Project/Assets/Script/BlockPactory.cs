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

            //����� ����ŭ �Ҵ��� 15�� ���� ����
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

            //Ȱ��ȭ �� �� �ִ� ������Ʈ�� ���ٸ� ����
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
            //�ش� ��ϵ��� �˻�
            for (int i = 0; i < ActiveBlockList[blockType].Count; ++i)
            {
                //��Ȱ��ȭ ��ϵ��� �˻�
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
            
            //������ ���� ����
            block.gameObject.AddComponent<Notice>();
            block.SetNotice(GetComponent<IObserver>());

            block.gameObject.SetActive(false);

            return block;
        }
    }
}
