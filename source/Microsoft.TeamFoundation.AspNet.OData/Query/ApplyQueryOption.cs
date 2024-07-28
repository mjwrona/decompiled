// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.ApplyQueryOption
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.OData.UriParser;
using Microsoft.OData.UriParser.Aggregation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.OData.Query
{
  public class ApplyQueryOption
  {
    private ApplyClause _applyClause;
    private ODataQueryOptionParser _queryOptionParser;

    public ApplyQueryOption(
      string rawValue,
      ODataQueryContext context,
      ODataQueryOptionParser queryOptionParser)
    {
      if (context == null)
        throw Error.ArgumentNull(nameof (context));
      if (string.IsNullOrEmpty(rawValue))
        throw Error.ArgumentNullOrEmpty(nameof (rawValue));
      if (queryOptionParser == null)
        throw Error.ArgumentNull(nameof (queryOptionParser));
      this.Context = context;
      this.RawValue = rawValue;
      this._queryOptionParser = queryOptionParser;
      this.ResultClrType = this.Context.ElementClrType;
    }

    public ODataQueryContext Context { get; private set; }

    public Type ResultClrType { get; private set; }

    public ApplyClause ApplyClause
    {
      get
      {
        if (this._applyClause == null)
        {
          this._applyClause = this._queryOptionParser.ParseApply();
          if (this._queryOptionParser.ParameterAliasNodes.Any<KeyValuePair<string, SingleValueNode>>())
          {
            List<TransformationNode> transformations = new List<TransformationNode>();
            foreach (TransformationNode transformation in this._applyClause.Transformations)
            {
              if (transformation is FilterTransformationNode transformationNode)
              {
                FilterClause filterClause1 = transformationNode.FilterClause;
                if (!(filterClause1.Expression.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) new ParameterAliasNodeTranslator(this._queryOptionParser.ParameterAliasNodes)) is SingleValueNode expression))
                  expression = (SingleValueNode) new ConstantNode((object) null);
                FilterClause filterClause2 = new FilterClause(expression, filterClause1.RangeVariable);
                transformations.Add((TransformationNode) new FilterTransformationNode(filterClause2));
              }
              else
                transformations.Add(transformation);
            }
            this._applyClause = new ApplyClause((IList<TransformationNode>) transformations);
          }
        }
        return this._applyClause;
      }
    }

    internal SelectExpandClause SelectExpandClause { get; private set; }

    public string RawValue { get; private set; }

    public IQueryable ApplyTo(IQueryable query, ODataQuerySettings querySettings)
    {
      if (query == null)
        throw Error.ArgumentNull(nameof (query));
      if (querySettings == null)
        throw Error.ArgumentNull(nameof (querySettings));
      if (this.Context.ElementClrType == (Type) null)
        throw Error.NotSupported(SRResources.ApplyToOnUntypedQueryOption, (object) nameof (ApplyTo));
      if (query.Provider.GetType().Namespace == "System.Data.Linq")
        throw Error.NotSupported(SRResources.ApplyQueryOptionNotSupportedForLinq2SQL);
      ApplyClause applyClause = this.ApplyClause;
      ApplyQueryOptionsBinder queryOptionsBinder = new ApplyQueryOptionsBinder(this.Context, this.Context.UpdateQuerySettings(querySettings, query), this.ResultClrType);
      query = queryOptionsBinder.Bind(query, this.ApplyClause);
      this.ResultClrType = queryOptionsBinder.ResultClrType;
      this.SelectExpandClause = queryOptionsBinder.SelectExpandClause;
      return query;
    }
  }
}
