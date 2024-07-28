// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.SelectTreeNormalizer
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

namespace Microsoft.OData.UriParser
{
  internal static class SelectTreeNormalizer
  {
    public static SelectToken NormalizeSelectTree(SelectToken selectToken)
    {
      selectToken = SelectTreeNormalizer.NormalizeSelectPaths(selectToken);
      return selectToken;
    }

    private static SelectToken NormalizeSelectPaths(SelectToken selectToken)
    {
      if (selectToken != null)
      {
        foreach (SelectTermToken selectTerm in selectToken.SelectTerms)
        {
          selectTerm.PathToProperty = selectTerm.PathToProperty.Reverse();
          if (selectTerm.SelectOption != null)
            selectTerm.SelectOption = SelectTreeNormalizer.NormalizeSelectPaths(selectTerm.SelectOption);
        }
      }
      return selectToken;
    }
  }
}
