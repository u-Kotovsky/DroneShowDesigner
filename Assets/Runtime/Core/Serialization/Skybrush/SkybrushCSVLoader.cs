using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.SharpZipLib.Zip;
using UnityEngine;

namespace Runtime.Core.Serialization.Skybrush
{
    public abstract class SkybrushCSVLoader
    {
        /// <summary>
        /// Loading skybrush data from csv file.
        /// </summary>
        /// <param name="pathToFile">*.csv</param>
        /// <exception cref="FileNotFoundException"></exception>
        public static List<List<LightingDroneKeyframe>> Load(string pathToFile)
        {
            // it is stored in .zip
            // When we extract all data from .zip
            // show.zip
            //  /Drone 1.csv
            //  ...
            // for default drone values:
            // int, float, float, float, byte, byte, byte
            // Time [msec],x [m],y [m],z [m],Red,Green,Blue
            // ex.:
            // 250,-10.5,-10.5,0.2491,255,255,255
            
            Debug.Log($"(SkybrushCSVLoader) Loading {pathToFile}");
            
            if (!File.Exists(pathToFile))
            {
                throw new FileNotFoundException("File not found: " + pathToFile);
            }

            var tempPath = Path.Join(Application.dataPath, "SkybrushCache", "CSVCache");
            if (!Directory.Exists(tempPath)) Directory.CreateDirectory(tempPath);

            var part = pathToFile.Replace("//", "/").Replace("/", @"\").Split(@"\")[^1];
            var title = part;
            var pathToTemp = Path.Join(tempPath, title);
            Debug.Log($"(SkybrushCSVLoader) Extracting data to {pathToTemp} {title} {part}");
            if (!Directory.Exists(pathToTemp)) Directory.CreateDirectory(pathToTemp);

            var fastZip = new FastZip();
            fastZip.ExtractZip(pathToFile, pathToTemp, null);
            
            // TODO: load each skybrush data file in memory
            
            var dirs = Directory.GetFiles(pathToTemp); // each drone file

            var drones = new List<List<LightingDroneKeyframe>>();

            int actualIndex = 0;
            for (var i = 0; i < dirs.Length; i++)
            {
                if (!dirs[i].EndsWith(".csv")) continue;
                var path = dirs[i];
                // TODO: redo this to not break when we have Drone somewhere in path
                var droneNum = int.Parse(path.Split("Drone ")[1].Split(".csv")[0]);

                var rawData = File.ReadAllLines(path).Skip(1).ToArray(); // We skip header and go right to the data
                var items = rawData.Select(LightingDroneKeyframe.Parse).ToList();
                
                drones.Add(items);

                actualIndex++;
            }

            return drones;
        }
    }
}