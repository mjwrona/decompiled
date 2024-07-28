// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Expressions.ExpressionParser
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.DistributedTask.Expressions
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ExpressionParser
  {
    private ExpressionParserOptions m_parserOptions;

    public ExpressionParser()
      : this((ExpressionParserOptions) null)
    {
    }

    public ExpressionParser(ExpressionParserOptions options) => this.m_parserOptions = options ?? new ExpressionParserOptions();

    public IExpressionNode CreateTree(
      string expression,
      ITraceWriter trace,
      IEnumerable<INamedValueInfo> namedValues,
      IEnumerable<IFunctionInfo> functions)
    {
      ExpressionParser.ParseContext context = new ExpressionParser.ParseContext(expression, trace, namedValues, functions, allowKeywordHyphens: this.m_parserOptions.AllowHyphens);
      context.Trace.Info("Parsing expression: <" + expression + ">");
      return ExpressionParser.CreateTree(context);
    }

    public void ValidateSyntax(string expression, ITraceWriter trace)
    {
      ExpressionParser.ParseContext context = new ExpressionParser.ParseContext(expression, trace, (IEnumerable<INamedValueInfo>) null, (IEnumerable<IFunctionInfo>) null, true, this.m_parserOptions.AllowHyphens);
      context.Trace.Info("Validating expression syntax: <" + expression + ">");
      ExpressionParser.CreateTree(context);
    }

    private static IExpressionNode CreateTree(ExpressionParser.ParseContext context)
    {
      while (ExpressionParser.TryGetNextToken(context))
      {
        switch (context.Token.Kind)
        {
          case TokenKind.StartIndex:
            ExpressionParser.HandleStartIndex(context);
            break;
          case TokenKind.EndIndex:
            ExpressionParser.HandleEndIndex(context);
            break;
          case TokenKind.EndParameter:
            ExpressionParser.HandleEndParameter(context);
            break;
          case TokenKind.Separator:
            ExpressionParser.HandleSeparator(context);
            break;
          case TokenKind.Dereference:
            ExpressionParser.HandleDereference(context);
            break;
          case TokenKind.Wildcard:
            ExpressionParser.HandleWildcard(context);
            break;
          case TokenKind.Boolean:
          case TokenKind.Number:
          case TokenKind.Version:
          case TokenKind.String:
          case TokenKind.ExtensionNamedValue:
            ExpressionParser.HandleValue(context);
            break;
          case TokenKind.WellKnownFunction:
          case TokenKind.ExtensionFunction:
            ExpressionParser.HandleFunction(context);
            break;
          case TokenKind.UnknownKeyword:
            ExpressionParser.HandleUnknownKeyword(context);
            break;
          case TokenKind.Unrecognized:
            throw new ParseException(ParseExceptionKind.UnrecognizedValue, context.Token, context.Expression);
          default:
            throw new ParseException(ParseExceptionKind.UnexpectedSymbol, context.Token, context.Expression);
        }
        if (context.Containers.Count >= ExpressionConstants.MaxDepth)
          throw new ParseException(ParseExceptionKind.ExceededMaxDepth, (Token) null, context.Expression);
      }
      if (context.Containers.Count <= 0)
        return (IExpressionNode) context.Root;
      ExpressionParser.ContainerInfo containerInfo = context.Containers.Peek();
      if (containerInfo.Node is FunctionNode)
        throw new ParseException(ParseExceptionKind.UnclosedFunction, containerInfo.Token, context.Expression);
      throw new ParseException(ParseExceptionKind.UnclosedIndexer, containerInfo.Token, context.Expression);
    }

    private static bool TryGetNextToken(ExpressionParser.ParseContext context)
    {
      context.LastToken = context.Token;
      if (!context.Lexer.TryGetNextToken(ref context.Token))
        return false;
      int count = context.Containers.Count;
      if (count > 0)
      {
        switch (context.Token.Kind)
        {
          case TokenKind.StartParameter:
          case TokenKind.EndIndex:
          case TokenKind.EndParameter:
            --count;
            break;
        }
      }
      string str = string.Empty.PadRight(count * 2, '.');
      switch (context.Token.Kind)
      {
        case TokenKind.StartIndex:
        case TokenKind.StartParameter:
        case TokenKind.EndIndex:
        case TokenKind.EndParameter:
        case TokenKind.Separator:
        case TokenKind.Dereference:
        case TokenKind.Wildcard:
        case TokenKind.WellKnownFunction:
        case TokenKind.ExtensionFunction:
        case TokenKind.ExtensionNamedValue:
        case TokenKind.UnknownKeyword:
          context.Trace.Verbose(str + context.Token.RawValue);
          break;
        case TokenKind.Boolean:
          context.Trace.Verbose(str + ExpressionUtil.FormatValue((ISecretMasker) null, context.Token.ParsedValue, ValueKind.Boolean));
          break;
        case TokenKind.Number:
          context.Trace.Verbose(str + ExpressionUtil.FormatValue((ISecretMasker) null, context.Token.ParsedValue, ValueKind.Number));
          break;
        case TokenKind.Version:
          context.Trace.Verbose(str + ExpressionUtil.FormatValue((ISecretMasker) null, context.Token.ParsedValue, ValueKind.Version));
          break;
        case TokenKind.String:
          context.Trace.Verbose(str + ExpressionUtil.FormatValue((ISecretMasker) null, context.Token.ParsedValue, ValueKind.String));
          break;
        case TokenKind.PropertyName:
        case TokenKind.Unrecognized:
          context.Trace.Verbose(string.Format("{0}{1} {2}", (object) str, (object) context.Token.Kind, (object) ExpressionUtil.FormatValue((ISecretMasker) null, (object) context.Token.RawValue, ValueKind.String)));
          break;
        default:
          throw new NotSupportedException(string.Format("Unexpected token kind: {0}", (object) context.Token.Kind));
      }
      return true;
    }

    private static void HandleStartIndex(ExpressionParser.ParseContext context)
    {
      if (context.LastToken == null || context.LastToken.Kind != TokenKind.EndParameter && context.LastToken.Kind != TokenKind.EndIndex && context.LastToken.Kind != TokenKind.PropertyName && context.LastToken.Kind != TokenKind.ExtensionNamedValue && context.LastToken.Kind != TokenKind.UnknownKeyword && context.LastToken.Kind != TokenKind.Wildcard)
        throw new ParseException(ParseExceptionKind.UnexpectedSymbol, context.Token, context.Expression);
      IndexerNode node1 = new IndexerNode();
      ExpressionNode node2;
      if (context.Containers.Count > 0)
      {
        ContainerNode node3 = context.Containers.Peek().Node;
        int index = node3.Parameters.Count - 1;
        node2 = node3.Parameters[index];
        node3.ReplaceParameter(index, (ExpressionNode) node1);
      }
      else
      {
        node2 = context.Root;
        context.Root = (ExpressionNode) node1;
      }
      node1.AddParameter(node2);
      context.Containers.Push(new ExpressionParser.ContainerInfo()
      {
        Node = (ContainerNode) node1,
        Token = context.Token
      });
    }

    private static void HandleDereference(ExpressionParser.ParseContext context)
    {
      if (context.LastToken == null || context.LastToken.Kind != TokenKind.EndParameter && context.LastToken.Kind != TokenKind.EndIndex && context.LastToken.Kind != TokenKind.PropertyName && context.LastToken.Kind != TokenKind.ExtensionNamedValue && context.LastToken.Kind != TokenKind.UnknownKeyword && context.LastToken.Kind != TokenKind.Wildcard)
        throw new ParseException(ParseExceptionKind.UnexpectedSymbol, context.Token, context.Expression);
      IndexerNode node1 = new IndexerNode();
      ExpressionNode node2;
      if (context.Containers.Count > 0)
      {
        ContainerNode node3 = context.Containers.Peek().Node;
        int index = node3.Parameters.Count - 1;
        node2 = node3.Parameters[index];
        node3.ReplaceParameter(index, (ExpressionNode) node1);
      }
      else
      {
        node2 = context.Root;
        context.Root = (ExpressionNode) node1;
      }
      node1.AddParameter(node2);
      if (!ExpressionParser.TryGetNextToken(context))
        throw new ParseException(ParseExceptionKind.ExpectedPropertyName, context.LastToken, context.Expression);
      if (context.Token.Kind == TokenKind.PropertyName)
      {
        node1.AddParameter((ExpressionNode) new LiteralValueNode((object) context.Token.RawValue));
      }
      else
      {
        if (context.Token.Kind != TokenKind.Wildcard)
          throw new ParseException(ParseExceptionKind.UnexpectedSymbol, context.Token, context.Expression);
        node1.AddParameter((ExpressionNode) new LiteralValueNode((object) context.Token.RawValue));
        node1.AddParameter((ExpressionNode) new LiteralValueNode((object) true));
      }
    }

    private static void HandleWildcard(ExpressionParser.ParseContext context)
    {
      if (context.LastToken == null || context.LastToken.Kind != TokenKind.StartIndex)
        throw new ParseException(ParseExceptionKind.UnexpectedSymbol, context.Token, context.Expression);
      context.Containers.Peek().Node.AddParameter((ExpressionNode) new LiteralValueNode((object) context.Token.RawValue));
      context.Containers.Peek().Node.AddParameter((ExpressionNode) new LiteralValueNode((object) true));
    }

    private static void HandleEndParameter(ExpressionParser.ParseContext context)
    {
      ExpressionParser.ContainerInfo containerInfo = context.Containers.Count > 0 ? context.Containers.Peek() : (ExpressionParser.ContainerInfo) null;
      if (containerInfo == null || !(containerInfo.Node is FunctionNode) || containerInfo.Node.Parameters.Count < ExpressionParser.GetMinParamCount(context, containerInfo.Token) || containerInfo.Node.Parameters.Count > ExpressionParser.GetMaxParamCount(context, containerInfo.Token) || context.LastToken.Kind == TokenKind.Separator)
        throw new ParseException(ParseExceptionKind.UnexpectedSymbol, context.Token, context.Expression);
      context.Containers.Pop();
    }

    private static void HandleEndIndex(ExpressionParser.ParseContext context)
    {
      IndexerNode node = context.Containers.Count > 0 ? context.Containers.Peek().Node as IndexerNode : (IndexerNode) null;
      if (node == null || node.Parameters.Count != 2 && node.Parameters.Count != 3)
        throw new ParseException(ParseExceptionKind.UnexpectedSymbol, context.Token, context.Expression);
      context.Containers.Pop();
    }

    private static void HandleUnknownKeyword(ExpressionParser.ParseContext context)
    {
      if (!context.AllowUnknownKeywords)
        throw new ParseException(ParseExceptionKind.UnrecognizedValue, context.Token, context.Expression);
      if (ExpressionParser.HandleFunction(context, true))
        return;
      ExpressionParser.HandleValue(context);
    }

    private static void HandleValue(ExpressionParser.ParseContext context)
    {
      if (context.LastToken != null && context.LastToken.Kind != TokenKind.StartIndex && context.LastToken.Kind != TokenKind.StartParameter && context.LastToken.Kind != TokenKind.Separator)
        throw new ParseException(ParseExceptionKind.UnexpectedSymbol, context.Token, context.Expression);
      ExpressionNode node;
      switch (context.Token.Kind)
      {
        case TokenKind.ExtensionNamedValue:
          string rawValue = context.Token.RawValue;
          node = (ExpressionNode) context.ExtensionNamedValues[rawValue].CreateNode();
          node.Name = rawValue;
          break;
        case TokenKind.UnknownKeyword:
          node = (ExpressionNode) new UnknownNamedValueNode();
          node.Name = context.Token.RawValue;
          break;
        default:
          node = (ExpressionNode) new LiteralValueNode(context.Token.ParsedValue);
          break;
      }
      if (context.Root == null)
        context.Root = node;
      else
        context.Containers.Peek().Node.AddParameter(node);
    }

    private static void HandleSeparator(ExpressionParser.ParseContext context)
    {
      ExpressionParser.ContainerInfo containerInfo = context.Containers.Count > 0 ? context.Containers.Peek() : (ExpressionParser.ContainerInfo) null;
      if (containerInfo == null || !(containerInfo.Node is FunctionNode) || containerInfo.Node.Parameters.Count < 1 || containerInfo.Node.Parameters.Count >= ExpressionParser.GetMaxParamCount(context, containerInfo.Token) || context.LastToken.Kind == TokenKind.Separator)
        throw new ParseException(ParseExceptionKind.UnexpectedSymbol, context.Token, context.Expression);
    }

    private static bool HandleFunction(ExpressionParser.ParseContext context, bool bestEffort = false)
    {
      if (context.LastToken != null && context.LastToken.Kind != TokenKind.Separator && context.LastToken.Kind != TokenKind.StartIndex && context.LastToken.Kind != TokenKind.StartParameter)
      {
        if (bestEffort)
          return false;
        throw new ParseException(ParseExceptionKind.UnexpectedSymbol, context.Token, context.Expression);
      }
      if (bestEffort)
      {
        Token token = (Token) null;
        if (!context.Lexer.TryPeekNextToken(ref token) || token.Kind != TokenKind.StartParameter)
          return false;
        ExpressionParser.TryGetNextToken(context);
      }
      else if (!ExpressionParser.TryGetNextToken(context) || context.Token.Kind != TokenKind.StartParameter)
        throw new ParseException(ParseExceptionKind.ExpectedStartParameter, context.LastToken, context.Expression);
      string rawValue = context.LastToken.RawValue;
      FunctionNode node;
      switch (context.LastToken.Kind)
      {
        case TokenKind.WellKnownFunction:
          node = ExpressionConstants.WellKnownFunctions[rawValue].CreateNode();
          node.Name = rawValue;
          break;
        case TokenKind.ExtensionFunction:
          node = context.ExtensionFunctions[rawValue].CreateNode();
          node.Name = rawValue;
          break;
        case TokenKind.UnknownKeyword:
          node = (FunctionNode) new UnknownFunctionNode();
          node.Name = rawValue;
          break;
        default:
          throw new NotSupportedException(string.Format("Unexpected function token name: '{0}'", (object) context.LastToken.Kind));
      }
      if (context.Root == null)
        context.Root = (ExpressionNode) node;
      else
        context.Containers.Peek().Node.AddParameter((ExpressionNode) node);
      context.Containers.Push(new ExpressionParser.ContainerInfo()
      {
        Node = (ContainerNode) node,
        Token = context.LastToken
      });
      return true;
    }

    private static int GetMinParamCount(ExpressionParser.ParseContext context, Token token)
    {
      switch (token.Kind)
      {
        case TokenKind.WellKnownFunction:
          return ExpressionConstants.WellKnownFunctions[token.RawValue].MinParameters;
        case TokenKind.ExtensionFunction:
          return context.ExtensionFunctions[token.RawValue].MinParameters;
        case TokenKind.UnknownKeyword:
          return 0;
        default:
          throw new NotSupportedException(string.Format("Unexpected token kind '{0}'. Unable to determine min param count.", (object) token.Kind));
      }
    }

    private static int GetMaxParamCount(ExpressionParser.ParseContext context, Token token)
    {
      switch (token.Kind)
      {
        case TokenKind.WellKnownFunction:
          return ExpressionConstants.WellKnownFunctions[token.RawValue].MaxParameters;
        case TokenKind.ExtensionFunction:
          return context.ExtensionFunctions[token.RawValue].MaxParameters;
        case TokenKind.UnknownKeyword:
          return int.MaxValue;
        default:
          throw new NotSupportedException(string.Format("Unexpected token kind '{0}'. Unable to determine max param count.", (object) token.Kind));
      }
    }

    private sealed class ContainerInfo
    {
      public ContainerNode Node { get; set; }

      public Token Token { get; set; }
    }

    private sealed class ParseContext
    {
      public readonly bool AllowUnknownKeywords;
      public readonly Stack<ExpressionParser.ContainerInfo> Containers = new Stack<ExpressionParser.ContainerInfo>();
      public readonly string Expression;
      public readonly Dictionary<string, IFunctionInfo> ExtensionFunctions = new Dictionary<string, IFunctionInfo>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      public readonly Dictionary<string, INamedValueInfo> ExtensionNamedValues = new Dictionary<string, INamedValueInfo>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      public readonly LexicalAnalyzer Lexer;
      public readonly ITraceWriter Trace;
      public Token Token;
      public Token LastToken;
      public ExpressionNode Root;

      public ParseContext(
        string expression,
        ITraceWriter trace,
        IEnumerable<INamedValueInfo> namedValues,
        IEnumerable<IFunctionInfo> functions,
        bool allowUnknownKeywords = false,
        bool allowKeywordHyphens = false)
      {
        this.Expression = expression ?? string.Empty;
        if (this.Expression.Length > ExpressionConstants.MaxLength)
          throw new ParseException(ParseExceptionKind.ExceededMaxLength, (Token) null, this.Expression);
        this.Trace = trace ?? (ITraceWriter) new ExpressionParser.ParseContext.NoOperationTraceWriter();
        foreach (INamedValueInfo namedValueInfo in (IEnumerable<INamedValueInfo>) ((object) namedValues ?? (object) Array.Empty<INamedValueInfo>()))
          this.ExtensionNamedValues.Add(namedValueInfo.Name, namedValueInfo);
        foreach (IFunctionInfo functionInfo in (IEnumerable<IFunctionInfo>) ((object) functions ?? (object) Array.Empty<IFunctionInfo>()))
          this.ExtensionFunctions.Add(functionInfo.Name, functionInfo);
        this.AllowUnknownKeywords = allowUnknownKeywords;
        this.Lexer = new LexicalAnalyzer(this.Expression, (IEnumerable<string>) this.ExtensionNamedValues.Keys, (IEnumerable<string>) this.ExtensionFunctions.Keys, allowKeywordHyphens);
      }

      private class NoOperationTraceWriter : ITraceWriter
      {
        public void Info(string message)
        {
        }

        public void Verbose(string message)
        {
        }
      }
    }
  }
}
