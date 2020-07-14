namespace Xu.Common
{
    /// <summary>
    /// 可空Int32运算
    /// </summary>
    public static class Int32NullableMathExtensionMethod
    {
        /// <summary>
        /// 加
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <returns></returns>
        public static int? Add(this int? i1, int? i2)
        {
            if (i1.HasValue)
            {
                if (i2.HasValue)
                    return i1.Value + i2.Value;
                else
                    return i1.Value;
            }
            else
            {
                if (i2.HasValue)
                    return i2.Value;
                else
                    return null;
            }
        }

        /// <summary>
        /// 减
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <returns></returns>
        public static int? Sub(this int? i1, int? i2)
        {
            if (i1.HasValue)
            {
                if (i2.HasValue)
                    return i1.Value - i2.Value;
                else
                    return i1.Value;
            }
            else
            {
                if (i2.HasValue)
                    return -i2.Value;
                else
                    return null;
            }
        }

        /// <summary>
        /// 乘
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <returns></returns>
        public static int? Mul(this int? i1, int? i2)
        {
            if (i1.HasValue && i2.HasValue)
                return i1.Value * i2.Value;
            else
                return null;
        }

        /// <summary>
        /// 除
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <returns></returns>
        public static int? Div(this int? i1, int? i2)
        {
            if (i1.HasValue && i2.HasValue)
            {
                if (i2.Value == 0)
                    return null;
                else
                    return i1.Value / i2.Value;
            }
            else
                return null;
        }
    }
}