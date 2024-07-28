// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.BatchSplitter
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Feed.Common
{
  public class BatchSplitter : IBatchSplitter
  {
    private readonly int batchSize;

    public BatchSplitter(int batchSize)
    {
      ArgumentUtility.CheckForNonPositiveInt(batchSize, nameof (batchSize));
      this.batchSize = batchSize;
    }

    public IEnumerable<IEnumerable<T>> Split<T>(IEnumerable<T> items)
    {
      List<T> objList = new List<T>(this.batchSize);
      foreach (T obj in items)
      {
        objList.Add(obj);
        if (objList.Count == this.batchSize)
        {
          yield return (IEnumerable<T>) objList;
          objList = new List<T>(this.batchSize);
        }
      }
      if (objList.Count > 0)
        yield return (IEnumerable<T>) objList;
    }
  }
}
