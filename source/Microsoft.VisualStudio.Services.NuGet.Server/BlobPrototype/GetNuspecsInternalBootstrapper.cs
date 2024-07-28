// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.GetNuspecsInternalBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class GetNuspecsInternalBootstrapper : 
    IBootstrapper<IAsyncHandler<NuGetGetNuspecsRequest, IReadOnlyDictionary<VssNuGetPackageVersion, ContentBytes>>>
  {
    public GetNuspecsInternalBootstrapper(IVssRequestContext requestContext)
    {
      // ISSUE: reference to a compiler-generated field
      this.\u003CrequestContext\u003EP = requestContext;
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }

    public IAsyncHandler<NuGetGetNuspecsRequest, IReadOnlyDictionary<VssNuGetPackageVersion, ContentBytes>> Bootstrap() => NuGetAggregationResolver.Bootstrap(this.\u003CrequestContext\u003EP).HandlerFor<NuGetGetNuspecsRequest, IReadOnlyDictionary<VssNuGetPackageVersion, ContentBytes>, INuGetMetadataService>((Func<INuGetMetadataService, IAsyncHandler<NuGetGetNuspecsRequest, IReadOnlyDictionary<VssNuGetPackageVersion, ContentBytes>>>) (agg => new NuGetGetPackageNuspecsHandler((IReadMetadataService<VssNuGetPackageIdentity, INuGetMetadataEntry>) agg).AsIAsyncHandler<NuGetGetNuspecsRequest, IReadOnlyDictionary<VssNuGetPackageVersion, ContentBytes>>()));
  }
}
