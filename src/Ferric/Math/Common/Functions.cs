using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Math.Common
{
    public static class Functions
    {

        #region Beta Function

        public static double LogBeta(double a, double b)
        {
            double result = (LogGamma(a) + LogGamma(b)) - LogGamma(a + b);
            return result;
        }

        #endregion

        #region Gamma Function

        public static double LogGamma(double x)
        {
            // A & S eq. 6.1.48 (continuing fraction)
            const double a0 = 1.0 / 12;
            const double a1 = 1.0 / 30;
            const double a2 = 53.0 / 210;
            const double a3 = 195.0 / 371;
            const double a4 = 22999.0 / 22737;
            const double a5 = 29944523.0 / 19733142;
            const double a6 = 109535241009.0 / 48264275462;

            double t6 = a6 / x;
            double t5 = a5 / (x + t6);
            double t4 = a4 / (x + t5);
            double t3 = a3 / (x + t4);
            double t2 = a2 / (x + t3);
            double t1 = a1 / (x + t2);
            double t0 = a0 / (x + t1);

            double result = t0 - x 
                + ((x - 0.5) * System.Math.Log(x)) 
                + (0.5 * System.Math.Log(2 * System.Math.PI));

            return result;
        }

        #endregion

    }
}
