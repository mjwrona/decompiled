// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Search3.OData.FilterRecognizer
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.Data.Edm;
using Microsoft.Data.Edm.Library;
using Microsoft.Data.OData.Query;
using Microsoft.Data.OData.Query.SemanticAst;
using Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation;
using Microsoft.VisualStudio.Services.NuGet.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Search3.OData
{
  public class FilterRecognizer : IFilterRecognizer
  {
    private readonly V2FeedPackageModelInfo modelInfo;

    public FilterRecognizer(V2FeedPackageModelInfo modelInfo) => this.modelInfo = modelInfo;

    public RecognizeResult<Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation.Filter> RecognizeFilter(
      string filterString)
    {
      FilterClause filter;
      try
      {
        filter = this.modelInfo.ODataUriParser.ParseFilter(filterString, (IEdmType) this.modelInfo.V2FeedPackageType, (IEdmEntitySet) null);
      }
      catch (Exception ex)
      {
        return new RecognizeResult<Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation.Filter>(ex);
      }
      return this.RecognizeFilter(filter);
    }

    public RecognizeResult<Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation.Filter> RecognizeFilter(
      FilterClause filterClause)
    {
      return this.RecognizeFilter((QueryNode) filterClause.Expression);
    }

    private RecognizeResult<Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation.Filter> RecognizeFilter(
      QueryNode node)
    {
      node = node.SkipConvertNodes();
      RecognizeResult<Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation.Filter> recognizeResult1 = this.RecognizePackageFilter(node);
      if (recognizeResult1.Succeeded)
        return recognizeResult1;
      RecognizeResult<Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation.Filter> recognizeResult2 = this.RecognizePackageVersionFilter(node);
      if (recognizeResult2.Succeeded)
        return recognizeResult2;
      RecognizeResult<Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation.Filter> recognizeResult3 = this.RecognizeAllPackagesFilter(node);
      return recognizeResult3.Succeeded ? recognizeResult3 : new RecognizeResult<Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation.Filter>((Exception) new UnsupportedODataFilterException(Resources.Error_ODataFilterNotSupported()));
    }

    private RecognizeResult<Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation.Filter> RecognizePackageVersionFilter(
      QueryNode node)
    {
      node = node.SkipConvertNodes();
      RecognizeResult<PackageSelectorList> recognizeResult = this.RecognizeIdVersionSelectorList(node);
      return recognizeResult.Succeeded ? new RecognizeResult<Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation.Filter>(true, new Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation.Filter(recognizeResult.Result.Selectors, ImmutableList<VersionCategorySelector>.Empty)) : new RecognizeResult<Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation.Filter>(false, (Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation.Filter) null);
    }

    private RecognizeResult<Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation.Filter> RecognizePackageFilter(
      QueryNode node)
    {
      node = node.SkipConvertNodes();
      RecognizeResult<PackageSelectorList> recognizeResult1 = this.RecognizePackageNameSelectorList(node);
      if (recognizeResult1.Succeeded)
        return new RecognizeResult<Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation.Filter>(true, new Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation.Filter(recognizeResult1.Result.Selectors, ImmutableList<VersionCategorySelector>.Empty));
      if (node is BinaryOperatorNode binaryOperatorNode && binaryOperatorNode.OperatorKind == BinaryOperatorKind.And)
      {
        RecognizeResult<PackageSelectorList> recognizeResult2 = this.RecognizePackageNameSelectorList((QueryNode) binaryOperatorNode.Left);
        if (recognizeResult2.Succeeded)
        {
          RecognizeResult<VersionCategorySelectorList> recognizeResult3 = this.RecognizeVersionCategorySelectorList((QueryNode) binaryOperatorNode.Right);
          if (recognizeResult3.Succeeded)
            return new RecognizeResult<Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation.Filter>(true, new Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation.Filter(recognizeResult2.Result.Selectors, recognizeResult3.Result.Selectors));
        }
        RecognizeResult<VersionCategorySelectorList> recognizeResult4 = this.RecognizeVersionCategorySelectorList((QueryNode) binaryOperatorNode.Left);
        if (recognizeResult4.Succeeded)
        {
          RecognizeResult<PackageSelectorList> recognizeResult5 = this.RecognizePackageNameSelectorList((QueryNode) binaryOperatorNode.Right);
          if (recognizeResult5.Succeeded)
            return new RecognizeResult<Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation.Filter>(true, new Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation.Filter(recognizeResult5.Result.Selectors, recognizeResult4.Result.Selectors));
        }
      }
      return new RecognizeResult<Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation.Filter>(false, (Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation.Filter) null);
    }

    private RecognizeResult<Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation.Filter> RecognizeAllPackagesFilter(
      QueryNode node)
    {
      node = node.SkipConvertNodes();
      RecognizeResult<VersionCategorySelectorList> recognizeResult = this.RecognizeVersionCategorySelectorList(node);
      return recognizeResult.Succeeded ? new RecognizeResult<Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation.Filter>(true, new Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation.Filter(ImmutableList<PackageSelector>.Empty, recognizeResult.Result.Selectors)) : new RecognizeResult<Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation.Filter>(false, (Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation.Filter) null);
    }

    private RecognizeResult<PackageSelectorList> RecognizeIdVersionSelectorList(QueryNode node)
    {
      node = node.SkipConvertNodes();
      RecognizeResult<PackageSelector> recognizeResult1 = this.RecognizeSingleVersionSelector(node);
      if (recognizeResult1.Succeeded)
        return new RecognizeResult<PackageSelectorList>(true, new PackageSelectorList(ImmutableList.Create<PackageSelector>(recognizeResult1.Result)));
      if (node is BinaryOperatorNode binaryOperatorNode && binaryOperatorNode.OperatorKind == BinaryOperatorKind.Or)
      {
        RecognizeResult<PackageSelectorList> recognizeResult2 = this.RecognizeIdVersionSelectorList((QueryNode) binaryOperatorNode.Left);
        if (recognizeResult2.Succeeded)
        {
          RecognizeResult<PackageSelectorList> recognizeResult3 = this.RecognizeIdVersionSelectorList((QueryNode) binaryOperatorNode.Right);
          if (recognizeResult3.Succeeded)
            return new RecognizeResult<PackageSelectorList>(true, new PackageSelectorList(recognizeResult2.Result.Selectors.AddRange((IEnumerable<PackageSelector>) recognizeResult3.Result.Selectors)));
        }
      }
      return new RecognizeResult<PackageSelectorList>(false, (PackageSelectorList) null);
    }

    private RecognizeResult<PackageSelector> RecognizeSingleVersionSelector(QueryNode node)
    {
      node = node.SkipConvertNodes();
      if (node is BinaryOperatorNode binaryOperatorNode && binaryOperatorNode.OperatorKind == BinaryOperatorKind.And)
      {
        RecognizeResult<PackageNameSelector> recognizeResult1 = this.RecognizeExactIdSelector((QueryNode) binaryOperatorNode.Left);
        if (recognizeResult1.Succeeded)
        {
          RecognizeResult<PackageVersionSelector> recognizeResult2 = this.RecognizeVersionSelector((QueryNode) binaryOperatorNode.Right);
          if (recognizeResult2.Succeeded)
            return new RecognizeResult<PackageSelector>(true, new PackageSelector(recognizeResult1.Result, recognizeResult2.Result));
        }
      }
      return new RecognizeResult<PackageSelector>(false, (PackageSelector) null);
    }

    private RecognizeResult<PackageVersionSelector> RecognizeVersionSelector(QueryNode node)
    {
      node = node.SkipConvertNodes();
      if (node is BinaryOperatorNode binaryOperatorNode && binaryOperatorNode.OperatorKind == BinaryOperatorKind.Equal)
      {
        RecognizeResult<IEdmProperty> recognizeResult1 = this.RecognizePropertyAccess((QueryNode) binaryOperatorNode.Left);
        if (recognizeResult1.Succeeded)
        {
          RecognizeResult<string> recognizeResult2 = this.RecognizeStringConstant((QueryNode) binaryOperatorNode.Right);
          if (recognizeResult2.Succeeded)
          {
            if (recognizeResult1.Result == this.modelInfo.VersionProperty)
              return new RecognizeResult<PackageVersionSelector>(true, new PackageVersionSelector(recognizeResult2.Result, VersionMatchFields.DisplayVersion, VersionMatchType.Exact));
            if (recognizeResult1.Result == this.modelInfo.NormalizedVersionProperty)
              return new RecognizeResult<PackageVersionSelector>(true, new PackageVersionSelector(recognizeResult2.Result, VersionMatchFields.NormalizedVersion, VersionMatchType.Exact));
          }
        }
      }
      return new RecognizeResult<PackageVersionSelector>(false, (PackageVersionSelector) null);
    }

    private RecognizeResult<PackageNameSelector> RecognizeExactIdSelector(QueryNode node)
    {
      node = node.SkipConvertNodes();
      if (node is BinaryOperatorNode binaryOperatorNode && binaryOperatorNode.OperatorKind == BinaryOperatorKind.Equal)
      {
        RecognizeResult<LowerableProperty> recognizeResult1 = this.RecognizeLowerablePropertyAccess((QueryNode) binaryOperatorNode.Left);
        if (recognizeResult1.Succeeded && recognizeResult1.Result.Property == this.modelInfo.IdProperty)
        {
          RecognizeResult<string> recognizeResult2 = this.RecognizeStringConstant((QueryNode) binaryOperatorNode.Right);
          if (recognizeResult2.Succeeded)
            return new RecognizeResult<PackageNameSelector>(true, new PackageNameSelector(recognizeResult2.Result, NameMatchFields.Id, NameMatchType.Exact, recognizeResult1.Result.HasToLower));
        }
      }
      return new RecognizeResult<PackageNameSelector>(false, (PackageNameSelector) null);
    }

    private RecognizeResult<VersionCategorySelectorList> RecognizeVersionCategorySelectorList(
      QueryNode node)
    {
      node = node.SkipConvertNodes();
      RecognizeResult<VersionCategorySelector> recognizeResult1 = this.RecognizeLatestSelector(node);
      if (recognizeResult1.Succeeded)
        return new RecognizeResult<VersionCategorySelectorList>(true, new VersionCategorySelectorList(ImmutableList.Create<VersionCategorySelector>(recognizeResult1.Result)));
      if (node is BinaryOperatorNode binaryOperatorNode && binaryOperatorNode.OperatorKind == BinaryOperatorKind.Or)
      {
        RecognizeResult<VersionCategorySelectorList> recognizeResult2 = this.RecognizeVersionCategorySelectorList((QueryNode) binaryOperatorNode.Left);
        if (recognizeResult2.Succeeded)
        {
          RecognizeResult<VersionCategorySelectorList> recognizeResult3 = this.RecognizeVersionCategorySelectorList((QueryNode) binaryOperatorNode.Right);
          if (recognizeResult3.Succeeded)
            return new RecognizeResult<VersionCategorySelectorList>(true, new VersionCategorySelectorList(recognizeResult2.Result.Selectors.AddRange((IEnumerable<VersionCategorySelector>) recognizeResult3.Result.Selectors)));
        }
      }
      return new RecognizeResult<VersionCategorySelectorList>(false, (VersionCategorySelectorList) null);
    }

    private RecognizeResult<VersionCategorySelector> RecognizeLatestSelector(QueryNode node)
    {
      node = node.SkipConvertNodes();
      RecognizeResult<IEdmProperty> recognizeResult = this.RecognizePropertyAccess(node);
      if (recognizeResult.Succeeded)
      {
        if (recognizeResult.Result == this.modelInfo.IsLatestVersionProperty)
          return new RecognizeResult<VersionCategorySelector>(true, VersionCategorySelector.LatestVersion);
        if (recognizeResult.Result == this.modelInfo.IsAbsoluteLatestVersionProperty)
          return new RecognizeResult<VersionCategorySelector>(true, VersionCategorySelector.AbsoluteLatestVersion);
      }
      return new RecognizeResult<VersionCategorySelector>(false, VersionCategorySelector.AllVersions);
    }

    private RecognizeResult<PackageNameSelector> RecognizePackageNameSelector(QueryNode node)
    {
      node = node.SkipConvertNodes();
      RecognizeResult<PackageNameSelector> recognizeResult1 = this.RecognizeExactIdSelector(node);
      if (recognizeResult1.Succeeded)
        return recognizeResult1;
      RecognizeResult<PackageNameSelector> recognizeResult2 = this.RecognizeIdSubstringSelector(node);
      if (recognizeResult2.Succeeded)
        return recognizeResult2;
      RecognizeResult<PackageNameSelector> recognizeResult3 = this.RecognizeIdPrefixSelector(node);
      return recognizeResult3.Succeeded ? recognizeResult3 : new RecognizeResult<PackageNameSelector>(false, (PackageNameSelector) null);
    }

    private RecognizeResult<PackageNameSelector> RecognizeIdPrefixSelector(QueryNode node)
    {
      node = node.SkipConvertNodes();
      if (node is SingleValueFunctionCallNode functionCallNode)
      {
        List<QueryNode> list = functionCallNode.Arguments.ToList<QueryNode>();
        if (functionCallNode.Name == "startswith" && functionCallNode.Source == null && list.Count == 2)
        {
          RecognizeResult<LowerableProperty> recognizeResult1 = this.RecognizeLowerablePropertyAccess(list[0]);
          RecognizeResult<string> recognizeResult2 = this.RecognizeStringConstant(list[1]);
          if (recognizeResult2.Succeeded && recognizeResult1.Succeeded && recognizeResult1.Result.Property == this.modelInfo.IdProperty)
            return new RecognizeResult<PackageNameSelector>(true, new PackageNameSelector(recognizeResult2.Result, NameMatchFields.Id, NameMatchType.Prefix, recognizeResult1.Result.HasToLower));
        }
      }
      return new RecognizeResult<PackageNameSelector>(false, (PackageNameSelector) null);
    }

    private RecognizeResult<PackageNameSelector> RecognizeIdSubstringSelector(QueryNode node)
    {
      node = node.SkipConvertNodes();
      RecognizeResult<PackageNameSelector> recognizeResult = RecognizeSimple(node);
      return recognizeResult.Succeeded ? recognizeResult : RecognizeComplex(node);

      RecognizeResult<PackageNameSelector> RecognizeSimple(QueryNode node1)
      {
        node1 = node1.SkipConvertNodes();
        RecognizeResult<SubstringCall> recognizeResult = this.RecognizeSubstringCall(node1, this.modelInfo.IdProperty);
        if (!recognizeResult.Succeeded)
          return new RecognizeResult<PackageNameSelector>(false, (PackageNameSelector) null);
        string needle = recognizeResult.Result.Needle;
        return new RecognizeResult<PackageNameSelector>(true, string.IsNullOrWhiteSpace(needle) ? PackageNameSelector.AllPackageNames : new PackageNameSelector(needle, NameMatchFields.Id, NameMatchType.Substring, recognizeResult.Result.Haystack.HasToLower));
      }

      RecognizeResult<PackageNameSelector> RecognizeComplex(QueryNode node1)
      {
        node1 = node1.SkipConvertNodes();
        if (node1 is BinaryOperatorNode binaryOperatorNode1 && binaryOperatorNode1.OperatorKind == BinaryOperatorKind.Or)
        {
          RecognizeResult<SubstringCall> recognizeResult1 = this.RecognizeSubstringCall((QueryNode) binaryOperatorNode1.Left, this.modelInfo.IdProperty);
          if (recognizeResult1.Succeeded && binaryOperatorNode1.Right.SkipConvertNodes() is BinaryOperatorNode binaryOperatorNode2 && binaryOperatorNode2.OperatorKind == BinaryOperatorKind.And && RecognizeTitleNotEqualNull((QueryNode) binaryOperatorNode2.Left))
          {
            RecognizeResult<SubstringCall> recognizeResult2 = this.RecognizeSubstringCall((QueryNode) binaryOperatorNode2.Right, this.modelInfo.TitleProperty);
            if (recognizeResult2.Succeeded && recognizeResult2.Result.Needle == recognizeResult1.Result.Needle && recognizeResult2.Result.Haystack.HasToLower == recognizeResult1.Result.Haystack.HasToLower)
            {
              string needle = recognizeResult2.Result.Needle;
              return new RecognizeResult<PackageNameSelector>(true, string.IsNullOrWhiteSpace(needle) ? PackageNameSelector.AllPackageNames : new PackageNameSelector(needle, NameMatchFields.Id | NameMatchFields.Title, NameMatchType.Substring, recognizeResult2.Result.Haystack.HasToLower));
            }
          }
        }
        return new RecognizeResult<PackageNameSelector>(false, (PackageNameSelector) null);
      }

      bool RecognizeTitleNotEqualNull(QueryNode node1)
      {
        node1 = node1.SkipConvertNodes();
        if (node1 is BinaryOperatorNode binaryOperatorNode3 && binaryOperatorNode3.OperatorKind == BinaryOperatorKind.NotEqual)
        {
          RecognizeResult<IEdmProperty> recognizeResult = this.RecognizePropertyAccess((QueryNode) binaryOperatorNode3.Left);
          if (recognizeResult.Succeeded && recognizeResult.Result == this.modelInfo.TitleProperty && binaryOperatorNode3.Right.SkipConvertNodes() is ConstantNode constantNode && constantNode.Value == null)
            return true;
        }
        return false;
      }
    }

    private RecognizeResult<PackageSelectorList> RecognizePackageNameSelectorList(QueryNode node)
    {
      node = node.SkipConvertNodes();
      RecognizeResult<PackageNameSelector> recognizeResult1 = this.RecognizePackageNameSelector(node);
      if (recognizeResult1.Succeeded)
        return new RecognizeResult<PackageSelectorList>(true, new PackageSelectorList(ImmutableList.Create<PackageSelector>(new PackageSelector(recognizeResult1.Result, PackageVersionSelector.AllVersions))));
      if (node is BinaryOperatorNode binaryOperatorNode && binaryOperatorNode.OperatorKind == BinaryOperatorKind.Or)
      {
        RecognizeResult<PackageSelectorList> recognizeResult2 = this.RecognizePackageNameSelectorList((QueryNode) binaryOperatorNode.Left);
        if (recognizeResult2.Succeeded)
        {
          RecognizeResult<PackageSelectorList> recognizeResult3 = this.RecognizePackageNameSelectorList((QueryNode) binaryOperatorNode.Right);
          if (recognizeResult3.Succeeded)
            return new RecognizeResult<PackageSelectorList>(true, PackageSelectorList.Combine(recognizeResult2.Result, recognizeResult3.Result));
        }
      }
      return new RecognizeResult<PackageSelectorList>(false, (PackageSelectorList) null);
    }

    private RecognizeResult<SubstringCall> RecognizeSubstringCall(
      QueryNode node,
      IEdmProperty expectedProperty = null)
    {
      node = node.SkipConvertNodes();
      if (node is SingleValueFunctionCallNode functionCallNode)
      {
        List<QueryNode> list = functionCallNode.Arguments.ToList<QueryNode>();
        if (functionCallNode.Name == "substringof" && functionCallNode.Source == null && list.Count == 2)
        {
          RecognizeResult<string> recognizeResult1 = this.RecognizeStringConstant(list[0]);
          RecognizeResult<LowerableProperty> recognizeResult2 = this.RecognizeLowerablePropertyAccess(list[1]);
          if (recognizeResult1.Succeeded && recognizeResult2.Succeeded && (expectedProperty == null || expectedProperty == recognizeResult2.Result.Property))
            return new RecognizeResult<SubstringCall>(true, new SubstringCall(recognizeResult1.Result, recognizeResult2.Result));
        }
      }
      return new RecognizeResult<SubstringCall>(false, (SubstringCall) null);
    }

    private RecognizeResult<LowerableProperty> RecognizeLowerablePropertyAccess(QueryNode node)
    {
      node = node.SkipConvertNodes();
      bool hasToLower = false;
      if (node is SingleValueFunctionCallNode functionCallNode)
      {
        List<QueryNode> list = functionCallNode.Arguments.ToList<QueryNode>();
        if (!(functionCallNode.Name == "tolower") || functionCallNode.Source != null || list.Count != 1)
          return new RecognizeResult<LowerableProperty>(false, (LowerableProperty) null);
        hasToLower = true;
        node = list.First<QueryNode>().SkipConvertNodes();
      }
      return node is SingleValuePropertyAccessNode propertyAccessNode ? new RecognizeResult<LowerableProperty>(true, new LowerableProperty(propertyAccessNode.Property, hasToLower)) : new RecognizeResult<LowerableProperty>(false, (LowerableProperty) null);
    }

    private RecognizeResult<IEdmProperty> RecognizePropertyAccess(QueryNode node)
    {
      node = node.SkipConvertNodes();
      return node is SingleValuePropertyAccessNode propertyAccessNode ? new RecognizeResult<IEdmProperty>(true, propertyAccessNode.Property) : new RecognizeResult<IEdmProperty>(false, (IEdmProperty) null);
    }

    private RecognizeResult<string> RecognizeStringConstant(QueryNode node)
    {
      node = node.SkipConvertNodes();
      return node is ConstantNode constantNode && constantNode.TypeReference is EdmStringTypeReference ? new RecognizeResult<string>(true, (string) constantNode.Value) : new RecognizeResult<string>(false, (string) null);
    }
  }
}
