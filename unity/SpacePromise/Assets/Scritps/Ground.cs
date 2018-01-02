using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[DisallowMultipleComponent]
public class Ground : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
		var size = 2;
		
		this.DestroyChildrenWithComponent<WaterSurface>();
		this.DestroyChildrenWithComponent<GroundChunk>();

		this.CreateChildGameObjectFromComponent<WaterSurface>();
		
		for (int verticalIndex = 0; verticalIndex < size; verticalIndex++)
		{
			for (int horizontalIndex = 0; horizontalIndex < size; horizontalIndex++)
			{
				var chunk = this.CreateChildGameObjectFromComponent<GroundChunk>();
				chunk.transform.localPosition = new Vector3(
					horizontalIndex * 500,
					0,
					verticalIndex * 500);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
