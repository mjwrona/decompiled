// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Controllers.Api.NuGetPackageDropController
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Drop.WebApi;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.NuGet.Server.Exceptions;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.Utils;
using Microsoft.VisualStudio.Services.NuGet.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Controllers.Api
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "nuget", ResourceName = "packageDrops")]
  public class NuGetPackageDropController : NuGetApiController
  {
    [HttpPost]
    [ClientIgnore]
    [FeatureEnabled("NuGet.Service.EnableNuGetLargePackages")]
    public async Task<DropInfo> CreatePackageDropAsync(
      string feedId,
      string packageId = null,
      string packageVersion = null)
    {
      NuGetPackageDropController packageDropController = this;
      FeedCore feed = packageDropController.GetFeedRequest(feedId).Feed;
      FeedSecurityHelper.CheckAddPackagePermissions(packageDropController.TfsRequestContext, feed);
      await packageDropController.ThrowIfPackageExistsAsync(feed, packageId, packageVersion);
      DropHttpClient client = packageDropController.TfsRequestContext.Elevate().GetClient<DropHttpClient>();
      string dropName = Guid.NewGuid().ToString();
      DropItem dropItem = new DropItem()
      {
        Name = dropName ?? "",
        FinalizedStatus = (DropCommitStatus) 0,
        ExpirationTime = new DateTime?()
      };
      HttpResponseMessage dropAsync;
      try
      {
        dropAsync = await client.CreateDropAsync(dropItem, packageDropController.TfsRequestContext.CancellationToken);
      }
      catch (Exception ex)
      {
        throw new FailedToCreateDropException(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_FailedToCreateDrop(), ex);
      }
      if (!dropAsync.IsSuccessStatusCode)
        throw new FailedToCreateDropException(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_FailedToCreateDropWithStatusCode((object) dropAsync.StatusCode));
      DropInfo packageDropAsync = new DropInfo()
      {
        Name = dropName
      };
      dropName = (string) null;
      return packageDropAsync;
    }

    private async Task ThrowIfPackageExistsAsync(
      FeedCore feed,
      string packageId,
      string packageVersion)
    {
      NuGetPackageDropController packageDropController = this;
      VssNuGetPackageIdentity packageIdentity;
      if (packageId == null)
        packageIdentity = (VssNuGetPackageIdentity) null;
      else if (packageVersion == null)
      {
        packageIdentity = (VssNuGetPackageIdentity) null;
      }
      else
      {
        packageIdentity = NuGetPackageRetrievalValidationUtils.ValidateAndParsePackageIdentity(packageId, packageVersion);
        packageDropController.TfsRequestContext.SetPackageIdentityForPackagingTraces((IPackageIdentity) packageIdentity);
        if (await NuGetServerUtils.GetStateFor(packageDropController.TfsRequestContext, feed, packageIdentity.Name, packageIdentity.Version) != null)
          throw new PackageAlreadyExistsException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_FeedAlreadyContainsPackage((object) packageIdentity));
        packageIdentity = (VssNuGetPackageIdentity) null;
      }
    }
  }
}
