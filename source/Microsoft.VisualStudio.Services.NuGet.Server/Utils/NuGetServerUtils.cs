// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Utils.NuGetServerUtils
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Utils
{
  public class NuGetServerUtils : INuGetServerUtils
  {
    public static async Task<INuGetMetadataEntry> GetStateFor(
      IVssRequestContext requestContext,
      FeedCore feed,
      VssNuGetPackageName name,
      VssNuGetPackageVersion version)
    {
      return await new NuGetMetadataHandlerBootstrapper(requestContext).Bootstrap().Handle(new PackageRequest<VssNuGetPackageIdentity>(feed, new VssNuGetPackageIdentity(name, version)));
    }

    public IdBlobReference GetIdBlobReference(Guid feedId, VssNuGetPackageIdentity packageIdentity) => new IdBlobReference(this.GetPackageBlobReferenceId(feedId, packageIdentity), "nuget");

    public string GetPackageBlobReferenceId(Guid feedId, VssNuGetPackageIdentity package) => string.Format("feed/{0}/{1}.{2}{3}", (object) feedId, (object) package.Name.NormalizedName, (object) package.Version.NormalizedVersion, (object) ".nupkg");

    public IStorageId CreatePackageStorageId(string storageId)
    {
      try
      {
        return StorageId.Parse(storageId);
      }
      catch (Exception ex)
      {
        throw new VssInvalidDataException(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_InvalidStorageId((object) storageId), ex);
      }
    }
  }
}
