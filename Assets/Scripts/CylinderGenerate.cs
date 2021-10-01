using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyliderGenerate : MonoBehaviour
{
	public MeshFilter mf;
	public Mesh mesh;

	public GameObject cube;

	[Range(10, 20)]
	public int iter;
	int num;
	public int leng;
	Vector3[] vertices;
	int[] tris;
	int[] FinalTri;
	int[] firstplane;
	void Start()
	{
		MeshFilter mf = GetComponent<MeshFilter>();
		mesh = new Mesh();
		MakingVertices(1, iter, leng, 0.5f, 0.1f);
	}

	void MakingVertices(int radius, int iterations, int lenggth, float gap, float noise)
	{
		float noise_x;
		float noise_y;
		float noise_z;
		float x;
		float y;
		float z = 0;
		int i;
		int p = 0;
		float angle;

		vertices = new Vector3[(iterations * lenggth) + 2];
		int tempo = 0;
		vertices[vertices.Length - 2] = Vector3.zero;

		while (p < lenggth)
		{
			i = 0;
			while (i < iterations)
			{
				angle = (i * 1.0f) / iterations * Mathf.PI * 2;
				x = Mathf.Sin(angle) * radius;
				y = Mathf.Cos(angle) * radius;
				vertices[tempo] = new Vector3(x, y, z);
				//GameObject go = Instantiate(cube, vertices[tempo], Quaternion.identity);
				//go.name = num.ToString();
				i++;
				num++;
				tempo += 1;
			}
			z += gap;
			p++;
		}


		vertices[vertices.Length - 1] = new Vector3(0, 0, vertices[vertices.Length - 3].z);
		Debug.Log("Vertices: " + num);
		mesh.vertices = vertices;
		MakingNormals();
	}
	void MakingNormals()
	{
		int i = 0;
		Vector3[] normals = new Vector3[num + 2];
		while (i < num)
		{
			normals[i] = Vector3.forward;
			i++;
		}
		mesh.normals = normals;

		MakingTrianges();
	}
	void MakingTrianges()
	{
		int i = 0;
		tris = new int[((3 * (leng - 1) * iter) * 2) + 3];
		while (i < (leng - 1) * iter)
		{
			tris[i * 3] = i;
			if ((i + 1) % iter == 0)
			{
				tris[i * 3 + 1] = 1 + i - iter;
			}
			else
			{
				tris[i * 3 + 1] = 1 + i;
			}
			tris[i * 3 + 2] = iter + i;
			i++;
		}
		int IndexofNewTriangles = -1;

		for (int u = (tris.Length - 3) / 2; u < tris.Length - 6; u += 3)
		{
			//mesh.RecalculateTangents();
			if ((IndexofNewTriangles + 2) % iter == 0)
			{
				tris[u] = IndexofNewTriangles + iter * 2 + 1;
			}
			else
				tris[u] = IndexofNewTriangles + iter + 1;

			tris[u + 1] = IndexofNewTriangles + 2;
			tris[u + 2] = IndexofNewTriangles + iter + 2;
			IndexofNewTriangles += 1;
		}
		tris[tris.Length - 3] = 0;
		tris[tris.Length - 2] = (iter * 2) - 1;
		tris[tris.Length - 1] = iter;

		firstplane = new int[(iter * 3) * 2];
		int felmnt = 0;
		for (int h = 0; h < firstplane.Length / 2; h += 3)
		{

			firstplane[h] = felmnt;

			if (felmnt + 1 != iter)
				firstplane[h + 1] = felmnt + 1;
			else
				firstplane[h + 1] = 0;
			firstplane[h + 2] = vertices.Length - 2;
			felmnt += 1;
		}

		felmnt = iter * (leng - 1);
		for (int h = firstplane.Length / 2; h < firstplane.Length; h += 3)
		{

			firstplane[h] = felmnt;

			if (felmnt + 1 != iter * (leng - 1))
				firstplane[h + 1] = felmnt + 1;
			else
				firstplane[h + 1] = iter * (leng - 1);
			firstplane[h + 2] = vertices.Length - 1;
			felmnt += 1;
		}

		firstplane[firstplane.Length - 3] = iter * (leng - 1);
		firstplane[firstplane.Length - 2] = vertices.Length - 3;
		firstplane[firstplane.Length - 1] = vertices.Length - 1;

		FinalTri = new int[tris.Length + firstplane.Length];

		int k = 0, l = 0;
		for (k = 0, l = 0; k < tris.Length; k++)
		{
			FinalTri[l++] = tris[k];
		}
		for (k = 0; k < firstplane.Length; k++)
		{
			FinalTri[l++] = firstplane[k];
		}

		mesh.triangles = FinalTri;
		mesh.Optimize();
		mesh.RecalculateNormals();
		mf.mesh = mesh;
	}
}
