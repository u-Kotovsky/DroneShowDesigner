using UnityEngine;

public class TrussPreset
{
    public byte xPositionCoarse;
    public byte xPositionFine;
    
    public byte yPositionCoarse;
    public byte yPositionFine;
    
    public byte zPositionCoarse;
    public byte zPositionFine;
    
    public byte xRotationCoarse;
    public byte xRotationFine;
    
    public byte yRotationCoarse;
    public byte yRotationFine;
    
    public byte zRotationCoarse;
    public byte zRotationFine;

    public TrussPreset()
    {
        
    }

    public TrussPreset(
        byte xpc, byte xpf,
        byte ypc, byte ypf,
        byte zpc, byte zpf,

        byte xrc, byte xrf,
        byte yrc, byte yrf,
        byte zrc, byte zrf)
    {
        this.xPositionCoarse = xpc;
        this.xPositionFine = xpf;
        this.yPositionCoarse = ypc;
        this.yPositionFine = ypf;
        this.zPositionCoarse = zpc;
        this.zPositionFine = zpf;
        this.xRotationCoarse = xrc;
        this.xRotationFine = xrf;
        this.yRotationCoarse = yrc;
        this.yRotationFine = yrf;
        this.zRotationCoarse = zrc;
        this.zRotationFine = zrf;
    }

    public Vector3 GetPosition()
    {
        return new Vector3(
            Utility.GetValueFromCoarseFine(xPositionCoarse, xPositionFine, -50, 50),
            Utility.GetValueFromCoarseFine(yPositionCoarse, yPositionFine, -50, 50),
            Utility.GetValueFromCoarseFine(zPositionCoarse, zPositionFine, -50, 50));
    }

    public Vector3 GetRotation()
    {
        return new Vector3(
            Utility.GetValueFromCoarseFine(xRotationCoarse, xRotationFine, -270, 270),
            Utility.GetValueFromCoarseFine(yRotationCoarse, yRotationFine, -270, 270),
            Utility.GetValueFromCoarseFine(zRotationCoarse, zRotationFine, -270, 270));
    }
}