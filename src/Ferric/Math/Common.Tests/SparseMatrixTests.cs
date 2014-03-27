using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ferric.Math.Common.Tests
{
    [TestClass]
    public class SparseMatrixTests
    {
        [TestMethod]
        public void Math_Common_SparseMatrix_Equality()
        {
            var a = new SparseMatrix<int>(new int[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } });
            var b = new SparseMatrix<int>(new int[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } });
            Assert.AreEqual(a, b);

            var c = new SparseMatrix<int>(new int[1, 6] { { 1, 2, 3, 4, 5, 6 } });
            Assert.AreNotEqual(a, c);

            var d = new SparseMatrix<double>(new double[2, 3] { { 1.0, 2.0, 3.0 }, { 4.0, 5.0, 6.0 } });
            Assert.AreNotEqual(a, d);

            var e = new SparseMatrix<double>(new double[2, 3] { { 1.001, 2.001, 3.001 }, { 4.001, 5.001, 6.001 } });
            Assert.IsTrue(e.Equals(e, 0.01));
        }

        [TestMethod]
        public void Math_Common_SparseMatrix_Transpose()
        {
            var a = new SparseMatrix<int>(new int[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } });
            var b = new SparseMatrix<int>(new int[3, 2] { { 1, 4 }, { 2, 5 }, { 3, 6 } });

            var t = a.Transpose();
            Assert.AreEqual(b, t);
        }

        [TestMethod]
        public void Math_Common_SparseMatrix_ScalarMult_Int()
        {
            var a = new SparseMatrix<int>(new int[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } });
            var p = a * 2;

            var b = new SparseMatrix<int>(new int[2, 3] { { 2, 4, 6 }, { 8, 10, 12 } });
            Assert.AreEqual(b, p);
        }

        [TestMethod]
        public void Math_Common_SparseMatrix_ScalarMult_Double()
        {
            var a = new SparseMatrix<double>(new double[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } });
            var p = a * 2.0;

            var b = new SparseMatrix<double>(new double[2, 3] { { 2, 4, 6 }, { 8, 10, 12 } });
            Assert.AreEqual(b, p);
        }

        [TestMethod]
        public void Math_Common_SparseMatrix_ScalarMult_Generic()
        {
            var a = new SparseMatrix<decimal>(new decimal[2, 3] { { 1, 0, 3 }, { 0, 5, 0 } });
            var p = a * 2.0m;

            var b = new SparseMatrix<decimal>(new decimal[2, 3] { { 2, 0, 6 }, { 0, 10, 0 } });
            Assert.AreEqual(b, p);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Math_Common_SparseMatrix_Addition_Dimensions()
        {
            var a = new SparseMatrix<int>(new int[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } });
            var b = new SparseMatrix<int>(new int[1, 2] { { 1, 2 } });
            var sum = a + b;
        }

        [TestMethod]
        public void Math_Common_SparseMatrix_Addition_Int()
        {
            var a = new SparseMatrix<int>(new int[2, 3] { { 1, 0, 3 }, { 4, 0, 6 } });
            var b = new SparseMatrix<int>(new int[2, 3] { { 0, 0, 9 }, { 0, 0, 12 } });

            var sum = a + b;

            var c = new SparseMatrix<int>(new int[2, 3] { { 1, 0, 12 }, { 4, 0, 18 } });
            Assert.AreEqual(c, sum);
        }

        [TestMethod]
        public void Math_Common_SparseMatrix_Addition_Double()
        {
            var a = new SparseMatrix<double>(new double[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } });
            var b = new SparseMatrix<double>(new double[2, 3] { { 7, 8, 9 }, { 10, 11, 12 } });

            var sum = a + b;

            var c = new SparseMatrix<double>(new double[2, 3] { { 8, 10, 12 }, { 14, 16, 18 } });
            Assert.AreEqual(c, sum);
        }

        [TestMethod]
        public void Math_Common_SparseMatrix_Addition_Generic()
        {
            var a = new SparseMatrix<decimal>(new decimal[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } });
            var b = new SparseMatrix<decimal>(new decimal[2, 3] { { 7, 8, 9 }, { 10, 11, 12 } });

            var sum = a + b;

            var c = new SparseMatrix<decimal>(new decimal[2, 3] { { 8, 10, 12 }, { 14, 16, 18 } });
            Assert.AreEqual(c, sum);
        }

        [TestMethod]
        public void Math_Common_SparseMatrix_Negation_Int()
        {
            var a = new SparseMatrix<int>(new int[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } });
            var neg = -a;

            var c = new SparseMatrix<int>(new int[2, 3] { { -1, -2, -3 }, { -4, -5, -6 } });
            Assert.AreEqual(c, neg);
        }

        [TestMethod]
        public void Math_Common_SparseMatrix_Negation_Double()
        {
            var a = new SparseMatrix<double>(new double[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } });
            var neg = -a;

            var c = new SparseMatrix<double>(new double[2, 3] { { -1, -2, -3 }, { -4, -5, -6 } });
            Assert.AreEqual(c, neg);
        }

        [TestMethod]
        public void Math_Common_SparseMatrix_Negation_Generic()
        {
            var a = new SparseMatrix<decimal>(new decimal[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } });
            var neg = -a;

            var c = new SparseMatrix<decimal>(new decimal[2, 3] { { -1, -2, -3 }, { -4, -5, -6 } });
            Assert.AreEqual(c, neg);
        }

        [TestMethod]
        public void Math_Common_SparseMatrix_Subtraction_Int()
        {
            var a = new SparseMatrix<int>(new int[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } });
            var b = new SparseMatrix<int>(new int[2, 3] { { 6, 5, 4 }, { 3, 2, 1 } });
            var diff = a - b;

            var c = new SparseMatrix<int>(new int[2, 3] { { -5, -3, -1 }, { 1, 3, 5 } });
            Assert.AreEqual(c, diff);
        }

        [TestMethod]
        public void Math_Common_SparseMatrix_Subtraction_Double()
        {
            var a = new SparseMatrix<double>(new double[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } });
            var b = new SparseMatrix<double>(new double[2, 3] { { 6, 5, 4 }, { 3, 2, 1 } });
            var diff = a - b;

            var c = new SparseMatrix<double>(new double[2, 3] { { -5, -3, -1 }, { 1, 3, 5 } });
            Assert.AreEqual(c, diff);
        }

        [TestMethod]
        public void Math_Common_SparseMatrix_Subtraction_Generic()
        {
            var a = new SparseMatrix<decimal>(new decimal[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } });
            var b = new SparseMatrix<decimal>(new decimal[2, 3] { { 6, 5, 4 }, { 3, 2, 1 } });
            var diff = a - b;

            var c = new SparseMatrix<decimal>(new decimal[2, 3] { { -5, -3, -1 }, { 1, 3, 5 } });
            Assert.AreEqual(c, diff);
        }

        [TestMethod]
        public void Math_Common_SparseMatrix_Multiplication_Double()
        {
            var a = new SparseMatrix<double>(new double[4, 2] { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } });
            var b = new SparseMatrix<double>(new double[2, 3] { { 9, 10, 11 }, { 12, 13, 14 } });
            var prod = a * b;

            var c = new SparseMatrix<double>(new double[4, 3] {
                { 33, 36, 39 },
                { 75, 82, 89 },
                { 117, 128, 139 },
                { 159, 174, 189 }
            });
            Assert.AreEqual(c, prod);
        }

        [TestMethod]
        public void Math_Common_SparseMatrix_Multiplication_Int()
        {
            var a = new SparseMatrix<int>(new int[4, 2] { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } });
            var b = new SparseMatrix<int>(new int[2, 3] { { 9, 10, 11 }, { 12, 13, 14 } });
            var prod = a * b;

            var c = new SparseMatrix<int>(new int[4, 3] {
                { 33, 36, 39 },
                { 75, 82, 89 },
                { 117, 128, 139 },
                { 159, 174, 189 }
            });
            Assert.AreEqual(c, prod);
        }

        [TestMethod]
        public void Math_Common_SparseMatrix_Multiplication_Generic()
        {
            var a = new SparseMatrix<decimal>(new decimal[4, 2] { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } });
            var b = new SparseMatrix<decimal>(new decimal[2, 3] { { 9, 10, 11 }, { 12, 13, 14 } });
            var prod = a * b;

            var c = new SparseMatrix<decimal>(new decimal[4, 3] {
                { 33, 36, 39 },
                { 75, 82, 89 },
                { 117, 128, 139 },
                { 159, 174, 189 }
            });
            Assert.AreEqual(c, prod);
        }

        [TestMethod]
        public void Math_Common_SparseMatrix_Inverse()
        {
            var a = new SparseMatrix<double>(new double[2, 2] { { 4, 3 }, { 3, 2 } });
            var ai = a.Inverse();

            var c = new SparseMatrix<double>(new double[2, 2] { { -2, 3 }, { 3, -4 } });
            Assert.AreEqual(c, ai);

            var id = new SparseMatrix<double>(new double[2, 2] { { 1, 0 }, { 0, 1 } });
            var d = a * ai;
            Assert.AreEqual(id, d);

            var e = ai * a;
            Assert.AreEqual(id, e);
        }
    }
}
