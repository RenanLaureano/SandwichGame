using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    public enum MoveDirection
    {
        NONE,
        UP,
        DOWN,
        RIGHT,
        LEFT
    }

    private const float TILE_HEIGHT = 0.1f;

    private GridController gridController = null;
    private GameController gameController = null;
    private bool inMoving = false;

    private int animMoveID;
    private int animRotateID;

    void Start()
    {
        gridController = GetComponent<GridController>();
        gameController = GetComponent<GameController>();
    }

    public void MoveToDirection(TileNodeObject selectedObject, MoveDirection direction)
    {
        if (inMoving)
        {
            return;
        }

        selectedObject = selectedObject.GetParentAll();

        TileNodeObject[,] grid = gridController.GetGrid();
        TileNodeObject target = GetTargetByDirection(grid, selectedObject.TileInfo, direction);

        if(target == null)
        {
            return;
        }

        TileInfo tileInfo = selectedObject.TileInfo;
        grid[tileInfo.row, tileInfo.column] = null;

        //Calcule new position
        float targetHeight = CalculateHeight(target);
        float selectedHeight = CalculateHeight(selectedObject);
        Vector3 targetPosition = target.transform.position;
        targetPosition.y = targetHeight + selectedHeight;

        //Calcule new rotation
        Vector3 rotationAngle = GetRotationAngle(direction);
        Vector3 targetRotation = selectedObject.transform.rotation.eulerAngles + new Vector3(rotationAngle.x, 0, rotationAngle.z);

        inMoving = true;

        target.AddChild(selectedObject);
        selectedObject.SetParent(target);

        animRotateID = selectedObject.transform.LeanRotate(targetRotation, 0.3f).uniqueId;

        animMoveID = selectedObject.transform
            .LeanMove(targetPosition, 0.3f)
            .setOnComplete(() =>
            {
                selectedObject.transform.SetParent(target.transform);
                selectedObject.UpdatePositionTileInfo(target.TileInfo.row, target.TileInfo.column);
                inMoving = false;
                gameController.Moved(selectedObject.GetParentAll());
            }).uniqueId;



    }

    private TileNodeObject GetTargetByDirection(TileNodeObject[,] grid, TileInfo selectedTile, MoveDirection direction)
    {
        TileNodeObject target = null;

        if (direction == MoveDirection.UP || direction == MoveDirection.DOWN)
        {
            int row = selectedTile.row + (direction == MoveDirection.UP ? 1 : -1);

            if(gridController.GridExist(row, selectedTile.column))
            {
                target = grid[row, selectedTile.column];
            }
        }
        else if (direction == MoveDirection.LEFT || direction == MoveDirection.RIGHT)
        {
            int column = selectedTile.column + (direction == MoveDirection.RIGHT ? 1 : -1);

            if (gridController.GridExist(selectedTile.row, column))
            {
                target = grid[selectedTile.row, column];
            }
        }

        if (target != null && target.isAvailable)
        {
            return target;
        }

        return null;
    }

    private float CalculateHeight(TileNodeObject tileNode)
    {
        var height = TILE_HEIGHT / 2.0f;
        var childCount = tileNode.ChildrenCount;
        height += childCount * TILE_HEIGHT;

        return height;
    }

    public Vector3 GetRotationAngle(MoveDirection direction)
    {
        switch (direction)
        {
            case MoveDirection.UP:
                return new Vector3(-180.0f, 0, 0);
            case MoveDirection.RIGHT:
                return new Vector3(0, 0, 180.0f);
            case MoveDirection.LEFT:
                return new Vector3(0, 0, -180.0f);
            case MoveDirection.DOWN:
                return new Vector3(180.0f, 0, 0);
        }

        return Vector3.zero;
    }
}
