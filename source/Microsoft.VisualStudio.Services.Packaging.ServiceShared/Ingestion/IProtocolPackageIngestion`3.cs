// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.IProtocolPackageIngestion`3
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System.Collections.Generic;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion
{
  public interface IProtocolPackageIngestion<in TInput, TParsed, TMetadataEntry> where TMetadataEntry : class, IMetadataEntry
  {
    Task CheckInputContentSize(
      IFeedRequest feedRequest,
      TInput input,
      IngestionDirection ingestionDirection);

    Task<TParsed> Parse(TInput input, IngestionDirection ingestionDirection);

    Task Validate(IFeedRequest feedRequest, TParsed parsed);

    Task Validate(
      IFeedRequest feedRequest,
      TParsed parsed,
      MetadataDocument<TMetadataEntry>? metadataDoc,
      TMetadataEntry? existingEntryForVersionBeingIngested);

    Task CommitStorage(IFeedRequest feedRequest, TParsed parsed);

    IAddOperationData PrepareAddOperation(
      TParsed parsed,
      ProvenanceInfo provenanceInfo,
      IEnumerable<UpstreamSourceInfo> sourceChain);
  }
}
