using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileNodeObject : MonoBehaviour
{
    public bool isAvailable = true;
    private TileInfo tileInfo;
    private List<TileNodeObject> children = new List<TileNodeObject>();
    private TileNodeObject parent;

    public int ChildrenCount
    {
        get
        {
            int childCount = children.Count;
            foreach (TileNodeObject childNode in children)
            {
                childCount += childNode.ChildrenCount;
            }

            return childCount;
        }
    }
    public TileInfo TileInfo { get => tileInfo; }
    public void SetParent(TileNodeObject parent)
    {
        this.parent = parent;
    }

    public void AddChild(TileNodeObject child)
    {
        children.Add(child);
    }

    public void SetTileInfo(TileInfo tileInfo)
    {
        this.tileInfo = tileInfo;
    }

    public void UpdatePositionTileInfo(int row, int column)
    {
        tileInfo.row = row;
        tileInfo.column = column;
    }

    public TileNodeObject GetParentAll()
    {
        TileNodeObject _parent = this;
        while (_parent.parent != null)
        {
            _parent = _parent.parent;
        }

        return _parent;
    }
}
