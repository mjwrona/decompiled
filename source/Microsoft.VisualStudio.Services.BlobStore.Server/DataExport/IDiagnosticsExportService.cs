// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.DataExport.IDiagnosticsExportService
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.DataExport
{
  [DefaultServiceImplementation(typeof (DiagnosticsExportService))]
  public interface IDiagnosticsExportService : IVssFrameworkService
  {
    Task AppendLogAsync(
      string containerName,
      string logFileName,
      ArraySegment<byte> payloadBytes,
      CancellationToken cancellationToken = default (CancellationToken));

    Task WriteFullLogAsync(
      string containerName,
      string logFileName,
      ArraySegment<byte> payloadBytes,
      CancellationToken cancellationToken = default (CancellationToken));
  }
}
