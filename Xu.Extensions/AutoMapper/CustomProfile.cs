using AutoMapper;
using Xu.Model;
using Xu.Model.Models;
using Xu.Model.ViewModels;
using Xu.Model.XmlModels;

namespace Xu.Extensions
{
    public class CustomProfile : Profile
    {
        // Profile不知有什么用，通过百度了解才了解是services.AddAutoMapper是会自动找到所有继承了Profile的类然后进行配置，
        // 而且我的这个配置文件是在api层的，如果Profile配置类放在别的层（比如Service层），
        // 如果没解耦的话，可以services.AddAutoMapper()，参数留空，AutoMapper会从所有引用的程序集里找继承Profile的类，如果解耦了，就得services.AddAutoMapper(Assembly.Load("Xu.Core.Service"))。

        /// <summary>
        /// 配置构造函数，用来创建关系映射
        /// </summary>
        public CustomProfile()
        {
            CreateMap<Menu, InfoMenu>(); //第一个参数是原对象，第二个是目的对象
            CreateMap<Role, InfoRole>();
            CreateMap<Systems, InfoSystem>();

            CreateMap<User, LoginViewModel>()
                .ForMember(d => d.RoleInfoList, s => s.Ignore())
                .ForMember(d => d.DeptInfoList, s => s.Ignore())
                .ForMember(d => d.MenuInfoList, s => s.Ignore());

            //接口返回映射类
            CreateMap<Role, RoleViewModel>();
            CreateMap<Menu, MenuViewModel>();
            CreateMap<Dept, DeptViewModel>();
            CreateMap<Systems, SystemViewModel>();

            //在使用当中，就这一句话完全搞定所有转换：XuViewModels models = IMapper.Map<XuViewModels>(XuArticle);
        }
    }
}