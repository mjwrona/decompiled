// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.DataExport.PackagingReferenceExporter
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.Models;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.DataExport
{
  public class PackagingReferenceExporter : ReferenceExporterBase
  {
    public PackagingReferenceExporter(
      IVssRequestContext requestContext,
      JobParameters runParameters)
      : base(requestContext, runParameters)
    {
    }

    protected override string TraceMethod { get; } = "PackagingReferenceExporter.Export";

    protected override string LogFolder { get; } = "PackagingReferenceExport";

    protected override string RegistryFolder { get; } = "Packaging";

    internal override string ExportFeatureName { get; } = "BlobStore.Features.DataExport.Packaging";

    protected override void ProcessFileIdReference(IdBlobReferenceRow idBlobReferenceRow)
    {
      if (!idBlobReferenceRow.ReferenceId.StartsWith("feed/"))
        return;
      string[] strArray = idBlobReferenceRow.ReferenceId.Split('/');
      if (strArray.Length < 2)
      {
        this.tracer.TraceError(string.Format("Invalid feed reference found. BlobId: {0}, ReferenceId: {1}", (object) idBlobReferenceRow.BlobId, (object) idBlobReferenceRow.ReferenceId));
      }
      else
      {
        string feedId = strArray[1];
        int startIndex = "feed".Length + feedId.Length + 2;
        string referenceIdDetails = idBlobReferenceRow.ReferenceId.Substring(startIndex);
        this.AppendLog(new ExportFeedInfo(idBlobReferenceRow.BlobId.ValueString, feedId, referenceIdDetails, new DateTimeOffset?(idBlobReferenceRow.LastModified)).ToString());
      }
    }

    protected override void ProcessChunkReference(DedupMetadataEntry dedupMetadataEntry)
    {
      if (!dedupMetadataEntry.ReferenceId.StartsWith("feed/"))
        return;
      string[] strArray = dedupMetadataEntry.ReferenceId.Split('/');
      if (strArray.Length < 2)
      {
        this.tracer.TraceError(string.Format("Invalid feed reference found. BlobId: {0}, ReferenceId: {1}", (object) dedupMetadataEntry.DedupId, (object) dedupMetadataEntry.ReferenceId));
      }
      else
      {
        string str1 = strArray[1];
        int startIndex = "feed".Length + str1.Length + 2;
        string str2 = dedupMetadataEntry.ReferenceId.Substring(startIndex);
        string valueString = dedupMetadataEntry.DedupId.ValueString;
        string feedId = str1;
        string referenceIdDetails = str2;
        DateTimeOffset? stateChangeTime = dedupMetadataEntry.StateChangeTime;
        ref DateTimeOffset? local = ref stateChangeTime;
        DateTime? nullable = local.HasValue ? new DateTime?(local.GetValueOrDefault().UtcDateTime) : new DateTime?();
        DateTimeOffset? modifiedTime = nullable.HasValue ? new DateTimeOffset?((DateTimeOffset) nullable.GetValueOrDefault()) : new DateTimeOffset?();
        this.AppendLog(new ExportFeedInfo(valueString, feedId, referenceIdDetails, modifiedTime).ToString());
      }
    }
  }
}
