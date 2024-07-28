// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.BlobMetadataProviderExtensions
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public static class BlobMetadataProviderExtensions
  {
    public static async Task<IEnumerable<BlobReference>> FailedRefsAsync(
      this Task<IEnumerable<ReferenceResult>> refResultsTask)
    {
      return (await refResultsTask.ConfigureAwait(false)).Where<ReferenceResult>((Func<ReferenceResult, bool>) (r => !r.Success)).Select<ReferenceResult, BlobReference>((Func<ReferenceResult, BlobReference>) (r => r.Reference));
    }

    public static IEnumerable<BlobReference> FailedRefs(this IEnumerable<ReferenceResult> refResults) => refResults.Where<ReferenceResult>((Func<ReferenceResult, bool>) (r => !r.Success)).Select<ReferenceResult, BlobReference>((Func<ReferenceResult, BlobReference>) (r => r.Reference));
  }
}
