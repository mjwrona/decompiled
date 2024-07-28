// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobsZipWriter
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Common.Tracing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public interface IBlobsZipWriter
  {
    Task WriteZippedContentsToStreamAsync(
      IDictionary<ManifestItem, IEnumerable<Uri>> nodeZipMappings,
      string zipRootFolderName,
      Stream outputStream,
      HttpClient httpClient,
      IAppTraceSource tracer,
      IClock clock,
      CancellationToken cancellationToken);

    Task WriteZippedContentsToStreamAsync(
      IEnumerable<ManifestItem> manifestItems,
      string zipRootFolderName,
      Stream outputStream,
      IDedupBlobMultipartHttpClient dedupBlobHttpClient,
      IClock clock,
      CancellationToken cancellationToken);
  }
}
