// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.RegistrationsPackageFrameCreator
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework.Facades.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Location;
using Newtonsoft.Json.Linq;
using NuGet.Versioning;
using System;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class RegistrationsPackageFrameCreator : IRegistrationsPackageFrameCreator
  {
    private readonly ILocationFacade locationFacade;
    private readonly ITracerService tracerService;

    public RegistrationsPackageFrameCreator(
      ILocationFacade locationFacade,
      ITracerService tracerService)
    {
      this.locationFacade = locationFacade;
      this.tracerService = tracerService;
    }

    public JObject GetPackagePageFrame(
      IPackageNameRequest<VssNuGetPackageName> packageNameRequest,
      VssNuGetPackageVersion lowerVersion,
      VssNuGetPackageVersion upperVersion,
      Uri registrationUri)
    {
      using (this.tracerService.Enter((object) this, nameof (GetPackagePageFrame)))
        return new JObject(new object[5]
        {
          (object) new JProperty("@id", (object) this.GetPackagePageNewRegistrationUri(packageNameRequest, lowerVersion.NuGetVersion, upperVersion.NuGetVersion)),
          (object) new JProperty("@type", (object) "catalog:CatalogPage"),
          (object) new JProperty("parent", (object) registrationUri.AbsoluteUri),
          (object) new JProperty("lower", (object) lowerVersion.NuGetVersion.ToNormalizedString()),
          (object) new JProperty("upper", (object) upperVersion.NuGetVersion.ToNormalizedString())
        });
    }

    private Uri GetPackagePageNewRegistrationUri(
      IPackageNameRequest<VssNuGetPackageName> packageNameRequest,
      NuGetVersion lowerVersion,
      NuGetVersion upperVersion)
    {
      return this.locationFacade.GetResourceUri("nuget", ResourceIds.Registrations2PackagePageResourceId, (IFeedRequest) packageNameRequest, PackagingUriNamePreference.PreferCanonicalId, (object) new
      {
        packageId = packageNameRequest.PackageName.NormalizedName,
        lower = lowerVersion,
        upper = upperVersion
      });
    }
  }
}
