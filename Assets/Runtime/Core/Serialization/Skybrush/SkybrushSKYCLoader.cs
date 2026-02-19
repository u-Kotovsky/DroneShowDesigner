using System.IO;
using Unity.SharpZipLib.Zip;
using UnityEngine;

namespace Runtime.Core.Serialization.Skybrush
{
    public abstract class SkybrushSKYCLoader
    {
        /// <summary>
        /// Load skybrush.skyc data
        /// </summary>
        /// <param name="pathToFile">*.skyc</param>
        /// <exception cref="FileNotFoundException"></exception>
        public static void Load(string pathToFile)
        {
            // it is stored in .skyc but it's essentially .zip
            // When we extract all data from .zip
            // structure is:
            // show.skyc
            //  /drones
            //  .. /Drone 65
            //   .. /lights.json
            //   .. /trajectory.json
            //  /cues.json
            //  /show.json
            
            // trajectory.json:
            // { version: 1, points: [ data ], takeoffTime: 0.0, landingTime: 10.0 }
            // data: [ 0.0, [ 0.0, 0.0, 0.0 ] ]
            
            // lights.json:
            // { data: base64 string, version: 1 } // not sure what data contains exactly, maybe bytes?
            
            // cues.json:
            // { version: 1, items: [ ... ] }
            
            // show.json:
            // { version: 1, settings: { .. }, swarm: { drones: { .. } }, environment: { type: outdoor }, meta: { title, segments: { takeoff: [ 0.0, 0.0 ] } }, media: { ... } }

            Debug.Log($"(SkybrushSKYCLoader) Loading {pathToFile}");
            
            if (!File.Exists(pathToFile))
            {
                throw new FileNotFoundException("File not found: " + pathToFile);
            }

            var tempPath = Path.Join(Application.dataPath, "SkybrushCache", "SKYCCache");
            if (!Directory.Exists(tempPath)) Directory.CreateDirectory(tempPath);

            var part = pathToFile.Replace("//", "/").Replace("/", @"\").Split(@"\")[^1];
            var title = part;
            var pathToTemp = Path.Join(tempPath, title);
            Debug.Log($"(SkybrushSKYCLoader) Extracting data to {pathToTemp} {title} {part}");
            if (!Directory.Exists(pathToTemp)) Directory.CreateDirectory(pathToTemp);

            FastZip fastZip = new FastZip();
            fastZip.ExtractZip(pathToFile, pathToTemp, null);
            
            // TODO: load each skybrush data file in memory
        }
    }
}