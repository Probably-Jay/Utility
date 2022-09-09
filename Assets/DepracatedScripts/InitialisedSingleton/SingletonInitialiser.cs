using Singleton;
using UnityEngine;


// created Jay 04/03

 /// <summary>
 /// Class responsible for ensuring there is only ever exactly one of each singleton in each scene
 /// </summary>
public class SingletonInitialiser : MonoBehaviour
{
   


    private void Awake()
    {
        CreateSingletons();
    }

    private void CreateSingletons()
    {
      //  CreateSingleon<EventsManager>(eventsManager);
     
    }

    private void CreateSingleon<T>(GameObject singletonPrefab) where T : Singleton<T>
    {
        if (Singleton<T>.InstanceExists)
        {
            return;
        }

        if(singletonPrefab == null)
        {
            Singleton<T>.WarnInstanceDoesNotExist();
            return;
        }

        var singletonObject = Instantiate(singletonPrefab);
      //  singletonObject.GetComponent<T>().Initialise();
    }
}
