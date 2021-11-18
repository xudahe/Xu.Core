using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xu.Model.ResultModel;

namespace Xu.WebApi.Controllers.Base
{
    /// <summary>
    /// 文件管理
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController:Controller
    {
        private readonly IWebHostEnvironment _env;

        public UploadController(IWebHostEnvironment env)
        {
            _env = env;
        }

        /// <summary>  
        /// 获取指定文件的已上传的最大文件块
        /// </summary>
        /// <param name="md5">文件唯一值</param>
        /// <param name="ext">文件后缀</param>
        /// <returns></returns>
        [HttpGet]
        public MessageModel<string> GetMaxChunk(string md5, string ext)
        {
            var data = new MessageModel<string>() { Success = true,Message= "获取成功" };

            try
            {
                // 检测文件夹是否存在，不存在则创建
                var userPath = GetPath();
                if (!Directory.Exists(userPath))
                {
                    Directory.CreateDirectory(userPath);
                }

                var md5Folder = GetFileMd5Folder(md5);
                if (!Directory.Exists(md5Folder))
                {
                    Directory.CreateDirectory(md5Folder);
                }

                var fileName = md5 + "." + ext;
                string targetPath = Path.Combine(md5Folder, fileName);
                // 文件已经存在，则可能存在问题，直接删除，重新上传
                if (System.IO.File.Exists(targetPath))
                {
                    System.IO.File.Delete(targetPath);

                    data.Response = "请重新上传！";
                    data.Success = false;
                    return data;
                }

                var dicInfo = new DirectoryInfo(md5Folder);
                var files = dicInfo.GetFiles();
                var chunk = files.Length;
                if (chunk > 1)
                {
                    //当文件上传中时，页面刷新，上传中断，这时最后一个保存的块的大小可能会有异常，所以这里直接删除最后一个块文件                  
                    data.Response = (chunk - 1).ToString();
                    return data;
                }

                return data;
            }
            catch (Exception ex)
            {
                data.Message = ex.Message;
                 data.Success = false;
                return data;
            }
        }

        /// <summary>
        /// 文件分块上传
        /// </summary>
        /// <param name="file">文件</param>
        /// <param name="md5">文件md5 值</param>
        /// <param name="chunk">当前分片在上传分片中的顺序（从0开始）</param>
        /// <param name="chunks">最大片数</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<MessageModel<string>> ChunkUpload(IFormFile file, string md5, int? chunk, int chunks = 0)
        {
            var data = new MessageModel<string>();

            try
            {
                var md5Folder = GetFileMd5Folder(md5);
                var filePath = "";  // 要保存的文件路径

                // 存在分片参数,并且，最大的片数大于1片时     
                if (chunk.HasValue && chunks > 1)
                {
                    var uploadNumsOfLoop = 10;
                    // 是10的倍数就休眠几秒（数据库设置的秒数）
                    if (chunk % uploadNumsOfLoop == 0)
                    {
                        var timesOfLoop = 10;   //休眠毫秒,可从数据库取值
                        Thread.Sleep(timesOfLoop);
                    }
                    //建立临时传输文件夹
                    if (!Directory.Exists(md5Folder))
                    {
                        Directory.CreateDirectory(md5Folder);
                    }

                    filePath = md5Folder + "/" + chunk;
                    data.Response = chunk.Value.ToString();
                    if (chunks == chunk)
                    {
                        data.Message = "上传完成";
                    }
                }
                else
                {
                    var fileName = file?.FileName;
                    if (string.IsNullOrEmpty(fileName))
                    {
                        var fileNameQuery = Request.Query.FirstOrDefault(t => t.Key == "name");
                        fileName = fileNameQuery.Value.FirstOrDefault();
                    }

                    //没有分片直接保存
                    filePath = md5Folder + Path.GetExtension(fileName);
                    data.Message = "chunked";
                }

                // 写入文件
                using (var addFile = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    if (file != null)
                    {
                        await file.CopyToAsync(addFile);
                    }
                    else
                    {
                        await Request.Body.CopyToAsync(addFile);
                    }
                }

                data.Response = filePath;
                return data;
            }
            catch (Exception ex)
            {
                data.Success = false;
                data.Message = ex.Message;
                return data;
            }
        }

        /// <summary>
        /// 合并文件
        /// </summary>
        /// <param name="md5">文件md5 值</param>
        /// <param name="filename">文件</param>
        /// <param name="fileTotalSize">文件字节总数</param>
        /// <returns></returns>
        [HttpPost]
        public MessageModel<string> MergeFiles(string md5, string filename,int fileTotalSize)
        {
            var data = new MessageModel<string>() { Success =true,Message = "合并成功" };

            try
            {
                //源数据文件夹
                string sourcePath = GetFileMd5Folder(md5);
                //合并后的文件路径
                string targetFilePath = sourcePath + Path.GetExtension(filename);
                // 目标文件不存在，则需要合并
                if (!System.IO.File.Exists(targetFilePath))
                {
                    if (!Directory.Exists(sourcePath))
                    {
                        data.Message = "未找到对应的文件片";
                        data.Success = false;
                        return data;
                    }

                    MergeDiskFile(sourcePath, targetFilePath);
                }


                #region 校验合并后的文件
                //1.是否没有漏掉块(chunk)
                //2.检测文件大小是否跟客户端一样
                //3.检查文件的MD5值是否一致

                // 文件字节总数
                var targetFile = new FileInfo(targetFilePath);
                var streamTotalSize = targetFile.Length;
              
                    if (streamTotalSize != fileTotalSize)
                    {
                        data.Message = "[" + filename + "]文件上传时发生损坏，请重新上传";
                        return data;
                    }

                    // 对文件进行 MD5 唯一验证
                    var fileMd5 = GetMD5HashFromFile(targetFilePath);
                    if (!fileMd5.Equals(md5))
                    {
                        data.Message = "[" + filename + "],文件MD5值不对等";
                        return data;
                    }

                #endregion

                DeleteFolder(sourcePath);//删除切片文件
      
                data.Response = targetFilePath;
                return data;
            }
            catch (Exception ex)
            {
                data.Success = false;
                data.Message = ex.Message;
                return data;
            }
        }


        /// <summary>
        /// 将磁盘上的切片源合并成一个文件
        /// <returns>返回所有切片文件的字节总和</returns>
        /// </summary>
        /// <param name="sourcePath">磁盘上的切片源</param>
        /// <param name="targetPath">目标文件路径</param>
        private static int MergeDiskFile(string sourcePath, string targetPath)
        {
            FileStream addFile = null;
            BinaryWriter addWriter = null;
            try
            {
                var streamTotalSize = 0;
                addFile = new FileStream(targetPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                addWriter = new BinaryWriter(addFile);
                // 获取目录下所有的切片文件块
                FileInfo[] files = new DirectoryInfo(sourcePath).GetFiles();
                // 按照文件名(数字)进行排序
                var orderFileInfoList = files.OrderBy(f => int.Parse(f.Name));
                foreach (FileInfo diskFile in orderFileInfoList)
                {
                    //获得上传的分片数据流 
                    Stream stream = diskFile.Open(FileMode.Open);
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
                return streamTotalSize;
            }
            catch (Exception)
            {
                if (addFile != null)
                {
                    addFile.Close();
                    addFile.Dispose();
                }

                if (addWriter != null)
                {
                    addWriter.Close();
                    addWriter.Dispose();
                }

                throw;
            }
        }

        /// <summary>
        /// C#获取文件MD5值方法
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static string GetMD5HashFromFile(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
        }

        /// <summary>
        /// 删除文件夹及其内容
        /// <para>附带删除超过一个月的文件以及文件夹</para>
        /// </summary>
        /// <param name="strPath"></param>
        private static void DeleteFolder(string strPath)
        {
            if (Directory.Exists(strPath))
                Directory.Delete(strPath, true);

            #region 删除一个月以前的临时文件夹与文件
            var chunkTemp = Path.GetDirectoryName(strPath);
            DirectoryInfo dir = new DirectoryInfo(chunkTemp);
            DirectoryInfo[] dii = dir.GetDirectories();
            // 超过一个月的文件夹和文件
            var expireDate = DateTime.Now.AddMonths(-1);
            var deleteExpire = dii.Where(t => t.LastWriteTime < expireDate).ToList();
            if (deleteExpire.Any())
            {
                foreach (var item in deleteExpire)
                {
                    Directory.Delete(chunkTemp + "/" + item, true);
                }
            }

            var deleteExpireFile = dir.GetFiles().Where(t => t.LastWriteTime < expireDate).ToList();
            if (deleteExpireFile.Any())
            {
                foreach (var item in deleteExpireFile)
                {
                    System.IO.File.Delete(chunkTemp + "/" + item);
                }
            }
            #endregion
        }


        /// <summary>
        /// 获得文件MD5文件夹
        /// </summary>
        /// <returns></returns>
        private string GetFileMd5Folder(string identifier)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                throw new Exception("缺少文件MD5值");
            }

            return GetPath() + "ChunkTemp\\" + identifier;
        }

        /// <summary>
        /// 获得上传文件目录
        /// </summary>
        /// <returns></returns>
        private string GetPath()
        {
            return _env.WebRootPath + "\\Files\\";
        }


    }
}
