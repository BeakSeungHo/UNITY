using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Arrow : MonoBehaviour
{
    TileNode node = null;

    public GameObject ArrowSprite = null;
    public GameObject TileText = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Ready(int x, int y)
    {
        node = TilemapSystem.Instance.GetTile(new Vector2Int(x, y));
        transform.position = node.worldPosition;

        TileText.GetComponent<TextMeshPro>().text = x.ToString() + ", " + y.ToString();
        TileText.SetActive(false);
    }

    public void TurnToKey(Vector2Int fieldKey)
    {
        if (TileText.activeSelf)
            return;

        var up = new Vector3(0f, 1f, 0f);

        if (!node.VectorField.ContainsKey(fieldKey))
        {
            Debug.Log("fieldKey does't exist : " + fieldKey);
            return;
        }

        var point = node.VectorField[fieldKey] - transform.position;

        var quaternion = Quaternion.FromToRotation(up, point.normalized);
        transform.eulerAngles = new Vector3(0f, 0f, quaternion.eulerAngles.z);
    }

    public void OptionChange(bool arrowOn)
    {
        if (arrowOn)
            ArrowOn();
        else
            TileTextOn();
    }

    public void ArrowOn()
    {
        TileText.SetActive(false);
        ArrowSprite.SetActive(true);


    }

    public void TileTextOn()
    {
        ArrowSprite.SetActive(false);
        TileText.SetActive(true);

        transform.eulerAngles = Vector3.zero;
    }
}
