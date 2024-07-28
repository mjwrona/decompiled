// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.DataPortResult
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  [CLSCompliant(false)]
  public class DataPortResult
  {
    public readonly int BatchResult;
    public readonly int[] ItemResults;

    public DataPortResult(int batchResult, uint batchCount, int[] itemResults)
    {
      if ((long) batchCount != (long) itemResults.Length)
        throw new ArgumentException("batchCount must match the length of itemResults");
      this.BatchResult = batchResult;
      this.ItemResults = itemResults;
    }
  }
}
