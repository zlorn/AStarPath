using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour 
{
	IEnumerator OnMouseDown()  
	{
		Vector3 offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));

		while(Input.GetMouseButton(0))  
		{
			transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0)) + offset;
			yield return null;
		}
	}  
}
