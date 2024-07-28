// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.Expressions.LinqParameterContainer
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.AspNet.OData.Query.Expressions
{
  internal abstract class LinqParameterContainer
  {
    private static ConcurrentDictionary<Type, Func<object, LinqParameterContainer>> _ctors = new ConcurrentDictionary<Type, Func<object, LinqParameterContainer>>();

    public abstract object Property { get; }

    public static Expression Parameterize(Type type, object value) => (Expression) Expression.Property((Expression) Expression.Constant((object) LinqParameterContainer.Create(type, value)), "TypedProperty");

    private static LinqParameterContainer Create(Type type, object value) => LinqParameterContainer._ctors.GetOrAdd(type, (Func<Type, Func<object, LinqParameterContainer>>) (t =>
    {
      MethodInfo method = typeof (LinqParameterContainer).GetMethod("CreateInternal").MakeGenericMethod(t);
      UnaryExpression unaryExpression = Expression.Convert((Expression) Expression.Parameter(typeof (object)), t);
      return ((Expression<Func<object, LinqParameterContainer>>) (parameterExpression => Expression.Call(method, (Expression) unaryExpression))).Compile();
    }))(value);

    public static LinqParameterContainer CreateInternal<T>(T value) => (LinqParameterContainer) new LinqParameterContainer.TypedLinqParameterContainer<T>(value);

    internal class TypedLinqParameterContainer<T> : LinqParameterContainer
    {
      public TypedLinqParameterContainer(T value) => this.TypedProperty = value;

      public T TypedProperty { get; set; }

      public override object Property => (object) this.TypedProperty;
    }
  }
}
