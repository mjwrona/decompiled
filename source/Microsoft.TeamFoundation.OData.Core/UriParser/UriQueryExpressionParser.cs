// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.UriQueryExpressionParser
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.UriParser.Aggregation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.OData.UriParser
{
  public sealed class UriQueryExpressionParser
  {
    private readonly int maxDepth;
    private static readonly string supportedKeywords = string.Join("|", new string[5]
    {
      "aggregate",
      "filter",
      "groupby",
      "compute",
      "expand"
    });
    private readonly HashSet<string> parameters = new HashSet<string>((IEqualityComparer<string>) StringComparer.Ordinal)
    {
      "$it"
    };
    private int recursionDepth;
    private ExpressionLexer lexer;
    private bool enableCaseInsensitiveBuiltinIdentifier;
    private int parseAggregateExpressionDepth;
    private Stack<QueryToken> aggregateExpressionParents = new Stack<QueryToken>();

    public UriQueryExpressionParser(int maxDepth)
      : this(maxDepth, false)
    {
    }

    internal UriQueryExpressionParser(int maxDepth, bool enableCaseInsensitiveBuiltinIdentifier = false)
    {
      this.maxDepth = maxDepth;
      this.enableCaseInsensitiveBuiltinIdentifier = enableCaseInsensitiveBuiltinIdentifier;
    }

    internal UriQueryExpressionParser(int maxDepth, ExpressionLexer lexer)
      : this(maxDepth)
    {
      this.lexer = lexer;
    }

    internal ExpressionLexer Lexer => this.lexer;

    private bool IsInAggregateExpression => this.parseAggregateExpressionDepth > 0;

    public QueryToken ParseFilter(string filter) => this.ParseExpressionText(filter);

    internal static LiteralToken TryParseLiteral(ExpressionLexer lexer)
    {
      switch (lexer.CurrentToken.Kind)
      {
        case ExpressionTokenKind.NullLiteral:
          return UriQueryExpressionParser.ParseNullLiteral(lexer);
        case ExpressionTokenKind.BooleanLiteral:
        case ExpressionTokenKind.StringLiteral:
        case ExpressionTokenKind.IntegerLiteral:
        case ExpressionTokenKind.Int64Literal:
        case ExpressionTokenKind.SingleLiteral:
        case ExpressionTokenKind.DateTimeOffsetLiteral:
        case ExpressionTokenKind.DurationLiteral:
        case ExpressionTokenKind.DecimalLiteral:
        case ExpressionTokenKind.DoubleLiteral:
        case ExpressionTokenKind.GuidLiteral:
        case ExpressionTokenKind.BinaryLiteral:
        case ExpressionTokenKind.GeographyLiteral:
        case ExpressionTokenKind.GeometryLiteral:
        case ExpressionTokenKind.QuotedLiteral:
        case ExpressionTokenKind.DateLiteral:
        case ExpressionTokenKind.TimeOfDayLiteral:
        case ExpressionTokenKind.CustomTypeLiteral:
          IEdmTypeReference edmTypeReference = lexer.CurrentToken.GetLiteralEdmTypeReference();
          string edmConstantNames = UriQueryExpressionParser.GetEdmConstantNames(edmTypeReference);
          return UriQueryExpressionParser.ParseTypedLiteral(lexer, edmTypeReference, edmConstantNames);
        case ExpressionTokenKind.BracedExpression:
        case ExpressionTokenKind.BracketedExpression:
        case ExpressionTokenKind.ParenthesesExpression:
          LiteralToken literal = new LiteralToken((object) lexer.CurrentToken.Text, lexer.CurrentToken.Text);
          lexer.NextToken();
          return literal;
        default:
          return (LiteralToken) null;
      }
    }

    internal static string GetEdmConstantNames(IEdmTypeReference edmTypeReference)
    {
      switch (edmTypeReference.PrimitiveKind())
      {
        case EdmPrimitiveTypeKind.Binary:
          return "Edm.Binary";
        case EdmPrimitiveTypeKind.Boolean:
          return "Edm.Boolean";
        case EdmPrimitiveTypeKind.DateTimeOffset:
          return "Edm.DateTimeOffset";
        case EdmPrimitiveTypeKind.Decimal:
          return "Edm.Decimal";
        case EdmPrimitiveTypeKind.Double:
          return "Edm.Double";
        case EdmPrimitiveTypeKind.Guid:
          return "Edm.Guid";
        case EdmPrimitiveTypeKind.Int32:
          return "Edm.Int32";
        case EdmPrimitiveTypeKind.Int64:
          return "Edm.Int64";
        case EdmPrimitiveTypeKind.Single:
          return "Edm.Single";
        case EdmPrimitiveTypeKind.String:
          return "Edm.String";
        case EdmPrimitiveTypeKind.Duration:
          return "Edm.Duration";
        case EdmPrimitiveTypeKind.Geography:
          return "Edm.Geography";
        case EdmPrimitiveTypeKind.Geometry:
          return "Edm.Geometry";
        case EdmPrimitiveTypeKind.Date:
          return "Edm.Date";
        case EdmPrimitiveTypeKind.TimeOfDay:
          return "Edm.TimeOfDay";
        default:
          return edmTypeReference.Definition.FullTypeName();
      }
    }

    internal ComputeToken ParseCompute()
    {
      this.lexer.NextToken();
      if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.OpenParen)
        throw UriQueryExpressionParser.ParseError(Microsoft.OData.Strings.UriQueryExpressionParser_OpenParenExpected((object) this.lexer.CurrentToken.Position, (object) this.lexer.ExpressionText));
      this.lexer.NextToken();
      List<ComputeExpressionToken> expressions = new List<ComputeExpressionToken>();
      while (true)
      {
        ComputeExpressionToken computeExpression = this.ParseComputeExpression();
        expressions.Add(computeExpression);
        if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.Comma)
          this.lexer.NextToken();
        else
          break;
      }
      if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.CloseParen)
        throw UriQueryExpressionParser.ParseError(Microsoft.OData.Strings.UriQueryExpressionParser_CloseParenOrCommaExpected((object) this.lexer.CurrentToken.Position, (object) this.lexer.ExpressionText));
      this.lexer.NextToken();
      return new ComputeToken((IEnumerable<ComputeExpressionToken>) expressions);
    }

    internal ComputeToken ParseCompute(string compute)
    {
      List<ComputeExpressionToken> expressions = new List<ComputeExpressionToken>();
      if (string.IsNullOrEmpty(compute))
        return new ComputeToken((IEnumerable<ComputeExpressionToken>) expressions);
      this.recursionDepth = 0;
      this.lexer = UriQueryExpressionParser.CreateLexerForFilterOrOrderByOrApplyExpression(compute);
      while (true)
      {
        ComputeExpressionToken computeExpression = this.ParseComputeExpression();
        expressions.Add(computeExpression);
        if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.Comma)
          this.lexer.NextToken();
        else
          break;
      }
      this.lexer.ValidateToken(ExpressionTokenKind.End);
      return new ComputeToken((IEnumerable<ComputeExpressionToken>) expressions);
    }

    internal ExpandToken ParseExpand()
    {
      this.lexer.NextToken();
      if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.OpenParen)
        throw UriQueryExpressionParser.ParseError(Microsoft.OData.Strings.UriQueryExpressionParser_OpenParenExpected((object) this.lexer.CurrentToken.Position, (object) this.lexer.ExpressionText));
      this.lexer.NextToken();
      List<ExpandTermToken> expandTerms = new List<ExpandTermToken>();
      PathSegmentToken term = new SelectExpandTermParser(this.lexer, this.maxDepth - 1, false).ParseTerm(true);
      QueryToken filterOption = (QueryToken) null;
      ExpandToken expandOption = (ExpandToken) null;
      while (this.lexer.CurrentToken.Kind == ExpressionTokenKind.Comma)
      {
        this.lexer.NextToken();
        if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.Identifier)
        {
          switch (this.lexer.CurrentToken.GetIdentifier())
          {
            case "filter":
              filterOption = this.ParseApplyFilter();
              continue;
            case "expand":
              ExpandToken expand = this.ParseExpand();
              expandOption = expandOption == null ? expand : new ExpandToken(expandOption.ExpandTerms.Concat<ExpandTermToken>(expand.ExpandTerms));
              continue;
            default:
              throw UriQueryExpressionParser.ParseError(Microsoft.OData.Strings.UriQueryExpressionParser_KeywordOrIdentifierExpected((object) UriQueryExpressionParser.supportedKeywords, (object) this.lexer.CurrentToken.Position, (object) this.lexer.ExpressionText));
          }
        }
      }
      if (filterOption == null && expandOption == null)
        throw UriQueryExpressionParser.ParseError(Microsoft.OData.Strings.UriQueryExpressionParser_InnerMostExpandRequireFilter((object) this.lexer.CurrentToken.Position, (object) this.lexer.ExpressionText));
      ExpandTermToken expandTermToken = new ExpandTermToken(term, filterOption, (IEnumerable<OrderByToken>) null, new long?(), new long?(), new bool?(), new long?(), (QueryToken) null, (SelectToken) null, expandOption);
      expandTerms.Add(expandTermToken);
      if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.CloseParen)
        throw UriQueryExpressionParser.ParseError(Microsoft.OData.Strings.UriQueryExpressionParser_CloseParenOrCommaExpected((object) this.lexer.CurrentToken.Position, (object) this.lexer.ExpressionText));
      this.lexer.NextToken();
      return new ExpandToken((IEnumerable<ExpandTermToken>) expandTerms);
    }

    internal IEnumerable<QueryToken> ParseApply(string apply)
    {
      List<QueryToken> list = new List<QueryToken>();
      if (string.IsNullOrEmpty(apply))
        return (IEnumerable<QueryToken>) list;
      this.recursionDepth = 0;
      this.lexer = UriQueryExpressionParser.CreateLexerForFilterOrOrderByOrApplyExpression(apply);
      while (true)
      {
        switch (this.lexer.CurrentToken.GetIdentifier())
        {
          case "aggregate":
            list.Add((QueryToken) this.ParseAggregate());
            break;
          case "filter":
            list.Add(this.ParseApplyFilter());
            break;
          case "groupby":
            list.Add((QueryToken) this.ParseGroupBy());
            break;
          case "compute":
            list.Add((QueryToken) this.ParseCompute());
            break;
          case "expand":
            list.Add((QueryToken) this.ParseExpand());
            break;
          default:
            goto label_9;
        }
        if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.Slash)
          this.lexer.NextToken();
        else
          goto label_12;
      }
label_9:
      throw UriQueryExpressionParser.ParseError(Microsoft.OData.Strings.UriQueryExpressionParser_KeywordOrIdentifierExpected((object) UriQueryExpressionParser.supportedKeywords, (object) this.lexer.CurrentToken.Position, (object) this.lexer.ExpressionText));
label_12:
      this.lexer.ValidateToken(ExpressionTokenKind.End);
      return (IEnumerable<QueryToken>) new ReadOnlyCollection<QueryToken>((IList<QueryToken>) list);
    }

    internal AggregateToken ParseAggregate()
    {
      this.lexer.NextToken();
      return new AggregateToken((IEnumerable<AggregateTokenBase>) this.ParseAggregateExpressions());
    }

    internal List<AggregateTokenBase> ParseAggregateExpressions()
    {
      if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.OpenParen)
        throw UriQueryExpressionParser.ParseError(Microsoft.OData.Strings.UriQueryExpressionParser_OpenParenExpected((object) this.lexer.CurrentToken.Position, (object) this.lexer.ExpressionText));
      this.lexer.NextToken();
      List<AggregateTokenBase> aggregateExpressions = new List<AggregateTokenBase>();
      while (true)
      {
        aggregateExpressions.Add(this.ParseAggregateExpression());
        if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.Comma)
          this.lexer.NextToken();
        else
          break;
      }
      if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.CloseParen)
        throw UriQueryExpressionParser.ParseError(Microsoft.OData.Strings.UriQueryExpressionParser_CloseParenOrCommaExpected((object) this.lexer.CurrentToken.Position, (object) this.lexer.ExpressionText));
      this.lexer.NextToken();
      return aggregateExpressions;
    }

    internal AggregateTokenBase ParseAggregateExpression()
    {
      try
      {
        ++this.parseAggregateExpressionDepth;
        QueryToken logicalOr = this.ParseLogicalOr();
        if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.OpenParen)
        {
          this.aggregateExpressionParents.Push(logicalOr);
          List<AggregateTokenBase> aggregateExpressions = this.ParseAggregateExpressions();
          this.aggregateExpressionParents.Pop();
          return (AggregateTokenBase) new EntitySetAggregateToken(logicalOr, (IEnumerable<AggregateTokenBase>) aggregateExpressions);
        }
        AggregationMethodDefinition methodDefinition = !(logicalOr is EndPathToken endPathToken) || !(endPathToken.Identifier == "$count") ? this.ParseAggregateWith() : AggregationMethodDefinition.VirtualPropertyCount;
        StringLiteralToken aggregateAs = this.ParseAggregateAs();
        return (AggregateTokenBase) new AggregateExpressionToken(logicalOr, methodDefinition, aggregateAs.Text);
      }
      finally
      {
        --this.parseAggregateExpressionDepth;
      }
    }

    internal GroupByToken ParseGroupBy()
    {
      this.lexer.NextToken();
      if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.OpenParen)
        throw UriQueryExpressionParser.ParseError(Microsoft.OData.Strings.UriQueryExpressionParser_OpenParenExpected((object) this.lexer.CurrentToken.Position, (object) this.lexer.ExpressionText));
      this.lexer.NextToken();
      if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.OpenParen)
        throw UriQueryExpressionParser.ParseError(Microsoft.OData.Strings.UriQueryExpressionParser_OpenParenExpected((object) this.lexer.CurrentToken.Position, (object) this.lexer.ExpressionText));
      this.lexer.NextToken();
      List<EndPathToken> properties = new List<EndPathToken>();
      while (this.ParsePrimary() is EndPathToken primary)
      {
        properties.Add(primary);
        if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.Comma)
        {
          this.lexer.NextToken();
        }
        else
        {
          if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.CloseParen)
            throw UriQueryExpressionParser.ParseError(Microsoft.OData.Strings.UriQueryExpressionParser_CloseParenOrOperatorExpected((object) this.lexer.CurrentToken.Position, (object) this.lexer.ExpressionText));
          this.lexer.NextToken();
          ApplyTransformationToken child = (ApplyTransformationToken) null;
          if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.Comma)
          {
            this.lexer.NextToken();
            if (!this.TokenIdentifierIs("aggregate"))
              throw UriQueryExpressionParser.ParseError(Microsoft.OData.Strings.UriQueryExpressionParser_KeywordOrIdentifierExpected((object) "aggregate", (object) this.lexer.CurrentToken.Position, (object) this.lexer.ExpressionText));
            child = (ApplyTransformationToken) this.ParseAggregate();
          }
          if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.CloseParen)
            throw UriQueryExpressionParser.ParseError(Microsoft.OData.Strings.UriQueryExpressionParser_CloseParenOrCommaExpected((object) this.lexer.CurrentToken.Position, (object) this.lexer.ExpressionText));
          this.lexer.NextToken();
          return new GroupByToken((IEnumerable<EndPathToken>) properties, child);
        }
      }
      throw UriQueryExpressionParser.ParseError(Microsoft.OData.Strings.UriQueryExpressionParser_ExpressionExpected((object) this.lexer.CurrentToken.Position, (object) this.lexer.ExpressionText));
    }

    internal QueryToken ParseApplyFilter()
    {
      this.lexer.NextToken();
      return this.ParseParenExpression();
    }

    internal ComputeExpressionToken ParseComputeExpression() => new ComputeExpressionToken(this.ParseExpression(), this.ParseAggregateAs().Text);

    internal QueryToken ParseExpressionText(string expressionText)
    {
      this.recursionDepth = 0;
      this.lexer = UriQueryExpressionParser.CreateLexerForFilterOrOrderByOrApplyExpression(expressionText);
      QueryToken expression = this.ParseExpression();
      this.lexer.ValidateToken(ExpressionTokenKind.End);
      return expression;
    }

    internal IEnumerable<OrderByToken> ParseOrderBy(string orderBy)
    {
      this.recursionDepth = 0;
      this.lexer = UriQueryExpressionParser.CreateLexerForFilterOrOrderByOrApplyExpression(orderBy);
      List<OrderByToken> list = new List<OrderByToken>();
      while (true)
      {
        QueryToken expression = this.ParseExpression();
        bool flag = true;
        if (this.TokenIdentifierIs("asc"))
          this.lexer.NextToken();
        else if (this.TokenIdentifierIs("desc"))
        {
          this.lexer.NextToken();
          flag = false;
        }
        OrderByToken orderByToken = new OrderByToken(expression, flag ? OrderByDirection.Ascending : OrderByDirection.Descending);
        list.Add(orderByToken);
        if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.Comma)
          this.lexer.NextToken();
        else
          break;
      }
      this.lexer.ValidateToken(ExpressionTokenKind.End);
      return (IEnumerable<OrderByToken>) new ReadOnlyCollection<OrderByToken>((IList<OrderByToken>) list);
    }

    internal QueryToken ParseExpression()
    {
      this.RecurseEnter();
      QueryToken logicalOr = this.ParseLogicalOr();
      this.RecurseLeave();
      return logicalOr;
    }

    private static ExpressionLexer CreateLexerForFilterOrOrderByOrApplyExpression(string expression) => new ExpressionLexer(expression, true, false, true);

    private static Exception ParseError(string message) => (Exception) new ODataException(message);

    private static Exception ParseError(string message, UriLiteralParsingException parsingException) => (Exception) new ODataException(message, (Exception) parsingException);

    private static FunctionParameterAliasToken ParseParameterAlias(ExpressionLexer lexer)
    {
      FunctionParameterAliasToken parameterAlias = new FunctionParameterAliasToken(lexer.CurrentToken.Text);
      lexer.NextToken();
      return parameterAlias;
    }

    private static LiteralToken ParseTypedLiteral(
      ExpressionLexer lexer,
      IEdmTypeReference targetTypeReference,
      string targetTypeName)
    {
      UriLiteralParsingException parsingException;
      object uriStringToType = DefaultUriLiteralParser.Instance.ParseUriStringToType(lexer.CurrentToken.Text, targetTypeReference, out parsingException);
      if (uriStringToType == null)
      {
        if (parsingException == null)
          throw UriQueryExpressionParser.ParseError(Microsoft.OData.Strings.UriQueryExpressionParser_UnrecognizedLiteral((object) targetTypeName, (object) lexer.CurrentToken.Text, (object) lexer.CurrentToken.Position, (object) lexer.ExpressionText));
        throw UriQueryExpressionParser.ParseError(Microsoft.OData.Strings.UriQueryExpressionParser_UnrecognizedLiteralWithReason((object) targetTypeName, (object) lexer.CurrentToken.Text, (object) lexer.CurrentToken.Position, (object) lexer.ExpressionText, (object) parsingException.Message), parsingException);
      }
      LiteralToken typedLiteral = new LiteralToken(uriStringToType, lexer.CurrentToken.Text);
      lexer.NextToken();
      return typedLiteral;
    }

    private static LiteralToken ParseNullLiteral(ExpressionLexer lexer)
    {
      LiteralToken nullLiteral = new LiteralToken((object) null, lexer.CurrentToken.Text);
      lexer.NextToken();
      return nullLiteral;
    }

    private QueryToken ParseLogicalOr()
    {
      this.RecurseEnter();
      QueryToken left = this.ParseLogicalAnd();
      while (this.TokenIdentifierIs("or"))
      {
        this.lexer.NextToken();
        QueryToken logicalAnd = this.ParseLogicalAnd();
        left = (QueryToken) new BinaryOperatorToken(BinaryOperatorKind.Or, left, logicalAnd);
      }
      this.RecurseLeave();
      return left;
    }

    private QueryToken ParseLogicalAnd()
    {
      this.RecurseEnter();
      QueryToken left = this.ParseComparison();
      while (this.TokenIdentifierIs("and"))
      {
        this.lexer.NextToken();
        QueryToken comparison = this.ParseComparison();
        left = (QueryToken) new BinaryOperatorToken(BinaryOperatorKind.And, left, comparison);
      }
      this.RecurseLeave();
      return left;
    }

    private QueryToken ParseComparison()
    {
      this.RecurseEnter();
      QueryToken left = this.ParseAdditive();
      while (true)
      {
        while (!this.TokenIdentifierIs("in"))
        {
          BinaryOperatorKind operatorKind;
          if (this.TokenIdentifierIs("eq"))
            operatorKind = BinaryOperatorKind.Equal;
          else if (this.TokenIdentifierIs("ne"))
            operatorKind = BinaryOperatorKind.NotEqual;
          else if (this.TokenIdentifierIs("gt"))
            operatorKind = BinaryOperatorKind.GreaterThan;
          else if (this.TokenIdentifierIs("ge"))
            operatorKind = BinaryOperatorKind.GreaterThanOrEqual;
          else if (this.TokenIdentifierIs("lt"))
            operatorKind = BinaryOperatorKind.LessThan;
          else if (this.TokenIdentifierIs("le"))
            operatorKind = BinaryOperatorKind.LessThanOrEqual;
          else if (this.TokenIdentifierIs("has"))
          {
            operatorKind = BinaryOperatorKind.Has;
          }
          else
          {
            this.RecurseLeave();
            return left;
          }
          this.lexer.NextToken();
          QueryToken additive = this.ParseAdditive();
          left = (QueryToken) new BinaryOperatorToken(operatorKind, left, additive);
        }
        this.lexer.NextToken();
        QueryToken additive1 = this.ParseAdditive();
        left = (QueryToken) new InToken(left, additive1);
      }
    }

    private QueryToken ParseAdditive()
    {
      this.RecurseEnter();
      QueryToken left = this.ParseMultiplicative();
      while (this.TokenIdentifierIs("add") || this.TokenIdentifierIs("sub"))
      {
        BinaryOperatorKind operatorKind = !this.TokenIdentifierIs("add") ? BinaryOperatorKind.Subtract : BinaryOperatorKind.Add;
        this.lexer.NextToken();
        QueryToken multiplicative = this.ParseMultiplicative();
        left = (QueryToken) new BinaryOperatorToken(operatorKind, left, multiplicative);
      }
      this.RecurseLeave();
      return left;
    }

    private QueryToken ParseMultiplicative()
    {
      this.RecurseEnter();
      QueryToken left = this.ParseUnary();
      while (this.TokenIdentifierIs("mul") || this.TokenIdentifierIs("div") || this.TokenIdentifierIs("mod"))
      {
        BinaryOperatorKind operatorKind = !this.TokenIdentifierIs("mul") ? (!this.TokenIdentifierIs("div") ? BinaryOperatorKind.Modulo : BinaryOperatorKind.Divide) : BinaryOperatorKind.Multiply;
        this.lexer.NextToken();
        QueryToken unary = this.ParseUnary();
        left = (QueryToken) new BinaryOperatorToken(operatorKind, left, unary);
      }
      this.RecurseLeave();
      return left;
    }

    private QueryToken ParseUnary()
    {
      this.RecurseEnter();
      if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.Minus || this.TokenIdentifierIs("not"))
      {
        ExpressionToken currentToken1 = this.lexer.CurrentToken;
        this.lexer.NextToken();
        if (currentToken1.Kind == ExpressionTokenKind.Minus && ExpressionLexerUtils.IsNumeric(this.lexer.CurrentToken.Kind))
        {
          ExpressionToken currentToken2 = this.lexer.CurrentToken;
          currentToken2.Text = "-" + currentToken2.Text;
          currentToken2.Position = currentToken1.Position;
          this.lexer.CurrentToken = currentToken2;
          this.RecurseLeave();
          return this.ParsePrimary();
        }
        QueryToken unary = this.ParseUnary();
        UnaryOperatorKind operatorKind = currentToken1.Kind != ExpressionTokenKind.Minus ? UnaryOperatorKind.Not : UnaryOperatorKind.Negate;
        this.RecurseLeave();
        return (QueryToken) new UnaryOperatorToken(operatorKind, unary);
      }
      this.RecurseLeave();
      return this.ParsePrimary();
    }

    private QueryToken ParsePrimary()
    {
      this.RecurseEnter();
      QueryToken parent1 = this.aggregateExpressionParents.Count > 0 ? this.aggregateExpressionParents.Peek() : (QueryToken) null;
      QueryToken parent2 = this.lexer.PeekNextToken().Kind != ExpressionTokenKind.Slash ? this.ParsePrimaryStart() : this.ParseSegment(parent1);
      while (this.lexer.CurrentToken.Kind == ExpressionTokenKind.Slash)
      {
        this.lexer.NextToken();
        parent2 = !this.TokenIdentifierIs("any") ? (!this.TokenIdentifierIs("all") ? (this.lexer.PeekNextToken().Kind != ExpressionTokenKind.Slash ? new IdentifierTokenizer(this.parameters, (IFunctionCallParser) new FunctionCallParser(this.lexer, this, this.IsInAggregateExpression)).ParseIdentifier(parent2) : this.ParseSegment(parent2)) : this.ParseAll(parent2)) : this.ParseAny(parent2);
      }
      this.RecurseLeave();
      return parent2;
    }

    private QueryToken ParsePrimaryStart()
    {
      switch (this.lexer.CurrentToken.Kind)
      {
        case ExpressionTokenKind.Identifier:
          return new IdentifierTokenizer(this.parameters, (IFunctionCallParser) new FunctionCallParser(this.lexer, this, this.IsInAggregateExpression)).ParseIdentifier(this.aggregateExpressionParents.Count > 0 ? this.aggregateExpressionParents.Peek() : (QueryToken) null);
        case ExpressionTokenKind.OpenParen:
          return this.ParseParenExpression();
        case ExpressionTokenKind.Star:
          return new IdentifierTokenizer(this.parameters, (IFunctionCallParser) new FunctionCallParser(this.lexer, this, this.IsInAggregateExpression)).ParseStarMemberAccess((QueryToken) null);
        case ExpressionTokenKind.ParameterAlias:
          return (QueryToken) UriQueryExpressionParser.ParseParameterAlias(this.lexer);
        default:
          return (QueryToken) UriQueryExpressionParser.TryParseLiteral(this.lexer) ?? throw UriQueryExpressionParser.ParseError(Microsoft.OData.Strings.UriQueryExpressionParser_ExpressionExpected((object) this.lexer.CurrentToken.Position, (object) this.lexer.ExpressionText));
      }
    }

    private QueryToken ParseParenExpression()
    {
      if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.OpenParen)
        throw UriQueryExpressionParser.ParseError(Microsoft.OData.Strings.UriQueryExpressionParser_OpenParenExpected((object) this.lexer.CurrentToken.Position, (object) this.lexer.ExpressionText));
      this.lexer.NextToken();
      QueryToken expression = this.ParseExpression();
      if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.CloseParen)
        throw UriQueryExpressionParser.ParseError(Microsoft.OData.Strings.UriQueryExpressionParser_CloseParenOrOperatorExpected((object) this.lexer.CurrentToken.Position, (object) this.lexer.ExpressionText));
      this.lexer.NextToken();
      return expression;
    }

    private QueryToken ParseAny(QueryToken parent) => this.ParseAnyAll(parent, true);

    private QueryToken ParseAll(QueryToken parent) => this.ParseAnyAll(parent, false);

    private QueryToken ParseAnyAll(QueryToken parent, bool isAny)
    {
      this.lexer.NextToken();
      if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.OpenParen)
        throw UriQueryExpressionParser.ParseError(Microsoft.OData.Strings.UriQueryExpressionParser_OpenParenExpected((object) this.lexer.CurrentToken.Position, (object) this.lexer.ExpressionText));
      this.lexer.NextToken();
      if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.CloseParen)
      {
        this.lexer.NextToken();
        return isAny ? (QueryToken) new AnyToken((QueryToken) new LiteralToken((object) true, "True"), (string) null, parent) : (QueryToken) new AllToken((QueryToken) new LiteralToken((object) true, "True"), (string) null, parent);
      }
      string identifier = this.lexer.CurrentToken.GetIdentifier();
      if (!this.parameters.Add(identifier))
        throw UriQueryExpressionParser.ParseError(Microsoft.OData.Strings.UriQueryExpressionParser_RangeVariableAlreadyDeclared((object) identifier));
      this.lexer.NextToken();
      this.lexer.ValidateToken(ExpressionTokenKind.Colon);
      this.lexer.NextToken();
      QueryToken expression = this.ParseExpression();
      if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.CloseParen)
        throw UriQueryExpressionParser.ParseError(Microsoft.OData.Strings.UriQueryExpressionParser_CloseParenOrCommaExpected((object) this.lexer.CurrentToken.Position, (object) this.lexer.ExpressionText));
      this.parameters.Remove(identifier);
      this.lexer.NextToken();
      return isAny ? (QueryToken) new AnyToken(expression, identifier, parent) : (QueryToken) new AllToken(expression, identifier, parent);
    }

    private QueryToken ParseSegment(QueryToken parent)
    {
      string identifier = this.lexer.CurrentToken.GetIdentifier();
      this.lexer.NextToken();
      return this.parameters.Contains(identifier) && parent == null ? (QueryToken) new RangeVariableToken(identifier) : (QueryToken) new InnerPathToken(identifier, parent, (IEnumerable<NamedValue>) null);
    }

    private AggregationMethodDefinition ParseAggregateWith()
    {
      if (!this.TokenIdentifierIs("with"))
        throw UriQueryExpressionParser.ParseError(Microsoft.OData.Strings.UriQueryExpressionParser_WithExpected((object) this.lexer.CurrentToken.Position, (object) this.lexer.ExpressionText));
      this.lexer.NextToken();
      int position = this.lexer.CurrentToken.Position;
      string str = this.lexer.ReadDottedIdentifier(false);
      AggregationMethodDefinition aggregateWith;
      switch (str)
      {
        case "average":
          aggregateWith = AggregationMethodDefinition.Average;
          break;
        case "countdistinct":
          aggregateWith = AggregationMethodDefinition.CountDistinct;
          break;
        case "max":
          aggregateWith = AggregationMethodDefinition.Max;
          break;
        case "min":
          aggregateWith = AggregationMethodDefinition.Min;
          break;
        case "sum":
          aggregateWith = AggregationMethodDefinition.Sum;
          break;
        default:
          aggregateWith = str.Contains(".") ? AggregationMethodDefinition.Custom(str) : throw UriQueryExpressionParser.ParseError(Microsoft.OData.Strings.UriQueryExpressionParser_UnrecognizedWithMethod((object) str, (object) position, (object) this.lexer.ExpressionText));
          break;
      }
      return aggregateWith;
    }

    private StringLiteralToken ParseAggregateAs()
    {
      if (!this.TokenIdentifierIs("as"))
        throw UriQueryExpressionParser.ParseError(Microsoft.OData.Strings.UriQueryExpressionParser_AsExpected((object) this.lexer.CurrentToken.Position, (object) this.lexer.ExpressionText));
      this.lexer.NextToken();
      StringLiteralToken aggregateAs = new StringLiteralToken(this.lexer.CurrentToken.Text);
      this.lexer.NextToken();
      return aggregateAs;
    }

    private bool TokenIdentifierIs(string id) => this.lexer.CurrentToken.IdentifierIs(id, this.enableCaseInsensitiveBuiltinIdentifier);

    private void RecurseEnter()
    {
      ++this.recursionDepth;
      if (this.recursionDepth > this.maxDepth)
        throw new ODataException(Microsoft.OData.Strings.UriQueryExpressionParser_TooDeep);
    }

    private void RecurseLeave() => --this.recursionDepth;

    internal delegate QueryToken Parser();
  }
}
