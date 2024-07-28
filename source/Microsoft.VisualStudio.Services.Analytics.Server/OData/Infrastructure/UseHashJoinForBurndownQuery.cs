// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.UseHashJoinForBurndownQuery
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Query.Validators;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Microsoft.OData.UriParser.Aggregation;
using Microsoft.VisualStudio.Services.Analytics.Model;
using Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  internal class UseHashJoinForBurndownQuery : IHintStrategy
  {
    private static readonly HashSet<string> _expected = new HashSet<string>()
    {
      "DateSK",
      "StateCategory",
      "WorkItemType"
    };

    public SqlOptions GetOptions(
      Type entitySetType,
      QueryType queryType,
      IQueryable query,
      ODataQueryOptions odataQueryOptions)
    {
      if (entitySetType == typeof (WorkItemSnapshot))
      {
        bool? nullable;
        if (odataQueryOptions == null)
        {
          nullable = new bool?();
        }
        else
        {
          ApplyQueryOption apply = odataQueryOptions.Apply;
          if (apply == null)
          {
            nullable = new bool?();
          }
          else
          {
            ApplyClause applyClause = apply.ApplyClause;
            nullable = applyClause != null ? new bool?(applyClause.IsAggregated()) : new bool?();
          }
        }
        if (nullable.GetValueOrDefault())
        {
          GroupByTransformationNode transformationNode1 = odataQueryOptions.Apply.ApplyClause.Transformations.OfType<GroupByTransformationNode>().FirstOrDefault<GroupByTransformationNode>();
          if (transformationNode1 != null && transformationNode1.GroupingProperties.Count<GroupByPropertyNode>() == 3 && transformationNode1.GroupingProperties.All<GroupByPropertyNode>((Func<GroupByPropertyNode, bool>) (p => UseHashJoinForBurndownQuery._expected.Contains(p.Name))))
          {
            UseHashJoinForBurndownQuery.NavigationCollector navigationCollector = new UseHashJoinForBurndownQuery.NavigationCollector(new DefaultQuerySettings()
            {
              EnableFilter = true
            });
            foreach (FilterTransformationNode transformationNode2 in odataQueryOptions.Apply.ApplyClause.Transformations.OfType<FilterTransformationNode>())
              navigationCollector.Validate(transformationNode2.FilterClause, AnalyticsODataDefaults.DefaultValidationSettings, odataQueryOptions.Context.Model);
            if (navigationCollector.NavigatesToIteration && navigationCollector.NavigatesToTeams)
              return SqlOptions.HashJoinForBurnDownHint;
          }
        }
      }
      return SqlOptions.None;
    }

    private class NavigationCollector : FilterQueryValidator
    {
      public bool NavigatesToIteration { get; private set; }

      public bool NavigatesToTeams { get; private set; }

      public NavigationCollector(DefaultQuerySettings defaultQuerySettings)
        : base(defaultQuerySettings)
      {
      }

      public override void ValidateNavigationPropertyNode(
        QueryNode sourceNode,
        IEdmNavigationProperty navigationProperty,
        ODataValidationSettings settings)
      {
        if (navigationProperty.Type.Definition.AsElementType() is EdmEntityType edmEntityType)
        {
          if (edmEntityType.Name == "Team")
            this.NavigatesToTeams = true;
          if (edmEntityType.Name == "Iteration")
            this.NavigatesToIteration = true;
        }
        base.ValidateNavigationPropertyNode(sourceNode, navigationProperty, settings);
      }
    }
  }
}
