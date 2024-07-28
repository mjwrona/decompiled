// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.Charts.EnumerableExtensions
// Assembly: Microsoft.Azure.Boards.Charts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EABADF19-3537-403E-8E3C-4185CE6D1F3B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.Charts.dll

using Microsoft.Azure.Boards.Charts.Wiql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Azure.Boards.Charts
{
  public static class EnumerableExtensions
  {
    public static double Sum<T>(this IEnumerable<T> source, Func<T, T, double> func)
    {
      double num = 0.0;
      T obj1 = default (T);
      foreach (T obj2 in source)
      {
        if ((object) obj1 != null)
          num += func(obj1, obj2);
        obj1 = obj2;
      }
      return num;
    }

    public static Collection<Condition> ToConditions(
      this IEnumerable<string> values,
      string field,
      SingleValueOperator conditionOperator)
    {
      Collection<Condition> conditions = new Collection<Condition>();
      foreach (string str in values)
        conditions.Add((Condition) new SingleValueCondition()
        {
          Field = field,
          Operator = conditionOperator,
          Value = (object) str
        });
      return conditions;
    }
  }
}
