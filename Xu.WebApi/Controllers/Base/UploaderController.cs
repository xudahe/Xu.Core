using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xu.Model.ResultModel;

namespace Xu.WebApi.Controllers.Base
{
    /// <summary>
    /// 文件管理--配合前端vue-simple-uploader插件使用
    /// </summary>
    [Route("api/[Controller]/[action]")]
    [ApiController]
    public class UploaderController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public UploaderController(IWebHostEnvironment env)
        {
            _env = env;
        }

        /// <summary>
        /// 分片上传文件前，检查哪些文件已上传
        /// </summary>
        /// <param name="chunkNumber">当前块的次序，第一个块是 1，注意不是从 0 开始的。</param>
        /// <param name="chunkSize">分块大小，根据 totalSize 和这个值你就可以计算出总共的块数。注意最后一块的大小可能会比这个要大</param>
        /// <param name="currentChunkSize">当前块的大小，实际大小</param>
        /// <param name="totalSize">文件总大小。</param>
        /// <param name="identifier">文件的唯一标示。MD5</param>
        /// <param name="filename">文件名。</param>
        /// <param name="relativePath">文件夹上传的时候文件的相对路径属性。</param>
        /// <param name="totalChunks">文件被分成块的总数。</param>
        /// <returns></returns>
        [HttpGet]
        public MessageModel<object> SimpleUploader(int chunkNumber, int chunkSize, int currentChunkSize
            , int totalSize, string identifier, string filename, string relativePath, int totalChunks)
        {
            var data = new MessageModel<object>() { Success = true, Message = "校验分片完成" };

            //这个是我自己的一个sql查询，根据前端请求发来的请求中的MD5码，查询是否已上传过完整的文件。
            //在所有分片都已经上传后生成的数据库记录
            //bool fileExists = _fileAppService.GetFileIsExistsByMD5(identifier) != null;
            //if (fileExists)//已上传过相同MD5码的文件
            //{
            //skipUpload  是我自己定义用来提示前端可以跳过已上传的部分
            //}

            //如果文件暂未完成上传，查询已上传的分片文件
            string filePath = Path.Combine(_env.WebRootPath, "chunkTemp", identifier);
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            var files = new List<string>();

            DirectoryInfo folder = new DirectoryInfo(filePath);
            if (folder.Exists)
            {
                folder.GetFiles().ToList().ForEach(s =>
                {
                    //vue-simple-uploader根据索引数组判断是否已上传
                    //返回格式参考：[2, 3, 4, 5, 6]
                    //s.name 我设置的分片文件名为：1.temp、2.temp、3.temp....
                    files.Add(s.Name.Substring(0, s.Name.LastIndexOf(".")));
                });
            }
            data.Response = files;
            return data;
        }

        /// <summary>
        /// 校验完成后，上传分片文件
        /// 重载，无参数的同名Upload方法
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<MessageModel<object>> SimpleUploader()
        {
            var data = new MessageModel<object>() { Success = true };

            //md5码
            string identifier = Request.Form["identifier"];
            // 分片索引
            string chunkNumber = Request.Form["chunkNumber"];
            //分片总数
            string totalChunks = Request.Form["totalChunks"];

            var file = Request.Form.Files["file"];//前端｛fileParameterName: 'file'｝设置的参数

            string filePath = Path.Combine(_env.WebRootPath, "chunkTemp", identifier);

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            string path = $"{filePath}/{chunkNumber}.temp";//文件完全路径名

            // 写入文件
            using (var addFile = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                await file.CopyToAsync(addFile); //这里最好用异步
            }

            data.Message = "分片上传完成";
            data.Response = new
            {
                needMerge = chunkNumber == totalChunks,//全部上传完成后，返回true
                filePath = path,     //返回分片文件
                tempPath = filePath, //返回分片文件夹
            };
            return data;
        }

        /// <summary>
        /// 分片合并
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="tempPath">临时文件夹（以文件MD5码命名）</param>
        [HttpGet]
        public MessageModel<object> FileMerge(string fileName, string tempPath)
        {
            var data = new MessageModel<object>() { Success = true, Message = "合并成功" };

            string filePath = Path.Combine(_env.WebRootPath, "uploadFiles");
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            string path = $"{filePath}/{fileName}";//文件完全路径名

            // 获取目录下所有的切片文件块,按照文件名(数字)进行排序
            List<string> files = Directory.GetFiles(tempPath).OrderBy(s => s).ToList();

            if (!(files.Count > 0))
            {
                data.Message = string.Format("文件列表为空");
                data.Success = false;
                return data;
            }
            //确保所有的文件都存在
            foreach (string item in files)
            {
                if (!System.IO.File.Exists(item))
                {
                    data.Message = string.Format("文件{0}不存在", item);
                    data.Success = false;
                    return data;
                }
            }

            var streamTotalSize = 0;
            FileStream addFile = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            BinaryWriter addWriter = new BinaryWriter(addFile);
            for (int i = 0; i < files.Count; i++)
            {
                //获得上传的分片数据流
                Stream stream = new FileStream(files[i], FileMode.Open);
                BinaryReader tempReader = new BinaryReader(stream);
                var streamSize = (int)stream.Length;
                //将上传的分片追加到临时文件末尾
                addWriter.Write(tempReader.ReadBytes(streamSize));
                streamTotalSize += streamSize;
                //关闭BinaryReader文件阅读器
                tempReader.Close();
                stream.Close();

                tempReader.Dispose();
                stream.Dispose();
            }
            addWriter.Close();
            addFile.Close();
            addWriter.Dispose();
            addFile.Dispose();

            //合并完后，删除掉临时的分片文件夹
            DirectoryInfo dir = new DirectoryInfo(tempPath);
            dir.Delete(true);//删除子目录和文件

            data.Response = path;
            return data;
        }
    }
}