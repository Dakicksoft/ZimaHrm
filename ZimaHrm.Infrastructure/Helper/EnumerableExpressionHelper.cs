using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ZimaHrm.Core.Infrastructure.Helper
{
  public static class EnumerableExpressionHelper
  {
    public static Expression<Func<TSource, String>> CreateEnumToStringExpression<TSource, TMember>(
        Expression<Func<TSource, TMember>> memberAccess, string defaultValue = "")
    {
      var type = typeof(TMember);
      if (!type.IsEnum)
      {
        throw new InvalidOperationException("TMember must be an Enum type");
      }

      var enumNames = Enum.GetNames(type);
      var enumValues = (TMember[])Enum.GetValues(type);

      var inner = (Expression)Expression.Constant(defaultValue);

      var parameter = memberAccess.Parameters[0];

      for (int i = 0; i < enumValues.Length; i++)
      {
        inner = Expression.Condition(
        Expression.Equal(memberAccess.Body, Expression.Constant(enumValues[i])),
        Expression.Constant(enumNames[i]),
        inner);
      }

      var expression = Expression.Lambda<Func<TSource, String>>(inner, parameter);

      return expression;
    }
  }
}
