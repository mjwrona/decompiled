// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.NuGetGetPackageIndexRequest`1
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class NuGetGetPackageIndexRequest<TRequest> : FeedRequest where TRequest : IFeedRequest
  {
    public NuGetGetPackageIndexRequest(
      TRequest packageRequest,
      int? pageSize,
      int? spillToPagesThreshold,
      bool includeSemVer2Versions)
      : base((IFeedRequest) packageRequest)
    {
      this.PackageRequest = packageRequest;
      this.PageSize = pageSize;
      this.SpillToPagesThreshold = spillToPagesThreshold;
      this.IncludeSemVer2Versions = includeSemVer2Versions;
    }

    public TRequest PackageRequest { get; }

    public int? PageSize { get; }

    public int? SpillToPagesThreshold { get; }

    public bool IncludeSemVer2Versions { get; }
  }
}
