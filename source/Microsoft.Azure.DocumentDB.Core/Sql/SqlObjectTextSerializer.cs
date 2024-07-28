// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Sql.SqlObjectTextSerializer
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Microsoft.Azure.Documents.Sql
{
  internal sealed class SqlObjectTextSerializer : SqlObjectVisitor
  {
    private const bool MongoDoesNotUseBaselineFiles = true;
    private static readonly string Tab = "    ";
    private readonly StringWriter writer;
    private readonly bool prettyPrint;
    private int indentLevel;

    public SqlObjectTextSerializer(bool prettyPrint)
    {
      this.writer = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture);
      this.prettyPrint = prettyPrint;
    }

    public override void Visit(
      SqlAliasedCollectionExpression sqlAliasedCollectionExpression)
    {
      sqlAliasedCollectionExpression.Collection.Accept((SqlObjectVisitor) this);
      if (sqlAliasedCollectionExpression.Alias == null)
        return;
      this.writer.Write(" AS ");
      sqlAliasedCollectionExpression.Alias.Accept((SqlObjectVisitor) this);
    }

    public override void Visit(
      SqlArrayCreateScalarExpression sqlArrayCreateScalarExpression)
    {
      switch (sqlArrayCreateScalarExpression.Items.Count<SqlScalarExpression>())
      {
        case 0:
          this.writer.Write("[]");
          break;
        case 1:
          this.writer.Write("[");
          sqlArrayCreateScalarExpression.Items[0].Accept((SqlObjectVisitor) this);
          this.writer.Write("]");
          break;
        default:
          this.WriteStartContext("[");
          for (int index = 0; index < sqlArrayCreateScalarExpression.Items.Count; ++index)
          {
            if (index > 0)
              this.WriteDelimiter(",");
            sqlArrayCreateScalarExpression.Items[index].Accept((SqlObjectVisitor) this);
          }
          this.WriteEndContext("]");
          break;
      }
    }

    public override void Visit(
      SqlArrayIteratorCollectionExpression sqlArrayIteratorCollectionExpression)
    {
      sqlArrayIteratorCollectionExpression.Alias.Accept((SqlObjectVisitor) this);
      this.writer.Write(" IN ");
      sqlArrayIteratorCollectionExpression.Collection.Accept((SqlObjectVisitor) this);
    }

    public override void Visit(SqlArrayScalarExpression sqlArrayScalarExpression)
    {
      this.writer.Write("ARRAY");
      this.WriteStartContext("(");
      sqlArrayScalarExpression.SqlQuery.Accept((SqlObjectVisitor) this);
      this.WriteEndContext(")");
    }

    public override void Visit(
      SqlBetweenScalarExpression sqlBetweenScalarExpression)
    {
      this.writer.Write("(");
      sqlBetweenScalarExpression.Expression.Accept((SqlObjectVisitor) this);
      if (sqlBetweenScalarExpression.IsNot)
        this.writer.Write(" NOT");
      this.writer.Write(" BETWEEN ");
      sqlBetweenScalarExpression.LeftExpression.Accept((SqlObjectVisitor) this);
      this.writer.Write(" AND ");
      sqlBetweenScalarExpression.RightExpression.Accept((SqlObjectVisitor) this);
      this.writer.Write(")");
    }

    public override void Visit(
      SqlBinaryScalarExpression sqlBinaryScalarExpression)
    {
      this.writer.Write("(");
      sqlBinaryScalarExpression.LeftExpression.Accept((SqlObjectVisitor) this);
      this.writer.Write(" ");
      this.writer.Write(SqlObjectTextSerializer.SqlBinaryScalarOperatorKindToString(sqlBinaryScalarExpression.OperatorKind));
      this.writer.Write(" ");
      sqlBinaryScalarExpression.RightExpression.Accept((SqlObjectVisitor) this);
      this.writer.Write(")");
    }

    public override void Visit(SqlBooleanLiteral sqlBooleanLiteral) => this.writer.Write(sqlBooleanLiteral.Value ? "true" : "false");

    public override void Visit(
      SqlCoalesceScalarExpression sqlCoalesceScalarExpression)
    {
      this.writer.Write("(");
      sqlCoalesceScalarExpression.LeftExpression.Accept((SqlObjectVisitor) this);
      this.writer.Write(" ?? ");
      sqlCoalesceScalarExpression.RightExpression.Accept((SqlObjectVisitor) this);
      this.writer.Write(")");
    }

    public override void Visit(
      SqlConditionalScalarExpression sqlConditionalScalarExpression)
    {
      this.writer.Write('(');
      sqlConditionalScalarExpression.ConditionExpression.Accept((SqlObjectVisitor) this);
      this.writer.Write(" ? ");
      sqlConditionalScalarExpression.FirstExpression.Accept((SqlObjectVisitor) this);
      this.writer.Write(" : ");
      sqlConditionalScalarExpression.SecondExpression.Accept((SqlObjectVisitor) this);
      this.writer.Write(')');
    }

    public override void Visit(
      SqlConversionScalarExpression sqlConversionScalarExpression)
    {
      sqlConversionScalarExpression.expression.Accept((SqlObjectVisitor) this);
    }

    public override void Visit(
      SqlExistsScalarExpression sqlExistsScalarExpression)
    {
      this.writer.Write("EXISTS");
      this.WriteStartContext("(");
      sqlExistsScalarExpression.SqlQuery.Accept((SqlObjectVisitor) this);
      this.WriteEndContext(")");
    }

    public override void Visit(SqlFromClause sqlFromClause)
    {
      this.writer.Write("FROM ");
      sqlFromClause.Expression.Accept((SqlObjectVisitor) this);
    }

    public override void Visit(
      SqlFunctionCallScalarExpression sqlFunctionCallScalarExpression)
    {
      if (sqlFunctionCallScalarExpression.IsUdf)
        this.writer.Write("udf.");
      sqlFunctionCallScalarExpression.Name.Accept((SqlObjectVisitor) this);
      switch (sqlFunctionCallScalarExpression.Arguments.Count<SqlScalarExpression>())
      {
        case 0:
          this.writer.Write("()");
          break;
        case 1:
          this.writer.Write("(");
          sqlFunctionCallScalarExpression.Arguments[0].Accept((SqlObjectVisitor) this);
          this.writer.Write(")");
          break;
        default:
          this.WriteStartContext("(");
          for (int index = 0; index < sqlFunctionCallScalarExpression.Arguments.Count; ++index)
          {
            if (index > 0)
              this.WriteDelimiter(",");
            sqlFunctionCallScalarExpression.Arguments[index].Accept((SqlObjectVisitor) this);
          }
          this.WriteEndContext(")");
          break;
      }
    }

    public override void Visit(
      SqlGeoNearCallScalarExpression sqlGeoNearCallScalarExpression)
    {
      this.writer.Write("(");
      this.writer.Write("_ST_DISTANCE");
      this.writer.Write("(");
      sqlGeoNearCallScalarExpression.PropertyRef.Accept((SqlObjectVisitor) this);
      this.writer.Write(",");
      sqlGeoNearCallScalarExpression.Geometry.Accept((SqlObjectVisitor) this);
      this.writer.Write(")");
      this.writer.Write(" BETWEEN ");
      if (!sqlGeoNearCallScalarExpression.NumberOfPoints.HasValue)
      {
        this.writer.Write(sqlGeoNearCallScalarExpression.MinimumDistance);
        this.writer.Write(" AND ");
        this.writer.Write(sqlGeoNearCallScalarExpression.MaximumDistance);
      }
      else
      {
        this.writer.Write("@nearMinimumDistance");
        this.writer.Write(" AND ");
        this.writer.Write("@nearMaximumDistance");
      }
      this.writer.Write(")");
    }

    public override void Visit(SqlGroupByClause sqlGroupByClause)
    {
      this.writer.Write("GROUP BY ");
      sqlGroupByClause.Expressions[0].Accept((SqlObjectVisitor) this);
      for (int index = 1; index < sqlGroupByClause.Expressions.Count; ++index)
      {
        this.writer.Write(", ");
        sqlGroupByClause.Expressions[index].Accept((SqlObjectVisitor) this);
      }
    }

    public override void Visit(SqlIdentifier sqlIdentifier) => this.writer.Write(sqlIdentifier.Value);

    public override void Visit(
      SqlIdentifierPathExpression sqlIdentifierPathExpression)
    {
      if (sqlIdentifierPathExpression.ParentPath != null)
      {
        sqlIdentifierPathExpression.ParentPath.Accept((SqlObjectVisitor) this);
        this.writer.Write(".");
      }
      sqlIdentifierPathExpression.Value.Accept((SqlObjectVisitor) this);
    }

    public override void Visit(SqlInputPathCollection sqlInputPathCollection)
    {
      sqlInputPathCollection.Input.Accept((SqlObjectVisitor) this);
      if (sqlInputPathCollection.RelativePath == null)
        return;
      sqlInputPathCollection.RelativePath.Accept((SqlObjectVisitor) this);
    }

    public override void Visit(SqlInScalarExpression sqlInScalarExpression)
    {
      this.writer.Write("(");
      sqlInScalarExpression.Expression.Accept((SqlObjectVisitor) this);
      if (sqlInScalarExpression.Not)
        this.writer.Write(" NOT");
      this.writer.Write(" IN ");
      switch (sqlInScalarExpression.Items.Count<SqlScalarExpression>())
      {
        case 0:
          this.writer.Write("()");
          break;
        case 1:
          this.writer.Write("(");
          sqlInScalarExpression.Items[0].Accept((SqlObjectVisitor) this);
          this.writer.Write(")");
          break;
        default:
          this.WriteStartContext("(");
          for (int index = 0; index < sqlInScalarExpression.Items.Count; ++index)
          {
            if (index > 0)
              this.WriteDelimiter(",");
            sqlInScalarExpression.Items[index].Accept((SqlObjectVisitor) this);
          }
          this.WriteEndContext(")");
          break;
      }
      this.writer.Write(")");
    }

    public override void Visit(
      SqlJoinCollectionExpression sqlJoinCollectionExpression)
    {
      sqlJoinCollectionExpression.LeftExpression.Accept((SqlObjectVisitor) this);
      this.WriteNewline();
      this.WriteTab();
      this.writer.Write(" JOIN ");
      sqlJoinCollectionExpression.RightExpression.Accept((SqlObjectVisitor) this);
    }

    public override void Visit(SqlLimitSpec sqlObject)
    {
      this.writer.Write("LIMIT ");
      this.writer.Write(sqlObject.Limit);
    }

    public override void Visit(
      SqlLiteralArrayCollection sqlLiteralArrayCollection)
    {
      this.writer.Write("[");
      for (int index = 0; index < sqlLiteralArrayCollection.Items.Count; ++index)
      {
        if (index > 0)
          this.writer.Write(", ");
        sqlLiteralArrayCollection.Items[index].Accept((SqlObjectVisitor) this);
      }
      this.writer.Write("]");
    }

    public override void Visit(
      SqlLiteralScalarExpression sqlLiteralScalarExpression)
    {
      sqlLiteralScalarExpression.Literal.Accept((SqlObjectVisitor) this);
    }

    public override void Visit(
      SqlMemberIndexerScalarExpression sqlMemberIndexerScalarExpression)
    {
      sqlMemberIndexerScalarExpression.MemberExpression.Accept((SqlObjectVisitor) this);
      this.writer.Write("[");
      sqlMemberIndexerScalarExpression.IndexExpression.Accept((SqlObjectVisitor) this);
      this.writer.Write("]");
    }

    public override void Visit(SqlNullLiteral sqlNullLiteral) => this.writer.Write("null");

    public override void Visit(SqlNumberLiteral sqlNumberLiteral)
    {
      if (sqlNumberLiteral.Value.IsDouble)
      {
        string s = sqlNumberLiteral.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        double result = 0.0;
        Number64 number64 = sqlNumberLiteral.Value;
        if (!number64.IsNaN)
        {
          number64 = sqlNumberLiteral.Value;
          if (!number64.IsInfinity && (!double.TryParse(s, NumberStyles.Number, (IFormatProvider) CultureInfo.InvariantCulture, out result) || !Number64.ToDouble(sqlNumberLiteral.Value).Equals(result)))
          {
            number64 = sqlNumberLiteral.Value;
            s = number64.ToString("G17", (IFormatProvider) CultureInfo.InvariantCulture);
          }
        }
        this.writer.Write(s);
      }
      else
        this.writer.Write(sqlNumberLiteral.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }

    public override void Visit(SqlNumberPathExpression sqlNumberPathExpression)
    {
      if (sqlNumberPathExpression.ParentPath != null)
        sqlNumberPathExpression.ParentPath.Accept((SqlObjectVisitor) this);
      this.writer.Write("[");
      sqlNumberPathExpression.Value.Accept((SqlObjectVisitor) this);
      this.writer.Write("]");
    }

    public override void Visit(
      SqlObjectCreateScalarExpression sqlObjectCreateScalarExpression)
    {
      switch (sqlObjectCreateScalarExpression.Properties.Count<SqlObjectProperty>())
      {
        case 0:
          this.writer.Write("{}");
          break;
        case 1:
          this.writer.Write("{");
          sqlObjectCreateScalarExpression.Properties.First<SqlObjectProperty>().Accept((SqlObjectVisitor) this);
          this.writer.Write("}");
          break;
        default:
          this.WriteStartContext("{");
          bool flag = false;
          foreach (SqlObjectProperty property in sqlObjectCreateScalarExpression.Properties)
          {
            if (flag)
              this.WriteDelimiter(",");
            property.Accept((SqlObjectVisitor) this);
            flag = true;
          }
          this.WriteEndContext("}");
          break;
      }
    }

    public override void Visit(SqlObjectLiteral sqlObjectLiteral)
    {
      if (sqlObjectLiteral.isValueSerialized)
        this.writer.Write(sqlObjectLiteral.Value);
      else
        this.writer.Write(JsonConvert.SerializeObject(sqlObjectLiteral.Value));
    }

    public override void Visit(SqlObjectProperty sqlObjectProperty)
    {
      sqlObjectProperty.Name.Accept((SqlObjectVisitor) this);
      this.writer.Write(": ");
      sqlObjectProperty.Expression.Accept((SqlObjectVisitor) this);
    }

    public override void Visit(SqlOffsetLimitClause sqlObject)
    {
      sqlObject.OffsetSpec.Accept((SqlObjectVisitor) this);
      this.writer.Write(" ");
      sqlObject.LimitSpec.Accept((SqlObjectVisitor) this);
    }

    public override void Visit(SqlOffsetSpec sqlObject)
    {
      this.writer.Write("OFFSET ");
      this.writer.Write(sqlObject.Offset);
    }

    public override void Visit(SqlOrderbyClause sqlOrderByClause)
    {
      this.writer.Write("ORDER BY ");
      sqlOrderByClause.OrderbyItems[0].Accept((SqlObjectVisitor) this);
      for (int index = 1; index < sqlOrderByClause.OrderbyItems.Count; ++index)
      {
        this.writer.Write(", ");
        sqlOrderByClause.OrderbyItems[index].Accept((SqlObjectVisitor) this);
      }
    }

    public override void Visit(SqlOrderByItem sqlOrderByItem)
    {
      sqlOrderByItem.Expression.Accept((SqlObjectVisitor) this);
      if (sqlOrderByItem.IsDescending)
        this.writer.Write(" DESC");
      else
        this.writer.Write(" ASC");
    }

    public override void Visit(SqlProgram sqlProgram) => sqlProgram.Query.Accept((SqlObjectVisitor) this);

    public override void Visit(SqlPropertyName sqlPropertyName)
    {
      this.writer.Write('"');
      this.writer.Write(sqlPropertyName.Value);
      this.writer.Write('"');
    }

    public override void Visit(
      SqlPropertyRefScalarExpression sqlPropertyRefScalarExpression)
    {
      if (sqlPropertyRefScalarExpression.MemberExpression != null)
      {
        sqlPropertyRefScalarExpression.MemberExpression.Accept((SqlObjectVisitor) this);
        this.writer.Write(".");
      }
      sqlPropertyRefScalarExpression.PropertyIdentifier.Accept((SqlObjectVisitor) this);
    }

    public override void Visit(SqlQuery sqlQuery)
    {
      sqlQuery.SelectClause.Accept((SqlObjectVisitor) this);
      if (sqlQuery.FromClause != null)
      {
        this.WriteDelimiter("");
        sqlQuery.FromClause.Accept((SqlObjectVisitor) this);
      }
      if (sqlQuery.WhereClause != null)
      {
        this.WriteDelimiter("");
        sqlQuery.WhereClause.Accept((SqlObjectVisitor) this);
      }
      if (sqlQuery.GroupByClause != null)
      {
        this.WriteDelimiter("");
        sqlQuery.GroupByClause.Accept((SqlObjectVisitor) this);
      }
      if (sqlQuery.OrderbyClause != null)
      {
        this.WriteDelimiter("");
        sqlQuery.OrderbyClause.Accept((SqlObjectVisitor) this);
      }
      if (sqlQuery.OffsetLimitClause != null)
      {
        this.WriteDelimiter("");
        sqlQuery.OffsetLimitClause.Accept((SqlObjectVisitor) this);
      }
      this.writer.Write(" ");
    }

    public override void Visit(SqlSelectClause sqlSelectClause)
    {
      this.writer.Write("SELECT ");
      if (sqlSelectClause.HasDistinct)
        this.writer.Write("DISTINCT ");
      if (sqlSelectClause.TopSpec != null)
      {
        sqlSelectClause.TopSpec.Accept((SqlObjectVisitor) this);
        this.writer.Write(" ");
      }
      sqlSelectClause.SelectSpec.Accept((SqlObjectVisitor) this);
    }

    public override void Visit(SqlSelectItem sqlSelectItem)
    {
      sqlSelectItem.Expression.Accept((SqlObjectVisitor) this);
      if (sqlSelectItem.Alias == null)
        return;
      this.writer.Write(" AS ");
      sqlSelectItem.Alias.Accept((SqlObjectVisitor) this);
    }

    public override void Visit(SqlSelectListSpec sqlSelectListSpec)
    {
      switch (sqlSelectListSpec.Items.Count<SqlSelectItem>())
      {
        case 0:
          throw new ArgumentException("Expected sqlSelectListSpec to have atleast 1 item.");
        case 1:
          sqlSelectListSpec.Items[0].Accept((SqlObjectVisitor) this);
          break;
        default:
          bool flag = false;
          ++this.indentLevel;
          this.WriteNewline();
          this.WriteTab();
          foreach (SqlSelectItem sqlSelectItem in (IEnumerable<SqlSelectItem>) sqlSelectListSpec.Items)
          {
            if (flag)
              this.WriteDelimiter(",");
            sqlSelectItem.Accept((SqlObjectVisitor) this);
            flag = true;
          }
          --this.indentLevel;
          break;
      }
    }

    public override void Visit(SqlSelectStarSpec sqlSelectStarSpec) => this.writer.Write("*");

    public override void Visit(SqlSelectValueSpec sqlSelectValueSpec)
    {
      this.writer.Write("VALUE ");
      sqlSelectValueSpec.Expression.Accept((SqlObjectVisitor) this);
    }

    public override void Visit(SqlStringLiteral sqlStringLiteral)
    {
      this.writer.Write("\"");
      this.writer.Write(SqlObjectTextSerializer.GetEscapedString(sqlStringLiteral.Value));
      this.writer.Write("\"");
    }

    public override void Visit(SqlStringPathExpression sqlStringPathExpression)
    {
      if (sqlStringPathExpression.ParentPath != null)
        sqlStringPathExpression.ParentPath.Accept((SqlObjectVisitor) this);
      this.writer.Write("[");
      sqlStringPathExpression.Value.Accept((SqlObjectVisitor) this);
      this.writer.Write("]");
    }

    public override void Visit(SqlSubqueryCollection sqlSubqueryCollection)
    {
      this.WriteStartContext("(");
      sqlSubqueryCollection.Query.Accept((SqlObjectVisitor) this);
      this.WriteEndContext(")");
    }

    public override void Visit(
      SqlSubqueryScalarExpression sqlSubqueryScalarExpression)
    {
      this.WriteStartContext("(");
      sqlSubqueryScalarExpression.Query.Accept((SqlObjectVisitor) this);
      this.WriteEndContext(")");
    }

    public override void Visit(SqlTopSpec sqlTopSpec)
    {
      this.writer.Write("TOP ");
      this.writer.Write(sqlTopSpec.Count);
    }

    public override void Visit(SqlUnaryScalarExpression sqlUnaryScalarExpression)
    {
      this.writer.Write("(");
      this.writer.Write(SqlObjectTextSerializer.SqlUnaryScalarOperatorKindToString(sqlUnaryScalarExpression.OperatorKind));
      this.writer.Write(" ");
      sqlUnaryScalarExpression.Expression.Accept((SqlObjectVisitor) this);
      this.writer.Write(")");
    }

    public override void Visit(SqlUndefinedLiteral sqlUndefinedLiteral) => this.writer.Write("undefined");

    public override void Visit(SqlWhereClause sqlWhereClause)
    {
      this.writer.Write("WHERE ");
      sqlWhereClause.FilterExpression.Accept((SqlObjectVisitor) this);
    }

    public override string ToString() => this.writer.ToString();

    private void WriteStartContext(string startCharacter)
    {
      ++this.indentLevel;
      this.writer.Write(startCharacter);
      this.WriteNewline();
      this.WriteTab();
    }

    private void WriteDelimiter(string delimiter)
    {
      this.writer.Write(delimiter);
      this.writer.Write(' ');
      this.WriteNewline();
      this.WriteTab();
    }

    private void WriteEndContext(string endCharacter)
    {
      --this.indentLevel;
      this.WriteNewline();
      this.WriteTab();
      this.writer.Write(endCharacter);
    }

    private void WriteNewline()
    {
      if (!this.prettyPrint)
        return;
      this.writer.WriteLine();
    }

    private void WriteTab()
    {
      if (!this.prettyPrint)
        return;
      for (int index = 0; index < this.indentLevel; ++index)
        this.writer.Write(SqlObjectTextSerializer.Tab);
    }

    private static string GetEscapedString(string value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      if (value.All<char>((Func<char, bool>) (c => !SqlObjectTextSerializer.IsEscapedCharacter(c))))
        return value;
      StringBuilder stringBuilder = new StringBuilder(value.Length);
      foreach (char ch in value)
      {
        switch (ch)
        {
          case '\b':
            stringBuilder.Append("\\b");
            break;
          case '\t':
            stringBuilder.Append("\\t");
            break;
          case '\n':
            stringBuilder.Append("\\n");
            break;
          case '\f':
            stringBuilder.Append("\\f");
            break;
          case '\r':
            stringBuilder.Append("\\r");
            break;
          case '"':
            stringBuilder.Append("\\\"");
            break;
          case '\\':
            stringBuilder.Append("\\\\");
            break;
          default:
            switch (CharUnicodeInfo.GetUnicodeCategory(ch))
            {
              case UnicodeCategory.UppercaseLetter:
              case UnicodeCategory.LowercaseLetter:
              case UnicodeCategory.TitlecaseLetter:
              case UnicodeCategory.OtherLetter:
              case UnicodeCategory.DecimalDigitNumber:
              case UnicodeCategory.LetterNumber:
              case UnicodeCategory.OtherNumber:
              case UnicodeCategory.SpaceSeparator:
              case UnicodeCategory.ConnectorPunctuation:
              case UnicodeCategory.DashPunctuation:
              case UnicodeCategory.OpenPunctuation:
              case UnicodeCategory.ClosePunctuation:
              case UnicodeCategory.InitialQuotePunctuation:
              case UnicodeCategory.FinalQuotePunctuation:
              case UnicodeCategory.OtherPunctuation:
              case UnicodeCategory.MathSymbol:
              case UnicodeCategory.CurrencySymbol:
              case UnicodeCategory.ModifierSymbol:
              case UnicodeCategory.OtherSymbol:
                stringBuilder.Append(ch);
                continue;
              default:
                stringBuilder.AppendFormat("\\u{0:x4}", (object) (int) ch);
                continue;
            }
        }
      }
      return stringBuilder.ToString();
    }

    private static bool IsEscapedCharacter(char c)
    {
      switch (c)
      {
        case '\b':
        case '\t':
        case '\n':
        case '\f':
        case '\r':
        case '"':
        case '\\':
          return true;
        default:
          switch (CharUnicodeInfo.GetUnicodeCategory(c))
          {
            case UnicodeCategory.UppercaseLetter:
            case UnicodeCategory.LowercaseLetter:
            case UnicodeCategory.TitlecaseLetter:
            case UnicodeCategory.OtherLetter:
            case UnicodeCategory.DecimalDigitNumber:
            case UnicodeCategory.LetterNumber:
            case UnicodeCategory.OtherNumber:
            case UnicodeCategory.SpaceSeparator:
            case UnicodeCategory.ConnectorPunctuation:
            case UnicodeCategory.DashPunctuation:
            case UnicodeCategory.OpenPunctuation:
            case UnicodeCategory.ClosePunctuation:
            case UnicodeCategory.InitialQuotePunctuation:
            case UnicodeCategory.FinalQuotePunctuation:
            case UnicodeCategory.OtherPunctuation:
            case UnicodeCategory.MathSymbol:
            case UnicodeCategory.CurrencySymbol:
            case UnicodeCategory.ModifierSymbol:
            case UnicodeCategory.OtherSymbol:
              return false;
            default:
              return true;
          }
      }
    }

    private static string SqlUnaryScalarOperatorKindToString(SqlUnaryScalarOperatorKind kind)
    {
      switch (kind)
      {
        case SqlUnaryScalarOperatorKind.BitwiseNot:
          return "~";
        case SqlUnaryScalarOperatorKind.Not:
          return "NOT";
        case SqlUnaryScalarOperatorKind.Minus:
          return "-";
        case SqlUnaryScalarOperatorKind.Plus:
          return "+";
        default:
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, "Unsupported operator {0}", (object) kind));
      }
    }

    private static string SqlBinaryScalarOperatorKindToString(SqlBinaryScalarOperatorKind kind)
    {
      switch (kind)
      {
        case SqlBinaryScalarOperatorKind.Add:
          return "+";
        case SqlBinaryScalarOperatorKind.And:
          return "AND";
        case SqlBinaryScalarOperatorKind.BitwiseAnd:
          return "&";
        case SqlBinaryScalarOperatorKind.BitwiseOr:
          return "|";
        case SqlBinaryScalarOperatorKind.BitwiseXor:
          return "^";
        case SqlBinaryScalarOperatorKind.Coalesce:
          return "??";
        case SqlBinaryScalarOperatorKind.Divide:
          return "/";
        case SqlBinaryScalarOperatorKind.Equal:
          return "=";
        case SqlBinaryScalarOperatorKind.GreaterThan:
          return ">";
        case SqlBinaryScalarOperatorKind.GreaterThanOrEqual:
          return ">=";
        case SqlBinaryScalarOperatorKind.LessThan:
          return "<";
        case SqlBinaryScalarOperatorKind.LessThanOrEqual:
          return "<=";
        case SqlBinaryScalarOperatorKind.Modulo:
          return "%";
        case SqlBinaryScalarOperatorKind.Multiply:
          return "*";
        case SqlBinaryScalarOperatorKind.NotEqual:
          return "!=";
        case SqlBinaryScalarOperatorKind.Or:
          return "OR";
        case SqlBinaryScalarOperatorKind.StringConcat:
          return "||";
        case SqlBinaryScalarOperatorKind.Subtract:
          return "-";
        default:
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, "Unsupported operator {0}", (object) kind));
      }
    }
  }
}
