// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.ETag
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Query.Expressions;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.AspNet.OData.Formatter
{
  public class ETag : DynamicObject
  {
    private IDictionary<string, object> _concurrencyProperties = (IDictionary<string, object>) new Dictionary<string, object>();

    public ETag() => this.IsWellFormed = true;

    public object this[string key]
    {
      get
      {
        if (!this.IsWellFormed)
          throw Error.InvalidOperation(SRResources.ETagNotWellFormed);
        return this.ConcurrencyProperties[key];
      }
      set => this.ConcurrencyProperties[key] = value;
    }

    public bool IsWellFormed { get; set; }

    public Type EntityType { get; set; }

    public bool IsAny { get; set; }

    public bool IsIfNoneMatch { get; set; }

    internal IDictionary<string, object> ConcurrencyProperties
    {
      get => this._concurrencyProperties;
      set => this._concurrencyProperties = value;
    }

    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
      if (binder == null)
        throw Error.ArgumentNull(nameof (binder));
      if (!this.IsWellFormed)
        throw Error.InvalidOperation(SRResources.ETagNotWellFormed);
      return this.ConcurrencyProperties.TryGetValue(binder.Name, out result);
    }

    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
      if (binder == null)
        throw Error.ArgumentNull(nameof (binder));
      this.ConcurrencyProperties[binder.Name] = value;
      return true;
    }

    public virtual IQueryable ApplyTo(IQueryable query)
    {
      if (this.IsAny)
        return query;
      Type entityType = this.EntityType;
      ParameterExpression parameterExpression = Expression.Parameter(entityType);
      Expression expression = (Expression) null;
      foreach (KeyValuePair<string, object> concurrencyProperty in (IEnumerable<KeyValuePair<string, object>>) this.ConcurrencyProperties)
      {
        MemberExpression left = Expression.Property((Expression) parameterExpression, concurrencyProperty.Key);
        object obj = concurrencyProperty.Value;
        Expression right1 = obj != null ? LinqParameterContainer.Parameterize(obj.GetType(), obj) : (Expression) Expression.Constant((object) null);
        BinaryExpression right2 = Expression.Equal((Expression) left, right1);
        expression = expression == null ? (Expression) right2 : (Expression) Expression.AndAlso(expression, (Expression) right2);
      }
      if (expression == null)
        return query;
      if (this.IsIfNoneMatch)
        expression = (Expression) Expression.Not(expression);
      Expression where = (Expression) Expression.Lambda(expression, parameterExpression);
      return ExpressionHelpers.Where(query, where, entityType);
    }
  }
}
