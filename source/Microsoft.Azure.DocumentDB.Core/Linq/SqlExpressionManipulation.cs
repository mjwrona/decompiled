// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Linq.SqlExpressionManipulation
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Sql;
using System;
using System.Linq;

namespace Microsoft.Azure.Documents.Linq
{
  internal static class SqlExpressionManipulation
  {
    public static SqlScalarExpression Substitute(
      SqlScalarExpression replacement,
      SqlIdentifier toReplace,
      SqlScalarExpression into)
    {
      if (into == null)
        return (SqlScalarExpression) null;
      if (replacement == null)
        throw new ArgumentNullException(nameof (replacement));
      switch (into.Kind)
      {
        case SqlObjectKind.ArrayCreateScalarExpression:
          if (!(into is SqlArrayCreateScalarExpression scalarExpression1))
            throw new DocumentQueryException("Expected a SqlArrayCreateScalarExpression, got a " + (object) into.GetType());
          SqlScalarExpression[] scalarExpressionArray1 = new SqlScalarExpression[scalarExpression1.Items.Count];
          for (int index = 0; index < scalarExpressionArray1.Length; ++index)
          {
            SqlScalarExpression into1 = scalarExpression1.Items[index];
            SqlScalarExpression scalarExpression2 = SqlExpressionManipulation.Substitute(replacement, toReplace, into1);
            scalarExpressionArray1[index] = scalarExpression2;
          }
          return (SqlScalarExpression) SqlArrayCreateScalarExpression.Create(scalarExpressionArray1);
        case SqlObjectKind.BinaryScalarExpression:
          if (!(into is SqlBinaryScalarExpression scalarExpression3))
            throw new DocumentQueryException("Expected a BinaryScalarExpression, got a " + (object) into.GetType());
          SqlScalarExpression leftExpression = SqlExpressionManipulation.Substitute(replacement, toReplace, scalarExpression3.LeftExpression);
          SqlScalarExpression rightExpression = SqlExpressionManipulation.Substitute(replacement, toReplace, scalarExpression3.RightExpression);
          return (SqlScalarExpression) SqlBinaryScalarExpression.Create(scalarExpression3.OperatorKind, leftExpression, rightExpression);
        case SqlObjectKind.ConditionalScalarExpression:
          SqlConditionalScalarExpression scalarExpression4 = (SqlConditionalScalarExpression) into;
          if (scalarExpression4 == null)
            throw new ArgumentException();
          SqlScalarExpression condition = SqlExpressionManipulation.Substitute(replacement, toReplace, scalarExpression4.ConditionExpression);
          SqlScalarExpression scalarExpression5 = SqlExpressionManipulation.Substitute(replacement, toReplace, scalarExpression4.FirstExpression);
          SqlScalarExpression scalarExpression6 = SqlExpressionManipulation.Substitute(replacement, toReplace, scalarExpression4.SecondExpression);
          SqlScalarExpression first = scalarExpression5;
          SqlScalarExpression second = scalarExpression6;
          return (SqlScalarExpression) SqlConditionalScalarExpression.Create(condition, first, second);
        case SqlObjectKind.FunctionCallScalarExpression:
          if (!(into is SqlFunctionCallScalarExpression scalarExpression7))
            throw new DocumentQueryException("Expected a SqlFunctionCallScalarExpression, got a " + (object) into.GetType());
          SqlScalarExpression[] scalarExpressionArray2 = new SqlScalarExpression[scalarExpression7.Arguments.Count];
          for (int index = 0; index < scalarExpressionArray2.Length; ++index)
          {
            SqlScalarExpression into2 = scalarExpression7.Arguments[index];
            SqlScalarExpression scalarExpression8 = SqlExpressionManipulation.Substitute(replacement, toReplace, into2);
            scalarExpressionArray2[index] = scalarExpression8;
          }
          return (SqlScalarExpression) SqlFunctionCallScalarExpression.Create(scalarExpression7.Name, scalarExpression7.IsUdf, scalarExpressionArray2);
        case SqlObjectKind.InScalarExpression:
          SqlInScalarExpression scalarExpression9 = (SqlInScalarExpression) into;
          if (scalarExpression9 == null)
            throw new ArgumentException();
          SqlScalarExpression expression1 = SqlExpressionManipulation.Substitute(replacement, toReplace, scalarExpression9.Expression);
          SqlScalarExpression[] scalarExpressionArray3 = new SqlScalarExpression[scalarExpression9.Items.Count];
          for (int index = 0; index < scalarExpressionArray3.Length; ++index)
            scalarExpressionArray3[index] = SqlExpressionManipulation.Substitute(replacement, toReplace, scalarExpression9.Items[index]);
          return (SqlScalarExpression) SqlInScalarExpression.Create(expression1, scalarExpression9.Not, scalarExpressionArray3);
        case SqlObjectKind.LiteralScalarExpression:
          return into;
        case SqlObjectKind.MemberIndexerScalarExpression:
          if (!(into is SqlMemberIndexerScalarExpression scalarExpression10))
            throw new DocumentQueryException("Expected a SqlMemberIndexerScalarExpression, got a " + (object) into.GetType());
          return (SqlScalarExpression) SqlMemberIndexerScalarExpression.Create(SqlExpressionManipulation.Substitute(replacement, toReplace, scalarExpression10.MemberExpression), SqlExpressionManipulation.Substitute(replacement, toReplace, scalarExpression10.IndexExpression));
        case SqlObjectKind.ObjectCreateScalarExpression:
          if (!(into is SqlObjectCreateScalarExpression scalarExpression11))
            throw new DocumentQueryException("Expected a SqlObjectCreateScalarExpression, got a " + (object) into.GetType());
          return (SqlScalarExpression) SqlObjectCreateScalarExpression.Create(scalarExpression11.Properties.Select<SqlObjectProperty, SqlObjectProperty>((Func<SqlObjectProperty, SqlObjectProperty>) (prop => SqlObjectProperty.Create(prop.Name, SqlExpressionManipulation.Substitute(replacement, toReplace, prop.Expression)))));
        case SqlObjectKind.PropertyRefScalarExpression:
          if (!(into is SqlPropertyRefScalarExpression scalarExpression12))
            throw new DocumentQueryException("Expected a SqlPropertyRefScalarExpression, got a " + (object) into.GetType());
          if (scalarExpression12.MemberExpression != null)
            return (SqlScalarExpression) SqlPropertyRefScalarExpression.Create(SqlExpressionManipulation.Substitute(replacement, toReplace, scalarExpression12.MemberExpression), scalarExpression12.PropertyIdentifier);
          return scalarExpression12.PropertyIdentifier.Value == toReplace.Value ? replacement : (SqlScalarExpression) scalarExpression12;
        case SqlObjectKind.UnaryScalarExpression:
          if (!(into is SqlUnaryScalarExpression scalarExpression13))
            throw new DocumentQueryException("Expected a SqlUnaryScalarExpression, got a " + (object) into.GetType());
          SqlScalarExpression expression2 = SqlExpressionManipulation.Substitute(replacement, toReplace, scalarExpression13.Expression);
          return (SqlScalarExpression) SqlUnaryScalarExpression.Create(scalarExpression13.OperatorKind, expression2);
        default:
          throw new ArgumentOutOfRangeException("Unexpected Sql Scalar expression kind " + (object) into.Kind);
      }
    }
  }
}
