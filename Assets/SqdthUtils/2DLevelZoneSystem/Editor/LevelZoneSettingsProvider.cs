using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using Resources = SqdthUtils._2DLevelZoneSystem.Resources;

#if UNITY_EDITOR

namespace SqdthUtils
{
    public class LevelZoneSettingsProvider : SettingsProvider
    {
        private SerializedObject _mLevelZoneSettings;

        private LevelZoneSettingsProvider(
            string path, SettingsScope scopes,
            IEnumerable<string> keywords = null
        ) : base(path, scopes, keywords) { }

        private static bool IsSettingsAvailable()
        {
            return File.Exists(Resources.KLevelZoneSettingsAssetPath);
        }

        /// <summary>
        /// This function is called when the user clicks on the target element in the Settings window.
        /// </summary>
        /// <param name="searchContext"></param>
        /// <param name="rootElement"></param>
        public override void OnActivate(string searchContext,
            VisualElement rootElement)
        {
            _mLevelZoneSettings = LevelZoneSettings.GetSerializedSettings();
        }

        public override void OnGUI(string searchContext)
        {
            EditorGUILayout.PropertyField(
                _mLevelZoneSettings.FindProperty("targetCameraAspectRatio"),
                new GUIContent("Target Camera Aspect Ratio"));
            EditorGUILayout.PropertyField(
                _mLevelZoneSettings.FindProperty("targetOrthographicCameraSize"),
                new GUIContent("Target Orthographic Camera Size"));
            EditorGUILayout.PropertyField(
                _mLevelZoneSettings.FindProperty("debugLineWidth"),
                new GUIContent("Debug Line Width"));

            _mLevelZoneSettings.ApplyModifiedPropertiesWithoutUndo();
        }

        // Register the SettingsProvider
        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            if (IsSettingsAvailable())
            {
                var provider = new LevelZoneSettingsProvider(
                    "Project/2DLevelZoneSettings", SettingsScope.Project)
                    {
                        label = "2D Level Zone System Settings",
                        keywords = new[]
                        {
                            "2D", "Levels", "Zones", "System", 
                            "Target Aspect Ratio", 
                            "Target Orthographic Camera Size",
                            "Debug Line Width", "Draw Line Width" 
                        }
                    };

                return provider;
            }
            
            // Settings Asset doesn't exist yet; no need to display anything in the Settings window.
            return null;
        }
    }
}

#endif
