// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Linq.BuiltinFunctionVisitor
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Spatial;
using Microsoft.Azure.Cosmos.SqlObjects;
using Microsoft.Azure.Documents;
using System;
using System.Globalization;
using System.Linq.Expressions;

namespace Microsoft.Azure.Cosmos.Linq
{
  internal abstract class BuiltinFunctionVisitor
  {
    public SqlScalarExpression Visit(
      MethodCallExpression methodCallExpression,
      TranslationContext context)
    {
      SqlScalarExpression scalarExpression1 = this.VisitExplicit(methodCallExpression, context);
      if ((SqlObject) scalarExpression1 != (SqlObject) null)
        return scalarExpression1;
      SqlScalarExpression scalarExpression2 = this.VisitImplicit(methodCallExpression, context);
      return (SqlObject) scalarExpression2 != (SqlObject) null ? scalarExpression2 : throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ClientResources.MethodNotSupported, (object) methodCallExpression.Method.Name));
    }

    public static SqlScalarExpression VisitBuiltinFunctionCall(
      MethodCallExpression methodCallExpression,
      TranslationContext context)
    {
      Type type;
      if (methodCallExpression.Method.IsStatic && methodCallExpression.Method.IsExtensionMethod())
      {
        if (methodCallExpression.Arguments.Count < 1)
          throw new ArgumentException();
        type = methodCallExpression.Arguments[0].Type;
        if (methodCallExpression.Method.DeclaringType.GeUnderlyingSystemType() == typeof (CosmosLinqExtensions))
          return TypeCheckFunctions.Visit(methodCallExpression, context);
      }
      else
        type = methodCallExpression.Method.DeclaringType;
      if (type == typeof (Math))
        return MathBuiltinFunctions.Visit(methodCallExpression, context);
      if (type == typeof (string))
        return StringBuiltinFunctions.Visit(methodCallExpression, context);
      if (type.IsEnumerable())
        return ArrayBuiltinFunctions.Visit(methodCallExpression, context);
      if (typeof (Geometry).IsAssignableFrom(type))
        return SpatialBuiltinFunctions.Visit(methodCallExpression, context);
      if (methodCallExpression.Method.Name == "ToString" && methodCallExpression.Arguments.Count == 0 && methodCallExpression.Object != null)
        return ExpressionToSql.VisitNonSubqueryScalarExpression(methodCallExpression.Object, context);
      throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ClientResources.MethodNotSupported, (object) methodCallExpression.Method.Name));
    }

    protected abstract SqlScalarExpression VisitExplicit(
      MethodCallExpression methodCallExpression,
      TranslationContext context);

    protected abstract SqlScalarExpression VisitImplicit(
      MethodCallExpression methodCallExpression,
      TranslationContext context);
  }
}
