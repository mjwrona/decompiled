// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.ExpressionHelperMethods
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.AspNet.OData
{
  internal class ExpressionHelperMethods
  {
    private static MethodInfo _enumerableWhereMethod = ExpressionHelperMethods.GenericMethodOf<IEnumerable<int>>((Expression<Func<object, IEnumerable<int>>>) (_ => default (IEnumerable<int>).Where<int>(default (Func<int, bool>))));
    private static MethodInfo _queryableToListMethod = ExpressionHelperMethods.GenericMethodOf<List<int>>((Expression<Func<object, List<int>>>) (_ => default (IEnumerable<int>).ToList<int>()));
    private static MethodInfo _orderByMethod = ExpressionHelperMethods.GenericMethodOf<IOrderedQueryable<int>>((Expression<Func<object, IOrderedQueryable<int>>>) (_ => default (IQueryable<int>).OrderBy<int, int>(default (Expression<Func<int, int>>))));
    private static MethodInfo _enumerableOrderByMethod = ExpressionHelperMethods.GenericMethodOf<IOrderedEnumerable<int>>((Expression<Func<object, IOrderedEnumerable<int>>>) (_ => default (IEnumerable<int>).OrderBy<int, int>(default (Func<int, int>))));
    private static MethodInfo _orderByDescendingMethod = ExpressionHelperMethods.GenericMethodOf<IOrderedQueryable<int>>((Expression<Func<object, IOrderedQueryable<int>>>) (_ => default (IQueryable<int>).OrderByDescending<int, int>(default (Expression<Func<int, int>>))));
    private static MethodInfo _enumerableOrderByDescendingMethod = ExpressionHelperMethods.GenericMethodOf<IOrderedEnumerable<int>>((Expression<Func<object, IOrderedEnumerable<int>>>) (_ => default (IEnumerable<int>).OrderByDescending<int, int>(default (Func<int, int>))));
    private static MethodInfo _thenByMethod = ExpressionHelperMethods.GenericMethodOf<IOrderedQueryable<int>>((Expression<Func<object, IOrderedQueryable<int>>>) (_ => default (IOrderedQueryable<int>).ThenBy<int, int>(default (Expression<Func<int, int>>))));
    private static MethodInfo _enumerableThenByMethod = ExpressionHelperMethods.GenericMethodOf<IOrderedEnumerable<int>>((Expression<Func<object, IOrderedEnumerable<int>>>) (_ => default (IOrderedEnumerable<int>).ThenBy<int, int>(default (Func<int, int>))));
    private static MethodInfo _thenByDescendingMethod = ExpressionHelperMethods.GenericMethodOf<IOrderedQueryable<int>>((Expression<Func<object, IOrderedQueryable<int>>>) (_ => default (IOrderedQueryable<int>).ThenByDescending<int, int>(default (Expression<Func<int, int>>))));
    private static MethodInfo _enumerableThenByDescendingMethod = ExpressionHelperMethods.GenericMethodOf<IOrderedEnumerable<int>>((Expression<Func<object, IOrderedEnumerable<int>>>) (_ => default (IOrderedEnumerable<int>).ThenByDescending<int, int>(default (Func<int, int>))));
    private static MethodInfo _countMethod = ExpressionHelperMethods.GenericMethodOf<long>((Expression<Func<object, long>>) (_ => default (IQueryable<int>).LongCount<int>()));
    private static MethodInfo _enumerableGroupByMethod = ExpressionHelperMethods.GenericMethodOf<IEnumerable<IGrouping<int, int>>>((Expression<Func<object, IEnumerable<IGrouping<int, int>>>>) (_ => default (IEnumerable<int>).GroupBy<int, int>(default (Func<int, int>))));
    private static MethodInfo _groupByMethod = ExpressionHelperMethods.GenericMethodOf<IQueryable<IGrouping<int, int>>>((Expression<Func<object, IQueryable<IGrouping<int, int>>>>) (_ => default (IQueryable<int>).GroupBy<int, int>(default (Expression<Func<int, int>>))));
    private static MethodInfo _aggregateMethod = ExpressionHelperMethods.GenericMethodOf<int>((Expression<Func<object, int>>) (_ => default (IQueryable<int>).Aggregate<int, int>(0, default (Expression<Func<int, int, int>>))));
    private static MethodInfo _skipMethod = ExpressionHelperMethods.GenericMethodOf<IQueryable<int>>((Expression<Func<object, IQueryable<int>>>) (_ => default (IQueryable<int>).Skip<int>(0)));
    private static MethodInfo _enumerableSkipMethod = ExpressionHelperMethods.GenericMethodOf<IEnumerable<int>>((Expression<Func<object, IEnumerable<int>>>) (_ => default (IEnumerable<int>).Skip<int>(0)));
    private static MethodInfo _whereMethod = ExpressionHelperMethods.GenericMethodOf<IQueryable<int>>((Expression<Func<object, IQueryable<int>>>) (_ => default (IQueryable<int>).Where<int>(default (Expression<Func<int, bool>>))));
    private static MethodInfo _queryableContainsMethod = ExpressionHelperMethods.GenericMethodOf<bool>((Expression<Func<object, bool>>) (_ => default (IQueryable<int>).Contains<int>(0)));
    private static MethodInfo _enumerableContainsMethod = ExpressionHelperMethods.GenericMethodOf<bool>((Expression<Func<object, bool>>) (_ => default (IEnumerable<int>).Contains<int>(0)));
    private static MethodInfo _queryableEmptyAnyMethod = ExpressionHelperMethods.GenericMethodOf<bool>((Expression<Func<object, bool>>) (_ => default (IQueryable<int>).Any<int>()));
    private static MethodInfo _queryableNonEmptyAnyMethod = ExpressionHelperMethods.GenericMethodOf<bool>((Expression<Func<object, bool>>) (_ => default (IQueryable<int>).Any<int>(default (Expression<Func<int, bool>>))));
    private static MethodInfo _queryableAllMethod = ExpressionHelperMethods.GenericMethodOf<bool>((Expression<Func<object, bool>>) (_ => default (IQueryable<int>).All<int>(default (Expression<Func<int, bool>>))));
    private static MethodInfo _enumerableEmptyAnyMethod = ExpressionHelperMethods.GenericMethodOf<bool>((Expression<Func<object, bool>>) (_ => default (IEnumerable<int>).Any<int>()));
    private static MethodInfo _enumerableNonEmptyAnyMethod = ExpressionHelperMethods.GenericMethodOf<bool>((Expression<Func<object, bool>>) (_ => default (IEnumerable<int>).Any<int>(default (Func<int, bool>))));
    private static MethodInfo _enumerableAllMethod = ExpressionHelperMethods.GenericMethodOf<bool>((Expression<Func<object, bool>>) (_ => default (IEnumerable<int>).All<int>(default (Func<int, bool>))));
    private static MethodInfo _enumerableOfTypeMethod = ExpressionHelperMethods.GenericMethodOf<IEnumerable<int>>((Expression<Func<object, IEnumerable<int>>>) (_ => default (IEnumerable).OfType<int>()));
    private static MethodInfo _queryableOfTypeMethod = ExpressionHelperMethods.GenericMethodOf<IQueryable<int>>((Expression<Func<object, IQueryable<int>>>) (_ => Queryable.OfType<int>(default (IQueryable))));
    private static MethodInfo _enumerableSelectManyMethod = ExpressionHelperMethods.GenericMethodOf<IEnumerable<int>>((Expression<Func<object, IEnumerable<int>>>) (_ => default (IEnumerable<int>).SelectMany<int, int>(default (Func<int, IEnumerable<int>>))));
    private static MethodInfo _queryableSelectManyMethod = ExpressionHelperMethods.GenericMethodOf<IQueryable<int>>((Expression<Func<object, IQueryable<int>>>) (_ => default (IQueryable<int>).SelectMany<int, int>(default (Expression<Func<int, IEnumerable<int>>>))));
    private static MethodInfo _enumerableSelectMethod = ExpressionHelperMethods.GenericMethodOf<IEnumerable<int>>((Expression<Func<object, IEnumerable<int>>>) (_ => default (IEnumerable<int>).Select<int, int>((Func<int, int>) (i => i))));
    private static MethodInfo _queryableSelectMethod = ExpressionHelperMethods.GenericMethodOf<IQueryable<int>>((Expression<Func<object, IQueryable<int>>>) (_ => default (IQueryable<int>).Select<int, int>((Expression<Func<int, int>>) (i => i))));
    private static MethodInfo _queryableTakeMethod = ExpressionHelperMethods.GenericMethodOf<IQueryable<int>>((Expression<Func<object, IQueryable<int>>>) (_ => default (IQueryable<int>).Take<int>(0)));
    private static MethodInfo _enumerableTakeMethod = ExpressionHelperMethods.GenericMethodOf<IEnumerable<int>>((Expression<Func<object, IEnumerable<int>>>) (_ => default (IEnumerable<int>).Take<int>(0)));
    private static MethodInfo _queryableAsQueryableMethod = ExpressionHelperMethods.GenericMethodOf<IQueryable<int>>((Expression<Func<object, IQueryable<int>>>) (_ => default (IEnumerable<int>).AsQueryable<int>()));
    private static MethodInfo _toQueryableMethod = ExpressionHelperMethods.GenericMethodOf<IQueryable>((Expression<Func<object, IQueryable>>) (_ => ExpressionHelperMethods.ToQueryable<int>(0)));
    private static Dictionary<Type, MethodInfo> _queryableSumMethods = ExpressionHelperMethods.GetQueryableAggregationMethods("Sum");
    private static Dictionary<Type, MethodInfo> _enumerableSumMethods = ExpressionHelperMethods.GetEnumerableAggregationMethods("Sum");
    private static MethodInfo _enumerableMinMethod = ExpressionHelperMethods.GenericMethodOf<int>((Expression<Func<object, int>>) (_ => default (IEnumerable<int>).Min<int, int>(default (Func<int, int>))));
    private static MethodInfo _enumerableMaxMethod = ExpressionHelperMethods.GenericMethodOf<int>((Expression<Func<object, int>>) (_ => default (IEnumerable<int>).Max<int, int>(default (Func<int, int>))));
    private static MethodInfo _enumerableDistinctMethod = ExpressionHelperMethods.GenericMethodOf<IEnumerable<int>>((Expression<Func<object, IEnumerable<int>>>) (_ => default (IEnumerable<int>).Distinct<int>()));
    private static MethodInfo _queryableMinMethod = ExpressionHelperMethods.GenericMethodOf<int>((Expression<Func<object, int>>) (_ => default (IQueryable<int>).Min<int, int>(default (Expression<Func<int, int>>))));
    private static MethodInfo _queryableMaxMethod = ExpressionHelperMethods.GenericMethodOf<int>((Expression<Func<object, int>>) (_ => default (IQueryable<int>).Max<int, int>(default (Expression<Func<int, int>>))));
    private static MethodInfo _queryableDistinctMethod = ExpressionHelperMethods.GenericMethodOf<IQueryable<int>>((Expression<Func<object, IQueryable<int>>>) (_ => default (IQueryable<int>).Distinct<int>()));
    private static MethodInfo _createQueryGenericMethod = ExpressionHelperMethods.GetCreateQueryGenericMethod();
    private static Dictionary<Type, MethodInfo> _enumerableAverageMethods;
    private static Dictionary<Type, MethodInfo> _queryableAverageMethods;
    private static MethodInfo _enumerableCountMethod;
    private static MethodInfo _safeConvertToDecimalMethod;

    public static MethodInfo EnumerableWhereGeneric => ExpressionHelperMethods._enumerableWhereMethod;

    public static MethodInfo QueryableToList => ExpressionHelperMethods._queryableToListMethod;

    public static MethodInfo QueryableOrderByGeneric => ExpressionHelperMethods._orderByMethod;

    public static MethodInfo EnumerableOrderByGeneric => ExpressionHelperMethods._enumerableOrderByMethod;

    public static MethodInfo QueryableOrderByDescendingGeneric => ExpressionHelperMethods._orderByDescendingMethod;

    public static MethodInfo EnumerableOrderByDescendingGeneric => ExpressionHelperMethods._enumerableOrderByDescendingMethod;

    public static MethodInfo QueryableThenByGeneric => ExpressionHelperMethods._thenByMethod;

    public static MethodInfo EnumerableThenByGeneric => ExpressionHelperMethods._enumerableThenByMethod;

    public static MethodInfo QueryableThenByDescendingGeneric => ExpressionHelperMethods._thenByDescendingMethod;

    public static MethodInfo EnumerableThenByDescendingGeneric => ExpressionHelperMethods._enumerableThenByDescendingMethod;

    public static MethodInfo QueryableCountGeneric => ExpressionHelperMethods._countMethod;

    public static Dictionary<Type, MethodInfo> QueryableSumGenerics => ExpressionHelperMethods._queryableSumMethods;

    public static Dictionary<Type, MethodInfo> EnumerableSumGenerics => ExpressionHelperMethods._enumerableSumMethods;

    public static MethodInfo QueryableMin => ExpressionHelperMethods._queryableMinMethod;

    public static MethodInfo EnumerableMin => ExpressionHelperMethods._enumerableMinMethod;

    public static MethodInfo QueryableMax => ExpressionHelperMethods._queryableMaxMethod;

    public static MethodInfo EnumerableMax => ExpressionHelperMethods._enumerableMaxMethod;

    public static Dictionary<Type, MethodInfo> QueryableAverageGenerics => ExpressionHelperMethods._queryableAverageMethods;

    public static Dictionary<Type, MethodInfo> EnumerableAverageGenerics => ExpressionHelperMethods._enumerableAverageMethods;

    public static MethodInfo QueryableDistinct => ExpressionHelperMethods._queryableDistinctMethod;

    public static MethodInfo EnumerableDistinct => ExpressionHelperMethods._enumerableDistinctMethod;

    public static MethodInfo QueryableGroupByGeneric => ExpressionHelperMethods._groupByMethod;

    public static MethodInfo EnumerableGroupByGeneric => ExpressionHelperMethods._enumerableGroupByMethod;

    public static MethodInfo QueryableAggregateGeneric => ExpressionHelperMethods._aggregateMethod;

    public static MethodInfo QueryableTakeGeneric => ExpressionHelperMethods._queryableTakeMethod;

    public static MethodInfo EnumerableTakeGeneric => ExpressionHelperMethods._enumerableTakeMethod;

    public static MethodInfo QueryableSkipGeneric => ExpressionHelperMethods._skipMethod;

    public static MethodInfo EnumerableSkipGeneric => ExpressionHelperMethods._enumerableSkipMethod;

    public static MethodInfo QueryableWhereGeneric => ExpressionHelperMethods._whereMethod;

    public static MethodInfo QueryableContainsGeneric => ExpressionHelperMethods._queryableContainsMethod;

    public static MethodInfo EnumerableContainsGeneric => ExpressionHelperMethods._enumerableContainsMethod;

    public static MethodInfo QueryableSelectGeneric => ExpressionHelperMethods._queryableSelectMethod;

    public static MethodInfo EnumerableSelectGeneric => ExpressionHelperMethods._enumerableSelectMethod;

    public static MethodInfo QueryableSelectManyGeneric => ExpressionHelperMethods._queryableSelectManyMethod;

    public static MethodInfo EnumerableSelectManyGeneric => ExpressionHelperMethods._enumerableSelectManyMethod;

    public static MethodInfo QueryableEmptyAnyGeneric => ExpressionHelperMethods._queryableEmptyAnyMethod;

    public static MethodInfo QueryableNonEmptyAnyGeneric => ExpressionHelperMethods._queryableNonEmptyAnyMethod;

    public static MethodInfo QueryableAllGeneric => ExpressionHelperMethods._queryableAllMethod;

    public static MethodInfo EnumerableEmptyAnyGeneric => ExpressionHelperMethods._enumerableEmptyAnyMethod;

    public static MethodInfo EnumerableNonEmptyAnyGeneric => ExpressionHelperMethods._enumerableNonEmptyAnyMethod;

    public static MethodInfo EnumerableAllGeneric => ExpressionHelperMethods._enumerableAllMethod;

    public static MethodInfo EnumerableOfType => ExpressionHelperMethods._enumerableOfTypeMethod;

    public static MethodInfo QueryableOfType => ExpressionHelperMethods._queryableOfTypeMethod;

    public static MethodInfo QueryableAsQueryable => ExpressionHelperMethods._queryableAsQueryableMethod;

    public static MethodInfo EntityAsQueryable => ExpressionHelperMethods._toQueryableMethod;

    public static IQueryable ToQueryable<T>(T value) => (IQueryable) new List<T>()
    {
      value
    }.AsQueryable<T>();

    public static MethodInfo EnumerableCountGeneric => ExpressionHelperMethods._enumerableCountMethod;

    public static MethodInfo ConvertToDecimal => ExpressionHelperMethods._safeConvertToDecimalMethod;

    public static MethodInfo CreateQueryGeneric => ExpressionHelperMethods._createQueryGenericMethod;

    public static Decimal? SafeConvertToDecimal(object value)
    {
      if (value == null || value == DBNull.Value)
        return new Decimal?();
      Type type1 = value.GetType();
      Type type2 = Nullable.GetUnderlyingType(type1);
      if ((object) type2 == null)
        type2 = type1;
      Type type3 = type2;
      return type3 == typeof (short) || type3 == typeof (int) || type3 == typeof (long) || type3 == typeof (Decimal) || type3 == typeof (double) || type3 == typeof (float) ? (Decimal?) Convert.ChangeType(value, typeof (Decimal), (IFormatProvider) CultureInfo.InvariantCulture) : new Decimal?();
    }

    private static MethodInfo GenericMethodOf<TReturn>(Expression<Func<object, TReturn>> expression) => ExpressionHelperMethods.GenericMethodOf((Expression) expression);

    private static MethodInfo GenericMethodOf(Expression expression) => ((expression as LambdaExpression).Body as MethodCallExpression).Method.GetGenericMethodDefinition();

    private static Dictionary<Type, MethodInfo> GetQueryableAggregationMethods(string methodName) => ((IEnumerable<MethodInfo>) typeof (Queryable).GetMethods()).Where<MethodInfo>((Func<MethodInfo, bool>) (m => m.Name == methodName)).Where<MethodInfo>((Func<MethodInfo, bool>) (m => ((IEnumerable<ParameterInfo>) m.GetParameters()).Count<ParameterInfo>() == 2)).ToDictionary<MethodInfo, Type>((Func<MethodInfo, Type>) (m => m.ReturnType));

    private static Dictionary<Type, MethodInfo> GetEnumerableAggregationMethods(string methodName) => ((IEnumerable<MethodInfo>) typeof (Enumerable).GetMethods()).Where<MethodInfo>((Func<MethodInfo, bool>) (m => m.Name == methodName)).Where<MethodInfo>((Func<MethodInfo, bool>) (m => ((IEnumerable<ParameterInfo>) m.GetParameters()).Count<ParameterInfo>() == 2)).ToDictionary<MethodInfo, Type>((Func<MethodInfo, Type>) (m => m.ReturnType));

    private static MethodInfo GetCreateQueryGenericMethod() => typeof (IQueryProvider).GetTypeInfo().GetDeclaredMethods("CreateQuery").Where<MethodInfo>((Func<MethodInfo, bool>) (m => m.IsGenericMethod)).FirstOrDefault<MethodInfo>();

    static ExpressionHelperMethods()
    {
      Dictionary<Type, MethodInfo> dictionary1 = new Dictionary<Type, MethodInfo>();
      dictionary1.Add(typeof (int), ExpressionHelperMethods.GenericMethodOf<double>((Expression<Func<object, double>>) (_ => default (IEnumerable<string>).Average<string>(default (Func<string, int>)))));
      dictionary1.Add(typeof (int?), ExpressionHelperMethods.GenericMethodOf<double?>((Expression<Func<object, double?>>) (_ => default (IEnumerable<string>).Average<string>(default (Func<string, int?>)))));
      dictionary1.Add(typeof (long), ExpressionHelperMethods.GenericMethodOf<double>((Expression<Func<object, double>>) (_ => default (IEnumerable<string>).Average<string>(default (Func<string, long>)))));
      dictionary1.Add(typeof (long?), ExpressionHelperMethods.GenericMethodOf<double?>((Expression<Func<object, double?>>) (_ => default (IEnumerable<string>).Average<string>(default (Func<string, long?>)))));
      dictionary1.Add(typeof (float), ExpressionHelperMethods.GenericMethodOf<float>((Expression<Func<object, float>>) (_ => default (IEnumerable<string>).Average<string>(default (Func<string, float>)))));
      dictionary1.Add(typeof (float?), ExpressionHelperMethods.GenericMethodOf<float?>((Expression<Func<object, float?>>) (_ => default (IEnumerable<string>).Average<string>(default (Func<string, float?>)))));
      dictionary1.Add(typeof (Decimal), ExpressionHelperMethods.GenericMethodOf<Decimal>((Expression<Func<object, Decimal>>) (_ => default (IEnumerable<string>).Average<string>(default (Func<string, Decimal>)))));
      dictionary1.Add(typeof (Decimal?), ExpressionHelperMethods.GenericMethodOf<Decimal?>((Expression<Func<object, Decimal?>>) (_ => default (IEnumerable<string>).Average<string>(default (Func<string, Decimal?>)))));
      dictionary1.Add(typeof (double), ExpressionHelperMethods.GenericMethodOf<double>((Expression<Func<object, double>>) (_ => default (IEnumerable<string>).Average<string>(default (Func<string, double>)))));
      dictionary1.Add(typeof (double?), ExpressionHelperMethods.GenericMethodOf<double?>((Expression<Func<object, double?>>) (_ => default (IEnumerable<string>).Average<string>(default (Func<string, double?>)))));
      ExpressionHelperMethods._enumerableAverageMethods = dictionary1;
      Dictionary<Type, MethodInfo> dictionary2 = new Dictionary<Type, MethodInfo>();
      dictionary2.Add(typeof (int), ExpressionHelperMethods.GenericMethodOf<double>((Expression<Func<object, double>>) (_ => default (IQueryable<string>).Average<string>(default (Expression<Func<string, int>>)))));
      dictionary2.Add(typeof (int?), ExpressionHelperMethods.GenericMethodOf<double?>((Expression<Func<object, double?>>) (_ => default (IQueryable<string>).Average<string>(default (Expression<Func<string, int?>>)))));
      dictionary2.Add(typeof (long), ExpressionHelperMethods.GenericMethodOf<double>((Expression<Func<object, double>>) (_ => default (IQueryable<string>).Average<string>(default (Expression<Func<string, long>>)))));
      dictionary2.Add(typeof (long?), ExpressionHelperMethods.GenericMethodOf<double?>((Expression<Func<object, double?>>) (_ => default (IQueryable<string>).Average<string>(default (Expression<Func<string, long?>>)))));
      dictionary2.Add(typeof (float), ExpressionHelperMethods.GenericMethodOf<float>((Expression<Func<object, float>>) (_ => default (IQueryable<string>).Average<string>(default (Expression<Func<string, float>>)))));
      dictionary2.Add(typeof (float?), ExpressionHelperMethods.GenericMethodOf<float?>((Expression<Func<object, float?>>) (_ => default (IQueryable<string>).Average<string>(default (Expression<Func<string, float?>>)))));
      dictionary2.Add(typeof (Decimal), ExpressionHelperMethods.GenericMethodOf<Decimal>((Expression<Func<object, Decimal>>) (_ => default (IQueryable<string>).Average<string>(default (Expression<Func<string, Decimal>>)))));
      dictionary2.Add(typeof (Decimal?), ExpressionHelperMethods.GenericMethodOf<Decimal?>((Expression<Func<object, Decimal?>>) (_ => default (IQueryable<string>).Average<string>(default (Expression<Func<string, Decimal?>>)))));
      dictionary2.Add(typeof (double), ExpressionHelperMethods.GenericMethodOf<double>((Expression<Func<object, double>>) (_ => default (IQueryable<string>).Average<string>(default (Expression<Func<string, double>>)))));
      dictionary2.Add(typeof (double?), ExpressionHelperMethods.GenericMethodOf<double?>((Expression<Func<object, double?>>) (_ => default (IQueryable<string>).Average<string>(default (Expression<Func<string, double?>>)))));
      ExpressionHelperMethods._queryableAverageMethods = dictionary2;
      ExpressionHelperMethods._enumerableCountMethod = ExpressionHelperMethods.GenericMethodOf<long>((Expression<Func<object, long>>) (_ => default (IEnumerable<int>).LongCount<int>()));
      ExpressionHelperMethods._safeConvertToDecimalMethod = typeof (ExpressionHelperMethods).GetMethod("SafeConvertToDecimal");
    }
  }
}
