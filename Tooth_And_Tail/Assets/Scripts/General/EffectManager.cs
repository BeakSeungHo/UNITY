using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance = null;
    // Start is called before the first frame update

    //이펙트 활성화
    public void EffectEnable(GameObject Object, ParticleObject.PARTICLETYPE Type)
    {
        GameObject effectObject = null;

        effectObject = PoolManager.Instance.PullObject(Pool_ObjType.ParticleEffect);
        ParticleObject effect = effectObject.GetComponent<ParticleObject>();

        if (null == effect)
        {
            Debug.Log("Effect failed. Object pulled is not Effect.");
            return;
        }
        effect.Ready(Object, Type);

        //PoolManager.Instance.PushObject(effectObject, Pool_ObjType.Effect);
    }

    public void EffectEnable(Vector3 Position, ParticleObject.PARTICLETYPE Type)
    {
        GameObject effectObject = null;

        effectObject = PoolManager.Instance.PullObject(Pool_ObjType.ParticleEffect);
        ParticleObject effect = effectObject.GetComponent<ParticleObject>();

        if (null == effect)
        {
            Debug.Log("Effect failed. Object pulled is not Effect.");
            return;
        }
        effect.Ready(Position, Type);
    }
    public void EffectScaleEnable(Vector3 Position,float Scale,ParticleObject.PARTICLETYPE Type)
    {
        GameObject effectObject = null;

        effectObject = PoolManager.Instance.PullObject(Pool_ObjType.ParticleEffect);
        ParticleObject effect = effectObject.GetComponent<ParticleObject>();

        if (null == effect)
        {
            Debug.Log("Effect failed. Object pulled is not Effect.");
            return;
        }
        effect.Ready(Position, Scale, Type);
    }
    //화염방사 활성화
    public void BoarEffectEnable(GameObject Object, GameObject Target, ParticleObject.PARTICLETYPE Type)
    {
        GameObject effectObject = null;

        effectObject = PoolManager.Instance.PullObject(Pool_ObjType.ParticleEffect);
        ParticleObject effect = effectObject.GetComponent<ParticleObject>();

        if (null == effect)
        {
            Debug.Log("ParticleEffect failed. Object pulled is not Effect.");
            return;
        }
        effect.Ready(Object, Target, Type);
    }

    public void SmokeEffectEnable(GameObject Object, Vector3 Pos, float Scale, bool flag, ParticleObject.PARTICLETYPE Type)
    {
        GameObject effectObject = null;

        effectObject = PoolManager.Instance.PullObject(Pool_ObjType.ParticleEffect);
        ParticleObject effect = effectObject.GetComponent<ParticleObject>();

        if (null == effect)
        {
            Debug.Log("FireParticleEffect failed. Object pulled is not Effect.");
            return;
        }
        effect.Ready(Object, Pos, Scale, flag, Type);
    }

    public void FontEffectEnable(GameObject Object, int NumIndex, FontEffect.FONTTYPE Type)
    {
        GameObject effectObject = null;

        effectObject = PoolManager.Instance.PullObject(Pool_ObjType.FontEffect);
        FontEffect effect = effectObject.GetComponent<FontEffect>();

        if (null == effect)
        {
            Debug.Log("FontEffect failed. Object pulled is not Effect.");
            return;
        }
        effect.Ready(Object, NumIndex, Type);
    }
    public void SpriteEffectEnable(GameObject Object, SpriteEffect.SPRITETYPE Type)
    {
        GameObject effectObject = null;

        effectObject = PoolManager.Instance.PullObject(Pool_ObjType.SpriteEffect);
        SpriteEffect effect = effectObject.GetComponent<SpriteEffect>();
        Debug.Log(Object);
        if (null == effect)
        {
            Debug.Log("SpriteEffect failed. Object pulled is not Effect.");
            return;
        }
        effect.Ready(Object, Type);
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }


}
