// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.SqlObjects.Visitors.SqlObjectTextSerializer
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Buffers;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Microsoft.Azure.Cosmos.SqlObjects.Visitors
{
  internal sealed class SqlObjectTextSerializer : SqlObjectVisitor
  {
    private const string Tab = "    ";
    private static readonly char[] CharactersThatNeedEscaping = Enumerable.Range(0, 32).Select<int, char>((Func<int, char>) (x => (char) x)).Concat<char>((IEnumerable<char>) new char[2]
    {
      '"',
      '\\'
    }).ToArray<char>();
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
      if (!((SqlObject) sqlAliasedCollectionExpression.Alias != (SqlObject) null))
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
          int index = 0;
          while (true)
          {
            int num = index;
            ImmutableArray<SqlScalarExpression> items = sqlArrayCreateScalarExpression.Items;
            int length = items.Length;
            if (num < length)
            {
              if (index > 0)
                this.WriteDelimiter(",");
              items = sqlArrayCreateScalarExpression.Items;
              items[index].Accept((SqlObjectVisitor) this);
              ++index;
            }
            else
              break;
          }
          this.WriteEndContext("]");
          break;
      }
    }

    public override void Visit(
      SqlArrayIteratorCollectionExpression sqlArrayIteratorCollectionExpression)
    {
      sqlArrayIteratorCollectionExpression.Identifier.Accept((SqlObjectVisitor) this);
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
      if (sqlBetweenScalarExpression.Not)
        this.writer.Write(" NOT");
      this.writer.Write(" BETWEEN ");
      sqlBetweenScalarExpression.StartInclusive.Accept((SqlObjectVisitor) this);
      this.writer.Write(" AND ");
      sqlBetweenScalarExpression.EndInclusive.Accept((SqlObjectVisitor) this);
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
      sqlCoalesceScalarExpression.Left.Accept((SqlObjectVisitor) this);
      this.writer.Write(" ?? ");
      sqlCoalesceScalarExpression.Right.Accept((SqlObjectVisitor) this);
      this.writer.Write(")");
    }

    public override void Visit(
      SqlConditionalScalarExpression sqlConditionalScalarExpression)
    {
      this.writer.Write('(');
      sqlConditionalScalarExpression.Condition.Accept((SqlObjectVisitor) this);
      this.writer.Write(" ? ");
      sqlConditionalScalarExpression.Consequent.Accept((SqlObjectVisitor) this);
      this.writer.Write(" : ");
      sqlConditionalScalarExpression.Alternative.Accept((SqlObjectVisitor) this);
      this.writer.Write(')');
    }

    public override void Visit(
      SqlExistsScalarExpression sqlExistsScalarExpression)
    {
      this.writer.Write("EXISTS");
      this.WriteStartContext("(");
      sqlExistsScalarExpression.Subquery.Accept((SqlObjectVisitor) this);
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
          for (int index = 0; index < sqlFunctionCallScalarExpression.Arguments.Length; ++index)
          {
            if (index > 0)
              this.WriteDelimiter(",");
            sqlFunctionCallScalarExpression.Arguments[index].Accept((SqlObjectVisitor) this);
          }
          this.WriteEndContext(")");
          break;
      }
    }

    public override void Visit(SqlGroupByClause sqlGroupByClause)
    {
      this.writer.Write("GROUP BY ");
      sqlGroupByClause.Expressions[0].Accept((SqlObjectVisitor) this);
      for (int index = 1; index < sqlGroupByClause.Expressions.Length; ++index)
      {
        this.writer.Write(", ");
        sqlGroupByClause.Expressions[index].Accept((SqlObjectVisitor) this);
      }
    }

    public override void Visit(SqlIdentifier sqlIdentifier) => this.writer.Write(sqlIdentifier.Value);

    public override void Visit(
      SqlIdentifierPathExpression sqlIdentifierPathExpression)
    {
      if ((SqlObject) sqlIdentifierPathExpression.ParentPath != (SqlObject) null)
        sqlIdentifierPathExpression.ParentPath.Accept((SqlObjectVisitor) this);
      this.writer.Write(".");
      sqlIdentifierPathExpression.Value.Accept((SqlObjectVisitor) this);
    }

    public override void Visit(SqlInputPathCollection sqlInputPathCollection)
    {
      sqlInputPathCollection.Input.Accept((SqlObjectVisitor) this);
      if (!((SqlObject) sqlInputPathCollection.RelativePath != (SqlObject) null))
        return;
      sqlInputPathCollection.RelativePath.Accept((SqlObjectVisitor) this);
    }

    public override void Visit(SqlInScalarExpression sqlInScalarExpression)
    {
      this.writer.Write("(");
      sqlInScalarExpression.Needle.Accept((SqlObjectVisitor) this);
      if (sqlInScalarExpression.Not)
        this.writer.Write(" NOT");
      this.writer.Write(" IN ");
      switch (sqlInScalarExpression.Haystack.Count<SqlScalarExpression>())
      {
        case 0:
          this.writer.Write("()");
          break;
        case 1:
          this.writer.Write("(");
          sqlInScalarExpression.Haystack[0].Accept((SqlObjectVisitor) this);
          this.writer.Write(")");
          break;
        default:
          this.WriteStartContext("(");
          int index = 0;
          while (true)
          {
            int num = index;
            ImmutableArray<SqlScalarExpression> haystack = sqlInScalarExpression.Haystack;
            int length = haystack.Length;
            if (num < length)
            {
              if (index > 0)
                this.WriteDelimiter(",");
              haystack = sqlInScalarExpression.Haystack;
              haystack[index].Accept((SqlObjectVisitor) this);
              ++index;
            }
            else
              break;
          }
          this.WriteEndContext(")");
          break;
      }
      this.writer.Write(")");
    }

    public override void Visit(
      SqlJoinCollectionExpression sqlJoinCollectionExpression)
    {
      sqlJoinCollectionExpression.Left.Accept((SqlObjectVisitor) this);
      this.WriteNewline();
      this.WriteTab();
      this.writer.Write(" JOIN ");
      sqlJoinCollectionExpression.Right.Accept((SqlObjectVisitor) this);
    }

    public override void Visit(SqlLimitSpec sqlObject)
    {
      this.writer.Write("LIMIT ");
      sqlObject.LimitExpression.Accept((SqlObjectVisitor) this);
    }

    public override void Visit(SqlLikeScalarExpression sqlObject)
    {
      this.writer.Write("(");
      sqlObject.Expression.Accept((SqlObjectVisitor) this);
      if (sqlObject.Not)
        this.writer.Write(" NOT ");
      this.writer.Write(" LIKE ");
      sqlObject.Pattern.Accept((SqlObjectVisitor) this);
      if ((SqlObject) sqlObject.EscapeSequence != (SqlObject) null)
      {
        this.writer.Write(" ESCAPE ");
        sqlObject.EscapeSequence.Accept((SqlObjectVisitor) this);
      }
      this.writer.Write(")");
    }

    public override void Visit(
      SqlLiteralScalarExpression sqlLiteralScalarExpression)
    {
      sqlLiteralScalarExpression.Literal.Accept((SqlObjectVisitor) this);
    }

    public override void Visit(
      SqlMemberIndexerScalarExpression sqlMemberIndexerScalarExpression)
    {
      sqlMemberIndexerScalarExpression.Member.Accept((SqlObjectVisitor) this);
      this.writer.Write("[");
      sqlMemberIndexerScalarExpression.Indexer.Accept((SqlObjectVisitor) this);
      this.writer.Write("]");
    }

    public override void Visit(SqlNullLiteral sqlNullLiteral) => this.writer.Write("null");

    public override void Visit(SqlNumberLiteral sqlNumberLiteral) => SqlObjectTextSerializer.WriteNumber64(this.writer.GetStringBuilder(), sqlNumberLiteral.Value);

    public override void Visit(SqlNumberPathExpression sqlNumberPathExpression)
    {
      if ((SqlObject) sqlNumberPathExpression.ParentPath != (SqlObject) null)
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

    public override void Visit(SqlObjectProperty sqlObjectProperty)
    {
      sqlObjectProperty.Name.Accept((SqlObjectVisitor) this);
      this.writer.Write(": ");
      sqlObjectProperty.Value.Accept((SqlObjectVisitor) this);
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
      sqlObject.OffsetExpression.Accept((SqlObjectVisitor) this);
    }

    public override void Visit(SqlOrderByClause sqlOrderByClause)
    {
      this.writer.Write("ORDER BY ");
      sqlOrderByClause.OrderByItems[0].Accept((SqlObjectVisitor) this);
      for (int index = 1; index < sqlOrderByClause.OrderByItems.Length; ++index)
      {
        this.writer.Write(", ");
        sqlOrderByClause.OrderByItems[index].Accept((SqlObjectVisitor) this);
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

    public override void Visit(SqlParameter sqlParameter) => this.writer.Write(sqlParameter.Name);

    public override void Visit(
      SqlParameterRefScalarExpression sqlParameterRefScalarExpression)
    {
      sqlParameterRefScalarExpression.Parameter.Accept((SqlObjectVisitor) this);
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
      if ((SqlObject) sqlPropertyRefScalarExpression.Member != (SqlObject) null)
      {
        sqlPropertyRefScalarExpression.Member.Accept((SqlObjectVisitor) this);
        this.writer.Write(".");
      }
      sqlPropertyRefScalarExpression.Identifier.Accept((SqlObjectVisitor) this);
    }

    public override void Visit(SqlQuery sqlQuery)
    {
      sqlQuery.SelectClause.Accept((SqlObjectVisitor) this);
      if ((SqlObject) sqlQuery.FromClause != (SqlObject) null)
      {
        this.WriteDelimiter(string.Empty);
        sqlQuery.FromClause.Accept((SqlObjectVisitor) this);
      }
      if ((SqlObject) sqlQuery.WhereClause != (SqlObject) null)
      {
        this.WriteDelimiter(string.Empty);
        sqlQuery.WhereClause.Accept((SqlObjectVisitor) this);
      }
      if ((SqlObject) sqlQuery.GroupByClause != (SqlObject) null)
      {
        sqlQuery.GroupByClause.Accept((SqlObjectVisitor) this);
        this.writer.Write(" ");
      }
      if ((SqlObject) sqlQuery.OrderByClause != (SqlObject) null)
      {
        this.WriteDelimiter(string.Empty);
        sqlQuery.OrderByClause.Accept((SqlObjectVisitor) this);
      }
      if (!((SqlObject) sqlQuery.OffsetLimitClause != (SqlObject) null))
        return;
      this.WriteDelimiter(string.Empty);
      sqlQuery.OffsetLimitClause.Accept((SqlObjectVisitor) this);
    }

    public override void Visit(SqlSelectClause sqlSelectClause)
    {
      this.writer.Write("SELECT ");
      if (sqlSelectClause.HasDistinct)
        this.writer.Write("DISTINCT ");
      if ((SqlObject) sqlSelectClause.TopSpec != (SqlObject) null)
      {
        sqlSelectClause.TopSpec.Accept((SqlObjectVisitor) this);
        this.writer.Write(" ");
      }
      sqlSelectClause.SelectSpec.Accept((SqlObjectVisitor) this);
    }

    public override void Visit(SqlSelectItem sqlSelectItem)
    {
      sqlSelectItem.Expression.Accept((SqlObjectVisitor) this);
      if (!((SqlObject) sqlSelectItem.Alias != (SqlObject) null))
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
          foreach (SqlSelectItem sqlSelectItem in sqlSelectListSpec.Items)
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
      SqlObjectTextSerializer.WriteEscapedString(this.writer.GetStringBuilder(), MemoryExtensions.AsSpan(sqlStringLiteral.Value));
      this.writer.Write("\"");
    }

    public override void Visit(SqlStringPathExpression sqlStringPathExpression)
    {
      if ((SqlObject) sqlStringPathExpression.ParentPath != (SqlObject) null)
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
      sqlTopSpec.TopExpresion.Accept((SqlObjectVisitor) this);
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
        this.writer.Write("    ");
    }

    private static unsafe void WriteNumber64(StringBuilder stringBuilder, Number64 value)
    {
      // ISSUE: untyped stack allocation
      Span<byte> destination = new Span<byte>((void*) __untypedstackalloc(new IntPtr(32)), 32);
      if (value.IsInteger)
      {
        int bytesWritten;
        destination = Utf8Formatter.TryFormat(Number64.ToLong(value), destination, out bytesWritten, new StandardFormat()) ? destination.Slice(0, bytesWritten) : throw new InvalidOperationException("Failed to write a long.");
        for (int index = 0; index < destination.Length; ++index)
          stringBuilder.Append((char) destination[index]);
      }
      else
        stringBuilder.Append(value.ToString("R", (IFormatProvider) CultureInfo.InvariantCulture));
    }

    private static unsafe void WriteEscapedString(
      StringBuilder stringBuilder,
      ReadOnlySpan<char> unescapedString)
    {
      while (!unescapedString.IsEmpty)
      {
        int? nullable = SqlObjectTextSerializer.IndexOfCharacterThatNeedsEscaping(unescapedString);
        if (!nullable.HasValue)
          nullable = new int?(unescapedString.Length);
        ReadOnlySpan<char> readOnlySpan = unescapedString.Slice(0, nullable.Value);
        fixed (char* chPtr = &readOnlySpan.GetPinnableReference())
          stringBuilder.Append(chPtr, readOnlySpan.Length);
        unescapedString = unescapedString.Slice(nullable.Value);
        if (!unescapedString.IsEmpty)
        {
          char ch1 = unescapedString[0];
          unescapedString = unescapedString.Slice(1);
          switch (ch1)
          {
            case '\b':
              stringBuilder.Append('\\');
              stringBuilder.Append('b');
              continue;
            case '\t':
              stringBuilder.Append('\\');
              stringBuilder.Append('t');
              continue;
            case '\n':
              stringBuilder.Append('\\');
              stringBuilder.Append('n');
              continue;
            case '\f':
              stringBuilder.Append('\\');
              stringBuilder.Append('f');
              continue;
            case '\r':
              stringBuilder.Append('\\');
              stringBuilder.Append('r');
              continue;
            case '"':
              stringBuilder.Append('\\');
              stringBuilder.Append('"');
              continue;
            case '/':
              stringBuilder.Append('\\');
              stringBuilder.Append('/');
              continue;
            case '\\':
              stringBuilder.Append('\\');
              stringBuilder.Append('\\');
              continue;
            default:
              char ch2 = ch1;
              stringBuilder.Append('\\');
              stringBuilder.Append('u');
              stringBuilder.Append(SqlObjectTextSerializer.GetHexDigit((int) ch2 >> 12 & 15));
              stringBuilder.Append(SqlObjectTextSerializer.GetHexDigit((int) ch2 >> 8 & 15));
              stringBuilder.Append(SqlObjectTextSerializer.GetHexDigit((int) ch2 >> 4 & 15));
              stringBuilder.Append(SqlObjectTextSerializer.GetHexDigit((int) ch2 & 15));
              continue;
          }
        }
      }
    }

    private static int? IndexOfCharacterThatNeedsEscaping(ReadOnlySpan<char> unescapedString)
    {
      int? nullable = new int?();
      int num = unescapedString.IndexOfAny<char>((ReadOnlySpan<char>) SqlObjectTextSerializer.CharactersThatNeedEscaping);
      if (num != -1)
        nullable = new int?(num);
      return nullable;
    }

    private static char GetHexDigit(int value) => value < 10 ? (char) (48 + value) : (char) (65 + value - 10);

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
