// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.UseNoHitViewsForRollupQuery
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Query.Validators;
using Microsoft.OData.UriParser;
using Microsoft.VisualStudio.Services.Analytics.Model;
using Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure;
using System;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  internal class UseNoHitViewsForRollupQuery : IHintStrategy
  {
    public SqlOptions GetOptions(
      Type entitySetType,
      QueryType queryType,
      IQueryable query,
      ODataQueryOptions odataQueryOptions)
    {
      if (queryType == QueryType.Raw && entitySetType == typeof (WorkItem) && odataQueryOptions?.Filter?.FilterClause != null)
      {
        bool? nullable;
        if (odataQueryOptions == null)
        {
          nullable = new bool?();
        }
        else
        {
          SelectExpandQueryOption selectExpand = odataQueryOptions.SelectExpand;
          nullable = selectExpand != null ? new bool?(selectExpand.SelectExpandClause.SelectedItems.OfType<ExpandedNavigationSelectItem>().Any<ExpandedNavigationSelectItem>((Func<ExpandedNavigationSelectItem, bool>) (e => e.NavigationSource.Name == "WorkItems" && e.ApplyOption.IsAggregated()))) : new bool?();
        }
        if (nullable.GetValueOrDefault())
        {
          UseNoHitViewsForRollupQuery.PropertyCollector propertyCollector = new UseNoHitViewsForRollupQuery.PropertyCollector(new DefaultQuerySettings()
          {
            EnableFilter = true
          });
          propertyCollector.Validate(odataQueryOptions.Filter.FilterClause, AnalyticsODataDefaults.DefaultValidationSettings, odataQueryOptions.Context.Model);
          if (propertyCollector.UsesWorkItemId)
            return SqlOptions.NoHintViewForRollup;
        }
      }
      return SqlOptions.None;
    }

    private class PropertyCollector : FilterQueryValidator
    {
      public bool UsesWorkItemId { get; private set; }

      public PropertyCollector(DefaultQuerySettings defaultQuerySettings)
        : base(defaultQuerySettings)
      {
      }

      public override void ValidateSingleValuePropertyAccessNode(
        SingleValuePropertyAccessNode propertyAccessNode,
        ODataValidationSettings settings)
      {
        if (propertyAccessNode.Property.Name == "WorkItemId")
          this.UsesWorkItemId = true;
        base.ValidateSingleValuePropertyAccessNode(propertyAccessNode, settings);
      }
    }
  }
}
