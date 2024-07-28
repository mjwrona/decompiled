// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.EnvironmentsController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Server.Converters;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(5.0)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "environments")]
  public sealed class EnvironmentsController : DistributedTaskProjectApiController
  {
    [HttpPost]
    public EnvironmentInstance AddEnvironment(
      [FromBody] EnvironmentCreateParameter environmentCreateParameter)
    {
      ArgumentUtility.CheckForNull<EnvironmentCreateParameter>(environmentCreateParameter, nameof (environmentCreateParameter));
      return this.TfsRequestContext.GetService<IEnvironmentService>().AddEnvironment(this.TfsRequestContext, this.ProjectId, environmentCreateParameter.ToEnvironment());
    }

    [HttpGet]
    public EnvironmentInstance GetEnvironmentById(int environmentId, [ClientQueryParameter] EnvironmentExpands expands = EnvironmentExpands.None)
    {
      bool includeResourceReferences = (expands & EnvironmentExpands.ResourceReferences) == EnvironmentExpands.ResourceReferences;
      return this.TfsRequestContext.GetService<IEnvironmentService>().GetEnvironmentById(this.TfsRequestContext, this.ProjectId, environmentId, includeResourceReferences: includeResourceReferences);
    }

    [HttpGet]
    [ClientResponseType(typeof (IPagedList<EnvironmentInstance>), null, null)]
    public HttpResponseMessage GetEnvironments([ClientQueryParameter] string name = null, [ClientQueryParameter] string continuationToken = "", [FromUri(Name = "$top")] int top = 50)
    {
      if (top < 0 || top > 1000)
        top = 1000;
      IPagedList<EnvironmentInstance> environments = this.TfsRequestContext.GetService<IEnvironmentService>().GetEnvironments(this.TfsRequestContext, this.ProjectId, name, continuationToken, maxEnvironmentsCount: top);
      HttpResponseMessage response = this.Request.CreateResponse<IPagedList<EnvironmentInstance>>(HttpStatusCode.OK, environments);
      if (!string.IsNullOrWhiteSpace(environments.ContinuationToken))
        DistributedTaskProjectApiController.SetContinuationToken(response, environments.ContinuationToken);
      return response;
    }

    [HttpPatch]
    public EnvironmentInstance UpdateEnvironment(
      int environmentId,
      EnvironmentUpdateParameter environmentUpdateParameter)
    {
      ArgumentUtility.CheckForNull<EnvironmentUpdateParameter>(environmentUpdateParameter, nameof (environmentUpdateParameter));
      return this.TfsRequestContext.GetService<IEnvironmentService>().UpdateEnvironment(this.TfsRequestContext, this.ProjectId, environmentId, environmentUpdateParameter.ToEnvironment());
    }

    [HttpDelete]
    public void DeleteEnvironment(int environmentId) => this.TfsRequestContext.GetService<IEnvironmentService>().DeleteEnvironment(this.TfsRequestContext, this.ProjectId, environmentId);
  }
}
