// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PublicRepositories.NuGetCatalogCursorBasedUpstreamChangeProvider
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server.PublicRepositories
{
  public class NuGetCatalogCursorBasedUpstreamChangeProvider : 
    ICursorBasedUpstreamChangeProvider<NuGetCatalogCursor, VssNuGetPackageIdentity>
  {
    private readonly INuGetV3Client client;
    private readonly ITimeProvider timeProvider;

    public NuGetCatalogCursorBasedUpstreamChangeProvider(
      INuGetV3Client client,
      ITimeProvider timeProvider)
    {
      this.client = client;
      this.timeProvider = timeProvider;
    }

    public async Task<IReadOnlyList<ChangedPackage<NuGetCatalogCursor, VssNuGetPackageIdentity>>> GetChanges(
      NuGetCatalogCursor? since)
    {
      List<ChangedPackage<NuGetCatalogCursor, VssNuGetPackageIdentity>> changedPackages = new List<ChangedPackage<NuGetCatalogCursor, VssNuGetPackageIdentity>>();
      if ((object) since == null)
        since = new NuGetCatalogCursor((DateTimeOffset) this.timeProvider.Now.AddHours(-1.0));
      foreach (NuGetCatalogIndexPageReference pageRef in (await this.client.GetCatalogIndexAsync()).PageReferences.Where<NuGetCatalogIndexPageReference>((Func<NuGetCatalogIndexPageReference, bool>) (x => x.CursorPosition > since)))
        changedPackages.AddRange((await this.client.GetCatalogPageAsync(pageRef)).Items.Where<NuGetCatalogPageItem>((Func<NuGetCatalogPageItem, bool>) (x => x.CursorPosition > since)).Select<NuGetCatalogPageItem, ChangedPackage<NuGetCatalogCursor, VssNuGetPackageIdentity>>((Func<NuGetCatalogPageItem, ChangedPackage<NuGetCatalogCursor, VssNuGetPackageIdentity>>) (x => new ChangedPackage<NuGetCatalogCursor, VssNuGetPackageIdentity>(x.CursorPosition, x.CursorPosition.Value.UtcDateTime, NuGetIdentityResolver.Instance.ResolvePackageIdentity(x.PackageName, x.PackageVersion)))));
      IReadOnlyList<ChangedPackage<NuGetCatalogCursor, VssNuGetPackageIdentity>> changes = (IReadOnlyList<ChangedPackage<NuGetCatalogCursor, VssNuGetPackageIdentity>>) changedPackages;
      changedPackages = (List<ChangedPackage<NuGetCatalogCursor, VssNuGetPackageIdentity>>) null;
      return changes;
    }
  }
}
