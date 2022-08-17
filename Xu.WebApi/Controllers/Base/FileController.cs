using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xu.Model;
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
        private readonly IWebHostEnvironment _env;

        public FileController(IWebHostEnvironment webHostEnvironment)
        {
            _env = webHostEnvironment;
        }

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

        [HttpGet]
        [Route("/images/Down/Bmd")]
        [AllowAnonymous]
        public FileStreamResult? DownBmd(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return null;
            }

            string filepath = Path.Combine(_env.WebRootPath, Path.GetFileName(filename));
            if (System.IO.File.Exists(filepath))
            {
                var stream = System.IO.File.OpenRead(filepath);
                //string fileExt = ".bmd";
                //获取文件的ContentType
                var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
                //var memi = provider.Mappings[fileExt];
                var fileName = Path.GetFileName(filepath);

                HttpContext.Response.Headers.Add("fileName", fileName);

                return File(stream, "application/octet-stream", fileName);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 上传图片,多文件
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/images/Upload/Pic")]
        public async Task<MessageModel<string>> InsertPicture([FromForm] UploadFileDto dto)
        {
            if (dto.Files == null || !dto.Files.Any()) return MessageModel<string>.Msg(false, "请选择上传的文件");

            //格式限制
            var allowType = new string[] { "image/jpg", "image/png", "image/jpeg" };

            var allowedFile = dto.Files.Where(c => allowType.Contains(c.ContentType));
            if (!allowedFile.Any()) return MessageModel<string>.Msg(false, "图片格式错误");
            if (allowedFile.Sum(c => c.Length) > 1024 * 1024 * 4) return MessageModel<string>.Msg(false, "图片过大");

            string foldername = "images";
            string folderpath = Path.Combine(_env.WebRootPath, foldername);
            if (!Directory.Exists(folderpath))
            {
                Directory.CreateDirectory(folderpath);
            }
            foreach (var file in allowedFile)
            {
                string strpath = Path.Combine(foldername, DateTime.Now.ToString("MMddHHmmss") + Path.GetFileName(file.FileName));
                var path = Path.Combine(_env.WebRootPath, strpath);

                using (var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    await file.CopyToAsync(stream);
                }
            }

            var excludeFiles = dto.Files.Except(allowedFile);

            if (excludeFiles.Any())
            {
                var infoMsg = $"{string.Join('、', excludeFiles.Select(c => c.FileName))} 图片格式错误";

                return MessageModel<string>.Msg(false, infoMsg);
            }

            return MessageModel<string>.Msg(false, "上传成功");
        }

        /// <summary>
        /// 文件切片上传(测试)
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