// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Utils.NuGetUriBuilder
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.Constants;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageDownload;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework.Facades.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Utils
{
  public static class NuGetUriBuilder
  {
    public static Uri GetPackageVersionNewRegistrationUri(
      IVssRequestContext requestContext,
      string feedId,
      VssNuGetPackageIdentity packageIdentity)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, NuGetTracePoints.NuGetUriBuilder.TraceData, 5722110, nameof (GetPackageVersionNewRegistrationUri)))
        return requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "nuget", ResourceIds.Registrations2PackageVersionResourceId, (object) new
        {
          feedId = feedId,
          packageId = packageIdentity.Name.NormalizedName,
          packageVersion = packageIdentity.Version.NormalizedVersion
        });
    }

    public static Uri GetCommitLogEntryUri(
      IVssRequestContext requestContext,
      string feedId,
      PackagingCommitId packagingCommitId)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, NuGetTracePoints.NuGetUriBuilder.TraceData, 5722120, nameof (GetCommitLogEntryUri)))
        return requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "nuget", ResourceIds.CommitLogResourceId, (object) new
        {
          feedId = feedId,
          commitId = packagingCommitId
        });
    }

    public static IDictionary<IPackageIdentity, Uri> GetPackageContentRedirectSourceUris(
      IVssRequestContext requestContext,
      PackageIdentityBatchRequest request,
      string sourceProtocolVersion)
    {
      ILocationFacade locationFacade = requestContext.GetLocationFacade();
      return new ChooseFirstNonNullFactory<IHandler<PackageIdentityBatchRequest, IDictionary<IPackageIdentity, Uri>>>(new IFactory<IHandler<PackageIdentityBatchRequest, IDictionary<IPackageIdentity, Uri>>>[2]
      {
        (IFactory<IHandler<PackageIdentityBatchRequest, IDictionary<IPackageIdentity, Uri>>>) new NuGetDownloadV2UriBuilderFactory(sourceProtocolVersion, locationFacade),
        (IFactory<IHandler<PackageIdentityBatchRequest, IDictionary<IPackageIdentity, Uri>>>) new ReturnSameInstanceFactory<IHandler<PackageIdentityBatchRequest, IDictionary<IPackageIdentity, Uri>>>((IHandler<PackageIdentityBatchRequest, IDictionary<IPackageIdentity, Uri>>) new NuGetDownloadV3UriCalculator(locationFacade))
      }).Get().Handle(request);
    }
  }
}
