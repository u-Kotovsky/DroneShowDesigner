using System;
using Drones;
using Truss;
using UnityEngine;
using Random = UnityEngine.Random;

public class FixtureSpawnManager : MonoBehaviour
{
    public DmxController dmxController;
        
    [Header("Drone Spawn Settings")]
    public GameObject dronePrefab;
    public GameObject[] lightingDronePool;
    public GameObject[] pyroDronePool;

    public int lightingDroneSpawnCount = 1000;
    public int pyroDroneSpawnCount = 16;
    public float droneMargin = 1;
        
    public bool usePyroDrone = false;
    public bool useLightingDrone = false;
    public bool useMobileTruss = true;
    public bool useMobileLight = true;
        
    [Header("Mobile Truss Spawn Settings")]
    public GameObject mobileTrussPrefab;
    public GameObject[] mobileTrussPool;
    public int mobileTrussSpawnCount = 12;
    
    private TrussPreset[][] trussPresets;

    private byte[] globalDmxBuffer;
    
    private void Start()
    {
        globalDmxBuffer = new byte[512 * 40]; // ~9000 channels
            
        if (usePyroDrone)
            SpawnPyroDrones();
        if (useLightingDrone)
            SpawnLightingDrones();
        if (useMobileTruss)
            SpawnMobileTruss();

        Application.targetFrameRate = 60;
        Application.runInBackground = true;
        
        lightingDronePool[0].transform.parent.localPosition = new Vector3(-15, 20, 50);
        
        int counter = 0;
        for (var i = 0; i < 500; i++)
        {
            for (int j = 0; j < 32; j++)
            {
                if (lightingDronePool.Length <= counter) return;
                var drone = lightingDronePool[counter];
                
                drone.transform.localPosition = new Vector3(j, i, 0);
                drone.GetComponent<LightingDrone>().color = Color.white;
                
                counter++;
            }
        }
    }

    private enum TrussState
    {
        None,
        WaitingForTimer,
        Moving
    }

    private TrussState trussState = TrussState.WaitingForTimer;
    private float trussSwapTimer;
    private int currentTrussPreset;
    private int nextTrussPreset = 1;

    private void Update()
    {
        if (usePyroDrone)
        {
            WritePyroDroneDataToGlobalBuffer();
            WritePyroSpecialData(globalDmxBuffer);
        }

        if (useLightingDrone)
        {
            WriteLightingDroneDataToGlobalBuffer();
            WriteLightingSpecialData(globalDmxBuffer);
        }

        if (useMobileTruss)
        {
            WriteMobileTrussDataToGlobalBuffer();
            WriteTrussSpecialData(globalDmxBuffer);
        }

        switch (trussState)
        {
            case TrussState.WaitingForTimer:
                trussSwapTimer += Time.deltaTime;
                if (trussSwapTimer >= 5)
                {
                    trussSwapTimer = 0;
                    trussState = TrussState.Moving;
                }
                break;
            case TrussState.Moving:
                trussSwapTimer += Time.deltaTime;
                
                for (var i = 0; i < mobileTrussPool.Length; i++)
                {
                    mobileTrussPool[i].transform.localPosition = Vector3.Lerp(
                        trussPresets[currentTrussPreset][i].GetPosition(), 
                        trussPresets[nextTrussPreset][i].GetPosition(), 
                        Utility.MapRange(trussSwapTimer, 0, 5, 0, 1));
                    
                    mobileTrussPool[i].transform.localRotation = Quaternion.Slerp(
                        trussPresets[currentTrussPreset][i].GetRotation(),
                        trussPresets[nextTrussPreset][i].GetRotation(), 
                        Utility.MapRange(trussSwapTimer, 0, 5, 0, 1));
                    
                    //mobileTrussPool[i].transform.localRotation = Quaternion.Slerp(
                    //    Quaternion.Euler(trussPresets[currentTrussPreset][i].GetRotation()),
                    //    Quaternion.Euler(trussPresets[nextTrussPreset][i].GetRotation()), 
                    //    Utility.MapRange(trussSwapTimer, 0, 5, 0, 1));
                }
                
                if (trussSwapTimer >= 7.5f)
                {
                    trussSwapTimer = 0;
                    currentTrussPreset = nextTrussPreset;
                    nextTrussPreset++;
                    trussState = TrussState.WaitingForTimer;
                    if (nextTrussPreset >= trussPresets.Length) nextTrussPreset = 0;
                }
                break;
        }
            
        SendGlobalBuffer();
    }

    private void SendGlobalBuffer()
    {
        byte[] dmxBuffer = new byte[512];
        for (var i = 0; i < globalDmxBuffer.Length / 512; i++)
        {
            Buffer.BlockCopy(globalDmxBuffer, i * 512, 
                dmxBuffer, 0, 512);
            dmxController.Send((short)i, dmxBuffer);
        }
    }

    private void GenerateNoise(short universe = 0, byte min = 0, byte max = 255)
    {
        byte[] buffer = new byte[511];
        for (var i = 0; i < buffer.Length; i++)
            buffer[i] = (byte)Random.Range(min, max);
        dmxController.Send(universe, buffer);
    }

    #region Mobile Lights

    private void a()
    {
            
    }

    #endregion

    #region Mobile Truss
    private void WriteTrussSpecialData(byte[] buffer)
    {
        buffer[5] = 255;
    }
        
    public void SpawnMobileTruss()
    {
        GameObject pool = new GameObject("MobileTrussPool");
        mobileTrussPool = new GameObject[mobileTrussSpawnCount];
        MobileTruss truss;
        int offset = 6; // Start for mobile truss.

        for (int i = 0; i < mobileTrussPool.Length; i++)
        {
            mobileTrussPool[i] = Instantiate(mobileTrussPrefab, new Vector3(i * droneMargin * 9, 2, 0), Quaternion.identity);
            mobileTrussPool[i].transform.SetParent(pool.transform);
            truss = mobileTrussPool[i].AddComponent<MobileTruss>();
            truss.spawnManager = this;
            truss.globalChannelStart = offset + (i * truss.GetDmxData().Length);
        }
            
        Debug.Log($"{mobileTrussPool.Length} mobile trusses are instanced");
        
        
        // Default Arches [Set Start[
        TrussPreset[] preset1 = new TrussPreset[12]; // bad

        preset1[0] = new TrussPreset(79, 216, 142, 185, 141, 129, 5, 63, 151, 129, 21, 238);
        preset1[1] = new TrussPreset(94, 190, 157, 68, 152, 222, 1, 251, 159, 141, 13, 213);
        preset1[2] = new TrussPreset(116, 40, 165, 81, 159, 40, 165, 145, 166, 0, 4, 211);
        preset1[3] = new TrussPreset(139, 214, 165, 81, 159, 40, 165, 145, 4, 169, 165, 214);
        preset1[4] = new TrussPreset(161, 64, 157, 69, 152, 222, 1, 251, 11, 28, 156, 212);
        preset1[5] = new TrussPreset(176, 38, 142, 184, 141, 129, 5, 63, 19, 40, 148, 187);
        preset1[6] = new TrussPreset(79, 216, 142, 185, 114, 125, 165, 93, 19, 23, 21, 244);
        preset1[7] = new TrussPreset(94, 190, 157, 68, 103, 32, 168, 192, 11, 38, 13, 212);
        preset1[8] = new TrussPreset(116, 40, 165, 81, 96, 214, 5, 32, 4, 173, 4, 211);
        preset1[9] = new TrussPreset(139, 214, 165, 81, 96, 214, 5, 11, 166, 2, 165, 213);
        preset1[10] = new TrussPreset(161, 64, 157, 69, 103, 32, 168, 174, 159, 142, 156, 211);
        preset1[11] = new TrussPreset(176, 38, 142, 184, 114, 125, 165, 97, 151, 140, 148, 184);
        
        // Circling
        TrussPreset[] preset2 = new TrussPreset[12]; // this works fineish, just truss are vertical aligned in world.
        
        preset2[0] = new TrussPreset(66, 20, 143, 91, 144, 114, 0, 0, 92, 113, 127, 255);
        preset2[1] = new TrussPreset(82, 164, 143, 91, 173, 35, 0, 0, 106, 170, 127, 255);
        preset2[2] = new TrussPreset(111, 85, 143, 91, 189, 179, 0, 0, 120, 226, 127, 255);
        preset2[3] = new TrussPreset(144, 118, 143, 91, 189, 180, 0, 0, 135, 27, 127, 255);
        preset2[4] = new TrussPreset(173, 38, 143, 91, 173, 35, 0, 0, 149, 84, 127, 255);
        preset2[5] = new TrussPreset(189, 183, 143, 91, 144, 115, 0, 0, 163, 141, 127, 255);
        preset2[6] = new TrussPreset(66, 20, 143, 91, 111, 81, 0, 0, 78, 56, 127, 255);
        preset2[7] = new TrussPreset(82, 165, 143, 91, 82, 161, 0, 0, 63, 255, 127, 255);
        preset2[8] = new TrussPreset(111, 85, 143, 91, 66, 16, 0, 0, 49, 198, 127, 255);
        preset2[9] = new TrussPreset(144, 118, 143, 91, 66, 17, 0, 0, 35, 141, 127, 255);
        preset2[10] = new TrussPreset(173, 39, 143, 91, 82, 161, 0, 0, 21, 85, 127, 255);
        preset2[11] = new TrussPreset(189, 183, 143, 91, 111, 82, 0, 0, 7, 28, 127, 255);
        
        // Grid 1
        TrussPreset[] preset3 = new TrussPreset[12]; // position is right, but rotation is fucked
        
        preset3[0] = new TrussPreset(102, 102, 158, 183, 163, 214, 0, 0, 0, 0, 0, 0);
        preset3[1] = new TrussPreset(127, 255, 158, 183, 163, 214, 0, 0, 0, 0, 0, 0);
        preset3[2] = new TrussPreset(153, 153, 158, 183, 163, 214, 0, 0, 0, 0, 0, 0);
        preset3[3] = new TrussPreset(102, 102, 158, 183, 140, 204, 0, 0, 0, 0, 0, 0);
        preset3[4] = new TrussPreset(127, 255, 158, 183, 140, 204, 0, 0, 0, 0, 0, 0);
        preset3[5] = new TrussPreset(153, 153, 158, 183, 140, 204, 0, 0, 0, 0, 0, 0);
        preset3[6] = new TrussPreset(102, 102, 158, 183, 115, 50, 0, 0, 0, 0, 0, 0);
        preset3[7] = new TrussPreset(127, 255, 158, 183, 115, 50, 0, 0, 0, 0, 0, 0);
        preset3[8] = new TrussPreset(153, 153, 158, 183, 115, 50, 0, 0, 0, 0, 0, 0);
        preset3[9] = new TrussPreset(102, 102, 158, 183, 92, 40, 0, 0, 0, 0, 0, 0);
        preset3[10] = new TrussPreset(127, 255, 158, 183, 92, 40, 0, 0, 0, 0, 0, 0);
        preset3[11] = new TrussPreset(153, 153, 158, 183, 92, 40, 0, 0, 0, 0, 0, 0);
        
        // Grid 2
        TrussPreset[] preset4 = new TrussPreset[12]; // position is right, but rotation is fucked x2
        
        preset4[0] = new TrussPreset(102, 102, 158, 183, 163, 214, 0, 0, 42, 170, 0, 0);
        preset4[1] = new TrussPreset(127, 255, 158, 183, 163, 214, 0, 0, 42, 170, 0, 0);
        preset4[2] = new TrussPreset(153, 153, 158, 183, 163, 214, 0, 0, 42, 170, 0, 0);
        preset4[3] = new TrussPreset(102, 102, 158, 183, 139, 230, 0, 0, 42, 170, 0, 0);
        preset4[4] = new TrussPreset(127, 255, 158, 183, 139, 230, 0, 0, 42, 170, 0, 0);
        preset4[5] = new TrussPreset(153, 153, 158, 183, 139, 230, 0, 0, 42, 170, 0, 0);
        preset4[6] = new TrussPreset(102, 102, 158, 183, 116, 24, 0, 0, 42, 170, 0, 0);
        preset4[7] = new TrussPreset(127, 255, 158, 183, 116, 24, 0, 0, 42, 170, 0, 0);
        preset4[8] = new TrussPreset(153, 153, 158, 183, 116, 24, 0, 0, 42, 170, 0, 0);
        preset4[9] = new TrussPreset(102, 102, 158, 183, 92, 40, 0, 0, 42, 170, 0, 0);
        preset4[10] = new TrussPreset(127, 255, 158, 183, 92, 40, 0, 0, 42, 170, 0, 0);
        preset4[11] = new TrussPreset(153, 153, 158, 183, 92, 40, 0, 0, 42, 170, 0, 0);
        
        // Arches Half Mirrored
        TrussPreset[] preset5 = new TrussPreset[12];
        
        preset5[0] = new TrussPreset(79, 216, 142, 185, 141, 129, 5, 63, 151, 129, 21, 238);
        preset5[1] = new TrussPreset(94, 190, 157, 68, 152, 222, 1, 251, 159, 141, 13, 213);
        preset5[2] = new TrussPreset(116, 40, 165, 81, 159, 40, 165, 145, 166, 0, 4, 211);
        preset5[3] = new TrussPreset(139, 214, 165, 81, 159, 40, 5, 24, 89, 254, 4, 211);
        preset5[4] = new TrussPreset(161, 64, 157, 69, 152, 222, 168, 174, 96, 113, 13, 213);
        preset5[5] = new TrussPreset(176, 38, 142, 184, 141, 129, 165, 106, 104, 125, 21, 238);
        preset5[6] = new TrussPreset(79, 216, 142, 132, 114, 62, 164, 211, 19, 67, 21, 106);
        preset5[7] = new TrussPreset(94, 190, 156, 187, 102, 121, 1, 121, 13, 101, 13, 121);
        preset5[8] = new TrussPreset(116, 40, 164, 153, 95, 245, 0, 8, 3, 228, 4, 162);
        preset5[9] = new TrussPreset(139, 214, 164, 153, 95, 245, 170, 161, 81, 112, 4, 162);
        preset5[10] = new TrussPreset(161, 64, 156, 187, 102, 120, 169, 48, 71, 239, 13, 121);
        preset5[11] = new TrussPreset(176, 38, 142, 131, 114, 62, 5, 214, 66, 17, 21, 106);
        
        trussPresets = new TrussPreset[5][];
        trussPresets[0] = preset1;
        trussPresets[1] = preset2;
        trussPresets[2] = preset3;
        trussPresets[3] = preset4;
        trussPresets[4] = preset5;
        
        SetTrussPreset(trussPresets[0]);

        Debug.Log($"Mathf inverse lerp -270 270 70: {Mathf.InverseLerp(-270, 270, 70)}");
        
        // test case:
        // we have 5, 63
        
        
        
        Test1(5, 63);
        Test1(176, 38);
    }

    private void SetTrussPreset(TrussPreset[] preset)
    {
        for (var i = 0; i < preset.Length; i++)
        {
            GameObject truss1 = mobileTrussPool[i];
            truss1.transform.localPosition = preset[i].GetPosition();
            truss1.transform.localRotation = preset[i].GetRotation();
            //truss1.transform.localRotation = Quaternion.Euler(preset[i].GetRotation());
        }
    }

    private void Test1(byte coarse1, byte fine1)
    {
        Debug.Log($"End/Start test");
        Debug.Log($"before reconstruction. coarse: {coarse1}, fine: {fine1}");
        float value1 = Utility.GetValueFromCoarseFine(coarse1, fine1, -50, 50);
        Debug.Log($"value from coarse&fine: {value1}");
        float coarse2 = Utility.GetCoarse(value1, -50, 50);
        float fine2 = Utility.GetFine(value1, -50, 50);
        Debug.Log($"after reconstruction. coarse: {coarse2}, fine: {fine2}");
    }
        
    public void WriteMobileTrussDataToGlobalBuffer()
    {
        foreach (var drone in mobileTrussPool)
        {
            var truss = drone.GetComponent<MobileTruss>();
            byte[] trussData = truss.GetDmxData();
                
            Buffer.BlockCopy(trussData, 0, 
                globalDmxBuffer, truss.globalChannelStart, trussData.Length);
        }
    }
    #endregion
        
    #region Pyro Drones
    private void WritePyroSpecialData(byte[] buffer)
    {
        int offset = 512 * 5 - 1;
        buffer[offset + 39] = 255;
        buffer[offset + 40] = 0;
        buffer[offset + 243] = 255;
        buffer[offset + 244] = 0;
        buffer[offset + 245] = 255;
        buffer[offset + 246] = 255;
    }
        
    public void SpawnPyroDrones()
    {
        GameObject pool = new GameObject("PyroDronePool");
        pyroDronePool = new GameObject[pyroDroneSpawnCount];
        PyroDrone drone;
        int offset = (512 * 5) + 41 - 1; // 2600 is start for pyro drone.

        for (int i = 0; i < pyroDronePool.Length; i++)
        {
            pyroDronePool[i] = Instantiate(dronePrefab, new Vector3(i * droneMargin, 2, 0), Quaternion.identity);
            pyroDronePool[i].transform.SetParent(pool.transform);
            drone = pyroDronePool[i].AddComponent<PyroDrone>();
            drone.spawnManager = this;
            drone.globalChannelStart = offset + (i * drone.GetDmxData().Length);
        }
            
        Debug.Log($"{pyroDronePool.Length} pyro drones are instanced");
    }
        
    public void WritePyroDroneDataToGlobalBuffer()
    {
        foreach (var drone in pyroDronePool)
        {
            var pyroDrone = drone.GetComponent<PyroDrone>();
            byte[] droneData = pyroDrone.GetDmxData();
                
            Buffer.BlockCopy(droneData, 0, 
                globalDmxBuffer, pyroDrone.globalChannelStart, droneData.Length);
        }
    }
    #endregion

    #region Lighting Drones
    private void WriteLightingSpecialData(byte[] buffer)
    {
        int offset = 512 * 5 - 1;
        buffer[offset + 39] = 255;
        buffer[offset + 40] = 0;
        buffer[offset + 243] = 255;
        buffer[offset + 244] = 0;
        buffer[offset + 245] = 255;
        buffer[offset + 246] = 255;
    }
        
    public void SpawnLightingDrones()
    {
        GameObject pool = new GameObject("LightingDronePool");
        lightingDronePool = new GameObject[lightingDroneSpawnCount];
        LightingDrone lightingDrone;
        int offset = (512 * 5) + 321 - 1; // 2880 is start for FX drone // Offset is probably correct (maybe?)

        for (int i = 0; i < lightingDronePool.Length; i++)
        {
            lightingDronePool[i] = Instantiate(dronePrefab, new Vector3(i * droneMargin, 1, 0), Quaternion.identity);
            lightingDronePool[i].transform.SetParent(pool.transform);
            lightingDrone = lightingDronePool[i].AddComponent<LightingDrone>();
            lightingDrone.spawnManager = this;
            lightingDrone.globalChannelStart = offset + (i * lightingDrone.GetDmxData().Length);
            lightingDrone.gameObject.AddComponent<DroneNavigation>();
        }
            
        Debug.Log($"{lightingDronePool.Length} lighting drones are instanced");
    }
        
    public void WriteLightingDroneDataToGlobalBuffer()
    {
        foreach (var drone in lightingDronePool)
        {
            var lightingDrone = drone.GetComponent<LightingDrone>();
            byte[] droneData = lightingDrone.GetDmxData();

            Buffer.BlockCopy(droneData, 0, 
                globalDmxBuffer, lightingDrone.globalChannelStart, droneData.Length);
        }
    }
    #endregion
}