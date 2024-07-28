// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.SimpleGetPackage.RedirectNonNormalizedSimpleRequestsHandler
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework.Facades.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.PyPi.WebApi;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.PyPi.Server.SimpleGetPackage
{
  public class RedirectNonNormalizedSimpleRequestsHandler : 
    IAsyncHandler<(RawPackageNameRequest Raw, PackageNameRequest<PyPiPackageName> Resolved), HttpResponseMessage>,
    IHaveInputType<(RawPackageNameRequest Raw, PackageNameRequest<PyPiPackageName> Resolved)>,
    IHaveOutputType<HttpResponseMessage>
  {
    private readonly ILocationFacade locationServiceFacade;

    public RedirectNonNormalizedSimpleRequestsHandler(ILocationFacade locationServiceFacade) => this.locationServiceFacade = locationServiceFacade;

    public Task<HttpResponseMessage> Handle(
      (RawPackageNameRequest Raw, PackageNameRequest<PyPiPackageName> Resolved) request)
    {
      if (request.Raw.PackageName.Equals(request.Resolved.PackageName.NormalizedName, StringComparison.Ordinal))
        return Task.FromResult<HttpResponseMessage>((HttpResponseMessage) null);
      Guid projectId = request.Resolved.Feed.Project == (ProjectReference) null ? Guid.Empty : request.Resolved.Feed.Project.Id;
      Uri uri = this.locationServiceFacade.GetResourceUri("pypi", ResourceIds.PyPiSimplePackageNameLocationId, projectId, (object) new
      {
        feedId = request.Resolved.Feed.FullyQualifiedId,
        packageName = request.Resolved.PackageName.NormalizedName
      }).EnsurePathEndsInSlash();
      return Task.FromResult<HttpResponseMessage>(new HttpResponseMessage(HttpStatusCode.MovedPermanently)
      {
        Headers = {
          Location = uri
        }
      });
    }
  }
}
