// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Utils
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.OData
{
  internal static class Utils
  {
    internal static bool TryDispose(object o)
    {
      if (!(o is IDisposable disposable))
        return false;
      disposable.Dispose();
      return true;
    }

    internal static Task FlushAsync(this Stream stream) => Task.Factory.StartNew(new Action(stream.Flush));

    internal static KeyValuePair<int, T>[] StableSort<T>(this T[] array, Comparison<T> comparison)
    {
      ExceptionUtils.CheckArgumentNotNull<T[]>(array, nameof (array));
      ExceptionUtils.CheckArgumentNotNull<Comparison<T>>(comparison, nameof (comparison));
      KeyValuePair<int, T>[] array1 = new KeyValuePair<int, T>[array.Length];
      for (int key = 0; key < array.Length; ++key)
        array1[key] = new KeyValuePair<int, T>(key, array[key]);
      Array.Sort<KeyValuePair<int, T>>(array1, (IComparer<KeyValuePair<int, T>>) new Utils.StableComparer<T>(comparison));
      return array1;
    }

    private sealed class StableComparer<T> : IComparer<KeyValuePair<int, T>>
    {
      private readonly Comparison<T> innerComparer;

      public StableComparer(Comparison<T> innerComparer) => this.innerComparer = innerComparer;

      public int Compare(KeyValuePair<int, T> x, KeyValuePair<int, T> y)
      {
        int num = this.innerComparer(x.Value, y.Value);
        if (num == 0)
          num = x.Key - y.Key;
        return num;
      }
    }
  }
}
