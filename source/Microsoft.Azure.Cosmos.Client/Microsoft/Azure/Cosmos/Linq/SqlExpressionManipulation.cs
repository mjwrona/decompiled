// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Linq.SqlExpressionManipulation
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.SqlObjects;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace Microsoft.Azure.Cosmos.Linq
{
  internal static class SqlExpressionManipulation
  {
    public static SqlScalarExpression Substitute(
      SqlScalarExpression replacement,
      SqlIdentifier toReplace,
      SqlScalarExpression into)
    {
      if ((SqlObject) into == (SqlObject) null)
        return (SqlScalarExpression) null;
      if ((SqlObject) replacement == (SqlObject) null)
        throw new ArgumentNullException(nameof (replacement));
      switch (into)
      {
        case SqlArrayCreateScalarExpression scalarExpression4:
          SqlScalarExpression[] scalarExpressionArray1 = new SqlScalarExpression[scalarExpression4.Items.Length];
          for (int index = 0; index < scalarExpressionArray1.Length; ++index)
          {
            SqlScalarExpression into1 = scalarExpression4.Items[index];
            SqlScalarExpression scalarExpression = SqlExpressionManipulation.Substitute(replacement, toReplace, into1);
            scalarExpressionArray1[index] = scalarExpression;
          }
          return (SqlScalarExpression) SqlArrayCreateScalarExpression.Create(scalarExpressionArray1);
        case SqlBinaryScalarExpression scalarExpression5:
          SqlScalarExpression left = SqlExpressionManipulation.Substitute(replacement, toReplace, scalarExpression5.LeftExpression);
          SqlScalarExpression right = SqlExpressionManipulation.Substitute(replacement, toReplace, scalarExpression5.RightExpression);
          return (SqlScalarExpression) SqlBinaryScalarExpression.Create(scalarExpression5.OperatorKind, left, right);
        case SqlUnaryScalarExpression scalarExpression6:
          SqlScalarExpression expression = SqlExpressionManipulation.Substitute(replacement, toReplace, scalarExpression6.Expression);
          return (SqlScalarExpression) SqlUnaryScalarExpression.Create(scalarExpression6.OperatorKind, expression);
        case SqlLiteralScalarExpression _:
          return into;
        case SqlFunctionCallScalarExpression scalarExpression7:
          SqlScalarExpression[] scalarExpressionArray2 = new SqlScalarExpression[scalarExpression7.Arguments.Length];
          for (int index = 0; index < scalarExpressionArray2.Length; ++index)
          {
            SqlScalarExpression into2 = scalarExpression7.Arguments[index];
            SqlScalarExpression scalarExpression = SqlExpressionManipulation.Substitute(replacement, toReplace, into2);
            scalarExpressionArray2[index] = scalarExpression;
          }
          return (SqlScalarExpression) SqlFunctionCallScalarExpression.Create(scalarExpression7.Name, scalarExpression7.IsUdf, scalarExpressionArray2);
        case SqlObjectCreateScalarExpression scalarExpression8:
          return (SqlScalarExpression) SqlObjectCreateScalarExpression.Create(scalarExpression8.Properties.Select<SqlObjectProperty, SqlObjectProperty>((Func<SqlObjectProperty, SqlObjectProperty>) (prop => SqlObjectProperty.Create(prop.Name, SqlExpressionManipulation.Substitute(replacement, toReplace, prop.Value)))).ToImmutableArray<SqlObjectProperty>());
        case SqlMemberIndexerScalarExpression scalarExpression9:
          return (SqlScalarExpression) SqlMemberIndexerScalarExpression.Create(SqlExpressionManipulation.Substitute(replacement, toReplace, scalarExpression9.Member), SqlExpressionManipulation.Substitute(replacement, toReplace, scalarExpression9.Indexer));
        case SqlPropertyRefScalarExpression scalarExpression10:
          if (!((SqlObject) scalarExpression10.Member == (SqlObject) null))
            return (SqlScalarExpression) SqlPropertyRefScalarExpression.Create(SqlExpressionManipulation.Substitute(replacement, toReplace, scalarExpression10.Member), scalarExpression10.Identifier);
          return scalarExpression10.Identifier.Value == toReplace.Value ? replacement : (SqlScalarExpression) scalarExpression10;
        case SqlConditionalScalarExpression scalarExpression11:
          SqlScalarExpression condition = SqlExpressionManipulation.Substitute(replacement, toReplace, scalarExpression11.Condition);
          SqlScalarExpression scalarExpression1 = SqlExpressionManipulation.Substitute(replacement, toReplace, scalarExpression11.Consequent);
          SqlScalarExpression scalarExpression2 = SqlExpressionManipulation.Substitute(replacement, toReplace, scalarExpression11.Alternative);
          SqlScalarExpression consequent = scalarExpression1;
          SqlScalarExpression alternative = scalarExpression2;
          return (SqlScalarExpression) SqlConditionalScalarExpression.Create(condition, consequent, alternative);
        case SqlInScalarExpression scalarExpression12:
          SqlScalarExpression needle = SqlExpressionManipulation.Substitute(replacement, toReplace, scalarExpression12.Needle);
          ImmutableArray<SqlScalarExpression> haystack = scalarExpression12.Haystack;
          SqlScalarExpression[] scalarExpressionArray3 = new SqlScalarExpression[haystack.Length];
          for (int index1 = 0; index1 < scalarExpressionArray3.Length; ++index1)
          {
            SqlScalarExpression[] scalarExpressionArray4 = scalarExpressionArray3;
            int index2 = index1;
            SqlScalarExpression replacement1 = replacement;
            SqlIdentifier toReplace1 = toReplace;
            haystack = scalarExpression12.Haystack;
            SqlScalarExpression into3 = haystack[index1];
            SqlScalarExpression scalarExpression3 = SqlExpressionManipulation.Substitute(replacement1, toReplace1, into3);
            scalarExpressionArray4[index2] = scalarExpression3;
          }
          return (SqlScalarExpression) SqlInScalarExpression.Create(needle, scalarExpression12.Not, scalarExpressionArray3);
        default:
          throw new ArgumentOutOfRangeException("Unexpected Sql Scalar expression kind " + into.GetType()?.ToString());
      }
    }
  }
}
