// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.RawV2PackageRequest
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class RawV2PackageRequest : 
    FeedRequest,
    IRawPackageRequest,
    IRawPackageNameRequest,
    IFeedRequest,
    IProtocolAgnosticFeedRequest
  {
    public RawV2PackageRequest(
      IFeedRequest feedRequest,
      string packageName,
      string packageVersion,
      System.Web.Http.OData.Query.ODataQueryOptions<V2FeedPackage> oDataQueryOptions)
      : base(feedRequest)
    {
      this.PackageName = packageName;
      this.PackageVersion = packageVersion;
      this.ODataQueryOptions = oDataQueryOptions;
    }

    public string PackageName { get; }

    public string PackageVersion { get; }

    public System.Web.Http.OData.Query.ODataQueryOptions<V2FeedPackage> ODataQueryOptions { get; }
  }
}
