using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
public class MeshColliderValidator : EditorUtility
{
    /// <summary>
    /// Editor utility function to find all mesh colliders and list them by their vertex weight
    /// </summary>

    [MenuItem("Tools/Utility/List Mesh Colliders")]
    static private void FindMeshCollidersAndDisplayThem()
    {
        // progress bar as this may take some time
        DisplayProgressBar("Finding All Mesh Colliders","Scanning...",0f);

        Debug.Log("Listed all mesh colliders in the curret scene. (" + SceneManager.GetActiveScene().name+") \n" +
                    "Note: The vertex count is only an indication of the size of the mesh " +
                    "and does not necessarily representive of the actual optimised collider mesh.");

        Debug.ClearDeveloperConsole();
       

        // get the meshe colliders
        List<MeshCollider> meshColliders = new List<MeshCollider>(GameObject.FindObjectsOfType(typeof(MeshCollider)) as MeshCollider[]);

        
        DisplayProgressBar("Finding All Mesh Colliders", "Scanning...", 0.5f);

        // sort them big to small by their number of vertices, no mesh is put to front
        meshColliders.Sort((x, y) => {
            int xVerts = x.sharedMesh != null ? x.sharedMesh.vertexCount : int.MaxValue;
            int yVerts = y.sharedMesh != null ? y.sharedMesh.vertexCount : int.MaxValue;
            return -xVerts.CompareTo(yVerts);
        });

        DisplayProgressBar("Finding All Mesh Colliders", "Scanning...", 1f);

        int progressCount = 0; // for the progress bar
        foreach (var meshCollider in meshColliders)
        {
            progressCount++;
            if(progressCount%50 == 0) // updating progress bar for each object would be more expensive than it's worth
                DisplayProgressBar("Finding All Mesh Colliders", "Scanning...", (float)progressCount/meshColliders.Count );


            if (meshCollider.sharedMesh != null) // collider has mesh assigned
            {
                string message = meshCollider.gameObject.name + "->" + meshCollider.sharedMesh.name + ": " + meshCollider.sharedMesh.vertexCount + " vertices";

                message += meshCollider.convex ? " – convex" : " – * concave *" ;

                // *** Clicking once on the debug message will highlight the mesh collider in the hierarchy ***
                Debug.Log(message,meshCollider);
            }
            else // no mesh
            {
                // *** Clicking once on the debug message will highlight the mesh collider in the hierarchy ***
                Debug.LogWarning(meshCollider.gameObject.name +
                    " has a mesh collider component but no mesh attached. Consider assigning one, or removing this object's collider.", meshCollider);
            }

        }
        // close the progress bar
        ClearProgressBar();
    
    }


}
