// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.Controllers.ServiceEndpointShareController
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server.Controllers
{
  [ControllerApiVersion(5.0)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "serviceendpoint", ResourceName = "share", ResourceVersion = 1)]
  public class ServiceEndpointShareController : ServiceEndpointsProjectApiController
  {
    [HttpPost]
    public void ShareEndpointWithProject(Guid endpointId, string fromProject, string withProject) => this.TfsRequestContext.GetService<ServiceEndpointShareService>().ShareEndpointWithProject(this.TfsRequestContext, endpointId, fromProject, withProject);

    [HttpGet]
    public List<ProjectReference> QuerySharedProjects(Guid endpointId, string project) => this.TfsRequestContext.GetService<ServiceEndpointShareService>().QuerySharedProjects(this.TfsRequestContext, endpointId, project);
  }
}
