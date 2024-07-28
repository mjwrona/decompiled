// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.OrderByQueryOption
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Query.Validators;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.AspNet.OData.Query
{
  public class OrderByQueryOption
  {
    private OrderByClause _orderByClause;
    private IList<OrderByNode> _orderByNodes;
    private ODataQueryOptionParser _queryOptionParser;

    public OrderByQueryOption(
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
      this.Validator = OrderByQueryValidator.GetOrderByQueryValidator(context);
      this._queryOptionParser = queryOptionParser;
    }

    internal OrderByQueryOption(string rawValue, ODataQueryContext context, string applyRaw)
    {
      if (context == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (context));
      if (string.IsNullOrEmpty(rawValue))
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNullOrEmpty(nameof (rawValue));
      if (applyRaw == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNullOrEmpty(nameof (applyRaw));
      this.Context = context;
      this.RawValue = rawValue;
      this.Validator = OrderByQueryValidator.GetOrderByQueryValidator(context);
      IEdmModel model = context.Model;
      IEdmType elementType = context.ElementType;
      IEdmNavigationSource navigationSource = context.NavigationSource;
      Dictionary<string, string> queryOptions = new Dictionary<string, string>();
      queryOptions.Add("$orderby", rawValue);
      queryOptions.Add("$apply", applyRaw);
      IServiceProvider requestContainer = context.RequestContainer;
      this._queryOptionParser = new ODataQueryOptionParser(model, elementType, navigationSource, (IDictionary<string, string>) queryOptions, requestContainer);
      this._queryOptionParser.ParseApply();
    }

    internal OrderByQueryOption(string rawValue, ODataQueryContext context)
    {
      if (context == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (context));
      if (string.IsNullOrEmpty(rawValue))
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNullOrEmpty(nameof (rawValue));
      this.Context = context;
      this.RawValue = rawValue;
      this.Validator = OrderByQueryValidator.GetOrderByQueryValidator(context);
      IEdmModel model = context.Model;
      IEdmType elementType = context.ElementType;
      IEdmNavigationSource navigationSource = context.NavigationSource;
      Dictionary<string, string> queryOptions = new Dictionary<string, string>();
      queryOptions.Add("$orderby", rawValue);
      IServiceProvider requestContainer = context.RequestContainer;
      this._queryOptionParser = new ODataQueryOptionParser(model, elementType, navigationSource, (IDictionary<string, string>) queryOptions, requestContainer);
    }

    internal OrderByQueryOption(OrderByQueryOption orderBy)
    {
      this.Context = orderBy.Context;
      this.RawValue = orderBy.RawValue;
      this.Validator = orderBy.Validator;
      this._queryOptionParser = orderBy._queryOptionParser;
      this._orderByClause = orderBy._orderByClause;
      this._orderByNodes = orderBy._orderByNodes;
    }

    public ODataQueryContext Context { get; private set; }

    public IList<OrderByNode> OrderByNodes
    {
      get
      {
        if (this._orderByNodes == null)
          this._orderByNodes = OrderByNode.CreateCollection(this.OrderByClause);
        return this._orderByNodes;
      }
    }

    public string RawValue { get; private set; }

    public OrderByQueryValidator Validator { get; set; }

    public OrderByClause OrderByClause
    {
      get
      {
        if (this._orderByClause == null)
        {
          this._orderByClause = this._queryOptionParser.ParseOrderBy();
          this._orderByClause = this.TranslateParameterAlias(this._orderByClause);
        }
        return this._orderByClause;
      }
    }

    public IOrderedQueryable<T> ApplyTo<T>(IQueryable<T> query) => this.ApplyToCore((IQueryable) query, new ODataQuerySettings()) as IOrderedQueryable<T>;

    public IOrderedQueryable<T> ApplyTo<T>(IQueryable<T> query, ODataQuerySettings querySettings) => this.ApplyToCore((IQueryable) query, querySettings) as IOrderedQueryable<T>;

    public IOrderedQueryable ApplyTo(IQueryable query) => this.ApplyToCore(query, new ODataQuerySettings());

    public IOrderedQueryable ApplyTo(IQueryable query, ODataQuerySettings querySettings) => this.ApplyToCore(query, querySettings);

    public void Validate(ODataValidationSettings validationSettings)
    {
      if (validationSettings == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (validationSettings));
      if (this.Validator == null)
        return;
      this.Validator.Validate(this, validationSettings);
    }

    private IOrderedQueryable ApplyToCore(IQueryable query, ODataQuerySettings querySettings)
    {
      if (this.Context.ElementClrType == (Type) null)
        throw Microsoft.AspNet.OData.Common.Error.NotSupported(SRResources.ApplyToOnUntypedQueryOption, (object) "ApplyTo");
      IList<OrderByNode> orderByNodes = this.OrderByNodes;
      bool alreadyOrdered = false;
      IQueryable core = query;
      HashSet<object> objectSet = new HashSet<object>();
      HashSet<string> stringSet = new HashSet<string>();
      bool flag = false;
      foreach (OrderByNode orderByNode in (IEnumerable<OrderByNode>) orderByNodes)
      {
        OrderByPropertyNode orderByPropertyNode = orderByNode as OrderByPropertyNode;
        OrderByOpenPropertyNode openPropertyNode = orderByNode as OrderByOpenPropertyNode;
        OrderByCountNode orderByCountNode = orderByNode as OrderByCountNode;
        if (orderByPropertyNode != null)
        {
          var data = new
          {
            Property = orderByPropertyNode.Property,
            PropertyPath = orderByPropertyNode.PropertyPath
          };
          OrderByDirection direction = orderByPropertyNode.Direction;
          if (objectSet.Contains((object) data))
            throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.OrderByDuplicateProperty, (object) data.PropertyPath));
          objectSet.Add((object) data);
          core = orderByPropertyNode.OrderByClause == null ? ExpressionHelpers.OrderByProperty(core, this.Context.Model, data.Property, direction, this.Context.ElementClrType, alreadyOrdered) : this.AddOrderByQueryForProperty(query, querySettings, orderByPropertyNode.OrderByClause, core, direction, alreadyOrdered);
          alreadyOrdered = true;
        }
        else if (openPropertyNode != null)
        {
          if (stringSet.Contains(openPropertyNode.PropertyName))
            throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.OrderByDuplicateProperty, (object) openPropertyNode.PropertyPath));
          stringSet.Add(openPropertyNode.PropertyName);
          core = this.AddOrderByQueryForProperty(query, querySettings, openPropertyNode.OrderByClause, core, openPropertyNode.Direction, alreadyOrdered);
          alreadyOrdered = true;
        }
        else if (orderByCountNode != null)
        {
          core = this.AddOrderByQueryForProperty(query, querySettings, orderByCountNode.OrderByClause, core, orderByCountNode.Direction, alreadyOrdered);
          alreadyOrdered = true;
        }
        else
        {
          if (flag)
            throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.OrderByDuplicateIt));
          core = ExpressionHelpers.OrderByIt(core, orderByNode.Direction, this.Context.ElementClrType, alreadyOrdered);
          alreadyOrdered = true;
          flag = true;
        }
      }
      return core as IOrderedQueryable;
    }

    private IQueryable AddOrderByQueryForProperty(
      IQueryable query,
      ODataQuerySettings querySettings,
      OrderByClause orderbyClause,
      IQueryable querySoFar,
      OrderByDirection direction,
      bool alreadyOrdered)
    {
      ODataQuerySettings querySettings1 = this.Context.UpdateQuerySettings(querySettings, query);
      LambdaExpression orderByLambda = Microsoft.AspNet.OData.Query.Expressions.FilterBinder.Bind(query, orderbyClause, this.Context.ElementClrType, this.Context, querySettings1);
      querySoFar = ExpressionHelpers.OrderBy(querySoFar, orderByLambda, direction, this.Context.ElementClrType, alreadyOrdered);
      return querySoFar;
    }

    private OrderByClause TranslateParameterAlias(OrderByClause orderBy)
    {
      if (orderBy == null)
        return (OrderByClause) null;
      if (!(orderBy.Expression.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) new ParameterAliasNodeTranslator(this._queryOptionParser.ParameterAliasNodes)) is SingleValueNode singleValueNode))
        singleValueNode = (SingleValueNode) new ConstantNode((object) null, "null");
      SingleValueNode expression = singleValueNode;
      return new OrderByClause(this.TranslateParameterAlias(orderBy.ThenBy), expression, orderBy.Direction, orderBy.RangeVariable);
    }
  }
}
