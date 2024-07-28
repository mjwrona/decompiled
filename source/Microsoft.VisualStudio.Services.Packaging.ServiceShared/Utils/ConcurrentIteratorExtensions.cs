// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils.ConcurrentIteratorExtensions
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils
{
  public static class ConcurrentIteratorExtensions
  {
    public static async Task<T?> NullableMinOrDefaultAsync<T>(
      this IConcurrentIterator<T> source,
      CancellationToken cancellationToken = default (CancellationToken))
      where T : struct, IComparable<T>
    {
      if (!await source.MoveNextAsync(cancellationToken))
        return new T?();
      T min = source.Current;
      while (true)
      {
        do
        {
          if (!await source.MoveNextAsync(cancellationToken))
            goto label_8;
        }
        while (source.Current.CompareTo(min) >= 0);
        min = source.Current;
      }
label_8:
      return new T?(min);
    }
  }
}
