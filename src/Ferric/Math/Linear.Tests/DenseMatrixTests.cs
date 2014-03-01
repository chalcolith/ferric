using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ferric.Math.Linear.Tests
{
    [TestClass]
    public class DenseMatrixTests
    {
        [TestMethod]
        public void Math_Linear_DenseMatrix_Equality()
        {
            var a = new DenseMatrix<int>(2, 3, new int[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } });
            var b = new DenseMatrix<int>(2, 3, new int[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } });
            Assert.AreEqual(a, b);

            var c = new DenseMatrix<int>(1, 6, new int[1, 6] { { 1, 2, 3, 4, 5, 6 } });
            Assert.AreNotEqual(a, c);

            var d = new DenseMatrix<double>(2, 3, new double[2, 3] { {1.0, 2.0, 3.0}, {4.0, 5.0, 6.0}});
            Assert.AreNotEqual(a, d);

            var e = new DenseMatrix<double>(2, 3, new double[2, 3] { { 1.001, 2.001, 3.001 }, { 4.001, 5.001, 6.001 } });
            Assert.IsTrue(e.Equals(e, 0.01));
        }

        [TestMethod]
        public void Math_Linear_DenseMatrix_Transpose()
        {
            var a = new DenseMatrix<int>(2, 3, new int[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } });
            var b = new DenseMatrix<int>(3, 2, new int[3, 2] { { 1, 4 }, { 2, 5 }, { 3, 6 } });

            var t = a.Transpose();
            Assert.AreEqual(b, t);
        }

        [TestMethod]
        public void Math_Linear_DenseMatrix_ScalarMult_Int()
        {
            var a = new DenseMatrix<int>(2, 3, new int[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } });
            var p = a * 2;

            var b = new DenseMatrix<int>(2, 3, new int[2, 3] { { 2, 4, 6 }, { 8, 10, 12 } });
            Assert.AreEqual(b, p);
        }

        [TestMethod]
        public void Math_Linear_DenseMatrix_ScalarMult_Double()
        {
            var a = new DenseMatrix<double>(2, 3, new double[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } });
            var p = a * 2.0;

            var b = new DenseMatrix<double>(2, 3, new double[2, 3] { { 2, 4, 6 }, { 8, 10, 12 } });
            Assert.AreEqual(b, p);
        }

        [TestMethod]
        public void Math_Linear_DenseMatrix_ScalarMult_Generic()
        {
            var a = new DenseMatrix<decimal>(2, 3, new decimal[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } });
            var p = a * 2.0m;

            var b = new DenseMatrix<decimal>(2, 3, new decimal[2, 3] { { 2, 4, 6 }, { 8, 10, 12 } });
            Assert.AreEqual(b, p);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Math_Linear_DenseMatrix_Addition_Dimensions()
        {
            var a = new DenseMatrix<int>(2, 3, new int[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } });
            var b = new DenseMatrix<int>(1, 2, new int[1, 2] { { 1, 2 } });
            var sum = a + b;
        }

        [TestMethod]
        public void Math_Linear_DenseMatrix_Addition_Int()
        {
            var a = new DenseMatrix<int>(2, 3, new int[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } });
            var b = new DenseMatrix<int>(2, 3, new int[2, 3] { { 7, 8, 9 }, { 10, 11, 12 } });

            var sum = a + b;

            var c = new DenseMatrix<int>(2, 3, new int[2, 3] { { 8, 10, 12 }, { 14, 16, 18 } });
            Assert.AreEqual(c, sum);
        }

        [TestMethod]
        public void Math_Linear_DenseMatrix_Addition_Double()
        {
            var a = new DenseMatrix<double>(2, 3, new double[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } });
            var b = new DenseMatrix<double>(2, 3, new double[2, 3] { { 7, 8, 9 }, { 10, 11, 12 } });

            var sum = a + b;

            var c = new DenseMatrix<double>(2, 3, new double[2, 3] { { 8, 10, 12 }, { 14, 16, 18 } });
            Assert.AreEqual(c, sum);
        }

        [TestMethod]
        public void Math_Linear_DenseMatrix_Addition_Generic()
        {
            var a = new DenseMatrix<decimal>(2, 3, new decimal[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } });
            var b = new DenseMatrix<decimal>(2, 3, new decimal[2, 3] { { 7, 8, 9 }, { 10, 11, 12 } });

            var sum = a + b;

            var c = new DenseMatrix<decimal>(2, 3, new decimal[2, 3] { { 8, 10, 12 }, { 14, 16, 18 } });
            Assert.AreEqual(c, sum);
        }

        [TestMethod]
        public void Math_Linear_DenseMatrix_Negation_Int()
        {
            var a = new DenseMatrix<int>(2, 3, new int[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } });
            var neg = -a;

            var c = new DenseMatrix<int>(2, 3, new int[2, 3] { { -1, -2, -3 }, { -4, -5, -6 } });
            Assert.AreEqual(c, neg);
        }

        [TestMethod]
        public void Math_Linear_DenseMatrix_Negation_Double()
        {
            var a = new DenseMatrix<double>(2, 3, new double[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } });
            var neg = -a;

            var c = new DenseMatrix<double>(2, 3, new double[2, 3] { { -1, -2, -3 }, { -4, -5, -6 } });
            Assert.AreEqual(c, neg);
        }

        [TestMethod]
        public void Math_Linear_DenseMatrix_Negation_Generic()
        {
            var a = new DenseMatrix<decimal>(2, 3, new decimal[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } });
            var neg = -a;

            var c = new DenseMatrix<decimal>(2, 3, new decimal[2, 3] { { -1, -2, -3 }, { -4, -5, -6 } });
            Assert.AreEqual(c, neg);
        }

        [TestMethod]
        public void Math_Linear_DenseMatrix_Subtraction_Int()
        {
            var a = new DenseMatrix<int>(2, 3, new int[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } });
            var b = new DenseMatrix<int>(2, 3, new int[2, 3] { { 6, 5, 4 }, { 3, 2, 1 } });
            var diff = a - b;

            var c = new DenseMatrix<int>(2, 3, new int[2, 3] { { -5, -3, -1 }, { 1, 3, 5 } });
            Assert.AreEqual(c, diff);
        }

        [TestMethod]
        public void Math_Linear_DenseMatrix_Subtraction_Double()
        {
            var a = new DenseMatrix<double>(2, 3, new double[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } });
            var b = new DenseMatrix<double>(2, 3, new double[2, 3] { { 6, 5, 4 }, { 3, 2, 1 } });
            var diff = a - b;

            var c = new DenseMatrix<double>(2, 3, new double[2, 3] { { -5, -3, -1 }, { 1, 3, 5 } });
            Assert.AreEqual(c, diff);
        }

        [TestMethod]
        public void Math_Linear_DenseMatrix_Subtraction_Generic()
        {
            var a = new DenseMatrix<decimal>(2, 3, new decimal[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } });
            var b = new DenseMatrix<decimal>(2, 3, new decimal[2, 3] { { 6, 5, 4 }, { 3, 2, 1 } });
            var diff = a - b;

            var c = new DenseMatrix<decimal>(2, 3, new decimal[2, 3] { { -5, -3, -1 }, { 1, 3, 5 } });
            Assert.AreEqual(c, diff);
        }

        [TestMethod]
        public void Math_Linear_DenseMatrix_Multiplication_Int()
        {
        }
    }
}
