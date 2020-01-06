OVERVIEW

This system allows to spawn/de-spawn a lot of objects without losing performance for expensive instantiate/destroy functions.
It is incredible easy to use and doesn’t require any additional coding. 
Moreover, clear and well documented source code (with no DLLs) will allow you to improve/adapt it easily, if needed.

The system allows:
   •	Create and maintain a pool of preloaded objects 
   •	Spawn/de-spawn them without performance loss
   •	Access pools either by name or ID
   •	You can add any object to pool (even wasn’t pooled before)

This system works on all platforms supported by Unity3D.





HOW TO USE

To use this system – you should just:
1.	Add PoolManager script to any object and specify list (and quantity) of objects you’d like to preload.
2.	If needed - for this list assign prefabs of objects to preload(generate objects on Start) 
3.	It’s ready! 
          You can extract preloaded or manually pooled objects using functions:
            •	GetObjectByName (objectName: String)
            •	GetObjectByID (id: int)
          You can pool any objects manually by function:
            •	PoolObject  (object: GameObject, PreloadedTypeOnly: boolean)

4.	ADDITIONAL:  Assign pooledObject script to any object (and set any existing PoolManager to parentPool property) to make this object pool-back to parentPool automatically onDIsable event.







FOR MORE INFO - please check Manual.pdf (".\_Easy_ObjectsPool_System_\Manual.pdf")
