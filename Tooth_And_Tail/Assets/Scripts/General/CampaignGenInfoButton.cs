using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

#if UNITY_EDITOR

[CustomEditor(typeof(CampaignManager))]
public class CampaignGenInfoButton : Editor
{
    CampaignManager manager;
    List<ObjectGenNode> List;

    public Transform SetInfoObj(ObjectGenNode node, int posIdx, Transform parent, bool isParent = false)
    {
        GameObject prefabObj = manager.genInfoObj;
        if (!isParent)
        {
            prefabObj = manager.genInfoChild;
        }
        var obj = GameObject.Instantiate(prefabObj, node.GenPosition[posIdx], Quaternion.identity, parent);

        Animator animator = obj.GetComponent<Animator>();

        if (!isParent)
        {
            if (node.Type >= CommonType.Squirrel && node.Type <= CommonType.Fox)
            {
                animator.runtimeAnimatorController = manager.animatorElements.UnitAniDic[node.Type];
            }
            else
                animator.runtimeAnimatorController = manager.animatorElements.BuildAniDic[node.Type];

            // 에디터 상에서 애니메이션 스프라이트가 드러나게 한다.
            animator.runtimeAnimatorController.animationClips[0].SampleAnimation(obj, 50f);
        }
        else
        {
            obj.GetComponent<GenInfo>().genNode = node;
        }

        return obj.transform;
    }


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Show GenInfo"))
        {
            manager = (CampaignManager)target;

            List = manager.genQueue;

            manager.animatorElements.InitializeElement();

            // 먼저 기존에 있던 GenInfo들을 제거한 후 다시 갱신한다.
            GenInfo[] childList = manager.genInfos.GetComponentsInChildren<GenInfo>(true);
            if (childList != null)
            {
                for (int i = 0; i < childList.Length; i++)
                {
                    if (childList[i] != null)
                    {
                        DestroyImmediate(childList[i].gameObject);
                    }
                }
            }

            for (int i = 0; i < List.Count; i++)
            {
                var parent = SetInfoObj(List[i], 0, manager.genInfos, true);
                int posIdx = 0;
                //먼저 설정된 포지션의 갯수만큼 오브젝트를 생성하고 포지션 갯수보다 genCount가 많으면 그만큼 추가로 마지막 위치에 생성한다.
                for(posIdx = 0; posIdx < List[i].GenPosition.Count;posIdx++)
                {
                    SetInfoObj(List[i], posIdx, parent);
                }
                for (int j = posIdx; j < List[i].GenCount; j++)
                {
                    SetInfoObj(List[i], posIdx - 1, parent);
                }
            }
        }

        if (GUILayout.Button("Add GenInfo"))
        {
            manager = (CampaignManager)target;
            List = manager.genQueue;

            manager.animatorElements.InitializeElement();

            var node = manager.CreateGenInfo();

            string folderPath = "Assets/Resources/GenInfo/" + manager.transform.parent.name;
            string fileName = "/GenInfo";

            // 해당 맵 이름으로 폴더가 없으면 만들어준다.
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            if (node.nodeType == GenNodeType.SpawningNode)
            {
                fileName = "/SpawnInfo";
            }

            string name = folderPath + fileName + manager.genQueue.Count.ToString();
            AssetDatabase.CreateAsset(node, name + ".asset");

            AssetDatabase.Refresh();

            manager.genQueue.Add(node);
            
            var parent = SetInfoObj(node, 0, manager.genInfos, true);
            if (node.nodeType != GenNodeType.SpawningNode)
            {
                for (int i = 0; i < node.GenCount; i++)
                {
                    SetInfoObj(node, 0, parent);
                }
            }
        }

        if(GUILayout.Button("Set All GenInfo"))
        {
            // 모든 자식들에 대한 GenInfo를 갱신한다.
            manager = (CampaignManager)target;
            Transform[] childList = manager.genInfos.GetComponentsInChildren<Transform>(true);
            if (childList != null)
            {
                Debug.Log(childList.Length);
                for (int i = 0; i < childList.Length; i++)
                {
                    if (childList[i] != null && childList[i] != manager.genInfos.transform)
                    {
                        GenInfo info = childList[i].GetComponent<GenInfo>();
                        if(info != null)
                        {
                            info.SetGenInfo();
                        }
                    }
                }
            }
        }
    }
}
#endif