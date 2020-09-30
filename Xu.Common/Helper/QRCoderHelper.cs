using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace Xu.Common
{
    /// <summary>
    /// 二维码帮助类
    /// </summary>
    public class QRCoderHelper
    {
        /// <summary>
        ///普通二维码
        /// </summary>
        /// <param name="url">存储内容</param>
        /// <param name="pixel">像素大小</param>
        /// <returns></returns>
        public static Bitmap GetPTQRCode(string url, int pixel)
        {
            QRCodeGenerator generator = new QRCodeGenerator();
            QRCodeData codeData = generator.CreateQrCode(url, QRCodeGenerator.ECCLevel.M, true);
            QRCode qrcode = new QRCode(codeData);
            Bitmap qrImage = qrcode.GetGraphic(pixel, Color.Black, Color.White, true);
            return qrImage;
        }

        /// <summary>
        ///带logo的二维码
        /// </summary>
        /// <param name="url">存储内容</param>
        /// <param name="logoPath">logo图片</param>
        /// <param name="pixel">像素大小</param>
        /// <returns></returns>
        public static Bitmap GetLogoQRCode(string url, string logoPath, int pixel)
        {
            QRCodeGenerator generator = new QRCodeGenerator();
            QRCodeData codeData = generator.CreateQrCode(url, QRCodeGenerator.ECCLevel.M, true);
            QRCode qrcode = new QRCode(codeData);
            Bitmap icon = new Bitmap(logoPath);
            Bitmap qrImage = qrcode.GetGraphic(pixel, Color.Black, Color.White, icon, 15, 6, true);

            //GetGraphic方法参数介绍
            //pixelsPerModule //生成二维码图片的像素大小 ，我这里设置的是5
            //darkColor       //暗色   一般设置为Color.Black 黑色
            //lightColor      //亮色   一般设置为Color.White  白色
            //icon             //二维码 水印图标 例如：Bitmap icon = new Bitmap(context.Server.MapPath("~/images/zs.png")); 默认为NULL ，加上这个二维码中间会显示一个图标
            //iconSizePercent  //水印图标的大小比例 ，可根据自己的喜好设置
            //iconBorderWidth  // 水印图标的边框
            //drawQuietZones   //静止区，位于二维码某一边的空白边界,用来阻止读者获取与正在浏览的二维码无关的信息 即是否绘画二维码的空白边框区域 默认为true

            return qrImage;
        }

        /// <summary>
        /// 生成文字图片
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Bitmap GetImageQRCode(string str)
        {
            Bitmap qrImage = new Bitmap(600, 40, PixelFormat.Format24bppRgb);

            Graphics g = Graphics.FromImage(qrImage);

            try
            {
                Font font = new Font("SimHei", 14, FontStyle.Regular);

                g.Clear(Color.White);

                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;

                Rectangle rectangle = new Rectangle(0, 0, 600, 40);

                g.DrawString(str, font, new SolidBrush(Color.Black), rectangle, format);

                return qrImage;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                GC.Collect();
            }
        }

        /// <summary>
        /// 创建二维码返回文件路径名称
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="plainText">二维码内容</param>
        /// <param name="pixel">像素大小</param>
        public static string CreateQRCodeToFile(string filePath, string plainText, int pixel)
        {
            try
            {
                string fileName = "";
                if (string.IsNullOrEmpty(plainText))
                {
                    return "";
                }

                //filePath = Path.Combine(filePath, "QRCode");
                //if (!Directory.Exists(filePath))//如果路径不存在
                //{
                //    Directory.CreateDirectory(filePath);//创建一个路径的文件夹
                //}

                //创建二维码文件路径名称
                fileName = Path.Combine(filePath, $@"{DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(100, 1000)}.jpeg");

                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                //QRCodeGenerator.ECCLevel:纠错能力,Q级：约可纠错25%的数据码字
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(plainText, QRCodeGenerator.ECCLevel.Q);
                QRCode qrcode = new QRCode(qrCodeData);
                Bitmap qrCodeImage = qrcode.GetGraphic(pixel);
                qrCodeImage.Save(fileName, ImageFormat.Jpeg);
                return fileName;
            }
            catch (Exception ex)
            {
                throw new Exception("创建二维码返回文件路径名称方法异常", ex);
            }
        }

        /// <summary>
        /// 创建二维码返回byte数组
        /// </summary>
        /// <param name="plainText">二维码内容</param>
        /// <param name="pixel">像素大小</param>
        public static byte[] CreateQRCodeToBytes(string plainText, int pixel)
        {
            try
            {
                if (string.IsNullOrEmpty(plainText))
                {
                    return null;
                }

                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                //QRCodeGenerator.ECCLevel:纠错能力,Q级：约可纠错25%的数据码字
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(plainText, QRCodeGenerator.ECCLevel.Q);
                QRCode qrcode = new QRCode(qrCodeData);
                Bitmap qrCodeImage = qrcode.GetGraphic(pixel);
                MemoryStream ms = new MemoryStream();
                qrCodeImage.Save(ms, ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();

                return arr;
            }
            catch (Exception ex)
            {
                throw new Exception("创建二维码返回byte数组方法异常", ex);
            }
        }

        /// <summary>
        /// 创建二维码返回Base64字符串
        /// </summary>
        /// <param name="plainText">二维码内容</param>
        /// <param name="pixel">像素大小</param>
        /// <param name="plainText"></param>
        public static string CreateQRCodeToBase64(string plainText, int pixel, bool hasEdify = true)
        {
            try
            {
                string result = "";
                if (string.IsNullOrEmpty(plainText))
                {
                    return "";
                }

                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                //QRCodeGenerator.ECCLevel:纠错能力,Q级：约可纠错25%的数据码字
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(plainText, QRCodeGenerator.ECCLevel.Q);
                QRCode qrcode = new QRCode(qrCodeData);
                Bitmap qrCodeImage = qrcode.GetGraphic(pixel);
                MemoryStream ms = new MemoryStream();
                qrCodeImage.Save(ms, ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                if (hasEdify)
                {
                    result = "data:image/jpeg;base64," + Convert.ToBase64String(arr);
                }
                else
                {
                    result = Convert.ToBase64String(arr);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("创建二维码返回Base64字符串方法异常", ex);
            }
        }
    }
}