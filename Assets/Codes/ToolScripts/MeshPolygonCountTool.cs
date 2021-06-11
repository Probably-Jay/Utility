using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace UtilityTools
{

    /// <summary>
    /// Editor utility function tool to find all meshes in the currently loaded scene and list them by their number of verticess
    /// </summary>
    public class MeshPolygonCountTool : EditorUtility
    {


        [MenuItem("Tools/Utility/List Meshes by Weight")]
        static public void FindMeshesAndListThierWeight()
        {

            // progress bar as this may take some time
            DisplayProgressBar("Finding All Meshes", "Scanning...", 0f);

            Debug.ClearDeveloperConsole();

            Debug.Log("Listed all meshes in the curret scene. (" + SceneManager.GetActiveScene().name + ")");


            // get the meshes
            List<MeshFilter> meshes = new List<MeshFilter>(GameObject.FindObjectsOfType(typeof(MeshFilter)) as MeshFilter[]);


            DisplayProgressBar("Finding All Meshes", "Scanning...", 0.5f);

            // sort them big to small by their number of vertices, no mesh is put to front
            meshes.Sort((x, y) =>
            {
                int xVerts = x.sharedMesh != null ? x.sharedMesh.vertexCount : int.MaxValue;
                int yVerts = y.sharedMesh != null ? y.sharedMesh.vertexCount : int.MaxValue;
                return -xVerts.CompareTo(yVerts);
            });

            DisplayProgressBar("Finding All Meshes", "Scanning...", 1f);

            int progressCount = 0; // for the progress bar
            foreach (var meshFilter in meshes)
            {
                progressCount++;
                if (progressCount % 50 == 0) // updating progress bar for each object would be more expensive than it's worth
                    DisplayProgressBar("Finding All Meshes", "Scanning...", (float)progressCount / meshes.Count);


                if (meshFilter.sharedMesh != null) // mesh filter has mesh assigned
                {
                    string message = meshFilter.gameObject.name + "->" + meshFilter.sharedMesh.name + ": " + meshFilter.sharedMesh.vertexCount + " vertices";

                    // *** Clicking once on the debug message will highlight the game object in the hierarchy ***
                    Debug.Log(message, meshFilter);
                }
                else // no mesh
                {
                    // *** Clicking once on the debug message will highlight the game object in the hierarchy ***
                    Debug.LogWarning(meshFilter.gameObject.name +
                        " has a mesh filter component but no mesh attached. Consider removing the mesh filter component, or removing this object from the scene.", meshFilter);
                }

            }
            // close the progress bar
            ClearProgressBar();

        }


    }
}
