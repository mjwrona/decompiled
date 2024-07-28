// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.Extensions.TableExtensionExpressionWriter
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Cosmos.Table.Queryable;
using Microsoft.Azure.Cosmos.Tables.SharedFiles;
using Microsoft.Azure.Documents.Interop.Common.Schema;
using System.Linq.Expressions;

namespace Microsoft.Azure.Cosmos.Table.Extensions
{
  internal sealed class TableExtensionExpressionWriter : ExpressionWriter
  {
    internal new static string ExpressionToString(Expression e) => new TableExtensionExpressionWriter().ConvertExpressionToString(e);

    protected override string TranslateMemberName(string memberName) => EntityTranslator.GetPropertyName(memberName);

    internal override Expression VisitConstant(ConstantExpression c)
    {
      this.builder.Append(SchemaUtil.ConvertEdmType(c.Value));
      return (Expression) c;
    }

    protected override string TranslateOperator(ExpressionType type)
    {
      switch (type)
      {
        case ExpressionType.Add:
        case ExpressionType.AddChecked:
          return "+";
        case ExpressionType.Divide:
          return "/";
        case ExpressionType.Equal:
          return "=";
        case ExpressionType.GreaterThan:
          return ">";
        case ExpressionType.GreaterThanOrEqual:
          return ">=";
        case ExpressionType.LessThan:
          return "<";
        case ExpressionType.LessThanOrEqual:
          return "<=";
        case ExpressionType.Modulo:
          return "%";
        case ExpressionType.Multiply:
        case ExpressionType.MultiplyChecked:
          return "*";
        case ExpressionType.Negate:
        case ExpressionType.NegateChecked:
          return "!";
        case ExpressionType.NotEqual:
          return "!=";
        case ExpressionType.Subtract:
        case ExpressionType.SubtractChecked:
          return "-";
        default:
          return base.TranslateOperator(type);
      }
    }
  }
}
