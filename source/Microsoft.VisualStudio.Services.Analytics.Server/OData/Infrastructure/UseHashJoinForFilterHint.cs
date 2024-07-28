// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.UseHashJoinForFilterHint
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.AspNet.OData.Query;
using Microsoft.OData.UriParser;
using Microsoft.OData.UriParser.Aggregation;
using Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  internal class UseHashJoinForFilterHint : IHintStrategy
  {
    private SqlOptions _hintOption;
    private HashSet<Type> _tableTypes = new HashSet<Type>();
    private HashSet<string> _filterTypes = new HashSet<string>();

    public UseHashJoinForFilterHint(
      SqlOptions sqlOptionsToReturn,
      ICollection<Type> entityTypes,
      ICollection<Type> filterOnTypes)
    {
      this._hintOption = sqlOptionsToReturn == SqlOptions.HashJoinFilterHint ? sqlOptionsToReturn : throw new ArgumentException(AnalyticsResources.UNSUPPORTED_SQL_OPTIONS((object) sqlOptionsToReturn, (object) nameof (sqlOptionsToReturn)));
      this._tableTypes.UnionWith((IEnumerable<Type>) entityTypes);
      foreach (Type filterOnType in (IEnumerable<Type>) filterOnTypes)
        this._filterTypes.Add(filterOnType.FullName);
    }

    public SqlOptions GetOptions(
      Type entitySetType,
      QueryType queryType,
      IQueryable query,
      ODataQueryOptions odataQueryOptions)
    {
      return this._tableTypes.Contains(entitySetType) && odataQueryOptions?.Apply != null && odataQueryOptions.Apply.ApplyClause.Transformations.Any<TransformationNode>((Func<TransformationNode, bool>) (t => t.Kind == TransformationNodeKind.Filter)) && odataQueryOptions.Apply.ApplyClause.Transformations.OfType<FilterTransformationNode>().Any<FilterTransformationNode>((Func<FilterTransformationNode, bool>) (n => this.HasNavigationToTag(n.FilterClause, odataQueryOptions))) ? this._hintOption : SqlOptions.None;
    }

    private bool HasNavigationToTag(FilterClause filter, ODataQueryOptions odataQueryOptions)
    {
      MyValidator myValidator = new MyValidator(new DefaultQuerySettings()
      {
        EnableFilter = true
      }, (ISet<string>) this._filterTypes);
      myValidator.Validate(filter, AnalyticsODataDefaults.DefaultValidationSettings, odataQueryOptions.Context.Model);
      return myValidator.NavigatesToTag;
    }
  }
}
