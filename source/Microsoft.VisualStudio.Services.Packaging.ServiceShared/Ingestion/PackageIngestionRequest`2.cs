// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.PackageIngestionRequest`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion
{
  public class PackageIngestionRequest<TPackageId, TContent> : FeedRequest where TPackageId : IPackageIdentity
  {
    public PackageIngestionRequest(
      IFeedRequest feedRequest,
      TContent packageContents,
      string protocolVersion)
      : base(feedRequest)
    {
      this.PackageContents = packageContents;
      this.ProtocolVersion = protocolVersion;
    }

    public string ProtocolVersion { get; }

    public TContent PackageContents { get; }

    public IEnumerable<UpstreamSourceInfo> SourceChain { get; set; }

    public TPackageId ExpectedIdentity { get; set; }
  }
}
