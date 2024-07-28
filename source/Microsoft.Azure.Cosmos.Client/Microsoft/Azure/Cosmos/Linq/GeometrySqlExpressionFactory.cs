// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Linq.GeometrySqlExpressionFactory
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Spatial;
using Microsoft.Azure.Cosmos.SqlObjects;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.Azure.Cosmos.Linq
{
  internal static class GeometrySqlExpressionFactory
  {
    public static SqlScalarExpression Construct(Expression geometryExpression)
    {
      if (!typeof (Geometry).IsAssignableFrom(geometryExpression.Type))
        throw new ArgumentException(nameof (geometryExpression));
      if (geometryExpression.NodeType == ExpressionType.Constant)
        return GeometrySqlExpressionFactory.FromJToken((JToken) JObject.FromObject(((ConstantExpression) geometryExpression).Value));
      Geometry o;
      try
      {
        o = Expression.Lambda<Func<Geometry>>(geometryExpression).Compile()();
      }
      catch (Exception ex)
      {
        throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ClientResources.FailedToEvaluateSpatialExpression), ex);
      }
      return GeometrySqlExpressionFactory.FromJToken((JToken) JObject.FromObject((object) o));
    }

    private static SqlScalarExpression FromJToken(JToken jToken)
    {
      switch (jToken.Type)
      {
        case JTokenType.Object:
          return (SqlScalarExpression) SqlObjectCreateScalarExpression.Create(((JObject) jToken).Properties().Select<JProperty, SqlObjectProperty>((Func<JProperty, SqlObjectProperty>) (p => SqlObjectProperty.Create(SqlPropertyName.Create(p.Name), GeometrySqlExpressionFactory.FromJToken(p.Value)))).ToArray<SqlObjectProperty>());
        case JTokenType.Array:
          return (SqlScalarExpression) SqlArrayCreateScalarExpression.Create(jToken.Select<JToken, SqlScalarExpression>(new Func<JToken, SqlScalarExpression>(GeometrySqlExpressionFactory.FromJToken)).ToArray<SqlScalarExpression>());
        case JTokenType.Integer:
        case JTokenType.Float:
          return (SqlScalarExpression) SqlLiteralScalarExpression.Create((SqlLiteral) SqlNumberLiteral.Create((Number64) jToken.Value<double>()));
        case JTokenType.String:
          return (SqlScalarExpression) SqlLiteralScalarExpression.Create((SqlLiteral) SqlStringLiteral.Create(jToken.Value<string>()));
        case JTokenType.Boolean:
          return (SqlScalarExpression) SqlLiteralScalarExpression.Create((SqlLiteral) SqlBooleanLiteral.Create(jToken.Value<bool>()));
        case JTokenType.Null:
          return (SqlScalarExpression) SqlLiteralScalarExpression.SqlNullLiteralScalarExpression;
        default:
          throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ClientResources.UnexpectedTokenType, (object) jToken.Type));
      }
    }
  }
}
