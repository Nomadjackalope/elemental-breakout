//--------------------------------------------------------------------------------------------------------
// Service script for pooled objects or objects to pool 
// Script allow to pool objects to custom (or parent) pool manualy or onDisable
//--------------------------------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PooledObject : MonoBehaviour 
{
	public PoolManager parentPool;		 // Link to PoolManager to add object
	public bool poolOnDisable = true;   // Set true if you want to pool object automatiacaly on Disable event

	public bool enableWarnings = true;

	//=======================================================================================================
	// Call to - Pool objects to current(parent) pool manualy 
	public void Pool () 
	{
		if (parentPool)
			parentPool.PoolObject(gameObject, false);
		else 
			if (enableWarnings) 
				Debug.Log ("parentPool is missing in PooledObject component of " + gameObject.name);
	}

	//------------------------------------------------------------------------
	// Call to pool objects to custom  pool manualy 
	public void PoolTo (PoolManager poolManager) 
	{
		if (poolManager)
			poolManager.PoolObject(gameObject, false);
		else 
			if (enableWarnings)
				Debug.Log ("PoolTo void have called with missing (or wrong) PoolManager parameter. For gameObject " + gameObject.name);
	}

	//------------------------------------------------------------------------
	// Automatiacally pools objects to current (parent) pool onDisable
	public void OnDisable  () 
	{
		if(poolOnDisable) 
			Invoke("Pool", 0.1f);

	}

	// Helps to avoid useless spam of warnings on ApplicationQuit
	public void OnApplicationQuit ()
	{
		enableWarnings = false;
	}

	//------------------------------------------------------------------------
}