using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImpactCreator : MonoBehaviour {

	private float decay = 1; // set at one until decay function is created;

	// user input values
	public int numOfImpacts; // number of Impact Objects user wants to add
	public float impactRadius; // radius for each impact object

	public GameObject gridController; // backgroundGrid gameobject
	public GameObject impactObjects; // impact object prefab
	private float[,] gridMatrix; // matrix from the GridGenerator script

	public float solutionRating; // score the algorithm gets

	public Text numText; // input field text for number of impact objects
	public Text radText; // input field text for radius of impact objects

	public void calculateButtonPressed()
	{
		//clear the field of impact objects
		for (int i = 0; i < gameObject.transform.childCount; i++)
		{
			Destroy(gameObject.transform.GetChild(i).gameObject);
		}

		numOfImpacts = int.Parse(numText.text);
		impactRadius = float.Parse(radText.text);
		PlaceImpactObjects();
	}

	void PlaceImpactObjects()
	{
		// call calculate method to find array of where impact objects should go
		gridMatrix = gridController.GetComponent<GridGenerator>().gridInfoArray;
		Placement test = new Placement();
		
		Vector2[] impactArray = test.calculate(
			MatrixToVector3(gridMatrix),
			numOfImpacts,
			impactRadius,
			decay,
			gridMatrix.GetLength(1),
			gridMatrix.GetLength(0),
			ref solutionRating);
		
		
		//instantiate the impact objects
		for (int i = 0; i < impactArray.Length; i++)
		{
			GameObject impactPrefab = Instantiate(impactObjects, impactArray[i], Quaternion.identity, gameObject.transform);
			impactPrefab.GetComponent<ImpactObject>().x = (int)impactArray[i].y;
			impactPrefab.GetComponent<ImpactObject>().y = (int)impactArray[i].x;
			impactPrefab.GetComponent<ImpactObject>().radius = impactRadius;

		}
	}

	Vector3[] MatrixToVector3(float[,] matrix)
	{
		Vector3[] returnArray = new Vector3[matrix.Length];
		int index = 0;
		for (int y = 0; y < matrix.GetLength(0); y++)
		{
			for (int x = 0; x < matrix.GetLength(1); x++)
			{
				returnArray[index] = new Vector3(x, y, matrix[y, x]);
				index++;
			}
		}
		return returnArray;
	}
}
