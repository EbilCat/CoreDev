using System;
using System.Linq.Expressions;
using UnityEngine;

namespace CoreDev.Extensions
{
    public static class TypeExtensions
    {
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
