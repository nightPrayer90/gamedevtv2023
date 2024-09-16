using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// https://www.youtube.com/watch?v=9O7uqbEe-xc
/// ObjectPoolManager.SpawnObject(prefab, position, rotation, 'optional: ObjectPoolManager.PoolType.Gameobject ) - for Instantiate()
/// ObjectPoolManager.ReturnObjectToPool(gameObject) - for Destroy()
/// 
/// for Particle Systems: new script - ReturnParticle to pool
/// private void OnParticleSystemStopped()
/// {
///     ObjectPoolManager.ReturnObjectToPool(gameObject);
/// }
/// 
/// for Overload use
/// ObjectPoolManager.SpawnObject(objectToSpawn, parentTransform);


public class ObjectPoolManager : MonoBehaviour
{
    public static List<PooledObjectInfo> ObjectPools = new List<PooledObjectInfo>();

    private GameObject _objectPoolEmptyHolder;

    private static GameObject _particleSystemEmpty;
    private static GameObject _gameObjectsEmpty;
    private static GameObject _objectFloatingTextEmpty;
    private static GameObject _objectPickUpEmpty;
    private static GameObject _objectWave1Empty;
    private static GameObject _objectWave2Empty;
    private static GameObject _objectWave3Empty;
    private static GameObject _objectWave4Empty;
    private static GameObject _objectWave5Empty;
    private static GameObject _objectWave6Empty;
    private static GameObject _objectWave7Empty;
    private static GameObject _objectWave8Empty;
    private static GameObject _objectWave9Empty;


    //Debug
    [SerializeField] private List<PooledObjectInfo> objectPoolsDebug;

    public enum PoolType
    {
        FloatingText,
        PickUps,
        ParticleSystem,
        Gameobject,
        Wave1,
        Wave2,
        Wave3,
        Wave4,
        Wave5,
        Wave6,
        Wave7,
        Wave8,
        Wave9,
        None
    }
    public static PoolType PoolingTyp;

    private void Awake()
    {
        
        SetupEmpties();
    }

    // Debug
    private void Update()
    {
        objectPoolsDebug = ObjectPools;
    }


    private void SetupEmpties()
    {
        _objectPoolEmptyHolder = new GameObject("Pooled Objects");
        _objectPoolEmptyHolder.transform.SetParent(gameObject.transform.parent, false);

        _objectFloatingTextEmpty = new GameObject("Floating Text");
        _objectFloatingTextEmpty.transform.SetParent(_objectPoolEmptyHolder.transform);

        _objectPickUpEmpty = new GameObject("Pick Ups");
        _objectPickUpEmpty.transform.SetParent(_objectPoolEmptyHolder.transform);

        _particleSystemEmpty = new GameObject("Particle Effects");
        _particleSystemEmpty.transform.SetParent(_objectPoolEmptyHolder.transform);

        _gameObjectsEmpty = new GameObject("Game Objects");
        _gameObjectsEmpty.transform.SetParent(_objectPoolEmptyHolder.transform);

        _objectWave1Empty = new GameObject("Wave1");
        _objectWave1Empty.transform.SetParent(_objectPoolEmptyHolder.transform);

        _objectWave2Empty = new GameObject("Wave2");
        _objectWave2Empty.transform.SetParent(_objectPoolEmptyHolder.transform);

        _objectWave3Empty = new GameObject("Wave3");
        _objectWave3Empty.transform.SetParent(_objectPoolEmptyHolder.transform);

        _objectWave4Empty = new GameObject("Wave4");
        _objectWave4Empty.transform.SetParent(_objectPoolEmptyHolder.transform);

        _objectWave5Empty = new GameObject("Wave5");
        _objectWave5Empty.transform.SetParent(_objectPoolEmptyHolder.transform);

        _objectWave6Empty = new GameObject("Wave6");
        _objectWave6Empty.transform.SetParent(_objectPoolEmptyHolder.transform);

        _objectWave7Empty = new GameObject("Wave7");
        _objectWave7Empty.transform.SetParent(_objectPoolEmptyHolder.transform);

        _objectWave8Empty = new GameObject("Wave8");
        _objectWave8Empty.transform.SetParent(_objectPoolEmptyHolder.transform);

        _objectWave9Empty = new GameObject("Wave9");
        _objectWave9Empty.transform.SetParent(_objectPoolEmptyHolder.transform);
    }

    public static GameObject SpawnObject(GameObject objectToSpawn, Vector3 spawnPosition, Quaternion spawnRotation, PoolType poolType = PoolType.None)
    {
        PooledObjectInfo pool = ObjectPools.Find(p => p.LookupString == objectToSpawn.name);

        // If the pool dosn't exist, create it
        if (pool == null)
        {
            pool = new PooledObjectInfo() { LookupString = objectToSpawn.name };
            ObjectPools.Add(pool);
        }

        // Check if there are any inactive objects in the pool
        GameObject spawnableObj = pool.InactivObjects.FirstOrDefault();

        // If there are no inactivate objects, create a new one
        if (spawnableObj == null)
        {
            // find the parent of the empty object
            GameObject parentObject = SetParentObject(poolType);

            // If there are no inactivate objects, create a new one
            spawnableObj = Instantiate(objectToSpawn, spawnPosition, spawnRotation);

            if (parentObject != null)
            {
                spawnableObj.transform.SetParent(parentObject.transform);
            }
        }

        // If there is an inactiv object, reactive it
        else
        {
            spawnableObj.transform.position = spawnPosition;
            spawnableObj.transform.rotation = spawnRotation;
            pool.InactivObjects.Remove(spawnableObj);
            spawnableObj.SetActive(true);
        }

        return spawnableObj;
    }

    public static GameObject SpawnObject_(GameObject objectToSpawn, Transform parentTransform)
    {
        PooledObjectInfo pool = ObjectPools.Find(p => p.LookupString == objectToSpawn.name);

        // If the pool dosn't exist, creat it
        if (pool == null)
        {
            pool = new PooledObjectInfo() { LookupString = objectToSpawn.name };
            ObjectPools.Add(pool);
        }

        // Check if there are any inactive objects in the pool
        GameObject spawnableObj = pool.InactivObjects.FirstOrDefault();

        // If there are no inactivate objects, create a new one
        if (spawnableObj == null)
        {


            // If there are no inactivate objects, create a new one
            spawnableObj = Instantiate(objectToSpawn, parentTransform);
        }

        // If there is an inactiv object, reactive it
        else
        {
            pool.InactivObjects.Remove(spawnableObj);
            spawnableObj.SetActive(true);
        }

        return spawnableObj;
    }

    public static void ReturnObjectToPool(GameObject obj)
    {
        string goName = obj.name.Substring(0, obj.name.Length - 7); //by taking off 7, we are removing the (Clone) from the name of the passed in obj

        PooledObjectInfo pool = ObjectPools.Find(p => p.LookupString == goName);

        if (pool == null)
        {
            Debug.Log("Trying to release an object that is not pooled: " + obj.name);
        }

        else
        {
            obj.SetActive(false);
            pool.InactivObjects.Add(obj);
        }
    }

    private static GameObject SetParentObject(PoolType poolType)
    {
        switch (poolType)
        {
            case PoolType.FloatingText:
                return _objectFloatingTextEmpty;

            case PoolType.PickUps:
                return _objectPickUpEmpty;

            case PoolType.ParticleSystem:
                return _particleSystemEmpty;
     
            case PoolType.Gameobject:
                return _gameObjectsEmpty;

            case PoolType.Wave1:
                return _objectWave1Empty;

            case PoolType.Wave2:
                return _objectWave2Empty;

            case PoolType.Wave3:
                return _objectWave3Empty;

            case PoolType.Wave4:
                return _objectWave4Empty;

            case PoolType.Wave5:
                return _objectWave5Empty;

            case PoolType.Wave6:
                return _objectWave6Empty;

            case PoolType.Wave7:
                return _objectWave7Empty;

            case PoolType.Wave8:
                return _objectWave8Empty;

            case PoolType.Wave9:
                return _objectWave9Empty;

            case PoolType.None:
                return null;

            default:
                return null;
        }
    }
}

[System.Serializable]
public class PooledObjectInfo
{
    public string LookupString;
    public List<GameObject> InactivObjects = new List<GameObject>();
        
}