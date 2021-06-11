using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine.SceneManagement;

namespace UtilityTools
{
    /// <summary>
    /// Editor window class to alter project settings to maximally optimise project for occulus quest
    /// </summary>
    public class ProjectSettingsOptimiser : EditorWindow
    {
        // on advice from https://developer.oculus.com/documentation/unity/unity-conf-settings/

        public static void Init()
        {
            ProjectSettingsOptimiser window = (ProjectSettingsOptimiser)EditorWindow.GetWindow(typeof(ProjectSettingsOptimiser));
            window.Show();
        }

        private void OnGUI()
        {

            GUILayout.Label("Optimise project settings for use in quest:", EditorStyles.boldLabel);

            GUILayout.Label("Build Settings", EditorStyles.miniBoldLabel);

            // linear colorspace looks best with these light settings
            if (GUILayout.Button("Set colour space to linear"))
            {
                PlayerSettings.colorSpace = ColorSpace.Linear;
                EditorUtility.DisplayDialog("Success", "Colour space updated", "Ok");
            }

            // only openGLE 3 is supported by Oculus Quest
            if (GUILayout.Button("Set graphics api to openGLE 3 only"))
            {
                PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
                PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3 });
                EditorUtility.DisplayDialog("Success", "Graphics api selected", "Ok");
            }

            // this is a no-brainer
            if (GUILayout.Button("Enable multithreaded rendering"))
            {
                PlayerSettings.SetMobileMTRendering(BuildTargetGroup.Android, true);
                EditorUtility.DisplayDialog("Success", "Multithreaded rendering enabled", "Ok");
            }

            GUILayout.Label("Quality Settings", EditorStyles.miniBoldLabel);

            // this is probably the most important, each pixel will only calculate lighting from one light
            if (GUILayout.Button("Reduce pixel light count"))
            {
                QualitySettings.pixelLightCount = 1;
                EditorUtility.DisplayDialog("Success", "Pixel light count set to 1", "Ok");
            }

            // this is setting anisotropic filtering to per-texture
            if (GUILayout.Button("Set anisotropic filtering level"))
            {
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
                EditorUtility.DisplayDialog("Success", "anisotropic filtering level set to per texxture", "Ok");
            }

            // 4 is reasonable
            if (GUILayout.Button("Set anti ailising level"))
            {
                QualitySettings.antiAliasing = 4;
                EditorUtility.DisplayDialog("Success", "anti ailising level set to 4x", "Ok");
            }

            // soft particles are not possible with forward rendering anyway but this should be enforced anyway
            if (GUILayout.Button("Disable soft particles"))
            {
                QualitySettings.softParticles = false;
                EditorUtility.DisplayDialog("Success", "soft particles disabled", "Ok");
            }

            // not tchnically optimisation but this is important to make billboards look correct in vr apparently
            if (GUILayout.Button("Make billboards face camera"))
            {
                QualitySettings.billboardsFaceCameraPosition = true;
                EditorUtility.DisplayDialog("Success", "billboards face camera", "Ok");
            }

            GUILayout.Space(15);

            // if yer mad and want to just optimise everything at once
            if (GUILayout.Button("All"))
            {
                if (EditorUtility.DisplayDialog("Are you sure?", "Apply all settings?", "yes", "Cancel"))
                {
                    PlayerSettings.colorSpace = ColorSpace.Linear;
                    PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
                    PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3 });
                    PlayerSettings.SetMobileMTRendering(BuildTargetGroup.Android, true);
                    QualitySettings.pixelLightCount = 1;
                    QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
                    QualitySettings.antiAliasing = 4;
                    QualitySettings.softParticles = false;
                    QualitySettings.billboardsFaceCameraPosition = true;
                    EditorUtility.DisplayDialog("Success", "All settings applied", "Ok");
                }

            }

            GUILayout.Space(25);


            // close window
            if (GUILayout.Button("Close"))
                Close();

            GUILayout.Space(25);







        }

    }
}