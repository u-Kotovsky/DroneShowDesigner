using Drones;
using UnityEngine;

public class BaseMobile : MonoBehaviour
{
    public FixtureSpawnManager spawnManager;
    public int globalChannelStart;
    protected byte[] Buffer;
    
    private void Awake()
    {
        Buffer = new byte[12]; // or 6 channels, depends if position are required or rotation
    }

    #region Position
    private byte xPositionCoarse;
    private byte xPositionFine;

    private byte yPositionCoarse;
    private byte yPositionFine;

    private byte zPositionCoarse;
    private byte zPositionFine;

    protected float MinPosition = -50;
    protected float MaxPosition = 50;
    
    public void WriteDmxPosition(int offset, Vector3 position, bool flipYtoZ = false)
    {
        xPositionCoarse = Utility.GetCoarse(position.x, MinPosition, MaxPosition);
        xPositionFine = Utility.GetFine(position.x, MinPosition, MaxPosition);
        
        yPositionCoarse = Utility.GetCoarse(position.y, MinPosition, MaxPosition);
        yPositionFine = Utility.GetFine(position.y, MinPosition, MaxPosition);
        
        zPositionCoarse = Utility.GetCoarse(position.z, MinPosition, MaxPosition);
        zPositionFine = Utility.GetFine(position.z, MinPosition, MaxPosition);
        
        Buffer[offset + 0] = xPositionCoarse;
        Buffer[offset + 1] = xPositionFine;
    
        Buffer[offset + (flipYtoZ ? 4 : 2)] = yPositionCoarse;
        Buffer[offset + (flipYtoZ ? 5 : 3)] = yPositionFine;
    
        Buffer[offset + (flipYtoZ ? 2 : 4)] = zPositionCoarse;
        Buffer[offset + (flipYtoZ ? 3 : 5)] = zPositionFine;
    }
    #endregion

    #region Rotation
    protected byte xRotationCoarse;
    protected byte xRotationFine;

    protected byte yRotationCoarse;
    protected byte yRotationFine;

    protected byte zRotationCoarse;
    protected byte zRotationFine;

    protected float MinAngle = -180;
    protected float MaxAngle = 180;
    
    public void WriteDmxRotation(int offset, Vector3 rotation, bool flipYtoZ = false)
    {
        var xRotationCoarseFine = CoarseFineChannelSet.GetCoarseFineChannelRepresentation(Mathf.InverseLerp(MinAngle, MaxAngle, rotation.x));
        xRotationCoarse = xRotationCoarseFine.coarse;
        xRotationFine = xRotationCoarseFine.fine;
        
        var yRotationCoarseFine = CoarseFineChannelSet.GetCoarseFineChannelRepresentation(Mathf.InverseLerp(MinAngle, MaxAngle, rotation.y));
        yRotationCoarse = yRotationCoarseFine.coarse;
        yRotationFine = yRotationCoarseFine.fine;
        
        var zRotationCoarseFine = CoarseFineChannelSet.GetCoarseFineChannelRepresentation(Mathf.InverseLerp(MinAngle, MaxAngle, rotation.z));
        zRotationCoarse = zRotationCoarseFine.coarse;
        zRotationFine = zRotationCoarseFine.fine;
        
        Buffer[offset + 0] = xRotationCoarse;
        Buffer[offset + 1] = xRotationFine;
    
        Buffer[offset + (flipYtoZ ? 4 : 2)] = yRotationCoarse;
        Buffer[offset + (flipYtoZ ? 5 : 3)] = yRotationFine;
    
        Buffer[offset + (flipYtoZ ? 2 : 4)] = zRotationCoarse;
        Buffer[offset + (flipYtoZ ? 3 : 5)] = zRotationFine;
    }
    #endregion
    
    public byte[] GetDmxData()
    {
        WriteDmxPosition(0, transform.position);
        WriteDmxRotation(6, transform.rotation.eulerAngles);
        return Buffer;
    }
}
