﻿#pragma warning disable RCS1123 // Add parentheses according to operator precedence.
#pragma warning disable RCS1138 // Add summary to documentation comment.

using System;

namespace primus.app
{
    public static class Easings
    {
        /// <summary>
        /// Constant Pi.
        /// </summary>
        private const double Pi = Math.PI;

        /// <summary>
        /// Constant Pi / 2.
        /// </summary>
        private const double Halfpi = Math.PI / 2.0f;

        /// <summary>
        /// Easing Functions enumeration
        /// </summary>
        public enum Functions
        {
            Linear,
            QuadraticEaseIn,
            QuadraticEaseOut,
            QuadraticEaseInOut,
            CubicEaseIn,
            CubicEaseOut,
            CubicEaseInOut,
            QuarticEaseIn,
            QuarticEaseOut,
            QuarticEaseInOut,
            QuinticEaseIn,
            QuinticEaseOut,
            QuinticEaseInOut,
            SineEaseIn,
            SineEaseOut,
            SineEaseInOut,
            CircularEaseIn,
            CircularEaseOut,
            CircularEaseInOut,
            ExponentialEaseIn,
            ExponentialEaseOut,
            ExponentialEaseInOut,
            ElasticEaseIn,
            ElasticEaseOut,
            ElasticEaseInOut,
            BackEaseIn,
            BackEaseOut,
            BackEaseInOut,
            BounceEaseIn,
            BounceEaseOut,
            BounceEaseInOut
        }

        /// <summary>
        /// Interpolate using the specified function.
        /// </summary>
        public static double Interpolate(double p, Functions function)
        {
            switch (function)
            {
                case Functions.Linear: return Linear(p);
                case Functions.QuadraticEaseOut: return QuadraticEaseOut(p);
                case Functions.QuadraticEaseIn: return QuadraticEaseIn(p);
                case Functions.QuadraticEaseInOut: return QuadraticEaseInOut(p);
                case Functions.CubicEaseIn: return CubicEaseIn(p);
                case Functions.CubicEaseOut: return CubicEaseOut(p);
                case Functions.CubicEaseInOut: return CubicEaseInOut(p);
                case Functions.QuarticEaseIn: return QuarticEaseIn(p);
                case Functions.QuarticEaseOut: return QuarticEaseOut(p);
                case Functions.QuarticEaseInOut: return QuarticEaseInOut(p);
                case Functions.QuinticEaseIn: return QuinticEaseIn(p);
                case Functions.QuinticEaseOut: return QuinticEaseOut(p);
                case Functions.QuinticEaseInOut: return QuinticEaseInOut(p);
                case Functions.SineEaseIn: return SineEaseIn(p);
                case Functions.SineEaseOut: return SineEaseOut(p);
                case Functions.SineEaseInOut: return SineEaseInOut(p);
                case Functions.CircularEaseIn: return CircularEaseIn(p);
                case Functions.CircularEaseOut: return CircularEaseOut(p);
                case Functions.CircularEaseInOut: return CircularEaseInOut(p);
                case Functions.ExponentialEaseIn: return ExponentialEaseIn(p);
                case Functions.ExponentialEaseOut: return ExponentialEaseOut(p);
                case Functions.ExponentialEaseInOut: return ExponentialEaseInOut(p);
                case Functions.ElasticEaseIn: return ElasticEaseIn(p);
                case Functions.ElasticEaseOut: return ElasticEaseOut(p);
                case Functions.ElasticEaseInOut: return ElasticEaseInOut(p);
                case Functions.BackEaseIn: return BackEaseIn(p);
                case Functions.BackEaseOut: return BackEaseOut(p);
                case Functions.BackEaseInOut: return BackEaseInOut(p);
                case Functions.BounceEaseIn: return BounceEaseIn(p);
                case Functions.BounceEaseOut: return BounceEaseOut(p);
                case Functions.BounceEaseInOut: return BounceEaseInOut(p);
                default: return Linear(p);
            }
        }

        /// <summary>
        /// Modeled after the line y = x
        /// </summary>
        public static double Linear(double p)
        {
            return p;
        }

        /// <summary>
        /// Modeled after the parabola y = x^2
        /// </summary>
        public static double QuadraticEaseIn(double p)
        {
            return p * p;
        }

        /// <summary>
        /// Modeled after the parabola y = -x^2 + 2x
        /// </summary>
        public static double QuadraticEaseOut(double p)
        {
            return -(p * (p - 2));
        }

        /// <summary>
        /// Modeled after the piecewise quadratic
        /// y = (1/2)((2x)^2)             ; [0, 0.5)
        /// y = -(1/2)((2x-1)*(2x-3) - 1) ; [0.5, 1]
        /// </summary>
        public static double QuadraticEaseInOut(double p)
        {
            if (p < 0.5f)
            {
                return 2 * p * p;
            }
            else
            {
                return (-2 * p * p) + (4 * p) - 1;
            }
        }

        /// <summary>
        /// Modeled after the cubic y = x^3
        /// </summary>
        public static double CubicEaseIn(double p)
        {
            return p * p * p;
        }

        /// <summary>
        /// Modeled after the cubic y = (x - 1)^3 + 1
        /// </summary>
        public static double CubicEaseOut(double p)
        {
            var f = (p - 1);
            return f * f * f + 1;
        }

        public static double RevCubicEaseInOut(double e)
        {
            if (e < 0.5f)
            {
                return Math.Pow(e / 4d, 1d / 3d);
            }
            else
            {
                return (Math.Pow(2e-2, 1d / 3d) + 2) / 2;
            }
        }

        /// <summary>
        /// Modeled after the piecewise cubic
        /// y = (1/2)((2x)^3)       ; [0, 0.5)
        /// y = (1/2)((2x-2)^3 + 2) ; [0.5, 1]
        /// </summary>
        public static double CubicEaseInOut(double p)
        {
            if (p < 0.5f)
            {
                return 4 * p * p * p;
            }
            else
            {
                var f = ((2 * p) - 2);
                return 0.5f * f * f * f + 1;
            }
        }

        /// <summary>
        /// Modeled after the quartic x^4
        /// </summary>
        public static double QuarticEaseIn(double p)
        {
            return p * p * p * p;
        }

        /// <summary>
        /// Modeled after the quartic y = 1 - (x - 1)^4
        /// </summary>
        public static double QuarticEaseOut(double p)
        {
            var f = (p - 1);
            return f * f * f * (1 - p) + 1;
        }

        /// <summary>
        /// Modeled after the piecewise quartic
        /// y = (1/2)((2x)^4)        ; [0, 0.5)
        /// y = -(1/2)((2x-2)^4 - 2) ; [0.5, 1]
        /// </summary>
        public static double QuarticEaseInOut(double p)
        {
            if (p < 0.5f)
            {
                return 8 * p * p * p * p;
            }
            else
            {
                var f = (p - 1);
                return -8 * f * f * f * f + 1;
            }
        }

        /// <summary>
        /// Modeled after the quintic y = x^5
        /// </summary>
        public static double QuinticEaseIn(double p)
        {
            return p * p * p * p * p;
        }

        /// <summary>
        /// Modeled after the quintic y = (x - 1)^5 + 1
        /// </summary>
        public static double QuinticEaseOut(double p)
        {
            var f = (p - 1);
            return f * f * f * f * f + 1;
        }

        /// <summary>
        /// Modeled after the piecewise quintic
        /// y = (1/2)((2x)^5)       ; [0, 0.5)
        /// y = (1/2)((2x-2)^5 + 2) ; [0.5, 1]
        /// </summary>
        public static double QuinticEaseInOut(double p)
        {
            if (p < 0.5f)
            {
                return 16 * p * p * p * p * p;
            }
            else
            {
                var f = ((2 * p) - 2);
                return 0.5f * f * f * f * f * f + 1;
            }
        }

        /// <summary>
        /// Modeled after quarter-cycle of sine wave
        /// </summary>
        public static double SineEaseIn(double p)
        {
            return Math.Sin((p - 1) * Halfpi) + 1;
        }

        /// <summary>
        /// Modeled after quarter-cycle of sine wave (different phase)
        /// </summary>
        public static double SineEaseOut(double p)
        {
            return Math.Sin(p * Halfpi);
        }

        /// <summary>
        /// Modeled after half sine wave
        /// </summary>
        public static double SineEaseInOut(double p)
        {
            return 0.5f * (1 - Math.Cos(p * Pi));
        }

        /// <summary>
        /// Modeled after shifted quadrant IV of unit circle
        /// </summary>
        public static double CircularEaseIn(double p)
        {
            return 1 - Math.Sqrt(1 - (p * p));
        }

        /// <summary>
        /// Modeled after shifted quadrant II of unit circle
        /// </summary>
        public static double CircularEaseOut(double p)
        {
            return Math.Sqrt((2 - p) * p);
        }

        /// <summary>
        /// Modeled after the piecewise circular function
        /// y = (1/2)(1 - Math.Sqrt(1 - 4x^2))           ; [0, 0.5)
        /// y = (1/2)(Math.Sqrt(-(2x - 3)*(2x - 1)) + 1) ; [0.5, 1]
        /// </summary>
        public static double CircularEaseInOut(double p)
        {
            if (p < 0.5f)
            {
                return 0.5f * (1 - Math.Sqrt(1 - 4 * (p * p)));
            }
            else
            {
                return 0.5f * (Math.Sqrt(-((2 * p) - 3) * ((2 * p) - 1)) + 1);
            }
        }

        /// <summary>
        /// Modeled after the exponential function y = 2^(10(x - 1))
        /// </summary>
        public static double ExponentialEaseIn(double p)
        {
            return (p == 0.0f) ? p : Math.Pow(2, 10 * (p - 1));
        }

        /// <summary>
        /// Modeled after the exponential function y = -2^(-10x) + 1
        /// </summary>
        public static double ExponentialEaseOut(double p)
        {
            return (p == 1.0f) ? p : 1 - Math.Pow(2, -10 * p);
        }

        /// <summary>
        /// Modeled after the piecewise exponential
        /// y = (1/2)2^(10(2x - 1))         ; [0,0.5)
        /// y = -(1/2)*2^(-10(2x - 1))) + 1 ; [0.5,1]
        /// </summary>
        public static double ExponentialEaseInOut(double p)
        {
            if (p == 0.0 || p == 1.0) return p;

            if (p < 0.5f)
            {
                return 0.5f * Math.Pow(2, (20 * p) - 10);
            }
            else
            {
                return -0.5f * Math.Pow(2, (-20 * p) + 10) + 1;
            }
        }

        /// <summary>
        /// Modeled after the damped sine wave y = sin(13pi/2*x)*Math.Pow(2, 10 * (x - 1))
        /// </summary>
        public static double ElasticEaseIn(double p)
        {
            return Math.Sin(13 * Halfpi * p) * Math.Pow(2, 10 * (p - 1));
        }

        /// <summary>
        /// Modeled after the damped sine wave y = sin(-13pi/2*(x + 1))*Math.Pow(2, -10x) + 1
        /// </summary>
        public static double ElasticEaseOut(double p)
        {
            return Math.Sin(-13 * Halfpi * (p + 1)) * Math.Pow(2, -10 * p) + 1;
        }

        /// <summary>
        /// Modeled after the piecewise exponentially-damped sine wave:
        /// y = (1/2)*sin(13pi/2*(2*x))*Math.Pow(2, 10 * ((2*x) - 1))      ; [0,0.5)
        /// y = (1/2)*(sin(-13pi/2*((2x-1)+1))*Math.Pow(2,-10(2*x-1)) + 2) ; [0.5, 1]
        /// </summary>
        public static double ElasticEaseInOut(double p)
        {
            if (p < 0.5f)
            {
                return 0.5f * Math.Sin(13 * Halfpi * (2 * p)) * Math.Pow(2, 10 * ((2 * p) - 1));
            }
            else
            {
                return 0.5f * (Math.Sin(-13 * Halfpi * ((2 * p - 1) + 1)) * Math.Pow(2, -10 * (2 * p - 1)) + 2);
            }
        }

        /// <summary>
        /// Modeled after the overshooting cubic y = x^3-x*sin(x*pi)
        /// </summary>
        public static double BackEaseIn(double p)
        {
            return p * p * p - p * Math.Sin(p * Pi);
        }

        /// <summary>
        /// Modeled after overshooting cubic y = 1-((1-x)^3-(1-x)*sin((1-x)*pi))
        /// </summary>
        public static double BackEaseOut(double p)
        {
            var f = (1 - p);
            return 1 - (f * f * f - f * Math.Sin(f * Pi));
        }

        /// <summary>
        /// Modeled after the piecewise overshooting cubic function:
        /// y = (1/2)*((2x)^3-(2x)*sin(2*x*pi))           ; [0, 0.5)
        /// y = (1/2)*(1-((1-x)^3-(1-x)*sin((1-x)*pi))+1) ; [0.5, 1]
        /// </summary>
        public static double BackEaseInOut(double p)
        {
            if (p < 0.5f)
            {
                var f = 2 * p;
                return 0.5f * (f * f * f - f * Math.Sin(f * Pi));
            }
            else
            {
                var f = (1 - (2 * p - 1));
                return 0.5f * (1 - (f * f * f - f * Math.Sin(f * Pi))) + 0.5f;
            }
        }

        /// <summary>
        /// </summary>
        public static double BounceEaseIn(double p)
        {
            return 1 - BounceEaseOut(1 - p);
        }

        /// <summary>
        /// </summary>
        public static double BounceEaseOut(double p)
        {
            if (p < 4 / 11.0f)
            {
                return (121 * p * p) / 16.0f;
            }
            else if (p < 8 / 11.0f)
            {
                return (363 / 40.0f * p * p) - (99 / 10.0f * p) + 17 / 5.0f;
            }
            else if (p < 9 / 10.0f)
            {
                return (4356 / 361.0f * p * p) - (35442 / 1805.0f * p) + 16061 / 1805.0f;
            }
            else
            {
                return (54 / 5.0f * p * p) - (513 / 25.0f * p) + 268 / 25.0f;
            }
        }

        /// <summary>
        /// </summary>
        public static double BounceEaseInOut(double p)
        {
            if (p < 0.5f)
            {
                return 0.5f * BounceEaseIn(p * 2);
            }
            else
            {
                return 0.5f * BounceEaseOut(p * 2 - 1) + 0.5f;
            }
        }
    }
}

#pragma warning restore RCS1138 // Add summary to documentation comment.
#pragma warning restore RCS1123 // Add parentheses according to operator precedence.
