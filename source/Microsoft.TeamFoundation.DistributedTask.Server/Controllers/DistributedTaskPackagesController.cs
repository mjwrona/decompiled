// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.DistributedTaskPackagesController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "packages")]
  public class DistributedTaskPackagesController : DistributedTaskApiController
  {
    [HttpGet]
    public IList<TaskPackageMetadata> GetPackages() => this.ResourceService.GetPackages(this.TfsRequestContext);

    [HttpGet]
    [ClientResponseType(typeof (TaskPackageMetadata), null, null, MethodName = "GetPackage")]
    [ClientResponseType(typeof (Stream), null, null, MethodName = "GetPackageZip", MediaType = "application/zip")]
    public HttpResponseMessage GetPackage(string packageType)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(packageType, nameof (packageType));
      if (!this.ZipFileRequested())
        return this.Request.CreateResponse<TaskPackageMetadata>(HttpStatusCode.OK, this.ResourceService.GetPackage(this.TfsRequestContext, packageType));
      this.ResourceService.GetPackage(this.TfsRequestContext, packageType);
      throw new PackageNotFoundException(TaskResources.PackageNotFound((object) packageType));
    }
  }
}
