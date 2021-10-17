using UnityEngine;

public class MoveUpCommand : Command
{
    private TileNodeObject tileNodeObject;
    private Vector3 position;
    private Vector3 rotation;

    public MoveUpCommand(TileNodeObject tileNodeObject)
    {
        this.tileNodeObject = tileNodeObject;
    }


    public override void Execute()
    {
        MoveController moveController = ServiceLocator.Instance.GetComponentRegistered<MoveController>();

        //Save state
        position = tileNodeObject.transform.position;
        rotation = tileNodeObject.transform.rotation.eulerAngles;

        moveController.MoveToDirection(tileNodeObject, MoveController.MoveDirection.UP);
    }

    public override void Undo()
    {
        MoveController moveController = ServiceLocator.Instance.GetComponentRegistered<MoveController>();
        moveController.UndoDirection(tileNodeObject, MoveController.MoveDirection.DOWN, position, rotation);
    }
}