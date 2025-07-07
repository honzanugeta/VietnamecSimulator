using UnityEngine;
using CodeMonkey.Toolkit.TGridSystem;


public class ShelfGridDebugObjectscript : MonoBehaviour, IGridDebugObject
{
    
    private ShelfScript.GridObject gridObject;


    public void SetGridObject(object gridObject) {
        this.gridObject = (ShelfScript.GridObject)gridObject;
    }

}

