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
    }

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
        buffer[6] = 255;
    }
        
    public void SpawnMobileTruss()
    {
        GameObject pool = new GameObject("MobileTrussPool");
        mobileTrussPool = new GameObject[mobileTrussSpawnCount];
        MobileTruss truss;
        int offset = 7 - 1; // 6 is start for mobile truss.

        for (int i = 0; i < mobileTrussPool.Length; i++)
        {
            mobileTrussPool[i] = Instantiate(mobileTrussPrefab, new Vector3(i * droneMargin * 9, 2, 0), Quaternion.identity);
            mobileTrussPool[i].transform.SetParent(pool.transform);
            truss = mobileTrussPool[i].AddComponent<MobileTruss>();
            truss.spawnManager = this;
            truss.globalChannelStart = offset + (i * truss.GetDmxData().Length);
        }
            
        Debug.Log($"{mobileTrussPool.Length} mobile trusses are instanced");
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