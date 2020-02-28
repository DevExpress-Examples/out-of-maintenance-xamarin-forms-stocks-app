using System;
using System.Reflection;

namespace Stocks.Models {
    public static class Extensions {
        public static string GetDescription(this object target) {
            Type targetType = target.GetType();
            MemberInfo[] memberInfo = targetType.GetMember(target.ToString());
            if ((memberInfo == null) || (memberInfo.Length <= 0)) {
                return target.ToString();
            }
            object[] attrs = memberInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            if ((attrs == null) || (attrs.Length <= 0)) {
                return target.ToString();
            }
            return ((System.ComponentModel.DescriptionAttribute)attrs[0]).Description;
        }
    }
}
