// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.ExpandTreeNormalizer
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.UriParser
{
  internal static class ExpandTreeNormalizer
  {
    public static ExpandToken NormalizeExpandTree(ExpandToken treeToNormalize) => treeToNormalize == null ? (ExpandToken) null : ExpandTreeNormalizer.CombineTerms(ExpandTreeNormalizer.NormalizePaths(treeToNormalize));

    public static ExpandToken NormalizePaths(ExpandToken treeToInvert)
    {
      if (treeToInvert != null)
      {
        foreach (ExpandTermToken expandTerm in treeToInvert.ExpandTerms)
        {
          expandTerm.PathToProperty = expandTerm.PathToProperty.Reverse();
          if (expandTerm.SelectOption != null)
            expandTerm.SelectOption = SelectTreeNormalizer.NormalizeSelectTree(expandTerm.SelectOption);
          if (expandTerm.ExpandOption != null)
            expandTerm.ExpandOption = ExpandTreeNormalizer.NormalizePaths(expandTerm.ExpandOption);
        }
      }
      return treeToInvert;
    }

    public static ExpandToken CombineTerms(ExpandToken treeToCollapse)
    {
      Dictionary<PathSegmentToken, ExpandTermToken> combinedTerms = new Dictionary<PathSegmentToken, ExpandTermToken>((IEqualityComparer<PathSegmentToken>) new PathSegmentTokenEqualityComparer());
      foreach (ExpandTermToken expandTerm in treeToCollapse.ExpandTerms)
      {
        ExpandTermToken expandedTerm = expandTerm;
        if (expandTerm.ExpandOption != null)
        {
          ExpandToken expandOption = ExpandTreeNormalizer.CombineTerms(expandTerm.ExpandOption);
          expandedTerm = new ExpandTermToken(expandTerm.PathToNavigationProp, expandTerm.FilterOption, expandTerm.OrderByOptions, expandTerm.TopOption, expandTerm.SkipOption, expandTerm.CountQueryOption, expandTerm.LevelsOption, expandTerm.SearchOption, ExpandTreeNormalizer.RemoveDuplicateSelect(expandTerm.SelectOption), expandOption, expandTerm.ComputeOption, expandTerm.ApplyOptions);
        }
        ExpandTreeNormalizer.AddOrCombine((IDictionary<PathSegmentToken, ExpandTermToken>) combinedTerms, expandedTerm);
      }
      return new ExpandToken((IEnumerable<ExpandTermToken>) combinedTerms.Values);
    }

    public static ExpandTermToken CombineTerms(
      ExpandTermToken existingToken,
      ExpandTermToken newToken)
    {
      List<ExpandTermToken> list = ExpandTreeNormalizer.CombineChildNodes(existingToken, newToken).ToList<ExpandTermToken>();
      SelectToken selectOption = ExpandTreeNormalizer.CombineSelects(existingToken, newToken);
      return new ExpandTermToken(existingToken.PathToNavigationProp, existingToken.FilterOption, existingToken.OrderByOptions, existingToken.TopOption, existingToken.SkipOption, existingToken.CountQueryOption, existingToken.LevelsOption, existingToken.SearchOption, selectOption, new ExpandToken((IEnumerable<ExpandTermToken>) list), existingToken.ComputeOption, existingToken.ApplyOptions);
    }

    public static IEnumerable<ExpandTermToken> CombineChildNodes(
      ExpandTermToken existingToken,
      ExpandTermToken newToken)
    {
      if (existingToken.ExpandOption == null && newToken.ExpandOption == null)
        return (IEnumerable<ExpandTermToken>) new List<ExpandTermToken>();
      Dictionary<PathSegmentToken, ExpandTermToken> combinedTerms = new Dictionary<PathSegmentToken, ExpandTermToken>((IEqualityComparer<PathSegmentToken>) new PathSegmentTokenEqualityComparer());
      if (existingToken.ExpandOption != null)
        ExpandTreeNormalizer.AddChildOptionsToDictionary(existingToken, combinedTerms);
      if (newToken.ExpandOption != null)
        ExpandTreeNormalizer.AddChildOptionsToDictionary(newToken, combinedTerms);
      return (IEnumerable<ExpandTermToken>) combinedTerms.Values;
    }

    private static void AddChildOptionsToDictionary(
      ExpandTermToken newToken,
      Dictionary<PathSegmentToken, ExpandTermToken> combinedTerms)
    {
      foreach (ExpandTermToken expandTerm in newToken.ExpandOption.ExpandTerms)
        ExpandTreeNormalizer.AddOrCombine((IDictionary<PathSegmentToken, ExpandTermToken>) combinedTerms, expandTerm);
    }

    private static void AddOrCombine(
      IDictionary<PathSegmentToken, ExpandTermToken> combinedTerms,
      ExpandTermToken expandedTerm)
    {
      ExpandTermToken newToken;
      if (combinedTerms.TryGetValue(expandedTerm.PathToNavigationProp, out newToken))
        combinedTerms[expandedTerm.PathToNavigationProp] = ExpandTreeNormalizer.CombineTerms(expandedTerm, newToken);
      else
        combinedTerms.Add(expandedTerm.PathToNavigationProp, expandedTerm);
    }

    private static SelectToken CombineSelects(
      ExpandTermToken existingToken,
      ExpandTermToken newToken)
    {
      if (existingToken.SelectOption == null)
        return newToken.SelectOption;
      if (newToken.SelectOption == null)
        return existingToken.SelectOption;
      List<PathSegmentToken> list = existingToken.SelectOption.Properties.ToList<PathSegmentToken>();
      list.AddRange(newToken.SelectOption.Properties);
      return new SelectToken(list.Distinct<PathSegmentToken>((IEqualityComparer<PathSegmentToken>) new PathSegmentTokenEqualityComparer()));
    }

    private static SelectToken RemoveDuplicateSelect(SelectToken selectToken) => selectToken == null ? (SelectToken) null : new SelectToken(selectToken.Properties.Distinct<PathSegmentToken>((IEqualityComparer<PathSegmentToken>) new PathSegmentTokenEqualityComparer()));
  }
}
