using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactObject : MonoBehaviour {

	public int x; // x coordinate for the impactObject location
	public int y; // y coordinate for the impactObject location

	public float cellSize; // size of the cell
	public float offset; // half of the cell for the offset

	public GameObject gridController; // backgroundGrid gameobject
	public GameObject gridImage; // grid image gameobject 

	public float radius; // the input radius that must be converted to radius scale
	private float radiusScale; // the actual radius value we push to the local scale
	private float oldRadius; // used to change radius on the fly

	void Start () {
		gridController = GameObject.Find("BackgroundGrid");
		gridImage = GameObject.Find("Grid");

		float gridWidth = gridController.GetComponent<GridGenerator>().width;
		float gridHeight = gridController.GetComponent<GridGenerator>().height;

		if (gridWidth > gridHeight)
		{
			cellSize = (float)(gridImage.GetComponent<RectTransform>().rect.width) / gridWidth;
		}
		else
		{
			cellSize = (float)(gridImage.GetComponent<RectTransform>().rect.height) / gridHeight;
		}

		offset = cellSize / 2;
		this.transform.localPosition = new Vector2((x * cellSize) + offset, ((gridHeight - y -1) * cellSize) + offset);

		calculateRadius();
		this.transform.localScale = new Vector3(radiusScale, radiusScale, 1f);
		oldRadius = radius;
	}

	void Update()
	{
		if (oldRadius != radius)
		{
			radius = Mathf.Abs(radius);
			oldRadius = radius;

			calculateRadius();
			Debug.Log(radiusScale);
			this.transform.localScale = new Vector3(radiusScale, radiusScale, 1f);
		}
	}

	// used to convert input radius to radiusScale
	void calculateRadius()
	{
		radiusScale = radius * cellSize * 2f; // 100 is used for scaling to the pixel size

	}

}
