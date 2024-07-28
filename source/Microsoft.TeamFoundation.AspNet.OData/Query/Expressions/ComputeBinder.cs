// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.Expressions.ComputeBinder
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Interfaces;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Microsoft.OData.UriParser.Aggregation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.AspNet.OData.Query.Expressions
{
  internal class ComputeBinder : TransformationBinderBase
  {
    private const string GroupByContainerProperty = "GroupByContainer";
    private IEnumerable<ComputeExpression> _expressions;

    internal ComputeBinder(
      ODataQuerySettings settings,
      IWebApiAssembliesResolver assembliesResolver,
      Type elementType,
      IEdmModel model,
      IEnumerable<ComputeExpression> expressions)
      : base(settings, assembliesResolver, elementType, model)
    {
      this._expressions = expressions;
      this.ResultClrType = typeof (ComputeWrapper<>).MakeGenericType(this._elementType);
    }

    internal ComputeBinder(
      ODataQuerySettings settings,
      IWebApiAssembliesResolver assembliesResolver,
      Type elementType,
      IEdmModel model,
      ComputeTransformationNode transformation)
      : this(settings, assembliesResolver, elementType, model, transformation.Expressions)
    {
    }

    public IQueryable Bind(IQueryable query)
    {
      this.PreprocessQuery(query);
      List<MemberAssignment> bindings = new List<MemberAssignment>();
      PropertyInfo property1 = this.ResultClrType.GetProperty("Instance");
      bindings.Add(Expression.Bind((MemberInfo) property1, (Expression) this._lambdaParameter));
      List<NamedPropertyExpression> properties = new List<NamedPropertyExpression>();
      foreach (ComputeExpression expression in this._expressions)
        properties.Add(new NamedPropertyExpression((Expression) Expression.Constant((object) expression.Alias), this.CreateComputeExpression(expression)));
      PropertyInfo property2 = this.ResultClrType.GetProperty("Container");
      bindings.Add(Expression.Bind((MemberInfo) property2, AggregationPropertyContainer.CreateNextNamedPropertyContainer((IList<NamedPropertyExpression>) properties)));
      LambdaExpression expression1 = Expression.Lambda((Expression) Expression.MemberInit(Expression.New(this.ResultClrType), (IEnumerable<MemberBinding>) bindings), this._lambdaParameter);
      return ExpressionHelpers.Select(query, expression1, this._elementType);
    }

    private Expression CreateComputeExpression(ComputeExpression expression) => this.WrapConvert(this.BindAccessor(expression.Expression));
  }
}
