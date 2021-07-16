using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Xu.Common
{
    /// <summary>
    /// 文件帮助类
    /// </summary>
    public class FileHelper : IDisposable
    {
        private bool _alreadyDispose = false;

        private readonly string key = "0123456789"; //默认密钥
        private byte[] sKey;
        private byte[] sIV;

        /// <summary>
        /// 同步标识
        /// </summary>
        private static readonly object sync = new object();

        #region 构造函数

        public FileHelper()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        ~FileHelper()
        {
            Dispose(); ;
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (_alreadyDispose) return;
            _alreadyDispose = true;
        }

        #endregion 构造函数

        #region IDisposable 成员

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable 成员

        #region 取得文件后缀名

        /****************************************
          * 函数名称：GetPostfixStr
          * 功能说明：取得文件后缀名
          * 参     数：filename:文件名称
          * 调用示列：
          *            string filename = "aaa.aspx";
          *            string s = EC.FileObj.GetPostfixStr(filename);
         *****************************************/

        /// <summary>
        /// 取后缀名
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>.gif|.html格式</returns>
        public static string GetPostfixStr(string filename)
        {
            int start = filename.LastIndexOf(".");
            int length = filename.Length;
            string postfix = filename[start..length];
            return postfix;
        }

        #endregion 取得文件后缀名

        #region 根据文件大小获取指定前缀的可用文件名

        /// <summary>
        /// 根据文件大小获取指定前缀的可用文件名
        /// </summary>
        /// <param name="folderPath">文件夹</param>
        /// <param name="prefix">文件前缀</param>
        /// <param name="size">文件大小(1m)</param>
        /// <param name="ext">文件后缀(.log)</param>
        /// <returns>可用文件名</returns>
        public static string GetAvailableFileWithPrefixOrderSize(string folderPath, string prefix, int size = 1 * 1024 * 1024, string ext = ".log")
        {
            var allFiles = new DirectoryInfo(folderPath);
            var selectFiles = allFiles.GetFiles().Where(fi => fi.Name.ToLower().Contains(prefix.ToLower()) && fi.Extension.ToLower() == ext.ToLower() && fi.Length < size).OrderByDescending(d => d.Name).ToList();

            if (selectFiles.Count > 0)
            {
                return selectFiles.FirstOrDefault().FullName;
            }

            return Path.Combine(folderPath, $@"{prefix}_{DateTime.Now.DateToTimeStamp()}.log");
        }

        public static string GetAvailableFileNameWithPrefixOrderSize(string _contentRoot, string prefix, int size = 1 * 1024 * 1024, string ext = ".log")
        {
            var folderPath = Path.Combine(_contentRoot, "Log");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var allFiles = new DirectoryInfo(folderPath);
            var selectFiles = allFiles.GetFiles().Where(fi => fi.Name.ToLower().Contains(prefix.ToLower()) && fi.Extension.ToLower() == ext.ToLower() && fi.Length < size).OrderByDescending(d => d.Name).ToList();

            if (selectFiles.Count > 0)
            {
                return selectFiles.FirstOrDefault().Name.Replace(".log", "");
            }

            return $@"{prefix}_{DateTime.Now.DateToTimeStamp()}";
        }

        #endregion 根据文件大小获取指定前缀的可用文件名

        #region 写文件

        /****************************************
          * 函数名称：WriteFile
          * 功能说明：写文件,会覆盖掉以前的内容
          * 参     数：Path:文件路径,Strings:文本内容
          * 调用示列：
          *            string Path = Server.MapPath("Default2.aspx");
          *            string Strings = "这是我写的内容啊";
          *            EC.FileObj.WriteFile(Path,Strings);
         *****************************************/

        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="Path">文件路径</param>
        /// <param name="Strings">文件内容</param>
        public static void WriteFile(string Path, string Strings)
        {
            if (!File.Exists(Path))
            {
                FileStream f = File.Create(Path);
                f.Close();
            }
            StreamWriter f2 = new StreamWriter(Path, false, Encoding.GetEncoding("gb2312"));
            f2.Write(Strings);
            f2.Close();
            f2.Dispose();
        }

        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="Path">文件路径</param>
        /// <param name="Strings">文件内容</param>
        /// <param name="encode">编码格式</param>
        public static void WriteFile(string Path, string Strings, Encoding encode)
        {
            if (!File.Exists(Path))
            {
                FileStream f = File.Create(Path);
                f.Close();
            }
            StreamWriter f2 = new StreamWriter(Path, false, encode);
            f2.Write(Strings);
            f2.Close();
            f2.Dispose();
        }

        #endregion 写文件

        #region 读文件

        /****************************************
          * 函数名称：ReadFile
          * 功能说明：读取文本内容
          * 参     数：Path:文件路径
          * 调用示列：
          *            string Path = Server.MapPath("Default2.aspx");
          *            string s = EC.FileObj.ReadFile(Path);
         *****************************************/

        /// <summary>
        /// 读文件
        /// </summary>
        /// <param name="Path">文件路径</param>
        /// <returns></returns>
        public static string ReadFile(string Path)
        {
            string s;
            if (!File.Exists(Path))
                s = "不存在相应的目录";
            else
            {
                StreamReader f2 = new StreamReader(Path, Encoding.GetEncoding("gb2312"));
                s = f2.ReadToEnd();
                f2.Close();
                f2.Dispose();
            }

            return s;
        }

        /// <summary>
        /// 读文件
        /// </summary>
        /// <param name="Path">文件路径</param>
        /// <param name="encode">编码格式</param>
        /// <returns></returns>
        public static string ReadFile(string Path, Encoding encode)
        {
            string s;
            if (!File.Exists(Path))
                s = "不存在相应的目录";
            else
            {
                StreamReader f2 = new StreamReader(Path, encode);
                s = f2.ReadToEnd();
                f2.Close();
                f2.Dispose();
            }

            return s;
        }

        #endregion 读文件

        #region 追加文件

        /****************************************
          * 函数名称：FileAdd
          * 功能说明：追加文件内容
          * 参     数：Path:文件路径,strings:内容
          * 调用示列：
          *            string Path = Server.MapPath("Default2.aspx");
          *            string Strings = "新追加内容";
          *            EC.FileObj.FileAdd(Path, Strings);
         *****************************************/

        /// <summary>
        /// 追加文件
        /// </summary>
        /// <param name="Path">文件路径</param>
        /// <param name="strings">内容</param>
        public static void FileAdd(string Path, string strings)
        {
            StreamWriter sw = File.AppendText(Path);
            sw.Write(strings);
            sw.Flush();
            sw.Close();
        }

        #endregion 追加文件

        #region 拷贝文件

        /****************************************
          * 函数名称：FileCoppy
          * 功能说明：拷贝文件
          * 参     数：OrignFile:原始文件,NewFile:新文件路径
          * 调用示列：
          *            string orignFile = Server.MapPath("Default2.aspx");
          *            string NewFile = Server.MapPath("Default3.aspx");
          *            EC.FileObj.FileCoppy(OrignFile, NewFile);
         *****************************************/

        /// <summary>
        /// 拷贝文件
        /// </summary>
        /// <param name="OrignFile">原始文件</param>
        /// <param name="NewFile">新文件路径</param>
        public static void FileCoppy(string orignFile, string NewFile)
        {
            File.Copy(orignFile, NewFile, true);
        }

        #endregion 拷贝文件

        #region 删除文件

        /****************************************
          * 函数名称：FileDel
          * 功能说明：删除文件
          * 参     数：Path:文件路径
          * 调用示列：
          *            string Path = Server.MapPath("Default3.aspx");
          *            EC.FileObj.FileDel(Path);
         *****************************************/

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="Path">路径</param>
        public static void FileDel(string Path)
        {
            File.Delete(Path);
        }

        #endregion 删除文件

        #region 移动文件

        /****************************************
          * 函数名称：FileMove
          * 功能说明：移动文件
          * 参     数：OrignFile:原始路径,NewFile:新文件路径
          * 调用示列：
          *             string orignFile = Server.MapPath("../说明.txt");
          *             string NewFile = Server.MapPath("http://www.cnXus.com/说明.txt");
          *             EC.FileObj.FileMove(OrignFile, NewFile);
         *****************************************/

        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="OrignFile">原始路径</param>
        /// <param name="NewFile">新路径</param>
        public static void FileMove(string orignFile, string NewFile)
        {
            File.Move(orignFile, NewFile);
        }

        #endregion 移动文件

        #region 在当前目录下创建目录

        /****************************************
          * 函数名称：FolderCreate
          * 功能说明：在当前目录下创建目录
          * 参     数：OrignFolder:当前目录,NewFloder:新目录
          * 调用示列：
          *            string orignFolder = Server.MapPath("test/");
          *            string NewFloder = "new";
          *            EC.FileObj.FolderCreate(OrignFolder, NewFloder);
         *****************************************/

        /// <summary>
        /// 在当前目录下创建目录
        /// </summary>
        /// <param name="OrignFolder">当前目录</param>
        /// <param name="NewFloder">新目录</param>
        public static void FolderCreate(string orignFolder, string NewFloder)
        {
            Directory.SetCurrentDirectory(orignFolder);
            Directory.CreateDirectory(NewFloder);
        }

        #endregion 在当前目录下创建目录

        #region 递归删除文件夹目录及文件

        /****************************************
          * 函数名称：DeleteFolder
          * 功能说明：递归删除文件夹目录及文件
          * 参     数：dir:文件夹路径
          * 调用示列：
          *            string dir = Server.MapPath("test/");
          *            EC.FileObj.DeleteFolder(dir);
         *****************************************/

        /// <summary>
        /// 递归删除文件夹目录及文件
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static void DeleteFolder(string dir)
        {
            if (Directory.Exists(dir)) //如果存在这个文件夹删除之
            {
                foreach (string d in Directory.GetFileSystemEntries(dir))
                {
                    if (File.Exists(d))
                        File.Delete(d); //直接删除其中的文件
                    else
                        DeleteFolder(d); //递归删除子文件夹
                }
                Directory.Delete(dir); //删除已空文件夹
            }
        }

        #endregion 递归删除文件夹目录及文件

        #region 将指定文件夹下面的所有内容copy到目标文件夹下面 果目标文件夹为只读属性就会报错。

        /****************************************
          * 函数名称：CopyDir
          * 功能说明：将指定文件夹下面的所有内容copy到目标文件夹下面 果目标文件夹为只读属性就会报错。
          * 参     数：srcPath:原始路径,aimPath:目标文件夹
          * 调用示列：
          *            string srcPath = Server.MapPath("test/");
          *            string aimPath = Server.MapPath("test1/");
          *            EC.FileObj.CopyDir(srcPath,aimPath);
         *****************************************/

        /// <summary>
        /// 指定文件夹下面的所有内容copy到目标文件夹下面
        /// </summary>
        /// <param name="srcPath">原始路径</param>
        /// <param name="aimPath">目标文件夹</param>
        public static void CopyDir(string srcPath, string aimPath)
        {
            try
            {
                // 检查目标目录是否以目录分割字符结束如果不是则添加之
                if (aimPath[^1] != Path.DirectorySeparatorChar)
                    aimPath += Path.DirectorySeparatorChar;
                // 判断目标目录是否存在如果不存在则新建之
                if (!Directory.Exists(aimPath))
                    Directory.CreateDirectory(aimPath);
                // 得到源目录的文件列表，该里面是包含文件以及目录路径的一个数组
                //如果你指向copy目标文件下面的文件而不包含目录请使用下面的方法
                //string[] fileList = Directory.GetFiles(srcPath);
                string[] fileList = Directory.GetFileSystemEntries(srcPath);
                //遍历所有的文件和目录
                foreach (string file in fileList)
                {
                    //先当作目录处理如果存在这个目录就递归Copy该目录下面的文件

                    if (Directory.Exists(file))
                        CopyDir(file, aimPath + Path.GetFileName(file));
                    //否则直接Copy文件
                    else
                        File.Copy(file, aimPath + Path.GetFileName(file), true);
                }
            }
            catch (Exception ee)
            {
                throw new Exception(ee.ToString());
            }
        }

        #endregion 将指定文件夹下面的所有内容copy到目标文件夹下面 果目标文件夹为只读属性就会报错。

        #region 加密文件

        /// <summary>
        /// 加密文件
        /// </summary>
        /// <param name="filePath">输入文件路径</param>
        /// <param name="savePath">加密后输出文件路径</param>
        /// <param name="keyStr">密码，可以为“”</param>
        /// <returns></returns>
        public bool EncryptFile(string filePath, string savePath, string keyStr)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            if (keyStr == "")
                keyStr = key;
            FileStream fs = File.OpenRead(filePath);
            byte[] inputByteArray = new byte[fs.Length];
            fs.Read(inputByteArray, 0, (int)fs.Length);
            fs.Close();
            byte[] keyByteArray = Encoding.Default.GetBytes(keyStr);
            SHA1 ha = new SHA1Managed();
            byte[] hb = ha.ComputeHash(keyByteArray);
            sKey = new byte[8];
            sIV = new byte[8];
            for (int i = 0; i < 8; i++)
                sKey[i] = hb[i];
            for (int i = 8; i < 16; i++)
                sIV[i - 8] = hb[i];
            des.Key = sKey;
            des.IV = sIV;
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            fs = File.OpenWrite(savePath);
            foreach (byte b in ms.ToArray())
            {
                fs.WriteByte(b);
            }
            fs.Close();
            cs.Close();
            ms.Close();
            return true;
        }

        #endregion 加密文件

        #region 解密文件

        /// <summary>
        /// 解密文件
        /// </summary>
        /// <param name="filePath">输入文件路径</param>
        /// <param name="savePath">解密后输出文件路径</param>
        /// <param name="keyStr">密码，可以为“”</param>
        /// <returns></returns>
        public bool DecryptFile(string filePath, string savePath, string keyStr)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            if (keyStr == "")
                keyStr = key;
            FileStream fs = File.OpenRead(filePath);
            byte[] inputByteArray = new byte[fs.Length];
            fs.Read(inputByteArray, 0, (int)fs.Length);
            fs.Close();
            byte[] keyByteArray = Encoding.Default.GetBytes(keyStr);
            SHA1 ha = new SHA1Managed();
            byte[] hb = ha.ComputeHash(keyByteArray);
            sKey = new byte[8];
            sIV = new byte[8];
            for (int i = 0; i < 8; i++)
                sKey[i] = hb[i];
            for (int i = 8; i < 16; i++)
                sIV[i - 8] = hb[i];
            des.Key = sKey;
            des.IV = sIV;
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            fs = File.OpenWrite(savePath);
            foreach (byte b in ms.ToArray())
            {
                fs.WriteByte(b);
            }
            fs.Close();
            cs.Close();
            ms.Close();
            return true;
        }

        #endregion 解密文件

        #region 获取文件路并自动创建目录

        /// <summary>
        /// 根据文件目录、编号、文件名生成文件路径，并且创建文件存放目录
        /// 格式为:/directory/code/filename
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="directory"></param>
        /// <param name="code"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string GetFiePathAndCreateDirectoryByCode<T>(string directory, T code, string fileName)
        {
            if (directory == null)
            {
                throw new ArgumentNullException("FileHelper.GetCreatePath.directory");
            }
            directory = directory.TrimEnd('/');
            string path = new StringBuilder("{0}//{1}//{2}").AppendFormat(directory, code, fileName).ToString();
            directory = Path.GetDirectoryName(path);
            if (!IsExistDirectory(directory))
            {
                CreateDirectory(directory);
            }
            return path;
        }

        /// <summary>
        /// 根据文件目录、日期、文件名生成文件路径，并且创建文件存放目录
        /// 格式为:/directory/2015/01/01/filename
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="directory"></param>
        /// <param name="code"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string GetFiePathAndCreateDirectoryByDate<T>(string directory, string fileName)
        {
            if (directory == null)
            {
                throw new ArgumentNullException("FileSugar.GetCreatePath.directory");
            }
            directory = directory.TrimEnd('/');
            string path = new StringBuilder("{0}//{1}//{2}//{3}//{4}").AppendFormat(directory, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, fileName).ToString();
            directory = Path.GetDirectoryName(path);
            if (!IsExistDirectory(directory))
            {
                CreateDirectory(directory);
            }
            return path;
        }

        #endregion 获取文件路并自动创建目录

        #region 获取缩略图名称

        public static string GetMinPic(string filename, int index)
        {
            string str = "";
            if (string.IsNullOrEmpty(filename))
                return str;
            int nLastDot = filename.LastIndexOf(".");
            if (nLastDot == -1)
                return str;
            str = filename.Substring(0, nLastDot) + "_" + index.ToString() + filename.Substring(nLastDot, filename.Length - nLastDot);
            if (index == -1)
            {
                str = filename.Substring(0, nLastDot) + filename.Substring(nLastDot, filename.Length - nLastDot);
            }
            return str;
        }

        /// <summary>
        /// 获取缩略图片路径
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="filename"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string GetMinPic(string dir, string filename, int index)
        {
            if (string.IsNullOrEmpty(filename))
                return "";
            if (index < 0)
                index = 0;
            string minPic = string.Empty;
            minPic = string.Format("{0}_{1}{2}", Path.GetFileNameWithoutExtension(filename), index, Path.GetExtension(filename));
            if (!string.IsNullOrEmpty(dir))
                minPic = Path.Combine(dir, minPic);
            return minPic;
        }

        #endregion 获取缩略图名称

        #region 检测指定目录是否存在

        /// <summary>
        /// 检测指定目录是否存在
        /// </summary>
        /// <param name="directoryPath">目录的绝对路径</param>
        public static bool IsExistDirectory(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }

        #endregion 检测指定目录是否存在

        #region 检测指定文件是否存在

        /// <summary>
        /// 检测指定文件是否存在,如果存在则返回true。
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        public static bool IsExistFile(string filePath)
        {
            return File.Exists(filePath);
        }

        #endregion 检测指定文件是否存在

        #region 检测指定目录是否为空

        /// <summary>
        /// 检测指定目录是否为空
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>
        public static bool IsEmptyDirectory(string directoryPath)
        {
            try
            {
                //判断是否存在文件
                string[] fileNames = GetFileNames(directoryPath);
                if (fileNames.Length > 0)
                {
                    return false;
                }
                //判断是否存在文件夹
                string[] directoryNames = GetDirectories(directoryPath);
                if (directoryNames.Length > 0)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion 检测指定目录是否为空

        #region 检测指定目录中是否存在指定的文件

        /// <summary>
        /// 检测指定目录中是否存在指定的文件,若要搜索子目录请使用重载方法.
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>
        /// <param name="searchPattern">模式字符串，"*"代表0或N个字符，"?"代表1个字符。
        /// 范例："Log*.xml"表示搜索所有以Log开头的Xml文件。</param>
        public static bool Contains(string directoryPath, string searchPattern)
        {
            try
            {
                //获取指定的文件列表
                string[] fileNames = GetFileNames(directoryPath, searchPattern, false);
                //判断指定文件是否存在
                if (fileNames.Length == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 检测指定目录中是否存在指定的文件
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>
        /// <param name="searchPattern">模式字符串，"*"代表0或N个字符，"?"代表1个字符。
        /// 范例："Log*.xml"表示搜索所有以Log开头的Xml文件。</param>
        /// <param name="isSearchChild">是否搜索子目录</param>
        public static bool Contains(string directoryPath, string searchPattern, bool isSearchChild)
        {
            try
            {
                //获取指定的文件列表
                string[] fileNames = GetFileNames(directoryPath, searchPattern, true);
                //判断指定文件是否存在
                if (fileNames.Length == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion 检测指定目录中是否存在指定的文件

        #region 创建一个目录

        /// <summary>
        /// 创建一个目录
        /// </summary>
        /// <param name="directoryPath">目录的绝对路径</param>
        public static void CreateDirectory(string directoryPath)
        {
            //如果目录不存在则创建该目录
            if (!IsExistDirectory(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        #endregion 创建一个目录

        #region 创建一个文件

        /// <summary>
        /// 创建一个文件
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        public static void CreateFile(string filePath)
        {
            try
            {
                //如果文件不存在则创建该文件
                if (!IsExistFile(filePath))
                {
                    //获取文件目录路径
                    string directoryPath = GetDirectoryFromFilePath(filePath);
                    //如果文件的目录不存在，则创建目录
                    CreateDirectory(directoryPath);
                    lock (sync)
                    {
                        //创建文件
                        using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
                        {
                        }
                    }
                }
            }
            catch
            {
            }
        }

        #endregion 创建一个文件

        #region 创建一个文件,并将字节流写入文件

        /// <summary>
        /// 创建一个文件,并将字节流写入文件。
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        /// <param name="buffer">二进制流数据</param>
        public static void CreateFile(string filePath, byte[] buffer)
        {
            try
            {
                //如果文件不存在则创建该文件
                if (!IsExistFile(filePath))
                {
                    //获取文件目录路径
                    string directoryPath = GetDirectoryFromFilePath(filePath);
                    //如果文件的目录不存在，则创建目录
                    CreateDirectory(directoryPath);
                    //创建一个FileInfo对象
                    FileInfo file = new FileInfo(filePath);
                    //创建文件
                    using (FileStream fs = file.Create())
                    {
                        //写入二进制流
                        fs.Write(buffer, 0, buffer.Length);
                    }
                }
            }
            catch
            {
            }
        }

        #endregion 创建一个文件,并将字节流写入文件

        #region 创建一个文件,并将字符串写入文件

        /// <summary>
        /// 创建一个文件,并将字符串写入文件。
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        /// <param name="text">字符串数据</param>
        public static void CreateFile(string filePath, string text)
        {
            CreateFile(filePath, text, Encoding.UTF8);
        }

        /// <summary>
        /// 创建一个文件,并将字符串写入文件。
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        /// <param name="text">字符串数据</param>
        /// <param name="encoding">字符编码</param>
        public static void CreateFile(string filePath, string text, Encoding encoding)
        {
            try
            {
                //如果文件不存在则创建该文件
                if (!IsExistFile(filePath))
                {
                    //获取文件目录路径
                    string directoryPath = GetDirectoryFromFilePath(filePath);
                    //如果文件的目录不存在，则创建目录
                    CreateDirectory(directoryPath);
                    //创建文件
                    FileInfo file = new FileInfo(filePath);
                    using (FileStream stream = file.Create())
                    {
                        using (StreamWriter writer = new StreamWriter(stream, encoding))
                        {
                            //写入字符串
                            writer.Write(text);
                            //输出
                            writer.Flush();
                        }
                    }
                }
            }
            catch
            {
            }
        }

        #endregion 创建一个文件,并将字符串写入文件

        #region 从文件绝对路径中获取目录路径

        /// <summary>
        /// 从文件绝对路径中获取目录路径
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        public static string GetDirectoryFromFilePath(string filePath)
        {
            //实例化文件
            FileInfo file = new FileInfo(filePath);
            //获取目录信息
            DirectoryInfo directory = file.Directory;
            //返回目录路径
            return directory.FullName;
        }

        #endregion 从文件绝对路径中获取目录路径

        #region 获取文本文件的行数

        /// <summary>
        /// 获取文本文件的行数
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        public static int GetLineCount(string filePath)
        {
            //创建流读取器
            using (StreamReader reader = new StreamReader(filePath))
            {
                //行数
                int i = 0;
                while (true)
                {
                    //如果读取到内容就把行数加1
                    if (reader.ReadLine() != null)
                    {
                        i++;
                    }
                    else
                    {
                        break;
                    }
                }
                //返回行数
                return i;
            }
        }

        #endregion 获取文本文件的行数

        #region 获取一个文件的长度

        /// <summary>
        /// 获取一个文件的长度,单位为Byte
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        public static int GetFileSize(string filePath)
        {
            //创建一个文件对象
            FileInfo fi = new FileInfo(filePath);
            //获取文件的大小
            return (int)fi.Length;
        }

        /// <summary>
        /// 获取一个文件的长度,单位为KB
        /// </summary>
        /// <param name="filePath">文件的路径</param>
        public static double GetFileSizeByKB(string filePath)
        {
            //创建一个文件对象
            FileInfo fi = new FileInfo(filePath);
            //获取文件的大小
            return Convert.ToDouble(Convert.ToDouble(fi.Length) / 1024);
        }

        /// <summary>
        /// 获取一个文件的长度,单位为MB
        /// </summary>
        /// <param name="filePath">文件的路径</param>
        public static double GetFileSizeByMB(string filePath)
        {
            //创建一个文件对象
            FileInfo fi = new FileInfo(filePath);
            //获取文件的大小
            return Convert.ToDouble(Convert.ToDouble(fi.Length) / 1024 / 1024);
        }

        #endregion 获取一个文件的长度

        #region 获取指定目录中的文件列表

        /// <summary>
        /// 获取指定目录中所有文件列表
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>
        public static string[] GetFileNames(string directoryPath)
        {
            //如果目录不存在，则抛出异常
            if (!IsExistDirectory(directoryPath))
            {
                throw new FileNotFoundException();
            }
            //获取文件列表
            return Directory.GetFiles(directoryPath);
        }

        /// <summary>
        /// 获取指定目录及子目录中所有文件列表
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>
        /// <param name="searchPattern">模式字符串，"*"代表0或N个字符，"?"代表1个字符。
        /// 范例："Log*.xml"表示搜索所有以Log开头的Xml文件。</param>
        /// <param name="isSearchChild">是否搜索子目录</param>
        public static string[] GetFileNames(string directoryPath, string searchPattern, bool isSearchChild)
        {
            //如果目录不存在，则抛出异常
            if (!IsExistDirectory(directoryPath))
            {
                throw new FileNotFoundException();
            }
            try
            {
                if (isSearchChild)
                {
                    return Directory.GetFiles(directoryPath, searchPattern, SearchOption.AllDirectories);
                }
                else
                {
                    return Directory.GetFiles(directoryPath, searchPattern, SearchOption.TopDirectoryOnly);
                }
            }
            catch (IOException ex)
            {
                throw ex;
            }
        }

        #endregion 获取指定目录中的文件列表

        #region 获取指定目录中的子目录列表

        /// <summary>
        /// 获取指定目录中所有子目录列表,若要搜索嵌套的子目录列表,请使用重载方法.
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>
        public static string[] GetDirectories(string directoryPath)
        {
            try
            {
                return Directory.GetDirectories(directoryPath);
            }
            catch (IOException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取指定目录及子目录中所有子目录列表
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>
        /// <param name="searchPattern">模式字符串，"*"代表0或N个字符，"?"代表1个字符。
        /// 范例："Log*.xml"表示搜索所有以Log开头的Xml文件。</param>
        /// <param name="isSearchChild">是否搜索子目录</param>
        public static string[] GetDirectories(string directoryPath, string searchPattern, bool isSearchChild)
        {
            try
            {
                if (isSearchChild)
                {
                    return Directory.GetDirectories(directoryPath, searchPattern, SearchOption.AllDirectories);
                }
                else
                {
                    return Directory.GetDirectories(directoryPath, searchPattern, SearchOption.TopDirectoryOnly);
                }
            }
            catch (IOException ex)
            {
                throw ex;
            }
        }

        #endregion 获取指定目录中的子目录列表

        #region 向文本文件写入内容

        /// <summary>
        /// 向文本文件中写入内容
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        /// <param name="text">写入的内容</param>
        public static void WriteText(string filePath, string text)
        {
            WriteText(filePath, text, Encoding.UTF8);
        }

        /// <summary>
        /// 向文本文件中写入内容
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        /// <param name="text">写入的内容</param>
        /// <param name="encoding">编码</param>
        public static void WriteText(string filePath, string text, Encoding encoding)
        {
            //向文件写入内容
            File.WriteAllText(filePath, text, encoding);
        }

        #endregion 向文本文件写入内容

        #region 向文本文件的尾部追加内容

        /// <summary>
        /// 向文本文件的尾部追加内容
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        /// <param name="text">写入的内容</param>
        public static void AppendText(string filePath, string text)
        {
            //======= 追加内容 =======
            try
            {
                lock (sync)
                {
                    //创建流写入器
                    using (StreamWriter writer = new StreamWriter(filePath, true))
                    {
                        writer.WriteLine(text);
                    }
                }
            }
            catch
            {
            }
        }

        #endregion 向文本文件的尾部追加内容

        #region 将现有文件的内容复制到新文件中

        /// <summary>
        /// 将源文件的内容复制到目标文件中
        /// </summary>
        /// <param name="sourceFilePath">源文件的绝对路径</param>
        /// <param name="destFilePath">目标文件的绝对路径</param>
        public static void CopyTo(string sourceFilePath, string destFilePath)
        {
            //有效性检测
            if (!IsExistFile(sourceFilePath))
            {
                return;
            }
            try
            {
                //检测目标文件的目录是否存在，不存在则创建
                string destDirectoryPath = GetDirectoryFromFilePath(destFilePath);
                CreateDirectory(destDirectoryPath);
                //复制文件
                FileInfo file = new FileInfo(sourceFilePath);
                file.CopyTo(destFilePath, true);
            }
            catch
            {
            }
        }

        #endregion 将现有文件的内容复制到新文件中

        #region 将文件移动到指定目录( 剪切 )

        /// <summary>
        /// 将文件移动到指定目录( 剪切 )
        /// </summary>
        /// <param name="sourceFilePath">需要移动的源文件的绝对路径</param>
        /// <param name="descDirectoryPath">移动到的目录的绝对路径</param>
        public static void MoveToDirectory(string sourceFilePath, string descDirectoryPath)
        {
            //有效性检测
            if (!IsExistFile(sourceFilePath))
            {
                return;
            }
            try
            {
                //获取源文件的名称
                string sourceFileName = GetFileName(sourceFilePath);
                //如果目标目录不存在则创建
                CreateDirectory(descDirectoryPath);
                //如果目标中存在同名文件,则删除
                if (IsExistFile(descDirectoryPath + "\\" + sourceFileName))
                {
                    DeleteFile(descDirectoryPath + "\\" + sourceFileName);
                }
                //目标文件路径
                string descFilePath;
                if (!descDirectoryPath.EndsWith(@"\"))
                {
                    descFilePath = descDirectoryPath + "\\" + sourceFileName;
                }
                else
                {
                    descFilePath = descDirectoryPath + sourceFileName;
                }
                //将文件移动到指定目录
                File.Move(sourceFilePath, descFilePath);
            }
            catch
            {
            }
        }

        #endregion 将文件移动到指定目录( 剪切 )

        #region 将文件移动到指定目录，并指定新的文件名( 剪切并改名 )

        /// <summary>
        /// 将文件移动到指定目录，并指定新的文件名( 剪切并改名 )
        /// </summary>
        /// <param name="sourceFilePath">需要移动的源文件的绝对路径</param>
        /// <param name="descFilePath">目标文件的绝对路径</param>
        public static void Move(string sourceFilePath, string descFilePath)
        {
            //有效性检测
            if (!IsExistFile(sourceFilePath))
            {
                return;
            }
            try
            {
                //获取目标文件目录
                string descDirectoryPath = GetDirectoryFromFilePath(descFilePath);
                //创建目标目录
                CreateDirectory(descDirectoryPath);
                //将文件移动到指定目录
                File.Move(sourceFilePath, descFilePath);
            }
            catch
            {
            }
        }

        #endregion 将文件移动到指定目录，并指定新的文件名( 剪切并改名 )

        #region 将流读取到缓冲区中

        /// <summary>
        /// 将流读取到缓冲区中
        /// </summary>
        /// <param name="stream">原始流</param>
        public static byte[] StreamToBytes(Stream stream)
        {
            try
            {
                //创建缓冲区
                byte[] buffer = new byte[stream.Length];
                //读取流
                stream.Read(buffer, 0, Convert.ToInt32(stream.Length));
                //返回流
                return buffer;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //关闭流
                stream.Close();
            }
        }

        #endregion 将流读取到缓冲区中

        #region 将文件读取到缓冲区中

        /// <summary>
        /// 将文件读取到缓冲区中
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        public static byte[] FileToBytes(string filePath)
        {
            //获取文件的大小
            int fileSize = GetFileSize(filePath);
            //创建一个临时缓冲区
            byte[] buffer = new byte[fileSize];
            //创建一个文件
            FileInfo file = new FileInfo(filePath);
            //创建一个文件流
            using (FileStream fs = file.Open(FileMode.Open))
            {
                //将文件流读入缓冲区
                fs.Read(buffer, 0, fileSize);
                return buffer;
            }
        }

        #endregion 将文件读取到缓冲区中

        #region 将文件读取到字符串中

        /// <summary>
        /// 将文件读取到字符串中
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        public static string FileToString(string filePath)
        {
            return FileToString(filePath, Encoding.UTF8);
        }

        /// <summary>
        /// 将文件读取到字符串中
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        /// <param name="encoding">字符编码</param>
        public static string FileToString(string filePath, Encoding encoding)
        {
            //创建流读取器
            StreamReader reader = new StreamReader(filePath, encoding);
            try
            {
                //读取流
                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //关闭流读取器
                reader.Close();
            }
        }

        #endregion 将文件读取到字符串中

        #region 从文件的绝对路径中获取文件名( 包含扩展名 )

        /// <summary>
        /// 从文件的绝对路径中获取文件名( 包含扩展名 )
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        public static string GetFileName(string filePath)
        {
            //获取文件的名称
            FileInfo fi = new FileInfo(filePath);
            return fi.Name;
        }

        #endregion 从文件的绝对路径中获取文件名( 包含扩展名 )

        #region 从文件的绝对路径中获取文件名( 不包含扩展名 )

        /// <summary>
        /// 从文件的绝对路径中获取文件名( 不包含扩展名 )
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        public static string GetFileNameNoExtension(string filePath)
        {
            //获取文件的名称
            FileInfo fi = new FileInfo(filePath);
            return fi.Name.Split('.')[0];
        }

        #endregion 从文件的绝对路径中获取文件名( 不包含扩展名 )

        #region 从文件的绝对路径中获取扩展名

        /// <summary>
        /// 从文件的绝对路径中获取扩展名
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        public static string GetExtension(string filePath)
        {
            //获取文件的名称
            FileInfo fi = new FileInfo(filePath);
            return fi.Extension;
        }

        #endregion 从文件的绝对路径中获取扩展名

        #region 清空指定目录

        /// <summary>
        /// 清空指定目录下所有文件及子目录,但该目录依然保存.
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>
        public static void ClearDirectory(string directoryPath)
        {
            if (IsExistDirectory(directoryPath))
            {
                //删除目录中所有的文件
                string[] fileNames = GetFileNames(directoryPath);
                for (int i = 0; i < fileNames.Length; i++)
                {
                    DeleteFile(fileNames[i]);
                }
                //删除目录中所有的子目录
                string[] directoryNames = GetDirectories(directoryPath);
                for (int i = 0; i < directoryNames.Length; i++)
                {
                    DeleteDirectory(directoryNames[i]);
                }
            }
        }

        #endregion 清空指定目录

        #region 清空文件内容

        /// <summary>
        /// 清空文件内容
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        public static void ClearFile(string filePath)
        {
            //删除文件
            File.Delete(filePath);
            //重新创建该文件
            CreateFile(filePath);
        }

        #endregion 清空文件内容

        #region 删除指定文件

        /// <summary>
        /// 删除指定文件
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        public static void DeleteFile(string filePath)
        {
            if (IsExistFile(filePath))
            {
                File.Delete(filePath);
            }
        }

        #endregion 删除指定文件

        #region 删除指定目录

        /// <summary>
        /// 删除指定目录及其所有子目录
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>
        public static void DeleteDirectory(string directoryPath)
        {
            if (IsExistDirectory(directoryPath))
            {
                Directory.Delete(directoryPath, true);
            }
        }

        #endregion 删除指定目录

        #region 根据路径得到文件流

        /// <summary>
        /// 根据路径得到文件流
        /// </summary>
        /// <param name="strFilePath"></param>
        /// <param name="strValue"></param>
        public static byte[] GetFileSream(string Path)
        {
            byte[] buffer = null;
            using (FileStream stream = new FileInfo(Path).OpenRead())
            {
                buffer = new byte[stream.Length];
                stream.Read(buffer, 0, Convert.ToInt32(stream.Length));
            }
            return buffer;
        }

        #endregion 根据路径得到文件流

        #region 按数顺序合并URL

        /// <summary>
        /// 按数顺序合并URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string MergeUrl(params string[] urls)
        {
            if (urls == null || urls.Length == 0)
            {
                return null;
            }
            else if (urls.Length == 1)
            {
                return urls[0];
            }
            StringBuilder reval = new StringBuilder();
            int i = 0;
            char slash = '\\';
            if (!urls.Any(it => it.Contains(slash.ToString())))
            {
                slash = '/';
            }
            foreach (var url in urls)
            {
                string itUrl = url;
                var isFirst = i == 0;
                var isLast = i == urls.Length - 1;
                if (!isFirst)
                {
                    itUrl = itUrl.TrimStart(slash);
                }
                if (!isLast)
                {
                    itUrl = url.TrimEnd(slash) + slash;
                }
                ++i;
                reval.Append(itUrl);
                itUrl = null;
            }
            return reval.ToString();
        }

        #endregion 按数顺序合并URL
    }
}