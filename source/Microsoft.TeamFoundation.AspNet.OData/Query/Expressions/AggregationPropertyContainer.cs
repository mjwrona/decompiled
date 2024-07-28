// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.Expressions.AggregationPropertyContainer
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.AspNet.OData.Query.Expressions
{
  internal class AggregationPropertyContainer : PropertyContainer.NamedProperty<object>
  {
    public GroupByWrapper NestedValue
    {
      get => (GroupByWrapper) this.Value;
      set => this.Value = (object) value;
    }

    public AggregationPropertyContainer Next { get; set; }

    public override void ToDictionaryCore(
      Dictionary<string, object> dictionary,
      IPropertyMapper propertyMapper,
      bool includeAutoSelected)
    {
      base.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
      if (this.Next == null)
        return;
      this.Next.ToDictionaryCore(dictionary, propertyMapper, includeAutoSelected);
    }

    public override object GetValue() => this.Value == DBNull.Value ? (object) null : base.GetValue();

    public static Expression CreateNextNamedPropertyContainer(
      IList<NamedPropertyExpression> properties)
    {
      Expression next = (Expression) null;
      foreach (NamedPropertyExpression property in (IEnumerable<NamedPropertyExpression>) properties)
        next = AggregationPropertyContainer.CreateNextNamedPropertyCreationExpression(property, next);
      return next;
    }

    private static Expression CreateNextNamedPropertyCreationExpression(
      NamedPropertyExpression property,
      Expression next)
    {
      Type type = next == null ? (!(property.Value.Type == typeof (GroupByWrapper)) ? typeof (AggregationPropertyContainer.LastInChain) : typeof (AggregationPropertyContainer.NestedPropertyLastInChain)) : (!(property.Value.Type == typeof (GroupByWrapper)) ? typeof (AggregationPropertyContainer) : typeof (AggregationPropertyContainer.NestedProperty));
      List<MemberBinding> bindings = new List<MemberBinding>();
      bindings.Add((MemberBinding) Expression.Bind((MemberInfo) type.GetProperty("Name"), property.Name));
      if (property.Value.Type == typeof (GroupByWrapper))
        bindings.Add((MemberBinding) Expression.Bind((MemberInfo) type.GetProperty("NestedValue"), property.Value));
      else
        bindings.Add((MemberBinding) Expression.Bind((MemberInfo) type.GetProperty("Value"), property.Value));
      if (next != null)
        bindings.Add((MemberBinding) Expression.Bind((MemberInfo) type.GetProperty("Next"), next));
      if (property.NullCheck != null)
        bindings.Add((MemberBinding) Expression.Bind((MemberInfo) type.GetProperty("IsNull"), property.NullCheck));
      return (Expression) Expression.MemberInit(Expression.New(type), (IEnumerable<MemberBinding>) bindings);
    }

    private class LastInChain : AggregationPropertyContainer
    {
    }

    private class NestedPropertyLastInChain : AggregationPropertyContainer
    {
    }

    private class NestedProperty : AggregationPropertyContainer
    {
    }
  }
}
