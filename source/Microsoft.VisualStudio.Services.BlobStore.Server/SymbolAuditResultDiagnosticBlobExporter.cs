// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.SymbolAuditResultDiagnosticBlobExporter
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Content.Server.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class SymbolAuditResultDiagnosticBlobExporter : 
    IReferenceAuditResultExporter,
    IVssFrameworkService
  {
    private IDiagnosticBlobUploader blobUploader;

    public Task ExportResultsAsync(
      IEnumerable<ReferenceAuditEntry> entries,
      CancellationToken cancellationToken,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer)
    {
      ReferenceAuditEntry referenceAuditEntry = entries != null ? entries.FirstOrDefault<ReferenceAuditEntry>() : throw new ArgumentNullException(nameof (entries));
      string blobName = "symbolaudit_hostId_" + (referenceAuditEntry != null ? referenceAuditEntry.HostId.ConvertToAzureCompatibleString() : (string) null) + "_" + string.Format("{0:yyyy-MM-dd_HH-mm-ss}.txt", (object) DateTime.UtcNow);
      if (!this.blobUploader.TryUploadBlobToDiagnosticStorageAccount((IVssRequestContext) null, tracer, entries.Where<ReferenceAuditEntry>((Func<ReferenceAuditEntry, bool>) (result => string.Equals(result.Reference.Scope, "symbol", StringComparison.OrdinalIgnoreCase))).Select<ReferenceAuditEntry, string>((Func<ReferenceAuditEntry, string>) (result => this.ExtractSymbolRequestId(result, tracer))).Where<string>((Func<string, bool>) (result => !string.IsNullOrWhiteSpace(result))).Distinct<string>(), blobName))
        throw new Exception("TryUploadBlobToDiagnosticStorageAccount failed.");
      return Task.CompletedTask;
    }

    private string ExtractSymbolRequestId(ReferenceAuditEntry auditEntry, Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer)
    {
      int length = auditEntry.Reference.Name.IndexOf('/');
      if (length != -1)
        return auditEntry.Reference.Name.Substring(0, length);
      tracer.TraceError(string.Format("Symbol reference in the blob is malformed. BlobId: {0}", (object) auditEntry.BlobIdentifier));
      return (string) null;
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.blobUploader = systemRequestContext.GetService<IDiagnosticBlobUploader>();
  }
}
