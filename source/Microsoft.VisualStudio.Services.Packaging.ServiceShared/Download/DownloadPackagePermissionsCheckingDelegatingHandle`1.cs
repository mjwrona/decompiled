// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download.DownloadPackagePermissionsCheckingDelegatingHandler`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;
using System.Net;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download
{
  public class DownloadPackagePermissionsCheckingDelegatingHandler<TPackageId> : 
    IAsyncHandler<IPackageFileRequest<TPackageId, IStorageId>, ContentResult>,
    IHaveInputType<IPackageFileRequest<TPackageId, IStorageId>>,
    IHaveOutputType<ContentResult>
    where TPackageId : IPackageIdentity
  {
    private readonly IAsyncHandler<IPackageFileRequest<TPackageId, IStorageId>, ContentResult> innerHandler;
    private readonly IFeedPerms feedPerms;
    private readonly IExecutionEnvironment executionEnvironment;
    private readonly IOrgLevelPackagingSetting<bool> allowUpstreamsForPublicFeedsSettings;
    private readonly IOrgLevelPackagingSetting<bool> allowUpstreamsForPublicFeedsMSFTSettings;

    public DownloadPackagePermissionsCheckingDelegatingHandler(
      IAsyncHandler<IPackageFileRequest<TPackageId, IStorageId>, ContentResult> innerHandler,
      IFeedPerms feedPerms,
      IExecutionEnvironment executionEnvironment,
      IOrgLevelPackagingSetting<bool> allowUpstreamsForPublicFeedsSettings,
      IOrgLevelPackagingSetting<bool> allowUpstreamsForPublicFeedsMSFTSettings)
    {
      this.innerHandler = innerHandler;
      this.feedPerms = feedPerms;
      this.executionEnvironment = executionEnvironment;
      this.allowUpstreamsForPublicFeedsSettings = allowUpstreamsForPublicFeedsSettings;
      this.allowUpstreamsForPublicFeedsMSFTSettings = allowUpstreamsForPublicFeedsMSFTSettings;
    }

    public Task<ContentResult> Handle(
      IPackageFileRequest<TPackageId, IStorageId> request)
    {
      if (!this.allowUpstreamsForPublicFeedsSettings.Get() && (!this.allowUpstreamsForPublicFeedsMSFTSettings.Get() || !this.executionEnvironment.IsCollectionInMicrosoftTenant()) || this.feedPerms.HasPermissions(request.Feed, FeedPermissionConstants.AddUpstreamPackage))
        return this.innerHandler.Handle(request);
      if (this.executionEnvironment.IsUnauthenticatedWebRequest())
      {
        TPackageId packageId = request.PackageId;
        string stringForMessages = packageId.DisplayStringForMessages;
        packageId = request.PackageId;
        IPackageVersion version = packageId.Version;
        throw new UnauthorizedRequestException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_AuthForVersionIngestion((object) stringForMessages, (object) version), HttpStatusCode.Unauthorized);
      }
      throw Microsoft.VisualStudio.Services.Packaging.ServiceShared.ExceptionHelper.PackageNotFound(request.Feed, (IPackageIdentity) request.PackageId, request.FilePath);
    }
  }
}
