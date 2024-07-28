// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.TableQueryProvider
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Cosmos.Table.Queryable;
using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Microsoft.Azure.Cosmos.Table
{
  internal class TableQueryProvider : IQueryProvider
  {
    internal CloudTable Table { get; private set; }

    public TableQueryProvider(CloudTable table) => this.Table = table;

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
      CommonUtility.AssertNotNull(nameof (expression), (object) expression);
      return (IQueryable<TElement>) new TableQuery<TElement>(expression, this);
    }

    public IQueryable CreateQuery(Expression expression)
    {
      CommonUtility.AssertNotNull(nameof (expression), (object) expression);
      Type type = typeof (TableQuery<>).MakeGenericType(TypeSystem.GetElementType(expression.Type));
      object[] arguments = new object[2]
      {
        (object) expression,
        (object) this
      };
      return (IQueryable) TableQueryProvider.ConstructorInvoke(type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, (Binder) null, new Type[2]
      {
        typeof (Expression),
        typeof (TableQueryProvider)
      }, (ParameterModifier[]) null), arguments);
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public object Execute(Expression expression)
    {
      CommonUtility.AssertNotNull(nameof (expression), (object) expression);
      return ReflectionUtil.TableQueryProviderReturnSingletonMethodInfo.MakeGenericMethod(expression.Type).Invoke((object) this, new object[1]
      {
        (object) expression
      });
    }

    public TResult Execute<TResult>(Expression expression)
    {
      CommonUtility.AssertNotNull(nameof (expression), (object) expression);
      return (TResult) ReflectionUtil.TableQueryProviderReturnSingletonMethodInfo.MakeGenericMethod(typeof (TResult)).Invoke((object) this, new object[1]
      {
        (object) expression
      });
    }

    internal TElement ReturnSingleton<TElement>(Expression expression)
    {
      IQueryable<TElement> source = (IQueryable<TElement>) new TableQuery<TElement>(expression, this);
      MethodCallExpression methodCallExpression = expression as MethodCallExpression;
      SequenceMethod sequenceMethod;
      if (ReflectionUtil.TryIdentifySequenceMethod(methodCallExpression.Method, out sequenceMethod))
      {
        switch (sequenceMethod)
        {
          case SequenceMethod.First:
            return source.AsEnumerable<TElement>().First<TElement>();
          case SequenceMethod.FirstOrDefault:
            return source.AsEnumerable<TElement>().FirstOrDefault<TElement>();
          case SequenceMethod.Single:
            return source.AsEnumerable<TElement>().Single<TElement>();
          case SequenceMethod.SingleOrDefault:
            return source.AsEnumerable<TElement>().SingleOrDefault<TElement>();
        }
      }
      throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The method '{0}' is not supported.", (object) methodCallExpression.Method.Name));
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    internal static object ConstructorInvoke(ConstructorInfo constructor, object[] arguments) => !(constructor == (ConstructorInfo) null) ? constructor.Invoke(arguments) : throw new MissingMethodException();
  }
}
