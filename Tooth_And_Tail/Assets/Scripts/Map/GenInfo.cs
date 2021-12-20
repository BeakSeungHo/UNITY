using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenInfo : MonoBehaviour
{
    Animator animator;
    public ObjectGenNode genNode;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        gameObject.SetActive(false);
    }

    public void SetGenInfo()
    {
        genNode.GenPosition.Clear();
        Transform[] childList = GetComponentsInChildren<Transform>(true);
        if (childList != null)
        {
            for (int i = 0; i < childList.Length; i++)
            {
                if (childList[i] != null && childList[i] != transform)
                    genNode.GenPosition.Add(childList[i].transform.position);
            }
        }
    }
}