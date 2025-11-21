using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Runtime.Dmx.Fixtures
{
    public class FixtureManager
    {
        public static List<FixtureType> FixtureTypes { get; set; }

        public static void Add(FixtureType fixtureType)
        {
            FixtureTypes.Add(fixtureType);
        }

        public static void Delete(FixtureType fixtureType)
        {
            int index = FixtureTypes.IndexOf(fixtureType);
            Debug.Log($"Deleting fixture type {fixtureType}");
            FixtureTypes.RemoveAt(index);
        }

        public static FixtureType Get(string fullName)
        {
            return FixtureTypes.FirstOrDefault(fixture => fixture.Info.FullName == fullName);
        }
    }
}