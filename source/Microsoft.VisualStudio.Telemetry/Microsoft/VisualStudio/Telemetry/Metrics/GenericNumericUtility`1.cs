// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.Metrics.GenericNumericUtility`1
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Telemetry.Metrics
{
  internal static class GenericNumericUtility<T>
  {
    public static T Add(T a, T b)
    {
      object obj1 = (object) a;
      object obj2 = (object) b;
      // ISSUE: reference to a compiler-generated field
      if (GenericNumericUtility<T>.\u003C\u003Eo__0.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GenericNumericUtility<T>.\u003C\u003Eo__0.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, T>>.Create(Binder.Convert(CSharpBinderFlags.CheckedContext | CSharpBinderFlags.ConvertExplicit, typeof (T), typeof (GenericNumericUtility<T>)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, T> target = GenericNumericUtility<T>.\u003C\u003Eo__0.\u003C\u003Ep__1.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, T>> p1 = GenericNumericUtility<T>.\u003C\u003Eo__0.\u003C\u003Ep__1;
      // ISSUE: reference to a compiler-generated field
      if (GenericNumericUtility<T>.\u003C\u003Eo__0.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GenericNumericUtility<T>.\u003C\u003Eo__0.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.CheckedContext, ExpressionType.Add, typeof (GenericNumericUtility<T>), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj3 = GenericNumericUtility<T>.\u003C\u003Eo__0.\u003C\u003Ep__0.Target((CallSite) GenericNumericUtility<T>.\u003C\u003Eo__0.\u003C\u003Ep__0, obj1, obj2);
      return target((CallSite) p1, obj3);
    }

    public static double? Average(T sum, long count) => count == 0L ? new double?() : new double?(Convert.ToDouble((object) sum) / (double) count);

    public static int Compare(T a, T b)
    {
      Comparer<T> comparer = Comparer<T>.Default;
      // ISSUE: reference to a compiler-generated field
      if (GenericNumericUtility<T>.\u003C\u003Eo__2.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GenericNumericUtility<T>.\u003C\u003Eo__2.\u003C\u003Ep__0 = CallSite<Func<CallSite, Comparer<T>, object, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, nameof (Compare), (IEnumerable<Type>) null, typeof (GenericNumericUtility<T>), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj = GenericNumericUtility<T>.\u003C\u003Eo__2.\u003C\u003Ep__0.Target((CallSite) GenericNumericUtility<T>.\u003C\u003Eo__2.\u003C\u003Ep__0, comparer, (object) a, (object) b);
      // ISSUE: reference to a compiler-generated field
      if (GenericNumericUtility<T>.\u003C\u003Eo__2.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GenericNumericUtility<T>.\u003C\u003Eo__2.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, int>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (int), typeof (GenericNumericUtility<T>)));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return GenericNumericUtility<T>.\u003C\u003Eo__2.\u003C\u003Ep__1.Target((CallSite) GenericNumericUtility<T>.\u003C\u003Eo__2.\u003C\u003Ep__1, obj);
    }
  }
}
