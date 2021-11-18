using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xu.Model.ResultModel;

namespace Blog.Core.Controllers
{
    /// <summary>
    /// 图片管理
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : Controller
    {
        /// <summary>
        /// 下载图片（支持中文字符）
        /// </summary>
        /// <param name="environment"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/File/imgDownload")]
        public FileStreamResult DownloadPicture([FromServices] IWebHostEnvironment environment)
        {
            string foldername = "images";
            string filepath = Path.Combine(environment.WebRootPath, foldername, "20201112145833764.jpeg");
            var stream = System.IO.File.OpenRead(filepath);
            string fileExt = ".jpg";  // 这里可以写一个获取文件扩展名的方法，获取扩展名
            //获取文件的ContentType
            var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
            var memi = provider.Mappings[fileExt];
            var fileName = Path.GetFileName(filepath);

            return File(stream, memi, fileName);
        }

        /// <summary>
        /// 上传图片,多文件，可以使用 postman 测试，
        /// 如果是单文件，可以 参数写 IFormFile file1
        /// </summary>
        /// <param name="environment"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/File/imgUpload")]
        public async Task<MessageModel<string>> InsertPicture([FromServices] IWebHostEnvironment environment)
        {
            var data = new MessageModel<string>();
            string path = string.Empty;
            string foldername = "images";
            IFormFileCollection files = null;

            try
            {
                files = Request.Form.Files;
            }
            catch (Exception)
            {
                files = null;
            }

            if (files == null || !files.Any()) { data.Message = "请选择上传的文件。"; return data; }
            //格式限制
            var allowType = new string[] { "image/jpg", "image/png", "image/jpeg" };

            string folderpath = Path.Combine(environment.WebRootPath, foldername);
            if (!Directory.Exists(folderpath))
            {
                Directory.CreateDirectory(folderpath);
            }

            if (files.Any(c => allowType.Contains(c.ContentType)))
            {
                if (files.Sum(c => c.Length) <= 1024 * 1024 * 4)
                {
                    var file = files.FirstOrDefault();
                    string strpath = Path.Combine(foldername, file.FileName);
                    path = Path.Combine(environment.WebRootPath, strpath);

                    using (var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        await file.CopyToAsync(stream);
                    }

                    data = new MessageModel<string>()
                    {
                        Response = strpath,
                        Message = "上传成功",
                        Success = true,
                    };
                    return data;
                }
                else
                {
                    data.Message = "图片过大,超过4M";
                    return data;
                }
            }
            else

            {
                data.Message = "图片格式错误";
                return data;
            }
        }

        /// <summary>
        /// 文件切片上传
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="file">文件</param>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/File/fileUpload")]
        public async Task<MessageModel<string>> FileUpload([FromServices] IWebHostEnvironment environment, IFormFile file)
        {
            var data = new MessageModel<string>();
            string fileName = Request.Form["fileName"]; //文件夹名称
            string chunkName = Request.Form["chunkName"]; //文件名称

            if (file == null) { data.Message = "请选择上传的文件。"; return data; }

            if (string.IsNullOrEmpty(fileName)) { data.Message = "文件夹名称不能为空。"; return data; }

            if (string.IsNullOrEmpty(chunkName)) { data.Message = "文件名称不能为空。"; return data; }

            string folderpath = Path.Combine(environment.WebRootPath, fileName);
            if (!Directory.Exists(folderpath))
            {
                Directory.CreateDirectory(folderpath);
            }

            string strpath = Path.Combine(fileName, chunkName);
            string path = Path.Combine(environment.WebRootPath, strpath);

            //写入文件
            using (var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                await file.CopyToAsync(stream);
            }

            data = new MessageModel<string>()
            {
                Response = path,
                Message = "上传成功",
                Success = true,
            };
            return data;
        }
    }
}