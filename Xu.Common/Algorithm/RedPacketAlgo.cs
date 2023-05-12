using System;

namespace Xu.Common.Algorithm
{
    public class RedPacketAlgo
    {
        /// <summary>
        /// 抢红包算法
        /// </summary>
        /// <param name="totalMonney">总钱数，单位：元</param>
        /// <param name="totalPerson">总人数</param>
        /// <returns></returns>
        private static string RedPacket(decimal totalMonney, int totalPerson)
        {
            //1.需求
            //   把 x 元钱分给 y 个人，每个人获得钱数不等，有多的，也有少的。
            //　 PS： x元钱要精确到分，每个人获得到的钱也是精确到分。

            //2.实现思路
            //  (1).先把 x 元钱 乘以 100，转换成 分，然后除以 y 人得到一个平均值，把这个平均值赋值给 每个人。
            //  (2).查看一下步骤①中平均值是否除尽了，如果没有除尽，则把剩下的值随机赋值给一个人；如果除尽，则忽略不计。
            //  (3).继续执行随机，随机次数根据分钱的人数自行设计合理数值。

            //随机算法为：随机获取两个人A和B，对应的钱数分别为AA和BB，然后随机获取 （0，AA）一个钱数，用A减去它，用B加上它。

            int m = (int)(Convert.ToDecimal(totalMonney) * 100);
            //红包个数
            int[] bags = new int[totalPerson];
            int avg = m / totalPerson;
            for (int i = 0; i < totalPerson; i++)
            {
                bags[i] = avg;
            }
            Random random = new Random();
            //剩下的钱平均分给某个人
            int leftMoney = m - avg * totalPerson;
            bags[random.Next(0, totalPerson)] += leftMoney;

            //继续随机
            int sjNum = 1000;   //随机次数(根据实际业务来设计随机次数)
            for (int i = 0; i < sjNum; i++)
            {
                //随机生成两个位置
                int i1 = random.Next(0, totalPerson);
                int i2 = random.Next(0, totalPerson);
                int delta = random.Next(0, bags[i1]);
                bags[i1] -= delta;
                bags[i2] += delta;
            }
            //输出每个人获得的值
            string msg = "";
            for (int i = 0; i < totalPerson; i++)
            {
                msg += Convert.ToDecimal(bags[i]) / 100 + ",";
            }

            return msg;
        }
    }
}