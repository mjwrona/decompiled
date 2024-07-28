// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PackageDownload.NuGetDownloadV2UriBuilderFactory
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework.Facades.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.NuGet.Server.PackageDownload
{
  public class NuGetDownloadV2UriBuilderFactory : 
    IFactory<IHandler<PackageIdentityBatchRequest, IDictionary<IPackageIdentity, Uri>>>
  {
    private readonly string sourceProtocolVersion;
    private readonly ILocationFacade locationService;

    public NuGetDownloadV2UriBuilderFactory(
      string sourceProtocolVersion,
      ILocationFacade locationService)
    {
      this.sourceProtocolVersion = sourceProtocolVersion;
      this.locationService = locationService;
    }

    public IHandler<PackageIdentityBatchRequest, IDictionary<IPackageIdentity, Uri>> Get() => !this.sourceProtocolVersion.Equals("v2") ? (IHandler<PackageIdentityBatchRequest, IDictionary<IPackageIdentity, Uri>>) null : (IHandler<PackageIdentityBatchRequest, IDictionary<IPackageIdentity, Uri>>) new NuGetDownloadV2UriCalculator(this.locationService);
  }
}
