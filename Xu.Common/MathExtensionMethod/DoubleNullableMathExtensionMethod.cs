namespace Xu.Common
{
    /// <summary>
    /// 可空Double运算
    /// </summary>
    public static class DoubleNullableMathExtensionMethod
    {
        /// <summary>
        /// 加
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <returns></returns>
        public static double? Add(this double? f1, double? f2)
        {
            if (f1.HasValue)
            {
                if (f2.HasValue)
                    return f1.Value + f2.Value;
                else
                    return f1.Value;
            }
            else
            {
                if (f2.HasValue)
                    return f2.Value;
                else
                    return null;
            }
        }

        /// <summary>
        /// 减
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <returns></returns>
        public static double? Sub(this double? f1, double? f2)
        {
            if (f1.HasValue)
            {
                if (f2.HasValue)
                    return f1.Value - f2.Value;
                else
                    return f1.Value;
            }
            else
            {
                if (f2.HasValue)
                    return -f2.Value;
                else
                    return null;
            }
        }

        /// <summary>
        /// 乘
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <returns></returns>
        public static double? Mul(this double? f1, double? f2)
        {
            if (f1.HasValue && f2.HasValue)
                return f1.Value * f2.Value;
            else
                return null;
        }

        /// <summary>
        /// 除
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <returns></returns>
        public static double? Div(this double? f1, double? f2)
        {
            if (f1.HasValue && f2.HasValue)
            {
                if (f2.Value == 0)
                    return null;
                else
                    return f1.Value / f2.Value;
            }
            else
                return null;
        }
    }
}