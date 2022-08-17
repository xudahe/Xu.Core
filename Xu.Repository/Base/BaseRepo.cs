using SqlSugar;
using SqlSugar.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xu.Common;
using Xu.IRepository;
using Xu.Model.ResultModel;

namespace Xu.Repository
{
    public class BaseRepo<T> : IBaseRepo<T> where T : class, new()
    {
        private readonly SqlSugarScope _dbBase;

        private ISqlSugarClient _db
        {
            get
            {
                /* 如果要开启多库支持，
                 * 1、在appsettings.json 中开启MutiDBEnabled节点为true，必填
                 * 2、设置一个主连接的数据库ID，节点MainDB，对应的连接字符串的Enabled也必须true，必填
                 */
                if (Appsettings.App(new string[] { "MutiDBEnabled" }).ToBoolReq())
                {
                    if (typeof(T).GetTypeInfo().GetCustomAttributes(typeof(SugarTable), true).FirstOrDefault((x => x.GetType() == typeof(SugarTable))) is SugarTable sugarTable && !string.IsNullOrEmpty(sugarTable.TableDescription))
                    {
                        _dbBase.ChangeDatabase(sugarTable.TableDescription.ToLower());
                    }
                    else
                    {
                        _dbBase.ChangeDatabase(MainDb.CurrentDbConnId.ToLower()); //切换数据库 ConfigId = 0
                    }
                }

                return _dbBase;
            }
        }

        public ISqlSugarClient Db
        {
            get { return _db; }
        }

        /// <summary>
        /// 构造函数，通过 unitofwork，来控制sqlsugar 实例
        /// 注入工作单元接口，用来将 sqlsugar 实例统一起来，这样就能保证每次一个scope ，都能是同一个实例。
        /// 不是每次都 new，通过工作单元来控制：
        /// </summary>
        /// <param name="unitOfWork"></param>
        public BaseRepo(IUnitOfWork unitOfWork)
        {
            _dbBase = unitOfWork.GetDbClient();
        }

        /// <summary>
        /// 根据ID查询一条数据
        /// </summary>
        /// <param name="objId"></param>
        /// <returns>数据实体</returns>
        public async Task<T> QueryById(object objId)
        {
            return await _db.Queryable<T>().In(objId).SingleAsync();
        }

        /// <summary>
        /// 根据ID查询一条数据
        /// </summary>
        /// <param name="objId">id（必须指定主键特性 [SugarColumn(IsPrimaryKey=true)]），如果是联合主键，请使用Where条件</param>
        /// <param name="blnUseCache">是否使用缓存</param>
        /// <returns>数据实体</returns>
        public async Task<T> QueryById(object objId, bool blnUseCache = false)
        {
            return await _db.Queryable<T>().WithCacheIF(blnUseCache).In(objId).SingleAsync();
        }

        /// <summary>
        /// 根据ID集合批量查询数据
        /// </summary>
        /// <param name="lstIds">id列表（必须指定主键特性 [SugarColumn(IsPrimaryKey=true)]），如果是联合主键，请使用Where条件</param>
        /// <returns>数据实体列表</returns>
        public async Task<List<T>> QueryByIds(object[] lstIds)
        {
            return await _db.Queryable<T>().In(lstIds).ToListAsync();
        }

        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns>返回实体Id</returns>
        public async Task<int> Add(T entity)
        {
            var insert = _db.Insertable(entity);
            return await insert.ExecuteReturnIdentityAsync();
            //return await insert.ExecuteCommandAsync();

            //这里你可以返回TEntity，这样的话就可以获取id值，无论主键是什么类型
            //var return3 = await insert.ExecuteReturnEntityAsync();
        }

        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <param name="insertColumns">指定只插入列</param>
        /// <returns>返回自增量列</returns>
        public async Task<int> Add(T entity, Expression<Func<T, object>> insertColumns = null)
        {
            var insert = _db.Insertable(entity);
            if (insertColumns == null)
            {
                return await insert.ExecuteReturnIdentityAsync();
            }
            else
            {
                return await insert.InsertColumns(insertColumns).ExecuteReturnIdentityAsync();
            }
        }

        /// <summary>
        /// 批量插入实体(速度快)
        /// </summary>
        /// <param name="listEntity">实体集合</param>
        /// <returns>影响行数</returns>
        public async Task<int> Add(List<T> listEntity)
        {
            return await _db.Insertable(listEntity.ToArray()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        public async Task<bool> Update(T entity)
        {
            //这种方式会以主键为条件
            return await _db.Updateable(entity).ExecuteCommandHasChangeAsync();
        }

        public async Task<bool> Update(T entity, string strWhere)
        {
            return await _db.Updateable(entity).Where(strWhere).ExecuteCommandHasChangeAsync();
        }

        public async Task<bool> Update(string strSql, SugarParameter[] parameters = null)
        {
            return await _db.Ado.ExecuteCommandAsync(strSql, parameters) > 0;
        }

        public async Task<bool> Update(object operateAnonymousObjects)
        {
            return await _db.Updateable<T>(operateAnonymousObjects).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Update(T entity, List<string> lstColumns = null, List<string> lstIgnoreColumns = null, string strWhere = "")
        {
            IUpdateable<T> up = _db.Updateable(entity);
            if (lstIgnoreColumns != null && lstIgnoreColumns.Count > 0)
            {
                up = up.IgnoreColumns(lstIgnoreColumns.ToArray());
            }
            if (lstColumns != null && lstColumns.Count > 0)
            {
                up = up.UpdateColumns(lstColumns.ToArray());
            }
            if (!string.IsNullOrEmpty(strWhere))
            {
                up = up.Where(strWhere);
            }
            return await up.ExecuteCommandHasChangeAsync();
        }

        /// <summary>
        /// 根据实体删除一条数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        public async Task<bool> Delete(T entity)
        {
            return await _db.Deleteable(entity).ExecuteCommandHasChangeAsync();
        }

        /// <summary>
        /// 根据Id删除一条数据
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public async Task<bool> DeleteById(object id)
        {
            return await _db.Deleteable<T>(id).ExecuteCommandHasChangeAsync();
        }

        /// <summary>
        /// 删除指定ID集合的数据(批量删除)
        /// </summary>
        /// <param name="ids">主键ID集合</param>
        /// <returns></returns>
        public async Task<bool> DeleteByIds(object[] ids)
        {
            return await _db.Deleteable<T>().In(ids).ExecuteCommandHasChangeAsync();
        }

        /// <summary>
        /// 查询所有数据
        /// </summary>
        /// <returns>数据列表</returns>
        public async Task<List<T>> Query()
        {
            return await _db.Queryable<T>().ToListAsync();
        }

        /// <summary>
        /// 查询数据列表
        /// </summary>
        /// <param name="strWhere">条件</param>
        /// <returns>数据列表</returns>
        public async Task<List<T>> Query(string strWhere)
        {
            return await _db.Queryable<T>().WhereIF(!string.IsNullOrEmpty(strWhere), strWhere).ToListAsync();
        }

        /// <summary>
        /// 查询数据列表
        /// </summary>
        /// <param name="whereExpression">whereExpression</param>
        /// <returns>数据列表</returns>
        public async Task<List<T>> Query(Expression<Func<T, bool>> whereExpression)
        {
            return await _db.Queryable<T>().WhereIF(whereExpression != null, whereExpression).ToListAsync();
        }

        /// <summary>
        /// 查询一个列表
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<List<T>> Query(Expression<Func<T, bool>> whereExpression, string strOrderByFileds)
        {
            return await _db.Queryable<T>().WhereIF(whereExpression != null, whereExpression).OrderByIF(strOrderByFileds != null, strOrderByFileds).ToListAsync();
        }

        /// <summary>
        /// 查询一个列表
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <param name="orderByExpression"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        public async Task<List<T>> Query(Expression<Func<T, bool>> whereExpression, Expression<Func<T, object>> orderByExpression, bool isAsc = true)
        {
            return await _db.Queryable<T>().OrderByIF(orderByExpression != null, orderByExpression, isAsc ? OrderByType.Asc : OrderByType.Desc).WhereIF(whereExpression != null, whereExpression).ToListAsync();
        }

        /// <summary>
        /// 查询一个列表
        /// </summary>
        /// <param name="strWhere">条件</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<List<T>> Query(string strWhere, string strOrderByFileds)
        {
            return await _db.Queryable<T>().OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds).WhereIF(!string.IsNullOrEmpty(strWhere), strWhere).ToListAsync();
        }

        /// <summary>
        /// 查询前N条数据
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="intTop">前N条</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<List<T>> Query(Expression<Func<T, bool>> whereExpression, int intTop, string strOrderByFileds)
        {
            return await _db.Queryable<T>().OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds).WhereIF(whereExpression != null, whereExpression).Take(intTop).ToListAsync();
        }

        /// <summary>
        /// 查询前N条数据
        /// </summary>
        /// <param name="strWhere">条件</param>
        /// <param name="intTop">前N条</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<List<T>> Query(string strWhere, int intTop, string strOrderByFileds)
        {
            return await _db.Queryable<T>().OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds).WhereIF(!string.IsNullOrEmpty(strWhere), strWhere).Take(intTop).ToListAsync();
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="intPageIndex">页码（下标0）</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="intTotalCount">数据总量</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<List<T>> Query(Expression<Func<T, bool>> whereExpression, int intPageIndex, int intPageSize, string strOrderByFileds)
        {
            return await _db.Queryable<T>().OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds).WhereIF(whereExpression != null, whereExpression).ToPageListAsync(intPageIndex, intPageSize);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="strWhere">条件</param>
        /// <param name="intPageIndex">页码（下标0）</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="intTotalCount">数据总量</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<List<T>> Query(string strWhere, int intPageIndex, int intPageSize, string strOrderByFileds)
        {
            return await _db.Queryable<T>().OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds).WhereIF(!string.IsNullOrEmpty(strWhere), strWhere).ToPageListAsync(intPageIndex, intPageSize);
        }

        /// <summary>
        /// 分页查询[使用版本，其他分页未测试]
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="intPageIndex">页码（下标0）</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns></returns>
        public async Task<PageModel<T>> QueryPage(Expression<Func<T, bool>> whereExpression, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null)
        {
            RefAsync<int> totalCount = 0;
            var list = await _db.Queryable<T>()
             .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
             .WhereIF(whereExpression != null, whereExpression)
             .ToPageListAsync(intPageIndex, intPageSize, totalCount);

            int pageCount = Math.Ceiling(totalCount.ObjToDecimal() / intPageSize.ObjToDecimal()).ObjToInt();
            return new PageModel<T>() { DataCount = totalCount, PageCount = pageCount, Page = intPageIndex, PageSize = intPageSize, Data = list };
        }

        /// <summary>
        ///查询-多表查询
        /// </summary>
        /// <typeparam name="T1">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式 (join1,join2) => new object[] {JoinType.Left,join1.UserNo==join2.UserNo}</param>
        /// <param name="selectExpression">返回表达式 (s1, s2) => new { Id =s1.UserNo, Id1 = s2.UserNo}</param>
        /// <param name="whereLambda">查询表达式 (w1, w2) =>w1.UserNo == "")</param>
        /// <returns>值</returns>
        public async Task<List<TResult>> QueryMuch<T1, T2, TResult>(
            Expression<Func<T1, T2, object[]>> joinExpression,
            Expression<Func<T1, T2, TResult>> selectExpression,
            Expression<Func<T1, T2, bool>> whereLambda = null) where T1 : class, new()
        {
            if (whereLambda == null)
            {
                return await _db.Queryable(joinExpression).Select(selectExpression).ToListAsync();
            }
            return await _db.Queryable(joinExpression).Where(whereLambda).Select(selectExpression).ToListAsync();
        }

        /// <summary>
        ///查询-多表查询
        /// </summary>
        /// <typeparam name="T1">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式 (join1,join2) => new object[] {JoinType.Left,join1.UserNo==join2.UserNo}</param>
        /// <param name="selectExpression">返回表达式 (s1, s2) => new { Id =s1.UserNo, Id1 = s2.UserNo}</param>
        /// <param name="whereLambda">查询表达式 (w1, w2) =>w1.UserNo == "")</param>
        /// <returns>值</returns>
        public async Task<List<TResult>> QueryMuch<T1, T2, T3, TResult>(
            Expression<Func<T1, T2, T3, object[]>> joinExpression,
            Expression<Func<T1, T2, T3, TResult>> selectExpression,
            Expression<Func<T1, T2, T3, bool>> whereLambda = null) where T1 : class, new()
        {
            if (whereLambda == null)
            {
                return await _db.Queryable(joinExpression).Select(selectExpression).ToListAsync();
            }
            return await _db.Queryable(joinExpression).Where(whereLambda).Select(selectExpression).ToListAsync();
        }
    }
}