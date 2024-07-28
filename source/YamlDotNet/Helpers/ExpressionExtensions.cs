// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Helpers.ExpressionExtensions
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace YamlDotNet.Helpers
{
  public static class ExpressionExtensions
  {
    public static PropertyInfo AsProperty(this LambdaExpression propertyAccessor) => ExpressionExtensions.TryGetMemberExpression<PropertyInfo>(propertyAccessor) ?? throw new ArgumentException("Expected a lambda expression in the form: x => x.SomeProperty", nameof (propertyAccessor));

    private static TMemberInfo TryGetMemberExpression<TMemberInfo>(LambdaExpression lambdaExpression) where TMemberInfo : MemberInfo
    {
      if (lambdaExpression.Parameters.Count != 1)
        return default (TMemberInfo);
      Expression expression = lambdaExpression.Body;
      if (expression is UnaryExpression unaryExpression)
      {
        if (unaryExpression.NodeType != ExpressionType.Convert)
          return default (TMemberInfo);
        expression = unaryExpression.Operand;
      }
      if (!(expression is MemberExpression memberExpression))
        return default (TMemberInfo);
      return memberExpression.Expression != lambdaExpression.Parameters[0] ? default (TMemberInfo) : memberExpression.Member as TMemberInfo;
    }
  }
}
