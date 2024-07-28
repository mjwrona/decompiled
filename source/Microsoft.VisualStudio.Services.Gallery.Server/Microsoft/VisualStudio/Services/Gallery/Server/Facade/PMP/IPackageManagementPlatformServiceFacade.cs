// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Facade.PMP.IPackageManagementPlatformServiceFacade
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Facade.PMP
{
  [DefaultServiceImplementation(typeof (PackageManagementPlatformServiceFacade))]
  public interface IPackageManagementPlatformServiceFacade : IVssFrameworkService
  {
    void UploadArtifactFile(
      IVssRequestContext requestContext,
      Stream artifactFile,
      ExtensionVersionToPublish extensionToPublish,
      string registryName,
      string artifactFileTypeMoniker);

    Task<HttpResponseMessage> UpdatePackageAggregateArchivedState(
      IVssRequestContext requestContext,
      string registryName,
      PublishedExtension extension,
      bool isPublished);

    Task<HttpResponseMessage> UpdatePackageAggregateLockState(
      IVssRequestContext requestContext,
      string registryName,
      PublishedExtension extension,
      bool isLocked);

    Task<HttpResponseMessage> DeletePackageAggregate(
      IVssRequestContext requestContext,
      string registryName,
      PublishedExtension extension);

    Task<HttpResponseMessage> DeletePackage(
      IVssRequestContext requestContext,
      string registryName,
      PublishedExtension extension,
      string version);
  }
}
