using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionInfo : MonoBehaviour
{
    public GameObject Sprite = null;
    public GameObject FirePos = null;

    public Collider2D Collider2D = null;

    private Vector2 spriteOffset = Vector2.zero;
    private CommonType commonType = CommonType.End;
    private float accTime = 0f;

    private bool move = false;

    public Vector3 HitPosition { get { return Sprite.transform.localPosition; } }
    public Vector3 FirePosition { get { return HitPosition + FirePos.transform.localPosition; } }

    public bool Ready(CommonType type)
    {
        commonType = type;
        accTime = 0f;
        
        //  Sprite 위치 설정
        switch (type)
        {
            case CommonType.Pigeon:
            case CommonType.Falcon:
            case CommonType.Owl:
            case CommonType.Mouse:
                spriteOffset = new Vector2(0, 0.75f);
                move = true;
                break;
            case CommonType.Squirrel:
                spriteOffset = new Vector2(0.016f, 0.132f);
                move = false;
                break;
            case CommonType.Lizard:
                spriteOffset = new Vector2(0.004f, 0.164f);
                move = false;
                break;
            case CommonType.Toad:
                spriteOffset = new Vector2(0.004f, 0.113f);
                move = false;
                break;
            case CommonType.Mole:
                spriteOffset = new Vector2(0f, 0.137f);
                move = false;
                break;

            case CommonType.Ferret:
                spriteOffset = new Vector2(0.008f, 0.161f);
                move = false;
                break;
            case CommonType.Chameleon:
                spriteOffset = new Vector2(0.004f, 0.232f);
                move = false;
                break;
            case CommonType.Skunk:
                spriteOffset = new Vector2(-0.012f, 0.145f);
                move = false;
                break;
            case CommonType.Snake:
                spriteOffset = new Vector2(0.004f, 0.098f);
                move = false;
                break;

            case CommonType.Boar:
                spriteOffset = new Vector2(0.024f, 0.233f);
                move = false;
                break;
            case CommonType.Badger:
                spriteOffset = new Vector2(0.028f, 0.241f);
                move = false;
                break;
            case CommonType.Wolf:
                spriteOffset = new Vector2(0.012f, 0.273f);
                move = false;
                break;
            case CommonType.Fox:
                spriteOffset = new Vector2(0.048f, 0.332f);
                move = false;
                break;

        }

        Sprite.transform.localPosition = spriteOffset;
        //Collider2D.offset = spriteOffset;

        //  FirePos 위치 설정
        switch (type)
        {
            case CommonType.Squirrel:
                FirePos.transform.localPosition = new Vector3(0.091f, -0.071f, 0f);
                break;
            case CommonType.Lizard:
                FirePos.transform.localPosition = new Vector3(-0.079f, -0.008f, 0f);
                break;
            case CommonType.Toad:
                FirePos.transform.localPosition = new Vector3(0f, 0f, 0f);
                break;
            case CommonType.Pigeon:
                FirePos.transform.localPosition = new Vector3(-0.008f, -0.032f, 0f);
                break;
            case CommonType.Mole:
                FirePos.transform.localPosition = new Vector3(0.127f, -0.107f, 0f);
                break;

            case CommonType.Ferret:
                FirePos.transform.localPosition = new Vector3(0.008f, 0.008f, 0f);
                break;
            case CommonType.Falcon:
                FirePos.transform.localPosition = new Vector3(0.099f, -0.047f, 0f);
                break;
            case CommonType.Chameleon:
                FirePos.transform.localPosition = new Vector3(0.166f, -0.17f, 0f);
                break;
            case CommonType.Skunk:
                FirePos.transform.localPosition = new Vector3(0.126f, -0.008f, 0f);
                break;
            case CommonType.Snake:
                FirePos.transform.localPosition = new Vector3(0.122f, 0.024f, 0f);
                break;

            case CommonType.Boar:
                FirePos.transform.localPosition = new Vector3(0.233f, -0.13f, 0f);
                break;
            case CommonType.Badger:
                FirePos.transform.localPosition = new Vector3(0.213f, -0.122f, 0f);
                break;
            case CommonType.Owl:
                FirePos.transform.localPosition = new Vector3(-0.013f, 0.052f, 0f);
                break;
            case CommonType.Wolf:
                FirePos.transform.localPosition = new Vector3(0.205f, 0.016f, 0f);
                break;
            case CommonType.Fox:
                FirePos.transform.localPosition = new Vector3(0.268f, -0.158f, 0f);
                break;

        }

        //  Layer Order 설정
        var renderer = Sprite.GetComponent<SpriteRenderer>();
        switch (type)
        {
            case CommonType.Pigeon:
            case CommonType.Falcon:
            case CommonType.Owl:
                renderer.sortingOrder = 6;
                break;
            case CommonType.Squirrel:
            case CommonType.Lizard:
            case CommonType.Toad:
            case CommonType.Mole:
            case CommonType.Ferret:
            case CommonType.Skunk:
            case CommonType.Chameleon:
            case CommonType.Snake:
            case CommonType.Boar:
            case CommonType.Badger:
            case CommonType.Wolf:
            case CommonType.Fox:
                renderer.sortingOrder = 5;
                break;
        }

        return true;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!move)
            return;

        switch (commonType)
        {
            case CommonType.Pigeon:
            case CommonType.Falcon:
            case CommonType.Owl:
                accTime += Time.deltaTime * 2f;
                if (accTime > 2 * Mathf.PI)
                {
                    accTime -= 2 * Mathf.PI;
                }
                /*Collider2D.offset = */Sprite.transform.localPosition = spriteOffset + new Vector2(0, 0.1f * Mathf.Sin(accTime));

                break;
            case CommonType.Mouse:
                accTime += Time.deltaTime * 3f;
                spriteOffset.y += (1.5f - accTime) * Time.deltaTime; 

                //spriteOffset.y -=       Time.deltaTime * 0.75f;
                if (spriteOffset.y < 0.096f)
                {
                    spriteOffset.y = 0.096f;
                    move = false;
                }
                /*Collider2D.offset = */Sprite.transform.localPosition = spriteOffset;

                break;
        }


    }
}
