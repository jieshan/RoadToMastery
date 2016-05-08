using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadToMastery.Analysis
{
    public class Utils
    {
        /// <summary>
        /// Fits a line to a collection of (x,y) points.
        /// </summary>
        /// <param name="xVals">The x-axis values.</param>
        /// <param name="yVals">The y-axis values.</param>
        /// <param name="inclusiveStart">The inclusive inclusiveStart index.</param>
        /// <param name="exclusiveEnd">The exclusive exclusiveEnd index.</param>
        /// <param name="rsquared">The r^2 value of the line.</param>
        /// <param name="yintercept">The y-intercept value of the line (i.e. y = ax + b, yintercept is b).</param>
        /// <param name="slope">The slop of the line (i.e. y = ax + b, slope is a).</param>
        public static void LinearRegression(double[] xVals, double[] yVals,
                                            int inclusiveStart, int exclusiveEnd,
                                            out double rsquared, out double yintercept,
                                            out double slope)
        {
            Debug.Assert(xVals.Length == yVals.Length);
            double sumOfX = 0;
            double sumOfY = 0;
            double sumOfXSq = 0;
            double sumOfYSq = 0;
            double ssX = 0;
            double ssY = 0;
            double sumCodeviates = 0;
            double sCo = 0;
            double count = exclusiveEnd - inclusiveStart;

            for (int ctr = inclusiveStart; ctr < exclusiveEnd; ctr++)
            {
                double x = xVals[ctr];
                double y = yVals[ctr];
                sumCodeviates += x * y;
                sumOfX += x;
                sumOfY += y;
                sumOfXSq += x * x;
                sumOfYSq += y * y;
            }
            ssX = sumOfXSq - ((sumOfX * sumOfX) / count);
            ssY = sumOfYSq - ((sumOfY * sumOfY) / count);
            double RNumerator = (count * sumCodeviates) - (sumOfX * sumOfY);
            double RDenom = (count * sumOfXSq - (sumOfX * sumOfX))
             * (count * sumOfYSq - (sumOfY * sumOfY));
            sCo = sumCodeviates - ((sumOfX * sumOfY) / count);

            double meanX = sumOfX / count;
            double meanY = sumOfY / count;
            double dblR = RNumerator / Math.Sqrt(RDenom);
            rsquared = dblR * dblR;
            yintercept = meanY - ((sCo / ssX) * meanX);
            slope = sCo / ssX;
        }

        static void ShowVector(double[] v, int dec)
        {
            for (int i = 0; i < v.Length; ++i)
                Console.Write(v[i].ToString("F" + dec) + " ");
            Console.WriteLine("");
        }

        public static double TTest(double[] x, double[] y)
        {
            // Welch's t-test for two samples
            double sumX = 0.0; // calculate means
            double sumY = 0.0;

            for (int i = 0; i < x.Length; ++i)
                sumX += x[i];

            for (int i = 0; i < y.Length; ++i)
                sumY += y[i];

            int n1 = x.Length;
            int n2 = y.Length;
            double meanX = sumX / n1;
            double meanY = sumY / n2;

            double sumXminusMeanSquared = 0.0; // calculate (sample) variances
            double sumYminusMeanSquared = 0.0;

            for (int i = 0; i < n1; ++i)
                sumXminusMeanSquared += (x[i] - meanX) * (x[i] - meanX);

            for (int i = 0; i < n2; ++i)
                sumYminusMeanSquared += (y[i] - meanY) * (y[i] - meanY);

            double varX = sumXminusMeanSquared / (n1 - 1);
            double varY = sumYminusMeanSquared / (n2 - 1);

            // t statistic
            double top = (meanX - meanY);
            double bot = Math.Sqrt((varX / n1) + (varY / n2));
            double t = top / bot;


            // df mildly tricky
            // http://www.statsdirect.com/help/default.htm#parametric_methods/unpaired_t.htm
            double num = ((varX / n1) + (varY / n2)) *
              ((varX / n1) + (varY / n2));
            double denomLeft = ((varX / n1) * (varX / n1)) / (n1 - 1);
            double denomRight = ((varY / n2) * (varY / n2)) / (n2 - 1);
            double denom = denomLeft + denomRight;
            double df = num / denom;

            double p = Student(t, df); // cumulative two-tail density

            //Console.WriteLine("mean of x = " + meanX.ToString("F2"));
            //Console.WriteLine("mean of y = " + meanY.ToString("F2"));
            //Console.WriteLine("t = " + t.ToString("F4"));
            //Console.WriteLine("df = " + df.ToString("F3"));
            //Console.WriteLine("p-value = " + p.ToString("F5"));

            //Explain();

            return p;
        }

        public static void Explain()
        {
            Console.Write("\nThe p-value is the probability that ");
            Console.WriteLine("the two means are equal.\n");
            Console.Write("If the p-value is low, typically less ");
            Console.WriteLine("than 0.05 or 0.01, then you");
            Console.Write("conclude there is statistcal evidence ");
            Console.WriteLine("the two means are different.");
            Console.Write("But if the p-value isn't below the 5% ");
            Console.WriteLine("or 1% critical value then you");
            Console.Write("conclude there's not enough evidence to ");
            Console.WriteLine("say the means are different.\n");
        }

        public static double Student(double t, double df)
        {
            // for large integer df or double df
            // adapted from ACM algorithm 395
            // returns 2-tail p-value
            double n = df; // to sync with ACM parameter name
            double a, b, y;

            t = t * t;
            y = t / n;
            b = y + 1.0;
            if (y > 1.0E-6) y = Math.Log(b);
            a = n - 0.5;
            b = 48.0 * a * a;
            y = a * y;

            y = (((((-0.4 * y - 3.3) * y - 24.0) * y - 85.5) /
              (0.8 * y * y + 100.0 + b) + y + 3.0) / b + 1.0) *
              Math.Sqrt(y);
            return 2.0 * Gauss(-y); // ACM algorithm 209
        }

        public static double Student(double t, int df)
        {
            // adapted from ACM algorithm 395
            // for small integer df, or int df with large t
            int n = df; // to sync with ACM parameter name
            double a, b, y, z;

            z = 1.0;
            t = t * t;
            y = t / n;
            b = 1.0 + y;

            if (n >= 20 && t < n || n > 200) // moderate df and smallish t  ( df >= 20, t < df ) or very large df
            {
                double x = 1.0 * n; // make df a double
                return Student(t, x); // call double df version
            }

            if (n < 20 && t < 4.0)
            {
                a = Math.Sqrt(y);
                y = Math.Sqrt(y);
                if (n == 1)
                    a = 0.0;
            }
            else
            {
                a = Math.Sqrt(b);
                y = a * n;
                for (int j = 2; a != z; j += 2)
                {
                    z = a;
                    y = y * (j - 1) / (b * j);
                    a = a + y / (n + j);
                }
                n = n + 2;
                z = y = 0.0;
                a = -a;
            }

            int sanityCt = 0;
            while (true && sanityCt < 10000)
            {
                ++sanityCt;
                n = n - 2;
                if (n > 1)
                {
                    a = (n - 1) / (b * n) * a + y;
                    continue;
                }

                if (n == 0)
                    a = a / Math.Sqrt(b);
                else // n == 1
                    a = (Math.Atan(y) + a / b) * 0.63661977236; // 2/Pi

                return z - a;
            }

            return -1.0; // error
        } // Student (int df)



        // ==========================================================

        //static double Gauss(double z)
        //{
        //  // aka Phi(z)
        //  // Abramowitz and Stegun eq. 7.1.26
        //  double p = 0.3275911;
        //  double a1 = 0.254829592;
        //  double a2 = -0.284496736;
        //  double a3 = 1.421413741;
        //  double a4 = -1.453152027;
        //  double a5 = 1.061405429;

        //  int sign;
        //  if (z < 0.0)
        //    sign = -1;
        //  else
        //    sign = 1;

        //  double x = Math.Abs(z) / Math.Sqrt(2.0);
        //  double t = 1.0 / (1.0 + p * x);
        //  double erf = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Math.Exp(-x * x);
        //  return 0.5 * (1.0 + (sign * erf));
        //}

        public static double Gauss(double z)
        {
            // input = z-value (-inf to +inf)
            // output = p under Standard Normal curve from -inf to z
            // e.g., if z = 0.0, function returns 0.5000
            // ACM Algorithm #209
            double y; // 209 scratch variable
            double p; // result. called 'z' in 209
            double w; // 209 scratch variable

            if (z == 0.0)
                p = 0.0;
            else
            {
                y = Math.Abs(z) / 2;
                if (y >= 3.0)
                {
                    p = 1.0;
                }
                else if (y < 1.0)
                {
                    w = y * y;
                    p = ((((((((0.000124818987 * w
                      - 0.001075204047) * w + 0.005198775019) * w
                      - 0.019198292004) * w + 0.059054035642) * w
                      - 0.151968751364) * w + 0.319152932694) * w
                      - 0.531923007300) * w + 0.797884560593) * y * 2.0;
                }
                else
                {
                    y = y - 2.0;
                    p = (((((((((((((-0.000045255659 * y
                      + 0.000152529290) * y - 0.000019538132) * y
                      - 0.000676904986) * y + 0.001390604284) * y
                      - 0.000794620820) * y - 0.002034254874) * y
                      + 0.006549791214) * y - 0.010557625006) * y
                      + 0.011630447319) * y - 0.009279453341) * y
                      + 0.005353579108) * y - 0.002141268741) * y
                      + 0.000535310849) * y + 0.999936657524;
                }
            }

            if (z > 0.0)
                return (p + 1.0) / 2;
            else
                return (1.0 - p) / 2;
        }
    }
}
