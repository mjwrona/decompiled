// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.ApplyQueryOptionsBinder
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Adapters;
using Microsoft.AspNet.OData.Interfaces;
using Microsoft.AspNet.OData.Query.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.UriParser;
using Microsoft.OData.UriParser.Aggregation;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.AspNet.OData.Query
{
  internal class ApplyQueryOptionsBinder
  {
    private ODataQueryContext _context;
    private ODataQuerySettings _settings;
    private IWebApiAssembliesResolver _assembliesResolver;

    public ApplyQueryOptionsBinder(
      ODataQueryContext context,
      ODataQuerySettings settings,
      Type clrType)
    {
      this._context = context;
      this._settings = settings;
      this.ResultClrType = clrType;
      IServiceProvider requestContainer = this._context.RequestContainer;
      this._assembliesResolver = (requestContainer != null ? requestContainer.GetService<IWebApiAssembliesResolver>() : (IWebApiAssembliesResolver) null) ?? WebApiAssembliesResolver.Default;
    }

    public Type ResultClrType { get; private set; }

    internal SelectExpandClause SelectExpandClause { get; private set; }

    public IQueryable Bind(IQueryable query, ApplyClause applyClause)
    {
      bool flag = false;
      foreach (TransformationNode transformation in applyClause.Transformations)
      {
        if (transformation.Kind == TransformationNodeKind.Aggregate || transformation.Kind == TransformationNodeKind.GroupBy)
        {
          AggregationBinder aggregationBinder = new AggregationBinder(this._settings, this._assembliesResolver, this.ResultClrType, this._context.Model, transformation, this._context, this.SelectExpandClause);
          query = aggregationBinder.Bind(query);
          this.ResultClrType = aggregationBinder.ResultClrType;
          flag = true;
        }
        else if (transformation.Kind == TransformationNodeKind.Compute)
        {
          Microsoft.AspNet.OData.Query.Expressions.ComputeBinder computeBinder = new Microsoft.AspNet.OData.Query.Expressions.ComputeBinder(this._settings, this._assembliesResolver, this.ResultClrType, this._context.Model, (ComputeTransformationNode) transformation);
          query = computeBinder.Bind(query);
          this.ResultClrType = computeBinder.ResultClrType;
          flag = true;
        }
        else if (transformation.Kind == TransformationNodeKind.Filter)
        {
          FilterTransformationNode transformationNode = (FilterTransformationNode) transformation;
          Expression where = Microsoft.AspNet.OData.Query.Expressions.FilterBinder.Bind(query, transformationNode.FilterClause, this.ResultClrType, this._context, this._settings);
          query = ExpressionHelpers.Where(query, where, this.ResultClrType);
        }
        else if (transformation.Kind == TransformationNodeKind.Expand)
        {
          SelectExpandClause expandClause = ((ExpandTransformationNode) transformation).ExpandClause;
          this.SelectExpandClause = this.SelectExpandClause != null ? new SelectExpandClause(this.SelectExpandClause.SelectedItems.Concat<SelectItem>(expandClause.SelectedItems), false) : expandClause;
        }
      }
      if (this.SelectExpandClause != null && !flag)
      {
        SelectExpandQueryOption selectExpandQuery = new SelectExpandQueryOption((string) null, ApplyQueryOptionsBinder.GetExpandsOnlyString(this.SelectExpandClause), this._context, this.SelectExpandClause);
        query = Microsoft.AspNet.OData.Query.Expressions.SelectExpandBinder.Bind(query, this._settings, selectExpandQuery);
      }
      return query;
    }

    private static string GetExpandsOnlyString(SelectExpandClause selectExpandClause) => "$expand=" + string.Join(",", selectExpandClause.SelectedItems.OfType<ExpandedNavigationSelectItem>().Select<ExpandedNavigationSelectItem, string>((Func<ExpandedNavigationSelectItem, string>) (i => i.NavigationSource.Name)));
  }
}
