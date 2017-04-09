using UnityEngine;
using LinearAlgebra;
using System.Collections;

public class SzilardAlgorithm
{
	public int PolynomialDegree;
	public double[] Coefficients;

	private LinearAlgebra.Matrices.DoubleMatrix polygonalMatrix;
	private double[] Q_Vals;

	public string GetSzilardMatrix(int k, float[] coefficients)
	{
		string OutputString = "";
		ConstructGrowthFunction (k, coefficients);
		ComputePolygonalMatrix ();
		OutputString = polygonalMatrix.ToString ();
		StoreQVals ();

		return OutputString;
	}

	public TransformDOL.RawDOLSystem GenerateRawSystem(int k)
	{
		TransformDOL.RawDOLSystem rawDOL = new TransformDOL.RawDOLSystem (k);
		rawDOL.GetAlphabet (k);
		rawDOL.GetAxiom((int)polygonalMatrix[0, 0]);
		rawDOL.GetProductions (Q_Vals);

		return rawDOL;
	}

	private void ConstructGrowthFunction(int degree, float[] coefficients)
	{
		PolynomialDegree = degree;
		Coefficients = new double[PolynomialDegree+1];
		for (int i = 0; i <= PolynomialDegree; i++)
			Coefficients [i] = coefficients [i];
	}

	private void ComputePolygonalMatrix()
	{
		polygonalMatrix = new LinearAlgebra.Matrices.DoubleMatrix (PolynomialDegree+1);
		for(int i = 0; i <= PolynomialDegree; i++)
		{
			polygonalMatrix[i, i] = ComputePolynomialGrowth(i);
			if(i > 0)
			{
				for(int j = i; j > 0; j--)
				{
					polygonalMatrix[i, j - 1] = polygonalMatrix[i, j] - polygonalMatrix[i - 1, j - 1];
				}
			}
		}
	}

	private double ComputePolynomialGrowth(int n)
	{
		double val = 0;
		for (int i = 0; i <= PolynomialDegree; i++)
			val += (Mathf.Pow (n, i) * Coefficients [i]);
		return val;
	}

	private void StoreQVals()
	{
		Q_Vals = new double[PolynomialDegree+1];
		for(int i = 0; i <= PolynomialDegree; i++)
		{
			Q_Vals[i] = Q_Value(i);
		}
	}

	private double F_Value(int n)
	{
		return polygonalMatrix[n, n];
	}

	private double Q_Value(int n)
	{
		return polygonalMatrix[n, 0];
	}
}
