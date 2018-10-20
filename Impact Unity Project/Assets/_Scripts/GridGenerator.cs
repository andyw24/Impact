using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using System;

/*
 * GridGenerator has 3 methods: 
 *		CreateGridFromGridInfo() 
 *			- creates a Texture2D from the variables inside this script's variables
 *		ReadGridInfo(string fileName)
 *			- reads a gridInfo.txt file and sets the variables inside this script
 *		MakeGridInfo(string fileName)
 *			- mostly for testing purposes
 *			- randomly generates a gridInfo.txt
 */

public class GridGenerator : MonoBehaviour
{
	public int width; // width of the grid
	public int height; // height of the grid
	public float max; // max or min population in a cell for color generation
	public TextAsset gridInfo; // gridInfo.txt file
	public GameObject gridImage; // image gameobject that holds the grid

	public GameObject impactObjects; // prefab impactObject

	private string fileName; // fileName of gridInfo.txt
	private int[,] gridInfoArray; // gridArray that was read from file
	Texture2D grid; // texture 2D that we are creating

	void Start()
	{
		fileName = "Assets/gridInfo.txt";
		MakeGridInfo(fileName);
		ReadGridInfo(fileName);
		CreateGridFromGridInfo();
		grid.filterMode = FilterMode.Point;
		gridImage.GetComponent<Image>().preserveAspect = true;

	}

	// Creates a Texture2D from the gridInfo.txt file
	void CreateGridFromGridInfo()
	{
		grid = new Texture2D(this.width, this.height, TextureFormat.ARGB32, false);
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				float c = gridInfoArray[x, y] / max;
				if (c >= 0)
				{
					c = 1 - c;
					grid.SetPixel(x, y, new Color(1f, c, c));
				} else
				{
					c = 1 - Mathf.Abs(c);
					grid.SetPixel(x, y, new Color(c, c, 1f));
				}
			}
		}
		grid.Apply();
		gridImage.GetComponent<Image>().sprite = Sprite.Create(grid, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
		byte[] bytes = grid.EncodeToPNG();
		File.WriteAllBytes(Application.dataPath + "/grid.png", bytes);
	}

	// Reads a file and sets GridGenerator's width, height, max, and gridInfoArray[,] accordingly
	public void ReadGridInfo(string fileName)
	{
		StreamReader sr = new StreamReader(fileName);
		//reading the first line for width and height and max
		string wString = "";
		string hString = "";
		string whLine = sr.ReadLine();
		int c = 0;
		while (!whLine.Substring(c, 1).Equals(" "))
		{
			wString += whLine.Substring(c, 1);
			c++;
		}
		c++;
		while (c < whLine.Length)
		{
			hString += whLine.Substring(c, 1);
			c++;
		}
		//reading the rest of the file and putting everything into the int[,]
		width = int.Parse(wString);
		height = int.Parse(hString);
		max = 0;
		int[,] gridArray = new int[width, height];
		try
		{
			for (int y = 0; y < height; y++)
			{
				string[] row = sr.ReadLine().Split(' ');
				for (int x = 0; x < width; x++)
				{
					int cellValue = int.Parse(row[x]);
					gridArray[x, y] = cellValue;
					if (Mathf.Abs(cellValue) > max)
					{
						max = cellValue;
					}
				}
			}
		}
		catch (FileNotFoundException) { }
		catch (IOException) { }
		gridInfoArray = gridArray;
	}

	// Makes a random gridInfo.txt file
	public void MakeGridInfo(string fileName)
	{
		try
		{
			StreamWriter sw = new StreamWriter(fileName);

			sw.WriteLine(width + " " + height); //first line in file is width and height

			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					sw.Write((int)(UnityEngine.Random.Range(0, 100f)));
					sw.Write(" ");
				}
				sw.Write("\n");
			}

			sw.Close();
		}
		catch (Exception) {}
	}
	
}
