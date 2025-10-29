using System;
using UnityEditor;
using UnityEngine;

namespace Fixtures.Drones
{
    // TODO: Boundary box visuals/limit for editor usage
    public class PyroDrone : BaseDrone
    {
        protected byte pitch;
        protected byte yaw;
        protected byte roll;
        
        protected byte index;
        
        private void Awake()
        {
            Buffer = new byte[10]; // (0 -> 6) Position Coarse + Fine, (7 -> 9) Pitch + Yaw + Roll, (10) Index
            MinAngle = -180;
            MaxAngle = 180;
            MinPosition = new Vector3(-800, -800, -800);
            MaxPosition = new Vector3(800, 800, 800);
        }

        public void WriteDmxRotation(Vector3 eulerAngles)
        {
            pitch = (byte)Utility.MapRange(eulerAngles.x, MinAngle, MaxAngle, 0, 255);
            yaw = (byte)Utility.MapRange(eulerAngles.y, MinAngle, MaxAngle, 0, 255);
            roll = (byte)Utility.MapRange(eulerAngles.z, MinAngle, MaxAngle, 0, 255);
            
            Buffer[6] = pitch;
            Buffer[7] = yaw;
            Buffer[8] = roll;
        }

        public void WriteDmxIndex(byte value)
        {
            Buffer[9] = value;
        }

        public new byte[] GetDmxData()
        {
            WriteDmxPosition(0, transform.position, true);
            WriteDmxRotation(transform.rotation.eulerAngles);
            WriteDmxIndex(index);
            
            return Buffer;
        }

        [CanEditMultipleObjects]
        [CustomEditor(typeof(PyroDrone))]
        public class PyroDrone_Editor : Editor
        {
            private bool updateOnValidate = true;
            private int pitch = 0;
            private int yaw = 0;
            private int roll = 0;
            private int pyroIndex = 0;

            private PyroDrone fixture;

            private static string[] _elements = null;

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                if (fixture == null) fixture = (PyroDrone)target;
                if (_elements == null) _elements = Enum.GetNames(typeof(PyroFXType));

                GUILayout.Space(10);
                GUILayout.Label("Editor-only preview features");

                updateOnValidate = EditorGUILayout.Toggle("Update on Validate", updateOnValidate);

                pyroIndex = EditorGUILayout.Popup(pyroIndex, _elements);
                pitch = EditorGUILayout.IntField("Pitch", pitch);
                yaw = EditorGUILayout.IntField("Yaw", yaw);
                roll = EditorGUILayout.IntField("Roll", roll);

                if (updateOnValidate)
                    WriteDataToDrone();

                if (GUILayout.Button("SetPyroEffect"))
                {
                    WriteDataToDrone();
                }
                
                GUILayout.Space(10);
                GUILayout.Label("Editor-only preview features");
                
                #region Collect data in format [VALUE, ...]
                EditorGUILayout.LabelField($"Copy DMX data in format [VALUE, ...]");
                EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button("Copy All"))
                    Utility.CopyDmxValuesAsArray(fixture.GetDmxData());
                
                if (GUILayout.Button("Copy Position"))
                    Utility.CopyDmxValuesAsArray(fixture.GetDmxData(), 0, 6);
                
                if (GUILayout.Button("Copy Pitch Yaw Roll"))
                    Utility.CopyDmxValuesAsArray(fixture.GetDmxData(), 6, 3);
                
                EditorGUILayout.EndHorizontal();
                
                if (GUILayout.Button("Copy Index"))
                    Utility.CopyDmxValuesAsArray(fixture.GetDmxData(), 9, 1);
                #endregion
                
                GUILayout.Space(10);
                
                #region Collect data as UNIVERSE.CHANNEL_VALUE array
                EditorGUILayout.LabelField($"Copy DMX data in format UNIVERSE.CHANNEL_VALUE");
                EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button("Copy All"))
                    Utility.CopyAllDmxValuesAsMa3Representation(fixture.GetDmxData(), fixture.globalChannelStart);
                
                if (GUILayout.Button("Copy Position"))
                    Utility.CopyDmxValuesWithOffsetAsMa3Representation(fixture.GetDmxData(), fixture.globalChannelStart, 0, 6);
                
                if (GUILayout.Button("Copy Pitch Yaw Roll"))
                    Utility.CopyDmxValuesWithOffsetAsMa3Representation(fixture.GetDmxData(), fixture.globalChannelStart, 6, 3);
                
                EditorGUILayout.EndHorizontal();
                
                if (GUILayout.Button("Copy Index"))
                    Utility.CopyDmxValuesWithOffsetAsMa3Representation(fixture.GetDmxData(), fixture.globalChannelStart, 9, 1);
                #endregion
            }

            private void WriteDataToDrone()
            {
                fixture.index = (byte)pyroIndex;
                fixture.pitch = (byte)pitch;
                fixture.yaw = (byte)yaw;
                fixture.roll = (byte)roll;
            }
        }

        public enum PyroFXType : byte
        {
            None,
            Gold_Willow,
            Silver_Willow,
            Giant_Gold_Willow_to_Multi,
            Blue_Crossette,
            Gold_Crossette,
            Green_Crossette,
            Purple_Crossette,
            Red_Crossette,
            White_Crossette,
            Blue_w_Silver,
            Gold_w_Silver,
            Green_w_Silver,
            Orange_w_Silver,
            Purple_w_Silver,
            Red_w_Silver,
            Rainbow_w_Silver,
            Blue_Peony,
            Green_Peony,
            Gold_Peony,
            Purple_Peony,
            Red_Peony,
            White_Peony,
            Gold_Giant_Peony,
            Gold_w_Red_Giant_Peony,
            White_Giant_Peony,
            White_w_Blue_Giant_Peony,
            Blue_Ringed_Peony,
            Green_Ringed_Peony,
            Red_Ringed_Peony,
            Rainbow_3_Stage,
            Rainbow_Ring,
            Gold_Horsetail,
            Gold_Comet,
            Large_Gold_Comet,
            Large_Silver_Comet,
            Blue_Meteor_Glitter,
            Gold_Meteor_Glitter,
            Green_Meteor_Glitter,
            Orange_Meteor_Glitter,
            Purple_Meteor_Glitter,
            White_Meteor_Glitter,
            Blue_Go_Getter,
            Gold_Go_Getter,
            Green_Go_Getter,
            Orange_Go_Getter,
            Purple_Go_Getter,
            Red_Go_Getter,
            White_Go_Getter,
            Blue_Stars,
            Gold_Stars,
            Green_Stars,
            Orange_Stars,
            Purple_Stars,
            Red_Stars,
            Rainbow_Stars,
            Blue_Stars_w_Glitter,
            Gold_Stars_w_Glitter,
            Green_Stars_w_Glitter,
            Orange_Stars_w_Glitter,
            Purple_Stars_w_Glitter,
            Red_Stars_w_Glitter,
            White_Stars_w_Glitter,
            Dragon_Eggs,
            Silver_Swarm,
            Silver_Tourbillions,
            None1,
            None2,
            Gold_Comet_Slice,
            Rainbow_Meteor_Slice,
            White_Meteor_Slice,
            Rainbow_Stars_Slice_LtoR,
            Rainbow_Stars_Slice_RtoL,
            Rainbow_Stars_Slice_Center,
            Fountain_Jet,
            Sparks_Aqua,
            Sparks_Blue,
            Sparks_Blurple,
            Sparks_Cool_White,
            Sparks_Gold,
            Sparks_Green,
            Sparks_Magenta,
            Sparks_Mint,
            Sparks_Orange,
            Sparks_Purple,
            Sparks_Red,
            Sparks_Uranium,
            Sparks_Warm_White,
            Flame_Jet_Emmie,
            Flame_Jet_Matthew,
            Flame_Jet_Rowan,
            Flame_Jet_Windyote,
            Flame_Jet_Red,
            Lightning_Blue_SOURCE_NO_SOUND,
            Lightning_Red_SOURCE_NO_SOUND,
            Lightning_Purple_SOURCE_NO_SOUND,
            Lightning_Green_SOURCE_NO_SOUND,
            Lightning_COLOR_SOURCE_NO_SOUND,
            Lightning_COLOR_SOURCE_NO_SOUND1,
            Lightning_Blue_DESTINATION_NO_SOUND,
            Lightning_Red_DESTINATION_NO_SOUND,
            Lightning_Purple_DESTINATION_NO_SOUND,
            Lightning_Green_DESTINATION_NO_SOUND,
            Lightning_COLOR_DESTINATION_NO_SOUND,
            Lightning_COLOR_DESTINATION_NO_SOUND1,
            Lightning_COLOR_SOURCE_SOUND,
            Lightning_COLOR_SOURCE_SOUND1,
            Lightning_COLOR_SOURCE_SOUND2,
            Lightning_COLOR_SOURCE_SOUND3,
            Lightning_COLOR_SOURCE_SOUND4,
            Lightning_COLOR_SOURCE_SOUND5,
            Lightning_COLOR_DESTINATION_SOUND,
            Lightning_COLOR_DESTINATION_SOUND1,
            Lightning_COLOR_DESTINATION_SOUND2,
            Lightning_COLOR_DESTINATION_SOUND3,
            Lightning_COLOR_DESTINATION_SOUND4,
            Lightning_COLOR_DESTINATION_SOUND5,
            Mini_Crackle_Blue,
            Mini_Crackle_Red,
            Mini_Crackle_Purple,
            Mini_Crackle_Green,
            Mini_Crackle_COLOR,
            Mini_Crackle_COLOR1,
            Straight_Jacobs_Ladder_Blue,
            Jacobs_Ladder_Blue,
            Rain_Red_1,
            Rain_Red_2,
            Rain_Red_3,
            Rain_Red_4,
            Rain_COLOR,
            Rain_COLOR1,
            Small_Crackle_Blue,
            Small_Crackle_Red,
            Small_Crackle_Purple,
            Small_Crackle_Green,
            Small_Crackle_COLOR,
            Small_Crackle_COLOR1,
            Large_Crackle_Blue,
            Large_Crackle_Red,
            Large_Crackle_Purple,
            Large_Crackle_Green,
            Large_Crackle_COLOR,
            Large_Crackle_COLOR1,
            Large_Flame_Jet,
            Dark_Flames,
            Wave_Pulse,
            Quantum_Burst,
            Dragon_Eggs_Rainbow,
            Dragon_Eggs_COLOR,
            Dragon_Eggs_COLOR1,
            Dragon_Eggs_COLOR2,
            Silver_Swarm_Rainbow,
            Silver_Swarm_COLOR,
            Silver_Swarm_COLOR1,
            Silver_Swarm_COLOR2,
            Rainbow_Tourbillions,
            COLOR_Tourbillions,
            COLOR_Tourbillions1,
            COLOR_Tourbillions2,
            Ember_Rain
        }
    }
}
