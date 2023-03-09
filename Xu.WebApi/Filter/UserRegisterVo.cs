using FluentValidation;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Xu.WebApi
{
    public class UserRegisterVo
    {
        public string WxUid { get; set; }

        public string Telphone { get; set; }

        public string NickName { get; set; }

        public string SourceType { get; set; }
        public IEnumerable<CarInfo> Cars { get; set; }
    }

    public class CarInfo
    {
        public int CarCount { get; set; }
        public int CarSize { get; set; }
    }

    public class UserRegisterVoValidator : AbstractValidator<UserRegisterVo>
    {
        public UserRegisterVoValidator()
        {
            When(x => !string.IsNullOrEmpty(x.NickName) || !string.IsNullOrEmpty(x.Telphone), () =>
            {
                RuleFor(x => x.NickName)
                    .Must(e => IsLegalName(e)).WithMessage("请填写合法的姓名，必须是汉字和字母");
                RuleFor(x => x.Telphone)
                    .Must(e => IsLegalPhone(e)).WithMessage("请填写正确的手机号码");
                RuleFor(x => x.Cars)
                    .NotNull().NotEmpty().WithMessage("车辆信息不正确");
                RuleForEach(x => x.Cars).SetValidator(new CarInfoValidator());
            });
        }

        /// <summary>
        /// 判断用户名是否合法
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static bool IsLegalName(string username)
        {
            //判断用户名是否合法
            const string pattern = "(^([A-Za-z]|[\u4E00-\u9FA5]){1,10}$)";
            return (!string.IsNullOrEmpty(username) && Regex.IsMatch(username, pattern));
        }

        /// <summary>
        /// 判断手机号
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static bool IsLegalPhone(string phone)
        {
            const string pattern = "(^1\\d{10}$)";
            return (!string.IsNullOrEmpty(phone) && Regex.IsMatch(phone, pattern));
        }
    }

    public class CarInfoValidator : AbstractValidator<CarInfo>
    {
        public CarInfoValidator()
        {
            RuleFor(x => x.CarCount)
                .GreaterThanOrEqualTo(0).WithMessage("车辆数量必须大于等于0")
                .LessThanOrEqualTo(500).WithMessage($"存在车型数量已达上限");
            RuleFor(x => x.CarSize)
                .IsInEnum().WithMessage("车型不正确");
        }
    }
}