// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackagingApiController`4
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public abstract class PackagingApiController<TPackageName, TPackageVersion, TPackageIdentity, TPackageFileName> : 
    PackagingApiController
    where TPackageName : class, IPackageName
    where TPackageVersion : class, IPackageVersion
    where TPackageIdentity : class, IPackageIdentity<TPackageName, TPackageVersion>
    where TPackageFileName : class, IPackageFileName
  {
    protected abstract IIdentityResolver<TPackageName, TPackageVersion, TPackageIdentity, TPackageFileName> IdentityResolver { get; }

    protected IPackageNameRequest<TPackageName> GetPackageNameRequest(
      string feedNameOrId,
      string packageName,
      IValidator<FeedCore>? feedValidator = null)
    {
      IPackageNameRequest<TPackageName> feedRequest = this.GetFeedRequest(feedNameOrId, feedValidator).WithPackageName<TPackageName>(this.IdentityResolver.ResolvePackageName(packageName));
      new PackagingTracesBasicInfo(this.TfsRequestContext).SetFromFeedRequest((IProtocolAgnosticFeedRequest) feedRequest);
      return feedRequest;
    }

    protected IPackageRequest<TPackageIdentity> GetPackageRequest(
      string feedNameOrId,
      string packageName,
      string packageVersion,
      IValidator<FeedCore>? feedValidator = null)
    {
      IPackageRequest<TPackageIdentity> feedRequest = this.GetFeedRequest(feedNameOrId, feedValidator).WithPackage<TPackageIdentity>(this.IdentityResolver.ResolvePackageIdentity(packageName, packageVersion));
      new PackagingTracesBasicInfo(this.TfsRequestContext).SetFromFeedRequest((IProtocolAgnosticFeedRequest) feedRequest);
      return feedRequest;
    }

    protected IPackageFileRequest<TPackageIdentity> GetPackageFileRequest(
      string feedNameOrId,
      string packageName,
      string packageVersion,
      string filePath,
      IValidator<FeedCore>? feedValidator = null)
    {
      IPackageFileRequest<TPackageIdentity> feedRequest = this.GetFeedRequest(feedNameOrId, feedValidator).WithPackage<TPackageIdentity>(this.IdentityResolver.ResolvePackageIdentity(packageName, packageVersion)).WithFile<TPackageIdentity>(filePath);
      new PackagingTracesBasicInfo(this.TfsRequestContext).SetFromFeedRequest((IProtocolAgnosticFeedRequest) feedRequest);
      return feedRequest;
    }

    protected IPackageInnerFileRequest<TPackageIdentity> GetPackageInnerFileRequest(
      string feedNameOrId,
      string packageName,
      string packageVersion,
      string filePath,
      string innerFilePath,
      IValidator<FeedCore>? feedValidator = null)
    {
      IPackageInnerFileRequest<TPackageIdentity> feedRequest = this.GetFeedRequest(feedNameOrId, feedValidator).WithPackage<TPackageIdentity>(this.IdentityResolver.ResolvePackageIdentity(packageName, packageVersion)).WithFile<TPackageIdentity>(filePath).WithInnerFile<TPackageIdentity>(innerFilePath);
      new PackagingTracesBasicInfo(this.TfsRequestContext).SetFromFeedRequest((IProtocolAgnosticFeedRequest) feedRequest);
      return feedRequest;
    }
  }
}
