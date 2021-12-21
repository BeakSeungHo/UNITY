using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class State_Idle : IBuildingState
{
    float animTime = 0;

    public void ShowTopper(bool show)
    {
        topper.SetBool("Show", show);
    }

    BuildingState Idle_Gristmill()
    {
        if(animTime == 0)
        {
            for (int i = 0; i < animator.runtimeAnimatorController.animationClips.Length; i++)
            {
                if ("Butcher" == animator.runtimeAnimatorController.animationClips[i].name)
                    animTime = animator.runtimeAnimatorController.animationClips[i].length;
            }
        }
        if (buildingBase.GetBase().MyCamp == Camp.End || animator.GetBool("Idle_Des"))
        {
            topper.SetBool("Show", false);
        }
        else
        {
            if (data.CommonType == CommonType.Gristmill)
            {
                if (topper.GetBool("Show") == false)
                {
                    topper.SetBool("Show", true);
                }

                curTime += Time.deltaTime;

                if (animator.GetBool("Butcher") == false)
                {
                    if (curTime >= 20)
                    {
                        animator.SetBool("Butcher", true);
                        buildingBase.Play_Building_Sound(BuildSoundType.Butcher, 1f, Sound_Channel.Ambient);
                    }
                }
                else
                {
                    if (curTime >= 20 + animTime)
                    {
                        animator.SetBool("Butcher", false);
                        curTime = 0f;
                    }
                }
                topper.SetTrigger("Idle");
            }
        }
        return BuildingState.End;
    }
}