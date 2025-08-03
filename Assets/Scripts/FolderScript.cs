using Unity.Cinemachine;
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

        LevelManager.Instance.LoopManager.RegisterTriggerableCallback(_pos, TryTeleport);
        
        GetComponent<Float>().Init();
    }

    public void TryTeleport(int loopIndex)
    {
        if (tpToFolder == null)
        {
            return;
        }

        _player = LevelManager.Instance.MainCharacter;
        if (loopIndex == LevelManager.Instance.LoopManager.CurrentLoops && _player._currentPosition == _pos && _player.DestPosition == _pos)
        {
            CinemachineImpulseSource cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
            cinemachineImpulseSource.GenerateImpulse();

            Vector2 tpPos = tpToFolder.GetComponent<FolderScript>()._pos;
            _player.TeleportMainCharacter(_gridManager.GetTileAtPosition(tpPos));
        }
        else
        {
            // Clone is triggering this
            // See LoopInstance, loopinstance is handling this
        }
    }

    public Vector2 GetTeleportLocation()
    {
        return tpToFolder.GetComponent<FolderScript>()._pos;
    }
}
