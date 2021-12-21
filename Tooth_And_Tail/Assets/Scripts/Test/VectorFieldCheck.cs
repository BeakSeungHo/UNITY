using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorFieldCheck : MonoBehaviour
{
    public GameObject ArrowPrefab = null;
    public GameObject MousePrefab = null;
    public GameObject ClickPrefab = null;
    private Arrow[,] arrowArray = null;
    private GameObject mousePoint = null;
    private GameObject clickPoint = null;
    public Vector3 MouseWorldPosition = Vector3.zero;
    public Vector2Int MouseTilePosition = Vector2Int.zero;
    private bool Init = false;
    private int xMax = 0;
    private int yMax = 0;

    public bool On = false;
    private bool preOn = false;

    public bool TileNumberOn = false;
    private bool preTileNumberOn = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (null == ArrowPrefab)
            return;

        if (!Init)
            Initialize();

        if (Input.GetMouseButtonDown(0))
        {
            ShowVectorField();
        }

        if (preOn != On)
        {
            VisualizeOnOff(On);
        }

        if (preTileNumberOn != TileNumberOn)
        {
            OptionChange();
        }

        preOn = On;
        preTileNumberOn = TileNumberOn;
    }

    bool Initialize()
    {
        Init = true;

        bool initSeccess = true;
        if (null != ArrowPrefab)
        {
            mousePoint = Instantiate(MousePrefab, transform);
            clickPoint = Instantiate(ClickPrefab, transform);

            var tileBounds = TilemapSystem.Instance.tileBounds;

            xMax = tileBounds.size.x;
            yMax = tileBounds.size.y;

            TilemapSystem mapInst = TilemapSystem.Instance;

            arrowArray = new Arrow[tileBounds.size.x, tileBounds.size.y];

            for (int x = 0; x < xMax; ++x)
            {
                for (int y = 0; y < yMax; ++y)
                {
                    var arrowInst = Instantiate(ArrowPrefab, transform);

                    Arrow arrowCom = arrowInst.GetComponent<Arrow>();
                    arrowCom.Ready(x, y);

                    arrowArray[x, y] = arrowCom;
                    arrowInst.SetActive(false);
                }
            }
        }

        return initSeccess;
    }

    Vector2Int Get_MouseTilePosition()
    {
        MouseWorldPosition = InGameManager.Instance.MainCamera.Cam.ScreenToWorldPoint(Input.mousePosition);
        MouseWorldPosition.z = 0f;

        Debug.Log("Mouse World Position : " + MouseWorldPosition);

        mousePoint.transform.position = MouseWorldPosition;

        MouseTilePosition = TilemapSystem.Instance.WorldToTilePos(MouseWorldPosition);
        Debug.Log("Mouse Tile Position : " + MouseTilePosition);

        var node = TilemapSystem.Instance.GetTile(MouseWorldPosition);
        if (null == node)
            Debug.Log("node is null - MousePos : " + MouseWorldPosition + " MouseTilePos : " + MouseTilePosition);
        else
            clickPoint.transform.position = node.worldPosition;

        return MouseTilePosition;
    }

    void ShowVectorField()
    {
        if (!On)
            return;

        var fieldKey = Get_MouseTilePosition();

        var node = TilemapSystem.Instance.GetTile(new Vector2Int(xMax / 2, yMax / 2));

        if (!node.VectorField.ContainsKey(fieldKey))
            return;

        for (int x = 0; x < xMax; ++x)
        {
            for (int y = 0; y < yMax; ++y)
            {
                arrowArray[x, y].TurnToKey(fieldKey);
            }
        }
    }

    void VisualizeOnOff(bool on)
    {
        for (int x = 0; x < xMax; ++x)
        {
            for (int y = 0; y < yMax; ++y)
            {
                arrowArray[x, y].gameObject.SetActive(on);
            }
        }
    }

    void OptionChange()
    {
        for (int x = 0; x < xMax; ++x)
        {
            for (int y = 0; y < yMax; ++y)
            {
                arrowArray[x, y].OptionChange(!TileNumberOn);
            }
        }
    }
}
