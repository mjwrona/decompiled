// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Tables.SharedFiles.ODataFilterTranslator
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Documents.Interop.Common.Schema;
using Microsoft.Azure.Documents.Interop.Common.Schema.Edm;
using Microsoft.OData.UriParser;
using Microsoft.OData.UriParser.Aggregation;
using System;
using System.Globalization;
using System.Text;

namespace Microsoft.Azure.Cosmos.Tables.SharedFiles
{
  internal static class ODataFilterTranslator
  {
    private const int ExpressionParserMaxDepth = 1000;

    public static string ToSql(
      string odataV4FilterString,
      bool isTableQuery,
      bool enableTimestampQuery = false)
    {
      QueryToken queryToken = !string.IsNullOrEmpty(odataV4FilterString) ? new UriQueryExpressionParser(1000).ParseFilter(odataV4FilterString) : throw new ArgumentNullException(nameof (odataV4FilterString));
      ODataFilterTranslator.QueryTokenVisitor queryTokenVisitor1 = new ODataFilterTranslator.QueryTokenVisitor(isTableQuery, enableTimestampQuery);
      ODataFilterTranslator.QueryTokenVisitor queryTokenVisitor2 = queryTokenVisitor1;
      queryToken.Accept<bool>((ISyntacticTreeVisitor<bool>) queryTokenVisitor2);
      return queryTokenVisitor1.SqlFilter;
    }

    private sealed class QueryTokenVisitor : ISyntacticTreeVisitor<bool>
    {
      private readonly StringBuilder stringBuilder;
      private readonly bool isTableQuery;
      private readonly bool enableTimestampQuery;
      private static readonly string tableEntityDotId = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) EdmSchemaMapping.EntityName, (object) "id");

      public QueryTokenVisitor(bool isTableQuery, bool enableTimestampQuery)
      {
        this.stringBuilder = new StringBuilder();
        this.isTableQuery = isTableQuery;
        this.enableTimestampQuery = enableTimestampQuery;
      }

      public string SqlFilter => this.stringBuilder.ToString();

      public bool Visit(AllToken tokenIn) => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "QueryToken of type '{0}' is not supported.", (object) ((QueryToken) tokenIn).Kind));

      public bool Visit(AnyToken tokenIn) => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "QueryToken of type '{0}' is not supported.", (object) ((QueryToken) tokenIn).Kind));

      public bool Visit(BinaryOperatorToken tokenIn)
      {
        this.stringBuilder.Append('(');
        tokenIn.Left.Accept<bool>((ISyntacticTreeVisitor<bool>) this);
        this.stringBuilder.Append(' ');
        switch ((int) tokenIn.OperatorKind)
        {
          case 0:
            this.stringBuilder.Append("OR");
            break;
          case 1:
            this.stringBuilder.Append("AND");
            break;
          case 2:
            this.stringBuilder.Append("=");
            break;
          case 3:
            this.stringBuilder.Append("!=");
            break;
          case 4:
            this.stringBuilder.Append(">");
            break;
          case 5:
            this.stringBuilder.Append(">=");
            break;
          case 6:
            this.stringBuilder.Append("<");
            break;
          case 7:
            this.stringBuilder.Append("<=");
            break;
          case 8:
            this.stringBuilder.Append("+");
            break;
          case 9:
            this.stringBuilder.Append("-");
            break;
          case 10:
            this.stringBuilder.Append("*");
            break;
          case 11:
            this.stringBuilder.Append("/");
            break;
          case 12:
            this.stringBuilder.Append("%");
            break;
          default:
            throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "QueryToken of type '{0}' is not supported.", (object) tokenIn.OperatorKind));
        }
        this.stringBuilder.Append(' ');
        if (tokenIn.Left is EndPathToken left && ((PathToken) left).Identifier.Equals("Timestamp") && this.enableTimestampQuery)
          this.stringBuilder.Append(SchemaUtil.GetTimestampEpochSecondString((tokenIn.Right as LiteralToken).Value));
        else
          tokenIn.Right.Accept<bool>((ISyntacticTreeVisitor<bool>) this);
        this.stringBuilder.Append(')');
        return true;
      }

      public bool Visit(InToken tokenIn) => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "QueryToken of type '{0}' is not supported.", (object) ((QueryToken) tokenIn).Kind));

      public bool Visit(DottedIdentifierToken tokenIn) => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "QueryToken of type '{0}' is not supported.", (object) ((QueryToken) tokenIn).Kind));

      public bool Visit(ExpandToken tokenIn) => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "QueryToken of type '{0}' is not supported.", (object) ((QueryToken) tokenIn).Kind));

      public bool Visit(ExpandTermToken tokenIn) => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "QueryToken of type '{0}' is not supported.", (object) ((QueryToken) tokenIn).Kind));

      public bool Visit(FunctionCallToken tokenIn) => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "QueryToken of type '{0}' is not supported.", (object) ((QueryToken) tokenIn).Kind));

      public bool Visit(LambdaToken tokenIn) => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "QueryToken of type '{0}' is not supported.", (object) ((QueryToken) tokenIn).Kind));

      public bool Visit(LiteralToken tokenIn)
      {
        this.stringBuilder.Append(SchemaUtil.ConvertEdmType(tokenIn.Value));
        return true;
      }

      public bool Visit(InnerPathToken tokenIn) => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "QueryToken of type '{0}' is not supported.", (object) ((QueryToken) tokenIn).Kind));

      public bool Visit(OrderByToken tokenIn) => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "QueryToken of type '{0}' is not supported.", (object) ((QueryToken) tokenIn).Kind));

      public bool Visit(EndPathToken tokenIn)
      {
        if (this.isTableQuery)
          this.stringBuilder.Append(ODataFilterTranslator.QueryTokenVisitor.tableEntityDotId);
        else
          this.stringBuilder.Append(EntityTranslator.GetPropertyName(((PathToken) tokenIn).Identifier, this.enableTimestampQuery));
        return true;
      }

      public bool Visit(CustomQueryOptionToken tokenIn) => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "QueryToken of type '{0}' is not supported.", (object) ((QueryToken) tokenIn).Kind));

      public bool Visit(RangeVariableToken tokenIn) => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "QueryToken of type '{0}' is not supported.", (object) ((QueryToken) tokenIn).Kind));

      public bool Visit(SelectToken tokenIn) => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "QueryToken of type '{0}' is not supported.", (object) ((QueryToken) tokenIn).Kind));

      public bool Visit(StarToken tokenIn) => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "QueryToken of type '{0}' is not supported.", (object) ((QueryToken) tokenIn).Kind));

      public bool Visit(UnaryOperatorToken tokenIn)
      {
        if (tokenIn.OperatorKind != 1)
          throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "QueryToken of type '{0}' is not supported.", (object) tokenIn.OperatorKind));
        this.stringBuilder.Append("NOT");
        tokenIn.Operand.Accept<bool>((ISyntacticTreeVisitor<bool>) this);
        this.stringBuilder.Append(' ');
        return true;
      }

      public bool Visit(FunctionParameterToken tokenIn) => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "QueryToken of type '{0}' is not supported.", (object) ((QueryToken) tokenIn).Kind));

      public bool Visit(AggregateToken tokenIn) => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "QueryToken of type '{0}' is not supported.", (object) ((QueryToken) tokenIn).Kind));

      public bool Visit(AggregateExpressionToken tokenIn) => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "QueryToken of type '{0}' is not supported.", (object) ((QueryToken) tokenIn).Kind));

      public bool Visit(EntitySetAggregateToken tokenIn) => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "QueryToken of type '{0}' is not supported.", (object) ((QueryToken) tokenIn).Kind));

      public bool Visit(GroupByToken tokenIn) => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "QueryToken of type '{0}' is not supported.", (object) ((QueryToken) tokenIn).Kind));
    }
  }
}
