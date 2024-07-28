// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Ingestion.MavenStreamStorablePackageInfo
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.VisualStudio.Services.Maven.Server.Ingestion
{
  public class MavenStreamStorablePackageInfo : 
    IStorablePackageInfo<MavenPackageIdentity, MavenPackageFileInfo>,
    IStorablePackageInfo<MavenPackageIdentity>,
    IPackageRequest<MavenPackageIdentity>,
    IPackageRequest,
    IFeedRequest,
    IProtocolAgnosticFeedRequest,
    IPackageFileRequest<MavenPackageIdentity>,
    IPackageFileRequest,
    IContentStreamStorable
  {
    public MavenStreamStorablePackageInfo(
      FeedCore feed,
      MavenPackageIdentity packageId,
      MavenPackageFileInfo protocolSpecificInfo,
      long packageSize)
    {
      this.Feed = feed;
      this.PackageId = packageId;
      this.ProtocolSpecificInfo = protocolSpecificInfo;
      this.PackageSize = packageSize;
    }

    public FeedCore Feed { get; }

    public IProtocol Protocol => this.PackageId.Name.Protocol;

    public MavenPackageIdentity PackageId { get; }

    public MavenPackageFileInfo ProtocolSpecificInfo { get; }

    public long PackageSize { get; }

    IStorageId IStorablePackageInfo<MavenPackageIdentity>.PackageStorageId => (IStorageId) this.PackageStorageId;

    public BlobStorageId PackageStorageId { get; set; }

    public IEnumerable<UpstreamSourceInfo> SourceChain { get; set; }

    public IngestionDirection IngestionDirection => PackageIngestionUtils.GetIngestionDirection(this.SourceChain);

    public string FilePath => this.ProtocolSpecificInfo.FilePath.FileName;

    public Stream ContentStream => this.ProtocolSpecificInfo.Stream;

    string IProtocolAgnosticFeedRequest.UserSuppliedFeedNameOrId => (string) null;

    string IProtocolAgnosticFeedRequest.UserSuppliedProjectNameOrId => (string) null;

    IPackageIdentity IPackageRequest.PackageId => (IPackageIdentity) this.PackageId;
  }
}
