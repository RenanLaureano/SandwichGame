using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
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

    [Header("Events")]
    [SerializeField] private VoidEvent onCancelCommand;

    [Header("Varibles")]
    [SerializeField] private IntVariable gameStateVariable;
    [SerializeField] private GameObjectVariable movedIgredient;

    private GridController gridController = null;

    private bool inMoving = false;

    private int animMoveID;
    private int animRotateID;

    private bool InMoving { set {
            gameStateVariable.SetValue(value ? (int)GameController.GameState.ANIMING : (int)GameController.GameState.PLAYING);
            inMoving = value;
        } }



void Start()
    {
        gridController = GetComponent<GridController>();
        ServiceLocator.Instance.Register<MoveController>(this);
    }

    public LTDescr MoveToDirection(TileNodeObject selectedObject, MoveDirection direction)
    {
        if (inMoving)
        {
            return null;
        }

        selectedObject = selectedObject.GetParentAll();

        TileNodeObject[,] grid = gridController.GetGrid();
        TileNodeObject target = GetTargetByDirection(grid, selectedObject.TileInfo, direction);

        if (target == null)
        {
            onCancelCommand.Raise();
            return null;
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

        InMoving = true;

        target.AddChild(selectedObject);
        selectedObject.SetParent(target);

        animRotateID = selectedObject.transform.LeanRotate(targetRotation, 0.3f).uniqueId;

        LTDescr animMove = selectedObject.transform
            .LeanMove(targetPosition, 0.3f)
            .setOnComplete(() =>
            {
                selectedObject.transform.SetParent(target.transform);
                selectedObject.UpdatePositionTileInfo(target.TileInfo.row, target.TileInfo.column);
                InMoving = false;
                movedIgredient.SetValue(selectedObject.GetParentAll().gameObject);
            });

        animMoveID = animMove.uniqueId;

        return animMove;
    }

    public LTDescr UndoDirection(TileNodeObject selectedObject, MoveDirection direction, Vector3 targetPosition, Vector3 targetRotation)
    {
        if (inMoving)
        {
            return null;
        }

        TileNodeObject[,] grid = gridController.GetGrid();
        TileNodeObject target = GetTargetByDirection(grid, selectedObject.TileInfo, direction);

        TileInfo tileInfo = selectedObject.TileInfo;

        InMoving = true;

        selectedObject.GetParentAll().RemoveChild(selectedObject);

        if (target != null)
        {
            target.AddChild(selectedObject);
            selectedObject.SetParent(target);
        }
        else
        {
            selectedObject.RemoveParent();
        }
        animRotateID = selectedObject.transform.LeanRotate(targetRotation, 0.3f).uniqueId;

        LTDescr animMove = selectedObject.transform
            .LeanMove(targetPosition, 0.3f)
            .setOnComplete(() =>
            {
                if (target != null)
                {
                    selectedObject.transform.SetParent(target.transform);
                }
                else
                {
                    selectedObject.transform.SetParent(null);
                }

                int row = selectedObject.TileInfo.row;
                int column = selectedObject.TileInfo.column;

                if(direction == MoveDirection.DOWN || direction == MoveDirection.UP)
                {
                    row += direction == MoveDirection.UP ? 1 : -1;
                }
                else if (direction == MoveDirection.RIGHT || direction == MoveDirection.LEFT)
                {
                    column += direction == MoveDirection.RIGHT ? 1 : -1;
                }

                selectedObject.UpdatePositionTileInfo(row, column);
                grid[row, column] = selectedObject;

                InMoving = false;
            });

            animMoveID = animMove.uniqueId;

        return animMove;
    }

    private TileNodeObject GetTargetByDirection(TileNodeObject[,] grid, TileInfo selectedTile, MoveDirection direction)
    {
        TileNodeObject target = null;

        if (direction == MoveDirection.UP || direction == MoveDirection.DOWN)
        {
            int row = selectedTile.row + (direction == MoveDirection.UP ? 1 : -1);

            if (gridController.GridExist(row, selectedTile.column))
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
