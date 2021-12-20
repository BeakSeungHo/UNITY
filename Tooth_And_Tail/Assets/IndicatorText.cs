using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IndicatorText : MonoBehaviour
{
    public TextMeshPro tMPro = null;

    private static int checkIndex = 0;
    private static int totalCount = 0;

    public int MyIndex = 0;
    public Vector2Int Location = Vector2Int.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (0 == totalCount)
        {
            var tileBounds = TilemapSystem.Instance.tileBounds;

            totalCount = tileBounds.size.x * tileBounds.size.y;
        }

        if (checkIndex++ == MyIndex)
        {
            if (TilemapSystem.Instance.IsWalkableTile(Location))
                tMPro.text = "true";
            else
                tMPro.text = "false";
        }

        if (checkIndex >= totalCount)
            checkIndex = 0;

    }
}
