// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Expressions.ParseException
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Logging;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Expressions
{
  public sealed class ParseException : ExpressionException
  {
    internal ParseException(ParseExceptionKind kind, Token token, string expression)
      : base((ISecretMasker) null, string.Empty)
    {
      this.Expression = expression;
      this.Kind = kind;
      this.RawToken = token?.RawValue;
      this.TokenIndex = token != null ? token.Index : 0;
      string str;
      switch (kind)
      {
        case ParseExceptionKind.ExceededMaxDepth:
          str = ExpressionResources.ExceededMaxExpressionDepth((object) ExpressionConstants.MaxDepth);
          break;
        case ParseExceptionKind.ExceededMaxLength:
          str = ExpressionResources.ExceededMaxExpressionLength((object) ExpressionConstants.MaxLength);
          break;
        case ParseExceptionKind.ExpectedPropertyName:
          str = ExpressionResources.ExpectedPropertyName();
          break;
        case ParseExceptionKind.ExpectedStartParameter:
          str = ExpressionResources.ExpectedStartParameter();
          break;
        case ParseExceptionKind.UnclosedFunction:
          str = ExpressionResources.UnclosedFunction();
          break;
        case ParseExceptionKind.UnclosedIndexer:
          str = ExpressionResources.UnclosedIndexer();
          break;
        case ParseExceptionKind.UnexpectedSymbol:
          str = ExpressionResources.UnexpectedSymbol();
          break;
        case ParseExceptionKind.UnrecognizedValue:
          str = ExpressionResources.UnrecognizedValue();
          break;
        default:
          throw new Exception(string.Format("Unexpected parse exception kind '{0}'.", (object) kind));
      }
      if (token == null)
        this.Message = ExpressionResources.ParseErrorWithFwlink((object) str);
      else
        this.Message = ExpressionResources.ParseErrorWithTokenInfo((object) str, (object) this.RawToken, (object) (this.TokenIndex + 1), (object) this.Expression);
    }

    internal string Expression { get; }

    internal ParseExceptionKind Kind { get; }

    internal string RawToken { get; }

    internal int TokenIndex { get; }

    public override sealed string Message { get; }
  }
}
