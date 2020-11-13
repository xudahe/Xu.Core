using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Xu.Common;
using Xu.Model.ResultModel;

namespace Xu.WebApi.Controllers
{
    /// <summary>
    /// 二维码
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class QRCodeController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public QRCodeController(IWebHostEnvironment env)
        {
            _env = env;
        }

        /// <summary>
        ///生成二维码（路径）
        /// </summary>
        /// <param name="plainText">存储内容</param>
        /// <param name="pixel">像素大小</param>
        /// <returns>返回二维码路径</returns>
        [HttpGet]
        public object GetPTQRCode(string plainText, int pixel = 5)
        {
            var data = new MessageModel<string>();

            if (string.IsNullOrEmpty(plainText))
            {
                data.Message = "存储内容不能为空！";
            }
            else
            {
                data.Message = "生成成功";
                data.Success = true;
                data.Response = QRCoderHelper.CreateQRCodeToFile(_env.WebRootPath, plainText, pixel);
            }

            return data;
        }

        /// <summary>
        ///生成二维码（byte数组）
        /// </summary>
        /// <param name="plainText">存储内容</param>
        /// <param name="pixel">像素大小</param>
        /// <returns>返回二维码路径</returns>
        [HttpGet]
        public object GetBTQRCode(string plainText, int pixel = 5)
        {
            var data = new MessageModel<byte[]>();

            if (string.IsNullOrEmpty(plainText))
            {
                data.Message = "存储内容不能为空！";
            }
            else
            {
                data.Message = "生成成功";
                data.Success = true;
                data.Response = QRCoderHelper.CreateQRCodeToBytes(plainText, pixel);
            }

            return data;
        }

        /// <summary>
        ///生成二维码（Base64）
        /// </summary>
        /// <param name="plainText">存储内容</param>
        /// <param name="pixel">像素大小</param>
        /// <returns>返回二维码路径</returns>
        [HttpGet]
        public object GetBSQRCode(string plainText, int pixel = 5)
        {
            var data = new MessageModel<string>();

            if (string.IsNullOrEmpty(plainText))
            {
                data.Message = "存储内容不能为空！";
            }
            else
            {
                data.Message = "生成成功";
                data.Success = true;
                data.Response = QRCoderHelper.CreateQRCodeToBase64(plainText, pixel);
            }

            return data;
        }
    }
}