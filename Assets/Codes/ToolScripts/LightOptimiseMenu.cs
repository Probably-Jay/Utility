using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using UnityEngine.SceneManagement;

namespace UtilityTools
{
    public class LightOptimiseMenu : EditorWindow
    {

        private enum SelectionMode
        {
            SelectionOnly,
            EntireScene
        }

        SelectionMode selectionMode = SelectionMode.EntireScene;
        LightShadows shadowMode = LightShadows.None;
        LightmapBakeType lightmapBakeMode = LightmapBakeType.Baked;

        //[MenuItem("Tools/Utility/Light Optimisation Settings")]
        static void Init()
        {
            LightOptimiseMenu window = (LightOptimiseMenu)EditorWindow.GetWindow(typeof(LightOptimiseMenu));

            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Alter light components:", EditorStyles.boldLabel);

            GUILayout.Space(10);

            selectionMode = (SelectionMode)EditorGUILayout.EnumPopup("Edit lights in: ", selectionMode);

            GUILayout.Space(10);

            shadowMode = (LightShadows)EditorGUILayout.EnumPopup("Shadow mode: ", shadowMode);

            lightmapBakeMode = (LightmapBakeType)EditorGUILayout.EnumPopup("Light map bake mode: ", lightmapBakeMode);

            GUILayout.Space(15);

            // display warning if settings are not optimal
            if (shadowMode != LightShadows.None && lightmapBakeMode != LightmapBakeType.Baked)
            {
                EditorGUILayout.HelpBox("The current lighting settings are quite demanding " +
                "and may result in performance issues.", MessageType.Warning);
            }

            GUILayout.Space(15);

            // button to display current lights and their settings 
            bool displayActiveLights = GUILayout.Button("List Lights in Current Scene");
            if (displayActiveLights)
            {
                DisplayActiveLights();
            }


            GUILayout.Space(20);

            // button to apply current settings
            string selectionText = selectionMode == SelectionMode.EntireScene ? "Current Scene" : "Selection";
            bool applySettingsToLights = GUILayout.Button("Apply Settings To Lights in " + selectionText);
            if (applySettingsToLights)
            {
                // returns true if lights altered
                bool sucess = ApplySettingsToLights();
                if (sucess)
                    Close();
            }



        }

        // applies the set settings to the lights, either all lights in scene or only those selected
        private bool ApplySettingsToLights()
        {

            EditorUtility.DisplayProgressBar("Applying Settings To Lights", "Applying...", 0.0f);

            // either all the lights in the scene or only the ones selected
            List<Light> lights = selectionMode == SelectionMode.EntireScene ? GetLightsInScene() : GetLightsSelection();

            EditorUtility.ClearProgressBar();

            // if there are no lights to alter, display message and close
            if (lights.Count == 0)
            {
                EditorUtility.DisplayDialog("No lights to alter", "There are no lights to apply these settings to.", "Ok");
                return false;
            }

            // in case of error pressing button, user is asked if they are sure
            if (!EditorUtility.DisplayDialog("Apply Settings", "Applying settings will alter " + lights.Count + " objects. \nDo you wish to continue?", "Yes", "Cancel"))
                return false;

            EditorUtility.DisplayProgressBar("Applying Settings To Lights", "Applying...", 0);

            // loop for each light
            int progressCount = 0; // for the progress bar
            foreach (var light in lights)
            {
                progressCount++;
                if (progressCount % 5 == 0) // updating progress bar for each object would be more expensive than it's worth
                    EditorUtility.DisplayProgressBar("Applying Settings To Lights", "Applying...", (float)progressCount / lights.Count);

                // apply changes
                light.shadows = shadowMode;
                light.lightmapBakeType = lightmapBakeMode;

            }
            // close the progress bar
            EditorUtility.ClearProgressBar();

            // message showing sucess, return true as objects sucessfully altered
            EditorUtility.DisplayDialog("Sucess", "Changes successful.", "Ok");
            return true;

        }


        // lists the lights in the scene
        private void DisplayActiveLights()
        {
            EditorUtility.DisplayProgressBar("Finding All Lights", "Scanning...", 0.0f);

            Debug.ClearDeveloperConsole();

            Debug.Log("Listed all lights in the curret scene. (" + SceneManager.GetActiveScene().name + ")");

            List<Light> lights = GetLightsInScene();

            int progressCount = 0; // for the progress bar
            foreach (var light in lights)
            {
                progressCount++;
                if (progressCount % 50 == 0) // updating progress bar for each object would be more expensive than it's worth
                    EditorUtility.DisplayProgressBar("Finding All Lights", "Scanning...", (float)progressCount / lights.Count);

                string message = light.gameObject.name + "->" + light.name + ": " + light.type + " light, shadow type: " + light.shadows
                    + ", light map baking type: " + light.lightmapBakeType;

                // *** Clicking once on the debug message will highlight the light in the hierarchy ***
                Debug.Log(message, light);

            }
            // close the progress bar
            EditorUtility.ClearProgressBar();
        }


        private static List<Light> GetLightsInScene()
        {
            // get the lights
            return new List<Light>(GameObject.FindObjectsOfType(typeof(Light)) as Light[]);
        }

        private List<Light> GetLightsSelection()
        {
            List<Transform> selected = new List<Transform>(Selection.GetTransforms(UnityEditor.SelectionMode.Editable));
            List<Light> lights = new List<Light>();
            foreach (var transform in selected)
            {
                Light light = transform.gameObject.GetComponent<Light>();
                if (light == null)
                    continue;
                lights.Add(light);

            }
            return lights;
        }
    }
}


