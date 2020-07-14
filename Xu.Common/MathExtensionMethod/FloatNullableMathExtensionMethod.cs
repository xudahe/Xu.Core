namespace Xu.Common
{
    /// <summary>
    /// 可空Float运算
    /// </summary>
    public static class FloatNullableMathExtensionMethod
    {
        /// <summary>
        /// 加
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <returns></returns>
        public static float? Add(this float? f1, float? f2)
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
        public static float? Sub(this float? f1, float? f2)
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
        public static float? Mul(this float? f1, float? f2)
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
        public static float? Div(this float? f1, float? f2)
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