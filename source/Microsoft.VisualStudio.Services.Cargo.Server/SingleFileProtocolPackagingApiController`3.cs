// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.SingleFileProtocolPackagingApiController`3
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server
{
  public abstract class SingleFileProtocolPackagingApiController<TPackageName, TPackageVersion, TPackageIdentity> : 
    PackagingApiController<TPackageName, TPackageVersion, TPackageIdentity, SimplePackageFileName>
    where TPackageName : class, IPackageName
    where TPackageVersion : class, IPackageVersion
    where TPackageIdentity : class, IPackageIdentity<TPackageName, TPackageVersion>, ISingleFileProtocolPackageIdentity
  {
    protected IPackageFileRequest<TPackageIdentity> GetPackageFileRequest(
      string feedNameOrId,
      string packageName,
      string packageVersion,
      IValidator<FeedCore>? feedValidator = null)
    {
      TPackageIdentity packageIdentity = this.IdentityResolver.ResolvePackageIdentity(packageName, packageVersion);
      IPackageFileRequest<TPackageIdentity> feedRequest = this.GetFeedRequest(feedNameOrId, feedValidator).WithPackage<TPackageIdentity>(packageIdentity).WithFile<TPackageIdentity>(packageIdentity.GetCanonicalFileName());
      new PackagingTracesBasicInfo(this.TfsRequestContext).SetFromFeedRequest((IProtocolAgnosticFeedRequest) feedRequest);
      return feedRequest;
    }

    protected IPackageInnerFileRequest<TPackageIdentity> GetPackageInnerFileRequest(
      string feedNameOrId,
      string packageName,
      string packageVersion,
      string innerFilePath,
      IValidator<FeedCore>? feedValidator = null)
    {
      TPackageIdentity packageIdentity = this.IdentityResolver.ResolvePackageIdentity(packageName, packageVersion);
      IPackageInnerFileRequest<TPackageIdentity> feedRequest = this.GetFeedRequest(feedNameOrId, feedValidator).WithPackage<TPackageIdentity>(packageIdentity).WithFile<TPackageIdentity>(packageIdentity.GetCanonicalFileName()).WithInnerFile<TPackageIdentity>(innerFilePath);
      new PackagingTracesBasicInfo(this.TfsRequestContext).SetFromFeedRequest((IProtocolAgnosticFeedRequest) feedRequest);
      return feedRequest;
    }
  }
}
