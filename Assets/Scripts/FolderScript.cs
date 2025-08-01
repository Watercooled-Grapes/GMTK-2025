using UnityEngine;

public class FolderScript : MonoBehaviour
{
    [SerializeField] private Vector2 _pos;
    [SerializeField] private GameObject tpToFolder;

    private MainCharacter _player;
    private GridManager _gridManager;

    public void Init(int[,] mapData)
    {   
        _gridManager = FindFirstObjectByType<GridManager>();
        if (_gridManager == null)
        {
            Debug.LogError("GridManager not found!");
            return;
        }
        Vector3 pos = _gridManager.GetTileCenterPosition(_pos);
        transform.position = pos;
    }

    public void Update()
    {
        if (tpToFolder == null) 
        {
            return;
        }
        _player = FindFirstObjectByType<MainCharacter>();
        if (_player._currentPosition == _pos)
        {
            Vector2 tpPos = tpToFolder.GetComponent<FolderScript>()._pos;
            _player.TeleportMainCharacter(_gridManager.GetTileAtPosition(tpPos));
        }
        
    }
}
