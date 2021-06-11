using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using UnityEngine.SceneManagement;
using System.Linq;


namespace UtilityTools
{

    /// <summary>
    /// Editor window class to alter project objects, components and assets so as to maximally optimise project for occulus quest
    /// </summary>
    public class OculusQuestOptimizer : EditorWindow
    {

        // containers used by optimisation functions 
        Dictionary<GameObject, int> parentDepth;
        Dictionary<GameObject, int> childCount;


        delegate void FunctionToApply<T>(T obj); // allows passing of functions

        private enum SelectionMode // what will be effected by the tool
        {
            SelectedInHirarchyOnly,
            EverythingInOpenScene,
            EverythingInAssetsFolder,
        }
        SelectionMode selectionMode;

        [SerializeField]
        Material newSkybox; // drag and drop new skybox
        SerializedProperty skyboxPorperty;

        Shader mobileShader; // mobile friendly skybox

        SerializedObject serialisedWindow;

        [MenuItem("Tools/Utility/Optimisation")]
        static void Init()
        {
            OculusQuestOptimizer window = (OculusQuestOptimizer)EditorWindow.GetWindow(typeof(OculusQuestOptimizer));    
            window.init();
            window.Show();
        }

        private void init()
        {
            serialisedWindow = new SerializedObject(this);

            skyboxPorperty = serialisedWindow.FindProperty("newSkybox");

            parentDepth = new Dictionary<GameObject, int>();
            childCount = new Dictionary<GameObject, int>();

            mobileShader = Shader.Find("Mobile/Skybox");

            selectionMode = SelectionMode.EverythingInOpenScene;
        }

        private void Awake()
        {
            init();
        }

        private void OnGUI() // ui update event
        {
            if(serialisedWindow == null) // if init has not been called
            {
                init();
            }

            GUILayout.Label("Optimise project for use in quest:", EditorStyles.boldLabel);

            if (GUILayout.Button("Open Quality Settings")) // button to launch projct settings optimisation window
            {
                if (EditorUtility.DisplayDialog("Warning", "Making changes to these settings is not easy to undo" +
                    "\n\nOnly coninue if you know what you are doing.", "Continue", "Cancel"))
                {
                    ProjectSettingsOptimiser.Init();
                }
            }

            GUILayout.Space(20);

            selectionMode = (SelectionMode)EditorGUILayout.EnumPopup("Edit objects in: ", selectionMode); // what will be optimised

            { // effect the hirarchy 
                if (selectionMode != SelectionMode.SelectedInHirarchyOnly && selectionMode != SelectionMode.EverythingInOpenScene) // functions that are performed on hirarchy objects
                    GUI.enabled = false; // this will grey out the buttons below

                GUILayout.Label("Objects", EditorStyles.miniBoldLabel);

                if (GUILayout.Button("List objects")) // list all objects in the scene
                    ApplyFunctionToSelectionInScene(obj => Debug.Log(obj.name, obj), "list");

                if (GUILayout.Button("Deactivate objects")) // deactivate objects in the scene
                    ApplyFunctionToSelectionInScene(obj => obj.SetActive(false), "deactivate");

                if (GUILayout.Button("List parent depth"))  // frame spikes caused by `Update Render Bounding Volume` possibly caused by too many child objects                                         
                {                                           //   Research indicates child depth is problem but could also be number of children total
                    ApplyFunctionToSelectionInScene(obj => parentDepth.Add(obj, CountParentDepth(obj)), "list"); // uses `parentDepth` dict
                    if (parentDepth != null)// not uninitialised 
                    {
                        foreach (var pair in parentDepth.OrderByDescending(key => key.Value)) // sort by value
                        {
                            Debug.Log(pair.Key.name + " has " + pair.Value + " parents, and " +pair.Key.transform.childCount+ " children.", pair.Key);
                        }
                        parentDepth.Clear(); // cleanup
                    }
                }
                
                if (GUILayout.Button("List child count"))  // if frame spikes caused by too many children                                     
                {                                           
                    ApplyFunctionToSelectionInScene(obj => childCount.Add(obj, obj.transform.childCount), "list");
                    if (childCount != null)// not uninitialised 
                    {
                        foreach (var pair in childCount.OrderByDescending(key => key.Value)) // sort by value
                        {
                            Debug.Log(pair.Key.name + " has " + pair.Value + " children, and " + CountParentDepth(pair.Key) + " parents.", pair.Key);
                        }
                        childCount.Clear(); // cleanup
                    }
                }

                GUILayout.Label("Reflection Probes", EditorStyles.miniBoldLabel);

                if (GUILayout.Button("Disable reflection probes")) // reflection probes can be expensive
                    ApplyFunctionToSelectionInScene<ReflectionProbe>(probe => probe.enabled = false, "disable");

                GUILayout.Label("Lights", EditorStyles.miniBoldLabel);

                if (GUILayout.Button("List lights")) // list the lights in the scene
                    ApplyFunctionToSelectionInScene<Light>(obj => Debug.Log(obj.name, obj), "list");

                if (GUILayout.Button("Disable light shadows and set lights to baked")) // optimise the lights
                    ApplyFunctionToSelectionInScene<Light>(light => { light.shadows = LightShadows.None; light.lightmapBakeType = LightmapBakeType.Baked; }, "alter");


                GUI.enabled = true;
            }


            { // effect the hirarchy whole scene only
                if (selectionMode != SelectionMode.EverythingInOpenScene) // functions that are performed on hirarchy objects and only effect the whole scene
                    GUI.enabled = false; // this will grey out the buttons below

                GUILayout.Label("Meshes", EditorStyles.miniBoldLabel);

                if (GUILayout.Button("List meshes by weight")) // meshes if too heavy can slow rendering significantly
                    MeshPolygonCountTool.FindMeshesAndListThierWeight();

                GUI.enabled = true;
            }

            {// effect the assets
                if (selectionMode != SelectionMode.EverythingInAssetsFolder) // functions performed on asset folder objects
                    GUI.enabled = false;

                GUILayout.Label("Materials", EditorStyles.miniBoldLabel);

                if (GUILayout.Button("List materials all")) // list all materials in the project (including bundled materials)
                    ApplyFunctionToSelectionInAssets<Material>(MaterialList, "list", returnBundledAsset: true);

                if (GUILayout.Button("List materials (excluding bundled)"))
                    ApplyFunctionToSelectionInAssets<Material>(MaterialList, "list", returnBundledAsset: false);

                if (GUILayout.Button("Optimise non-bundled materials"))
                {
                    // Asks are you sure
                    if (EditorUtility.DisplayDialog("Warning", "This action is very difficult to reverse and will cause materials" +
                        " in the inspector to display incorrect values.\n\nOnly coninue if you know what you are doing.", "Continue", "Cancel"))
                    {
                        ApplyFunctionToSelectionInAssets<Material>(MaterialOptimise, "optimise"); // optimise materials
                    }

                }

                GUILayout.Label("Skyboxes", EditorStyles.miniBoldLabel);

                if (GUILayout.Button("List skyboxes in assets")) // find all skyboxes in assets
                    ApplyFunctionToSelectionInAssets<Material>(SkyboxList, "list");

                if (GUILayout.Button("Set all skyboxes in assets to mobile friendly")) // set them all to mobile friendly
                {
                    if (EditorUtility.DisplayDialog("Warning", "This will set all skyboxes types including procedural and cubemaps to " +
                        "mobile skybox.\n\nThis may cause issues, are you sure you wish to continue", "Continue", "Cancel"))
                    {
                        ApplyFunctionToSelectionInAssets<Material>(ConvertSkyboxToMobile, "convert");
                    }
                }
            
                GUI.enabled = true;

            }

            {  // effect current scene
                if (selectionMode != SelectionMode.EverythingInOpenScene) // functions that are performed on hirarchy objects and only effect the whole scene
                    GUI.enabled = false; // this will grey out the buttons below


                if (GUILayout.Button("Find current scene skybox")) // display the currenly used skybox
                {
                    Material sky = RenderSettings.skybox;
                    Debug.Log(sky, sky);
                }
                
                if(GUILayout.Button("Set scene skybox to mobile friendly")) // set current scene skybox to mobile friendly
                {
                    Material sky = RenderSettings.skybox;
                    if (EditorUtility.DisplayDialog("Alter skybox", "Do you want to change skybox " + sky.name + " from " + sky.shader.name +
                        " to " + mobileShader.name + "?", "Yes", "Cancel"))
                    {
                        sky.shader = mobileShader;
                        Debug.Log(sky, sky); 
                    }

                }
                

                
                EditorGUILayout.PropertyField(skyboxPorperty);

                
               

                serialisedWindow.ApplyModifiedProperties();

                if (GUILayout.Button("Set scene skybox to new skybox")) // sets scene skybox to the dropped in skybox
                {
                    if (EditorUtility.DisplayDialog("Set skybox", "Set scene skybox to " + newSkybox != null ? newSkybox.name : "nothing" + "?", "Yes", "Cancel"))
                    {
                        RenderSettings.skybox = newSkybox;
                        DynamicGI.UpdateEnvironment();
                        Debug.Log(newSkybox, newSkybox);
                    }
                }


                GUI.enabled = true;

            }


            GUILayout.Space(25);



           

        }

      

        // functions used by optimisations



        // list materials in assets 
        private void MaterialList(UnityEngine.Object mat)
        {
           
            if(mat.GetType() == typeof(GameObject)) // mat could be within an asset bundle
            {
                Debug.Log("Bundle: " + mat, mat);
                return;
            }
            
            // display the material and it's properties
            string flags = mat.name + " " + ((Material)mat).shader.name + ": ";
            foreach (var str in ((Material)mat).shaderKeywords)
            {
                flags += ", " + str;
            }
            Debug.Log(flags, mat);


        }
        
        // list skyboxes in assets
        private void SkyboxList(UnityEngine.Object mat)
        {
           
            if(mat.GetType() != typeof(Material)) // mat could be within an asset bundle
            {
                Debug.Log("Bundle: " + mat, mat);
                return;
            }
            
            if (((Material)mat).shader.name.Contains("Skybox"))
            {
                string flags = mat.name + " " + ((Material)mat).shader.name + ": ";
                // display the material and it's properties
                foreach (var str in ((Material)mat).shaderKeywords)
                {
                    flags += ", " + str;
                }
                Debug.Log(flags, mat);

            }


        }

        // convert skybox to mobile
        private void ConvertSkyboxToMobile(UnityEngine.Object mat)
        {
            if (mat.GetType() != typeof(Material)) // mat could be within an asset bundle
            {
                return;
            }

            if (((Material)mat).shader.name.Contains("Skybox"))
            {
                ((Material)mat).shader = mobileShader;
            }
        }

        // optimise materials
        private void MaterialOptimise(UnityEngine.Object mat)
        {

            // these sould optimise the materials somewhat, this can be expanded
            ((Material)mat).EnableKeyword("_SPECULARHIGHLIGHTS_OFF");
            ((Material)mat).EnableKeyword("_GLOSSYREFLECTIONS_OFF");

            MaterialList(mat);
            
        }

        // count parent depth in hirarchy
        private int CountParentDepth(GameObject obj)
        {
            int localCount = 0;
            Transform t = obj.transform;
            while (t.parent != null)
            {
                t = t.parent;
                localCount++;
            }
            return localCount;
        }




        // framework functions


  
        /// <summary>
        /// Method to apply a passed-in function to all game objects that meet the selection criteria in scene
        /// </summary>
        bool ApplyFunctionToSelectionInScene(FunctionToApply<GameObject> function, string functionDescription = "")
        {

            List<GameObject> objects = GetObjectsSelectedFromGameObject();

            return ApplyFunction(function, functionDescription, objects);
            
        }


        /// <summary>
        /// Method to apply a passed-in function to all <components> that meet the selection criteria in scene
        /// </summary>
        bool ApplyFunctionToSelectionInScene<T>(FunctionToApply<T> function, string functionDescription = "effect") where T : Component
        {

            List<T> components = GetObjectsSelectedFromComponent<T>();

            return ApplyFunction(function, functionDescription, components);
        }

        /// <summary>
        /// Method to apply a passed-in function to all <components> that meet the selection criteria in assets folder
        /// </summary>
        bool ApplyFunctionToSelectionInAssets<T>(FunctionToApply<UnityEngine.Object> function, string functionDescription = "effect", bool returnBundledAsset = false) where T: UnityEngine.Object
        {
            List<UnityEngine.Object> assets = GetObjectsSelectedFromAssets<T>(returnBundledAsset);

            return ApplyFunction(function,functionDescription,assets);

        }



        // applies the function
        private bool ApplyFunction<T>(FunctionToApply<T> function, string functionName, List<T> unityObjects) where T : UnityEngine.Object
        {            

            // no items matched the search paramaters, nothing was found
            if (unityObjects.Count == 0)
            {
                EditorUtility.DisplayDialog("No " + typeof(T).Name + "s found", "There are no " + typeof(T).Name + "s to " + functionName + ".", "Ok");
                return false;
            }

            // in case pressing button by error, user is asked if they are sure they mean to apply the operation
            if (!EditorUtility.DisplayDialog(functionName + " objects?", "Continuing will " + functionName + " up to " + unityObjects.Count + " " + typeof(T).Name + "s.\nDo you wish to continue?", "Yes", "Cancel"))
                return false;

            int progressCount = 0; // for the progress bar
            foreach (var component in unityObjects)
            {
                progressCount++;
                if (progressCount % 15 == 0) // updating progress bar for each object would be more expensive than it's worth
                    EditorUtility.DisplayProgressBar(functionName + " " + typeof(T).Name + "s", "Completing...", (float)progressCount / unityObjects.Count);

                function(component); // apply the passed function
            }
            // close the progress bar
            EditorUtility.ClearProgressBar();

            return true;
        }

        // gets all type T in the assets folder, if T is part of a bundled asset then it wont be returned unless flag is set to true
        private List<UnityEngine.Object> GetObjectsSelectedFromAssets<T>(bool returnBundledAsset = false) where T : UnityEngine.Object
        {
            EditorUtility.DisplayProgressBar("Gathering Assets", "Please wait...", 0);

            List<UnityEngine.Object> objects = new List<UnityEngine.Object>(); // list that will be returned
            
            string type = typeof(T).FullName; // get the typename (this includes `parentClass.``actualTypeName`)
            type = type.Replace("UnityEngine.", null); // remove `parentClass.`
            string searchfor = "t:" + type; // this will search for the type we want
            var assetGUIDs = AssetDatabase.FindAssets(searchfor, new[] { "Assets" });

            int progressCount = 0; // for the progress bar
            foreach (var guid in assetGUIDs) // get adress of all matching types 
            {
                var path = AssetDatabase.GUIDToAssetPath(guid); // translate adress to asset path
                var obj = AssetDatabase.LoadMainAssetAtPath(path); // loads asset at path

                if(returnBundledAsset || obj.GetType() == typeof(T)) // imported assets that have components embedded/bundled will be skipped
                {
                    objects.Add(obj);
                }

                // progress bar
                progressCount++;
                if (progressCount % 15 == 0) // updating progress bar for each object would be more expensive than it's worth
                    EditorUtility.DisplayProgressBar("Gathering Assets", "Please wait...", (float)progressCount/assetGUIDs.Length);
            }
            EditorUtility.ClearProgressBar();
            return objects;
        }

        // gets all of type T that is currently highlighted, or in the entire scene depending on mode
        // must take T as child of Component for GetComponent<T> to work
        private List<T> GetObjectsSelectedFromComponent<T>() where T : Component
        {
           
            List<T> selectedComponents = new List<T>(); // list to be returned

            // depending on mode selected in editor window
            switch (selectionMode)
            {
                case SelectionMode.SelectedInHirarchyOnly:
                    List<GameObject> selectedObjects = new List<GameObject>(); // temp buffer list

                    List<Transform> selectedTransforms = new List<Transform>(Selection.GetTransforms(UnityEditor.SelectionMode.Editable)); // all transforms in selection

                    selectedObjects.AddRange(selectedTransforms.Select(obj => obj.gameObject)); // get gameobjects from transforms
                    selectedObjects.Where((obj) => obj.GetComponent<T>() as Component != null); // remove game objects that do not have the required component
                    selectedComponents.AddRange(selectedObjects.Select(obj => (T)obj.GetComponent<T>())); // get the component

                    break;

                case SelectionMode.EverythingInOpenScene:

                    selectedComponents.AddRange(GameObject.FindObjectsOfType<T>() as T[]); // get everything in the scene

                    break;
                default:
                    break;
            }

            return selectedComponents;
        }

        // same as above but T is GameObject, this seperation is unavoidable
        private List<GameObject> GetObjectsSelectedFromGameObject()
        {
            List<GameObject> selectedObjects = new List<GameObject>(); // list that will be returned

            // depending on mode selected in editor window
            switch (selectionMode)
            {
                case SelectionMode.SelectedInHirarchyOnly:
                    List<Transform> selectedTransforms = new List<Transform>(Selection.GetTransforms(UnityEditor.SelectionMode.Editable)); // all transfroms from selection
                    selectedObjects.AddRange(selectedTransforms.Select(trans => trans.gameObject)); // get gameobjects from transform

                    break;

                case SelectionMode.EverythingInOpenScene:

                    selectedObjects.AddRange(GameObject.FindObjectsOfType<GameObject>() as GameObject[]); // get everything in the scene

                    break;
                default:
                    break;
            }

            return selectedObjects;
        }


       

    }




}