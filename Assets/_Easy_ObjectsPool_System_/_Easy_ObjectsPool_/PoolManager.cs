//--------------------------------------------------------------------------------------------------------
// This is the main script to handle  objects pool.
// Script  allow to create pool of preseted objects in needed quantity and  handle  their  extraction or pooling back
// If needed you can add any object to pool. It's  better to use PooledObject script with objects to add.
//--------------------------------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PoolManager : MonoBehaviour 
{
	// Class to specify object parameters to preload to pool
	[System.Serializable]
	public class PoolingObjects
	{
		public string customName;				// Specify custom name if  you want all instances to be renamed
		public GameObject objectPrefab;			// Prefab to instanciate and  pool
		public int quantity = 1;				// Quantity of instances
		public bool addAutoPoolScript = true;  	// Add script to pool object back automatically
	}


	public PoolingObjects[] preloadObjects; 					// List of objects that should be preloaded to the pool
	// public List<GameObject> Pool = new List<GameObject> ();	// Array of all stored objects
	public Dictionary<string, List<GameObject>> poolDict = new Dictionary<string, List<GameObject>>();
	// List<GameObject> pool = new List<GameObject>(); // Array of only one type

	//=======================================================================================================
	// Preload all objects in needed quantity (specified in preloadObjects list) to pool 
	void Start () 
	{

		Random.InitState (System.DateTime.Now.Millisecond);

		for (int i=0; i < preloadObjects.Length; i++)
		{
			if (preloadObjects[i] != null)
				poolDict.Add(preloadObjects[i].objectPrefab.name, new List<GameObject>(preloadObjects[i].quantity));
				for (int j=0; j < preloadObjects[i].quantity; j++)
				{
					preloadObject(preloadObjects[i], true);
				}
		}
	}

	private GameObject preloadObject(string name, bool pool) {
		for (int i = 0; i < preloadObjects.Length; i++)
		{
			if(preloadObjects[i].customName == name) {
				return preloadObject(preloadObjects[i], pool);
			}	
		}

		return null;
	}

	private GameObject preloadObject(PoolingObjects preloadObject, bool pool) {
		GameObject newObj;
		PooledObject gameObjScript;

		if (preloadObject.customName == "") 
			preloadObject.customName = preloadObject.objectPrefab.name;
			
		// Instantiate object and reset it transformation. Assign it as child to Pool
		newObj = Instantiate(preloadObject.objectPrefab) as GameObject;
		newObj.name = preloadObject.customName;

		// Add PooledObject script, if needed		 
		if (preloadObject.addAutoPoolScript)
		{
			newObj.AddComponent<PooledObject>();
			gameObjScript = newObj.GetComponent<PooledObject>();
			gameObjScript.parentPool = this;
		}

		// Add the new object to pool
		if(pool) {
			PoolObject(newObj, false);
		}

		return newObj;
				
	}

	//------------------------------------------------------------------------
	// Extract gameObject from pool by name (customName if it's  specified)
	public GameObject GetObjectByName(string objectName)
	{
		if(poolDict.ContainsKey(objectName) && poolDict[objectName].Count > 0) {
			return ExtractGameObject(objectName);
		}
		// for (int i=0; i < Pool.Count; i++)
		// 	if (Pool[i] != null)
		// 		if (Pool[i].name == objectName)
		// 		{		
		// 		  	return ExtractGameObject(i);
		// 		}


		// Add new object to pool if pool is too small
		for (int i = 0; i < preloadObjects.Length; i++)
		{
			if(preloadObjects[i] != null && preloadObjects[i].customName == objectName) {
				return preloadObject(objectName, false);
			}
		}

		return null;
	}

	//------------------------------------------------------------------------
	// Extract gameObject from pool by ID in the pool
	// public GameObject GetObjectByID(int id)
	// {

	// 	if (Pool.Count > id)
	// 	{
	// 		return ExtractGameObject(id);
	// 	}

	// 	return null;
	// }

	// private GameObject ExtractGameObject(int id) {
	// 	GameObject gameObj = Pool[id];
	// 	Pool.RemoveAt(id);

	// 	if (gameObj)
	// 	{
	// 		gameObj.transform.parent = null;
	// 		gameObj.SetActive(true);

	// 		return gameObj;
	// 	}

	// 	return null;
	// }

	private GameObject ExtractGameObject(string objectName) {
		GameObject gameObj = poolDict[objectName][0];
		poolDict[objectName].RemoveAt(0);

		if(gameObj) {
			gameObj.transform.parent = null;
			gameObj.SetActive(true);

			return gameObj;
		}

		return null;
	}

	//------------------------------------------------------------------------
	// Extract random gameObject from pool
	// public GameObject GetRandomObject()
	// {
	// 	GameObject gameObj;

	// 	if (Pool.Count > 0)  
	// 	{
	// 		int id = Mathf.FloorToInt(Random.Range(0, Pool.Count));
	// 		gameObj = Pool[id];
	// 		Pool.RemoveAt(id);

	// 		if (gameObj)
	// 		{
	// 			gameObj.transform.parent = null;
	// 			gameObj.SetActive(true);

	// 			return gameObj;
	// 		}
	// 	}

	// 	return null;

	// }

	//------------------------------------------------------------------------
	// Pool object back to pool and reset object transformation. 
	// Set PreloadedTypeOnly to true if  you'd like to allow addingonly objects with one of the names from preloadObjects
	public void PoolObject (GameObject _object, bool PreloadedTypeOnly)
	{
		if (_object && !poolDict[_object.name].Contains(_object))
			if (!PreloadedTypeOnly) 
			{
				ResetObjectTransform(_object);
				_object.SetActive(false);			
				_object.transform.parent = transform;
				poolDict[_object.name].Add(_object);
			}
			else  // If only objects with preseted names allowed
				for (int i=0; i<preloadObjects.Length; i++)
				{
					if(preloadObjects[i].customName == _object.name)
					{
						ResetObjectTransform(_object);
						_object.SetActive(false);
						_object.transform.parent = transform;
						poolDict[_object.name].Add(_object);
					}
				}

	}

	//------------------------------------------------------------------------
	// Reset object position and rotation(and make it a child of this game object) and disable it
	public GameObject ResetObjectTransform(GameObject _object)
	{
		if (_object)
		{
			_object.transform.parent = transform;
			_object.transform.localPosition = Vector3.zero;
			_object.transform.localRotation = transform.localRotation;

			if(_object.GetComponent<Rigidbody>())
			{
				_object.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
				_object.GetComponent<Rigidbody>().angularVelocity = new Vector3(0,0,0);
			}

			// Maybe reset animation to beginning

			return _object;
		}

		return null;
	}

	//------------------------------------------------------------------------
}