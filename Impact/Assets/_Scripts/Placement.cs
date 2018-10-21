using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class Placement : MonoBehaviour {
	
	// Only call this one
	public Vector2[] calculate(Vector3[] data, int num, float radius, float decay, int width, int height, ref float bestValue)
	{
		Vector3[] rawData = data;
		data = cleanData (rawData, width, height);

		float[,] impactMap = calcImpacts (data, radius, decay, width, height);

		//Debug.Log (impactMap);
		for (int i = 0; i < impactMap.GetLength (0); i++) {
			string line = "";
			for (int x = 0; x < impactMap.GetLength (1); x++) {
				line += (impactMap [i, x] + " ");
			}
			Debug.Log (line);

		}

		Vector2[][] solSets = partitionPlace (impactMap, num);

		Vector2[] bestSet = solSets [0];
		float bestImpact = evaluate(data, solSets[0], radius, decay);
		float currImpact;

		for (int i = 1; i < solSets.Length; i++) {
			currImpact = evaluate (data, solSets [i], radius, decay);
			if (currImpact > bestImpact)
			{
				bestImpact = currImpact;
				bestSet = solSets [i];
			}


		}


		bestValue = bestImpact;
		Debug.Log (bestSet[0] + " " + impactMap[(int)bestSet[0].x, (int)bestSet[0].y]);
		return bestSet;
	}

	public float[,] calcImpacts(Vector3[] data, float radius, float decay, int width, int height) //still need to implement decay
	{
		float[,] impactMap = new float[width,height];
		for (int d = 0; d < data.Length; d++) {
			
			for (int y = (int)(radius + .5f); y >= 0; y--) {
				int x = 0;
				float distance = withinCircle (radius, data [d], new Vector2 (data [d].x + x, data [d].y + y));
				while (distance >= 0) 
				{

					if (inMap(width, height, new Vector2(data[d].x + x, data[d].y + y)))
					{
						impactMap [(int)(data [d].x + x), (int)(data [d].y + y)] += data [d].z;// / Mathf.Pow(distance, 2) * decay;
					}
					if (x != 0 && inMap(width, height, new Vector2(data[d].x - x, data[d].y + y)))
					{
						impactMap [(int)(data [d].x - x), (int)(data [d].y + y)] += data [d].z;// / Mathf.Pow(distance, 2) * decay;
					}
					if (y != 0 && inMap(width, height, new Vector2(data[d].x + x, data[d].y - y)))
					{
						impactMap[(int)(data[d].x + x), (int)(data[d].y - y)] += data[d].z;// / Mathf.Pow(distance, 2) * decay;
					}
					if (x != 0 && y != 0 && inMap(width, height, new Vector2(data[d].x - x, data[d].y - y)))
					{
						impactMap[(int)(data[d].x - x), (int)(data[d].y - y)] += data[d].z;// / Mathf.Pow(distance, 2) * decay;
					}
					x++;
					distance = withinCircle(radius, data[d], new Vector2(data[d].x + x, data[d].y + y));
				}

			}
		}



		return impactMap;
	}

	public Vector3[] cleanData(Vector3[] data, int width, int height)
	{
		List<Vector3> cleanedRaw = new List<Vector3>();
		for (int i = 0; i < data.Length; i++) {
			if (data [i].z != 0 && inMap (width, height, new Vector2 (data[i].x, data[i].y))) 
			{
				cleanedRaw.Add(data[i]);
			}
		}
		Vector3[] cleaned = cleanedRaw.ToArray();

		return cleaned;
	}


	public float withinCircle(float radius, Vector3 center, Vector2 check)
	{
		float distance = Mathf.Sqrt (Mathf.Pow (center.x - check.x, 2) + Mathf.Pow (center.y - check.y, 2));
		if (distance <= radius) {
			return distance;
		}
		return -1;

	}

	public bool inMap(int width, int height, Vector2 check)
	{
		return (check.x >= 0 && check.x < width && check.y >= 0 && check.y < height);

	}

	public Vector2[][] partitionPlace(float[,] impactMap, int num)
	{
		Vector2 globalMinVec = new Vector2(0,0);
		float globalMinVal = 0;
		for(int x=0; x < impactMap.GetLength(0); x++)
		{
			for(int y=0; y<impactMap.GetLength(1); y++)
			{
				if (impactMap [x, y] < globalMinVal) {
					globalMinVal = impactMap [x, y];
					globalMinVec = new Vector2 (x, y);
				}
			}
		}
		Vector2[][] solSets = new Vector2[4][];

		solSets[0] = partitionVertical (impactMap, num, globalMinVal);
		solSets[1] = partitionHorizontal (impactMap, num, globalMinVal);
		solSets[2] = partitionCheckers (impactMap, num, globalMinVal);
		solSets[3] = partitionSpokes (impactMap, num, globalMinVal);



		return solSets;
	}

	public Vector2[] partitionVertical (float[,] impactMap, int num, float globalMinVal)
	{
		float[] currMax = new float[num];
		for (int i = 0; i < num; i++)
		{
			currMax [i] = globalMinVal - 1;
		}

		Vector2[] solutions = new Vector2[num];
		for(int x=0; x < impactMap.GetLength(0); x++)
		{
			for(int y=0; y<impactMap.GetLength(1); y++)
			{
				int index=(int)(num*x/impactMap.GetLength(0));
				if (impactMap [x, y] > currMax [index]) {
					currMax [index] = impactMap [x, y];
					solutions [index] = new Vector2 (x,y);
				}
			}
		}

		return solutions;
	}

	public Vector2[] partitionHorizontal (float[,] impactMap, int num, float globalMinVal)
	{
		float[] currMax = new float[num];
		for (int i = 0; i < num; i++)
		{
			currMax [i] = globalMinVal - 1;
		}

		Vector2[] solutions = new Vector2[num];
		for(int x=0; x < impactMap.GetLength(0); x++)		//Finding absolute lowest impact score so that we can populate all solutionsets with the globalmin -1
		{													//This way, we have a way to check if a solution set failed to assign any well to a particular partition
			for(int y=0; y<impactMap.GetLength(1); y++)
			{
				int index=(int)(num*y/impactMap.GetLength(1));
				if (impactMap [x, y] > currMax [index]) {
					currMax [index] = impactMap [x, y];
					solutions [index] = new Vector2 (x, y);
				}
			}
		}

		return solutions;
	}

	public Vector2[] partitionCheckers (float[,] impactMap, int num, float globalMinVal)
	{
		int col, row = (int)Mathf.Sqrt(num);
		while (num % row != 0 && row > 1)
		{
			row--;
		}

		col = num / row;

		float[] currMax = new float[num];
		for (int i = 0; i < num; i++)
		{
			currMax [i] = globalMinVal - 1;
		}
		Vector2[] solutions = new Vector2[num];
		for(int x=0; x < impactMap.GetLength(0); x++)
		{
			for(int y=0; y<impactMap.GetLength(1); y++)
			{
				int index=(int)(col*x/impactMap.GetLength(0)) * row + (int)(row * y/impactMap.GetLength(1));
				if (impactMap [x, y] > currMax [index]) {
					currMax [index] = impactMap [x, y];
					solutions [index] = new Vector2 (x, y);
				}
			}
		}


		return solutions;
	}

	public Vector2[] partitionSpokes (float[,] impactMap, int num, float globalMinVal)
	{
		Vector2 center = new Vector2 (impactMap.GetLength (0) / 2.0f, impactMap.GetLength (1) / 2.0f);
		
		float[] currMax = new float[num];
		for (int i = 0; i < num; i++)
		{
			currMax [i] = globalMinVal - 1;
		}
		Vector2[] solutions = new Vector2[num];
		for(int x=0; x < impactMap.GetLength(0); x++)
		{
			for(int y=0; y<impactMap.GetLength(1); y++)
			{
				if(y==center.y)
				{
					continue;
				}
				float theta = Mathf.Atan ((x-center.x) / (y - center.y));
				if (center.y > y)
				{
					theta = Mathf.PI + theta;
				}
				theta += Mathf.PI / 2;

				int index=(int)(theta * num / Mathf.PI / 2);

				//Debug.Log(theta);

				if (impactMap [x, y] > currMax [index]) {
					currMax [index] = impactMap [x, y];
					solutions [index] = new Vector2 (x, y);
				}
			}
		}

		return solutions;
	}

	public float evaluate(Vector3[] data, Vector2[] wells, float radius, float decay)
	{
		float total = 0;
		for (int j = 0; j < data.Length; j++) {
			float max = radius;
			int currMax = -1;
			for (int k = 0; k < wells.Length; k++) {
				float distance = withinCircle(radius,data[j],wells[k]);
				if (distance != -1&&distance<=max) {

					distance = max;
					currMax = k;
				}
			}
			if(currMax!=-1)
			{
				//total += data[j].z / Mathf.Pow(max, 2) * decay;
				total+=data[j].z;
			}
		}
		return total;
	}


}
