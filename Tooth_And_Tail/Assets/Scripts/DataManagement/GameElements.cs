using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eBullet
{
    Error = -1,
    Pistol, Spear, Medkit,
    Artillery , Grenade, Venom,
    Minigun0, Minigun1, Minigun2, Minigun3, Sniper,
    bullet_artillery, bullet_grenade, bullet_medkit_tint,
    bullet_pistol, bullet_sniper, bullet_venom,
    End
}

public class GameElements : ScriptableObject
{
    public Dictionary<eBullet, Sprite> BullletSpriteDic = new Dictionary<eBullet, Sprite>();
    
    public void InitializeElement()
    {
        Sprite[] BulletSprite = Resources.LoadAll<Sprite>("Bullets");
        
        foreach(var i in BulletSprite)
        {
            if (i.name.ToEnum<eBullet>().Equals((int)eBullet.Error))
                Debug.Log("Bullet Sprite Failed");
            else if (!BullletSpriteDic.ContainsKey((eBullet)i.name.ToEnum<eBullet>()))
                BullletSpriteDic.Add((eBullet)i.name.ToEnum<eBullet>(), i);
        }
    }
}
