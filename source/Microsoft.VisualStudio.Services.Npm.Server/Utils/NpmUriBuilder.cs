// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Utils.NpmUriBuilder
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Npm.Server.Constants;
using Microsoft.VisualStudio.Services.Npm.Server.Exceptions;
using Microsoft.VisualStudio.Services.Npm.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobStore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework.Facades.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Location;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Npm.Server.Utils
{
  public static class NpmUriBuilder
  {
    public static Uri GetPackageDownloadRedirectUri(
      IVssRequestContext requestContext,
      IPackageNameRequest<NpmPackageName> packageNameRequest,
      string packageVersion)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IPackageNameRequest<NpmPackageName>>(packageNameRequest, nameof (packageNameRequest));
      ArgumentUtility.CheckForNull<NpmPackageName>(packageNameRequest.PackageName, "PackageName");
      ArgumentUtility.CheckStringForNullOrEmpty(packageVersion, nameof (packageVersion));
      NpmPackageName packageName = packageNameRequest.PackageName;
      string str = packageName.UnscopedName + "-" + packageVersion + ".tgz";
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, NpmTracePoints.NpmUriBuilder.TraceData, 12000300, nameof (GetPackageDownloadRedirectUri)))
      {
        ILocationFacade locationFacade = requestContext.GetLocationFacade();
        return packageName.IsScoped ? locationFacade.GetResourceUri("npm", ResourceIds.DownloadScopedPackageResourceId, (IFeedRequest) packageNameRequest, PackagingUriNamePreference.PreferUserSuppliedNameOrId, (object) new
        {
          packageScope = packageName.Scope,
          unscopedPackageName = packageName.UnscopedName,
          packageFileName = str
        }) : locationFacade.GetResourceUri("npm", ResourceIds.DownloadPackageResourceId, (IFeedRequest) packageNameRequest, PackagingUriNamePreference.PreferUserSuppliedNameOrId, (object) new
        {
          packageName = packageName.UnscopedName,
          packageFileName = str
        });
      }
    }

    public static async Task<Uri> GetDirectPackageContentUriAsync(
      IVssRequestContext requestContext,
      Guid feedId,
      BlobIdentifier blobId,
      string packageName,
      string packageVersion)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(feedId, nameof (feedId));
      ArgumentUtility.CheckForNull<BlobIdentifier>(blobId, nameof (blobId));
      ArgumentUtility.CheckStringForNullOrEmpty(packageName, nameof (packageName));
      ArgumentUtility.CheckStringForNullOrEmpty(packageVersion, nameof (packageVersion));
      Uri blobContentUriAsync;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, NpmTracePoints.NpmUriBuilder.TraceData, 12000310, nameof (GetDirectPackageContentUriAsync)))
        blobContentUriAsync = await NpmUriBuilder.GetDirectBlobContentUriAsync(requestContext, feedId, blobId, packageName + "-" + packageVersion + ".tgz");
      return blobContentUriAsync;
    }

    public static async Task<Uri> GetDirectBlobContentUriAsync(
      IVssRequestContext requestContext,
      Guid feedId,
      BlobIdentifier blobId,
      string blobName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(feedId, nameof (feedId));
      ArgumentUtility.CheckForNull<BlobIdentifier>(blobId, nameof (blobId));
      DateTimeOffset expiry = new SASTokenTTL().GetExpiry(requestContext);
      BlobIdWithHeaders blobId1 = new BlobIdWithHeaders(blobId, EdgeCache.NotAllowed, blobName?.Replace('+', '-'), expiryTime: new DateTimeOffset?(expiry));
      IElevatedBlobStore service = requestContext.GetService<IElevatedBlobStore>();
      Uri notNullUri;
      try
      {
        notNullUri = (await service.GetDownloadUriAsync(requestContext, WellKnownDomainIds.OriginalDomainId, blobId1)).NotNullUri;
      }
      catch (Exception ex)
      {
        throw new GetDownloadUriFailedException(blobId, blobName, ex);
      }
      return notNullUri;
    }
  }
}
