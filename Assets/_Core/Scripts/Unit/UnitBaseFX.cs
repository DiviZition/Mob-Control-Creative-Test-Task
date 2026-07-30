using UnityEngine;

public class UnitBaseFX : MonoBehaviour
{
    [SerializeField] private ParticleSystem _spawnVFX;

    //TODO: Consider global VXF pool for optimization if needed
    public void PlaySpawnVFX()
    {
        _spawnVFX.Play();
    }
}
