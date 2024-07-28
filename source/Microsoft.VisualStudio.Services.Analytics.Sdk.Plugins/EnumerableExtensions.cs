// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Analytics.Plugins.EnumerableExtensions
// Assembly: Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3E9FDCC8-8891-4D47-89A2-C972B6459647
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.TeamFoundation.Analytics.Plugins
{
  public static class EnumerableExtensions
  {
    public static IEnumerable<EnumerableExtensions.RecordsBatch> BatchRecords<T>(
      this IEnumerable<T> source,
      int batchSize,
      CancellationToken cancellationToken)
    {
      ArgumentUtility.CheckForNull<IEnumerable<T>>(source, nameof (source));
      ArgumentUtility.CheckForOutOfRange(batchSize, nameof (batchSize), 1);
      EnumerableExtensions.RecordsBatch batch = new EnumerableExtensions.RecordsBatch()
      {
        Records = (IList) new List<T>(batchSize),
        IsLast = false
      };
      IEnumerator<T> enumerator = source.GetEnumerator();
      bool flag = enumerator.MoveNext();
      if (flag)
      {
        batch.Records.Add((object) enumerator.Current);
        batch.IsLast = !enumerator.MoveNext();
      }
      else
        batch.IsLast = true;
      if (!cancellationToken.IsCancellationRequested & flag && batch.IsLast)
        yield return batch;
      while (!cancellationToken.IsCancellationRequested && !batch.IsLast)
      {
        batch.Records.Add((object) enumerator.Current);
        batch.IsLast = !enumerator.MoveNext();
        if (batch.Records.Count == batchSize || batch.IsLast)
        {
          yield return batch;
          batch.Records = (IList) new List<T>(batchSize);
        }
      }
    }

    public class RecordsBatch
    {
      public IList Records { get; set; }

      public bool IsLast { get; set; }
    }
  }
}
