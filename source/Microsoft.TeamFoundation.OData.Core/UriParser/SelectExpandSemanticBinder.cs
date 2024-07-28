// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.SelectExpandSemanticBinder
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

namespace Microsoft.OData.UriParser
{
  internal sealed class SelectExpandSemanticBinder
  {
    public static SelectExpandClause Bind(
      ODataPathInfo odataPathInfo,
      ExpandToken expandToken,
      SelectToken selectToken,
      ODataUriParserConfiguration configuration,
      BindingState state)
    {
      ExpandToken expandToken1 = ExpandTreeNormalizer.NormalizeExpandTree(expandToken);
      SelectToken selectToken1 = SelectTreeNormalizer.NormalizeSelectTree(selectToken);
      SelectExpandClause selectExpandClause = new SelectExpandBinder(configuration, odataPathInfo, state).Bind(expandToken1, selectToken1);
      SelectExpandClauseFinisher.AddExplicitNavPropLinksWhereNecessary(selectExpandClause);
      new ExpandDepthAndCountValidator(configuration.Settings.MaximumExpansionDepth, configuration.Settings.MaximumExpansionCount).Validate(selectExpandClause);
      return selectExpandClause;
    }
  }
}
