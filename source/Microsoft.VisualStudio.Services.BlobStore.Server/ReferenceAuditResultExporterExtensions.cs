// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.ReferenceAuditResultExporterExtensions
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public static class ReferenceAuditResultExporterExtensions
  {
    public static async Task<bool> TryExportResultsAsync(
      this IReferenceAuditResultExporter exporter,
      IEnumerable<ReferenceAuditEntry> entries,
      CancellationToken cancellationToken,
      Tracer tracer)
    {
      try
      {
        await exporter.ExportResultsAsync(entries, cancellationToken, tracer);
        return true;
      }
      catch (Exception ex)
      {
        tracer.TraceException(ex);
        return false;
      }
    }
  }
}
