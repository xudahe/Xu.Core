using System;

namespace Xu.Model
{
    public class FrameSeed
    {
        /// <summary>
        /// 生成Model层
        /// </summary>
        /// <param name="myContext"></param>
        /// <returns></returns>
        public static bool CreateModels(MyContext myContext)
        {
            try
            {
                myContext.Create_Model_ClassFileByDBTalbe($@"E:\demo-core\demo3\Xu.Core\Xu.Model\Models", "Xu.Model", new string[] { }, "");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 生成IRepository层
        /// </summary>
        /// <param name="myContext"></param>
        /// <returns></returns>
        public static bool CreateIRepositorys(MyContext myContext)
        {
            try
            {
                myContext.Create_IRepository_ClassFileByDBTalbe($@"E:\demo-core\demo3\Xu.Core\Xu.IRepository", "Xu.IRepository", new string[] { }, "");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 生成 IService 层
        /// </summary>
        /// <param name="myContext"></param>
        /// <returns></returns>
        public static bool CreateIServices(MyContext myContext)
        {
            try
            {
                myContext.Create_IServices_ClassFileByDBTalbe($@"E:\demo-core\demo3\Xu.Core\Xu.IServices", "Xu.IServices", new string[] { "Module" }, "");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 生成 Repository 层
        /// </summary>
        /// <param name="myContext"></param>
        /// <returns></returns>
        public static bool CreateRepository(MyContext myContext)
        {
            try
            {
                myContext.Create_Repository_ClassFileByDBTalbe($@"E:\demo-core\demo3\Xu.Core\Xu.Repository", "Xu.Repository", new string[] { "Module" }, "");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 生成 Repository 层
        /// </summary>
        /// <param name="myContext"></param>
        /// <returns></returns>
        public static bool CreateServices(MyContext myContext)
        {
            try
            {
                myContext.Create_Repository_ClassFileByDBTalbe($@"E:\demo-core\demo3\Xu.Core\Xu.Services", "Xu.Services", new string[] { "Module" }, "");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}