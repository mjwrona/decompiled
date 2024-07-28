// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.FilterQueryOption
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Query.Validators;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.AspNet.OData.Query
{
  public class FilterQueryOption
  {
    private FilterClause _filterClause;
    private ODataQueryOptionParser _queryOptionParser;

    public FilterQueryOption(
      string rawValue,
      ODataQueryContext context,
      ODataQueryOptionParser queryOptionParser)
    {
      if (context == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (context));
      if (string.IsNullOrEmpty(rawValue))
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNullOrEmpty(nameof (rawValue));
      if (queryOptionParser == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (queryOptionParser));
      this.Context = context;
      this.RawValue = rawValue;
      this.Validator = FilterQueryValidator.GetFilterQueryValidator(context);
      this._queryOptionParser = queryOptionParser;
    }

    internal FilterQueryOption(string rawValue, ODataQueryContext context)
    {
      if (context == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (context));
      if (string.IsNullOrEmpty(rawValue))
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNullOrEmpty(nameof (rawValue));
      this.Context = context;
      this.RawValue = rawValue;
      this.Validator = FilterQueryValidator.GetFilterQueryValidator(context);
      IEdmModel model = context.Model;
      IEdmType elementType = context.ElementType;
      IEdmNavigationSource navigationSource = context.NavigationSource;
      Dictionary<string, string> queryOptions = new Dictionary<string, string>();
      queryOptions.Add("$filter", rawValue);
      IServiceProvider requestContainer = context.RequestContainer;
      this._queryOptionParser = new ODataQueryOptionParser(model, elementType, navigationSource, (IDictionary<string, string>) queryOptions, requestContainer);
    }

    public ODataQueryContext Context { get; private set; }

    public FilterQueryValidator Validator { get; set; }

    public FilterClause FilterClause
    {
      get
      {
        if (this._filterClause == null)
        {
          this._filterClause = this._queryOptionParser.ParseFilter();
          if (!(this._filterClause.Expression.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) new ParameterAliasNodeTranslator(this._queryOptionParser.ParameterAliasNodes)) is SingleValueNode expression))
            expression = (SingleValueNode) new ConstantNode((object) null);
          this._filterClause = new FilterClause(expression, this._filterClause.RangeVariable);
        }
        return this._filterClause;
      }
    }

    public string RawValue { get; private set; }

    public IQueryable ApplyTo(IQueryable query, ODataQuerySettings querySettings)
    {
      if (query == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (query));
      if (querySettings == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (querySettings));
      if (this.Context.ElementClrType == (Type) null)
        throw Microsoft.AspNet.OData.Common.Error.NotSupported(SRResources.ApplyToOnUntypedQueryOption, (object) nameof (ApplyTo));
      FilterClause filterClause = this.FilterClause;
      ODataQuerySettings querySettings1 = this.Context.UpdateQuerySettings(querySettings, query);
      Expression where = Microsoft.AspNet.OData.Query.Expressions.FilterBinder.Bind(query, filterClause, this.Context.ElementClrType, this.Context, querySettings1);
      query = ExpressionHelpers.Where(query, where, query.ElementType);
      return query;
    }

    public void Validate(ODataValidationSettings validationSettings)
    {
      if (validationSettings == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (validationSettings));
      if (this.Validator == null)
        return;
      this.Validator.Validate(this, validationSettings);
    }
  }
}
