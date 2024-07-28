// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.ODataQueryClassifier
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Query.Validators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Microsoft.OData.UriParser.Aggregation;
using Microsoft.VisualStudio.Services.Analytics.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  internal class ODataQueryClassifier
  {
    private const int MaxAllowedProperties = 10;
    private static readonly HashSet<string> _selfExpandNavigationSources = new HashSet<string>()
    {
      "WorkItem",
      "WorkItemRevision"
    };
    private static readonly Dictionary<ODataQueryWarnings, string> _warningsMap = new Dictionary<ODataQueryWarnings, string>()
    {
      {
        ODataQueryWarnings.NoSelectOrApply,
        AnalyticsResources.ODATA_QUERY_NO_SELECT_OR_APPLY()
      },
      {
        ODataQueryWarnings.ParentChildRelations,
        AnalyticsResources.ODATA_QUERY_PARENT_CHILD_RELATIONS()
      },
      {
        ODataQueryWarnings.WideSelectOrApply,
        AnalyticsResources.ODATA_QUERY_TOO_WIDE((object) 10)
      },
      {
        ODataQueryWarnings.DistinctPropertyInGroupBy,
        AnalyticsResources.ODATA_QUERY_DISTINCT_COLUMNS_IN_LAST_GROUPBY()
      }
    };
    private ODataQueryOptions _query;
    private IEdmModel _model;
    private ODataQueryWarnings? _warnings;
    private bool _containsValidApply;
    private bool _containsValidSelect;
    private ODataValidationSettings _validationSettings;

    public ODataQueryClassifier(ODataQueryOptions query)
    {
      this._query = query;
      this._model = this._query.Request.GetModel();
      this._validationSettings = this._query.Request.GetRequestContainer().GetRequiredService<ODataValidationSettings>();
    }

    public bool IsWidgetLike()
    {
      this.Validate();
      Type elementClrType = this._query.Context.ElementClrType;
      if (this._containsValidApply && typeof (WorkItemCommon).IsAssignableFrom(elementClrType))
        return true;
      return this._containsValidSelect && elementClrType == typeof (WorkItem);
    }

    public void Validate()
    {
      if (this._warnings.HasValue)
        return;
      this._warnings = new ODataQueryWarnings?(ODataQueryWarnings.None);
      if (this._query.Apply == null && this._query.SelectExpand == null)
      {
        ODataQueryWarnings? warnings = this._warnings;
        this._warnings = warnings.HasValue ? new ODataQueryWarnings?(warnings.GetValueOrDefault() | ODataQueryWarnings.NoSelectOrApply) : new ODataQueryWarnings?();
      }
      Type elementClrType = this._query.Context.ElementClrType;
      this.ValidateApply(this._query.Apply?.ApplyClause);
      this.ValidateSelect(this._query.SelectExpand?.SelectExpandClause, this._query.Filter?.FilterClause);
    }

    public ODataQueryWarnings Warnings
    {
      get
      {
        this.Validate();
        return this._warnings.Value;
      }
    }

    public List<string> WarningsAsStrings
    {
      get
      {
        ODataQueryWarnings warnings = this.Warnings;
        return warnings != ODataQueryWarnings.None ? Enum.GetValues(typeof (ODataQueryWarnings)).Cast<ODataQueryWarnings>().Where<ODataQueryWarnings>((Func<ODataQueryWarnings, bool>) (f => f != 0)).Where<ODataQueryWarnings>((Func<ODataQueryWarnings, bool>) (f => (f & warnings) == f)).Select<ODataQueryWarnings, string>((Func<ODataQueryWarnings, string>) (f => ODataQueryClassifier._warningsMap[f])).ToList<string>() : new List<string>();
      }
    }

    private void ValidateApply(ApplyClause apply)
    {
      if (apply == null)
        return;
      ODataQueryWarnings odataQueryWarnings1 = ODataQueryWarnings.None;
      if (!apply.Transformations.Any<TransformationNode>((Func<TransformationNode, bool>) (t => t.Kind != TransformationNodeKind.Filter)))
      {
        ODataQueryWarnings odataQueryWarnings2 = odataQueryWarnings1 | ODataQueryWarnings.NoSelectOrApply;
      }
      else
      {
        GroupByTransformationNode transformationNode1 = apply.Transformations.OfType<GroupByTransformationNode>().LastOrDefault<GroupByTransformationNode>();
        if (transformationNode1 != null && transformationNode1.GroupingProperties.Count<GroupByPropertyNode>() > 10)
          odataQueryWarnings1 |= ODataQueryWarnings.WideSelectOrApply;
        FilterTransformationNode transformationNode2 = apply.Transformations.OfType<FilterTransformationNode>().FirstOrDefault<FilterTransformationNode>();
        if (transformationNode2 != null && this.HasNavigationToWorkItem(transformationNode2.FilterClause))
          odataQueryWarnings1 |= ODataQueryWarnings.ParentChildRelations;
        ODataQueryWarnings? warnings = this._warnings;
        ODataQueryWarnings odataQueryWarnings3 = odataQueryWarnings1;
        this._warnings = warnings.HasValue ? new ODataQueryWarnings?(warnings.GetValueOrDefault() | odataQueryWarnings3) : new ODataQueryWarnings?();
        this._containsValidApply = odataQueryWarnings1 == ODataQueryWarnings.None;
      }
    }

    private void ValidateSelect(SelectExpandClause select, FilterClause filter)
    {
      if (select == null)
        return;
      ODataQueryWarnings odataQueryWarnings1 = ODataQueryWarnings.None;
      if (select.SelectedItems.OfType<ExpandedNavigationSelectItem>().Any<ExpandedNavigationSelectItem>((Func<ExpandedNavigationSelectItem, bool>) (e => ODataQueryClassifier._selfExpandNavigationSources.Contains(e.NavigationSource.EntityType().Name))))
        odataQueryWarnings1 |= ODataQueryWarnings.ParentChildRelations;
      if (select.AllSelected)
        odataQueryWarnings1 |= ODataQueryWarnings.NoSelectOrApply;
      if (select.SelectedItems.Count<SelectItem>() > 10)
        odataQueryWarnings1 |= ODataQueryWarnings.WideSelectOrApply;
      if (filter != null && this.HasNavigationToWorkItem(filter))
        odataQueryWarnings1 |= ODataQueryWarnings.ParentChildRelations;
      ODataQueryWarnings? warnings = this._warnings;
      ODataQueryWarnings odataQueryWarnings2 = odataQueryWarnings1;
      this._warnings = warnings.HasValue ? new ODataQueryWarnings?(warnings.GetValueOrDefault() | odataQueryWarnings2) : new ODataQueryWarnings?();
      this._containsValidSelect = odataQueryWarnings1 == ODataQueryWarnings.None;
    }

    private bool HasNavigationToWorkItem(FilterClause filter)
    {
      ODataQueryClassifier.FilterValidator filterValidator = new ODataQueryClassifier.FilterValidator(new DefaultQuerySettings()
      {
        EnableFilter = true
      });
      filterValidator.Validate(filter, this._validationSettings, this._model);
      return filterValidator.NavigatesToWorkItem;
    }

    private class FilterValidator : FilterQueryValidator
    {
      public bool NavigatesToWorkItem { get; private set; }

      public FilterValidator(DefaultQuerySettings defaultQuerySettings)
        : base(defaultQuerySettings)
      {
      }

      public override void ValidateNavigationPropertyNode(
        QueryNode sourceNode,
        IEdmNavigationProperty navigationProperty,
        ODataValidationSettings settings)
      {
        HashSet<string> navigationSources = ODataQueryClassifier._selfExpandNavigationSources;
        IEdmEntityTypeReference type = navigationProperty.Type.AsEntity();
        string name = type != null ? type.EntityDefinition()?.Name : (string) null;
        if (navigationSources.Contains(name))
          this.NavigatesToWorkItem = true;
        base.ValidateNavigationPropertyNode(sourceNode, navigationProperty, settings);
      }
    }
  }
}
