using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CoreDev.Extensions
{
    public static class TypeExtensions
    {
        public static void GetTypes(this Type currentType, List<Type> typeList)
        {
            typeList.Clear();
            GetType_Recursive(currentType, typeList);
            typeList.AddRange(currentType.GetInterfaces());
        }

        private static void GetType_Recursive(this Type currentType, List<Type> typeList)
        {
            if(currentType == null) { return; }
            typeList.Add(currentType);
            GetType_Recursive(currentType.BaseType, typeList);
        }

        public static bool IsSubclassOfRawGeneric(this Type toCheck, Type generic)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur) { return true; }
                toCheck = toCheck.BaseType;
            }
            return false;
        }

        public static string GetMemberName<T, TValue>(this T type, Expression<Func<T, TValue>> memberAccess)
        {
            MemberExpression memberExpression = (MemberExpression)memberAccess.Body;
            string memberName = memberExpression.Member.Name;
            return memberName;
        }
    }
}
