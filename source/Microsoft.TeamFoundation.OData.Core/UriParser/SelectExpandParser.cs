// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.SelectExpandParser
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;

namespace Microsoft.OData.UriParser
{
  internal sealed class SelectExpandParser
  {
    private readonly ODataUriResolver resolver;
    private readonly IEdmStructuredType parentStructuredType;
    private readonly int maxRecursiveDepth;
    private readonly bool enableNoDollarQueryOptions;
    private readonly bool enableCaseInsensitiveBuiltinIdentifier;
    private SelectExpandOptionParser selectExpandOptionParser;
    private ExpressionLexer lexer;
    private bool isSelect;

    public SelectExpandParser(
      string clauseToParse,
      int maxRecursiveDepth,
      bool enableCaseInsensitiveBuiltinIdentifier = false,
      bool enableNoDollarQueryOptions = false)
    {
      this.maxRecursiveDepth = maxRecursiveDepth;
      this.MaxPathDepth = maxRecursiveDepth;
      this.MaxFilterDepth = maxRecursiveDepth;
      this.MaxOrderByDepth = maxRecursiveDepth;
      this.MaxSearchDepth = maxRecursiveDepth;
      this.lexer = clauseToParse != null ? new ExpressionLexer(clauseToParse, false, false) : (ExpressionLexer) null;
      this.enableCaseInsensitiveBuiltinIdentifier = enableCaseInsensitiveBuiltinIdentifier;
      this.enableNoDollarQueryOptions = enableNoDollarQueryOptions;
    }

    public SelectExpandParser(
      ODataUriResolver resolver,
      string clauseToParse,
      IEdmStructuredType parentStructuredType,
      int maxRecursiveDepth,
      bool enableCaseInsensitiveBuiltinIdentifier = false,
      bool enableNoDollarQueryOptions = false)
      : this(clauseToParse, maxRecursiveDepth, enableCaseInsensitiveBuiltinIdentifier, enableNoDollarQueryOptions)
    {
      this.resolver = resolver;
      this.parentStructuredType = parentStructuredType;
    }

    internal SelectExpandOptionParser SelectExpandOptionParser
    {
      get
      {
        if (this.selectExpandOptionParser == null)
          this.selectExpandOptionParser = new SelectExpandOptionParser(this.resolver, this.parentStructuredType, this.maxRecursiveDepth, this.enableCaseInsensitiveBuiltinIdentifier, this.enableNoDollarQueryOptions)
          {
            MaxFilterDepth = this.MaxFilterDepth,
            MaxOrderByDepth = this.MaxOrderByDepth,
            MaxSearchDepth = this.MaxSearchDepth
          };
        return this.selectExpandOptionParser;
      }
    }

    internal int MaxPathDepth { get; set; }

    internal int MaxFilterDepth { get; set; }

    internal int MaxOrderByDepth { get; set; }

    internal int MaxSearchDepth { get; set; }

    public SelectToken ParseSelect()
    {
      this.isSelect = true;
      return this.ParseCommaSeparatedSelectList((Func<IEnumerable<SelectTermToken>, SelectToken>) (termTokens => new SelectToken(termTokens)), new Func<SelectTermToken>(this.ParseSingleSelectTerm));
    }

    public ExpandToken ParseExpand()
    {
      this.isSelect = false;
      return this.ParseCommaSeparatedExpandList((Func<IEnumerable<ExpandTermToken>, ExpandToken>) (termTokens => new ExpandToken(termTokens)), new Func<List<ExpandTermToken>>(this.ParseSingleExpandTerm));
    }

    private SelectTermToken ParseSingleSelectTerm()
    {
      this.isSelect = true;
      PathSegmentToken term = new SelectExpandTermParser(this.lexer, this.MaxPathDepth, this.isSelect).ParseTerm();
      string optionsText = (string) null;
      if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.OpenParen)
      {
        optionsText = this.lexer.AdvanceThroughBalancedParentheticalExpression();
        this.lexer.NextToken();
      }
      return this.SelectExpandOptionParser.BuildSelectTermToken(term, optionsText);
    }

    private List<ExpandTermToken> ParseSingleExpandTerm()
    {
      this.isSelect = false;
      PathSegmentToken term = new SelectExpandTermParser(this.lexer, this.MaxPathDepth, this.isSelect).ParseTerm(true);
      string optionsText = (string) null;
      if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.OpenParen)
      {
        optionsText = this.lexer.AdvanceThroughBalancedParentheticalExpression();
        this.lexer.NextToken();
      }
      return this.SelectExpandOptionParser.BuildExpandTermToken(term, optionsText);
    }

    private ExpandToken ParseCommaSeparatedExpandList(
      Func<IEnumerable<ExpandTermToken>, ExpandToken> ctor,
      Func<List<ExpandTermToken>> termParsingFunc)
    {
      List<ExpandTermToken> expandTermTokenList1 = new List<ExpandTermToken>();
      List<ExpandTermToken> expandTermTokenList2 = new List<ExpandTermToken>();
      if (this.lexer == null)
        return ctor((IEnumerable<ExpandTermToken>) expandTermTokenList1);
      this.lexer.NextToken();
      if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.End)
        return ctor((IEnumerable<ExpandTermToken>) expandTermTokenList1);
      if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.Star)
        expandTermTokenList2 = termParsingFunc();
      else
        expandTermTokenList1.AddRange((IEnumerable<ExpandTermToken>) termParsingFunc());
      while (this.lexer.CurrentToken.Kind == ExpressionTokenKind.Comma)
      {
        this.lexer.NextToken();
        if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.End && this.lexer.CurrentToken.Kind != ExpressionTokenKind.Star)
          expandTermTokenList1.AddRange((IEnumerable<ExpandTermToken>) termParsingFunc());
        else if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.Star)
        {
          if (expandTermTokenList2.Count > 0)
            throw new ODataException(Microsoft.OData.Strings.UriExpandParser_TermWithMultipleStarNotAllowed((object) this.lexer.ExpressionText));
          expandTermTokenList2 = termParsingFunc();
        }
        else
          break;
      }
      if (expandTermTokenList2.Count > 0)
      {
        List<string> stringList = new List<string>();
        foreach (ExpandTermToken expandTermToken in expandTermTokenList1)
        {
          PathSegmentToken toNavigationProp = expandTermToken.PathToNavigationProp;
          if (toNavigationProp.Identifier != "$ref")
            stringList.Add(toNavigationProp.Identifier);
          else
            stringList.Add(toNavigationProp.NextToken.Identifier);
        }
        foreach (ExpandTermToken expandTermToken in expandTermTokenList2)
        {
          PathSegmentToken toNavigationProp = expandTermToken.PathToNavigationProp;
          if (toNavigationProp.Identifier != "$ref" && !stringList.Contains(toNavigationProp.Identifier))
            expandTermTokenList1.Add(expandTermToken);
          else if (toNavigationProp.Identifier == "$ref" && !stringList.Contains(toNavigationProp.NextToken.Identifier))
            expandTermTokenList1.Add(expandTermToken);
        }
      }
      if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.End)
        throw new ODataException(Microsoft.OData.Strings.UriSelectParser_TermIsNotValid((object) this.lexer.ExpressionText));
      return ctor((IEnumerable<ExpandTermToken>) expandTermTokenList1);
    }

    private SelectToken ParseCommaSeparatedSelectList(
      Func<IEnumerable<SelectTermToken>, SelectToken> ctor,
      Func<SelectTermToken> termParsingFunc)
    {
      List<SelectTermToken> selectTermTokenList = new List<SelectTermToken>();
      if (this.lexer == null)
        return ctor((IEnumerable<SelectTermToken>) selectTermTokenList);
      this.lexer.NextToken();
      if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.End)
        return ctor((IEnumerable<SelectTermToken>) selectTermTokenList);
      selectTermTokenList.Add(termParsingFunc());
      while (this.lexer.CurrentToken.Kind == ExpressionTokenKind.Comma)
      {
        this.lexer.NextToken();
        if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.End)
          selectTermTokenList.Add(termParsingFunc());
        else
          break;
      }
      if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.End)
        throw new ODataException(Microsoft.OData.Strings.UriSelectParser_TermIsNotValid((object) this.lexer.ExpressionText));
      return ctor((IEnumerable<SelectTermToken>) selectTermTokenList);
    }
  }
}
