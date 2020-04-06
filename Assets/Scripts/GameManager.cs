using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    /*
     * TODO:
     * -Track current game level  [+]
     * -Load and unload levels  [+]
     * -Track game state  []
     * -Generate other persistent systems  [+]
     */

    public GameObject[] SystemPrefabs;//Tracks things we want to create

    private List<GameObject> _instancedSystemPrefabs;//Tracks things that have been created
    private string _currentLevelName = string.Empty;
    List<AsyncOperation> _loadOperations;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        _instancedSystemPrefabs = new List<GameObject>();
        _loadOperations = new List<AsyncOperation>();

        InstantiateSystemPrefabs();
        LoadLevel("PathFindingTest");
    }


    public void LoadLevel(string levelName)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);//non-blocking 

        if (ao == null)
        {
            Debug.LogError("[GameManager] Unable to load level " + levelName);
            return;
        }
        _loadOperations.Add(ao);

        ao.completed += OnLoadOperationConplete;
        _currentLevelName = levelName;
    }

    public void UnloadLevel(string levelName)
    {
        AsyncOperation ao = SceneManager.UnloadSceneAsync(levelName);
        if (ao == null)
        {
            Debug.LogError("[GameManager] Unable to unload level " + levelName);
            return;
        }
        ao.completed += OnUnloadOperationConplete;
    }

    void InstantiateSystemPrefabs()
    {
        foreach(GameObject g in SystemPrefabs)
        {
            _instancedSystemPrefabs.Add(Instantiate(g));
        }
    }

    public void OnLoadOperationConplete(AsyncOperation ao)
    {
        if(_loadOperations.Contains(ao))
        {
            _loadOperations.Remove(ao);


        }
        Debug.Log("Scene Loaded");
        
    }
    public void OnUnloadOperationConplete(AsyncOperation ao)
    {
        Debug.Log("Scene Unloaded");
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _instancedSystemPrefabs.ForEach(Destroy);
        _instancedSystemPrefabs.Clear();

    }

}
