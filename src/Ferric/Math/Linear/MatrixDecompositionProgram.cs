using System;
//using System.Threading.Tasks; // optional for TPL parallelization

// library of static matrix methods including decomposition, inverse, determinant

namespace MatrixDecomposition
{
  class MatrixDecompositionProgram
  {
    static void Main(string[] args)
    {
      Console.WriteLine("\nBegin matrix decomposition demo\n");

      double[][] m = MatrixCreate(4, 4);
      m[0][0] = 3.0; m[0][1] = 7.0; m[0][2] = 2.0; m[0][3] = 5.0;
      m[1][0] = 1.0; m[1][1] = 8.0; m[1][2] = 4.0; m[1][3] = 2.0;
      m[2][0] = 2.0; m[2][1] = 1.0; m[2][2] = 9.0; m[2][3] = 3.0;
      m[3][0] = 5.0; m[3][1] = 4.0; m[3][2] = 7.0; m[3][3] = 1.0;

      Console.WriteLine("Matrix m = \n" + MatrixAsString(m));

      int[] perm;
      int toggle;
      double[][] luMatrix = MatrixDecompose(m, out perm, out toggle);
      double[][] lower = ExtractLower(luMatrix);
      double[][] upper = ExtractUpper(luMatrix);

      Console.WriteLine("The (combined) LUP decomposition of m is\n" + MatrixAsString(luMatrix));
      Console.WriteLine("The decomposition permutation array is: " + VectorAsString(perm));
      Console.WriteLine("The decomposition toggle value is: " + toggle);
      Console.WriteLine("\nThe lower part of LU is \n" + MatrixAsString(lower));
      Console.WriteLine("The upper part of LU is \n" + MatrixAsString(upper));

      double[][] inverse = MatrixInverse(m);
      Console.WriteLine("Inverse of m computed from its decomposition is\n" + MatrixAsString(inverse));

      double det = MatrixDeterminant(m);
      Console.WriteLine("Determinant of m computed via decomposition = " + det.ToString("F1"));

      Console.WriteLine("\nLinear system mx = b problem: \n");
      Console.WriteLine("3x0 + 7x1 + 2x2 + 5x3 = 49");
      Console.WriteLine(" x0 + 8x1 + 4x2 + 2x3 = 30");
      Console.WriteLine("2x0 +  x1 + 9x2 + 3x3 = 43");
      Console.WriteLine("5x0 + 4x1 + 7x2 +  x3 = 52");
      Console.WriteLine("");

      double[] b = new double[] { 49.0, 30.0, 43.0, 52.0 };
      Console.WriteLine("Solving system via decomposition");
      double[] x = SystemSolve(m, b);
      Console.WriteLine("\nSolution is x = \n" + VectorAsString(x));

      // extra: matrix decomposition concepts
      double[][] lu = MatrixProduct(lower, upper);
      double[][] orig = UnPermute(lu, perm);  // use a custom method with the perm array to unscramble LU
      if (MatrixAreEqual(orig, m, 0.000000001) == true)
        Console.WriteLine("Product of L and U successfully unpermuted using perm array");

      double[][] permMatrix = PermArrayToMatrix(perm); // convert the perm array to a matrix
      orig = MatrixProduct(permMatrix, lu); // another way to unscramble
      if (MatrixAreEqual(orig, m, 0.000000001) == true)
        Console.WriteLine("\nProduct of L and U successfully unpermuted using perm matrix\n");

      Console.WriteLine("End matrix decomposition demo\n");
      Console.ReadLine();
    }

    // static matrix methods - consider placing in a class.

    // --------------------------------------------------------------------------------------------------------------

    static double[][] MatrixCreate(int rows, int cols)
    {
      // allocates/creates a matrix initialized to all 0.0. assume rows and cols > 0
      // do error checking here
      double[][] result = new double[rows][];
      for (int i = 0; i < rows; ++i)
        result[i] = new double[cols];

      //for (int i = 0; i < rows; ++i)
      //  for (int j = 0; j < cols; ++j)
      //    result[i][j] = 0.0; // explicit initialization needed in some languages

      return result;
    }

    // --------------------------------------------------------------------------------------------------------------

    static double[][] MatrixRandom(int rows, int cols, double minVal, double maxVal, int seed)
    {
      // return a matrix with random values
      Random ran = new Random(seed);
      double[][] result = MatrixCreate(rows, cols);
      for (int i = 0; i < rows; ++i)
        for (int j = 0; j < cols; ++j)
          result[i][j] = (maxVal - minVal) * ran.NextDouble() + minVal;
      return result;
    }

    // --------------------------------------------------------------------------------------------------------------

    static double[][] MatrixIdentity(int n)
    {
      // return an n x n Identity matrix
      double[][] result = MatrixCreate(n, n);
      for (int i = 0; i < n; ++i)
        result[i][i] = 1.0;

      return result;
    }

    // --------------------------------------------------------------------------------------------------------------

    static string MatrixAsString(double[][] matrix)
    {
      string s = "";
      for (int i = 0; i < matrix.Length; ++i)
      {
        for (int j = 0; j < matrix[i].Length; ++j)
          s += matrix[i][j].ToString("F3").PadLeft(8) + " ";
        s += Environment.NewLine;
      }
      return s;
    }

    // --------------------------------------------------------------------------------------------------------------

    static bool MatrixAreEqual(double[][] matrixA, double[][] matrixB, double epsilon)
    {
      // true if all values in matrixA == corresponding values in matrixB
      int aRows = matrixA.Length; int aCols = matrixA[0].Length;
      int bRows = matrixB.Length; int bCols = matrixB[0].Length;
      if (aRows != bRows || aCols != bCols)
        throw new Exception("Non-conformable matrices in MatrixAreEqual");

      for (int i = 0; i < aRows; ++i) // each row of A and B
        for (int j = 0; j < aCols; ++j) // each col of A and B
          //if (matrixA[i][j] != matrixB[i][j])
          if (Math.Abs(matrixA[i][j] - matrixB[i][j]) > epsilon)
            return false;
      return true;
    }

    // --------------------------------------------------------------------------------------------------------------

    static double[][] MatrixProduct(double[][] matrixA, double[][] matrixB)
    {
      int aRows = matrixA.Length; int aCols = matrixA[0].Length;
      int bRows = matrixB.Length; int bCols = matrixB[0].Length;
      if (aCols != bRows)
        throw new Exception("Non-conformable matrices in MatrixProduct");

      double[][] result = MatrixCreate(aRows, bCols);

      for (int i = 0; i < aRows; ++i) // each row of A
        for (int j = 0; j < bCols; ++j) // each col of B
          for (int k = 0; k < aCols; ++k) // could use k < bRows
            result[i][j] += matrixA[i][k] * matrixB[k][j];

      //Parallel.For(0, aRows, i =>
      //  {
      //    for (int j = 0; j < bCols; ++j) // each col of B
      //      for (int k = 0; k < aCols; ++k) // could use k < bRows
      //        result[i][j] += matrixA[i][k] * matrixB[k][j];
      //  }
      //);

      return result;
    }

    // --------------------------------------------------------------------------------------------------------------

    static double[] MatrixVectorProduct(double[][] matrix, double[] vector)
    {
      // result of multiplying an n x m matrix by a m x 1 column vector (yielding an n x 1 column vector)
      int mRows = matrix.Length; int mCols = matrix[0].Length;
      int vRows = vector.Length;
      if (mCols != vRows)
        throw new Exception("Non-conformable matrix and vector in MatrixVectorProduct");
      double[] result = new double[mRows]; // an n x m matrix times a m x 1 column vector is a n x 1 column vector
      for (int i = 0; i < mRows; ++i)
        for (int j = 0; j < mCols; ++j)
          result[i] += matrix[i][j] * vector[j];
      return result;
    }

    // --------------------------------------------------------------------------------------------------------------

    static double[][] MatrixDecompose(double[][] matrix, out int[] perm, out int toggle)
    {
      // Doolittle LUP decomposition with partial pivoting.
      // rerturns: result is L (with 1s on diagonal) and U; perm holds row permutations; toggle is +1 or -1 (even or odd)
      int rows = matrix.Length;
      int cols = matrix[0].Length; // assume all rows have the same number of columns so just use row [0].
      if (rows != cols)
        throw new Exception("Attempt to MatrixDecompose a non-square mattrix");

      int n = rows; // convenience

      double[][] result = MatrixDuplicate(matrix); // make a copy of the input matrix

      perm = new int[n]; // set up row permutation result
      for (int i = 0; i < n; ++i) { perm[i] = i; }

      toggle = 1; // toggle tracks row swaps. +1 -> even, -1 -> odd. used by MatrixDeterminant

      for (int j = 0; j < n - 1; ++j) // each column
      {
        double colMax = Math.Abs(result[j][j]); // find largest value in col j
        int pRow = j;
        for (int i = j + 1; i < n; ++i)
        {
          if (result[i][j] > colMax)
          {
            colMax = result[i][j];
            pRow = i;
          }
        }

        if (pRow != j) // if largest value not on pivot, swap rows
        {
          double[] rowPtr = result[pRow];
          result[pRow] = result[j];
          result[j] = rowPtr;

          int tmp = perm[pRow]; // and swap perm info
          perm[pRow] = perm[j];
          perm[j] = tmp;

          toggle = -toggle; // adjust the row-swap toggle
        }

        if (Math.Abs(result[j][j]) < 1.0E-20) // if diagonal after swap is zero . . .
          return null; // consider a throw

        for (int i = j + 1; i < n; ++i)
        {
          result[i][j] /= result[j][j];
          for (int k = j + 1; k < n; ++k)
          {
            result[i][k] -= result[i][j] * result[j][k];
          }
        }
      } // main j column loop

      return result;
    } // MatrixDecompose

    // --------------------------------------------------------------------------------------------------------------

    static double[][] MatrixInverse(double[][] matrix)
    {
      int n = matrix.Length;
      double[][] result = MatrixDuplicate(matrix);

      int[] perm;
      int toggle;
      double[][] lum = MatrixDecompose(matrix, out perm, out toggle);
      if (lum == null)
        throw new Exception("Unable to compute inverse");

      double[] b = new double[n];
      for (int i = 0; i < n; ++i)
      {
        for (int j = 0; j < n; ++j)
        {
          if (i == perm[j])
            b[j] = 1.0;
          else
            b[j] = 0.0;
        }

        double[] x = HelperSolve(lum, b); // 

        for (int j = 0; j < n; ++j)
          result[j][i] = x[j];
      }
      return result;
    }

    // --------------------------------------------------------------------------------------------------------------

    static double MatrixDeterminant(double[][] matrix)
    {
      int[] perm;
      int toggle;
      double[][] lum = MatrixDecompose(matrix, out perm, out toggle);
      if (lum == null)
        throw new Exception("Unable to compute MatrixDeterminant");
      double result = toggle;
      for (int i = 0; i < lum.Length; ++i)
        result *= lum[i][i];
      return result;
    }

    // --------------------------------------------------------------------------------------------------------------

    static double[] HelperSolve(double[][] luMatrix, double[] b) // helper
    {
      // before calling this helper, permute b using the perm array from MatrixDecompose that generated luMatrix
      int n = luMatrix.Length;
      double[] x = new double[n];
      b.CopyTo(x, 0);

      for (int i = 1; i < n; ++i)
      {
        double sum = x[i];
        for (int j = 0; j < i; ++j)
          sum -= luMatrix[i][j] * x[j];
        x[i] = sum;
      }

      x[n - 1] /= luMatrix[n - 1][n - 1];
      for (int i = n - 2; i >= 0; --i)
      {
        double sum = x[i];
        for (int j = i + 1; j < n; ++j)
          sum -= luMatrix[i][j] * x[j];
        x[i] = sum / luMatrix[i][i];
      }

      return x;
    }

    // --------------------------------------------------------------------------------------------------------------

    static double[] SystemSolve(double[][] A, double[] b)
    {
      // Solve Ax = b
      int n = A.Length;

      // 1. decompose A
      int[] perm;
      int toggle;
      double[][] luMatrix = MatrixDecompose(A, out perm, out toggle);
      if (luMatrix == null)
        return null;

      // 2. permute b according to perm[] into bp
      double[] bp = new double[b.Length];
      for (int i = 0; i < n; ++i)
        bp[i] = b[perm[i]];

      // 3. call helper
      double[] x = HelperSolve(luMatrix, bp);
      return x;
    } // SystemSolve

    // --------------------------------------------------------------------------------------------------------------

    static double[][] MatrixDuplicate(double[][] matrix)
    {
      // allocates/creates a duplicate of a matrix. assumes matrix is not null.
      double[][] result = MatrixCreate(matrix.Length, matrix[0].Length);
      for (int i = 0; i < matrix.Length; ++i) // copy the values
        for (int j = 0; j < matrix[i].Length; ++j)
          result[i][j] = matrix[i][j];
      return result;
    }

    // --------------------------------------------------------------------------------------------------------------

    static double[][] ExtractLower(double[][] matrix)
    {
      // lower part of a Doolittle decomposition (1.0s on diagonal, 0.0s in upper)
      int rows = matrix.Length; int cols = matrix[0].Length;
      double[][] result = MatrixCreate(rows, cols);
      for (int i = 0; i < rows; ++i)
      {
        for (int j = 0; j < cols; ++j)
        {
          if (i == j)
            result[i][j] = 1.0;
          else if (i > j)
            result[i][j] = matrix[i][j];
        }
      }
      return result;
    }

    static double[][] ExtractUpper(double[][] matrix)
    {
      // upper part of a Doolittle decomposition (0.0s in the strictly lower part)
      int rows = matrix.Length; int cols = matrix[0].Length;
      double[][] result = MatrixCreate(rows, cols);
      for (int i = 0; i < rows; ++i)
      {
        for (int j = 0; j < cols; ++j)
        {
          if (i <= j)
            result[i][j] = matrix[i][j];
        }
      }
      return result;
    }

    // --------------------------------------------------------------------------------------------------------------

    static double[][] PermArrayToMatrix(int[] perm)
    {
      // convert Doolittle perm array to corresponding perm matrix
      int n = perm.Length;
      double[][] result = MatrixCreate(n, n);
      for (int i = 0; i < n; ++i)
        result[i][perm[i]] = 1.0;
      return result;
    }

    static double[][] UnPermute(double[][] luProduct, int[] perm)
    {
      // unpermute product of Doolittle lower * upper matrix according to perm[]
      // no real use except to demo LU decomposition, or for consistency testing
      double[][] result = MatrixDuplicate(luProduct);

      int[] unperm = new int[perm.Length];
      for (int i = 0; i < perm.Length; ++i)
        unperm[perm[i]] = i;

      for (int r = 0; r < luProduct.Length; ++r)
        result[r] = luProduct[unperm[r]];
 
      return result;
    } // UnPermute

    // --------------------------------------------------------------------------------------------------------------

    static string VectorAsString(double[] vector)
    {
      string s = "";
      for (int i = 0; i < vector.Length; ++i)
        s += vector[i].ToString("F3").PadLeft(8) + Environment.NewLine;
      s += Environment.NewLine;
      return s;
    }

    static string VectorAsString(int[] vector)
    {
      string s = "";
      for (int i = 0; i < vector.Length; ++i)
        s += vector[i].ToString().PadLeft(2) + " ";
      s += Environment.NewLine;
      return s;
    }

    // --------------------------------------------------------------------------------------------------------------

  } // class MatrixDecompositionProgram

} // ns
