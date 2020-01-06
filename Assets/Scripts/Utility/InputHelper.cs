using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//https://answers.unity.com/questions/448771/simulate-touch-with-mouse.html

/*

 DON'T CALL THIS MORE THAN ONCE PER UPDATE

 it breaks delta position

*/
 
public class InputHelper : MonoBehaviour {
 
	private static TouchCreator lastFakeTouch;

	private static List<Touch> touches = new List<Touch>();

	public static List<Touch> GetTouches()
	{
		touches.Clear();
		touches.AddRange(Input.touches);
#if UNITY_EDITOR
		if(Input.GetMouseButtonDown(0))
		{
			if(lastFakeTouch == null) lastFakeTouch = new TouchCreator();

			lastFakeTouch.phase = TouchPhase.Began;
			lastFakeTouch.deltaPosition = new Vector2(0,0);
			lastFakeTouch.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			lastFakeTouch.fingerId = 0;
		}
		else if (Input.GetMouseButtonUp(0))
		{
			if(lastFakeTouch == null) lastFakeTouch = new TouchCreator();

			lastFakeTouch.phase = TouchPhase.Ended;
			Vector2 newPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			lastFakeTouch.deltaPosition = newPosition - lastFakeTouch.position;
			lastFakeTouch.position = newPosition;
			lastFakeTouch.fingerId = 0;
		}
		else if (Input.GetMouseButton(0))
			{
				if(lastFakeTouch == null) lastFakeTouch = new TouchCreator();

				//lastFakeTouch.phase = TouchPhase.Moved;
				Vector2 newPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
				lastFakeTouch.deltaPosition = newPosition - lastFakeTouch.position;
				lastFakeTouch.phase = TouchPhase.Moved;//lastFakeTouch.deltaPosition.magnitude == 0 ? TouchPhase.Stationary : TouchPhase.Moved;
				lastFakeTouch.position = newPosition;
				lastFakeTouch.fingerId = 0;
			}
		else
		{
			lastFakeTouch = null;
		}
		if (lastFakeTouch != null) touches.Add(lastFakeTouch.Create());
#endif
        
  		return touches;      
	}
 
}
