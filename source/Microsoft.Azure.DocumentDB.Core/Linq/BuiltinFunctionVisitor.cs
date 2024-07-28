// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Linq.BuiltinFunctionVisitor
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Spatial;
using Microsoft.Azure.Documents.Sql;
using Microsoft.Azure.Documents.SystemFunctions;
using System;
using System.Globalization;
using System.Linq.Expressions;

namespace Microsoft.Azure.Documents.Linq
{
  internal abstract class BuiltinFunctionVisitor
  {
    public SqlScalarExpression Visit(
      MethodCallExpression methodCallExpression,
      TranslationContext context)
    {
      SqlScalarExpression scalarExpression = this.VisitExplicit(methodCallExpression, context);
      if (scalarExpression != null)
        return scalarExpression;
      return this.VisitImplicit(methodCallExpression, context) ?? throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ClientResources.MethodNotSupported, (object) methodCallExpression.Method.Name));
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
        if ((object) methodCallExpression.Method.DeclaringType.GeUnderlyingSystemType() == (object) typeof (TypeCheckFunctionsExtensions))
          return TypeCheckFunctions.Visit(methodCallExpression, context);
      }
      else
        type = methodCallExpression.Method.DeclaringType;
      if ((object) type == (object) typeof (Math))
        return MathBuiltinFunctions.Visit(methodCallExpression, context);
      if ((object) type == (object) typeof (string))
        return StringBuiltinFunctions.Visit(methodCallExpression, context);
      if (type.IsEnumerable())
        return ArrayBuiltinFunctions.Visit(methodCallExpression, context);
      if (CustomTypeExtensions.IsAssignableFrom(typeof (Geometry), type) || (object) methodCallExpression.Method.DeclaringType == (object) typeof (GeometryOperationExtensions))
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
