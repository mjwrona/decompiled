// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.SimpleGetPackage.PyPiDownloadUriCalculator
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework.Facades.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.PyPi.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.PyPi.Server.SimpleGetPackage
{
  public class PyPiDownloadUriCalculator : 
    IAsyncHandler<BatchPackageFileRequest<PyPiPackageIdentity>, IDictionary<IPackageFileRequest<PyPiPackageIdentity>, Uri>>,
    IHaveInputType<BatchPackageFileRequest<PyPiPackageIdentity>>,
    IHaveOutputType<IDictionary<IPackageFileRequest<PyPiPackageIdentity>, Uri>>
  {
    private readonly ILocationFacade locationService;

    public PyPiDownloadUriCalculator(ILocationFacade locationService) => this.locationService = locationService;

    public Task<IDictionary<IPackageFileRequest<PyPiPackageIdentity>, Uri>> Handle(
      BatchPackageFileRequest<PyPiPackageIdentity> request)
    {
      string str1 = "{0}";
      string str2 = "{1}";
      string str3 = "{2}";
      string fullyQualifiedId = request.Feed.FullyQualifiedId;
      Guid projectIdOrEmptyGuid = request.Feed.Project.ToProjectIdOrEmptyGuid();
      string templateUriString = this.locationService.GetResourceUri("pypi", ResourceIds.PyPiDownloadLocationId, projectIdOrEmptyGuid, (object) new
      {
        feedId = fullyQualifiedId,
        packageName = str3,
        packageVersion = str2,
        fileName = str1
      }).OriginalString;
      return Task.FromResult<IDictionary<IPackageFileRequest<PyPiPackageIdentity>, Uri>>((IDictionary<IPackageFileRequest<PyPiPackageIdentity>, Uri>) request.Requests.ToDictionary<IPackageFileRequest<PyPiPackageIdentity>, IPackageFileRequest<PyPiPackageIdentity>, Uri>((Func<IPackageFileRequest<PyPiPackageIdentity>, IPackageFileRequest<PyPiPackageIdentity>>) (r => r), (Func<IPackageFileRequest<PyPiPackageIdentity>, Uri>) (r => new Uri(string.Format(templateUriString, (object) r.FilePath, (object) r.PackageId.Version.NormalizedVersion, (object) r.PackageId.Name.NormalizedName)))));
    }
  }
}
