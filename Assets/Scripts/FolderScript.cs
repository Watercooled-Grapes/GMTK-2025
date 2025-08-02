using UnityEngine;

public class FolderScript : MonoBehaviour
{
    [SerializeField] private Vector2 _pos;
    [SerializeField] private GameObject tpToFolder;

    private MainCharacter _player;
    private GridManager _gridManager;

    public void Init(int[,] mapData)
    {   
        _gridManager = LevelManager.Instance.GridManager;
        if (_gridManager == null)
        {
            Debug.LogError("GridManager not found!");
            return;
        }
        Vector3 pos = _gridManager.GetTileCenterPosition(_pos);
        transform.position = pos;
    }

    public FolderScript tryTp()
    {
        if (tpToFolder == null)
        {
            return null;
        }

        _player = LevelManager.Instance.MainCharacter;
        if (_player._currentPosition == _pos)
        {
            Vector2 tpPos = tpToFolder.GetComponent<FolderScript>()._pos;
            _player.TeleportMainCharacter(_gridManager.GetTileAtPosition(tpPos));
            return this;
        }
        return null;
    }

    public Vector2 getLocation()
    {
        return _pos;
    }

    public Vector2 tpLocation()
    {
        return tpToFolder.GetComponent<FolderScript>().getLocation();
    }

    public void Update()
    {
        
    }
}
