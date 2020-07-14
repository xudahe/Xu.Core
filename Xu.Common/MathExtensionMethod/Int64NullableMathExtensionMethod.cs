using System;

namespace Xu.Common
{
    /// <summary>
    /// 可空Int64运算
    /// </summary>
    public static class Int64NullableMathExtensionMethod
    {
        /// <summary>
        /// 加
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <returns></returns>
        public static long? Add(this long? i1, long? i2)
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
        public static long? Sub(this long? i1, long? i2)
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
        public static long? Mul(this long? i1, long? i2)
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
        public static long? Div(this long? i1, long? i2)
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