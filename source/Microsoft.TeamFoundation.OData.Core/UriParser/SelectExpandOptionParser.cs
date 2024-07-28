// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.SelectExpandOptionParser
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.OData.UriParser
{
  internal sealed class SelectExpandOptionParser
  {
    private readonly ODataUriResolver resolver;
    private readonly IEdmStructuredType parentStructuredType;
    private readonly int maxRecursionDepth;
    private readonly bool enableNoDollarQueryOptions;
    private readonly bool enableCaseInsensitiveBuiltinIdentifier;
    private ExpressionLexer lexer;

    internal SelectExpandOptionParser(
      int maxRecursionDepth,
      bool enableCaseInsensitiveBuiltinIdentifier = false,
      bool enableNoDollarQueryOptions = false)
    {
      this.maxRecursionDepth = maxRecursionDepth;
      this.enableCaseInsensitiveBuiltinIdentifier = enableCaseInsensitiveBuiltinIdentifier;
      this.enableNoDollarQueryOptions = enableNoDollarQueryOptions;
    }

    internal SelectExpandOptionParser(
      ODataUriResolver resolver,
      IEdmStructuredType parentStructuredType,
      int maxRecursionDepth,
      bool enableCaseInsensitiveBuiltinIdentifier = false,
      bool enableNoDollarQueryOptions = false)
      : this(maxRecursionDepth, enableCaseInsensitiveBuiltinIdentifier, enableNoDollarQueryOptions)
    {
      this.resolver = resolver;
      this.parentStructuredType = parentStructuredType;
    }

    internal int MaxFilterDepth { get; set; }

    internal int MaxOrderByDepth { get; set; }

    internal int MaxSearchDepth { get; set; }

    internal SelectTermToken BuildSelectTermToken(PathSegmentToken pathToken, string optionsText)
    {
      this.lexer = new ExpressionLexer(optionsText ?? "", true, true);
      QueryToken filterOption = (QueryToken) null;
      IEnumerable<OrderByToken> orderByOptions = (IEnumerable<OrderByToken>) null;
      long? topOption = new long?();
      long? skipOption = new long?();
      bool? countQueryOption = new bool?();
      QueryToken searchOption = (QueryToken) null;
      SelectToken selectOption = (SelectToken) null;
      ComputeToken computeOption = (ComputeToken) null;
      if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.OpenParen)
      {
        this.lexer.NextToken();
        if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.CloseParen)
          throw new ODataException(Microsoft.OData.Strings.UriParser_MissingSelectOption((object) pathToken.Identifier));
        while (this.lexer.CurrentToken.Kind != ExpressionTokenKind.CloseParen)
        {
          string str = this.enableCaseInsensitiveBuiltinIdentifier ? this.lexer.CurrentToken.Text.ToLowerInvariant() : this.lexer.CurrentToken.Text;
          if (this.enableNoDollarQueryOptions && !str.StartsWith("$", StringComparison.Ordinal))
            str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", new object[2]
            {
              (object) "$",
              (object) str
            });
          switch (str)
          {
            case "$compute":
              computeOption = this.ParseInnerCompute();
              continue;
            case "$count":
              countQueryOption = this.ParseInnerCount();
              continue;
            case "$filter":
              filterOption = this.ParseInnerFilter();
              continue;
            case "$orderby":
              orderByOptions = this.ParseInnerOrderBy();
              continue;
            case "$search":
              searchOption = this.ParseInnerSearch();
              continue;
            case "$select":
              selectOption = this.ParseInnerSelect(pathToken);
              continue;
            case "$skip":
              skipOption = this.ParseInnerSkip();
              continue;
            case "$top":
              topOption = this.ParseInnerTop();
              continue;
            default:
              throw new ODataException(Microsoft.OData.Strings.UriSelectParser_TermIsNotValid((object) this.lexer.ExpressionText));
          }
        }
        this.lexer.NextToken();
      }
      if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.End)
        throw new ODataException(Microsoft.OData.Strings.UriSelectParser_TermIsNotValid((object) this.lexer.ExpressionText));
      return new SelectTermToken(pathToken, filterOption, orderByOptions, topOption, skipOption, countQueryOption, searchOption, selectOption, computeOption);
    }

    internal List<ExpandTermToken> BuildExpandTermToken(
      PathSegmentToken pathToken,
      string optionsText)
    {
      this.lexer = new ExpressionLexer(optionsText ?? "", true, true);
      if (pathToken.Identifier == "*" || pathToken.Identifier == "$ref" && pathToken.NextToken.Identifier == "*")
        return this.BuildStarExpandTermToken(pathToken);
      QueryToken filterOption = (QueryToken) null;
      IEnumerable<OrderByToken> orderByOptions = (IEnumerable<OrderByToken>) null;
      long? topOption = new long?();
      long? skipOption = new long?();
      bool? countQueryOption = new bool?();
      long? levelsOption = new long?();
      QueryToken searchOption = (QueryToken) null;
      SelectToken selectOption = (SelectToken) null;
      ExpandToken expandOption = (ExpandToken) null;
      ComputeToken computeOption = (ComputeToken) null;
      IEnumerable<QueryToken> applyOptions = (IEnumerable<QueryToken>) null;
      if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.OpenParen)
      {
        this.lexer.NextToken();
        if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.CloseParen)
          throw new ODataException(Microsoft.OData.Strings.UriParser_MissingExpandOption((object) pathToken.Identifier));
        while (this.lexer.CurrentToken.Kind != ExpressionTokenKind.CloseParen)
        {
          string str = this.enableCaseInsensitiveBuiltinIdentifier ? this.lexer.CurrentToken.Text.ToLowerInvariant() : this.lexer.CurrentToken.Text;
          if (this.enableNoDollarQueryOptions && !str.StartsWith("$", StringComparison.Ordinal))
            str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", new object[2]
            {
              (object) "$",
              (object) str
            });
          switch (str)
          {
            case "$apply":
              applyOptions = this.ParseInnerApply();
              continue;
            case "$compute":
              computeOption = this.ParseInnerCompute();
              continue;
            case "$count":
              countQueryOption = this.ParseInnerCount();
              continue;
            case "$expand":
              expandOption = this.ParseInnerExpand(pathToken);
              continue;
            case "$filter":
              filterOption = this.ParseInnerFilter();
              continue;
            case "$levels":
              levelsOption = this.ParseInnerLevel();
              continue;
            case "$orderby":
              orderByOptions = this.ParseInnerOrderBy();
              continue;
            case "$search":
              searchOption = this.ParseInnerSearch();
              continue;
            case "$select":
              selectOption = this.ParseInnerSelect(pathToken);
              continue;
            case "$skip":
              skipOption = this.ParseInnerSkip();
              continue;
            case "$top":
              topOption = this.ParseInnerTop();
              continue;
            default:
              throw new ODataException(Microsoft.OData.Strings.UriSelectParser_TermIsNotValid((object) this.lexer.ExpressionText));
          }
        }
        this.lexer.NextToken();
      }
      if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.End)
        throw new ODataException(Microsoft.OData.Strings.UriSelectParser_TermIsNotValid((object) this.lexer.ExpressionText));
      return new List<ExpandTermToken>()
      {
        new ExpandTermToken(pathToken, filterOption, orderByOptions, topOption, skipOption, countQueryOption, levelsOption, searchOption, selectOption, expandOption, computeOption, applyOptions)
      };
    }

    private List<ExpandTermToken> BuildStarExpandTermToken(PathSegmentToken pathToken)
    {
      if (this.parentStructuredType == null)
        throw new ODataException(Microsoft.OData.Strings.UriExpandParser_ParentStructuredTypeIsNull((object) this.lexer.ExpressionText));
      List<ExpandTermToken> expandTermTokenList = new List<ExpandTermToken>();
      long? levelsOption = new long?();
      bool flag = pathToken.Identifier == "$ref";
      if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.OpenParen)
      {
        this.lexer.NextToken();
        if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.CloseParen)
          throw new ODataException(Microsoft.OData.Strings.UriParser_MissingExpandOption((object) pathToken.Identifier));
        while (this.lexer.CurrentToken.Kind != ExpressionTokenKind.CloseParen)
        {
          if (!((this.enableCaseInsensitiveBuiltinIdentifier ? this.lexer.CurrentToken.Text.ToLowerInvariant() : this.lexer.CurrentToken.Text) == "$levels"))
            throw new ODataException(Microsoft.OData.Strings.UriExpandParser_TermIsNotValidForStar((object) this.lexer.ExpressionText));
          if (flag)
            throw new ODataException(Microsoft.OData.Strings.UriExpandParser_TermIsNotValidForStarRef((object) this.lexer.ExpressionText));
          levelsOption = this.ParseInnerLevel();
        }
        this.lexer.NextToken();
      }
      if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.End)
        throw new ODataException(Microsoft.OData.Strings.UriSelectParser_TermIsNotValid((object) this.lexer.ExpressionText));
      foreach (IEdmNavigationProperty navigationProperty in this.parentStructuredType.NavigationProperties())
      {
        ExpandTermToken expandTermToken = new ExpandTermToken(!pathToken.Identifier.Equals("$ref") ? (PathSegmentToken) new NonSystemToken(navigationProperty.Name, (IEnumerable<NamedValue>) null, pathToken.NextToken) : (PathSegmentToken) new NonSystemToken("$ref", (IEnumerable<NamedValue>) null, (PathSegmentToken) new NonSystemToken(navigationProperty.Name, (IEnumerable<NamedValue>) null, pathToken.NextToken.NextToken)), (QueryToken) null, (IEnumerable<OrderByToken>) null, new long?(), new long?(), new bool?(), levelsOption, (QueryToken) null, (SelectToken) null, (ExpandToken) null, (ComputeToken) null, (IEnumerable<QueryToken>) null);
        expandTermTokenList.Add(expandTermToken);
      }
      return expandTermTokenList;
    }

    private QueryToken ParseInnerFilter()
    {
      this.lexer.NextToken();
      return new UriQueryExpressionParser(this.MaxFilterDepth, this.enableCaseInsensitiveBuiltinIdentifier).ParseFilter(this.ReadQueryOption());
    }

    private IEnumerable<OrderByToken> ParseInnerOrderBy()
    {
      this.lexer.NextToken();
      return new UriQueryExpressionParser(this.MaxOrderByDepth, this.enableCaseInsensitiveBuiltinIdentifier).ParseOrderBy(this.ReadQueryOption());
    }

    private long? ParseInnerTop()
    {
      this.lexer.NextToken();
      string str = this.ReadQueryOption();
      long result;
      return long.TryParse(str, out result) && result >= 0L ? new long?(result) : throw new ODataException(Microsoft.OData.Strings.UriSelectParser_InvalidTopOption((object) str));
    }

    private long? ParseInnerSkip()
    {
      this.lexer.NextToken();
      string str = this.ReadQueryOption();
      long result;
      return long.TryParse(str, out result) && result >= 0L ? new long?(result) : throw new ODataException(Microsoft.OData.Strings.UriSelectParser_InvalidSkipOption((object) str));
    }

    private bool? ParseInnerCount()
    {
      this.lexer.NextToken();
      string p0 = this.ReadQueryOption();
      switch (p0)
      {
        case "true":
          return new bool?(true);
        case "false":
          return new bool?(false);
        default:
          throw new ODataException(Microsoft.OData.Strings.UriSelectParser_InvalidCountOption((object) p0));
      }
    }

    private QueryToken ParseInnerSearch()
    {
      this.lexer.NextToken();
      return new SearchParser(this.MaxSearchDepth).ParseSearch(this.ReadQueryOption());
    }

    private SelectToken ParseInnerSelect(PathSegmentToken pathToken)
    {
      this.lexer.NextToken();
      string clauseToParse = this.ReadQueryOption();
      IEdmStructuredType parentStructuredType = (IEdmStructuredType) null;
      if (this.resolver != null && this.parentStructuredType != null)
      {
        IEdmProperty edmProperty = this.resolver.ResolveProperty(this.parentStructuredType, pathToken.Identifier);
        if (edmProperty != null)
          parentStructuredType = edmProperty.Type.ToStructuredType();
      }
      return new SelectExpandParser(this.resolver, clauseToParse, parentStructuredType, this.maxRecursionDepth - 1, this.enableCaseInsensitiveBuiltinIdentifier, this.enableNoDollarQueryOptions).ParseSelect();
    }

    private ExpandToken ParseInnerExpand(PathSegmentToken pathToken)
    {
      this.lexer.NextToken();
      string clauseToParse = this.ReadQueryOption();
      IEdmStructuredType parentStructuredType = (IEdmStructuredType) null;
      if (this.resolver != null && this.parentStructuredType != null)
      {
        IEdmProperty edmProperty = this.resolver.ResolveProperty(this.parentStructuredType, pathToken.Identifier);
        if (edmProperty != null)
          parentStructuredType = edmProperty.Type.ToStructuredType();
      }
      return new SelectExpandParser(this.resolver, clauseToParse, parentStructuredType, this.maxRecursionDepth - 1, this.enableCaseInsensitiveBuiltinIdentifier, this.enableNoDollarQueryOptions).ParseExpand();
    }

    private long? ParseInnerLevel()
    {
      long? innerLevel = new long?();
      this.lexer.NextToken();
      string str = this.ReadQueryOption();
      if (string.Equals("max", str, this.enableCaseInsensitiveBuiltinIdentifier ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
      {
        innerLevel = new long?(long.MinValue);
      }
      else
      {
        long result;
        if (!long.TryParse(str, NumberStyles.None, (IFormatProvider) CultureInfo.InvariantCulture, out result) || result < 0L)
          throw new ODataException(Microsoft.OData.Strings.UriSelectParser_InvalidLevelsOption((object) str));
        innerLevel = new long?(result);
      }
      return innerLevel;
    }

    private ComputeToken ParseInnerCompute()
    {
      this.lexer.NextToken();
      return new UriQueryExpressionParser(this.MaxOrderByDepth, this.enableCaseInsensitiveBuiltinIdentifier).ParseCompute(this.ReadQueryOption());
    }

    private IEnumerable<QueryToken> ParseInnerApply()
    {
      this.lexer.NextToken();
      return new UriQueryExpressionParser(this.MaxOrderByDepth, this.enableCaseInsensitiveBuiltinIdentifier).ParseApply(this.ReadQueryOption());
    }

    private string ReadQueryOption()
    {
      if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.Equal)
        throw new ODataException(Microsoft.OData.Strings.UriSelectParser_TermIsNotValid((object) this.lexer.ExpressionText));
      string str = this.lexer.AdvanceThroughExpandOption();
      if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.SemiColon)
      {
        this.lexer.NextToken();
        return str;
      }
      this.lexer.ValidateToken(ExpressionTokenKind.CloseParen);
      return str;
    }
  }
}
