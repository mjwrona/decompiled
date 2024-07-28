// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.DataExport.SymbolReferenceExporter
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.Models;
using Microsoft.VisualStudio.Services.Content.Common;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.DataExport
{
  public class SymbolReferenceExporter : ReferenceExporterBase
  {
    public SymbolReferenceExporter(IVssRequestContext requestContext, JobParameters runParameters)
      : base(requestContext, runParameters)
    {
    }

    protected override string TraceMethod { get; } = "SymbolReferenceExporter.Export";

    protected override string LogFolder { get; } = "SymbolReferenceExport";

    protected override string RegistryFolder { get; } = "Symbols";

    internal override string ExportFeatureName { get; } = "BlobStore.Features.DataExport.Symbols";

    protected override void ProcessFileIdReference(IdBlobReferenceRow idBlobReferenceRow)
    {
      if (idBlobReferenceRow.ScopeId != "symbol")
        return;
      int length = idBlobReferenceRow.ReferenceId.IndexOf('/');
      if (length == -1)
        this.tracer.TraceError(string.Format("Symbol ID reference is malformed. BlobId: {0}, ReferenceId: {1}", (object) idBlobReferenceRow.BlobId, (object) idBlobReferenceRow.ReferenceId));
      else
        this.AppendLog(idBlobReferenceRow.ReferenceId.Substring(0, length));
    }

    protected override void ProcessChunkReference(DedupMetadataEntry dedupMetadataEntry)
    {
    }
  }
}
