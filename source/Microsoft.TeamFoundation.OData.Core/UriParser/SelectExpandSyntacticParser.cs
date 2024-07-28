// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.SelectExpandSyntacticParser
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
  internal static class SelectExpandSyntacticParser
  {
    public static void Parse(
      string selectClause,
      string expandClause,
      IEdmStructuredType parentStructuredType,
      ODataUriParserConfiguration configuration,
      out ExpandToken expandTree,
      out SelectToken selectTree)
    {
      SelectExpandParser selectExpandParser1 = new SelectExpandParser(selectClause, configuration.Settings.SelectExpandLimit, configuration.EnableCaseInsensitiveUriFunctionIdentifier)
      {
        MaxPathDepth = configuration.Settings.PathLimit
      };
      selectTree = selectExpandParser1.ParseSelect();
      SelectExpandParser selectExpandParser2 = new SelectExpandParser(configuration.Resolver, expandClause, parentStructuredType, configuration.Settings.SelectExpandLimit, configuration.EnableCaseInsensitiveUriFunctionIdentifier, configuration.EnableNoDollarQueryOptions)
      {
        MaxPathDepth = configuration.Settings.PathLimit,
        MaxFilterDepth = configuration.Settings.FilterLimit,
        MaxOrderByDepth = configuration.Settings.OrderByLimit,
        MaxSearchDepth = configuration.Settings.SearchLimit
      };
      expandTree = selectExpandParser2.ParseExpand();
    }
  }
}
